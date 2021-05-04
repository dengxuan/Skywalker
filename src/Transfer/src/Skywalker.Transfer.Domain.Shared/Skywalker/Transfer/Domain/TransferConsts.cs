using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain
{
    public static class TransferConsts
    {
        public const string DefaultDbTablePrefix = "Tsfr";

        public const string DefaultDbSchema = null;

        public static class Validations
        {
            /// <summary>
            /// 最大商户标识长度
            /// </summary>
            public const int MaxMerchantSchemeLength = 20;

            /// <summary>
            /// 最大商户号长度
            /// </summary>
            public const int MaxMerchantNumberLength = 128;

            /// <summary>
            /// 最大商户名长度
            /// </summary>
            public const int MaxMerchantNameLength = 64;

            /// <summary>
            /// 最大商户描述长度
            /// </summary>
            public const int MaxMerchantDescriptionLength = 200;

            /// <summary>
            /// 最大商户密钥长度
            /// </summary>
            public const int MaxMerchantCipherKeyLength = 128;

            /// <summary>
            /// 最大商户接口地址长度
            /// </summary>
            public const int MaxMerchantAddressLength = 256;

            /// <summary>
            /// 最大撤单原因长度
            /// </summary>

            public const int MaxTradeOrderRevokeReasonLength = 1000;

            /// <summary>
            /// 最大转账消息长度
            /// </summary>

            public const int MaxTransferDetailMessageLength = 1000;
        }
    }
}
