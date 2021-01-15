dotnet build -c Release

dotnet pack Skywalker.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet nuget push nupkgs\* -k 13456e84-367c-3678-b7bf-67eea84d9804 -s http://nexus.letggame.com/repository/nuget-skywalkers/