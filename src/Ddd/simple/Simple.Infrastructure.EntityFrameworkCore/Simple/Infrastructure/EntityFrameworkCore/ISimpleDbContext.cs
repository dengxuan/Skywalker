using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

namespace Simple.Infrastructure.EntityFrameworkCore
{
    [ConnectionStringName("Simple")]
    public interface ISimpleDbContext: ISkywalkerDbContext
    {
        IDataCollection<User>? Users { get; set; }
    }
}
