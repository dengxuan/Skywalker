using Skywalker.Domain.Entities;
using System;

namespace Simple.Domain.Users
{
    public class User : AggregateRoot<short>
    {
        public string? Name { get; set; }

        public User(short id)
        {
            Id = id;
        }
    }
}
