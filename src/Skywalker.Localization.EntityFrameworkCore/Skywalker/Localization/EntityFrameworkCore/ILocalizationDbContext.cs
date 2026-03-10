using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Localization.EntityFrameworkCore.Entities;

namespace Skywalker.Localization.EntityFrameworkCore;

/// <summary>
/// Interface for the localization database context.
/// </summary>
public interface ILocalizationDbContext : ISkywalkerDbContext
{
    /// <summary>
    /// Gets the DbSet for localization texts.
    /// </summary>
    DbSet<LocalizationText> LocalizationTexts { get; set; }
}

