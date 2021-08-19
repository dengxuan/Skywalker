#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Transfer/src/Skywalker.Transfer.WebApi.Hosting/Skywalker.Transfer.WebApi.Hosting.csproj", "src/Transfer/src/Skywalker.Transfer.WebApi.Hosting/"]
RUN dotnet restore "src/Transfer/src/Skywalker.Transfer.WebApi.Hosting/Skywalker.Transfer.WebApi.Hosting.csproj"
COPY . .
WORKDIR "/src/src/Transfer/src/Skywalker.Transfer.WebApi.Hosting"
RUN dotnet build "Skywalker.Transfer.WebApi.Hosting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skywalker.Transfer.WebApi.Hosting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skywalker.Transfer.WebApi.Hosting.dll"]