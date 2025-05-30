using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record RespostaEmbalagemDTO
    {
        [JsonPropertyName("pedidos")]
        public List<PedidoEmbaladoDTO> Pedidos { get; init; } = [];
    }
}
