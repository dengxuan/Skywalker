using Simple.Application.Abstractions;
using Skywalker.Ddd.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Application.Users
{
    public class UserQuery : IQuery<List<UserDto>>
    {
        public string Name { get; set; }
    }
}
