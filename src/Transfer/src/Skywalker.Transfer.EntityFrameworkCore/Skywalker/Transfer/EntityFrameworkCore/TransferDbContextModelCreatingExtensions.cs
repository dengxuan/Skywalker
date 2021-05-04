using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.Modeling;
using Skywalker.Transfer.Domain;
using Skywalker.Transfer.Domain.Merchants;
using Skywalker.Transfer.Domain.TradeOrders;
using Skywalker.Transfer.Domain.TradeUsers;
using Skywalker.Transfer.Domain.TransferDetails;
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

                b.Property(x => x.Scheme).HasMaxLength(TransferConsts.Validations.MaxMerchantSchemeLength).IsRequired();
                b.Property(x => x.Number).HasMaxLength(TransferConsts.Validations.MaxMerchantNumberLength).IsRequired();
                b.Property(x => x.Name).HasMaxLength(TransferConsts.Validations.MaxMerchantNameLength).IsRequired();
                b.Property(x => x.Description).HasMaxLength(TransferConsts.Validations.MaxMerchantDescriptionLength).IsRequired();
                b.Property(x => x.CipherKey).HasMaxLength(TransferConsts.Validations.MaxMerchantCipherKeyLength).IsRequired();
                b.Property(x => x.Address).HasMaxLength(TransferConsts.Validations.MaxMerchantAddressLength).IsRequired();
                b.Property(x => x.MerchantType).IsRequired();

                b.HasMany(x => x.TradeOrders).WithOne(x => x.Merchant);

                b.HasKey(x => new { x.Id, x.Scheme, x.Number });
            });

            builder.Entity<TradeOrder>(b =>
            {
                b.ToTable(options.TablePrefix + "TradeOrders", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Amount).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.HandlingFee).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.Withholding).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.TradeOrderType).IsRequired();
                b.Property(x => x.TradeAuditedType).IsRequired();
                b.Property(x => x.RevokeTime);
                b.Property(x => x.RevokeReason).HasMaxLength(TransferConsts.Validations.MaxTradeOrderRevokeReasonLength);

                b.HasOne(x => x.Merchant).WithMany(x => x.TradeOrders);
                b.HasOne(x => x.Trader).WithMany(x => x.TradeOrders);

            });

            builder.Entity<Trader>(b =>
            {
                b.ToTable(options.TablePrefix + "Traders", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Balance).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.TraderType).IsRequired();

                b.HasMany(x => x.TradeOrders).WithOne(x => x.Trader);
                b.HasMany(x => x.TransferDetails).WithOne(x => x.Trader);

            });


            builder.Entity<TransferDetail>(b =>
            {
                b.ToTable(options.TablePrefix + "TransferDetails", options.Schema);

                b.ConfigureByConvention();

                b.Property(x => x.Amount).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.Balance).HasPrecision(18, 4).IsRequired();
                b.Property(x => x.TransferType).IsRequired();
                b.Property(x => x.Message).HasMaxLength(TransferConsts.Validations.MaxTransferDetailMessageLength).IsRequired();

                b.HasOne(x => x.Trader).WithMany(x => x.TransferDetails);

            });
        }
    }
}
