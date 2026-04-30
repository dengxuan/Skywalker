# Skywalker.Sample.MultiDbContext

App with **two** EF Core `DbContext`s in the same DI container.

## What this sample validates

- EF-Repository SG handles multiple DbContexts without name collisions
- Generated `AddXxxDbContextRepositories()` extension methods are uniquely named
- Repository<T,TKey> resolves to the correct DbContext per entity

## Current state (Sprint 0)

Skeleton. Real DbContexts will be added when the EF-Repository SG lands.

## Filled in by

- **Sprint 1 (EF Repository SG)** — adds `OrdersDbContext` + `BillingDbContext`,
  one repository in each, end-to-end smoke test
