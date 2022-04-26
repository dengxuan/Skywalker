using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class SkywalkerEntityTypeBuilderExtensions
{
    public static void ConfigureByConvention(this EntityTypeBuilder b)
    {
        b.TryConfigureConcurrencyStamp();
        b.TryConfigureCreationTime();
        b.TryConfigureModificationTime();
        b.TryConfigureDeleteable();
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

    public static void TryConfigureModificationTime(this EntityTypeBuilder b)
    {
        if (b.Metadata.ClrType.IsAssignableTo<IHasModificationTime>())
        {
            b.Property(nameof(IHasModificationTime.ModificationTime))
                .IsRequired(false)
                .HasColumnName(nameof(IHasModificationTime.ModificationTime));
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

            b.Property(nameof(IDeleteable.DeletionTime))
                .IsRequired(false)
                .HasColumnName(nameof(IDeleteable.DeletionTime));
        }
    }
}
