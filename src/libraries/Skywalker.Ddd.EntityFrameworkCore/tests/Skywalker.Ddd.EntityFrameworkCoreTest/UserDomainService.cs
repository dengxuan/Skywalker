// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Domain.Repositories;
using Skywalker.Ddd.Domain.Services;
using Skywalker.Extensions.Linq;

namespace Skywalker.Ddd.EntityFrameworkCoreTest;

public class UserDomainService : DomainService<User>, IUserDomainService
{
    public UserDomainService(IRepository<User, Guid> repository, IAsyncQueryableExecuter asyncExecuter) : base(repository, asyncExecuter)
    {
    }

    public Task<User> GetEnabledAsync() => throw new NotImplementedException();
}

public static class UserDomainServiceExtensions
{
    public static void ConfigureTestDbContext(this DomainServiceBuilder builder)
    {
        DomainServiceBuilder.ConfigureServices(services =>
        {
            services.AddTransient<IUserDomainService, UserDomainService>();
        });
    }
}
