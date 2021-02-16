using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Simple.Domain.Users
{
    public class User : AggregateRoot<Guid>
    {
        public string? Name { get; set; }

       public List<UserOrder>? UserOrders { get; set; }
    }
}
