# Source Generator Edge Cases

This checklist is the shared coverage ledger for Skywalker v2.0 source generators.
Every generator PR should mark the scenarios it covers with snapshot, diagnostic, or
sample coverage. Leave a short note with the test name when a box is checked.

Legend:

- `[ ]` Not covered yet
- `[x]` Covered by committed tests or samples
- `N/A` Not applicable to the specific generator; explain why in the note

## Type Shapes

- [x] Plain class — `DependencyInjectionRegistrationGeneratorSnapshotTests.SelfRegisteredService`
- [ ] Partial class
- [ ] Partial class split across multiple files
- [ ] Record class
- [ ] Record struct
- [ ] Struct
- [x] Sealed class — `RepositoryRegistrationGeneratorSnapshotTests.SingleDbContext_WithIntKey`
- [x] Abstract class — `RepositoryRegistrationGeneratorSnapshotTests.DbContext_WithAbstractEntityOnly` reports `SKY3004`
- [ ] Static class where unsupported scenarios must report diagnostics
- [ ] Generic class with open type parameters
- [ ] Closed generic derived class
- [x] Nested class one level deep — `RepositoryRegistrationGeneratorSnapshotTests.PublicNestedEntity_IsGenerated`
- [x] Nested class multiple levels deep — `RepositoryRegistrationGeneratorSnapshotTests.DeepNestedEntity_IsGenerated`
- [x] File-scoped type — `RepositoryRegistrationGeneratorSnapshotTests.SingleDbContext_WithGuidKey`
- [x] Type in the global namespace — `RepositoryRegistrationGeneratorSnapshotTests.GlobalNamespace_IsGenerated`

## Visibility And Namespaces

- [x] Public type — `RepositoryRegistrationGeneratorSnapshotTests.SingleDbContext_WithGuidKey`
- [x] Internal type — `RepositoryRegistrationGeneratorSnapshotTests.InternalEntity_IsGenerated`
- [x] Private nested type where unsupported scenarios must report diagnostics — `RepositoryRegistrationGeneratorSnapshotTests.PrivateNestedEntity_ProducesNoSources` reports `SKY3002`
- [x] File-scoped namespace — `RepositoryRegistrationGeneratorSnapshotTests.SingleDbContext_WithGuidKey`
- [x] Block-scoped namespace — `RepositoryRegistrationGeneratorSnapshotTests.BlockScopedNamespace_IsGenerated`
- [ ] Global using affects referenced types
- [x] Using alias affects referenced types — `RepositoryRegistrationGeneratorSnapshotTests.UsingAlias_ForDbSet_IsGenerated`
- [x] Type name collides with generated helper name — `RepositoryRegistrationGeneratorSnapshotTests.DbContextNameCollision_WithGeneratedHelperSuffix`
- [x] Same short type name in different namespaces — `RepositoryRegistrationGeneratorSnapshotTests.SameShortEntityName_InDifferentNamespaces`

## Inheritance And Interfaces

- [x] Single interface implementation — `DependencyInjectionRegistrationGeneratorSnapshotTests.DiscoveredInterface`
- [x] Multiple interface implementations — `RepositoryRegistrationGeneratorSnapshotTests.DbContext_WithConflictingKeyTypes` reports `SKY3006`
- [ ] Interface inheritance chain
- [x] Explicit interface implementation — `RepositoryRegistrationGeneratorSnapshotTests.ExplicitIEntityImplementation_IsGenerated`
- [x] Base class in the same assembly — `RepositoryRegistrationGeneratorSnapshotTests.DerivedEntityBaseClass_InSameAssembly`
- [ ] Base class from a referenced assembly
- [x] Generic base class — `RepositoryRegistrationGeneratorSnapshotTests.GenericEntityClosedByDerivedClass_IsGenerated`
- [ ] Generic interface with constraints

## Members And Signatures

- [ ] Synchronous method
- [ ] Async `Task` method
- [ ] Async `Task<T>` method
- [ ] Async `ValueTask` method
- [ ] Async `ValueTask<T>` method
- [ ] `IAsyncEnumerable<T>` method
- [ ] Void method
- [ ] Property getter/setter
- [ ] Static method where unsupported scenarios must report diagnostics
- [ ] Generic method
- [ ] Generic method with `where T : new()` constraint
- [ ] Generic method with interface/base constraints
- [ ] `ref` parameter
- [ ] `in` parameter
- [ ] `out` parameter
- [ ] `ref struct` parameter where unsupported scenarios must report diagnostics
- [ ] `params` array parameter
- [ ] Optional parameter with default value
- [ ] Nullable reference type annotations
- [ ] Attribute forwarding from source member to generated member

## Project And Compilation Inputs

- [ ] Analyzer config option read from `.editorconfig`
- [ ] MSBuild property exposed via `CompilerVisibleProperty`
- [ ] Additional file input
- [ ] Multi-targeting project
- [ ] Consumer project references generator as analyzer
- [ ] Generator packaged under `analyzers/dotnet/cs`
- [ ] Incremental cache hit on repeated runs
- [x] Generated file name remains stable between runs — EF repository snapshots include generated file paths

## Diagnostics

- [ ] Missing `partial` reports `SKY9001` or generator-specific diagnostic
- [x] Unsupported type shape reports a diagnostic instead of silently skipping — `RepositoryRegistrationGeneratorSnapshotTests.DbContext_WithAbstractEntityOnly`
- [ ] Unsupported member signature reports a diagnostic instead of silently skipping
- [x] Duplicate generated registration reports a diagnostic with both locations where possible — `RepositoryRegistrationGeneratorSnapshotTests.DbContext_WithDuplicateEntityDbSets` reports duplicate property names; DI generated registrar idempotency covered by `GeneratedDependencyInjectionRegistrationBridgeTests.AddSkywalker_GeneratedDependencyInjectionRegistration_IsIdempotent`
- [x] Diagnostic `helpLinkUri` points to a `docs/diagnostics/SKYxxxx.md` page — `SKY3001`-`SKY3006` descriptors
- [x] Diagnostic test asserts ID, severity, message, and location — `RepositoryRegistrationGeneratorTests` asserts ID, severity, and message; location coverage remains future work

## Runtime And Integration

- [ ] Minimal sample builds and runs
- [ ] Multi-DbContext sample builds and runs
- [ ] Generic services sample builds and runs
- [ ] Nested types sample builds and runs
- [ ] Internal services sample builds and runs
- [ ] Modular sample builds and runs
- [ ] Legacy migration sample builds and runs
- [ ] AspireAOT sample publishes with `PublishAot=true`
- [ ] AspireAOT sample emits no `IL2xxx` warnings
- [ ] AspireAOT sample emits no `IL3xxx` warnings
