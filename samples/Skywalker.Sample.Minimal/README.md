# Skywalker.Sample.Minimal

Smallest possible Skywalker app. The "引包即用" smoke test.

## What this sample validates

- Adding the Skywalker NuGet package(s) and a single `AddSkywalker()` call boots the host
- No additional registration code is required for the happy-path scenario
- Source generator output participates in DI resolution at startup

## Current state (Sprint 0)

Skeleton. Prints a placeholder message; no real Skywalker registration yet because the
v2.0 generators don't exist on `main` until Sprint 1 lands.

## Filled in by

- **Sprint 1 (#185 EF Repository SG)** — wires up a single repository
- **Sprint 3 (DI 自动注册 SG)** — replaces manual `AddX()` with `AddSkywalker()`

## Run

```sh
dotnet run --project samples/Skywalker.Sample.Minimal
```
