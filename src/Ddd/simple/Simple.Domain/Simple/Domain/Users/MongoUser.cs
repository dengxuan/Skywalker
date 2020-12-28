using Skywalker.Domain.Entities;
using System;

namespace Simple.Domain.Users
{
    public class UserValue : AggregateRoot<int>
    {
        public string Value { get; set; }
    }

    public class UserOrder : AggregateRoot<int>
    {
        public int Amount { get; set; }

        public UserValue UserValue { get; set; }
    }
}
