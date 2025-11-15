using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Partner.BrasilConnect.Did.Api.Data;
using Partner.BrasilConnect.Did.Api.Endpoints;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapDidActivationEndpoints();

app.Run();
