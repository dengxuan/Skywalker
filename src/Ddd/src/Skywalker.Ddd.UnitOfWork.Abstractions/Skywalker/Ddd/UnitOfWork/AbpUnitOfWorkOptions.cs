using Skywalker.Ddd.UnitOfWork.Abstractions;
using System.Data;

namespace Skywalker.Ddd.UnitOfWork
{
    public class AbpUnitOfWorkOptions : IAbpUnitOfWorkOptions
    {
        /// <summary>
        /// Default: false.
        /// </summary>
        public bool IsTransactional { get; set; }

        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Milliseconds
        /// </summary>
        public int? Timeout { get; set; }

        public AbpUnitOfWorkOptions()
        {

        }

        public AbpUnitOfWorkOptions(bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
        {
            IsTransactional = isTransactional;
            IsolationLevel = isolationLevel;
            Timeout = timeout;
        }

        public AbpUnitOfWorkOptions Clone()
        {
            return new AbpUnitOfWorkOptions
            {
                IsTransactional = IsTransactional,
                IsolationLevel = IsolationLevel,
                Timeout = Timeout
            };
        }
    }
}
