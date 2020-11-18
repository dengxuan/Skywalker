using Skywalker.Application.Dtos.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Simple.Application.Abstractions
{
    public class UserDto : EntityDto
    {
        [NotNull]
        public string Name { get; set; }

        public UserValueDto UserValue { get; set; }

        public UserDto([NotNull] string name)
        {
            Name = name;
        }
    }

    public class UserValueDto : EntityDto
    {
        public string Value { get; set; }

    }
}
