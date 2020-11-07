# Monitorizare Vot - Rest API for mobile apps & web NGO platform

[![GitHub contributors](https://img.shields.io/github/contributors/code4romania/monitorizare-vot.svg?style=for-the-badge)](https://github.com/code4romania/monitorizare-vot/graphs/contributors) [![GitHub last commit](https://img.shields.io/github/last-commit/code4romania/monitorizare-vot.svg?style=for-the-badge)](https://github.com/code4romania/monitorizare-vot/commits/master) [![License: MPL 2.0](https://img.shields.io/badge/license-MPL%202.0-brightgreen.svg?style=for-the-badge)](https://opensource.org/licenses/MPL-2.0)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=code4romania_monitorizare-vot&metric=alert_status)](https://sonarcloud.io/dashboard?id=code4romania_monitorizare-vot)
[![Build status](https://dev.azure.com/code4romania/monitorizare-vot-ci/_apis/build/status/monitorizare-vot/mv-api)](https://dev.azure.com/code4romania/monitorizare-vot-ci/_build/latest?definitionId=20)

[See the project live](https://votemonitor.org/)

Monitorizare Vot is a mobile app for monitoring elections by authorized observers. They can use the app in order to offer a real-time snapshot on what is going on at polling stations and they can report on any noticeable irregularities.

The NGO-s with authorized observers for monitoring elections have real time access to the data the observers are transmitting therefore they can report on how voting is evolving and they can quickly signal to the authorities where issues need to be solved.

Moreover, where it is allowed, observers can also photograph and film specific situations and send the images to the NGO they belong to.

The app also has a web version, available for every citizen who wants to report on election irregularities. Monitorizare Vot was launched in 2016 and it has been used for the Romanian parliamentary elections so far, but it is available for further use, regardless of the type of elections or voting process.

[Contributing](#contributing) | [Built with](#built-with) | [Repos and projects](#repos-and-projects) | [Deployment](#deployment) | [Feedback](#feedback) | [License](#license) | [About Code4Ro](#about-code4ro)

## Contributing

This project is built by amazing volunteers and you can be one of them! Here's a list of ways in [which you can contribute to this project](https://github.com/code4romania/.github/blob/master/CONTRIBUTING.md).

## Built With

 .Net Core 3.1
 
 Swagger docs for the API are available [here](https://app-vmon-api-dev.azurewebsites.net/swagger/index.html).

## Repos and projects

Please see more info and docs about the MV apps [in the wiki](https://github.com/code4romania/monitorizare-vot/wiki).

Client apps:

- Android - https://github.com/code4romania/mon-vot-android-kotlin
- iOS - https://github.com/code4romania/monitorizare-vot-ios
- Web admin for NGOs - https://github.com/code4romania/monitorizare-vot-ong

## Creating the database --- WIP you might encounter issues here.

The Assembly VotingIrregularities.Domain has EF Migrations configured and can generate a database complete with test data.

To do this, follow the steps bellow:

Fill-in `appsetings.json` OR add in a new `appsettings.target.json` file the connectionstring to the SQL instance where the DB should be created.

Run the following console command from the `VotingIrregularities.Domain.Seed` folder:

 ```sh
src\api\VotingIrregularities.Domain.Seed> dotnet run
```

**Important:** the migrate action with delete the data from the following tables: `Answers`, `Questions`, `FormSections`, `Options`.

## Test data

- to test admin features, you will need to add a test NGO and a test NGO admin linked to that NGO
- to test observer features, you will need to add observer accounts

- some dummy db data can be found [here](https://github.com/code4romania/monitorizare-vot/tree/develop/docs/dummy-db-data/)

## Deployment

1. install .NetCore (refer to the [Built With](#built-with) section for the proper version) (Open Source/Free/Multiplatform) from [here](https://www.microsoft.com/net/core#windows)

2. run the following console command from the `src` folder:
    ```sh
    src> dotnet restore
    ```
  
3. run the following console command from the `VoteMonitor.Api` folder:
    ```sh
    src\api\VoteMonitor.Api> dotnet run
    ```
  
4. browse to indicated address: <http://localhost:5000/swagger>

## Testing out the API

Use the swagger UI to understand the endpoints and routes; you can also use the UI for token generation (testing out the `POST /api/v1/access/token` route)
For the rest of the endpoints you will need to use this token and inject a header (this is not currently possible using the swagger UI - it is probably a configuration effort - any hints are welcome)

The auth header is 

```
Authorization: Bearer {token}
```

and it needs to be injected into each request.

## Feedback

* Request a new feature on GitHub.
* Vote for popular feature requests.
* File a bug in GitHub Issues.
* Email us with other feedback contact@code4.ro

## License

This project is licensed under the MPL 2.0 License - see the [LICENSE](LICENSE) file for details

## About Code4Ro

Started in 2016, Code for Romania is a civic tech NGO, official member of the Code for All network. We have a community of over 500 volunteers (developers, ux/ui, communications, data scientists, graphic designers, devops, it security and more) who work pro-bono for developing digital solutions to solve social problems. #techforsocialgood. If you want to learn more details about our projects [visit our site](https://www.code4.ro/en/) or if you want to talk to one of our staff members, please e-mail us at contact@code4.ro.

Last, but not least, we rely on donations to ensure the infrastructure, logistics and management of our community that is widely spread across 11 timezones, coding for social change to make Romania and the world a better place. If you want to support us, [you can do it here](https://code4.ro/en/donate/).
