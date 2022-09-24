// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }
}

public class Username : AggregateRoot<int>
{
    public string Name { get; set; }
}


public class Schoole : AggregateRoot
{
    public int Id { get; set; }

    public override object[] GetKeys() => new[] { "1", "2" };
}
