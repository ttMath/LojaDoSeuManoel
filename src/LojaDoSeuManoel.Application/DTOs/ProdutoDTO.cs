using System.Text.Json.Serialization;

namespace LojaDoSeuManoel.Application.DTOs
{
    public record ProdutoDTO
    {
        [JsonPropertyName("produto_id")]
        public string ProdutoId { get; init; } = string.Empty;

        [JsonPropertyName("dimensoes")]
        public DimensaoDTO Dimensoes { get; init; } = new();
    }
}
