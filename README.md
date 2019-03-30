# Monitorizare Vot - Rest API for mobile apps

[![GitHub contributors](https://img.shields.io/github/contributors/code4romania/monitorizare-vot.svg?style=for-the-badge)](https://github.com/code4romania/monitorizare-vot/graphs/contributors) [![GitHub last commit](https://img.shields.io/github/last-commit/code4romania/monitorizare-vot.svg?style=for-the-badge)](https://github.com/code4romania/monitorizare-vot/commits/master) [![License: MPL 2.0](https://img.shields.io/badge/license-MPL%202.0-brightgreen.svg?style=for-the-badge)](https://opensource.org/licenses/MPL-2.0)

[See the project live](http://monitorizarevot.ro/)

Monitorizare Vot is a mobile app for monitoring elections by authorized observers. They can use the app in order to offer a real-time snapshot on what is going on at polling stations and they can report on any noticeable irregularities. 

The NGO-s with authorized observers for monitoring elections have real time access to the data the observers are transmitting therefore they can report on how voting is evolving and they can quickly signal to the authorities where issues need to be solved. 

Moreover, where it is allowed, observers can also photograph and film specific situations and send the images to the NGO they belong to. 

The app also has a web version, available for every citizen who wants to report on election irregularities. Monitorizare Vot was launched in 2016 and it has been used for the Romanian parliamentary elections so far, but it is available for further use, regardless of the type of elections or voting process. 

[Built with](#built-with) | [Repos and projects](#repos-and-projects) | [Deployment](#deployment) | [Contributing](#contributing) | [Feedback](#feedback) | [License](#license) | [About Code4Ro](#about-code4ro)

## Built With

 .Net Core 2.2
 
 Swagger docs for the API are available [here](https://mv-mobile-prod.azurewebsites.net/swagger/ui/index.html).

## Repos and projects

Client apps:

- android - https://github.com/code4romania/monitorizare-vot-android
- iOS - https://github.com/code4romania/monitorizare-vot-ios

Other MV related repos:

- https://github.com/code4romania/monitorizare-vot-admin
- https://github.com/code4romania/monitorizare-vot-ong
- https://github.com/code4romania/monitorizare-vot-votanti-client/
- https://github.com/code4romania/monitorizare-vot-votanti-api
- https://github.com/code4romania/monitorizare-vot-votanti-admin
- https://github.com/code4romania/monitorizare-vot-docs

## Creating the database

The Assembly VotingIrregularities.Domain has EF Migrations configured and can generate a database complete with test data.

To do this, follow the steps bellow:

Fill-in `appsetings.json` OR add in a new `appsettings.target.json` file the connectionstring to the SQL instance where the DB should be created.

Run the following console command from the `VotingIrregularities.Domain` folder:

 ```sh
private-api\app\VotingIrregularities.Domain> dotnet run
```

**Important:** the migrate action with delete the data from the following tables: RaspunsDisponibil, Intrebare, Sectiune, Optiune.

## Deployment

1. install .NetCore (Open Source/Free/Multiplatform) from [here](https://www.microsoft.com/net/core#windows)

2. run the following console command form the `app` folder:
    ```sh
    private-api\app> dotnet restore
    ```
  
3. run the following console command form the `VotingIrregularities.Api` folder:
    ```sh
    private-api\app\VotingIrregularities.Api> dotnet run
    ```
  
4. browse to indicated address: <http://localhost:5000/swagger/ui>

## Contributing

If you would like to contribute to one of our repositories, first identify the scale of what you would like to contribute. If it is small (grammar/spelling or a bug fix) feel free to start working on a fix. If you are submitting a feature or substantial code contribution, please discuss it with the team and ensure it follows the product roadmap.

* Fork it (https://github.com/code4romania/monitorizare-vot/fork)
* Create your feature branch (git checkout -b feature/fooBar)
* Commit your changes (git commit -am 'Add some fooBar')
* Push to the branch (git push origin feature/fooBar)
* Create a new Pull Request

[Pending issues](https://github.com/code4romania/monitorizare-vot/issues)

## Feedback

* Request a new feature on GitHub.
* Vote for popular feature requests.
* File a bug in GitHub Issues.
* Email us with other feedback contact@code4.ro

## License

This project is licensed under the MPL 2.0 License - see the [LICENSE](LICENSE) file for details

## About Code4Ro

Started in 2016, Code for Romania is a civic tech NGO, official member of the Code for All network. We have a community of over 500 volunteers (developers, ux/ui, communications, data scientists, graphic designers, devops, it security and more) who work pro-bono for developing digital solutions to solve social problems. #techforsocialgood. If you want to learn more details about our projects [visit our site](https://www.code4.ro/en/) or if you want to talk to one of our staff members, please e-mail us at contact@code4.ro.

Last, but not least, we rely on donations to ensure the infrastructure, logistics and management of our community that is widely spread accross 11 timezones, coding for social change to make Romania and the world a better place. If you want to support us, [you can do it here](https://code4.ro/en/donate/).
