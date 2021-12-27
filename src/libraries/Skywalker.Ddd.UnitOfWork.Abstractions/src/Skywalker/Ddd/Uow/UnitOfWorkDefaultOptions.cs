// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;
using Skywalker.Extensions.Exceptions;

namespace Skywalker.Ddd.Uow;

//TODO: Implement default options!

/// <summary>
/// Global (default) unit of work options
/// </summary>
public class UnitOfWorkDefaultOptions
{
    /// <summary>
    /// Default value: <see cref="UnitOfWorkTransactionBehavior.Auto"/>.
    /// </summary>
    public UnitOfWorkTransactionBehavior TransactionBehavior { get; set; } = UnitOfWorkTransactionBehavior.Auto;

    public IsolationLevel? IsolationLevel { get; set; }

    public int? Timeout { get; set; }

    internal UnitOfWorkOptions Normalize(UnitOfWorkOptions options)
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
