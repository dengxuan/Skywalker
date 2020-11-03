using Skywalker.Domain.Entities;
using System;

namespace Simple.Domain.Users
{
    public class User : AggregateRoot<Guid>
    {
        public string? Name { get; set; }

        public User(Guid id)
        {
            Id = id;
        }
    }
}
