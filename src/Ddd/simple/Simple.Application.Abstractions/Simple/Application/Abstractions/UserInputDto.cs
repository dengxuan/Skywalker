using Skywalker.Application.Dtos;
using Skywalker.Application.Dtos.Contracts;
using System;

namespace Simple.Application.Users
{
    public class UserInputDto : EntityDto
    {
        public string? Name { get; set; }
    }
}
