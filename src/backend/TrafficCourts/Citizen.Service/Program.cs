using MediatR;
using TrafficCourts.Citizen.Service.Configuration;
using TrafficCourts.Messaging;
using TrafficCourts.Citizen.Service.Features.Tickets;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit<CitizenServiceConfiguration>(builder);

builder.Services.AddMediatR(typeof(Program).GetType().Assembly);
// Need to manually register a MediatR.IRequest, getting a "Handler was not found for request of type MediatR.IRequestHandler" otherwise.
builder.Services.AddTransient<IRequestHandler<Analyse.AnalyseRequest, Analyse.AnalyseResponse>, Analyse.Handler>();

// Bind FormRecognizer configuration properties
// builder.Services.AddOptions<FormRecognizerConfigurationOptions>().ValidateDataAnnotations().ValidateOnStart(); // doesn't recognize ENV vars
builder.Services.Configure<FormRecognizerConfigurationOptions>(builder.Configuration.GetSection(FormRecognizerConfigurationOptions.FormRecognizer)); // Better, recognizes ENV vars, but doesn't validate on start.

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
