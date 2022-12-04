using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace Skywalker.Extensions.Snowflake;

/// <summary>
/// ѩ���㷨
/// </summary>
internal class SnowflakeGenerator : ISnowflakeGenerator
{

    private static readonly object s_locker = new();

    private readonly SnowflakeGeneratorOptions _options;

    private ushort? _currentWorkerId;

    private long _latestIndex = 0;
    private long _latestTimestamp = -1L;

    private readonly int _maxIndex;
    private readonly int _indexLength;
    private readonly int _workerIdLength;

    private readonly long _maxWorkerId;


    private readonly IWorker _distributedSupport;

    public SnowflakeGenerator(IWorker  distributedSupport, IOptions<SnowflakeGeneratorOptions> options)
    {
        _distributedSupport = distributedSupport;
        _options = options.Value;
        _workerIdLength = _options.WorkerIdLength;
        _maxWorkerId = 1 << _workerIdLength;
        _indexLength = 25 - _workerIdLength;
        _maxIndex = 1 << _indexLength;
    }

    private long Timestamp(long latestTimestamp = 0L)
    {
        var now = DateTime.Now;

        //��12λ��֧�ֵ�4095��
        var year = (long)now.Year << 51;

        //�·�4λ
        var mouth = (long)now.Month << 47;

        //��5λ
        var day = (long)now.Day << 42;

        //ʱ5λ
        var hour = (long)now.Hour << 37;

        //��6λ
        var minute = (long)now.Minute << 31;

        //��6λ
        var second = (long)now.Second << 25;

        var current = year | mouth | day | hour | minute | second;

        if (latestTimestamp == current)
        {
            return Timestamp(latestTimestamp);
        }
        return current;
    }

#if NET
    [MemberNotNull("_currentWorkerId")]
#endif
    protected void RefreshWorkerId()
    {
        _currentWorkerId = _distributedSupport.NextWorkerId();
    }

    public long Create()
    {
        if (_currentWorkerId > _maxWorkerId)
        {
            throw new ArgumentOutOfRangeException($"The machine number ranges from 0 to {_maxWorkerId}");
        }

        lock (s_locker)
        {
            if(_currentWorkerId == null)
            {
                RefreshWorkerId();
            }
            var currentTimestamp = Timestamp();
            if (_latestIndex > _maxIndex)
            {
                currentTimestamp = Timestamp(_latestTimestamp);
            }
            if (currentTimestamp > _latestTimestamp)
            {
                _latestIndex = 0;
                _latestTimestamp = currentTimestamp;
            }
            else if (currentTimestamp < _latestTimestamp)
            {
                RefreshWorkerId();
                return Create();
            }
            return currentTimestamp | (long)_currentWorkerId! << _indexLength | _latestIndex++;
        }
    }
}
