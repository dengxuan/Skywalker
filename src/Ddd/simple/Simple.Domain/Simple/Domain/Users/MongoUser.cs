using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class UserValue
    {
        public string Value { get; set; }

        public UserValue(string value)
        {
            Value = value;
        }
    }
}
