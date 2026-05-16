using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Persistencia
{
    public class FreteRepositorio : IFreteRepositorio
    {
        private string stringConexao;

        public FreteRepositorio(string strConexao)
        {
            stringConexao = strConexao;
        }

        public Guid Inserir(Frete frete)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;
                comando.CommandText = @"INSERT INTO public.frete
                                            (id_frete, pedido_id, endereco_entrega_id, transportadora_id, valor_frete,
                                             codigo_rastreio, criado_em, data_envio, data_entrega, prazo_entrega, status_entrega,
                                             logradouro, numero, complemento, bairro, cidade, estado, cep_destino)
                                        VALUES
                                            (@idFrete, @pedidoId, @enderecoEntregaId, @transportadoraId, @valorFrete,
                                             @codigoRastreio, @criadoEm, @dataEnvio, @dataEntrega, @prazoEntrega, @statusEntrega,
                                             @logradouro, @numero, @complemento, @bairro, @cidade, @estado, @cepDestino);";

                frete.IdFrete = Guid.NewGuid();
                frete.CriadoEm = DateTime.UtcNow;

                comando.Parameters.AddWithValue("idFrete", frete.IdFrete);
                comando.Parameters.AddWithValue("pedidoId", frete.PedidoId);
                comando.Parameters.AddWithValue("enderecoEntregaId", frete.EnderecoEntregaId);
                comando.Parameters.AddWithValue("transportadoraId", frete.TransportadoraId);
                comando.Parameters.AddWithValue("valorFrete", frete.ValorFrete);
                comando.Parameters.AddWithValue("codigoRastreio", (object?)frete.CodigoRastreio ?? DBNull.Value);
                comando.Parameters.AddWithValue("criadoEm", frete.CriadoEm);
                comando.Parameters.AddWithValue("dataEnvio", (object?)frete.DataEnvio ?? DBNull.Value);
                comando.Parameters.AddWithValue("dataEntrega", (object?)frete.DataEntrega ?? DBNull.Value);
                comando.Parameters.AddWithValue("prazoEntrega", frete.PrazoEntrega);
                comando.Parameters.AddWithValue("statusEntrega", (int)frete.StatusEntrega);
                comando.Parameters.AddWithValue("logradouro", frete.Logradouro);
                comando.Parameters.AddWithValue("numero", frete.Numero);
                comando.Parameters.AddWithValue("complemento", (object?)frete.Complemento ?? DBNull.Value);
                comando.Parameters.AddWithValue("bairro", frete.Bairro);
                comando.Parameters.AddWithValue("cidade", frete.Cidade);
                comando.Parameters.AddWithValue("estado", frete.Estado);
                comando.Parameters.AddWithValue("cepDestino", frete.CepDestino);

                comando.ExecuteNonQuery();
                transacao.Commit();
                return frete.IdFrete;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public void AtualizarStatus(Guid id, StatusEntrega novoStatus)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;

                // Preenche data_envio quando enviado, data_entrega quando entregue
                string sql = novoStatus == StatusEntrega.Enviado
                    ? "UPDATE public.frete SET status_entrega = @status, data_envio = NOW() WHERE id_frete = @id;"
                    : novoStatus == StatusEntrega.Entregue
                        ? "UPDATE public.frete SET status_entrega = @status, data_entrega = NOW() WHERE id_frete = @id;"
                        : "UPDATE public.frete SET status_entrega = @status WHERE id_frete = @id;";

                comando.CommandText = sql;
                comando.Parameters.AddWithValue("status", (int)novoStatus);
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

        public void Enviar(Guid id, string codigoRastreio)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;
                comando.CommandText = @"UPDATE public.frete
                                        SET status_entrega = @status,
                                            data_envio = NOW(),
                                            codigo_rastreio = @codigoRastreio
                                        WHERE id_frete = @id;";
                comando.Parameters.AddWithValue("status", (int)StatusEntrega.Enviado);
                comando.Parameters.AddWithValue("codigoRastreio", codigoRastreio);
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

        public Frete Procurar(Guid id)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT f.id_frete, f.pedido_id, f.endereco_entrega_id, f.transportadora_id,
                                           t.nome AS nome_transportadora,
                                           f.valor_frete, f.codigo_rastreio, f.criado_em,
                                           f.data_envio, f.data_entrega, f.prazo_entrega, f.status_entrega,
                                           f.logradouro, f.numero, f.complemento, f.bairro, f.cidade, f.estado, f.cep_destino
                                    FROM public.frete f
                                    LEFT JOIN public.transportadora t ON f.transportadora_id = t.transportadora_id
                                    WHERE f.id_frete = @id;";
            comando.Parameters.AddWithValue("id", id);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearFrete(reader);
            return null;
        }

        public Frete ProcurarPorPedido(Guid pedidoId)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT f.id_frete, f.pedido_id, f.endereco_entrega_id, f.transportadora_id,
                                           t.nome AS nome_transportadora,
                                           f.valor_frete, f.codigo_rastreio, f.criado_em,
                                           f.data_envio, f.data_entrega, f.prazo_entrega, f.status_entrega,
                                           f.logradouro, f.numero, f.complemento, f.bairro, f.cidade, f.estado, f.cep_destino
                                    FROM public.frete f
                                    LEFT JOIN public.transportadora t ON f.transportadora_id = t.transportadora_id
                                    WHERE f.pedido_id = @pedidoId
                                    ORDER BY f.criado_em DESC
                                    LIMIT 1;";
            comando.Parameters.AddWithValue("pedidoId", pedidoId);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearFrete(reader);
            return null;
        }

        public List<Frete> ProcurarTodos()
        {
            var fretes = new List<Frete>();
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT f.id_frete, f.pedido_id, f.endereco_entrega_id, f.transportadora_id,
                                           t.nome AS nome_transportadora,
                                           f.valor_frete, f.codigo_rastreio, f.criado_em,
                                           f.data_envio, f.data_entrega, f.prazo_entrega, f.status_entrega,
                                           f.logradouro, f.numero, f.complemento, f.bairro, f.cidade, f.estado, f.cep_destino
                                    FROM public.frete f
                                    LEFT JOIN public.transportadora t ON f.transportadora_id = t.transportadora_id
                                    ORDER BY f.criado_em DESC;";

            using var reader = comando.ExecuteReader();
            while (reader.Read())
                fretes.Add(MapearFrete(reader));
            return fretes;
        }

        private Frete MapearFrete(NpgsqlDataReader reader)
        {
            int ordCodigoRastreio    = reader.GetOrdinal("codigo_rastreio");
            int ordDataEnvio         = reader.GetOrdinal("data_envio");
            int ordDataEntrega       = reader.GetOrdinal("data_entrega");
            int ordNomeTransportadora = reader.GetOrdinal("nome_transportadora");
            int ordComplemento       = reader.GetOrdinal("complemento");

            return new Frete
            {
                IdFrete           = reader.GetGuid(reader.GetOrdinal("id_frete")),
                PedidoId          = reader.GetGuid(reader.GetOrdinal("pedido_id")),
                EnderecoEntregaId = reader.GetGuid(reader.GetOrdinal("endereco_entrega_id")),
                TransportadoraId  = reader.GetGuid(reader.GetOrdinal("transportadora_id")),
                NomeTransportadora = reader.IsDBNull(ordNomeTransportadora) ? null : reader.GetString(ordNomeTransportadora),
                ValorFrete        = reader.GetDecimal(reader.GetOrdinal("valor_frete")),
                CodigoRastreio    = reader.IsDBNull(ordCodigoRastreio) ? null : reader.GetString(ordCodigoRastreio),
                CriadoEm          = reader.GetDateTime(reader.GetOrdinal("criado_em")),
                DataEnvio         = reader.IsDBNull(ordDataEnvio) ? null : reader.GetDateTime(ordDataEnvio),
                DataEntrega       = reader.IsDBNull(ordDataEntrega) ? null : reader.GetDateTime(ordDataEntrega),
                PrazoEntrega      = reader.GetInt32(reader.GetOrdinal("prazo_entrega")),
                StatusEntrega     = (StatusEntrega)reader.GetInt32(reader.GetOrdinal("status_entrega")),
                Logradouro        = reader.GetString(reader.GetOrdinal("logradouro")),
                Numero            = reader.GetString(reader.GetOrdinal("numero")),
                Complemento       = reader.IsDBNull(ordComplemento) ? null : reader.GetString(ordComplemento),
                Bairro            = reader.GetString(reader.GetOrdinal("bairro")),
                Cidade            = reader.GetString(reader.GetOrdinal("cidade")),
                Estado            = reader.GetString(reader.GetOrdinal("estado")),
                CepDestino        = reader.GetString(reader.GetOrdinal("cep_destino"))
            };
        }
    }
}
