using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.Data.Filtering;
using Skywalker.Ddd.Domain.Entities;
using Skywalker.Ddd.Domain.Events;
using Skywalker.Ddd.EntityFrameworkCore.ValueConverters;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Ddd.EntityFrameworkCore;

public abstract class SkywalkerDbContext<TDbContext>(DbContextOptions<TDbContext> options) : DbContext(options), ISkywalkerDbContext where TDbContext : DbContext
{
    protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<IDeletable>() ?? false;

    protected IDataFilter? DataFilter { get; set; }

    protected IEntityChangeEventHelper EntityChangeEventHelper { get; set; } = NullEntityChangeEventHelper.Instance;

    protected IClock? Clock { get; set; }

    private static readonly MethodInfo s_configureBasePropertiesMethodInfo
        = typeof(SkywalkerDbContext<TDbContext>)
            .GetMethod(
                nameof(ConfigureBaseProperties),
                BindingFlags.Instance | BindingFlags.NonPublic
            )!;

    private static readonly MethodInfo s_configureValueConverterMethodInfo
        = typeof(SkywalkerDbContext<TDbContext>)
            .GetMethod(
                nameof(ConfigureValueConverter),
                BindingFlags.Instance | BindingFlags.NonPublic
            )!;

    private static readonly MethodInfo s_configureValueGeneratedMethodInfo
        = typeof(SkywalkerDbContext<TDbContext>)
            .GetMethod(
                nameof(ConfigureValueGenerated),
                BindingFlags.Instance | BindingFlags.NonPublic
            )!;

