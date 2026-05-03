# DynamicProxy Source Generator Contract

> Status: Draft for v2.0.0-preview.4  
> Tracking: #253, #264

## Goal

v2.0 replaces Castle.DynamicProxy runtime proxy creation with compile-time static
proxy generation. The generator must preserve the existing interceptor programming
model where possible while removing runtime IL emit, hidden reflection-based proxy
construction, and the Castle.Core package dependency from the supported path.

The first implementation target is deliberately narrow: generated interface proxies
for services already registered in dependency injection and marked as interceptable.
Class proxy replacement is a compatibility boundary, not a preview.4 requirement.

## Existing Contract

The current runtime model has these public pieces:

- `IInterceptable` marks services that should be proxied.
- `IInterceptor` implements `Task InterceptAsync(IMethodInvocation invocation)`.
- `IMethodInvocation` exposes target, `MethodInfo`, arguments, return type, return
  value, and `ProceedAsync()`.
- `AddInterceptedServices()` scans registered services, finds implementations that
  implement `IInterceptable`, and replaces interface registrations with Castle
  interface proxies.
- `IProxyGenerator` and `CastleProxyGenerator` expose runtime proxy creation APIs.

The source-generator path should keep `IInterceptor` and `IMethodInvocation` as the
interceptor authoring surface for v2.0. The compatibility status of `IProxyGenerator`
is different: it is a Castle-era runtime factory API and should not be the primary
v2.0 extension point.

## Generated Proxy Shape

For each supported service registration, the generator emits a sealed proxy class in
the consumer assembly. The proxy:

- Implements the service interface.
- Receives the concrete target service and interceptors through DI.
- Preserves the target service lifetime by replacing the service interface
  registration only, while keeping the implementation available for proxy creation.
- Creates an `IMethodInvocation` context for each intercepted method.
- Executes registered interceptors in registration order.
- Calls the concrete target when the interceptor chain proceeds.

Generated names must be deterministic, collision-resistant, and stable across OS path
separators. Nested and generic type names must be fully encoded in generated metadata
and source hint names.

## Supported Shapes

Preview.4 supports these shapes:

- Interface service registrations where the implementation type implements
  `IInterceptable`.
- Public and internal implementation types in the consumer assembly.
- Instance interface methods returning `void`, `T`, `Task`, `Task<T>`, `ValueTask`,
  or `ValueTask<T>`.
- Method overloads.
- Closed generic service registrations.
- Generic implementation types when the generated proxy can be emitted as a matching
  generic type without runtime type construction.

The generator may support generic methods if it can avoid per-call reflective method
lookup. If generic method support requires reflection or `MakeGenericMethod` in the
hot path, it should be deferred behind a diagnostic.

## Unsupported Shapes

Unsupported shapes must produce diagnostics instead of silently falling back to Castle
or failing at runtime. Preview.4 should reject or defer:

- Class-only proxying without a service interface.
- `ref`, `out`, or `in` parameters.
- `ref struct`, pointer, function pointer, and unsafe signatures.
- Static abstract interface members and static methods.
- Default interface method bodies that require proxy-side dispatch semantics.
- Open generic service registrations that cannot be represented by a generated open
  generic proxy type.
- Services registered only by factory or instance when the implementation type cannot
  be determined at compile time.

## Sync And Async Semantics

The Castle adapter currently bridges all interception through async code and uses
sync-over-async for synchronous methods. The generated path should not keep that
behavior as a default implementation detail.

The preferred v2.0 rule is:

- Async target methods await the interceptor chain normally.
- Synchronous target methods execute synchronously until an interceptor needs async
  work. If the existing `IInterceptor` async-only contract makes a purely synchronous
  chain impossible, the generated proxy should isolate the sync-over-async behavior
  in one well-documented helper and emit a diagnostic or documentation warning for
  synchronous intercepted methods.

A future strongly typed interceptor API may add separate sync and async interceptor
contracts. That is allowed as a follow-up, but preview.4 should first preserve the
current `IInterceptor` model so migration is possible.

## Interceptor Selection

Preview.4 may continue to run all registered `IInterceptor` instances for every
intercepted method, matching current behavior. Interceptors remain responsible for
checking method or type attributes such as unit-of-work attributes.

The generator should leave room for compile-time interceptor filtering later. If
attribute-based filtering is added, it must not remove an interceptor that depends on
runtime state unless the interceptor explicitly opts into compile-time filtering.

## Performance Rules

Generated proxies should avoid repeating expensive work per call:

- Do not rebuild the interceptor pipeline for every invocation when the interceptor
  set is stable for the proxy instance.
- Do not use Castle, `DispatchProxy`, runtime IL emit, or dynamic type generation.
- Do not use per-call method discovery with `GetMethods().First(...)`.
- Cache `MethodInfo` values in generated static fields when `IMethodInvocation`
  requires them.
- Keep argument arrays limited to calls that actually enter interception.

## Castle Compatibility Boundary

Castle.Core was a transition dependency until generated interface proxies, tests,
and samples were in place. The v2.0 supported path is generated static proxies.

Compatibility policy:

- Existing `IInterceptor` implementations remain source-compatible where possible.
- Existing `IInterceptable` markers remain the initial discovery signal.
- `AddInterceptedServices()` may become a generated-registration bridge or a
  compatibility wrapper around generated metadata.
- Runtime calls to `IProxyGenerator`/`CastleProxyGenerator` are removed in the v2.0
  path. Existing users should migrate to interface registrations that are covered by
  the DynamicProxy source generator.
- No NativeAOT path may require Castle.Core, runtime IL emit, or dynamic proxy
  construction.

## Diagnostics

DynamicProxy diagnostics should use stable SKY3xxx IDs after the EF repository
generator range. At minimum the generator needs diagnostics for:

- Unsupported service registration shape.
- Missing implementation type for an intercepted service.
- Unsupported method signature.
- Generic method or open generic shape deferred from preview.4.
- Conflicting generated proxy names.
- Castle fallback required in a NativeAOT or dynamic-code-disabled scenario.

Every diagnostic added for preview.4 must have a docs page before release.

## Validation Gates

Preview.4 is not ready until these gates pass:

- DynamicProxy generator builds as an analyzer-only package.
- Snapshot tests cover at least 50 proxy-generation cases.
- Runtime tests prove interceptor ordering, proceed behavior, return values, and
  non-intercepted service behavior.
- A sample demonstrates generated proxy usage and is included in the sample matrix.
- NativeAOT publish canary has no proxy-related IL2xxx/IL3xxx warnings.
- `Castle.Core` removal has package dependency verification before the epic closes.
