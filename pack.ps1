dotnet build -c Release

dotnet pack Skywalker.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet nuget push nupkgs\* -k 75ab86cf-c3b0-331e-ba67-a4d6a7a061a5 -s http://47.108.173.4:8081/nexus/repository/nuget-hosted/