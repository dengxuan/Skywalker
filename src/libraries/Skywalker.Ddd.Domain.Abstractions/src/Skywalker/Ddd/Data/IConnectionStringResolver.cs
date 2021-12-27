namespace Skywalker.Ddd.Data
{
    public interface IConnectionStringResolver
    {
        string Resolve(string connectionStringName);
    }
}
