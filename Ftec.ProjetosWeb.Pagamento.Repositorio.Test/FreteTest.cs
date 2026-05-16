using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Persistencia;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Repositorio.Test
{
    [TestClass]
    public sealed class FreteTest
    {
        private const string StrConexao = "Server=localhost;Port=5432;Database=projetosweb;User Id=postgres;Password=280600;";

        // Guids de referência para testes
        private static readonly Guid PedidoIdTeste = new Guid("00000000-0000-0000-0000-000000000001");
        private static readonly Guid EnderecoEntregaIdTeste = new Guid("00000000-0000-0000-0000-000000000002");
        private static readonly Guid TransportadoraIdTeste = new Guid("aaaaaaaa-0000-0000-0000-000000000001");

        //[TestMethod]
        public void TestInserirFrete()
        {
            // Insere transportadora real para respeitar a FK
            var transportadoraRepositorio = new TransportadoraRepositorio(StrConexao);
            var transportadora = new Transportadora
            {
                Nome = "Correios SEDEX",
                CodigoServico = "SEDEX",
                Ativo = true
            };
            transportadoraRepositorio.Inserir(transportadora);

            var frete = new Frete
            {
                PedidoId = PedidoIdTeste,
                EnderecoEntregaId = EnderecoEntregaIdTeste,
                TransportadoraId = transportadora.TransportadoraId,
                ValorFrete = 33.25m,
                PrazoEntrega = 3,
                StatusEntrega = StatusEntrega.Pendente
            };

            var repositorio = new FreteRepositorio(StrConexao);
            try
            {
                Guid idGerado = repositorio.Inserir(frete);
                Assert.AreNotEqual(Guid.Empty, idGerado, "IdFrete deve ser preenchido após inserção.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exceção lançada durante a inserção: {ex.Message}");
            }
        }

        //[TestMethod]
        public void TestAtualizarStatus()
        {
            // Substitua pelo Guid de um frete existente no banco
            var freteId = Guid.Parse("00000000-0000-0000-0000-000000000099");
            var repositorio = new FreteRepositorio(StrConexao);
            try
            {
                repositorio.AtualizarStatus(freteId, StatusEntrega.Preparando);
                Assert.IsTrue(true, "Status atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Erro ao atualizar status: {ex.Message}");
            }
        }

        //[TestMethod]
        public void TestProcurarFrete()
        {
            // Substitua pelo Guid de um frete existente no banco
            var freteId = Guid.Parse("b3d88922-6749-4e3b-b858-1aa17d7fe038");
            var repositorio = new FreteRepositorio(StrConexao);
            var frete = repositorio.Procurar(freteId);

            Assert.IsNotNull(frete, "Frete não foi encontrado.");
            Assert.AreEqual(freteId, frete.IdFrete);
        }

        //[TestMethod]
        public void TestProcurarPorPedido()
        {
            var repositorio = new FreteRepositorio(StrConexao);
            var frete = repositorio.ProcurarPorPedido(PedidoIdTeste);

            Assert.IsNotNull(frete, "Nenhum frete encontrado para o pedido informado.");
        }

        //[TestMethod]
        public void TestProcurarTodos()
        {
            var repositorio = new FreteRepositorio(StrConexao);
            var fretes = repositorio.ProcurarTodos();

            Assert.IsNotNull(fretes);
            Assert.IsTrue(fretes.Count > 0, "Nenhum frete encontrado.");
        }
    }
}
