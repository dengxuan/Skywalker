using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

namespace Simple.Infrastructure.EntityFrameworkCore
{
    public interface ISimpleDbContext: ISkywalkerDbContext
    {
        DbSet<User>? Users { get; set; }
    }
}
