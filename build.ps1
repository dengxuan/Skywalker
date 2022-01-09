
Remove-Item nupkgs\*

dotnet pack src/libraries/Skywalker.Extensions.Universal/Skywalker.Extensions.Universal.sln -c Release -p:SymbolPackageFormat=snupkg  -p:IncludeSymbols=true --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Timing/Skywalker.Extensions.Timing.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Threading/Skywalker.Extensions.Threading.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.GuidGenerator/Skywalker.Extensions.GuidGenerator.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet pack src/libraries/Skywalker.EventBus.Abstractions/Skywalker.EventBus.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet pack src/libraries/Skywalker.Ddd.UnitOfWork.Abstractions/Skywalker.Ddd.UnitOfWork.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet pack src/libraries/Skywalker.Ddd.Domain.Abstractions/Skywalker.Ddd.Domain.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore/Skywalker.Ddd.EntityFrameworkCore.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.MySQL/Skywalker.Ddd.EntityFrameworkCore.MySQL.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.SqlServer/Skywalker.Ddd.EntityFrameworkCore.SqlServer.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

dotnet pack src/libraries/Skywalker.Ddd.Application.Abstractions/Skywalker.Ddd.Application.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs


#dotnet nuget push nupkgs\*.nupkg -k f2205526-ad54-351e-a993-54ad510ce1ed -s https://nexus.tankswar.com/repository/nuget-hosted/ -ss https://nexus.tankswar.com/repository/nuget-hosted/
