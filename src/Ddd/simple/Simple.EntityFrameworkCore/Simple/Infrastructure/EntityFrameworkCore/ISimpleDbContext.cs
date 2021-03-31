using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;

namespace Simple.EntityFrameworkCore
{
    public interface ISimpleDbContext
    {
        DbSet<User>? Users { get; set; }
    }
}
