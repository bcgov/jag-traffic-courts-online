using System.Reflection;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service;
using TrafficCourts.Common.Configuration;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplication(); // this can throw ConfigurationErrorsException

// Add services to the container.
builder.Services.AddControllers();

var swagger = SwaggerConfiguration.Get(builder.Configuration);

if (swagger.Enabled)
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Traffic Court Online Citizen Api",
            Description = "An API for creating violation ticket disputes",
        });

        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        options.CustomSchemaIds(x => x.FullName);
    });
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
