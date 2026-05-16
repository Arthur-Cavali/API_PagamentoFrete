using System.ComponentModel.DataAnnotations;

namespace Ftec.ProjetosWeb.Pagamento.Web.Models
{
    public class TransportadoraViewModel
    {
        public Guid TransportadoraId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CodigoServico { get; set; } = string.Empty;
        public decimal ValorBase { get; set; }
        public decimal ValorPorKg { get; set; }
        public int PrazoMinDias { get; set; }
        public int PrazoMaxDias { get; set; }
        public bool Ativo { get; set; }
    }

    public class CriarTransportadoraViewModel
    {
        [Required] public string Nome { get; set; } = string.Empty;
        [Required] public string CodigoServico { get; set; } = string.Empty;
        [Required] public decimal ValorBase { get; set; }
        [Required] public decimal ValorPorKg { get; set; }
        [Required] public int PrazoMinDias { get; set; }
        [Required] public int PrazoMaxDias { get; set; }
    }
}
