using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Interfaces;
using LojaDoSeuManoel.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace LojaDoSeuManoel.Infrastructure.Repositories
{
    public class CaixaRepository : ICaixaRepository
    {
        private readonly AppDbContext _context;
        public CaixaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EspecificacaoCaixa>> ObterTodasAsync() 
            => await _context.Caixas.AsNoTracking().ToListAsync();
    }
}
