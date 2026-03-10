// Licensed to the Gordon

using AutoMapper;
using Skywalker.Ddd.Application.Dtos;
using Skywalker.Extensions.Collections.Generic;

namespace Skywalker.Ddd.Application;

public class SkywalkerProfile : Profile
{
    public SkywalkerProfile()
    {
        CreateMap(typeof(PagedList<>), typeof(PagedResultDto<>));
    }
}
