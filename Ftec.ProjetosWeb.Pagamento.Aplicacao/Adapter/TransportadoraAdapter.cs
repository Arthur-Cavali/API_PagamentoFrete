using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter
{
    public static class TransportadoraAdapter
    {
        public static Transportadora ParaEntidade(TransportadoraDTO dto)
        {
            return new Transportadora
            {
                TransportadoraId = dto.TransportadoraId,
                Nome = dto.Nome,
                CodigoServico = dto.CodigoServico,
                ValorBase = dto.ValorBase,
                ValorPorKg = dto.ValorPorKg,
                PrazoMinDias = dto.PrazoMinDias,
                PrazoMaxDias = dto.PrazoMaxDias,
                Ativo = dto.Ativo
            };
        }

        public static TransportadoraDTO ParaDTO(Transportadora transportadora)
        {
            return new TransportadoraDTO
            {
                TransportadoraId = transportadora.TransportadoraId,
                Nome = transportadora.Nome,
                CodigoServico = transportadora.CodigoServico,
                ValorBase = transportadora.ValorBase,
                ValorPorKg = transportadora.ValorPorKg,
                PrazoMinDias = transportadora.PrazoMinDias,
                PrazoMaxDias = transportadora.PrazoMaxDias,
                Ativo = transportadora.Ativo
            };
        }
    }
}
