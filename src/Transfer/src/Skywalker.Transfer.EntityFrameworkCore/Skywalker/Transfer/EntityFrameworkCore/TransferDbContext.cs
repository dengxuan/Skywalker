using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Transfer.Domain;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.TradeUsers;

namespace Skywalker.Transfer.EntityFrameworkCore
{
    public class TransferDbContext : SkywalkerDbContext<TransferDbContext>
    {
        public static string TablePrefix { get; set; } = TransferConsts.DefaultDbTablePrefix;

        public static string Schema { get; set; } = TransferConsts.DefaultDbSchema;

        public DbSet<Merchant>? Merchants { get; set; }

        public DbSet<TradeOrder<ITrader>>? TradeOrders { get; set; }

        public DbSet<Trader>? Traders { get; set; }

        protected TransferDbContext(DbContextOptions<TransferDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureTransfer();
        }
    }
}
