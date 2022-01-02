// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Domain.Services;

namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public interface IUserDomainService : IDomainService<User>
{
    Task<User> GetEnabledAsync();
}
