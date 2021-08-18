using Skywalker.Splider.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Splider.Scheduler.Component
{
    /// <summary>
    /// 通过哈希去重
    /// </summary>
    public class HashSetDuplicateRemover : IDuplicateRemover
    {
        private bool disposedValue = false;
        private string _spiderId = string.Empty;
        private readonly HashSet<string> _sets = new();

        /// <summary>
        /// Check whether the request is duplicate.
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Whether the request is duplicate.</returns>
        public Task<bool> IsDuplicateAsync(Request request)
        {
            return Task.Run(() =>
            {
                request.NotNull(nameof(request));
                request.Owner.NotNullOrWhiteSpace(nameof(request.Owner));

                if (request.Owner != _spiderId)
                {
                    throw new SpiderException("请求所属爬虫的标识与去重器所属的爬虫标识不一致");
                }
                lock (this)
                {
                    bool isDuplicate = _sets.Add(request.Hash);
                    return !isDuplicate;
                }
            });

        }

        public Task InitializeAsync(string spiderId)
        {
            return Task.Run(() =>
            {
                spiderId.NotNullOrWhiteSpace(nameof(spiderId));
                _spiderId = spiderId;
            });
        }

        public Task<long> GetTotalAsync()
        {
            return Task.Run(() =>
            {
                lock (this)
                {
                    return (long)_sets.Count;
                }
            });
        }

        /// <summary>
        /// 重置去重器
        /// </summary>
        public Task ResetDuplicateCheckAsync()
        {
            return Task.Run(() =>
            {
                lock (this)
                {
                    _sets.Clear();
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sets.Clear();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ///  Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        /// </summary>
        ~HashSetDuplicateRemover()
        {
            Dispose(disposing: false);
        }
    }
}