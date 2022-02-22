
Remove-Item nupkgs\*

#Extensions
dotnet pack src/libraries/Skywalker.Extensions.Universal/Skywalker.Extensions.Universal.sln -c Release -p:SymbolPackageFormat=snupkg  -p:IncludeSymbols=true --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Timing/Skywalker.Extensions.Timing.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Threading/Skywalker.Extensions.Threading.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.GuidGenerator/Skywalker.Extensions.GuidGenerator.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Specifications/Skywalker.Extensions.Specifications.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.RateLimiters/Skywalker.Extensions.RateLimiters.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.MessagePack/Skywalker.Extensions.MessagePack.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Logging.File/Skywalker.Extensions.Logging.File.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Linq/Skywalker.Extensions.Linq.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Json/Skywalker.Extensions.Json.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.HtmlAgilityPack/Skywalker.Extensions.HtmlAgilityPack.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.HashedWheelTimer/Skywalker.Extensions.HashedWheelTimer.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Channels/Skywalker.Extensions.Channels.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#DependencyInjection
dotnet pack src/libraries/Skywalker.Extensions.DependencyInjection.Abstractions/Skywalker.Extensions.DependencyInjection.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.DependencyInjection/Skywalker.Extensions.DependencyInjection.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Exceptions
dotnet pack src/libraries/Skywalker.Extensions.Exceptions.Abstractions/Skywalker.Extensions.Exceptions.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#ObjectMapper
dotnet pack src/libraries/Skywalker.ObjectMapper.Abstractions/Skywalker.ObjectMapper.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.ObjectMapper.AutoMapper/Skywalker.ObjectMapper.AutoMapper.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Eventbus
dotnet pack src/libraries/Skywalker.EventBus.Abstractions/Skywalker.EventBus.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#UnitOfWork
dotnet pack src/libraries/Skywalker.Ddd.UnitOfWork.Abstractions/Skywalker.Ddd.UnitOfWork.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.UnitOfWork/Skywalker.Ddd.UnitOfWork.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Ddd
dotnet pack src/libraries/Skywalker.Ddd.Domain.Abstractions/Skywalker.Ddd.Domain.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore/Skywalker.Ddd.EntityFrameworkCore.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.MySQL/Skywalker.Ddd.EntityFrameworkCore.MySQL.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.SqlServer/Skywalker.Ddd.EntityFrameworkCore.SqlServer.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.Application.Abstractions/Skywalker.Ddd.Application.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.Application.Commands.Abstractions/Skywalker.Ddd.Application.Commands.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.Application.Queries.Abstractions/Skywalker.Ddd.Application.Queries.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs


dotnet nuget push nupkgs\*.nupkg -k f2205526-ad54-351e-a993-54ad510ce1ed -s https://nexus.tankswar.com/repository/nuget-hosted/index.json -ss https://nexus.tankswar.com/repository/nuget-hosted/index.json
