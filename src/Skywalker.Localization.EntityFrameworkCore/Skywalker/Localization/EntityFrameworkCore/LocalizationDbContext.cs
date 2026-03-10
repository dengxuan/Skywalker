using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Database context for localization data.
/// </summary>
public class LocalizationDbContext(DbContextOptions<LocalizationDbContext> options) 
    : SkywalkerDbContext<LocalizationDbContext>(options), ILocalizationDbContext
{
    /// <inheritdoc/>
    public virtual DbSet<LocalizationText> LocalizationTexts { get; set; } = default!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureLocalization();
    }
}

