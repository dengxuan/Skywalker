using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Transfer.Application;
using Skywalker.Transfer.EntityFrameworkCore;
using System.Reflection;

namespace Skywalker.Transfer.WebApi.Hosting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

            //services.AddRedisCaching(configure =>
            //{

            //});
            services.AddApplication();
            services.AddAutoMapper(options=>
            {
                options.AddProfile<TransferApplicationAutoMapperProfile>();
            });
            services.AddSkywalker(skywalker =>
            {
                skywalker.AddEntityFrameworkCore<TransferDbContext>(options =>
                {
                    options.UseMySql(mysql =>
                    {
                        mysql.MigrationsAssembly("Skywalker.Transfer.EntityFrameworkCore.DbMigrations");
                    });
                });
            });

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddControllers().ConfigureApplicationPartManager(apm =>
            {
                apm.ApplicationParts.Add(new AssemblyPart(Assembly.Load("Skywalker.Transfer.WebApi")));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Skywalker.Transfer.WebApi.Hosting", Version = "v1" });
            });
            services.AddWebApiResponseWrapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skywalker.Transfer.WebApi.Hosting v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
