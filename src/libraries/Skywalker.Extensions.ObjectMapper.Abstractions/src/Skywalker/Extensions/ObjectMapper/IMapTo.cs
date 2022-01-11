namespace Skywalker.Extensions.ObjectMapper;

public interface IMapTo<TDestination>
{
    TDestination MapTo();

    void MapTo(TDestination destination);
}
