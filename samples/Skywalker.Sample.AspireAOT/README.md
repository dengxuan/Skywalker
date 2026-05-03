# Skywalker.Sample.AspireAOT

The AOT-publish canary. The whole "Skywalker is NativeAOT-friendly" promise rides on
this sample's `dotnet publish -p:PublishAot=true` succeeding with **zero warnings**.

## What this sample validates

- `dotnet publish -p:PublishAot=true` produces a native binary
- No `IL2xxx` (trim) or `IL3xxx` (AOT) warnings — generated code is reflection-free
- EF repository generated registration can be called directly without runtime scanning
- DynamicProxy generated static proxies execute registered interceptors without Castle or runtime IL emit
- Output binary is small and starts fast

## Current state

This sample is an active source-generator AOT canary. It exercises the EF repository
generated registrar and the DynamicProxy generated static proxy path in one small
console app so `sg-quality.yml` can publish it as a NativeAOT binary and fail on any
`IL2xxx` / `IL3xxx` warning.

## Verify locally

```sh
dotnet publish samples/Skywalker.Sample.AspireAOT -p:PublishAot=true -c Release
```

## Filled in by

- **Sprint 1 (EF Repository SG)** — first SG-generated code participates in AOT publish
- **Sprint 2 (DynamicProxy SG + Castle removal)** — generated static proxy path participates in AOT publish
- **Sprint 5 (release)** — strict warning-as-error gate
