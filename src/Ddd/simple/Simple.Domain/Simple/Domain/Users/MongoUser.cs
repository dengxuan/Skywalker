using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Simple.Domain.Users
{
    public class UserValue : AggregateRoot<Guid>
    {
        public string? Value { get; set; }
    }

    public class UserOrder : AggregateRoot<Guid>
    {
        public int Amount { get; set; }

        public List<UserValue>? UserValues { get; set; }
    }
}
