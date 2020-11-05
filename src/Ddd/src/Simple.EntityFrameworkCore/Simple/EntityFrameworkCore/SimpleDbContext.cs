using Microsoft.EntityFrameworkCore;
using Simple.Domain;
using Simple.Domain.Users;
using Simple.EntityFrameworkCore.Weixin.EntityFrameworkCore;
using Skywalker.Data;
using Skywalker.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Simple.EntityFrameworkCore
{
    [ConnectionStringName("Simple")]
    public class SimpleDbContext : EntityFrameworkCoreDbContext<SimpleDbContext>, ISimpleDbContext
    {
        public static string TablePrefix { get; set; } = SimpleConsts.DefaultDbTablePrefix;

        public static string Schema { get; set; } = SimpleConsts.DefaultDbSchema;

        /// <summary>
        /// 用户
        /// </summary>
        [NotNull]
        public DbSet<User>? Users { get; set; }

        public SimpleDbContext(DbContextOptions<SimpleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureWeixin(options =>
            {
                options.TablePrefix = TablePrefix;
                options.Schema = Schema;
            });
        }
    }
}
