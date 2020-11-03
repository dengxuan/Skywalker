using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.EntityFrameworkCore;

namespace Simple.EntityFrameworkCore.Weixin.EntityFrameworkCore
{
    [ConnectionStringName("Simple")]
    public interface ISimpleDbContext : IEfCoreDbContext
    {
        DbSet<User> Users{ get; set; }
    }
}
