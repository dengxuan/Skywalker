using Skywalker.Ddd.ExceptionHandling;
using System.Data;

namespace Skywalker.Ddd.Uow;

//TODO: Implement default options!

/// <summary>
/// Global (default) unit of work options
/// </summary>
public class AbpUnitOfWorkDefaultOptions
{
    /// <summary>
    /// Default value: <see cref="UnitOfWorkTransactionBehavior.Auto"/>.
    /// </summary>
    public UnitOfWorkTransactionBehavior TransactionBehavior { get; set; } = UnitOfWorkTransactionBehavior.Auto;

    public IsolationLevel? IsolationLevel { get; set; }

    public int? Timeout { get; set; }

    internal AbpUnitOfWorkOptions Normalize(AbpUnitOfWorkOptions options)
    {
        if (options.IsolationLevel == null)
        {
            options.IsolationLevel = IsolationLevel;
        }

        if (options.Timeout == null)
        {
            options.Timeout = Timeout;
        }

        return options;
    }

    public bool CalculateIsTransactional(bool autoValue)
    {
        return TransactionBehavior switch
        {
            UnitOfWorkTransactionBehavior.Enabled => true,
            UnitOfWorkTransactionBehavior.Disabled => false,
            UnitOfWorkTransactionBehavior.Auto => autoValue,
            _ => throw new SkywalkerException("Not implemented TransactionBehavior value: " + TransactionBehavior),
        };
    }
}
