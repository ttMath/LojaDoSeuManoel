namespace LojaDoSeuManoel.Domain.Entities
{
    public class EspecificacaoCaixa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Altura { get; set; }
        public int Largura { get; set; }
        public int Comprimento { get; set; }
        public long Volume => (long)Altura * Largura * Comprimento;
    }
}
