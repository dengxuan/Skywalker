using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skywalker.Data;
using Skywalker.Domain.Entities;
using System;

namespace Skywalker.Ddd.EntityFrameworkCore.Modeling
{
    public static class SkywalkerEntityTypeBuilderExtensions
    {
        public static void ConfigureByConvention(this EntityTypeBuilder b)
        {
            b.TryConfigureConcurrencyStamp();
            b.TryConfigureCreationTime();
            b.TryConfigureDeletionEntity();
            b.TryConfigureModificationEntity();
        }

        public static void TryConfigureConcurrencyStamp(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IHasConcurrencyStamp>())
            {
                b.Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
                    .IsConcurrencyToken()
                    .HasMaxLength(ConcurrencyStampConsts.MaxLength)
                    .HasColumnName(nameof(IHasConcurrencyStamp.ConcurrencyStamp));
            }
        }

        public static void TryConfigureCreationTime(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IHasCreationTime>())
            {
                b.Property(nameof(IHasCreationTime.CreationTime))
                    .IsRequired()
                    .HasColumnName(nameof(IHasCreationTime.CreationTime));
            }
        }

        public static void TryConfigureDeletionEntity(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IDeletionEntity>())
            {
                b.TryConfigureDeleteable();

                b.Property(nameof(IDeletionEntity.DeleterId))
                    .IsRequired(false)
                    .HasColumnName(nameof(IDeletionEntity.DeleterId));

                b.Property(nameof(IDeletionEntity.DeletionTime))
                    .IsRequired(false)
                    .HasColumnName(nameof(IDeletionEntity.DeletionTime));
            }
        }

        public static void TryConfigureDeleteable(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IDeleteable>())
            {
                b.Property(nameof(IDeleteable.IsDeleted))
                    .IsRequired()
                    .HasDefaultValue(false)
                    .HasColumnName(nameof(IDeleteable.IsDeleted));
            }
        }

        public static void TryConfigureModificationEntity(this EntityTypeBuilder b)
        {
            if (b.Metadata.ClrType.IsAssignableTo<IModificationEntity>())
            {
                b.Property(nameof(IModificationEntity.LastModifierId))
                    .IsRequired(false)
                    .HasColumnName(nameof(IModificationEntity.LastModifierId));

                b.Property(nameof(IModificationEntity.LastModificationTime))
                    .IsRequired(false)
                    .HasColumnName(nameof(IModificationEntity.LastModificationTime));
            }
        }
    }
}
