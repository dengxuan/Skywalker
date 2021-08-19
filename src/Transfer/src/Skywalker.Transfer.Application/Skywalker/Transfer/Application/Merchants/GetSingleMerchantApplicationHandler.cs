using Skywalker.Application.Dtos;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.ObjectMapping;
using Skywalker.Transfer.Application.Dtos;
using Skywalker.Transfer.Domain.Merchants;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Transfer.Application.Merchants
{
    public class GetSingleMerchantApplicationHandler : IApplicationHandler<EntityDto<Guid>, MerchantOutputDto>
    {
        private readonly IMerchantManager _merchantManager;
        private readonly IObjectMapper _objectMapper;

        public GetSingleMerchantApplicationHandler(IMerchantManager merchantManager, IObjectMapper objectMapper)
        {
            _merchantManager = merchantManager;
            _objectMapper = objectMapper;
        }

        public async Task<MerchantOutputDto?> HandleAsync(EntityDto<Guid> inputDto, CancellationToken cancellationToken = default)
        {
            Merchant? merchant = await _merchantManager.GetAsync(inputDto.Id);
            return _objectMapper.Map<Merchant?, MerchantOutputDto>(merchant);
        }
    }
}
