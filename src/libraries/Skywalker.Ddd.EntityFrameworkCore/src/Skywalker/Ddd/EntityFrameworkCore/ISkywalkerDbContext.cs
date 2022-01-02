// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.EntityFrameworkCore;

public interface ISkywalkerDbContext
{
    void Initialize(IUnitOfWork unitOfWork);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
}