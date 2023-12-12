#!/usr/bin/env node
import yargs from 'yargs'
import readXlsxFile from 'read-excel-file/node'
import { forIn } from 'lodash-es';
import * as fs from 'node:fs';
import * as path from 'node:path';
import { hideBin } from 'yargs/helpers';

const options = yargs(hideBin(process.argv))
    .usage('Usage: -f <file path> -s 1 -o <output directory> -v 1')
    .option('f', { alias: 'filePath', describe: 'Excel file to be parsed', type: 'string', demandOption: true })
    .option('s', { alias: 'sheetIndex', describe: 'Sheet index to parse (1 indexed)', type: 'number', demandOption: true })
    .option('o', { alias: 'outputDirectory', describe: 'Output folder where to write imported forms.', type: 'string', demandOption: true })
    .argv;

if (!fs.existsSync(options.outputDirectory)) {
    throw new Error(`Specified output directory does not exist!outputDirectory= ${options.outputDirectory}`)
}

readXlsxFile(fs.readFileSync(options.filePath), { sheet: options.sheetIndex }).then((rows) => {
    // remove header
    rows = rows.slice(1);
    const forms = {};

    rows.forEach(row => {
        let form = undefined;

        if (!(row[0] in forms)) {
            form = {
                code: row[0],
                currentVersion: 1,
                description: row[0],
                formSections: [],
            };

            forms[row[0]] = form;
        } else {
            form = forms[row[0]]
        }

        let section = form.formSections.find(fs=>fs.code == row[1]);

        if (section === undefined) {
            section = {
                uniqueId: row[1],
                code: row[1].toString(),
                description: '',
                orderNumber: Object.keys(form.formSections).length + 1,
                questions: []
            };

            form.formSections.push(section);
        }

        section.questions.push({
            formCode: form.code,
            code: row[2].toString(),
            questionType: mapQuestionType(row[4]),
            text: row[3],
            orderNumber: section.questions.length + 1,
            optionsToQuestions: getOptions(row.slice(5))
        });
    });


    const generatedFiles = [];

    let formIndex = 1;
    forIn(forms,f => {
        f.order = formIndex;
        const content = JSON.stringify(f, null, 4);

        const outputFilePath = path.join(options.outputDirectory, f.code  + '.json');
        generatedFiles.push(outputFilePath);

        fs.writeFileSync(outputFilePath, content);
        formIndex++;
    })


    console.log(`All done!`);
    console.log(`Generated files:`);
    generatedFiles.forEach(f=>console.log(f))
});

function getOptions(options) {
    return options
    .filter(o=>o)
        .map(o => o.toString())
        .map(parseOption);
}

function parseOption(option, index) {
    const isFlagged = option.indexOf('$flag') !== -1;
    const isFreeText = option.indexOf('$text') !== -1;

    option = option.replace('$text', '').replace('$flag', '');

    return {
        isFreeText,
        isFlagged,
        text: option,
        orderNumber: index+1
    };
}

function mapQuestionType(excelQuestionType) {
    switch (excelQuestionType) {
        case 'multiple choice':
            return 0;
        case 'single choice':
            return 1;
        case 'single choice with text':
            return 2;
        case 'multiple choice with text':
            return 3;
        default:
            throw new Error(`Unknown question type: ${excelQuestionType}`);
    }
}
