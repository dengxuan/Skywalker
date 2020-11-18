using Skywalker.Domain.Entities;

namespace Simple.Domain.Users
{
    public class User : AggregateRoot<short>
    {
        public string? Name { get; set; }

        public virtual UserValue UserValue { get; set; }
    }
}
