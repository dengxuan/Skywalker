using Skywalker.Extensions.WheelTimer;
using Skywalker.Spider.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Skywalker.Spider;

public class InProgressRequests : IDisposable
{
    private readonly ConcurrentDictionary<string, Request> _requests;
    private readonly HashedWheelTimer _timer;
    private ConcurrentBag<Request> _timeoutRequests;

    public InProgressRequests()
    {
        _requests = new ConcurrentDictionary<string, Request>();
        _timeoutRequests = new ConcurrentBag<Request>();
        _timer = new HashedWheelTimer(TimeSpan.FromSeconds(1), 100000);
    }

    public int Count => _requests.Count;

    public bool Enqueue(Request request)
    {
        if (request.Timeout < 2000)
        {
            throw new SpiderException("Timeout should not less than 2000 milliseconds");
        }

        if (!_requests.TryAdd(request.Hash, request))
        {
            return false;
        }

        _timer.NewTimeout(new TimeoutTask(this, request.Hash), TimeSpan.FromMilliseconds(request.Timeout));
        return true;
    }


    public Request? Dequeue(string hash)
    {
        return _requests.TryRemove(hash, out var request) ? request : null;
    }

    public Request[] GetAllTimeoutList()
    {
        var data = _timeoutRequests.ToArray();
        _timeoutRequests.Clear();
        return data;
    }

    private void Timeout(string hash)
    {
        if (_requests.TryRemove(hash, out var request))
        {
            _timeoutRequests.Add(request);
        }
    }

    private class TimeoutTask : IWheelTimerTask
    {
        private readonly string _hash;
        private readonly InProgressRequests _requestedQueue;

        public TimeoutTask(InProgressRequests requestedQueue, string hash)
        {
            _hash = hash;
            _requestedQueue = requestedQueue;
        }

        public Task RunAsync(IWheelTimeout timeout)
        {
            _requestedQueue.Timeout(_hash);
            return Task.CompletedTask;
        }
    }

    public void Dispose()
    {
        _requests.Clear();
        _timeoutRequests.Clear();
        _timer.Stop();
        _timer.Dispose();
    }
}
