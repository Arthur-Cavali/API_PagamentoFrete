using Ftec.ProjetosWeb.Pagamento.Aplicacao;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ftec.ProjetosWeb.Pagamento.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagamentoController : ControllerBase
    {
        PagamentoAplicacao pagamentoAplicacao;

        public PagamentoController(IConfiguration config)
        {
            pagamentoAplicacao = new PagamentoAplicacao(config["strConexao"]);
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var pagamentos = pagamentoAplicacao.ListarPagamentos();
                return Ok(pagamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var pagamento = pagamentoAplicacao.ObterPagamento(id);
                return Ok(pagamento);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] PagamentoDTO pagamento)
        {
            try
            {
                var pagamentoCriado = pagamentoAplicacao.RegistrarPagamento(pagamento);
                return Created(string.Empty, pagamentoCriado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("transacao")]
        public IActionResult Transacao([FromBody] TransacaoPagamentoDTO transacao)
        {
            try
            {
                var resultado = pagamentoAplicacao.ProcessarGateway(transacao);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                pagamentoAplicacao.CancelarPagamento(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
