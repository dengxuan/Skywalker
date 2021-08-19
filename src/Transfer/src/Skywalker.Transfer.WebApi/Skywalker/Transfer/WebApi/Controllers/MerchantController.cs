using Microsoft.AspNetCore.Mvc;
using Skywalker.Application.Dtos;
using Skywalker.Transfer.Application.Dtos;
using System;
using System.Threading.Tasks;

namespace Skywalker.Transfer.WebApi.Controllers
{

    public class MerchantController : TransferController
    {
        [HttpGet]
        [Route("list")]
        public async Task<PagedResultDto<MerchantOutputDto>?> GetMerchantsAsync(int skip, int limit)
        {
            PagedResultDto<MerchantOutputDto>? result = await Application.ExecuteAsync<PagedResultRequestDto, PagedResultDto<MerchantOutputDto>>(new PagedResultRequestDto { SkipCount = skip, MaxResultCount = limit });
            return result;
        }

        [HttpGet]
        [Route("single")]
        public async Task<MerchantOutputDto?> GetMerchantAsync(Guid id)
        {
            MerchantOutputDto? result = await Application.ExecuteAsync<EntityDto<Guid>, MerchantOutputDto>(new EntityDto<Guid>(id));
            return result;
        }
    }
}
