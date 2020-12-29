﻿using AutoMapper;
using Simple.Application.Abstractions;
using Simple.Domain.Users;

namespace Simple.Application
{
    public class SimpleApplicationAutoMapperProfile : Profile
    {
        public SimpleApplicationAutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserValue, UserValueDto>();
        }
    }
}
