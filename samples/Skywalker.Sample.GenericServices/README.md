# Skywalker.Sample.GenericServices

App that exercises **generic application services** in both open and closed forms.

## What this sample validates

- Open-generic `[ApplicationService] partial class GenericAppService<T>` registers as
  open-generic in the DI container
- Closed-generic `[ApplicationService] partial class IntService : GenericAppService<int>`
  registers normally
- Generic constraints (`where T : new()`, `where T : IEntity` etc.) are preserved in
  the SG-generated code

## Current state (Sprint 0)

Skeleton. Generic services pending DI auto-registration SG.

## Filled in by

- **Sprint 1 / Sprint 3** — once `[ApplicationService]` SG exists
