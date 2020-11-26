dotnet build -c Release

dotnet pack Skywalker.sln -c Release --output nupkgs

dotnet nuget push nupkgs\*.nupkg -k 13456e84-367c-3678-b7bf-67eea84d9804 -s http://nexus.letggame.com/repository/nuget-skywalkers/