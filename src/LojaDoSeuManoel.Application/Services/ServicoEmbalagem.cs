
using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services.Interfaces;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Interfaces;
using LojaDoSeuManoel.Domain.ValueObjects;

namespace LojaDoSeuManoel.Application.Services
{
    public class ServicoEmbalagem : IServicoEmbalagem
    {
        private readonly ICaixaRepository _caixaRepository;
        private readonly IResultadoEmbalagemRepository _resultadoRepository;

        public ServicoEmbalagem(ICaixaRepository caixaRepository,IResultadoEmbalagemRepository resultadoRepository)
        {
            _caixaRepository = caixaRepository;
            _resultadoRepository = resultadoRepository;
        }
        public async Task<RespostaEmbalagemDTO> ProcessarRequisicaoEmbalagem(RequisicaoEmbalagemDTO requisicao)
        {
            var caixasDisponiveis = (await _caixaRepository.ObterTodasAsync()).ToList();

            var pedidosEmbalados = new List<PedidoEmbaladoDTO>();
            var todosOsResultadosParaSalvar = new List<ResultadoEmbalagem>();

            foreach (var pedidoRequest in requisicao.Pedidos)
            {
                var produtosDoDominio = pedidoRequest.Produtos
                    .Select(p => new Produto(
                        p.ProdutoId,
                        new Dimensoes(p.Dimensoes.Altura, p.Dimensoes.Largura, p.Dimensoes.Comprimento)))
                    .ToList();

                var pedidoEmbaladoDto = new PedidoEmbaladoDTO { PedidoId = pedidoRequest.PedidoId, Caixas = new List<CaixaEmbaladaDTO>() };
                var produtosParaEmbalar = new List<Produto>();

                foreach (var produto in produtosDoDominio)
                {
                    bool cabeEmAlgumaCaixa = caixasDisponiveis.Any(c =>
                        produto.Dimensoes.CabeEm(new Dimensoes(c.Altura, c.Largura, c.Comprimento)));

                    if (!cabeEmAlgumaCaixa)
                    {
                        pedidoEmbaladoDto.Caixas.Add(new CaixaEmbaladaDTO
                        {
                            CaixaId = null,
                            Produtos = new List<string> { produto.ProdutoId },
                            Observacao = "Produto não cabe em nenhuma caixa disponível."
                        });
                    }
                    else
                    {
                        produtosParaEmbalar.Add(produto);
                    }
                }

                produtosParaEmbalar = produtosParaEmbalar.OrderByDescending(p => p.Dimensoes.Volume).ToList();

                while (produtosParaEmbalar.Any())
                {
                    EspecificacaoCaixa? melhorCaixaParaEstaIteracao = null;
                    List<Produto> produtosEmbaladosNestaIteracao = new List<Produto>();
                    double melhorPontuacao = -1;

                    foreach (var especCaixa in caixasDisponiveis.OrderBy(c => c.Volume))
                    {
                        var produtosTentativaAtual = new List<Produto>();
                        long volumePreenchidoAtual = 0;
                        var dimensoesDaCaixaAtual = new Dimensoes(especCaixa.Altura, especCaixa.Largura, especCaixa.Comprimento);

                        foreach (var produto in produtosParaEmbalar.OrderByDescending(p => p.Dimensoes.Volume))
                        {
                            if (produto.Dimensoes.CabeEm(dimensoesDaCaixaAtual) &&
                                (volumePreenchidoAtual + produto.Dimensoes.Volume) <= especCaixa.Volume)
                            {
                                produtosTentativaAtual.Add(produto);
                                volumePreenchidoAtual += produto.Dimensoes.Volume;
                            }
                        }

                        if (produtosTentativaAtual.Any())
                        {
                            double pontuacaoAtual = produtosTentativaAtual.Count;

                            if (pontuacaoAtual > melhorPontuacao)
                            {
                                melhorPontuacao = pontuacaoAtual;
                                melhorCaixaParaEstaIteracao = especCaixa;
                                produtosEmbaladosNestaIteracao = new List<Produto>(produtosTentativaAtual);
                            }
                            else if (pontuacaoAtual == melhorPontuacao && melhorCaixaParaEstaIteracao != null && especCaixa.Volume < melhorCaixaParaEstaIteracao.Volume)
                            {
                                melhorCaixaParaEstaIteracao = especCaixa;
                                produtosEmbaladosNestaIteracao = new List<Produto>(produtosTentativaAtual);
                            }
                        }
                    }

                    if (melhorCaixaParaEstaIteracao != null && produtosEmbaladosNestaIteracao.Any())
                    {
                        pedidoEmbaladoDto.Caixas.Add(new CaixaEmbaladaDTO
                        {
                            CaixaId = melhorCaixaParaEstaIteracao.Nome,
                            Produtos = produtosEmbaladosNestaIteracao.Select(p => p.ProdutoId).ToList()
                        });
                        produtosParaEmbalar.RemoveAll(p => produtosEmbaladosNestaIteracao.Contains(p));
                    }
                    else
                    {
                        if (produtosParaEmbalar.Any())
                        {
                            var primeiroRestante = produtosParaEmbalar.First();
                            var menorCaixaParaItem = caixasDisponiveis
                                .Where(ec => primeiroRestante.Dimensoes.CabeEm(new Dimensoes(ec.Altura, ec.Largura, ec.Comprimento)))
                                .OrderBy(ec => ec.Volume)
                                .FirstOrDefault();

                            if (menorCaixaParaItem != null)
                            {
                                pedidoEmbaladoDto.Caixas.Add(new CaixaEmbaladaDTO
                                {
                                    CaixaId = menorCaixaParaItem.Nome,
                                    Produtos = new List<string> { primeiroRestante.ProdutoId }
                                });
                                produtosParaEmbalar.Remove(primeiroRestante);
                                continue;
                            }
                            else
                            {
                                pedidoEmbaladoDto.Caixas.Add(new CaixaEmbaladaDTO
                                {
                                    CaixaId = null,
                                    Produtos = new List<string> { primeiroRestante.ProdutoId },
                                    Observacao = "Não foi possível alocar este produto (fallback falhou)."
                                });
                                produtosParaEmbalar.Remove(primeiroRestante);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                pedidosEmbalados.Add(pedidoEmbaladoDto);

                foreach (var caixaEmbalada in pedidoEmbaladoDto.Caixas)
                {
                    todosOsResultadosParaSalvar.Add(new ResultadoEmbalagem
                    {
                        Id = Guid.NewGuid(),
                        PedidoId = pedidoEmbaladoDto.PedidoId,
                        CaixaId = caixaEmbalada.CaixaId,
                        Produtos = string.Join(", ", caixaEmbalada.Produtos),
                        Observacao = caixaEmbalada.Observacao,
                        ProcessadoEm = DateTime.UtcNow
                    });
                }
            }

            if (todosOsResultadosParaSalvar.Any())
            {
                await _resultadoRepository.AdicionarVariosAsync(todosOsResultadosParaSalvar);
            }

            return new RespostaEmbalagemDTO { Pedidos = pedidosEmbalados };
        }
    }
}
