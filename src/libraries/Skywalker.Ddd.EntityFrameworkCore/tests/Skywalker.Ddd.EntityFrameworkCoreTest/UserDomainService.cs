// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using System.CodeDom.Compiler;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Ddd.EntityFrameworkCoreTest.Domain.Entities;
using Skywalker.Ddd.Uow;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public class UserDomainService : DomainService<User>, IUserDomainService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UserDomainService(IRepository<User, Guid> repository, IAsyncQueryableExecuter asyncExecuter, IUnitOfWorkManager unitOfWorkManager) : base(repository, asyncExecuter)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task<User> GetEnabledAsync()
    {
        using var uow = _unitOfWorkManager.Begin();
        var user = new User();
        await uow.CompleteAsync();
        return user;
    }

    public User GetUser()
    {
        using var uow = _unitOfWorkManager.Begin();
        var user = new User();
        uow.CompleteAsync().GetAwaiter().GetResult();
        return user;
    }

    public Task SetEnabledAsync() => throw new NotImplementedException();
    public void SetUser() => throw new NotImplementedException();
}
