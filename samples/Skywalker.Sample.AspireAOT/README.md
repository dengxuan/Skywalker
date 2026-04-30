# Skywalker.Sample.AspireAOT

The AOT-publish canary. The whole "Skywalker is NativeAOT-friendly" promise rides on
this sample's `dotnet publish -p:PublishAot=true` succeeding with **zero warnings**.

## What this sample validates

- `dotnet publish -p:PublishAot=true` produces a native binary
- No `IL2xxx` (trim) or `IL3xxx` (AOT) warnings — generated code is reflection-free
- Output binary is small and starts fast

## Current state (Sprint 0)

Skeleton. Currently only validates that `PublishAot=true` doesn't outright fail.
Warning-free publish is the bar for Sprint 1+ once each SG lands its AOT story.

## Verify locally

```sh
dotnet publish samples/Skywalker.Sample.AspireAOT -p:PublishAot=true -c Release
```

## Filled in by

- **Sprint 1 (EF Repository SG)** — first SG-generated code participates in AOT publish
- **Sprint 2 (DynamicProxy SG + Castle removal)** — major reflection elimination
- **Sprint 5 (release)** — strict warning-as-error gate
