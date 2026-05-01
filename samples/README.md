# Skywalker Source Generator Samples

This directory is the integration sample matrix for the v2.0 Source Generator work.
The samples are intentionally small: each project is a focused canary for one edge
case that should stay buildable and runnable in CI as generators are added.

## Current State

Sprint 0 created the sample matrix skeleton. Sprint 1 starts filling it in with
real generator-driven code. Each sample should stay buildable and runnable in CI
as it moves from placeholder to scenario canary.

## Sample Matrix

| Sample | Scenario | Sprint 0 status | Filled in by |
|---|---|---|---|
| `Skywalker.Sample.Minimal` | Smallest app; EF repository generator analyzer + `AddSkywalkerDbContext<TDbContext>()` happy path | Sprint 1 smoke test: generated metadata + repository/domain-service registration | Sprint 1, Sprint 3 |
| `Skywalker.Sample.MultiDbContext` | Multiple EF Core DbContexts without generated-name collisions | Sprint 1 smoke test: generated metadata + repository/domain-service registration for two DbContexts | Sprint 1 |
| `Skywalker.Sample.GenericServices` | Open and closed generic application services | Skeleton, build/run smoke test | Sprint 1, Sprint 3 |
| `Skywalker.Sample.NestedTypes` | Nested partial types and fully-qualified generated names | Skeleton, build/run smoke test | Sprint 3 |
| `Skywalker.Sample.InternalServices` | `internal` service types generated in the same assembly | Skeleton, build/run smoke test | Sprint 3 |
| `Skywalker.Sample.Modular` | Cross-assembly module discovery and registration | Skeleton, single-project placeholder | Sprint 3 |
| `Skywalker.Sample.AspireAOT` | NativeAOT publish canary | Skeleton; requires local AOT toolchain for publish | Sprint 1+ |
| `Skywalker.Sample.LegacyMigration` | v1.x manual registration coexisting with v2.0 SG registration | Skeleton, build/run smoke test | Sprint 1, Sprint 5 |

`Skywalker.Sample.RealWorldShop` is intentionally out of this Sprint 0 matrix and
belongs to a later sprint once the generator stack is mature enough for a full app.

## Shared Files

`samples/Common/` is reserved for simple entities, service contracts, fixtures, and
helpers that are reused by multiple samples. Keep shared code dependency-light so
individual samples remain clear edge-case canaries rather than hidden mini-apps.

## Verify Locally

Build every sample:

```sh
for project in samples/Skywalker.Sample.*/*.csproj; do dotnet build "$project"; done
```

Run every sample smoke test:

```sh
for project in samples/Skywalker.Sample.*/*.csproj; do dotnet run --project "$project" --no-build; done
```

Publish the AOT canary:

```sh
dotnet publish samples/Skywalker.Sample.AspireAOT/Skywalker.Sample.AspireAOT.csproj -p:PublishAot=true -c Release
```

On Windows, NativeAOT publish requires the Visual Studio C++ toolchain, including the
Desktop development with C++ workload.