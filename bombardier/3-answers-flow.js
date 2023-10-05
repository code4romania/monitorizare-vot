import http from "k6/http";
import { check, fail } from "k6";
import { Trend } from 'k6/metrics';
import { SharedArray } from 'k6/data';
import { scenario } from 'k6/execution';

import { uuidv4, randomItem } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';
import papaparse from 'https://jslib.k6.io/papaparse/5.1.1/index.js';
import { getRandomAnswers } from './utils.js';
const BASE_URL = "https://localhost:5001";

const durationOnGetFormVersions = new Trend('durationOnGetFormVersions', true);
const durationOnGetFormData = new Trend('durationOnGetFormData', true);
const durationOnSubmitAnswers = new Trend('durationOnSubmitAnswers', true);

const users = new SharedArray('users', function () {
    const data = papaparse.parse(open('./users.csv'), { header: true }).data;
    return data;
});

const pollingStations = new SharedArray('polling-stations', function () {
    const data = papaparse.parse(open('./polling-stations.csv'), { header: true }).data;
    return data;
});


export const options = {
    // Key configurations for avg load test in this section
    stages: [
        { duration: '5m', target: 100 }, // traffic ramp-up from 1 to 100 users over 5 minutes.
        { duration: '5m', target: 100 }, // stay at 100 users for 30 minutes
        { duration: '5m', target: 0 }, // ramp-down to 0 users
    ],
};


// Retrieve authentication token for subsequent API requests
export function setup() {
    const user = randomItem(users);

    const body = {
        password: user.pin,
        user: user.phone,
        channelName: uuidv4(),
        fcmToken: uuidv4(),
    };

    const loginRes = http.post(`${BASE_URL}/api/v2/access/authorize`, JSON.stringify(body), {
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const isSuccess = check(loginRes, {
        "logged in successfully": (r) => r.status === 200 && !!loginRes.json('access_token')
    });

    if (!isSuccess) {
        console.log(loginRes);
        fail("user was not logged in");
    }
    const authToken = loginRes.json('access_token');

    return authToken;
}

const getFormVersions = (requestParams) => {
    let url = BASE_URL + `/api/v1/form`;

    let response = http.get(url, requestParams);

    const isSuccess = check(response, {
        "fetched forms versions successfully": (r) => r.status === 200
    });

    durationOnGetFormVersions.add(response.timings.duration);

    if (isSuccess) {
        return response.json();
    }

    console.log('getFormVersions:', response);
    fail('Failed to get forms versions. Status code was *not* 200');
}

const getForm = (requestParams, formId) => {
    let url = BASE_URL + `/api/v1/form/${formId}`;

    let response = http.get(url, requestParams);

    const isSuccess = check(response, {
        "fetched form successfully": (r) => r.status === 200
    });

    durationOnGetFormData.add(response.timings.duration);

    if (isSuccess) {
        return response.json();
    }

    console.log('getForm:', response);
    fail('Failed to get form. Status code was *not* 200');
}

const submitAnswers = (requestParams, answers) => {
    let url = BASE_URL + `/api/v1/answers`;

    const body = {
        answers: answers
    };

    let response = http.post(url, JSON.stringify(body), requestParams);

    const isSuccess = check(response, {
        "submitted answers successfully": (r) => r.status === 200
    });

    durationOnSubmitAnswers.add(response.timings.duration);

    if (!isSuccess) {
        console.log('submitAnswers:', response);
        fail('Failed to submit answers. Status code was *not* 200');
    }
}

export default (authToken) => {
    // set the authorization header on the session for the subsequent requests
    const requestConfig = {
        headers: {
            Authorization: `Bearer ${authToken}`,
            'Content-Type': 'application/json'
        },
        timeout: "300s",
    };

    const data = getFormVersions(requestConfig);
    for (const formVersion of data.formVersions) {
        const formSections = getForm(requestConfig, formVersion.id);
        const pollingStation = pollingStations[scenario.iterationInTest % pollingStations.length];

        const randomAnswers = getRandomAnswers(formSections, pollingStation.code, pollingStation.number);
        submitAnswers(requestConfig, randomAnswers);
    }
};