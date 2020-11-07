using Microsoft.EntityFrameworkCore;
using Simple.Domain;
using Simple.Domain.Users;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

namespace Simple.Infrastructure.EntityFrameworkCore
{
    [ConnectionStringName("Simple")]
    public class SimpleDbContext : SkywalkerDbContext<SimpleDbContext>, ISimpleDbContext
    {
        public SimpleDbContext(DbContextOptions<SimpleDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public static string TablePrefix { get; set; } = SimpleConsts.DefaultDbTablePrefix;

        public static string Schema { get; set; } = SimpleConsts.DefaultDbSchema;

        /// <summary>
        /// 用户
        /// </summary>
        public IDataCollection<User>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureWeixin();
        }
    }
}
