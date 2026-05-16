using Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Ftec.ProjetosWeb.Pagamento.Persistencia;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao
{
    using Pagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.Pagamento;

    public class PagamentoAplicacao
    {
        IPagamentoRepositorio pagamentoRepositorio;
        IFreteRepositorio freteRepositorio;

        public PagamentoAplicacao(string strConexao)
        {
            pagamentoRepositorio = new PagamentoRepositorio(strConexao);
            freteRepositorio = new FreteRepositorio(strConexao);
        }

        public PagamentoDTO RegistrarPagamento(PagamentoDTO pagamentoDto)
        {
            if (pagamentoDto.PedidoId == Guid.Empty)
                throw new Exception("O Id do pedido é obrigatório.");

            if (string.IsNullOrEmpty(pagamentoDto.CpfCliente))
                throw new Exception("O CPF do cliente é obrigatório.");

            if (pagamentoDto.ValorProdutos <= 0)
                throw new Exception("O valor dos produtos deve ser maior que zero.");

            Frete frete = freteRepositorio.ProcurarPorPedido(pagamentoDto.PedidoId);
            if (frete == null)
                throw new Exception("Frete ainda não calculado para este pedido. Calcule o frete antes de registrar o pagamento.");

            if (pagamentoDto.MetodoPagamento == MetodoPagamento.CartaoCredito)
            {
                int parcelas = pagamentoDto.NumeroParcelas;
                if (parcelas < 1 || parcelas > 12)
                    throw new Exception("Número de parcelas inválido. Cartão de crédito aceita de 1 a 12 parcelas.");
            }
            else
            {
                pagamentoDto.NumeroParcelas = 1;
            }

            Pagamento pag = PagamentoAdapter.ParaEntidade(pagamentoDto);
            pag.ValorFrete = frete.ValorFrete;
            pag.ValorTotal = pag.ValorProdutos + pag.ValorFrete;

            // Calcula parcelas e juros
            if (pag.MetodoPagamento == MetodoPagamento.CartaoCredito && pag.NumeroParcelas > 6)
            {
                // Juros compostos de 2,2% ao mês para 7 a 12 parcelas
                decimal totalComJuros = pag.ValorTotal * (decimal)Math.Pow(1.022, pag.NumeroParcelas);
                pag.ValorTotal = Math.Round(totalComJuros, 2);
            }

            pag.ValorParcela = pag.NumeroParcelas > 0
                ? Math.Round(pag.ValorTotal / pag.NumeroParcelas, 2)
                : pag.ValorTotal;

            pag.StatusPagamento = StatusPagamento.Pendente;
            pag.DataPagamento = null;

            pagamentoRepositorio.Inserir(pag);

            return PagamentoAdapter.ParaDTO(pag);
        }

        public void CancelarPagamento(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do pagamento é obrigatório.");

            pagamentoRepositorio.Cancelar(id);
        }

        public TransacaoPagamentoDTO ProcessarGateway(TransacaoPagamentoDTO transacaoDto)
        {
            if (transacaoDto.PagamentoId == Guid.Empty)
                throw new Exception("O Id do pagamento é obrigatório.");

            if (string.IsNullOrEmpty(transacaoDto.RetornoGateway))
                throw new Exception("O retorno do gateway é obrigatório.");

            pagamentoRepositorio.AtualizarStatus(transacaoDto.PagamentoId, StatusPagamento.Processando);

            TransacaoPagamento transacao = PagamentoAdapter.ParaEntidadeTransacao(transacaoDto);
            pagamentoRepositorio.InserirTransacao(transacao);

            StatusPagamento novoStatus = transacao.StatusTransacao
                ? StatusPagamento.Pago
                : StatusPagamento.Recusado;

            pagamentoRepositorio.AtualizarStatus(transacao.PagamentoId, novoStatus);

            return PagamentoAdapter.ParaDTOTransacao(transacao);
        }

        public PagamentoDTO ObterPagamento(Guid id)
        {
            Pagamento pag = pagamentoRepositorio.Procurar(id);
            if (pag == null)
                throw new Exception("Pagamento não encontrado.");

            return PagamentoAdapter.ParaDTO(pag);
        }

        public List<PagamentoDTO> ListarPagamentos()
        {
            List<Pagamento> pagamentos = pagamentoRepositorio.ProcurarTodos();
            var dtos = new List<PagamentoDTO>();
            foreach (Pagamento pag in pagamentos)
                dtos.Add(PagamentoAdapter.ParaDTO(pag));
            return dtos;
        }
    }
}
