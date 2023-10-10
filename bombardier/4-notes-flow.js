import http from "k6/http";
import { check, fail } from "k6";
import { Trend } from 'k6/metrics';
import { SharedArray } from 'k6/data';
import { scenario } from 'k6/execution';

import { uuidv4, randomItem, randomString } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';
import papaparse from 'https://jslib.k6.io/papaparse/5.1.1/index.js';
import { FormData } from 'https://jslib.k6.io/formdata/0.0.2/index.js';


const BASE_URL = "https://api-staging.votemonitor.org";

const durationOnAddNote = new Trend('durationOnAddNote', true);

const users = new SharedArray('users', function () {
    const data = papaparse.parse(open('./users.csv'), { header: true }).data;
    return data;
});

const pollingStations = new SharedArray('polling-stations', function () {
    const data = papaparse.parse(open('./polling-stations.csv'), { header: true }).data;
    return data;
});

const image1 = open('./attachments/image1.jpg', 'b');
// const image2 = open('./attachments/image2.jpg', 'b');
// const image3 = open('./attachments/image3.jpg', 'b');
// const image4 = open('./attachments/image4.jpg', 'b');
// const image5 = open('./attachments/image5.jpg', 'b');
// const image6 = open('./attachments/image6.jpg', 'b');
// const image7 = open('./attachments/image7.jpg', 'b');
// const image8 = open('./attachments/image8.jpg', 'b');
// const image9 = open('./attachments/image9.jpg', 'b');

// const video1m = open('./attachments/video1m.mp4', 'b');
// const video2m = open('./attachments/video2m.mp4', 'b');
// const video3m = open('./attachments/video3m.mp4', 'b');


export const options = {
    setupTimeout: '5m',
    // Key configurations for avg load test in this section
    stages: [
        { duration: '5m', target: 5 }, // traffic ramp-up from 1 to 100 users over 5 minutes.
        { duration: '5m', target: 5 }, // stay at 100 users for 30 minutes
        { duration: '5m', target: 0 }, // ramp-down to 0 users
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


const addNote = (token, municipalityCode, pollingStationNumber) => {
    let url = BASE_URL + `/api/v2/note`;

    const formData = new FormData();

    formData.append('files', http.file(image1, 'image1.jpg', 'image/jpeg'));
    // formData.append('files', http.file(video1m, 'video.mp4', 'video/mp4'));

    formData.append('provinceCode', municipalityCode.slice(0, 2));
    formData.append('countyCode', municipalityCode.slice(0, 4));
    formData.append('municipalityCode', municipalityCode);
    formData.append('pollingStationNumber', pollingStationNumber);
    formData.append('text', randomString(256));

    const response = http.post(url, formData.body(), {
        headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'multipart/form-data; boundary=' + formData.boundary
        },
        timeout: "300s"
    });

    const isSuccess = check(response, {
        "added note successfully": (r) => r.status === 200
    });

    durationOnAddNote.add(response.timings.duration);

    if (!isSuccess) {
        console.log('addNote:', (response + '').slice(0, 1000));
        fail('Failed to added note. Status code was *not* 200');
    }
}

export default () => {
    const authToken = login();

    const pollingStation = pollingStations[scenario.iterationInTest % pollingStations.length];
    addNote(authToken, pollingStation.code, pollingStation.number);
};