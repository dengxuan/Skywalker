# Skywalker Transport 模块

`Skywalker.Transport.*` 是字节级双向通信抽象。它**不**关心消息语义、不关心序列化——这些是上层 `Skywalker.Messaging` 的职责。Transport 只负责把若干帧（`ReadOnlyMemory<byte>`）原子地从一端送到另一端，并在对端连接状态变化时通知上层。

本文面向**两类读者**：

1. 想新增一个 transport 实现（gRPC、QUIC、WebSocket、SignalR 等）的开发者
2. 排查现有 transport 行为异常时需要核对契约的维护者

---

## 抽象层次

```
┌────────────────────────────────────────────────────────┐
│ 业务代码：IRpcClient / IRpcHandler<TReq,TRes>          │
├────────────────────────────────────────────────────────┤
│ Skywalker.Messaging.MessagingChannel                   │  ← 协议层（请求/响应配对、Pub/Sub 派发）
├────────────────────────────────────────────────────────┤
│ Skywalker.Transport.ITransport                         │  ← 帧级传输（本文重点）
├────────────────────────────────────────────────────────┤
│ NetMQ / gRPC / QUIC / 自定义 socket                    │
└────────────────────────────────────────────────────────┘
```

`MessagingChannel` 已经把 4 条铁律满足了**协议层**那一半；Transport 实现者只需保证**传输层**那一半。两者合起来才能让 bidi 通信稳健工作。

---

## Transport 实现者四条铁律

这 4 条铁律来自真实生产事故的复盘。每一条都在历史上至少踩过一次，不允许新的实现绕过。

### 铁律 #1：read loop 只内联做「不会阻塞的轻量路由」

读循环（从底层 socket / stream 把消息读出来的循环）**绝不能**在自己线程上同步执行任何业务 handler。它的工作只能是：

- 把入站消息塞进一个 `Channel<TransportMessage>` 让上层异步消费
- 或者顶多内联做「路由 Ack」这类**不会**调用用户代码、不会等 IO 的操作

如果读循环在自己线程上 `await` 了用户 handler，就会出现**队头阻塞**：一条慢消息把整个 stream 后面所有消息都卡住。

> **历史事故**：`gaming-go-sdk` v1.3.0 之前 `readLoop` 同步调用 `dispatchHandler`，单条耗时 5 秒的请求把后续所有消息（包括对方的 Ack）全部 hold 住，最终触发超时并被错误判定为「连接断开」。
>
> **修复**：read loop 改为 `go c.dispatchHandler(ctx, message)`（每条消息一个 goroutine），或在 .NET 里用 `_ = DispatchAsync(message, ct)`。

### 铁律 #2：单条消息的发送/处理失败 ≠ 断开连接

一条 reply 写不出去（被对端拒收、序列化报错、单条超时）只能让**这一条**失败，**绝不能**：

- 触发 `PeerConnectionChanged(Disconnected)`
- 关闭 `_inChannel` / 取消 read loop
- 标记整条 stream 不可用

只有两种情况允许判定整条连接挂掉：底层 socket / stream 抛出 transport-fatal 异常（IO 失败、TLS 握手失败、HTTP/2 GOAWAY 等）——而这种判定**只能**在铁律 #4 描述的位置做。

> **历史事故**：`gaming-go-sdk` v1.3.0 之前，处理一条 reverse request 时如果用户 `build()` 函数返回 error，代码会把整条 bidi stream 关掉。结果一个商户的业务异常会让平台对该商户**所有** in-flight 请求一起失败。
>
> **修复**：`handleReverseOrder` 的失败只发回单条 failure envelope，不动 stream。

### 铁律 #3：CT 只用于「pre-wire」，写入开始后必须忽略

`SendAsync(target, frames, ct)` 的 `cancellationToken` 只能在**「字节还没真的开始往 wire 上推」**之前生效，例如：

- 等写锁（`SemaphoreSlim.WaitAsync(ct)`）
- 等连接建立（`ConnectAsync(ct)`）
- 等 backpressure 让步（`Channel.Writer.WriteAsync(item, ct)` 时 channel 满）

**一旦** 写入已经开始（已经 `WriteAsync(frame[0])` 进 HTTP/2 stream、已经 `Send(frame[0])` 到 socket），实现**必须**忽略后续 CT 取消，把整组帧写完或抛出 transport 级异常。

