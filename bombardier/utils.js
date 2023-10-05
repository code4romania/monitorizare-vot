import { randomIntBetween, randomString } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

export const randomBool = (chance = 0.5) => {
    return Math.random() < chance;
}

export const randomDateToday = () => {
    const startOfDay = new Date();
    startOfDay.setUTCHours(0, 0, 0, 0);

    const random = Math.floor(Math.random() * 86400000); // 1 day in ms

    startOfDay.setMilliseconds(startOfDay.getMilliseconds() + random);
    return startOfDay.toISOString();
}

// inspired by https://github.com/faker-js/faker/blob/fd6557e4/src/modules/helpers/index.ts#L963
export const randomArrayElements = (array) => {
    if (array.length === 0) {
        return [];
    }

    const numElements = randomIntBetween(1, array.length);

    const arrayCopy = array.slice(0);
    let i = array.length;
    const min = i - numElements;
    let temp;
    let index;

    // Shuffle the last `count` elements of the array
    while (i-- > min) {
        index = randomIntBetween(0, i);
        temp = arrayCopy[index];
        arrayCopy[index] = arrayCopy[i];
        arrayCopy[i] = temp;
    }

    return arrayCopy.slice(min);
}

export const getRandomAnswers = (sections, municipalityCode, pollingStationNumber) => {
    const answers = [];
    sections.forEach(section => {
        section.questions.forEach(question => {
            answers.push({
                questionId: question.id,
                countyCode: municipalityCode.slice(0, 4),
                municipalityCode: municipalityCode,
                pollingStationNumber: pollingStationNumber,
                options: randomArrayElements(question.optionsToQuestions).map(o => ({ optionId: o.optionId, value: randomString(32) }))
            });
        });
    });

    return answers;
}