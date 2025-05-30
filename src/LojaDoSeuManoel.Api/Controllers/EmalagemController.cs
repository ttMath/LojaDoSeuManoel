using LojaDoSeuManoel.Application.DTOs;
using LojaDoSeuManoel.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaDoSeuManoel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmbalagemController : ControllerBase
    {
        private readonly IServicoEmbalagem _servicoEmbalagem;
        public EmbalagemController(IServicoEmbalagem servicoEmbalagem, ILogger<EmbalagemController> logger)
        {
            _servicoEmbalagem = servicoEmbalagem;
        }

        [HttpPost("processar")]
        public async Task<IActionResult> ProcessarEmbalagem([FromBody] RequisicaoEmbalagemDTO requisicao)
        {
            if (requisicao == null || !requisicao.Pedidos.Any()) return BadRequest("Requisição inválida.");

            var resposta = await _servicoEmbalagem.ProcessarRequisicaoEmbalagem(requisicao);
            return Ok(resposta);
        }
    }
}
