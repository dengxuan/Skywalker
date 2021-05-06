using Skywalker.Application.Dtos;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Transfer.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Application.Merchants
{
    public class ISingleMerchantApplicationHandler : IApplicationHandler<EntityDto<Guid>, MerchantOutputDto>
    {
    }
}
