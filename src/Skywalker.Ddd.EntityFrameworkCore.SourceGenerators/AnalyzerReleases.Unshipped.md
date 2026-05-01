; Unshipped analyzer release.
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
SKY3001 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | DbSet property must be public instance - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3001.md
SKY3002 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | Entity type must be accessible to generated code - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3002.md
SKY3003 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | Entity type must implement IEntity - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3003.md
SKY3004 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | Entity type must be a concrete class - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3004.md
SKY3005 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | Entity type is exposed by multiple DbSet properties - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3005.md
SKY3006 | Skywalker.Ddd.EntityFrameworkCore.SourceGenerators | Warning | Entity key type inference is ambiguous - see https://github.com/dengxuan/Skywalker/blob/main/docs/diagnostics/SKY3006.md
