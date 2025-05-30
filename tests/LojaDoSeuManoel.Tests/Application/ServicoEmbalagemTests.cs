using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services;
using LojaDoSeuManoel.Domain.Entities;
using LojaDoSeuManoel.Domain.Interfaces;
using Moq;
using Xunit;

namespace LojaDoSeuManoel.Tests.Application
{
    public class ServicoEmbalagemTests
    {
        private readonly Mock<ICaixaRepository> _caixaRepoMock;
        private readonly Mock<IResultadoEmbalagemRepository> _resultadoRepoMock;
        private readonly ServicoEmbalagem _servico;

        public ServicoEmbalagemTests()
        {
            _caixaRepoMock = new Mock<ICaixaRepository>();
            _resultadoRepoMock = new Mock<IResultadoEmbalagemRepository>();

            var caixasPadrao = new List<EspecificacaoCaixa>
            {
            new() { Id = 1, Nome = "Caixa 1", Altura = 30, Largura = 40, Comprimento = 80 },
            new() { Id = 2, Nome = "Caixa 2", Altura = 80, Largura = 50, Comprimento = 40 },
            new() { Id = 3, Nome = "Caixa 3", Altura = 50, Largura = 80, Comprimento = 60 }
            };
            _caixaRepoMock.Setup(repo => repo.ObterTodasAsync()).ReturnsAsync(caixasPadrao);

            _servico = new ServicoEmbalagem(_caixaRepoMock.Object, _resultadoRepoMock.Object);
        }

        [Fact]
        public async Task ProcessarRequisicao_QuandoProdutoNaoCabeEmNenhumaCaixa_DeveRetornarObservacaoCorreta()
        {
            var requisicao = new RequisicaoEmbalagemDTO
            {
                Pedidos = new List<PedidoRequestDTO>
                {
                    new()
                    {
                        PedidoId = 1,
                        Produtos = new List<ProdutoDTO>
                        {
                            new() { ProdutoId = "Cadeira Gamer Gigante", Dimensoes = new DimensaoDTO { Altura = 120, Largura = 90, Comprimento = 90 } }
                        }
                    }
                }
            };

            var resultado = await _servico.ProcessarRequisicaoEmbalagem(requisicao);

            var pedidoResultado = Assert.Single(resultado.Pedidos);
            var caixaResultado = Assert.Single(pedidoResultado.Caixas);

            Assert.NotNull(caixaResultado);
            Assert.Null(caixaResultado.CaixaId);
            Assert.Equal("Produto não cabe em nenhuma caixa disponível.", caixaResultado.Observacao);

            _resultadoRepoMock.Verify(r => r.AdicionarVariosAsync(It.IsAny<IEnumerable<ResultadoEmbalagem>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessarRequisicao_QuandoDoisProdutosCabemJuntos_DeveAgrupaLosNaMesmaCaixa()
        {
            var requisicao = new RequisicaoEmbalagemDTO
            {
                Pedidos = new List<PedidoRequestDTO>
            {
                new() {
                    PedidoId = 2,
                    Produtos = new List<ProdutoDTO>
                    {
                        new() { ProdutoId = "PS5", Dimensoes = new DimensaoDTO { Altura = 40, Largura = 10, Comprimento = 25 } },
                        new() { ProdutoId = "Volante", Dimensoes = new DimensaoDTO { Altura = 40, Largura = 30, Comprimento = 30 } }
                    }
                }
            }
            };

            var resultado = await _servico.ProcessarRequisicaoEmbalagem(requisicao);

            var pedidoResultado = Assert.Single(resultado.Pedidos);
            var caixaResultado = Assert.Single(pedidoResultado.Caixas);

            Assert.Equal("Caixa 1", caixaResultado.CaixaId);
            Assert.Contains("PS5", caixaResultado.Produtos);
            Assert.Contains("Volante", caixaResultado.Produtos);
            Assert.Equal(2, caixaResultado.Produtos.Count);

            _resultadoRepoMock.Verify(r => r.AdicionarVariosAsync(It.IsAny<IEnumerable<ResultadoEmbalagem>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessarRequisicao_ProdutosGrandesEMedios_DeveRetornarDuasCaixas()
        {
            var requisicao = new RequisicaoEmbalagemDTO
            {
                Pedidos = new List<PedidoRequestDTO>
                {
                    new()
                    {
                        PedidoId = 6,
                        Produtos = new()
                        {
                            new() { ProdutoId = "MonitorPainelCurvo", Dimensoes = new DimensaoDTO { Altura = 45, Largura = 75, Comprimento = 55 } },
                            new() { ProdutoId = "SoundbarLonga", Dimensoes = new DimensaoDTO { Altura = 20, Largura = 70, Comprimento = 30 } },
                            new() { ProdutoId = "BaseVentiladaNotebook", Dimensoes = new DimensaoDTO { Altura = 20, Largura = 70, Comprimento = 30 } }
                        }
                    }
                }
            };

            var resultado = await _servico.ProcessarRequisicaoEmbalagem(requisicao);

            var pedidoResultado = Assert.Single(resultado.Pedidos);
            Assert.Equal(2, pedidoResultado.Caixas.Count);

            var caixaDoMonitor = Assert.Single(pedidoResultado.Caixas, c => c.Produtos.Contains("MonitorPainelCurvo"));
            Assert.NotNull(caixaDoMonitor);
            Assert.Equal("Caixa 3", caixaDoMonitor.CaixaId);
            Assert.Single(caixaDoMonitor.Produtos);

            var caixaDosMedios = Assert.Single(pedidoResultado.Caixas, c => c.Produtos.Contains("SoundbarLonga"));
            Assert.NotNull(caixaDosMedios);
            Assert.Equal("Caixa 1", caixaDosMedios.CaixaId);
            Assert.Contains("SoundbarLonga", caixaDosMedios.Produtos);
            Assert.Contains("BaseVentiladaNotebook", caixaDosMedios.Produtos);
            Assert.Equal(2, caixaDosMedios.Produtos.Count);

            _resultadoRepoMock.Verify(r => r.AdicionarVariosAsync(It.IsAny<IEnumerable<ResultadoEmbalagem>>()), Times.Once);
        }
    }
}
