// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace DependencyInjectionTest;

public class TestClass: TestInterface
{
}

public class TestService :TestClass
{
}

public interface TestInterface: ISingletonDependency { }
