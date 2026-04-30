# Skywalker.SourceGenerators.Common

Source-only shared library for Skywalker's internal source generators.

## What this is

A collection of helper types — `EquatableArray<T>`, `SymbolExtensions`, common
`DiagnosticDescriptors` — that every Skywalker source generator needs but that
should not be duplicated across generator projects.

## How generator projects consume it

Source-only via `<Compile Include>`, **not** `<ProjectReference>`. Reason: generator
DLLs are packaged into `analyzers/dotnet/cs` of the host NuGet package, and at
analyzer-load time the runtime cannot resolve dependencies that are not in that
same folder. Compiling the shared sources directly into each generator assembly
sidesteps that problem entirely.

In a generator project's csproj:

```xml
<ItemGroup>
  <Compile Include="$(MSBuildThisFileDirectory)..\Skywalker.SourceGenerators.Common\**\*.cs"
           Exclude="$(MSBuildThisFileDirectory)..\Skywalker.SourceGenerators.Common\bin\**;
                    $(MSBuildThisFileDirectory)..\Skywalker.SourceGenerators.Common\obj\**" />
</ItemGroup>
```

Each generator project still needs its own `Microsoft.CodeAnalysis.CSharp`
PackageReference — Common does not transitively provide it via this mechanism.

## How tests consume it

Test projects (`net8.0`) reference Common as a normal `<ProjectReference>`. The
project still builds a real assembly; only `IsPackable=false` keeps it out of
NuGet output.

## What lives here

| File | Purpose |
|---|---|
| `EquatableArray.cs` | Value-equal wrapper around `ImmutableArray<T>`, required by SG record models |
| `SymbolExtensions.cs` | `IsPartial`, `GetFullyQualifiedName`, `GetNamespace`, `HasAttributeOfType` |
| `DiagnosticDescriptors.cs` | Shared diagnostics (SKY9xxx). Generator-specific diagnostics belong in their own generator project. |

Adding a new helper here: keep it generator-shape-agnostic. If a helper only
applies to one specific SG (e.g. EF-Repository-specific), it belongs in that
generator project, not here.
