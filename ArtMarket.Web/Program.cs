using ArtMarket.Application.Interfaces;
using ArtMarket.Application.Services;
using ArtMarket.Domain.Interfaces;
using ArtMarket.Infrastructure.MachineLearning;
using ArtMarket.Infrastructure.Persistence;
using ArtMarket.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ArtMarketDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IArtworkRepository, ArtworkRepository>();
builder.Services.AddSingleton<IPricePredictionEngine, MlPricePredictionEngine>();
builder.Services.AddScoped<IArtAnalyticsService, ArtAnalyticsService>();

var app = builder.Build();

app.UseSwagger(); 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Art Market API v1");
    c.RoutePrefix = string.Empty; 
});
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();