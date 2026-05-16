using System;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Entidades
{
    public class Transportadora
    {
        public Guid TransportadoraId { get; set; }
        public string Nome { get; set; }
        public string CodigoServico { get; set; }
        public decimal ValorBase { get; set; }
        public decimal ValorPorKg { get; set; }
        public int PrazoMinDias { get; set; }
        public int PrazoMaxDias { get; set; }
        public bool Ativo { get; set; }
    }
}
