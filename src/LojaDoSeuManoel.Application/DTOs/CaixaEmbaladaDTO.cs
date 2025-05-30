using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record CaixaEmbaladaDTO
    {
        [JsonPropertyName("caixa_id")]
        public string? CaixaId { get; init; }

        [JsonPropertyName("produtos")]
        public List<string> Produtos { get; init; } = new();

        [JsonPropertyName("observacao")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Observacao { get; init; }
    }
}