    public virtual void Initialize(IUnitOfWork unitOfWork)
    {
        if (unitOfWork.Options?.Timeout.HasValue == true && Database.IsRelationalDatabase() && !Database.GetCommandTimeout().HasValue)
        {
            Database.SetCommandTimeout(TimeSpan.FromMilliseconds(unitOfWork.Options!.Timeout!.Value));
        }

        ChangeTracker.CascadeDeleteTiming = CascadeTiming.OnSaveChanges;

        ChangeTracker.Tracked += ChangeTracker_Tracked;
    }
    protected virtual void ChangeTracker_Tracked(object? sender, EntityTrackedEventArgs e)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ReplaceService<IValueGeneratorSelector, SkywalkerValueGeneratorSelector>();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        TrySetDatabaseProvider(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!entityType.ClrType.IsAssignableTo(typeof(IEntity)))
            {
                continue;
            }
            s_configureBasePropertiesMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]);

            s_configureValueConverterMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]);

            s_configureValueGeneratedMethodInfo
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]);
        }
    }

    protected virtual void TrySetDatabaseProvider(ModelBuilder modelBuilder)
    {
        var provider = GetDatabaseProviderOrNull(modelBuilder);
        if (provider != null)
        {
            modelBuilder.SetDatabaseProvider(provider.Value);
        }
    }

    protected virtual EntityFrameworkCoreDatabaseProvider? GetDatabaseProviderOrNull(ModelBuilder modelBuilder)
    {
        return Database.ProviderName switch
        {
            "Microsoft.EntityFrameworkCore.SqlServer" => EntityFrameworkCoreDatabaseProvider.SqlServer,
            "Npgsql.EntityFrameworkCore.PostgreSQL" => EntityFrameworkCoreDatabaseProvider.PostgreSql,
            "MySQL.EntityFrameworkCore" or "Pomelo.EntityFrameworkCore.MySql" => EntityFrameworkCoreDatabaseProvider.MySql,
            "Oracle.EntityFrameworkCore" or "Devart.Data.Oracle.Entity.EFCore" => EntityFrameworkCoreDatabaseProvider.Oracle,
            "Microsoft.EntityFrameworkCore.Sqlite" => EntityFrameworkCoreDatabaseProvider.Sqlite,
            "Microsoft.EntityFrameworkCore.InMemory" => EntityFrameworkCoreDatabaseProvider.InMemory,
            "FirebirdSql.EntityFrameworkCore.Firebird" => EntityFrameworkCoreDatabaseProvider.Firebird,
            "Microsoft.EntityFrameworkCore.Cosmos" => EntityFrameworkCoreDatabaseProvider.Cosmos,
            _ => null,
        };
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            var changeReport = ApplySkywalkerConcepts();
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new SkywalkerDbConcurrencyException(ex.Message, ex);
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    protected virtual EntityChangeReport ApplySkywalkerConcepts()
    {
        var changeReport = new EntityChangeReport();

        foreach (var entry in ChangeTracker.Entries().ToList())
        {
            ApplySkywalkerConcepts(entry, changeReport);
        }

        return changeReport;
    }

    protected virtual void ApplySkywalkerConcepts(EntityEntry entry, EntityChangeReport changeReport)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                ApplySkywalkerConceptsForAddedEntity(entry, changeReport);
                break;
            case EntityState.Modified:
                ApplySkywalkerConceptsForModifiedEntity(entry, changeReport);
                break;
            case EntityState.Deleted:
                ApplySkywalkerConceptsForDeletedEntity(entry, changeReport);
                break;
        }

        AddDomainEvents(changeReport, entry.Entity);
    }

    protected virtual void ApplySkywalkerConceptsForAddedEntity(EntityEntry entry, EntityChangeReport changeReport)
    {
        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
    }

    protected virtual void ApplySkywalkerConceptsForModifiedEntity(EntityEntry entry, EntityChangeReport changeReport)
    {
        UpdateConcurrencyStamp(entry);

        if (entry.Entity is IDeletable && entry.Entity.As<IDeletable>()!.IsDeleted)
        {
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
        }
        else
        {
            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Updated));
        }
    }

    protected virtual void ApplySkywalkerConceptsForDeletedEntity(EntityEntry entry, EntityChangeReport changeReport)
    {
        if (TryCancelDeletionForSoftDelete(entry))
        {
            UpdateConcurrencyStamp(entry);
        }

        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
    }

    protected virtual bool IsHardDeleted(EntityEntry entry)
    {
        return entry is not IDeletable;
    }

    protected virtual void AddDomainEvents(EntityChangeReport changeReport, object entityAsObj)
    {
        if (entityAsObj is not IGeneratesDomainEvents generatesDomainEventsEntity)
        {
            return;
        }

        var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents()?.ToArray();
        if (distributedEvents != null && distributedEvents.Length != 0)
        {
            changeReport.DistributedEvents.AddRange(distributedEvents.Select(eventData => new DomainEventEntry(entityAsObj, eventData)));
            generatesDomainEventsEntity.ClearDistributedEvents();
        }
    }

    protected virtual void UpdateConcurrencyStamp(EntityEntry entry)
    {
        if (entry.Entity is not IHasConcurrencyStamp entity)
        {
            return;
        }

        Entry(entity).Property(x => x.ConcurrencyStamp).OriginalValue = entity.ConcurrencyStamp;
        entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
    }

    protected virtual bool TryCancelDeletionForSoftDelete(EntityEntry entry)
    {
        if (entry.Entity is not IDeletable)
        {
            return false;
        }

        if (IsHardDeleted(entry))
        {
            return false;
        }

        entry.Reload();
        entry.State = EntityState.Modified;
        entry.Entity.As<IDeletable>()!.IsDeleted = true;
        return true;
    }

    protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class, IEntity
    {
        if (mutableEntityType.IsOwned())
        {
            return;
        }

        modelBuilder.Entity<TEntity>().ConfigureByConvention();

        ConfigureGlobalFilters<TEntity>(modelBuilder, mutableEntityType);
    }

    protected virtual void ConfigureGlobalFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.BaseType == null && ShouldFilterEntity<TEntity>(mutableEntityType))
        {
            var filterExpression = CreateFilterExpression<TEntity>();
            if (filterExpression != null)
            {
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }
    }

    protected virtual void ConfigureValueConverter<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        if (mutableEntityType.BaseType == null &&
            !typeof(TEntity).IsDefined(typeof(DisableDateTimeNormalizationAttribute), true) &&
            !typeof(TEntity).IsDefined(typeof(OwnedAttribute), true) &&
            !mutableEntityType.IsOwned())
        {
            if (Clock == null || !Clock.SupportsMultipleTimezone)
            {
                return;
            }

            var dateTimeValueConverter = new SkywalkerDateTimeValueConverter(Clock);

            var dateTimePropertyInfos = typeof(TEntity).GetProperties()
                .Where(property =>
                    (property.PropertyType == typeof(DateTime) ||
                     property.PropertyType == typeof(DateTime?)) &&
                    property.CanWrite &&
                    !property.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true)
                ).ToList();

            dateTimePropertyInfos.ForEach(property =>
            {
                modelBuilder
                    .Entity<TEntity>()
                    .Property(property.Name)
                    .HasConversion(dateTimeValueConverter);
            });
        }
    }

    protected virtual void ConfigureValueGenerated<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class
    {
        var entityType = typeof(TEntity);

        // 只处理实现了 IEntity<TPrimaryKey> 的实体
        if (entityType.IsAssignableFrom(typeof(IEntity<>)))
        {
            return;
        }

        // 获取主键类型
        var primaryKeyName = nameof(IEntity<object>.Id);
        var idProperty = mutableEntityType.FindProperty(primaryKeyName);

        if (idProperty == null)
        {
            return;
        }
        var idPropertyBuilder = modelBuilder.Entity<TEntity>().Property(primaryKeyName);
        if (idPropertyBuilder.Metadata.PropertyInfo!.IsDefined(typeof(DatabaseGeneratedAttribute), true))
        {
            return;
        }
        idPropertyBuilder.ValueGeneratedOnAdd();
    }


    protected virtual bool ShouldFilterEntity<TEntity>(IMutableEntityType entityType) where TEntity : class
    {
        return typeof(IDeletable).IsAssignableFrom(typeof(TEntity));
    }

    protected virtual Expression<Func<TEntity, bool>>? CreateFilterExpression<TEntity>()
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? expression = null;

        if (typeof(IDeletable).IsAssignableFrom(typeof(TEntity)))
        {
            expression = e => !IsSoftDeleteFilterEnabled || !EF.Property<bool>(e, nameof(IDeletable.IsDeleted));
        }

        return expression;
    }

    protected virtual Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expression1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expression2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
    }

    class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node)
        {
            if (node == oldValue)
            {
                return newValue;
            }

            return base.Visit(node);
        }
    }
}
