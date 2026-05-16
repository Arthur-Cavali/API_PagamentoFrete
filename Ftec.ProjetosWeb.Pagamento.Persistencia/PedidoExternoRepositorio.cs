using Npgsql;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Persistencia
{
    public class PedidoExternoRepositorio
    {
        private string strConexaoPedidos;
        private string strConexaoProdutos;

        public PedidoExternoRepositorio(string strConexaoPedidos, string strConexaoProdutos)
        {
            this.strConexaoPedidos = strConexaoPedidos;
            this.strConexaoProdutos = strConexaoProdutos;
        }

        public decimal CalcularValorPedido(Guid pedidoId)
        {
            var itens = new List<(Guid produtoId, int quantidade)>();

            using (var conexao = new NpgsqlConnection(strConexaoPedidos))
            {
                conexao.Open();
                var cmd = new NpgsqlCommand(
                    "SELECT produtoid, quantidade FROM produto_pedido WHERE pedidoid = @pedidoId",
                    conexao);
                cmd.Parameters.AddWithValue("pedidoId", pedidoId.ToString());

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var produtoIdStr = reader.GetString(0);
                    var quantidadeStr = reader.GetString(1);
                    if (Guid.TryParse(produtoIdStr, out var prodId) && int.TryParse(quantidadeStr, out var qtd))
                        itens.Add((prodId, qtd));
                }
            }

            if (itens.Count == 0)
                throw new Exception("Nenhum produto encontrado para este pedido.");

            decimal total = 0;
            using (var conexao = new NpgsqlConnection(strConexaoProdutos))
            {
                conexao.Open();
                foreach (var (produtoId, quantidade) in itens)
                {
                    var cmd = new NpgsqlCommand(
                        "SELECT preco FROM produtos WHERE id = @id AND excluido = false",
                        conexao);
                    cmd.Parameters.AddWithValue("id", produtoId);
                    var preco = cmd.ExecuteScalar();
                    if (preco != null && preco != DBNull.Value)
                        total += Convert.ToDecimal(preco) * quantidade;
                }
            }

            return Math.Round(total, 2);
        }

        public (string cepDestino, string numero) ObterEnderecoPedido(Guid pedidoId)
        {
            using var conexao = new NpgsqlConnection(strConexaoPedidos);
            conexao.Open();
            var cmd = new NpgsqlCommand(
                "SELECT cependerecoentrega, numeroenderecoentrega FROM pedido WHERE id = @id",
                conexao);
            cmd.Parameters.AddWithValue("id", pedidoId.ToString());

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var cep = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                var numero = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                return (cep, numero);
            }

            throw new Exception("Pedido não encontrado.");
        }
    }
}
