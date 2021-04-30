using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Modeling;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.TradeUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.EntityFrameworkCore
{
    public static class TransferDbContextModelCreatingExtensions
    {

        public static void ConfigureTransfer(this ModelBuilder builder, Action<TransferModelBuilderConfigurationOptions>? optionsAction = null)
        {

            Check.NotNull(builder, nameof(builder));

            var options = new TransferModelBuilderConfigurationOptions();

            optionsAction?.Invoke(options);

            builder.Entity<Merchant>(b =>
            {
                b.ToTable(options.TablePrefix + "Merchants", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Scheme).IsRequired();
                b.Property(x => x.Key).IsRequired();
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.Description).IsRequired();
                b.Property(x => x.CipherKey).IsRequired();
                b.Property(x => x.Address).IsRequired();
                b.Property(x => x.NotifyAddress).IsRequired();
                b.Property(x => x.MerchantType).IsRequired();

                b.HasMany(x => x.TradeOrders).WithOne(x => x.Merchant);
            });

            builder.Entity<TradeOrder<ITrader>>(b =>
            {
                b.ToTable(options.TablePrefix + "TradeOrders", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Amount).IsRequired();
                b.Property(x => x.HandlingFee).IsRequired();
                b.Property(x => x.Withholding).IsRequired();
                b.Property(x => x.TradeOrderType).IsRequired();
                b.Property(x => x.TradeAuditedTypes).IsRequired();
                b.Property(x => x.RevokeTime);
                b.Property(x => x.RevokeReason);

                b.HasOne(x => x.Merchant).WithMany(x => x.TradeOrders);
                b.HasOne(x => x.User).WithMany(x => x.TradeOrders);

            });

            builder.Entity<ITrader>(b =>
            {
                b.ToTable(options.TablePrefix + "Traders", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Balance).IsRequired();
                b.Property(x => x.TraderType).IsRequired();

                b.HasMany(x => x.TradeOrders).WithOne(x => x.User);
                b.HasMany(x => x.TransferDetails).WithOne(x => x.Trader);

            });
        }
    }
}
