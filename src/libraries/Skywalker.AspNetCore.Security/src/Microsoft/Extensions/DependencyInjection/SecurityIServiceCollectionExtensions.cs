using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.AspNetCore.Security;
using Skywalker.AspNetCore.Security.Abstractions;
using Skywalker.Security.Cryptography;
using Skywalker.Security.Cryptography.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
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
