namespace Skywalker.Extensions.ObjectMapper;

public interface IMapFrom<in TSource>
{
    void MapFrom(TSource source);
}
