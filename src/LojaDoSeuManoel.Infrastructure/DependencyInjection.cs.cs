using LojaDoSeuManoel.Domain.Interfaces;
using LojaDoSeuManoel.Infrastructure.Persistence;
using LojaDoSeuManoel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LojaDoSeuManoel.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            services.AddScoped<ICaixaRepository, CaixaRepository>();
            services.AddScoped<IResultadoEmbalagemRepository, ResultadoEmbalagemRepository>();

            return services;
        }
    }
}
