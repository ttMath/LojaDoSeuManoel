using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record PedidoRequestDTO
    {
        [JsonPropertyName("pedido_id")]
        public int PedidoId { get; init; }

        [JsonPropertyName("produtos")]
        public List<ProdutoDTO> Produtos { get; init; } = new();
    }
}
