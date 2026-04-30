# Skywalker Source Generator Template

This template creates a Roslyn incremental source generator project and a focused xUnit test project.

## Install Locally

From the Skywalker repository root:

```powershell
dotnet new install .\templates\skywalker-generator
```

## Create A Generator

```powershell
dotnet new skywalker-generator -n Skywalker.Ddd.EntityFrameworkCore.SourceGenerators -o src\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators
dotnet build src\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators.csproj
dotnet test src\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators\tests\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators.Tests\Skywalker.Ddd.EntityFrameworkCore.SourceGenerators.Tests.csproj
```

When generated under `src/` in this repository, the project links `Skywalker.SourceGenerators.Common` source files automatically. When generated outside the repository, the sample generator still builds and runs so the scaffold can be evaluated independently.

## What To Change First

- Rename `MarkerAttributeGenerator` to the generator you are building.
- Replace the marker attribute metadata name with the public attribute your feature owns.
- Add generator-specific diagnostics in the `SKY1xxx`-`SKY5xxx` range.
- Add snapshot and diagnostics tests before wiring the generator into a host package.