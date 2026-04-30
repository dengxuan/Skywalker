# Source Generator Edge Cases

This checklist is the shared coverage ledger for Skywalker v2.0 source generators.
Every generator PR should mark the scenarios it covers with snapshot, diagnostic, or
sample coverage. Leave a short note with the test name when a box is checked.

Legend:

- `[ ]` Not covered yet
- `[x]` Covered by committed tests or samples
- `N/A` Not applicable to the specific generator; explain why in the note

## Type Shapes

- [ ] Plain class
- [ ] Partial class
- [ ] Partial class split across multiple files
- [ ] Record class
- [ ] Record struct
- [ ] Struct
- [ ] Sealed class
- [ ] Abstract class
- [ ] Static class where unsupported scenarios must report diagnostics
- [ ] Generic class with open type parameters
- [ ] Closed generic derived class
- [ ] Nested class one level deep
- [ ] Nested class multiple levels deep
- [ ] File-scoped type
- [ ] Type in the global namespace

## Visibility And Namespaces

- [ ] Public type
- [ ] Internal type
- [ ] Private nested type where unsupported scenarios must report diagnostics
- [ ] File-scoped namespace
- [ ] Block-scoped namespace
- [ ] Global using affects referenced types
- [ ] Using alias affects referenced types
- [ ] Type name collides with generated helper name
- [ ] Same short type name in different namespaces

## Inheritance And Interfaces

- [ ] Single interface implementation
- [ ] Multiple interface implementations
- [ ] Interface inheritance chain
- [ ] Explicit interface implementation
- [ ] Base class in the same assembly
- [ ] Base class from a referenced assembly
- [ ] Generic base class
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
- [ ] Generated file name remains stable between runs

## Diagnostics

- [ ] Missing `partial` reports `SKY9001` or generator-specific diagnostic
- [ ] Unsupported type shape reports a diagnostic instead of silently skipping
- [ ] Unsupported member signature reports a diagnostic instead of silently skipping
- [ ] Duplicate generated registration reports a diagnostic with both locations where possible
- [ ] Diagnostic `helpLinkUri` points to a `docs/diagnostics/SKYxxxx.md` page
- [ ] Diagnostic test asserts ID, severity, message, and location

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