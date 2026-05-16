using System.ComponentModel.DataAnnotations;

namespace Ftec.ProjetosWeb.Pagamento.Web.Models
{
    public class FreteViewModel
    {
        public Guid IdFrete { get; set; }
        public Guid PedidoId { get; set; }
        public Guid EnderecoEntregaId { get; set; }
        public Guid TransportadoraId { get; set; }
        public string NomeTransportadora { get; set; } = string.Empty;
        public decimal ValorFrete { get; set; }
        public string? CodigoRastreio { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? DataEnvio { get; set; }
        public DateTime? DataEntrega { get; set; }
        public int PrazoEntrega { get; set; }
        public int StatusEntrega { get; set; }
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CepDestino { get; set; } = string.Empty;
    }

    public class CalcularFreteViewModel
    {
        [Required] public Guid PedidoId { get; set; }
        [Required] public Guid EnderecoEntregaId { get; set; }
        [Required] public Guid TransportadoraId { get; set; }
        [Required] public string CepDestino { get; set; } = string.Empty;
        [Required] public string Logradouro { get; set; } = string.Empty;
        [Required] public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        [Required] public string Bairro { get; set; } = string.Empty;
        [Required] public string Cidade { get; set; } = string.Empty;
        [Required] public string Estado { get; set; } = string.Empty;
    }

    public class EnderecoPedidoViewModel
    {
        public string CepDestino { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
    }

    public class EnviarFreteViewModel
    {
        [Required] public string CodigoRastreio { get; set; } = string.Empty;
    }
}
