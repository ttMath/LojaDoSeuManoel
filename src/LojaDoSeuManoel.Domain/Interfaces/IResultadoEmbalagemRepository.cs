using LojaDoSeuManoel.Domain.Entities;

namespace LojaDoSeuManoel.Domain.Interfaces
{
    public interface IResultadoEmbalagemRepository
    {
        Task AdicionarVariosAsync(IEnumerable<ResultadoEmbalagem> resultados);
    }
}
