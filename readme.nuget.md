### NuGet commands
___
###### Publish a package
```powershell
# pack specified version (in release, symbols)
dotnet pack <PROJECT> --include-symbols -c Release -o nupkgs -p:PackageVersion=x.0.0`

# publish the resulting symbols package
nuget push nupkgs\<PROJECT>.1.0.0.symbols.nupkg
```
___
###### Autopublish

Run `NuGet-PublishAllLocally.ps1` to locally force-publish the packages for all projects
