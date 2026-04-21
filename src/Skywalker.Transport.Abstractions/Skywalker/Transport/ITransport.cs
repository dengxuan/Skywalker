// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Transport;

/// <summary>
/// 字节级网络传输抽象。不关心消息语义、不关心序列化。
/// 实现可以是 NetMQ 的 Pub/Sub/Router/Dealer，也可以是 InProc 测试桩。
/// </summary>
/// <remarks>
/// 同一个 transport 实例同时支持发送和接收（即使底层是单向 socket，
/// 不支持的方向应抛出 <see cref="NotSupportedException"/>）。
/// 实现必须线程安全：业务可以从任意线程调用 <see cref="SendAsync"/>。
/// </remarks>
public interface ITransport : IAsyncDisposable
{
    /// <summary>
    /// 该 transport 的逻辑名（DI 注册时指定，用于多实例隔离）。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 发送一条多帧消息。
    /// </summary>
    /// <param name="target">目标对端。Router 必须指定；Dealer/Pub 通常忽略；Sub 不支持发送。</param>
    /// <param name="frames">要发送的帧序列。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    ValueTask SendAsync(PeerId target, IReadOnlyList<ReadOnlyMemory<byte>> frames, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步消费入站消息流。每个调用方应只创建一个消费循环。
    /// </summary>
    IAsyncEnumerable<TransportMessage> ReceiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 对端连接状态变化（Connected / Disconnected）。
    /// 仅在能区分对端的 transport 上有意义（典型：Router、Dealer）。Pub / Sub 实现可不触发。
    /// 触发线程不保证；订阅方需自行加锁或转发到本地队列。
    /// </summary>
    event EventHandler<PeerConnectionEvent>? PeerConnectionChanged;
}
