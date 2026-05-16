using System;
using System.Collections.Generic;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Entidades
{
    public class Pagamento
    {
        public Guid PagamentoId { get; set; }
        public Guid PedidoId { get; set; }
        public string CpfCliente { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotal { get; set; }
        public int NumeroParcelas { get; set; }
        public decimal ValorParcela { get; set; }
        public MetodoPagamento MetodoPagamento { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? DataPagamento { get; set; }

        public ICollection<TransacaoPagamento> Transacoes { get; set; }
            = new List<TransacaoPagamento>();
    }
}
