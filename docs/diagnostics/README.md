# Skywalker Diagnostics

This directory contains documentation for `SKYxxxx` diagnostics emitted by Skywalker
source generators and analyzers.

Every diagnostic descriptor must set `helpLinkUri` to a page in this directory, using
the diagnostic ID as the file name: `docs/diagnostics/SKYxxxx.md`.

## Diagnostic Ranges

| Range | Area |
|---|---|
| `SKY1xxx` | DI and service registration |
| `SKY2xxx` | DynamicProxy and interceptors |
| `SKY3xxx` | EF Repository generation |
| `SKY4xxx` | EventBus handler generation |
| `SKY5xxx` | Permission, localization, and settings generators |
| `SKY9xxx` | Common source-generator diagnostics |

## EF Repository Generation

| ID | Title |
|---|---|
| `SKY3001` | DbSet property must be public instance |
| `SKY3002` | Entity type must be accessible to generated code |
| `SKY3003` | Entity type must implement IEntity |
| `SKY3004` | Entity type must be a concrete class |
| `SKY3005` | Entity type is exposed by multiple DbSet properties |
| `SKY3006` | Entity key type inference is ambiguous |

## Required Page Structure

Each diagnostic page must include:

- Diagnostic ID and title
- Severity
- Introduced version
- Cause
- Incorrect example
- Correct example
- Related links

Use [TEMPLATE.md](TEMPLATE.md) when adding a new diagnostic page.