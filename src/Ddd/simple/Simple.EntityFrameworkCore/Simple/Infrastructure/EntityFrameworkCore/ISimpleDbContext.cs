using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;

namespace Simple.Infrastructure.EntityFrameworkCore
{
    public interface ISimpleDbContext
    {
        DbSet<User>? Users { get; set; }
    }
}
