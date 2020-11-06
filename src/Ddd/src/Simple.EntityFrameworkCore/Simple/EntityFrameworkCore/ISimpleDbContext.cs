using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.EntityFrameworkCore;

namespace Simple.EntityFrameworkCore.Weixin.EntityFrameworkCore
{
    [ConnectionStringName("Simple")]
    public interface ISimpleDbContext: ISkywalkerDbContext
    {
        IDataCollection<User> Users { get; set; }
    }
}
