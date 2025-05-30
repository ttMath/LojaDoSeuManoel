using LojaDoSeuManoel.Domain.Entities;
namespace LojaDoSeuManoel.Domain.Interfaces
{
    public interface ICaixaRepository
    {
        Task<IEnumerable<EspecificacaoCaixa>> ObterTodasAsync();
    }
}
