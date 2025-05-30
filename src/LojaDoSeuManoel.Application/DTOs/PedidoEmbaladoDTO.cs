using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record PedidoEmbaladoDTO
    {
        [JsonPropertyName("pedido_id")]
        public int PedidoId { get; init; }

        [JsonPropertyName("caixas")]
        public List<CaixaEmbaladaDTO> Caixas { get; init; } = [];
    }
}
