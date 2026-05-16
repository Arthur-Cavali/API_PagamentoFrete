using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter
{
    using Pagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.Pagamento;

    public static class PagamentoAdapter
    {
        public static Pagamento ParaEntidade(PagamentoDTO dto)
        {
            return new Pagamento
            {
                PagamentoId = dto.PagamentoId,
                PedidoId = dto.PedidoId,
                CpfCliente = dto.CpfCliente,
                ValorProdutos = dto.ValorProdutos,
                ValorFrete = dto.ValorFrete,
                ValorTotal = dto.ValorTotal,
                NumeroParcelas = dto.NumeroParcelas,
                ValorParcela = dto.ValorParcela,
                MetodoPagamento = dto.MetodoPagamento,
                StatusPagamento = dto.StatusPagamento,
                CriadoEm = dto.CriadoEm,
                DataPagamento = dto.DataPagamento
            };
        }

        public static PagamentoDTO ParaDTO(Pagamento pagamento)
        {
            return new PagamentoDTO
            {
                PagamentoId = pagamento.PagamentoId,
                PedidoId = pagamento.PedidoId,
                CpfCliente = pagamento.CpfCliente,
                ValorProdutos = pagamento.ValorProdutos,
                ValorFrete = pagamento.ValorFrete,
                ValorTotal = pagamento.ValorTotal,
                NumeroParcelas = pagamento.NumeroParcelas,
                ValorParcela = pagamento.ValorParcela,
                MetodoPagamento = pagamento.MetodoPagamento,
                StatusPagamento = pagamento.StatusPagamento,
                CriadoEm = pagamento.CriadoEm,
                DataPagamento = pagamento.DataPagamento
            };
        }

        public static TransacaoPagamento ParaEntidadeTransacao(TransacaoPagamentoDTO dto)
        {
            return new TransacaoPagamento
            {
                IdTransacaoPagamento = dto.IdTransacaoPagamento,
                PagamentoId = dto.PagamentoId,
                Valor = dto.Valor,
                RetornoGateway = dto.RetornoGateway,
                StatusTransacao = dto.StatusTransacao,
                DataTransacao = dto.DataTransacao
            };
        }

        public static TransacaoPagamentoDTO ParaDTOTransacao(TransacaoPagamento transacao)
        {
            return new TransacaoPagamentoDTO
            {
                IdTransacaoPagamento = transacao.IdTransacaoPagamento,
                PagamentoId = transacao.PagamentoId,
                Valor = transacao.Valor,
                RetornoGateway = transacao.RetornoGateway,
                StatusTransacao = transacao.StatusTransacao,
                DataTransacao = transacao.DataTransacao
            };
        }
    }
}
