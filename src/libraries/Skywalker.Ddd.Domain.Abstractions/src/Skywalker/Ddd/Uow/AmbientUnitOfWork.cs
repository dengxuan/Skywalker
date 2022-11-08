// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 当前线程环境下的工作单元
/// </summary>
public class AmbientUnitOfWork : IAmbientUnitOfWork
{

    private readonly AsyncLocal<IUnitOfWork?> _currentUow;

    /// <summary>
    /// 
    /// </summary>
    public IUnitOfWork? UnitOfWork => _currentUow.Value;

    /// <summary>
    /// 初始化当前线程环境下的工作单元
    /// </summary>
    public AmbientUnitOfWork()
    {
        _currentUow = new AsyncLocal<IUnitOfWork?>();
    }

    /// <summary>
    /// 设置当前线程环境下的工作单元，将其存储到AsyncLocal中
    /// </summary>
    /// <param name="unitOfWork"></param>
    public void SetUnitOfWork(IUnitOfWork? unitOfWork)
    {
        _currentUow.Value = unitOfWork;
    }
}
