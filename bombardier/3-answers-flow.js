import http from "k6/http";
import { check, fail } from "k6";
import { Trend } from 'k6/metrics';
import { SharedArray } from 'k6/data';

import { uuidv4, randomItem, randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';
import papaparse from 'https://jslib.k6.io/papaparse/5.1.1/index.js';
import { getRandomAnswers, randomDateToday, randomBool } from './utils.js';
const BASE_URL = "https://localhost:5001";

const durationOnGetProvinces = new Trend('durationOnGetProvinces', true);
const durationOnGetCounties = new Trend('durationOnGetCounties', true);
const durationOnGetMunicipalities = new Trend('durationOnGetMunicipalities', true);
const durationOnAddDetails = new Trend('durationOnAddDetails', true);

const durationOnGetFormVersions = new Trend('durationOnGetFormVersions', true);
const durationOnGetFormData = new Trend('durationOnGetFormData', true);
const durationOnSubmitAnswers = new Trend('durationOnSubmitAnswers', true);

const users = new SharedArray('users', function () {
    const data = papaparse.parse(open('./users.csv'), { header: true }).data;
    return data;
});

const pollingStations = new SharedArray('polling-stations', function () {
    const data = papaparse.parse(open('./polling-stations.csv'), { header: true }).data;

    return [data.reduce((result, item) => {
        const { communityCode, number } = item;

        // Check if the code already exists in the result object
        if (!result[communityCode]) {
            result[communityCode] = [];
        }

        // Add the number to the list associated with the code
        result[communityCode].push(number);

        return result;
    }, {})];
});


export const options = {
    // Key configurations for avg load test in this section
    stages: [
        { duration: '30s', target: 10 }, // traffic ramp-up from 1 to 100 users over 5 minutes.
        { duration: '2m', target: 10 }, // stay at 100 users for 30 minutes
        { duration: '30s', target: 0 }, // ramp-down to 0 users
    ],
};

// Retrieve authentication token for subsequent API requests
const login = () => {
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


const getProvinces = (requestParams) => {
    let url = BASE_URL + `/api/v1/province`;
    let response = http.get(url, requestParams);

    const isSuccess = check(response, {
        "fetched provinces successfully": (r) => r.status === 200
    });

    durationOnGetProvinces.add(response.timings.duration);

    if (isSuccess) {
        return response.json();
    }

    console.log('getProvinces:', response);
    fail('Failed to get provinces. Status code was *not* 200');
}

const getCounties = (requestParams, province) => {
    let url = BASE_URL + `/api/v1/province/${province.code}/counties`;

    let response = http.get(url, requestParams);

    const isSuccess = check(response, {
        "fetched counties successfully": (r) => r.status === 200
    });

    durationOnGetCounties.add(response.timings.duration);

    if (isSuccess) {
        return response.json();
    }

    console.log('getCounties:', response);
    fail('Failed to get counties. Status code was *not* 200');
}

const getMunicipalities = (requestParams, county) => {
    let url = BASE_URL + `/api/v1/county/${county.code}/municipalities`;

    let response = http.get(url, requestParams);
    durationOnGetMunicipalities.add(response.timings.duration);

    const isSuccess = check(response, {
        "fetched municipalities successfully": (r) => r.status === 200
    });

    if (isSuccess) {
        return response.json();
    }

    console.log('getMunicipalities:', response);
    fail('Failed to get municipalities. Status code was *not* 200');
}

const addPollingStationsDetails = (requestParams, countyCode, municipalityCode, pollingStationNumber) => {
    let url = BASE_URL + `/api/v1/polling-station`;

    const body = {
        countyCode: countyCode,
        municipalityCode: municipalityCode,
        pollingStationNumber: pollingStationNumber,
        observerArrivalTime: randomDateToday(),
        numberOfVotersOnTheList: randomIntBetween(100, 10000),
        numberOfCommissionMembers: randomIntBetween(5, 50),
        numberOfFemaleMembers: randomIntBetween(5, 50),
        minPresentMembers: randomIntBetween(5, 50),
        chairmanPresence: randomBool(),
        singlePollingStationOrCommission: randomBool(),
        adequatePollingStationSize: randomBool()
    };

    let response = http.post(url, JSON.stringify(body), requestParams);
    durationOnAddDetails.add(response.timings.duration);

    const isSuccess = check(response, {
        "added polling stations details successfully": (r) => r.status === 200
    });

    if (!isSuccess) {
        console.log('addPollingStationsDetails', request);
        fail('Failed to add polling stations details. Response code was *not* 200');
    }
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

export default () => {
    const authToken = login();
    // set the authorization header on the session for the subsequent requests
    const requestConfig = {
        headers: {
            Authorization: `Bearer ${authToken}`,
            'Content-Type': 'application/json'
        },
        timeout: "300s",
    };
    const provinces = getProvinces(requestConfig);
    const counties = getCounties(requestConfig, randomItem(provinces));
    const county = randomItem(counties);
    const municipalities = getMunicipalities(requestConfig, county);
    const municipality = randomItem(municipalities);
    const pollingStationNumber = randomItem(pollingStations[0][municipality.code]);
    addPollingStationsDetails(requestConfig, county.code, municipality.code, pollingStationNumber);

    const data = getFormVersions(requestConfig);
    for (const formVersion of data.formVersions) {
        const formSections = getForm(requestConfig, formVersion.id);


        const randomAnswers = getRandomAnswers(formSections, municipality.code, pollingStationNumber);
        submitAnswers(requestConfig, randomAnswers);
    }
};