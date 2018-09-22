# MV mobile API
a short guideline to get this up and running

There are serveral areas that need to be taken into consideration here:
- Settings
- Database
- Generating passwords

## Settings

in the API:
```json
{
  "SecretKey": "{secretKey}", // whatever you want here
  "ConnectionStrings": {
    "DefaultConnection": "{sqlServerConnectionString}" // refer to connectionstrings.com for standards; make sure it's the same for the domain project.
  },
  "BlobStorageOptions": { // these are the details of your azure blob storage - you can retrieve these from the Azure portal quite easy: navigate to the storage account -> settings section -> Access Keys. Make sure you have a container set up for that storage.
    "Container": "{containerName}",
    "AccountName": "{accountName}",
    "AccountKey": "{accountKey}"
  },
  "ApplicationCacheOptions": {
    "Enabled": false,
    "Implementation": "MemoryDistributedCache" // MemoryDistributedCache or RedisCache
  },
  "HashOptions": {
        "Salt": "{salt}" // make sure you have the same one in the tests project when generating the hashes ;)
  }
}
```

in the domain:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "{sqlServerConnectionString}" // refer to connectionstrings.com for standards;
  }
}
```

in the tests:
```json
{
  "HashOptions": {
        "Salt": "{salt}" // make sure you have the same one in the api project when generating the hashes ;)
  }
}
```

## Database

The .Domain project contains the EF context with Migrations activated.
So once you have a SQL server set up and the settings in place, you can generate the database and the tables with some initial seed data just by building the `VotingIrregularities.Domain` project and opening a command prompt in that folder's root and running:

```powershell
dotnet run
```



## Generating passwords

Build the `VotingIrregularities.Tests`
Under the build output folder (probably: monitorizare-vot\private-api\app\test\VotingIrregularities.Tests\bin\Debug\netcoreapp2.1) create `conturi.txt` file with the format 
```{phoneNumber}\t{desiredPin}```
on each line. (yes, these are tab separated values, you can export an excel file with that format)

Uncomment the `Skip` handle from the `Fact` attribute in the HashTests.cs\SetPasswords test.

The result of the test should be another file called `conturi-cu-parole.txt` that will also contain the hashes of those PINs as another column.

Reminder: make sure you have the same `Salt` configured in the appsettings.json file in the tests and the api.


Getting back to the database:
- add an ngo in the `NGOs` table.
- add an observer in the `Observers` table using one of the rows in the output file. Use the generated hash for the `Pin` column. 

## Testing out the API

Use the swagger UI to understand the endpoints and routes; you can also use the UI for token generation (testing out the `POST /api/v1/access/token` route)
For the rest of the endpoints you will need to use this token and inject a header (this is not currently possible using the swagger UI - it is probably a configuration effort - any hints are welcome)

The auth header is 

```Authorization: Bearer {token}```

and it needs to be injected into each other request.