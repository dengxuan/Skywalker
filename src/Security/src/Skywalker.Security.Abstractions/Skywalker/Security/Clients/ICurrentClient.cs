namespace Skywalker.Security.Clients
{
    public interface ICurrentClient
    {
        string? Id { get; }

        bool IsAuthenticated { get; }
    }
}