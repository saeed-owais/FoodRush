using FoodRush.API.Extensions;
using FoodRush.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
/// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
///builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis("localhost:6379");

builder.Services.AddOptionsConfiguration(builder.Configuration);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseExceptionHandler();
app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
