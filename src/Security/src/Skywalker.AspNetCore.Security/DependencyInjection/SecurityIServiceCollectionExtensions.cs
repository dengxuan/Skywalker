using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.AspNetCore.Security.Abstractions;
using Skywalker.Extensions.Security.Cryptography;
using Skywalker.Extensions.Security.Cryptography.Abstractions;

namespace Skywalker.Extensions.AspNetCore.Security.DependencyInjection
{
    public static class SecurityIServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services.AddTransient<ICrypterFactory, CrypterFactory>();
            services.TryAddSingleton<IResponseEncrpytionProvider, ResponseEncryptionProvider>();
            return services;
        }
    }
}
