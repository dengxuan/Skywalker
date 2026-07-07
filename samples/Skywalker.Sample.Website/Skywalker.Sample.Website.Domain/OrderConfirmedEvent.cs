namespace Skywalker.Sample.Website.Domain;

/// <summary>
/// 订单确认领域事件：业务事务提交后经 IUnitOfWork.OnCompleted 发布到事件总线。
/// </summary>
public record OrderConfirmedEvent(Guid OrderId, string OrderNo, string CustomerEmail, decimal TotalAmount);
