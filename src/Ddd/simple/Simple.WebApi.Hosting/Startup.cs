using Microsoft.AspNetCore.Builder;
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

namespace Simple.WebApi.Hosting;

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
            skywalker.AddUnitOfWork();
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
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Simple.WebApi.Hosting", Version = "v1" });
        });
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple.WebApi.Hosting v1"));
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
