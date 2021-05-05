using Skywalker.Application.Dtos.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Simple.Application.Abstractions
{
    public class UserOutputDto : EntityDto<Guid>
    {
        [NotNull]
        public string? Name { get; set; }

        public List<UserOrderDto>? UserOrders { get; set; }

        public UserOutputDto([NotNull] string name)
        {
            Name = name;
        }
    }

    public class UserOrderDto:EntityDto<int>
    {
        public int? Amount { get; set; }

        public List<UserValueDto>? UserValues { get; set; }

    }

    public class UserValueDto : EntityDto
    {
        public string? Value { get; set; }

    }
}
