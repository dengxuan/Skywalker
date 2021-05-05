using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application;
using Simple.Application.Abstractions;
using Simple.Application.Users;
using Simple.Domain.Users;
using Simple.EntityFrameworkCore;
using Skywalker.Ddd.Application;
using Skywalker.Ddd.Application.Abstractions;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Localization;
using Skywalker.Localization.Resources.SkywalkerLocalization;
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
            services.AddAutoMapper(options =>
            {
                options.AddProfile<SimpleApplicationAutoMapperProfile>();
            });
            services.AddApplication();
            services.AddSkywalker(skywalker =>
            {
                skywalker.AddEntityFrameworkCore<SimpleDbContext>(options =>
                {
                    options.UseMySql();
                });
                skywalker.AddAspNetCore();
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

            //app.UseSkywalkerRequestLocalization(options =>
            //{
            //});

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
