using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Persistencia
{
    using Pagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.Pagamento;
    using TransacaoPagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.TransacaoPagamento;

    public class PagamentoRepositorio : IPagamentoRepositorio
    {
        private string stringConexao;

        public PagamentoRepositorio(string strConexao)
        {
            stringConexao = strConexao;
        }

        public void Inserir(Pagamento pagamento)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;
                comando.CommandText = @"INSERT INTO public.pagamento (pagamento_id, pedido_id, cpf_cliente, valor_produtos, valor_frete, valor_total, numero_parcelas, valor_parcela, metodo_pagamento, status_pagamento, criado_em, data_pagamento)
                                      VALUES (@pagamentoId, @pedidoId, @cpfCliente, @valorProdutos, @valorFrete, @valorTotal, @numeroParcelas, @valorParcela, @metodoPagamento, @statusPagamento, @criadoEm, @dataPagamento);";

                pagamento.PagamentoId = Guid.NewGuid();
                pagamento.CriadoEm = DateTime.UtcNow;

                comando.Parameters.AddWithValue("pagamentoId", pagamento.PagamentoId);
                comando.Parameters.AddWithValue("pedidoId", pagamento.PedidoId);
                comando.Parameters.AddWithValue("cpfCliente", pagamento.CpfCliente);
                comando.Parameters.AddWithValue("valorProdutos", pagamento.ValorProdutos);
                comando.Parameters.AddWithValue("valorFrete", pagamento.ValorFrete);
                comando.Parameters.AddWithValue("valorTotal", pagamento.ValorTotal);
                comando.Parameters.AddWithValue("numeroParcelas", pagamento.NumeroParcelas);
                comando.Parameters.AddWithValue("valorParcela", pagamento.ValorParcela);
                comando.Parameters.AddWithValue("metodoPagamento", (int)pagamento.MetodoPagamento);
                comando.Parameters.AddWithValue("statusPagamento", (int)pagamento.StatusPagamento);
                comando.Parameters.AddWithValue("criadoEm", pagamento.CriadoEm);
                comando.Parameters.AddWithValue("dataPagamento", (object?)pagamento.DataPagamento ?? DBNull.Value);

                comando.ExecuteNonQuery();
                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

        public void AtualizarStatus(Guid id, StatusPagamento novoStatus)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var transacao = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = transacao;

                string sql = novoStatus == StatusPagamento.Pago
                    ? "UPDATE public.pagamento SET status_pagamento = @status, data_pagamento = NOW() WHERE pagamento_id = @id;"
                    : "UPDATE public.pagamento SET status_pagamento = @status WHERE pagamento_id = @id;";

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

        public void Cancelar(Guid id)
        {
            AtualizarStatus(id, StatusPagamento.Cancelado);
        }

        public Pagamento Procurar(Guid id)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT pagamento_id, pedido_id, cpf_cliente, valor_produtos, valor_frete, valor_total, numero_parcelas, valor_parcela, metodo_pagamento, status_pagamento, criado_em, data_pagamento
                                  FROM public.pagamento WHERE pagamento_id = @id;";
            comando.Parameters.AddWithValue("id", id);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearPagamento(reader);
            return null;
        }

        public Pagamento ProcurarPorPedido(Guid pedidoId)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT pagamento_id, pedido_id, cpf_cliente, valor_produtos, valor_frete, valor_total, numero_parcelas, valor_parcela, metodo_pagamento, status_pagamento, criado_em, data_pagamento
                                  FROM public.pagamento WHERE pedido_id = @pedidoId
                                  ORDER BY criado_em DESC LIMIT 1;";
            comando.Parameters.AddWithValue("pedidoId", pedidoId);

            using var reader = comando.ExecuteReader();
            if (reader.Read())
                return MapearPagamento(reader);
            return null;
        }

        public List<Pagamento> ProcurarTodos()
        {
            var pagamentos = new List<Pagamento>();
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            var comando = new NpgsqlCommand();
            comando.Connection = conexao;
            comando.CommandText = @"SELECT pagamento_id, pedido_id, cpf_cliente, valor_produtos, valor_frete, valor_total, numero_parcelas, valor_parcela, metodo_pagamento, status_pagamento, criado_em, data_pagamento
                                  FROM public.pagamento ORDER BY criado_em DESC;";

            using var reader = comando.ExecuteReader();
            while (reader.Read())
                pagamentos.Add(MapearPagamento(reader));
            return pagamentos;
        }

        public void InserirTransacao(TransacaoPagamento transacao)
        {
            using var conexao = new NpgsqlConnection(stringConexao);
            conexao.Open();
            using var tx = conexao.BeginTransaction();
            try
            {
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;
                comando.Transaction = tx;
                comando.CommandText = @"INSERT INTO public.transacao_pagamento (id_transacao_pagamento, pagamento_id, valor, retorno_gateway, status_transacao, data_transacao)
                                        VALUES (@id, @pagamentoId, @valor, @retornoGateway, @statusTransacao, @dataTransacao);";

                transacao.IdTransacaoPagamento = Guid.NewGuid();
                transacao.DataTransacao = DateTime.UtcNow;

                comando.Parameters.AddWithValue("id", transacao.IdTransacaoPagamento);
                comando.Parameters.AddWithValue("pagamentoId", transacao.PagamentoId);
                comando.Parameters.AddWithValue("valor", transacao.Valor);
                comando.Parameters.AddWithValue("retornoGateway", transacao.RetornoGateway);
                comando.Parameters.AddWithValue("statusTransacao", transacao.StatusTransacao);
                comando.Parameters.AddWithValue("dataTransacao", transacao.DataTransacao);

                comando.ExecuteNonQuery();
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private Pagamento MapearPagamento(NpgsqlDataReader reader)
        {
            int ordDataPagamento = reader.GetOrdinal("data_pagamento");

            return new Pagamento
            {
                PagamentoId = reader.GetGuid(reader.GetOrdinal("pagamento_id")),
                PedidoId = reader.GetGuid(reader.GetOrdinal("pedido_id")),
                CpfCliente = reader.GetString(reader.GetOrdinal("cpf_cliente")),
                ValorProdutos = reader.GetDecimal(reader.GetOrdinal("valor_produtos")),
                ValorFrete = reader.GetDecimal(reader.GetOrdinal("valor_frete")),
                ValorTotal = reader.GetDecimal(reader.GetOrdinal("valor_total")),
                NumeroParcelas = reader.GetInt32(reader.GetOrdinal("numero_parcelas")),
                ValorParcela = reader.GetDecimal(reader.GetOrdinal("valor_parcela")),
                MetodoPagamento = (MetodoPagamento)reader.GetInt32(reader.GetOrdinal("metodo_pagamento")),
                StatusPagamento = (StatusPagamento)reader.GetInt32(reader.GetOrdinal("status_pagamento")),
                CriadoEm = reader.GetDateTime(reader.GetOrdinal("criado_em")),
                DataPagamento = reader.IsDBNull(ordDataPagamento) ? null : reader.GetDateTime(ordDataPagamento)
            };
        }
    }
}
