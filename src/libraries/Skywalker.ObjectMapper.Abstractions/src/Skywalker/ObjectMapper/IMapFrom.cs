namespace Skywalker.ObjectMapper;

public interface IMapFrom<in TSource>
{
    void MapFrom(TSource source);
}
