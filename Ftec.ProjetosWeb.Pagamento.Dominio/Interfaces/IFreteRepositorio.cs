using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces
{
    public interface IFreteRepositorio
    {
        Guid Inserir(Frete frete);
        void AtualizarStatus(Guid id, StatusEntrega novoStatus);
        void Enviar(Guid id, string codigoRastreio);
        Frete Procurar(Guid id);
        Frete ProcurarPorPedido(Guid pedidoId);
        List<Frete> ProcurarTodos();
    }
}
