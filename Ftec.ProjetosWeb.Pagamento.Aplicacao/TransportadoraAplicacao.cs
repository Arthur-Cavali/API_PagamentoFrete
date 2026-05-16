using Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Ftec.ProjetosWeb.Pagamento.Persistencia;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao
{
    public class TransportadoraAplicacao
    {
        ITransportadoraRepositorio transportadoraRepositorio;

        public TransportadoraAplicacao(string strConexao)
        {
            transportadoraRepositorio = new TransportadoraRepositorio(strConexao);
        }

        public TransportadoraDTO CadastrarTransportadora(TransportadoraDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                throw new Exception("O nome da transportadora é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.CodigoServico))
                throw new Exception("O código do serviço é obrigatório.");

            dto.Ativo = true;
            Transportadora entidade = TransportadoraAdapter.ParaEntidade(dto);
            transportadoraRepositorio.Inserir(entidade);

            return TransportadoraAdapter.ParaDTO(entidade);
        }

        public TransportadoraDTO ObterTransportadora(Guid id)
        {
            Transportadora transportadora = transportadoraRepositorio.Procurar(id);
            if (transportadora == null)
                throw new Exception("Transportadora não encontrada.");

            return TransportadoraAdapter.ParaDTO(transportadora);
        }

        public List<TransportadoraDTO> ListarTransportadoras()
        {
            List<Transportadora> lista = transportadoraRepositorio.ProcurarTodos();
            var dtos = new List<TransportadoraDTO>();
            foreach (Transportadora t in lista)
                dtos.Add(TransportadoraAdapter.ParaDTO(t));
            return dtos;
        }

        public void AtivarDesativar(Guid id, bool ativo)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id da transportadora é obrigatório.");

            var transportadora = transportadoraRepositorio.Procurar(id);
            if (transportadora == null)
                throw new Exception("Transportadora não encontrada.");

            transportadoraRepositorio.AtivarDesativar(id, ativo);
        }
    }
}
