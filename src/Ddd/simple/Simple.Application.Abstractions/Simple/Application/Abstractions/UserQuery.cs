using Skywalker.Application.Dtos.Contracts;

namespace Simple.Application.Users
{
    public class UserQuery : IEntityDto
    {
        public string? Name { get; set; }
    }
}
