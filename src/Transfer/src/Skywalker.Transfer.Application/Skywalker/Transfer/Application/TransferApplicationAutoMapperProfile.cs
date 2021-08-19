using AutoMapper;
using Skywalker.Transfer.Application.Dtos;
using Skywalker.Transfer.Domain.Merchants;

namespace Skywalker.Transfer.Application
{
    public class TransferApplicationAutoMapperProfile : Profile
    {
        public TransferApplicationAutoMapperProfile()
        {
            CreateMap<Merchant, MerchantOutputDto>();
        }
    }
}
