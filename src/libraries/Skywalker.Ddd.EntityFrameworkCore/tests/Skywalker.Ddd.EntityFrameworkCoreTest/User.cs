// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Entities;

namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public record class User(Guid Id, string Name) : AggregateRoot<Guid>(Id);

public record class Username(int Id, string Name) : AggregateRoot<int>(Id);

public record class Schoole : AggregateRoot
{
    public int Id { get; set; }

    public Schoole(AggregateRoot original) : base(original)
    {
    }

    public override object[] GetKeys() => throw new NotImplementedException();
}
