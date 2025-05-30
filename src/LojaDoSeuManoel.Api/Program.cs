using LojaDoSeuManoel.Application.Services;
using LojaDoSeuManoel.Application.Services.Interfaces;
using LojaDoSeuManoel.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(config);
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Loja do Seu Manoel - API de Embalagem",
        Description = "API para otimizar a embalagem dos pedidos da loja online. Protegida por JWT.",
    });
});

builder.Services.AddScoped<IServicoEmbalagem, ServicoEmbalagem>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<LojaDoSeuManoel.Infrastructure.Persistence.AppDbContext>();
    dbContext.Database.Migrate();
}

app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Manoel Packages");
});

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();