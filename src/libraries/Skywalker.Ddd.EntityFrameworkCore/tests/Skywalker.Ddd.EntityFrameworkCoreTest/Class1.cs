﻿//// <auto-generated>
////     Generated by the Skywalker EntityFrameworkCore Generators.
//// </auto-generated>

//#pragma warning disable CS8019 // Unused usings

//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using Skywalker.Ddd.Domain.Entities;
//using Skywalker.Ddd.Domain.Repositories;
//using Skywalker.Ddd.Domain.Services;
//using Skywalker.Ddd.Uow;
//using Skywalker.Ddd.Uow.Abstractions;
//using Skywalker.Ddd.EntityFrameworkCore;
//using Skywalker.Ddd.EntityFrameworkCore.Repositories;
//using Skywalker.Identifier.Abstractions;
//using Skywalker.Extensions.DependencyInjection.Interceptors;
//using Skywalker.Extensions.DependencyInjection.Interceptors.Abstractions;
//using Skywalker.Extensions.DependencyInjection;
//using Skywalker.Extensions.DependencyInjection.Abstractions;
//using Skywalker.Extensions.Timezone;
//using System.CodeDom.Compiler;
//using System.Linq;

//namespace Skywalker.Ddd.EntityFrameworkCore.Generators;

//[GeneratedCode("Skywalker.Ddd.EntityFrameworkCore.Generators", "3.0.0.0")]
//public static class IntecepterIServiceColle1ctionExtensions
//{
//    public static IServiceCollection AddDependencyServices(this IServiceCollection services)
//    {
//        services.AddScoped<IUserDomainService, UserDomainServiceIntecepter>();
//        services.AddScoped<IUserDomainService, UserDomainServiceIntecepterIntecepter>();
//        services.AddScoped<IRepository<User>, UserRepository>();
//        services.AddScoped<IBasicRepository<User>, UserRepository>();
//        services.AddScoped<IReadOnlyRepository<User>, UserRepository>();
//        services.AddScoped<IRepository<User, System.Guid>, UserRepository>();
//        services.AddScoped<IBasicRepository<User, System.Guid>, UserRepository>();
//        services.AddScoped<IReadOnlyRepository<User, System.Guid>, UserRepository>();
//        services.AddScoped<IRepository<Username>, UsernameRepository>();
//        services.AddScoped<IBasicRepository<Username>, UsernameRepository>();
//        services.AddScoped<IReadOnlyRepository<Username>, UsernameRepository>();
//        services.AddScoped<IRepository<Username, int>, UsernameRepository>();
//        services.AddScoped<IBasicRepository<Username, int>, UsernameRepository>();
//        services.AddScoped<IReadOnlyRepository<Username, int>, UsernameRepository>();
//        services.AddScoped<IRepository<Schoole>, SchooleRepository>();
//        services.AddScoped<IBasicRepository<Schoole>, SchooleRepository>();
//        services.AddScoped<IReadOnlyRepository<Schoole>, SchooleRepository>();
//        services.AddScoped<IRepository<Test>, TestRepository>();
//        services.AddScoped<IBasicRepository<Test>, TestRepository>();
//        services.AddScoped<IReadOnlyRepository<Test>, TestRepository>();
//        services.AddScoped<IRepository<TestA>, TestARepository>();
//        services.AddScoped<IBasicRepository<TestA>, TestARepository>();
//        services.AddScoped<IReadOnlyRepository<TestA>, TestARepository>();
//        services.AddScoped<IRepository<TestA, long>, TestARepository>();
//        services.AddScoped<IBasicRepository<TestA, long>, TestARepository>();
//        services.AddScoped<IReadOnlyRepository<TestA, long>, TestARepository>();
//        return services;
//    }
//}