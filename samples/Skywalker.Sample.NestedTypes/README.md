# Skywalker.Sample.NestedTypes

App with **multi-level nested classes** carrying SG attributes.

## What this sample validates

- SG-generated partial declarations correctly chain `partial class Outer` →
  `partial class Inner` → target
- Fully-qualified names (`global::App.Outer.Inner.OrderAppService`) resolve in DI
- Diagnostic SKY9001 fires only on the leaf type if it's missing `partial`,
  not on outer types that are syntactically partial

## Current state (Sprint 0)

Skeleton. Real nested type fixtures pending SG attribute existence.

## Filled in by

- **Sprint 3 (DI 自动注册 SG)** — adds the `[ApplicationService]` nested fixtures
