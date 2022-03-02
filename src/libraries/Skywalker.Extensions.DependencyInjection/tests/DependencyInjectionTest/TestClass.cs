// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace DependencyInjectionTest;


[ScopedDependency]
public partial class TestClass: TestInterface
{
}

[SingletonDependency]
public class TestService : TestClass
{
}

public interface TestInterface { }
