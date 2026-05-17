using Ftec.ProjetosWeb.Pagamento.Persistencia;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.Pagamento.Api.Controllers
{
    //[Route("api/pedido-externo")]
    [Route("http://pedido.neurosky.com.br/api/pedido")]
    [ApiController]
    public class PedidoExternoController : ControllerBase
    {
        private readonly IConfiguration config;

        public PedidoExternoController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet("{pedidoId:guid}")]
        public IActionResult Get(Guid pedidoId)
        {
            try
            {
                var repo = new PedidoExternoRepositorio(
                    config["strConexaoPedidos"]!,
                    config["strConexaoProdutos"]!);

                var valorTotal = repo.CalcularValorPedido(pedidoId);
                return Ok(new { pedidoId, valorTotal });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{pedidoId:guid}/endereco")]
        public IActionResult GetEndereco(Guid pedidoId)
        {
            try
            {
                var repo = new PedidoExternoRepositorio(
                    config["strConexaoPedidos"]!,
                    config["strConexaoProdutos"]!);

                var (cepDestino, numero) = repo.ObterEnderecoPedido(pedidoId);
                return Ok(new { cepDestino, numero });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
