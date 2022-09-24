using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skywalker.Extensions.DependencyInjection;
using Skywalker.Identifier.Abstractions;

namespace Skywalker.Identifier;

/// <summary>
/// ѩ���㷨
/// </summary>
public class SnowflakeGenerator : IIdentifierGenerator<long>, ISingletonDependency
{

    private static readonly object s_locker = new();

    private readonly IdentifierGeneratorOptions _options;

    private long _latestTimestamp = -1L;

    private long _latestIndex = 0;

    private readonly int _workIdLength;
    private readonly long _maxWorkId;

    private readonly int _indexLength;

    private readonly int _maxIndex;

    private ushort? _workId;

    private readonly IServiceProvider _serviceProvider;

    public SnowflakeGenerator(IServiceProvider serviceProvider, IOptions<IdentifierGeneratorOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _workIdLength = _options.WorkIdLength;
        _maxWorkId = 1 << _workIdLength;
        _indexLength = 25 - _workIdLength;
        _maxIndex = 1 << _indexLength;
    }

    private async Task Initialize()
    {
        var distributed = _serviceProvider.GetRequiredService<IDistributedSupport>();
        if (distributed != null)
        {
            _workId = await distributed.GetNextMechineId();
        }
        else
        {
            _workId = _options.WorkId;
        }
    }

    private long Timestamp(long latestTimestamp = 0L)
    {
        var now = DateTime.Now;

        //��12λ��֧�ֵ�4095��
        var year = ((long)now.Year) << 51;

        //�·�4λ
        var mouth = ((long)now.Month) << 47;

        //��5λ
        var day = ((long)now.Day) << 42;

        //ʱ5λ
        var hour = ((long)now.Hour) << 37;

        //��6λ
        var minute = ((long)now.Minute) << 31;

        //��6λ
        var second = ((long)now.Second) << 25;

        var current = year | mouth | day | hour | minute | second;

        if (latestTimestamp == current)
        {
            return Timestamp(latestTimestamp);
        }
        return current;
    }

    public long Generate()
    {
        if (_workId > _maxWorkId)
        {
            throw new ArgumentOutOfRangeException($"The machine number ranges from 0 to {_maxWorkId}");
        }

        lock (s_locker)
        {
            if (_workId == null)
            {
                Initialize().Wait();
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
                Initialize().Wait();
                return Generate();
            }
            return currentTimestamp | (long)_workId!.Value << _indexLength | _latestIndex++;
        }
    }
}
