using System;

namespace Skywalker.SourceGenerators.Tests.Sample;

/// <summary>
/// Marker attribute for <see cref="HelloGenerator"/> — applied to a partial class so the
/// generator emits a <c>Hello()</c> method into it. Lives in the test project to demonstrate
/// the infrastructure; not shipped as a public API.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class HelloAttribute : Attribute
{
}
