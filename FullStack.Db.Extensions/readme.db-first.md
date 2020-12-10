### DbContext generation (db-first)
___
###### User Secrets
You may wish to have a user secret to deliver the connection string. This should target your consuming application project:

`dotnet user-secrets set MyDB "<Connection String>" -p MyApiProject`
___
###### Scaffold Db
You can then pick out the db connection from secret store:
```powershell
dotnet-ef dbcontext scaffold Name=<Secrets Key> <Provider> --context-dir . -f -p MyDbProject -s MyApiProject -o Models -c MyDbContext [-t MyTable ...]
```
***NB**: You can also swap out `Name=MyDB` with a connection string.*