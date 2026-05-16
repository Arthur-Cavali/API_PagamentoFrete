using System.ComponentModel.DataAnnotations;

namespace Ftec.ProjetosWeb.Pagamento.Web.Models
{
    public class PagamentoViewModel
    {
        public Guid PagamentoId { get; set; }
        public Guid PedidoId { get; set; }
        public string CpfCliente { get; set; } = string.Empty;
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotal { get; set; }
        public int NumeroParcelas { get; set; }
        public decimal ValorParcela { get; set; }
        public int MetodoPagamento { get; set; }
        public int StatusPagamento { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? DataPagamento { get; set; }
    }

    public class CriarPagamentoViewModel
    {
        [Required] public Guid PedidoId { get; set; }
        [Required] public string CpfCliente { get; set; } = string.Empty;
        [Required] public decimal ValorProdutos { get; set; }
        [Required] public int MetodoPagamento { get; set; }
        public int NumeroParcelas { get; set; } = 1;
    }

    public class ProcessarGatewayViewModel
    {
        [Required] public Guid PagamentoId { get; set; }
        [Required] public decimal Valor { get; set; }
        [Required] public string RetornoGateway { get; set; } = string.Empty;
        [Required] public bool StatusTransacao { get; set; }
    }

    public class PedidoExternoViewModel
    {
        public Guid PedidoId { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class TransacaoPagamentoViewModel
    {
        public Guid IdTransacaoPagamento { get; set; }
        public Guid PagamentoId { get; set; }
        public decimal Valor { get; set; }
        public string RetornoGateway { get; set; } = string.Empty;
        public bool StatusTransacao { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}
