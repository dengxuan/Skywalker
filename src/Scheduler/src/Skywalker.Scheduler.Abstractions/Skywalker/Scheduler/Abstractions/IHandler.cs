using System;

namespace Skywalker.Scheduler.Abstractions
{
    public interface IHandler : IEquatable<IHandler>
    {
        void Handle(IHandlerContext context);
    }
}
