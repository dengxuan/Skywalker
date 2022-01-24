using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{

    public static IServiceCollection AddSkywalker<TStartupModule>(this IServiceCollection services) where TStartupModule : IModular
    {
        var moduleLoader = new ModuleLoader();
        services.TryAddSingleton<IModuleLoader>(moduleLoader);
        services.AddSingleton(typeof(IObjectAccessor<>), typeof(ObjectAccessor<>));
        var moduleDescriptors = moduleLoader.LoadModules(services, typeof(TStartupModule));

        //PreConfigureServices
        foreach (var module in moduleDescriptors)
        {
            try
            {
                module.Instance.PreConfigureServices(services);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during {nameof(IPreConfigureServices.PreConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
            }
        }

        //ConfigureServices
        foreach (var module in moduleDescriptors.Where(predicate => predicate.Instance is IModular))
        {
            try
            {
                ((IModular)module.Instance).ConfigureServices(services);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during {nameof(IModular.ConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
            }
        }

        //PostConfigureServices
        foreach (var module in moduleDescriptors)
        {
            try
            {
                module.Instance.PostConfigureServices(services);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred during {nameof(IPostConfigureServices.PostConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", ex);
            }
        }

        return services;
    }
}
