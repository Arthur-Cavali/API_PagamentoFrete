using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces
{
    public interface ITransportadoraRepositorio
    {
        void Inserir(Transportadora transportadora);
        Transportadora Procurar(Guid id);
        Transportadora ProcurarPorCodigoServico(string codigoServico);
        List<Transportadora> ProcurarTodos();
        void AtivarDesativar(Guid id, bool ativo);
    }
}
