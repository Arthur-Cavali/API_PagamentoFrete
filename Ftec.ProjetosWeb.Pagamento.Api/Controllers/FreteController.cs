using Ftec.ProjetosWeb.Pagamento.Aplicacao;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ftec.ProjetosWeb.Pagamento.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreteController : ControllerBase
    {
        FreteAplicacao freteAplicacao;
        private readonly IConfiguration _config;

        public FreteController(IConfiguration config)
        {
            _config = config;
            freteAplicacao = new FreteAplicacao(config["strConexao"]);
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var fretes = freteAplicacao.ListarFretes();
                return Ok(fretes);
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
                var frete = freteAplicacao.ObterFrete(id);
                return Ok(frete);
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

        [HttpGet("pedido/{pedidoId:guid}")]
        public IActionResult GetPorPedido(Guid pedidoId)
        {
            try
            {
                var frete = freteAplicacao.ObterFretePorPedido(pedidoId);
                return Ok(frete);
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

        [HttpPost("calcular")]
        public IActionResult Calcular([FromBody] FreteCalculoDTO calculo)
        {
            try
            {
                calculo.CepOrigem = _config["CepOrigemCD"] ?? "90000-000";
                var frete = freteAplicacao.CalcularFrete(calculo);
                return Created(string.Empty, frete);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Fluxo de status: Pendente → Preparando → Enviado → EmTransito → Entregue

        [HttpPost("{id:guid}/confirmar")]
        public IActionResult Confirmar(Guid id)
        {
            try
            {
                freteAplicacao.ConfirmarFrete(id);
                return Ok(new { mensagem = "Frete confirmado. Status: Preparando." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/enviar")]
        public IActionResult Enviar(Guid id, [FromBody] FreteEnvioDTO envio)
        {
            try
            {
                freteAplicacao.EnviarFrete(id, envio);
                return Ok(new { mensagem = "Frete enviado ao transportador. Status: Enviado." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/em-transito")]
        public IActionResult EmTransito(Guid id)
        {
            try
            {
                freteAplicacao.MarcarEmTransito(id);
                return Ok(new { mensagem = "Frete em trânsito. Status: EmTransito." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/entregar")]
        public IActionResult Entregar(Guid id)
        {
            try
            {
                freteAplicacao.MarcarEntregue(id);
                return Ok(new { mensagem = "Frete entregue ao destinatário. Status: Entregue." });
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
                freteAplicacao.CancelarFrete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
