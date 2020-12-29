using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.EntityFrameworkCore;

namespace Simple.EntityFrameworkCore
{

    [ConnectionStringName("MySQL.Simple")]
    public class SimpleMigrationsDbContext : SkywalkerDbContext<SimpleMigrationsDbContext>
    {
        public SimpleMigrationsDbContext(DbContextOptions<SimpleMigrationsDbContext> options) : base(options)
        {

        }

        public DbSet<User>? Users { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           builder.ConfigureSimple();
        }
    }
}