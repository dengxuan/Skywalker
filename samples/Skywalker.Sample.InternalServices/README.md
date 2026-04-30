# Skywalker.Sample.InternalServices

App that exposes services as `internal` rather than `public`.

## What this sample validates

- `internal partial class OrderAppService` is still picked up by the SG
- The generated `__SkywalkerModuleInitializer__` registers it via its concrete type
- `InternalsVisibleTo` is **not** required for SG output to compile — the generated
  partial lives in the same assembly as the user code

## Current state (Sprint 0)

Skeleton. Internal-visibility scenario will materialise alongside DI auto-register SG.

## Filled in by

- **Sprint 3 (DI 自动注册 SG)** — adds `internal` service fixtures
