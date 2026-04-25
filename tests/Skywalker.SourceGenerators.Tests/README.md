# Skywalker.SourceGenerators.Tests

Shared test infrastructure for every Skywalker source generator. Each generator's own
test project (`Skywalker.Xxx.SourceGenerators.Tests`) references **this** project and
calls into `GeneratorTestHelper` instead of rolling its own driver.

## Authoring tests for a new generator

1. Create `tests/Skywalker.Xxx.SourceGenerators.Tests/Skywalker.Xxx.SourceGenerators.Tests.csproj`
   (net8.0, IsPackable=false) with these references:

   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Skywalker.SourceGenerators.Tests\Skywalker.SourceGenerators.Tests.csproj" />
     <ProjectReference Include="..\..\src\Skywalker.Xxx.SourceGenerators\Skywalker.Xxx.SourceGenerators.csproj"
                       OutputItemType="Analyzer"
                       ReferenceOutputAssembly="false"
                       PrivateAssets="all" />
   </ItemGroup>
   ```

2. Mark each test class with `[UsesVerify]` (required by Verify.Xunit 22.x).

3. Snapshot test (preferred):

   ```csharp
   [Fact]
   public Task AppService_WithRepository_Generates()
   {
       const string source = """
           [ApplicationService]
           public partial class OrderAppService
           {
               public Task<Order> GetAsync(Guid id) => default!;
           }
           """;
       var driver = GeneratorTestHelper.RunDriver<AppServiceGenerator>(source);
       return Verifier.Verify(driver);
   }
   ```

   First run produces `Snapshots/<TestClass>.<TestMethod>.received.cs`. After human review,
   rename to `.verified.cs` and commit. CI will fail on any `*.received.*` left in the tree.

4. Diagnostic test:

   ```csharp
   [Fact]
   public void NonPartialClass_ReportsSky9001()
   {
       var result = GeneratorTestHelper.Run<AppServiceGenerator>(...);
       var d = Assert.Single(result.Diagnostics);
       Assert.Equal("SKY9001", d.Id);
   }
   ```

5. Compilability test (optional but useful for sanity):

   ```csharp
   var (_, output) = GeneratorTestHelper.RunWithCompilation<AppServiceGenerator>(source);
   var diagnostics = output.GetDiagnostics()
       .Where(d => d.Severity >= DiagnosticSeverity.Warning);
   Assert.Empty(diagnostics);
   ```

## What `GeneratorTestHelper` gives you

| Method | When to use |
|---|---|
| `Run<TGen>(source, additionalReferences?)` | Inspect diagnostics / generated trees programmatically |
| `RunDriver<TGen>(source, additionalReferences?)` | Pass to `Verifier.Verify(driver)` for snapshot tests |
| `RunWithCompilation<TGen>(source, additionalReferences?)` | Verify generated code itself compiles cleanly |

Default references are loaded once per process via `Basic.Reference.Assemblies.Net80` plus
`Microsoft.Extensions.DependencyInjection.Abstractions`. Pass `additionalReferences` for any
NuGet package or assembly your test source needs (typical: the SG's own attribute assembly,
EF Core, Skywalker domain types).

## Snapshot conventions

- Snapshots live in `<test-file-dir>/Snapshots/`. Verify is configured globally to put them
  there via `DerivePathInfo` so reviewers see test source and snapshot side-by-side.
- File naming: `<TestClass>.<TestMethod>#<source-name>.verified.cs` (`#<source-name>` is
  Verify's own splitting when a generator emits multiple files).
- A `*.received.*` file in the tree means the snapshot didn't match — review the diff,
  then `mv received → verified` to accept. **Never** auto-accept on CI; the build is
  configured to fail when received files are present.

## Sample fixture

`Sample/` holds a tiny `HelloGenerator` and three smoke tests. Treat them as a known-good
pattern for new SG tests; if they go red, the infrastructure itself is broken.
