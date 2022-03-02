using DependencyInjectionTest;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.DependencyInjection;
Console.WriteLine();
var services = new ServiceCollection();

services.AddSkywalker();

var sp = services.BuildServiceProvider();
var testInterface = sp.GetRequiredService<TestInterface>();
Console.WriteLine(services.Count);
