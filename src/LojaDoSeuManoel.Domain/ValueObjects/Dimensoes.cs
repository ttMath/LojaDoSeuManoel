namespace LojaDoSeuManoel.Domain.ValueObjects
{
    public record Dimensoes(int Altura, int Largura, int Comprimento)
    {
        public long Volume => (long)Altura * Largura * Comprimento;

        public bool CabeEm(Dimensoes dimensoesCaixa)
        {
            int p_a = Altura;
            int p_l = Largura;
            int p_c = Comprimento;

            int c_a = dimensoesCaixa.Altura;
            int c_l = dimensoesCaixa.Largura;
            int c_c = dimensoesCaixa.Comprimento;

            return (p_a <= c_a && p_l <= c_l && p_c <= c_c) || (p_a <= c_a && p_c <= c_l && p_l <= c_c) ||
                   (p_l <= c_a && p_a <= c_l && p_c <= c_c) || (p_l <= c_a && p_c <= c_l && p_a <= c_c) ||
                   (p_c <= c_a && p_a <= c_l && p_l <= c_c) || (p_c <= c_a && p_l <= c_l && p_a <= c_c);
        }
    }
}
