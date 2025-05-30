using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record DimensaoDTO
    {
        [JsonPropertyName("altura")]
        public int Altura { get; init; }

        [JsonPropertyName("largura")]
        public int Largura { get; init; }

        [JsonPropertyName("comprimento")]
        public int Comprimento { get; init; }
    }
}
