using System;

namespace Skywalker.Scheduler
{
    public class WorkerStateChangedEventArgs : EventArgs
    {
        public string ActionName { get; }

        public WorkerState State { get; }

        public Exception Exception { get; }

        public WorkerStateChangedEventArgs(string actionName, WorkerState state) : this(actionName, state, null) { }

        public WorkerStateChangedEventArgs(string actionName, WorkerState state, Exception exception)
        {
            if(string.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentNullException("actionName");
            }

            ActionName = actionName.Trim();
            State = state;
            Exception = exception;
        }
    }
}
