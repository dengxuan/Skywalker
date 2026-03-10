namespace Skywalker.Extensions.Threading;

/// <summary>
/// 
/// </summary>
public class NullCancellationTokenProvider : ICancellationTokenProvider
{
    /// <summary>
    /// 
    /// </summary>
    public static NullCancellationTokenProvider Instance { get; } = new NullCancellationTokenProvider();

    /// <summary>
    /// 
    /// </summary>
    public CancellationToken Token { get; } = CancellationToken.None;

    private NullCancellationTokenProvider()
    {

    }
}
