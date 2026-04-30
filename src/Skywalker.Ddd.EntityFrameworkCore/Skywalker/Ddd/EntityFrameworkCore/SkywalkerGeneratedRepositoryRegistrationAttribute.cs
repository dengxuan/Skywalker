namespace Skywalker.Ddd.EntityFrameworkCore;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class SkywalkerGeneratedRepositoryRegistrationAttribute : Attribute
{
    public SkywalkerGeneratedRepositoryRegistrationAttribute(Type dbContextType, Type registrarType, string methodName)
    {
        DbContextType = dbContextType;
        RegistrarType = registrarType;
        MethodName = methodName;
    }

    public Type DbContextType { get; }

    public Type RegistrarType { get; }

    public string MethodName { get; }
}