namespace Skywalker.Data
{
    public interface IConnectionStringResolver
    {
        string Resolve(string connectionStringName = null);
    }
}
