using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.Domain.Tests.Repositories;

public class TestEntity : Entity<Guid>
{
    public TestEntity() : this(Guid.NewGuid(), string.Empty) { }

    public TestEntity(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Name { get; set; }
}

