using Skywalker.Ddd.Data;
using Skywalker.Ddd.Domain.Entities;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class SkywalkerEntityTypeBuilderExtensions
{
    public static void ConfigureByConvention(this EntityTypeBuilder b)
    {
        b.TryConfigureIdentity();
        b.TryConfigureConcurrencyStamp();
        b.TryConfigureCreationTime();
        b.TryConfigureModificationTime();
        b.TryConfigureDeleteable();
    }

    public static void TryConfigureIdentity(this EntityTypeBuilder b)
    {
        if (!b.Metadata.ClrType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>)))
        {
            return;
        }
        if (b.Metadata.ClrType.IsAssignableTo<IEntity<string>>())
        {
            b.Property(nameof(IEntity<string>.Id))
             .IsRequired()
             .HasMaxLength(Entity<string>.MaxIdLength)
             .HasColumnName(nameof(IEntity<string>.Id));

            b.HasKey(nameof(IEntity<string>.Id));
        }
        else
        {
            b.Property(nameof(IEntity<object>.Id))
             .IsRequired()
             .HasColumnName(nameof(IEntity<object>.Id));

            b.HasKey(nameof(IEntity<object>.Id));
        }
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
        if (b.Metadata.ClrType.IsAssignableTo<IDeletable>())
        {
            b.Property(nameof(IDeletable.IsDeleted))
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName(nameof(IDeletable.IsDeleted));

            b.Property(nameof(IDeletable.DeletionTime))
                .IsRequired(false)
                .HasColumnName(nameof(IDeletable.DeletionTime));
        }
    }
}
