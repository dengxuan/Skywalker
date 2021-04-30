namespace Skywalker.Transfer.Domain.Enumerations
{
    public enum TransferTypes : byte
    {
        /// <summary>
        /// 充值
        /// </summary>
        Recharge,

        /// <summary>
        /// 提现
        /// </summary>
        Withdraw,

        /// <summary>
        /// 赠送
        /// </summary>
        Present,

        /// <summary>
        /// 支付
        /// </summary>
        Payment,

        /// <summary>
        /// 出账
        /// </summary>
        In,

        /// <summary>
        /// 入账
        /// </summary>
        Out,
    }
}
