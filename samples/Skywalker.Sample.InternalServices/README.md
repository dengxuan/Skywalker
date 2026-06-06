# Skywalker.Sample.InternalServices

App that verifies `internal` service declarations are still discoverable by the DI
auto-registration source generator in the same assembly.

## What this sample validates

- `[ApplicationService] internal sealed class InternalGreetingService` is discovered by the DI SG.
- The generated registrar is consumed by `AddSkywalker(typeof(Program).Assembly)`.
- `IInternalGreetingService -> InternalGreetingService` is registered as scoped and can be resolved from the provider.
- Visibility does not force runtime reflection fallback for services that are accessible from generated code in the same compilation.

## Verify Locally

```powershell
dotnet build samples\Skywalker.Sample.InternalServices\Skywalker.Sample.InternalServices.csproj
dotnet run --project samples\Skywalker.Sample.InternalServices\Skywalker.Sample.InternalServices.csproj --no-build
```
