using Skywalker.Application.Dtos;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Transfer.Application.Dtos;
using System;

namespace Skywalker.Transfer.Application.Merchants
{
    public interface ISingleMerchantApplicationHandler : IApplicationHandler<EntityDto<Guid>, MerchantOutputDto>
    {
    }
}
