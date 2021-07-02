using Gov.TicketSearch.Auth;
using Gov.TicketSearch.Services;
using Gov.TicketSearch.Services.RsiServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Refit;
using System;

namespace Gov.TicketSearch
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ticket_search", Version = "v1" });
            });
            ConfigTicketsServices(services);
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ticket_search v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigTicketsServices(IServiceCollection services)
        {
            string operationMode = Configuration.GetSection("RSI:OPERATIONMODE").Value;
            if (operationMode == Keys.RsiOperationModeFake)
                services.AddTransient<ITicketsService, FakeTicketsService>();
            else
            {
                services.AddOptions<OAuthOptions>()
                    .Bind(Configuration.GetSection("OAuth"))
                    .ValidateDataAnnotations();
                services.AddMemoryCache();
                services.AddHttpClient<IOAuthClient, OAuthClient>();
                services.AddTransient<ITokenService, TokenService>();
                services.AddTransient<OAuthHandler>();
                services.AddRefitClient<IRsiRestApi>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration.GetSection("RSI:BASEADDRESS").Value))
                    .AddHttpMessageHandler<OAuthHandler>();
                services.AddTransient<ITicketsService, RsiTicketsService>();
            }
        }
    }

}
