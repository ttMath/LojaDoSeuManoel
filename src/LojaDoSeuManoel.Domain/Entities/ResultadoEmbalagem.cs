namespace LojaDoSeuManoel.Domain.Entities
{
    public class ResultadoEmbalagem
    {
        public Guid Id { get; set; }
        public int PedidoId { get; set; }
        public string? CaixaId { get; set; }
        public string Produtos { get; set; } = string.Empty;
        public string? Observacao { get; set; }
        public DateTime ProcessadoEm { get; set; }
    }
}
