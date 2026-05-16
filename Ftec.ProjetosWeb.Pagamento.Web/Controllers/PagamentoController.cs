using Ftec.ProjetosWeb.Pagamento.Web.Models;
using Ftec.ProjetosWeb.Pagamento.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.Pagamento.Web.Controllers
{
    public class PagamentoController : Controller
    {
        private readonly APIHttpClient api;

        public PagamentoController(APIHttpClient api)
        {
            this.api = api;
        }

        public IActionResult Index()
        {
            try
            {
                var pagamentos = api.Get<List<PagamentoViewModel>>("pagamento");
                return View(pagamentos);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
                return View(new List<PagamentoViewModel>());
            }
        }

        public IActionResult Detalhes(Guid id)
        {
            try
            {
                var pagamento = api.Get<PagamentoViewModel>("pagamento/" + id);
                return View(pagamento);
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult Criar()
        {
            return View(new CriarPagamentoViewModel());
        }

        public IActionResult BuscarPedido(Guid pedidoId)
        {
            var model = new CriarPagamentoViewModel { PedidoId = pedidoId };
            try
            {
                var resultado = api.Get<PedidoExternoViewModel>("pedido-externo/" + pedidoId);
                model.ValorProdutos = resultado.ValorTotal;
                TempData["Sucesso"] = $"Pedido encontrado. Valor dos produtos: R$ {resultado.ValorTotal:F2}";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível buscar o pedido: " + ex.Message;
            }
            return View("Criar", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(CriarPagamentoViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var criado = api.PostRetorno<CriarPagamentoViewModel, PagamentoViewModel>("pagamento", model);
                TempData["Sucesso"] = "Pagamento criado com sucesso.";
                return RedirectToAction(nameof(Detalhes), new { id = criado.PagamentoId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Aprovar(Guid id)
        {
            try
            {
                var pagamento = api.Get<PagamentoViewModel>("pagamento/" + id);
                var transacao = new ProcessarGatewayViewModel
                {
                    PagamentoId = id,
                    Valor = pagamento.ValorTotal,
                    RetornoGateway = "Aprovado",
                    StatusTransacao = true
                };
                api.PostRetorno<ProcessarGatewayViewModel, TransacaoPagamentoViewModel>("pagamento/transacao", transacao);
                TempData["Sucesso"] = "Pagamento aprovado com sucesso.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Recusar(Guid id)
        {
            try
            {
                var pagamento = api.Get<PagamentoViewModel>("pagamento/" + id);
                var transacao = new ProcessarGatewayViewModel
                {
                    PagamentoId = id,
                    Valor = pagamento.ValorTotal,
                    RetornoGateway = "Recusado",
                    StatusTransacao = false
                };
                api.PostRetorno<ProcessarGatewayViewModel, TransacaoPagamentoViewModel>("pagamento/transacao", transacao);
                TempData["Erro"] = "Pagamento recusado.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(Guid id)
        {
            try
            {
                api.Delete("pagamento/", id);
                TempData["Sucesso"] = "Pagamento cancelado.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
