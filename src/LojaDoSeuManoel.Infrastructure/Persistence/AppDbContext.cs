using LojaDoSeuManoel.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LojaDoSeuManoel.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<EspecificacaoCaixa> Caixas { get; set; }
        public DbSet<ResultadoEmbalagem> ResultadosEmbalagem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EspecificacaoCaixa>().HasKey(c => c.Id);
            modelBuilder.Entity<EspecificacaoCaixa>().Property(c => c.Nome).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<ResultadoEmbalagem>().HasKey(r => r.Id);
            modelBuilder.Entity<ResultadoEmbalagem>().Property(r => r.Produtos).IsRequired();

            modelBuilder.Entity<EspecificacaoCaixa>().HasData(
                new EspecificacaoCaixa { Id = 1, Nome = "Caixa 1", Altura = 30, Largura = 40, Comprimento = 80 },
                new EspecificacaoCaixa { Id = 2, Nome = "Caixa 2", Altura = 80, Largura = 50, Comprimento = 40 },
                new EspecificacaoCaixa { Id = 3, Nome = "Caixa 3", Altura = 50, Largura = 80, Comprimento = 60 }
            );
        }
    }
}
