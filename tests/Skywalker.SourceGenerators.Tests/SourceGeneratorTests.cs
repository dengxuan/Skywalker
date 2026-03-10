using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.SourceGenerators.Tests;

/// <summary>
/// Tests for the SkywalkerModuleGenerator Source Generator.
/// Validates that the generated AddSkywalker() method exists and works correctly.
/// </summary>
public class SourceGeneratorTests
{
    [Fact]
    public void Generator_GeneratesAddSkywalkerMethod()
    {
        // The fact that this test compiles proves the generator works
        // The AddSkywalker method is generated at compile time

        var methodInfo = typeof(SkywalkerServiceCollectionExtensions)
            .GetMethod("AddSkywalker", BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(methodInfo);
        Assert.True(methodInfo.IsStatic);
        Assert.True(methodInfo.IsPublic);
    }

    [Fact]
    public void Generator_MethodHasCorrectSignature()
    {
        // Arrange
        var methodInfo = typeof(SkywalkerServiceCollectionExtensions)
            .GetMethod("AddSkywalker", BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Act
        var parameters = methodInfo.GetParameters();
        var returnType = methodInfo.ReturnType;

        // Assert
        Assert.Single(parameters);
        Assert.Equal("services", parameters[0].Name);
        Assert.True(typeof(IServiceCollection).IsAssignableFrom(returnType));
    }

    [Fact]
    public void Generator_IsExtensionMethod()
    {
        // Arrange
        var methodInfo = typeof(SkywalkerServiceCollectionExtensions)
            .GetMethod("AddSkywalker", BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Act
        var isExtension = methodInfo.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false);

        // Assert
        Assert.True(isExtension);
    }

    [Fact]
    public void Generator_GeneratedClassInCorrectNamespace()
    {
        // Arrange & Act
        var type = typeof(SkywalkerServiceCollectionExtensions);

        // Assert
        Assert.Equal("Microsoft.Extensions.DependencyInjection", type.Namespace);
    }

    [Fact]
    public void Generator_GeneratedClassIsStatic()
    {
        // Arrange & Act
        var type = typeof(SkywalkerServiceCollectionExtensions);

        // Assert
        Assert.True(type.IsAbstract && type.IsSealed); // Static class
    }

    [Fact]
    public void Generator_MethodCanBeInvokedViaReflection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var methodInfo = typeof(SkywalkerServiceCollectionExtensions)
            .GetMethod("AddSkywalker", BindingFlags.Public | BindingFlags.Static);

        Assert.NotNull(methodInfo);

        // Act
        var result = methodInfo.Invoke(null, new object[] { services });

        // Assert
        Assert.Same(services, result);
    }
}

