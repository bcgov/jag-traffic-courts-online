using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

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
    }

    public interface ITicketSearch
    {
        public Task SearchAsync(string ticketNumber, string time, CancellationToken cancellationToken);
    }

    public class TicketSearch : ITicketSearch
    {
        public Task SearchAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class TicketSearchQuery
    {
        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
        public string TicketNumber { get; set; }

        [Required]
        [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
        public string Time { get; set; }
    }

    public class TicketSearchResponse
    {
        public string TicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public bool InformationCertified { get; set; }
        public List<Offence> Offences { get; set; }
    }

    public class Offence
    {
        public short CountNumber { get; set; }
        public decimal TicketedAmount { get; set; }//total
        public decimal AmountDue { get; set; } //total-discount-paid
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        //public OffenceDisputeDetail OffenceDisputeDetail { get; set; }
        public decimal DiscountAmount { get; set; }//discount
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }
    }
}
