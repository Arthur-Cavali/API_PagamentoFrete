using Ftec.ProjetosWeb.Pagamento.Aplicacao;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ftec.ProjetosWeb.Pagamento.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportadoraController : ControllerBase
    {
        TransportadoraAplicacao transportadoraAplicacao;

        public TransportadoraController(IConfiguration config)
        {
            transportadoraAplicacao = new TransportadoraAplicacao(config["strConexao"]);
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var lista = transportadoraAplicacao.ListarTransportadoras();
                return Ok(lista);
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
                var transportadora = transportadoraAplicacao.ObterTransportadora(id);
                return Ok(transportadora);
            }
            catch (Exception ex) when (ex.Message.Contains("não encontrada"))
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] TransportadoraDTO dto)
        {
            try
            {
                var criada = transportadoraAplicacao.CadastrarTransportadora(dto);
                return Created(string.Empty, criada);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/ativar")]
        public IActionResult Ativar(Guid id)
        {
            try
            {
                transportadoraAplicacao.AtivarDesativar(id, true);
                return Ok(new { mensagem = "Transportadora ativada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id:guid}/desativar")]
        public IActionResult Desativar(Guid id)
        {
            try
            {
                transportadoraAplicacao.AtivarDesativar(id, false);
                return Ok(new { mensagem = "Transportadora desativada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
