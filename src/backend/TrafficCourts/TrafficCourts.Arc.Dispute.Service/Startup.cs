using AutoMapper;
using Microsoft.AspNetCore.Builder;
using TrafficCourts.Arc.Dispute.Service.Mappings;

namespace TrafficCourts.Arc.Dispute.Service
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
            // Start Registering and Initializing AutoMapper

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            // End Registering and Initializing AutoMapper

            services.AddMvc();

        }
    }
}
