using Skywalker.Application.Dtos.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Simple.Application.Abstractions
{
    public class UserDto : EntityDto
    {
        [NotNull]
        public string Name { get; set; }

        public UserDto([NotNull] string name)
        {
            Name = name;
        }
    }
}
