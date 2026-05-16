using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces
{
    using Pagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.Pagamento;
    using TransacaoPagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.TransacaoPagamento;

    public interface IPagamentoRepositorio
    {
        void Inserir(Pagamento pagamento);
        void AtualizarStatus(Guid id, StatusPagamento novoStatus);
        void Cancelar(Guid id);
        Pagamento Procurar(Guid id);
        Pagamento ProcurarPorPedido(Guid pedidoId);
        List<Pagamento> ProcurarTodos();
        void InserirTransacao(TransacaoPagamento transacao);
    }
}
