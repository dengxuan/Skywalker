using Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Extensions.AspNetCore.Security;
using Skywalker.Security.Cryptography;
using Skywalker.Security.Cryptography.Abstractions;

namespace DependencyInjection
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
