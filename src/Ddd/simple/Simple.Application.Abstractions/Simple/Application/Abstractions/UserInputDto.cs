using Skywalker.Application.Dtos;

namespace Simple.Application.Abstractions;

public class UserInputDto : EntityDto
{
    public string Name { get; set; }

    public UserInputDto(string name)
    {
        Name = name;
    }
}
