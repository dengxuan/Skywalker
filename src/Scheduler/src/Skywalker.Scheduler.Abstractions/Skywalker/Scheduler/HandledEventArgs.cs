using Skywalker.Scheduler.Abstractions;
using System;

namespace Skywalker.Scheduler
{
    public class HandledEventArgs : EventArgs
    {
        public HandledEventArgs(IHandler handler, IHandlerContext context, Exception exception)
        {
            this.Handler = handler;
            this.Context = context;
            this.Exception = exception;
        }

        public IHandler Handler { get; }

        public ITrigger Trigger { get { return this.Context.Trigger; } }

        public IHandlerContext Context { get; }

        public Exception Exception { get; }
    }
}
