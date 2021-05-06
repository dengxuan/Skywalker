using Microsoft.AspNetCore.Mvc;
using Skywalker.Application.Dtos;
using Skywalker.Transfer.Application.Dtos;
using System;
using System.Threading.Tasks;

namespace Skywalker.Transfer.WebApi.Controllers
{
    public class TradeOrderController : TransferController
    {
        [HttpGet]
        [Route("list")]
        public async Task<PagedResultDto<MerchantOutputDto>?> GetTradeOrdersAsync(int skip, int limit)
        {
            PagedResultDto<MerchantOutputDto>? result = await Application.ExecuteAsync<PagedResultDto<MerchantOutputDto>>();
            return result;
        }

        [HttpGet]
        [Route("single")]
        public async Task<MerchantOutputDto?> GetTradeOrderAsync(Guid id)
        {
            MerchantOutputDto? result = await Application.ExecuteAsync<EntityDto<Guid>, MerchantOutputDto>(new EntityDto<Guid>(id));
            return result;
        }
    }
}
