using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Persistencia;
using System;

namespace Ftec.ProjetosWeb.Pagamento.Repositorio.Test
{
    using Pagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.Pagamento;
    using TransacaoPagamento = global::Ftec.ProjetosWeb.Pagamento.Dominio.Entidades.TransacaoPagamento;

    [TestClass]
    public sealed class PagamentoTest
    {
        private const string StrConexao = "Server=localhost;Port=5432;Database=projetosweb;User Id=postgres;Password=280600;";

        private static readonly Guid PedidoIdTeste = new Guid("00000000-0000-0000-0000-000000000001");

        //[TestMethod]
        public void TestInserirPagamento()
        {
            var pagamento = new Pagamento
            {
                PedidoId = PedidoIdTeste,
                CpfCliente = "123.456.789-99",
                ValorProdutos = 150.00m,
                ValorFrete = 25.50m,
                ValorTotal = 175.50m,
                MetodoPagamento = MetodoPagamento.Pix,
                StatusPagamento = StatusPagamento.Pendente
            };

            var repositorio = new PagamentoRepositorio(StrConexao);
            try
            {
                repositorio.Inserir(pagamento);
                Assert.AreNotEqual(Guid.Empty, pagamento.PagamentoId, "PagamentoId deve ser preenchido após inserção.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exceção lançada durante a inserção: {ex.Message}");
            }
        }

        //[TestMethod]
        public void TestAtualizarStatus()
        {
            var pagamentoId = Guid.Parse("003a7504-bc30-4ec4-bcc1-03adae40a4c8");
            var repositorio = new PagamentoRepositorio(StrConexao);
            try
            {
                repositorio.AtualizarStatus(pagamentoId, StatusPagamento.Pago);
                Assert.IsTrue(true, "Status atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Erro ao atualizar status: {ex.Message}");
            }
        }

        //[TestMethod]
        public void TestCancelarPagamento()
        {
            var pagamentoId = Guid.Parse("00000000-0000-0000-0000-000000000099");
            var repositorio = new PagamentoRepositorio(StrConexao);
            try
            {
                repositorio.Cancelar(pagamentoId);
                Assert.IsTrue(true, "Pagamento cancelado com sucesso.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Erro ao cancelar pagamento: {ex.Message}");
            }
        }

        //[TestMethod]
        public void TestProcurarPagamento()
        {
            var pagamentoId = Guid.Parse("003a7504-bc30-4ec4-bcc1-03adae40a4c8");
            var repositorio = new PagamentoRepositorio(StrConexao);
            var pagamentoEncontrado = repositorio.Procurar(pagamentoId);

            Assert.IsNotNull(pagamentoEncontrado, "Pagamento não foi encontrado.");
            Assert.AreEqual(pagamentoId, pagamentoEncontrado.PagamentoId);
        }

        //[TestMethod]
        public void TestProcurarTodos()
        {
            var repositorio = new PagamentoRepositorio(StrConexao);
            var pagamentos = repositorio.ProcurarTodos();

            Assert.IsNotNull(pagamentos);
            Assert.IsTrue(pagamentos.Count > 0, "Nenhum pagamento encontrado.");
        }

        //[TestMethod]
        public void TestInserirTransacao()
        {
            var repositorio = new PagamentoRepositorio(StrConexao);

            var pagamento = new Pagamento
            {
                PedidoId = PedidoIdTeste,
                CpfCliente = "123.456.789-99",
                ValorProdutos = 150.00m,
                ValorFrete = 25.50m,
                ValorTotal = 175.50m,
                MetodoPagamento = MetodoPagamento.CartaoCredito,
                StatusPagamento = StatusPagamento.Processando
            };
            repositorio.Inserir(pagamento);

            var transacao = new TransacaoPagamento
            {
                PagamentoId = pagamento.PagamentoId,
                Valor = pagamento.ValorTotal,
                RetornoGateway = "{\"status\":\"approved\",\"code\":\"TXN123\"}",
                StatusTransacao = true
            };

            try
            {
                repositorio.InserirTransacao(transacao);
                Assert.AreNotEqual(Guid.Empty, transacao.IdTransacaoPagamento, "IdTransacaoPagamento deve ser preenchido após inserção.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exceção lançada durante a inserção da transação: {ex.Message}");
            }
        }
    }
}
