### Creating and populating db
___
###### Migrations
Be sure to add a reference to the Sql provider NuGet package in the db project
```powershell
# add a migration (no db connection needed)
dotnet-ef migrations add <MigrationName> -p MyDbProject -s MyApp

# remove the previous migration (no db connection needed)
dotnet-ef migrations remove -p MyDbProject -s MyApp

# Below scripts work fine, but a dedicated seeding app is recommended
# Not least, for local development and/or testing purposes

# apply all migrations (db needed!)
#$env:DOTNET_ENVIRONMENT='Local'; dotnet-ef database update -p MyDbProject -s MyApp

# apply specific migration
#$env:DOTNET_ENVIRONMENT='Local'; dotnet-ef database update <MigrationName> -p MyDbProject -s MyApp
```
___
###### Seeding
Kick off a dedicated seeding console app from cli, editing DOTNET_ENVIRONMENT as necessary

```powershell
# e.g. passing a flag to skip seeding and just apply migrations
$env:DOTNET_ENVIRONMENT='Local'; dotnet run -p MySeedingApp --migrate-only
```