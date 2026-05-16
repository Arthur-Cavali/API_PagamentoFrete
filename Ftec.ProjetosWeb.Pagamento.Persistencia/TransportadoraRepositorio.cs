using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Persistencia
{
    public class TransportadoraRepositorio : ITransportadoraRepositorio
    {
        private string stringConexao;

        public TransportadoraRepositorio(string strConexao)
        {
            stringConexao = strConexao;
        }

        public void Inserir(Transportadora transportadora)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;
                comando.CommandText = @"INSERT INTO public.transportadora (transportadora_id, nome, codigo_servico, valor_base, valor_por_kg, prazo_min_dias, prazo_max_dias, ativo)
                                        VALUES (@id, @nome, @codigoServico, @valorBase, @valorPorKg, @prazoMinDias, @prazoMaxDias, @ativo);";

                transportadora.TransportadoraId = Guid.NewGuid();

                comando.Parameters.AddWithValue("id", transportadora.TransportadoraId);
                comando.Parameters.AddWithValue("nome", transportadora.Nome);
                comando.Parameters.AddWithValue("codigoServico", transportadora.CodigoServico);
                comando.Parameters.AddWithValue("valorBase", transportadora.ValorBase);
                comando.Parameters.AddWithValue("valorPorKg", transportadora.ValorPorKg);
                comando.Parameters.AddWithValue("prazoMinDias", transportadora.PrazoMinDias);
                comando.Parameters.AddWithValue("prazoMaxDias", transportadora.PrazoMaxDias);
                comando.Parameters.AddWithValue("ativo", transportadora.Ativo);

                comando.ExecuteNonQuery();
                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public Transportadora Procurar(Guid id)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT transportadora_id, nome, codigo_servico, valor_base, valor_por_kg, prazo_min_dias, prazo_max_dias, ativo
                                    FROM public.transportadora WHERE transportadora_id = @id;";
            comando.Parameters.AddWithValue("id", id);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearTransportadora(reader);
            return null;
        }

        public Transportadora ProcurarPorCodigoServico(string codigoServico)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT transportadora_id, nome, codigo_servico, valor_base, valor_por_kg, prazo_min_dias, prazo_max_dias, ativo
                                    FROM public.transportadora
                                    WHERE UPPER(codigo_servico) = UPPER(@codigo) AND ativo = true
                                    LIMIT 1;";
            comando.Parameters.AddWithValue("codigo", codigoServico);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearTransportadora(reader);
            return null;
        }

        public List<Transportadora> ProcurarTodos()
        {
            var lista = new List<Transportadora>();
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT transportadora_id, nome, codigo_servico, valor_base, valor_por_kg, prazo_min_dias, prazo_max_dias, ativo
                                    FROM public.transportadora ORDER BY nome;";

            using var reader = comando.ExecuteReader();
            while (reader.Read())
                lista.Add(MapearTransportadora(reader));
            return lista;
        }

        public void AtivarDesativar(Guid id, bool ativo)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;
                comando.CommandText = "UPDATE public.transportadora SET ativo = @ativo WHERE transportadora_id = @id;";
                comando.Parameters.AddWithValue("ativo", ativo);
                comando.Parameters.AddWithValue("id", id);
                comando.ExecuteNonQuery();
                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        private Transportadora MapearTransportadora(NpgsqlDataReader reader)
        {
            return new Transportadora
            {
                TransportadoraId = reader.GetGuid(reader.GetOrdinal("transportadora_id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                CodigoServico = reader.GetString(reader.GetOrdinal("codigo_servico")),
                ValorBase = reader.GetDecimal(reader.GetOrdinal("valor_base")),
                ValorPorKg = reader.GetDecimal(reader.GetOrdinal("valor_por_kg")),
                PrazoMinDias = reader.GetInt32(reader.GetOrdinal("prazo_min_dias")),
                PrazoMaxDias = reader.GetInt32(reader.GetOrdinal("prazo_max_dias")),
                Ativo = reader.GetBoolean(reader.GetOrdinal("ativo"))
            };
        }
    }
}
