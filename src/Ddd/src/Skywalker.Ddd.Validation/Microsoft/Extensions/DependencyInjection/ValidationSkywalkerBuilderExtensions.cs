using Skywalker.Ddd;
using Skywalker.Validation;

namespace Microsoft.Extensions.DependencyInjection;

internal static class ValidationSkywalkerBuilderExtensions
{
    public static SkywalkerDddBuilder AddValidation(SkywalkerDddBuilder builder)
    {
        builder.Services.AddTransient<IObjectValidationContributor, DataAnnotationObjectValidationContributor>();
        builder.Services.AddTransient<IMethodInvocationValidator, MethodInvocationValidator>();
        builder.Services.AddTransient<IObjectValidator, ObjectValidator>();
        return builder;
    }
}
