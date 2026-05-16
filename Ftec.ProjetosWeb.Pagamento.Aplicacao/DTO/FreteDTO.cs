using System;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO
{
    public class FreteDTO
    {
        public Guid IdFrete { get; set; }
        public Guid PedidoId { get; set; }
        public Guid EnderecoEntregaId { get; set; }
        public Guid TransportadoraId { get; set; }
        public string NomeTransportadora { get; set; }
        public decimal ValorFrete { get; set; }
        public string? CodigoRastreio { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? DataEnvio { get; set; }
        public DateTime? DataEntrega { get; set; }
        public int PrazoEntrega { get; set; }
        public StatusEntrega StatusEntrega { get; set; }
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CepDestino { get; set; } = string.Empty;
    }

    public class FreteCalculoDTO
    {
        public Guid PedidoId { get; set; }
        public Guid EnderecoEntregaId { get; set; }
        public Guid TransportadoraId { get; set; }
        public string CepOrigem { get; set; } = string.Empty;
        public string CepDestino { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class FreteEnvioDTO
    {
        public string CodigoRastreio { get; set; }
    }
}
