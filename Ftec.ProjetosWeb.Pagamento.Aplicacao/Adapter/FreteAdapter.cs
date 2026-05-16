using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter
{
    public static class FreteAdapter
    {
        public static Frete ParaEntidade(FreteDTO dto)
        {
            return new Frete
            {
                IdFrete = dto.IdFrete,
                PedidoId = dto.PedidoId,
                EnderecoEntregaId = dto.EnderecoEntregaId,
                TransportadoraId = dto.TransportadoraId,
                NomeTransportadora = dto.NomeTransportadora,
                ValorFrete = dto.ValorFrete,
                CodigoRastreio = dto.CodigoRastreio,
                CriadoEm = dto.CriadoEm,
                DataEnvio = dto.DataEnvio,
                DataEntrega = dto.DataEntrega,
                PrazoEntrega = dto.PrazoEntrega,
                StatusEntrega = dto.StatusEntrega,
                Logradouro = dto.Logradouro,
                Numero = dto.Numero,
                Complemento = dto.Complemento,
                Bairro = dto.Bairro,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CepDestino = dto.CepDestino
            };
        }

        public static FreteDTO ParaDTO(Frete frete)
        {
            return new FreteDTO
            {
                IdFrete = frete.IdFrete,
                PedidoId = frete.PedidoId,
                EnderecoEntregaId = frete.EnderecoEntregaId,
                TransportadoraId = frete.TransportadoraId,
                NomeTransportadora = frete.NomeTransportadora,
                ValorFrete = frete.ValorFrete,
                CodigoRastreio = frete.CodigoRastreio,
                CriadoEm = frete.CriadoEm,
                DataEnvio = frete.DataEnvio,
                DataEntrega = frete.DataEntrega,
                PrazoEntrega = frete.PrazoEntrega,
                StatusEntrega = frete.StatusEntrega,
                Logradouro = frete.Logradouro,
                Numero = frete.Numero,
                Complemento = frete.Complemento,
                Bairro = frete.Bairro,
                Cidade = frete.Cidade,
                Estado = frete.Estado,
                CepDestino = frete.CepDestino
            };
        }
    }
}
