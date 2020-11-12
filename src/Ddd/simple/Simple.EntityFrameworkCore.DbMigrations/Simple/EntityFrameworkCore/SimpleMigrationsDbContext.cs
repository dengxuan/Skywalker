using Microsoft.EntityFrameworkCore;
using Simple.Domain.Users;
using Simple.Infrastructure.EntityFrameworkCore;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

namespace Simple.EntityFrameworkCore
{

    [ConnectionStringName("MySQL.Simple")]
    public class SimpleMigrationsDbContext : SkywalkerDbContext<SimpleMigrationsDbContext>
    {
        public SimpleMigrationsDbContext(DbContextOptions<SimpleMigrationsDbContext> options) : base(options)
        {

        }

        public IDataCollection<User>? Users { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           builder.ConfigureSimple();
        }
    }
}