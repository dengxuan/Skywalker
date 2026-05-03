namespace Skywalker.DependencyInjection;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class SkywalkerGeneratedDependencyInjectionRegistrationAttribute : Attribute
{
    public SkywalkerGeneratedDependencyInjectionRegistrationAttribute(Type registrarType, string methodName)
    {
        RegistrarType = registrarType;
        MethodName = methodName;
    }

    public Type RegistrarType { get; }

    public string MethodName { get; }
}