using HealthChecks.UI.Client;
using localshortener.api.Infrastructure.Contracts;
using localshortener.api.Infrastructure.Implementations;
using localshortener.api.Middlewares;
using localshortener.api.Presentation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddStackExchangeRedisCache(opt=>
{
    opt.Configuration = "localhost:6379";
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalErrorHandlerMiddleware>();
app.AddShortenerEndpoints();
app.UseCors("AllowLocalhost4200");
var healthCheckEndpoint = builder.Configuration["HealthCheckEndpoint"] ?? "/health";
//app.UseHealthChecks(healthCheckEndpoint, new HealthCheckOptions
//{
//    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
//});
app.UseHttpsRedirection();


app.Run();