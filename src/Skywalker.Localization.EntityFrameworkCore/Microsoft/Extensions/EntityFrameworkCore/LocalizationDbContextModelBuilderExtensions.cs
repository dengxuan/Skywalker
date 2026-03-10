using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skywalker.Localization.EntityFrameworkCore;
using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Microsoft.Extensions.EntityFrameworkCore;

/// <summary>
/// Extension methods for configuring localization entities in EF Core.
/// </summary>
public static class LocalizationDbContextModelBuilderExtensions
{
    /// <summary>
    /// Configures the localization database entities.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    /// <param name="tablePrefix">Optional table prefix.</param>
    /// <param name="schema">Optional database schema.</param>
    public static void ConfigureLocalization(
        this ModelBuilder builder, 
        string tablePrefix = "", 
        string? schema = null)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<LocalizationText>(b =>
        {
            b.ToTable($"{tablePrefix}LocalizationTexts", schema);

            b.ConfigureByConvention();

            b.Property(x => x.Id)
                .HasMaxLength(LocalizationConsts.MaxIdLength)
                .IsRequired();

            b.Property(x => x.ResourceName)
                .HasMaxLength(LocalizationConsts.MaxResourceNameLength)
                .IsRequired();

            b.Property(x => x.CultureName)
                .HasMaxLength(LocalizationConsts.MaxCultureNameLength)
                .IsRequired();

            b.Property(x => x.Key)
                .HasMaxLength(LocalizationConsts.MaxKeyLength)
                .IsRequired();

            b.Property(x => x.Value)
                .HasMaxLength(LocalizationConsts.MaxValueLength)
                .IsRequired();

            // Create a unique index on ResourceName, CultureName, and Key
            b.HasIndex(x => new { x.ResourceName, x.CultureName, x.Key })
                .IsUnique();

            // Create an index on ResourceName and CultureName for faster queries
            b.HasIndex(x => new { x.ResourceName, x.CultureName });
        });
    }
}

