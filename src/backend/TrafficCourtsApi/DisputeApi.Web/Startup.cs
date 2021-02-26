using System;
using System.Collections.Generic;
using System.Text;
using DisputeApi.Web.Features.TicketService.Configuration;
using DisputeApi.Web.Features.TicketService.DBContexts;
using DisputeApi.Web.Features.TokenService.Configuration;
using DisputeApi.Web.Health;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using NSwag;
using Serilog;

namespace DisputeApi.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            services.AddDbContext<TicketContext>(opt => opt.UseInMemoryDatabase("DisputeApi"));
            services.AddControllers();
            ConfigureOpenApi(services);
            services.AddHealthChecks().AddCheck<DisputeApiHealthCheck>("service_health_check", failureStatus: HealthStatus.Degraded);
            services.AddTicketService();
            services.AddTokenService();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt.TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Serilog middleware will not time or log components that appear before it in the pipeline
            // This can be utilized to exclude noisy handlers from logging, such as UseStaticFiles(), by placing UseSerilogRequestLogging() after them
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3();
            }

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                 new CacheControlHeaderValue()
                 {
                     NoStore = true,
                     NoCache = true,
                     MustRevalidate = true,
                     MaxAge = TimeSpan.FromSeconds(0),
                     Private = true,
                 };
                context.Response.Headers.Add("Pragma", "no-cache");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });


            app.UseRouting();

            app.UseOpenApi();

            // Add use Cors here

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });
        }


        /// <summary>
        /// Configure Open Api using NSwag
        /// https://github.com/RicoSuter/NSwag
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureOpenApi(IServiceCollection services)
        {

            services.AddSwaggerDocument(config =>
            {
                // configure swagger properties
                config.PostProcess = document =>
                {
                    document.Info.Version = "V0.1";
                    document.Info.Description = "Dispute API";
                    document.Info.Title = "Dispute API";
                    document.Tags = new List<OpenApiTag>()
                    {
                        new OpenApiTag() {
                            Name = "Dispute API",
                            Description = "Dispute API"
                        }
                    };
                };
            });

        }
    }
}
