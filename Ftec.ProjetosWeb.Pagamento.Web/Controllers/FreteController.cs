using Ftec.ProjetosWeb.Pagamento.Web.Models;
using Ftec.ProjetosWeb.Pagamento.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.Pagamento.Web.Controllers
{
    public class FreteController : Controller
    {
        private readonly APIHttpClient api;

        public FreteController(APIHttpClient api)
        {
            this.api = api;
        }

        public IActionResult Index()
        {
            try
            {
                var fretes = api.Get<List<FreteViewModel>>("frete");
                return View(fretes);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
                return View(new List<FreteViewModel>());
            }
        }

        public IActionResult Detalhes(Guid id)
        {
            try
            {
                var frete = api.Get<FreteViewModel>("frete/" + id);
                return View(frete);
            }
            catch
            {
                return NotFound();
            }
        }

        public IActionResult Calcular()
        {
            CarregarTransportadoras();
            return View(new CalcularFreteViewModel());
        }

        public IActionResult BuscarPedido(Guid pedidoId)
        {
            var model = new CalcularFreteViewModel { PedidoId = pedidoId, EnderecoEntregaId = pedidoId };
            try
            {
                var endereco = api.Get<EnderecoPedidoViewModel>("pedido-externo/" + pedidoId + "/endereco");
                model.CepDestino = endereco.CepDestino;
                model.Numero = endereco.Numero;
                TempData["Sucesso"] = "Pedido encontrado. CEP de entrega preenchido automaticamente.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Não foi possível buscar o pedido: " + ex.Message;
            }
            CarregarTransportadoras();
            return View("Calcular", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calcular(CalcularFreteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                CarregarTransportadoras();
                return View(model);
            }
            try
            {
                var frete = api.PostRetorno<CalcularFreteViewModel, FreteViewModel>("frete/calcular", model);
                TempData["Sucesso"] = "Frete calculado com sucesso.";
                return RedirectToAction(nameof(Detalhes), new { id = frete.IdFrete });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CarregarTransportadoras();
                return View(model);
            }
        }

        private void CarregarTransportadoras()
        {
            try { ViewBag.Transportadoras = api.Get<List<TransportadoraViewModel>>("transportadora"); }
            catch { ViewBag.Transportadoras = new List<TransportadoraViewModel>(); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Confirmar(Guid id)
        {
            try
            {
                api.Post("frete/" + id + "/confirmar");
                TempData["Sucesso"] = "Frete confirmado. Status: Preparando.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        public IActionResult Enviar(Guid id)
        {
            return View(new EnviarFreteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enviar(Guid id, EnviarFreteViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                api.Post("frete/" + id + "/enviar", model);
                TempData["Sucesso"] = "Frete enviado. Status: Enviado.";
                return RedirectToAction(nameof(Detalhes), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EmTransito(Guid id)
        {
            try
            {
                api.Post("frete/" + id + "/em-transito");
                TempData["Sucesso"] = "Frete em trânsito.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Detalhes), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Entregar(Guid id)
        {
            try
            {
                api.Post("frete/" + id + "/entregar");
                TempData["Sucesso"] = "Frete entregue.";
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
                api.Delete("frete/", id);
                TempData["Sucesso"] = "Frete cancelado.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
