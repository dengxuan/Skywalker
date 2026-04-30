# Skywalker.Sample.LegacyMigration

The "users mid-migration" sample. Demonstrates v1.x and v2.0 styles **coexisting in
the same process** so existing apps can move to v2.0 incrementally.

## What this sample validates

- `services.AddDefaultServices<TDbContext>()` (v1 reflection registration) still works
- New `[ApplicationService] partial class` sit alongside v1-style services in the same
  DI container with no conflict
- A user can rename a service to v2.0 style **one type at a time** without rewriting
  Program.cs

## Current state (Sprint 0)

Skeleton. The "two styles coexisting" story comes alive when v2.0 SG ships; for now
this is a placeholder so the matrix is complete.

## Filled in by

- **Sprint 1 (EF Repository SG)** — first co-existence scenario
- **Sprint 5 (release)** — comprehensive migration guide reference
