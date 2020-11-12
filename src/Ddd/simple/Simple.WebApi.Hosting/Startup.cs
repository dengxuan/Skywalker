using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simple.Application;
using Simple.Infrastructure.EntityFrameworkCore;
using Simple.Infrastructure.Mongodb;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

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
                skywalker.AddInfrastructure(initializer =>
                {
                    initializer.AddEntityFrameworkCore<SimpleDbContext>(options =>
                    {
                        options.UseSqlServer();
                    });
                    initializer.AddMongodb<SimpleMongoContext>();
                });
                skywalker.AddAutoMapper(options =>
                {
                    options.AddProfile<SimpleApplicationAutoMapperProfile>();
                });
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
