using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Skywalker.Ddd.EntityFrameworkCore;

namespace Simple.EntityFrameworkCore
{
    public interface ISimpleDbContext : ISkywalkerDbContext
    {
        DbSet<User>? Users { get; set; }
    }
}
