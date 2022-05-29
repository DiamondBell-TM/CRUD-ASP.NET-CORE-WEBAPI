using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD_ASP.NET_CORE_WEBAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace CRUD_ASP.NET_CORE_WEBAPI
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
            services.AddControllers();
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddIdentity<IdentityUser, IdentityRole>(Configuration =>
             {

             }).AddEntityFrameworkStores<AppDbContext>();
            var corsURLS = Configuration.GetSection("CORSURLS").GetChildren().ToDictionary(x => x.Key, x => x.Value);
            List<string> allowedURLS = new List<string>();
            if (corsURLS.Count > 0)
            {
                foreach (var item in corsURLS)
                {
                    if (item.Key.ToUpperInvariant() == "ALLOWEDURLS")
                    {
                        foreach (var value in item.Value.Split(";"))
                        {
                            allowedURLS.Add(value);
                        }
                    }
                }
            }
            services.AddCors(options =>
            {
                options.AddPolicy("CORSAPI", builder =>
                {
                    builder.WithOrigins(allowedURLS.ToArray())
                    .WithHeaders(HeaderNames.ContentType, "application/json")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .WithMethods("GET", "POST");
                });
            });
        }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CORSAPI");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
