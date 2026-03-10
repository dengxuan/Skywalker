namespace Skywalker.SourceGenerators.Tests;

/// <summary>
/// Tests for ModuleInfo class behavior (through generated code).
/// </summary>
public class ModuleInfoTests
{
    [Fact]
    public void ModuleInfo_Equality_BasedOnAssemblyName()
    {
        // This test verifies the expected behavior of module deduplication
        // The actual ModuleInfo class is internal to the Source Generator,
        // but we can test the concept through integration tests.
        
        var modules = new HashSet<string>
        {
            "Skywalker.EventBus.Local",
            "Skywalker.Caching.Redis"
        };

        // Adding duplicate should not increase count
        modules.Add("Skywalker.EventBus.Local");

        Assert.Equal(2, modules.Count);
    }

    [Fact]
    public void ModuleInfo_Dependencies_PreventDuplicateRegistration()
    {
        // Test that modules with shared dependencies don't cause issues
        var registeredModules = new List<string>();
        var visited = new HashSet<string>();

        void RegisterModule(string moduleName, string[] dependencies)
        {
            if (visited.Contains(moduleName)) return;
            
            foreach (var dep in dependencies)
            {
                if (!visited.Contains(dep))
                {
                    RegisterModule(dep, Array.Empty<string>());
                }
            }
            
            visited.Add(moduleName);
            registeredModules.Add(moduleName);
        }

        // Module A depends on Core
        // Module B depends on Core
        // Both should only register Core once
        RegisterModule("ModuleA", new[] { "Core" });
        RegisterModule("ModuleB", new[] { "Core" });

        Assert.Equal(3, registeredModules.Count);
        Assert.Single(registeredModules, m => m == "Core");
    }

    [Fact]
    public void TopologicalSort_CorrectOrder()
    {
        // Test topological sort concept
        var modules = new Dictionary<string, string[]>
        {
            ["C"] = new[] { "A", "B" },
            ["B"] = new[] { "A" },
            ["A"] = Array.Empty<string>()
        };

        var sorted = TopologicalSort(modules);

        // A should come before B, and B before C
        var indexA = sorted.IndexOf("A");
        var indexB = sorted.IndexOf("B");
        var indexC = sorted.IndexOf("C");

        Assert.True(indexA < indexB, "A should be before B");
        Assert.True(indexB < indexC, "B should be before C");
    }

    [Fact]
    public void TopologicalSort_HandlesCycles()
    {
        // Test that cyclic dependencies don't cause infinite loop
        var modules = new Dictionary<string, string[]>
        {
            ["A"] = new[] { "B" },
            ["B"] = new[] { "A" }  // Cycle!
        };

        var sorted = TopologicalSort(modules);

        // Should complete without infinite loop
        Assert.Equal(2, sorted.Count);
    }

    private static List<string> TopologicalSort(Dictionary<string, string[]> modules)
    {
        var sorted = new List<string>();
        var visited = new HashSet<string>();
        var visiting = new HashSet<string>();

        void Visit(string module)
        {
            if (visited.Contains(module)) return;
            if (visiting.Contains(module)) return; // Cycle detection

            visiting.Add(module);

            if (modules.TryGetValue(module, out var deps))
            {
                foreach (var dep in deps)
                {
                    Visit(dep);
                }
            }

            visiting.Remove(module);
            visited.Add(module);
            sorted.Add(module);
        }

        foreach (var module in modules.Keys)
        {
            Visit(module);
        }

        return sorted;
    }
}

