
namespace Ftec.ProjetosWeb.Pagamento.Dominio.Entidades
{
    public class TransacaoPagamento
    {
        public Guid IdTransacaoPagamento { get; set; }

        // FK do pagamento
        public Guid PagamentoId { get; set; }

        // Valor processado na transação
        public decimal Valor { get; set; }

        // Retorno do gateway
        public string RetornoGateway { get; set; }

        // Indica se a transação foi aprovada
        public bool StatusTransacao { get; set; }

        // Data da transação
        public DateTime DataTransacao { get; set; }

        // Navegação
        public Pagamento Pagamento { get; set; }
    }
}
