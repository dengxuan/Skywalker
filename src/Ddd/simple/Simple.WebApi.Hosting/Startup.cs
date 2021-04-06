﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application;
using Simple.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Localization;
using Skywalker.Localization.Resources.SkywalkerLocalization;
using System.Collections.Generic;

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
            services.AddHttpContextAccessor();
            services.AddRedisCaching(configure =>
            {
                configure.ConnectionString = "127.0.0.1";
            });
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

            services.Configure<SkywalkerLocalizationOptions>(options =>
            {
                options
                    .Resources
                    .Add<DefaultResource>("en");

                options
                    .Resources
                    .Add<SkywalkerLocalizationResource>("en")
                    .AddVirtualJson("/Localization/Resources/SkywalkerLocalization");
            });
            services.AddWebApiResponseWrapper();
            services.AddLocalization();
            services.AddRequestLocalization(configureOptions =>
            {
                configureOptions.AddSupportedCultures();
                configureOptions.AddSupportedUICultures();
            });
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
