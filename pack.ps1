dotnet build -c Release

dotnet pack Skywalker.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet nuget push nupkgs\* -k 2774f634-bfc5-37e1-97d4-cc079a073278 -s http://47.108.173.4:8081/nexus/repository/nuget-hosted/