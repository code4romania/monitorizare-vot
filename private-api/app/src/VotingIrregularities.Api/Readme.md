
# Configuration
This API works with Microsoft's SQL Server thus, if you want to run the API locally you'll have to install SQL Server.

I do recommend to use VS 2017 Local DB which is a limeweight version of the SQL Server which fits perfectly for development purposes.
https://www.mssqltips.com/sqlservertip/5612/getting-started-with-sql-server-2017-express-localdb/

If you already have it installed make sure you have it updated with the latest cumulative patch from Microsoft, otherwise you'll get Access Denied kind of errors (Download link is embedded below)
https://support.microsoft.com/en-ie/help/4096875/fix-access-is-denied-error-when-you-try-to-create-a-database-in-sql-se


# Setting up
The backend initialization is pretty much straight-forward with no need to run any manual special dotnet scripts, it's handled in the Startup.cs of the webAPI.

# Adjusting any new changes 
After you make your changes you just have to go to Package Manager Console which can be found in Visual Studio (Ctrl+Q and search for it it), right there you'll just have to choose src\VotingIrregularities.Domain from the drop-down menu.
Once you do that just write down 'Add-Migration  <migrationName>'.
If you don't want it anymore just type 'Remove-Migration', this will remove the latest migration.
If you want to rollback to a previous migration just use 'Update-Migration <migrationName>'

# More info can be found here: https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/#revert-a-migration