> **历史事故**：`gaming-dotnet-sdk` v1.3.0 之前，`GamingSession.SendAsync(message, ct)` 把请求方的 `ct` 直接透传给 `RequestStream.WriteAsync(message, ct)`。请求方一超时，CT 触发 → gRPC 发出 HTTP/2 RST_STREAM → **整条 bidi stream** 上所有 in-flight 请求一起失败、连接被强制重建。10 个并发请求里只要任意一个超时，全部 9 个无辜请求一起阵亡。
>
> **修复**：`_writeLock.WaitAsync(ct)` 之后调用 `WriteAsync(message)`（不传 ct）。

NetMq 实现因为「写入 = 入队 `NetMQQueue<T>`」，物理上无法被 CT 撕到一半，**碰巧**满足铁律 #3。但任何**真正同步流式**的 transport（gRPC / QUIC / TCP）必须显式遵守。

### 铁律 #4：唯一的「连接挂掉」判定源 = read loop 的读异常

判断「对端挂了 / 网络断了」**有且仅有一处**入口——read loop 的 `MoveNext()` / `Recv()` / `ReadAsync()` 抛 transport-fatal 异常。

不允许：
- `SendAsync` 失败时主动 `RaisePeerDisconnected()`（违反铁律 #2）
- 业务 handler 异常时关连接（违反铁律 #2）
- 心跳超时另起一套断连判定（应改为 read loop 的读超时）

为什么？因为多源判定一定会**误判** + **死锁**：A 路径判定挂了把 stream 关掉，B 路径正在往 stream 里写、抛异常被当成「又挂了一次」，最终留下一堆未释放的资源和重复的 disconnect 事件。

> **历史事故**：`gaming-go-sdk` 早期版本 `isTransientStreamError(nil)` 返回 `true`，导致 read loop 收到正常 EOF（`nil` error）时也被当成「短暂错误」无限重连。
>
> **修复**：所有「这是不是断连」的判定收敛到 read loop 一处，并明确 `nil`/EOF 不算 transient。

---

## 实现 checklist

写新 transport 时按顺序勾选：

- [ ] `ITransport.Name` 由构造参数传入，对外只读。
- [ ] `SendAsync` 满足 **铁律 #3**：CT 只用于争抢资源，开写后不再响应。
- [ ] `SendAsync` 失败只抛异常给调用方，**不**触发 `PeerConnectionChanged`、**不**关闭入站 channel（铁律 #2）。
- [ ] `ReceiveAsync` 返回 `Channel<TransportMessage>.Reader.ReadAllAsync(ct)`，每实例只支持单消费者。
- [ ] read loop 把消息塞进 channel 后**立刻**回去读下一条；任何 handler 派发由上层完成（铁律 #1）。
- [ ] `PeerConnectionChanged` 的所有触发都来自 read loop 的异常路径或底层连接事件回调，**不在** `SendAsync` 路径上（铁律 #4）。
- [ ] `DisposeAsync` 关闭 `_inChannel.Writer`、停止 read loop、释放底层 socket / stream / poller。
- [ ] 单元测试覆盖：
  - 写入开始后取消 CT，断言写入完成、connection 不断
  - `SendAsync` 抛异常后，`PeerConnectionChanged` 未触发，后续 `SendAsync` 仍可用
  - 服务端关流后，`PeerConnectionChanged(Disconnected)` 触发**且仅触发一次**
  - 多帧消息按序到达对端，跨消息无穿插

---

## 参考实现

| Transport | 项目 | 满足方式 |
|---|---|---|
| NetMQ (Pub/Sub/Router/Dealer) | `Skywalker.Transport.NetMq` | 铁律 #3 通过 `NetMQQueue` 入队天然满足；事件源唯一（`NetMQMonitor`）满足 #4 |
| gRPC bidi stream | `Skywalker.Transport.Grpc`（规划中，见 issue #203） | 显式 `_writeLock.WaitAsync(ct)` + 不传 CT 给 `RequestStream.WriteAsync` |

---

## 相关历史

- [gaming-dotnet-sdk v1.3.0 CHANGELOG](https://github.com/L8CHAT/gaming-dotnet-sdk/blob/main/CHANGELOG.md)
- [gaming-go-sdk v1.3.0 CHANGELOG](https://github.com/L8CHAT/gaming-go-sdk/blob/main/CHANGELOG.md)
- Epic：[Skywalker 作为统一 bidi 消息内核](https://github.com/dengxuan/Skywalker/issues/201)
