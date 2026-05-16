using Ftec.ProjetosWeb.Pagamento.Web.Models;
using Ftec.ProjetosWeb.Pagamento.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.Pagamento.Web.Controllers
{
    public class TransportadoraController : Controller
    {
        private readonly APIHttpClient api;

        public TransportadoraController(APIHttpClient api)
        {
            this.api = api;
        }

        public IActionResult Index()
        {
            try
            {
                var transportadoras = api.Get<List<TransportadoraViewModel>>("transportadora");
                return View(transportadoras);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
                return View(new List<TransportadoraViewModel>());
            }
        }

        public IActionResult Criar()
        {
            return View(new CriarTransportadoraViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(CriarTransportadoraViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                api.PostRetorno<CriarTransportadoraViewModel, TransportadoraViewModel>("transportadora", model);
                TempData["Sucesso"] = "Transportadora cadastrada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ativar(Guid id)
        {
            try
            {
                api.Post("transportadora/" + id + "/ativar");
                TempData["Sucesso"] = "Transportadora ativada.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Desativar(Guid id)
        {
            try
            {
                api.Post("transportadora/" + id + "/desativar");
                TempData["Sucesso"] = "Transportadora desativada.";
            }
            catch (Exception ex)
            {
                TempData["Erro"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
