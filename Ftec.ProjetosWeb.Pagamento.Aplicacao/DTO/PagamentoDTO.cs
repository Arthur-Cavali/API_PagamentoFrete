using System;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO
{
    public class PagamentoDTO
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
    }

    public class TransacaoPagamentoDTO
    {
        public Guid IdTransacaoPagamento { get; set; }
        public Guid PagamentoId { get; set; }
        public decimal Valor { get; set; }
        public string RetornoGateway { get; set; }
        public bool StatusTransacao { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}
