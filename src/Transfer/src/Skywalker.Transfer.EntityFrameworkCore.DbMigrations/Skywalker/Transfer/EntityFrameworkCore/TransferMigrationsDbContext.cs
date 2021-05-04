using Microsoft.EntityFrameworkCore;
using Skywalker.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.TradeUsers;

namespace Skywalker.Transfer.EntityFrameworkCore
{
    [ConnectionStringName("Transfer")]
    public class TransferMigrationsDbContext : SkywalkerDbContext<TransferMigrationsDbContext>
    {

        public DbSet<Merchant>? Merchants { get; set; }

        public DbSet<TradeOrder>? TradeOrders { get; set; }

        public DbSet<Trader>? Traders { get; set; }

        public TransferMigrationsDbContext(DbContextOptions<TransferMigrationsDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureTransfer();
        }
    }
}
