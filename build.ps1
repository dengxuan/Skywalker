


Remove-Item nupkgs\*

#Extensions
dotnet pack src/libraries/Skywalker.Extensions.Universal/Skywalker.Extensions.Universal.sln -c Release -p:SymbolPackageFormat=snupkg  -p:IncludeSymbols=true --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Timezone/Skywalker.Extensions.Timezone.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Specifications/Skywalker.Extensions.Specifications.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.RateLimiters/Skywalker.Extensions.RateLimiters.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.Logging.File/Skywalker.Extensions.Logging.File.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.HtmlAgilityPack/Skywalker.Extensions.HtmlAgilityPack.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.HashedWheelTimer/Skywalker.Extensions.HashedWheelTimer.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#DependencyInjection
dotnet pack src/libraries/Skywalker.Extensions.DependencyInjection.Abstractions/Skywalker.Extensions.DependencyInjection.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Extensions.DependencyInjection/Skywalker.Extensions.DependencyInjection.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#ExceptionHandler
dotnet pack src/libraries/Skywalker.ExceptionHandler.Abstractions/Skywalker.ExceptionHandler.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#ObjectMapper
dotnet pack src/libraries/Skywalker.ObjectMapper.Abstractions/Skywalker.ObjectMapper.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.ObjectMapper.AutoMapper/Skywalker.ObjectMapper.AutoMapper.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Eventbus
dotnet pack src/libraries/Skywalker.EventBus.Abstractions/Skywalker.EventBus.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Security
dotnet pack src/libraries/Skywalker.Security.Abstractions/Skywalker.Security.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Security/Skywalker.Security.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Authorization
dotnet pack src/libraries/Skywalker.Authorization.Abstractions/Skywalker.Authorization.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.AspNetCore.Authorization/Skywalker.AspNetCore.Authorization.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Caching
dotnet pack src/libraries/Skywalker.Caching.Abstractions/Skywalker.Caching.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Caching.Redis/Skywalker.Caching.Redis.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Serialization
dotnet pack src/libraries/Skywalker.Serialization.Abstractions/Skywalker.Serialization.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Serialization.NewtonsoftJson/Skywalker.Serialization.NewtonsoftJson.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Serialization.MessagePack/Skywalker.Serialization.MessagePack.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Identifier
dotnet pack src/libraries/Skywalker.Identifier.Abstractions/Skywalker.Identifier.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Identifier.Guid/Skywalker.Identifier.Guid.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Identifier.Snowflake/Skywalker.Identifier.Snowflake.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Identifier.Snowflake.Redis/Skywalker.Identifier.Snowflake.Redis.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#Ddd
dotnet pack src/libraries/Skywalker.Ddd.Domain.Abstractions/Skywalker.Ddd.Domain.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore/Skywalker.Ddd.EntityFrameworkCore.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.MySQL/Skywalker.Ddd.EntityFrameworkCore.MySQL.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.EntityFrameworkCore.SqlServer/Skywalker.Ddd.EntityFrameworkCore.SqlServer.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.UnitOfWork.Abstractions/Skywalker.Ddd.UnitOfWork.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.Application.Abstractions/Skywalker.Ddd.Application.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.Ddd.Application/Skywalker.Ddd.Application.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
dotnet pack src/libraries/Skywalker.AspNetCore.Application/Skywalker.AspNetCore.Application.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs

#dotnet pack src/libraries/Skywalker.Ddd.Application.Commands.Abstractions/Skywalker.Ddd.Application.Commands.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
#dotnet pack src/libraries/Skywalker.Ddd.Application.Queries.Abstractions/Skywalker.Ddd.Application.Queries.Abstractions.sln -c Release -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --output nupkgs
