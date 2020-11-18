using Skywalker.Domain.Entities;
using System;

namespace Simple.Domain.Users
{
    public class MongoUser : AggregateRoot<Guid>
    {
        public string Name { get; set; }

        public UserValue? Value { get; set; }

        public MongoUser(string name)
        {
            Name = name;
        }
    }

    public class UserValue : AggregateRoot<int>
    {
        public string Value { get; set; }
    }
}
