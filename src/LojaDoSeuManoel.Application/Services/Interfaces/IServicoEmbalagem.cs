using LojaDoSeuManoel.Application.DTOs;

namespace LojaDoSeuManoel.Application.Services.Interfaces
{
    public  interface IServicoEmbalagem
    {
        Task<RespostaEmbalagemDTO> ProcessarRequisicaoEmbalagem(RequisicaoEmbalagemDTO requisicao);
    }
}
