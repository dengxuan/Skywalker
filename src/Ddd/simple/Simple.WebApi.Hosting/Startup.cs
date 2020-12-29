﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application;
using Simple.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Uow;
using System;

namespace Simple.WebApi.Hosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().ConfigureApplicationPartManager(apm =>
            {
                apm.ApplicationParts.Add(new AssemblyPart(typeof(SimpleController).Assembly));
            });
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddSkywalker(skywalker =>
            {
                skywalker.AddEntityFrameworkCore<SimpleDbContext>(options =>
                {
                    options.UseMySql();
                });
                skywalker.AddAutoMapper(options =>
                {
                    options.AddProfile<SimpleApplicationAutoMapperProfile>();
                });
            });
            services.AddAspects(options=>
            {
                options.Use(new UnitOfWorkProvider());
            });
            services.AddWebApiResponseWrapper();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
