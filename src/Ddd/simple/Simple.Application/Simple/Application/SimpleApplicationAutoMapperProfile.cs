using AutoMapper;
using Simple.Application.Abstractions;
using Simple.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Application
{
    public class SimpleApplicationAutoMapperProfile : Profile
    {
        public SimpleApplicationAutoMapperProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
