using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Interfaces;
using LojaDoSeuManoel.Infrastructure.Persistence;

namespace LojaDoSeuManoel.Infrastructure.Repositories
{
    public class ResultadoEmbalagemRepository : IResultadoEmbalagemRepository
    {
        private readonly AppDbContext _context;
        public ResultadoEmbalagemRepository(AppDbContext context) => _context = context;

        public async Task AdicionarVariosAsync(IEnumerable<ResultadoEmbalagem> resultados)
        {
            await _context.ResultadosEmbalagem.AddRangeAsync(resultados);
            await _context.SaveChangesAsync();
        }
    }
}
