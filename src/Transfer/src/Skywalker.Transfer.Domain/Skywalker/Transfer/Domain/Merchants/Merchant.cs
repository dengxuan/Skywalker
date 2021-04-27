using Skywalker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Domain.Merchants
{
    public class Merchant : AggregateRoot<Guid>
    {
        /// <summary>
        /// 商户标识
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 商户Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 商户密钥
        /// </summary>
        public string CipherKey { get; set; }


    }
}
