namespace Skywalker.Extensions.Linq.Parser
{
    interface IKeywordsHelper
    {
        bool TryGetValue(string name, out object type);
    }
}
