using Microsoft.EntityFrameworkCore;
using Simple.Domain;
using Simple.Domain.Users;
using Simple.EntityFrameworkCore.Weixin.EntityFrameworkCore;
using Skywalker.Data;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Simple.EntityFrameworkCore
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
        [NotNull]
        public IDataCollection<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureWeixin();
        }
    }
}
