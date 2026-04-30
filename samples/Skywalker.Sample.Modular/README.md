# Skywalker.Sample.Modular

App composed of **multiple module assemblies**, each contributing services that the
SG discovers and auto-registers via `[ModuleInitializer]`.

## What this sample validates

- Each module assembly emits its own `__SkywalkerModuleInitializer__`
- `AddSkywalker()` discovers and stitches them together at startup with **no reflection**
- Module load order is stable and idempotent
- Cross-assembly `[ApplicationService]` and `[Repository]` resolution works

## Current state (Sprint 0)

Skeleton — single project. Will be split into a host app + at least one module library
(e.g. `Skywalker.Sample.Modular.Orders`) once the DI auto-registration SG can prove the
cross-assembly discovery story.

## Filled in by

- **Sprint 3 (DI 自动注册 SG)** — adds the module library project + cross-assembly
  registration test
