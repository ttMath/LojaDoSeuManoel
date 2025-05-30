using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record RequisicaoEmbalagemDTO
    {
        [JsonPropertyName("pedidos")]
        public List<PedidoRequestDTO> Pedidos { get; init; } = [];
    }
}
