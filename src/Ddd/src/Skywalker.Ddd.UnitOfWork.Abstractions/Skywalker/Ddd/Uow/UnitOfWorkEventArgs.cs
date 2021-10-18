using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public class UnitOfWorkEventArgs : EventArgs
{
    /// <summary>
    /// Reference to the unit of work related to this event.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; }

    public UnitOfWorkEventArgs(IUnitOfWork unitOfWork)
    {
        unitOfWork.NotNull(nameof(unitOfWork));

        UnitOfWork = unitOfWork;
    }
}
