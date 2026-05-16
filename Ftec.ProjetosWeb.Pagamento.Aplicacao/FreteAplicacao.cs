using Ftec.ProjetosWeb.Pagamento.Aplicacao.Adapter;
using Ftec.ProjetosWeb.Pagamento.Aplicacao.DTO;
using Ftec.ProjetosWeb.Pagamento.Dominio.Entidades;
using Ftec.ProjetosWeb.Pagamento.Dominio.Enums;
using Ftec.ProjetosWeb.Pagamento.Dominio.Interfaces;
using Ftec.ProjetosWeb.Pagamento.Persistencia;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Pagamento.Aplicacao
{
    public class FreteAplicacao
    {
        IFreteRepositorio freteRepositorio;
        ITransportadoraRepositorio transportadoraRepositorio;
        IPagamentoRepositorio pagamentoRepositorio;

        public FreteAplicacao(string strConexao)
        {
            freteRepositorio = new FreteRepositorio(strConexao);
            transportadoraRepositorio = new TransportadoraRepositorio(strConexao);
            pagamentoRepositorio = new PagamentoRepositorio(strConexao);
        }

        public FreteDTO CalcularFrete(FreteCalculoDTO calculo)
        {
            if (calculo.PedidoId == Guid.Empty)
                throw new Exception("O Id do pedido é obrigatório.");

            if (calculo.EnderecoEntregaId == Guid.Empty)
                throw new Exception("O endereço de entrega é obrigatório.");

            if (string.IsNullOrWhiteSpace(calculo.CepOrigem) || string.IsNullOrWhiteSpace(calculo.CepDestino))
                throw new Exception("CEP de origem e destino são obrigatórios.");

            if (calculo.TransportadoraId == Guid.Empty)
                throw new Exception("Selecione uma transportadora.");

            var transportadora = transportadoraRepositorio.Procurar(calculo.TransportadoraId);
            if (transportadora == null || !transportadora.Ativo)
                throw new Exception("Transportadora não encontrada ou inativa.");

            decimal adicionalRegiao = CalcularAdicionalPorRegiao(calculo.CepOrigem, calculo.CepDestino);
            decimal valor = Math.Round(transportadora.ValorBase + adicionalRegiao, 2);

            int prazo = CalcularPrazo(calculo.CepOrigem, calculo.CepDestino);
            prazo = Math.Clamp(prazo, transportadora.PrazoMinDias, transportadora.PrazoMaxDias);

            var frete = new Frete
            {
                PedidoId = calculo.PedidoId,
                EnderecoEntregaId = calculo.EnderecoEntregaId,
                TransportadoraId = transportadora.TransportadoraId,
                ValorFrete = valor,
                PrazoEntrega = prazo,
                StatusEntrega = StatusEntrega.Pendente,
                Logradouro = calculo.Logradouro ?? string.Empty,
                Numero = calculo.Numero ?? string.Empty,
                Complemento = calculo.Complemento,
                Bairro = calculo.Bairro ?? string.Empty,
                Cidade = calculo.Cidade ?? string.Empty,
                Estado = calculo.Estado ?? string.Empty,
                CepDestino = calculo.CepDestino ?? string.Empty
            };

            freteRepositorio.Inserir(frete);

            // Busca novamente para trazer o NomeTransportadora via JOIN
            return FreteAdapter.ParaDTO(freteRepositorio.Procurar(frete.IdFrete));
        }

        public void ConfirmarFrete(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do frete é obrigatório.");

            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            if (frete.StatusEntrega == StatusEntrega.Cancelado)
                throw new Exception("Não é possível confirmar um frete cancelado.");

            var pagamento = pagamentoRepositorio.ProcurarPorPedido(frete.PedidoId);
            if (pagamento == null)
                throw new Exception("Nenhum pagamento encontrado para este pedido. Registre o pagamento antes de confirmar o frete.");

            if (pagamento.StatusPagamento != StatusPagamento.Pago)
                throw new Exception("O pagamento do pedido ainda não foi aprovado. Confirme o frete somente após a aprovação do pagamento.");

            freteRepositorio.AtualizarStatus(id, StatusEntrega.Preparando);
        }

        public void EnviarFrete(Guid id, FreteEnvioDTO envio)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do frete é obrigatório.");

            if (string.IsNullOrWhiteSpace(envio?.CodigoRastreio))
                throw new Exception("O código de rastreio é obrigatório para envio.");

            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            if (frete.StatusEntrega != StatusEntrega.Preparando)
                throw new Exception("O frete precisa estar no status 'Preparando' para ser enviado.");

            freteRepositorio.Enviar(id, envio.CodigoRastreio);
        }

        public void MarcarEmTransito(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do frete é obrigatório.");

            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            if (frete.StatusEntrega != StatusEntrega.Enviado)
                throw new Exception("O frete precisa estar no status 'Enviado' para ser marcado em trânsito.");

            freteRepositorio.AtualizarStatus(id, StatusEntrega.EmTransito);
        }

        public void MarcarEntregue(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do frete é obrigatório.");

            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            if (frete.StatusEntrega != StatusEntrega.EmTransito)
                throw new Exception("O frete precisa estar no status 'Em Trânsito' para ser marcado como entregue.");

            freteRepositorio.AtualizarStatus(id, StatusEntrega.Entregue);
        }

        public void CancelarFrete(Guid id)
        {
            if (id == Guid.Empty)
                throw new Exception("O Id do frete é obrigatório.");

            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            if (frete.StatusEntrega == StatusEntrega.Entregue)
                throw new Exception("Não é possível cancelar um frete já entregue.");

            freteRepositorio.AtualizarStatus(id, StatusEntrega.Cancelado);
        }

        public FreteDTO ObterFrete(Guid id)
        {
            var frete = freteRepositorio.Procurar(id);
            if (frete == null)
                throw new Exception("Frete não encontrado.");

            return FreteAdapter.ParaDTO(frete);
        }

        public FreteDTO ObterFretePorPedido(Guid pedidoId)
        {
            var frete = freteRepositorio.ProcurarPorPedido(pedidoId);
            if (frete == null)
                throw new Exception("Frete não encontrado para o pedido informado.");

            return FreteAdapter.ParaDTO(frete);
        }

        public List<FreteDTO> ListarFretes()
        {
            var fretes = freteRepositorio.ProcurarTodos();
            var dtos = new List<FreteDTO>();
            foreach (var f in fretes)
                dtos.Add(FreteAdapter.ParaDTO(f));
            return dtos;
        }

        // ── Regras de cálculo (mock) ─────────────────────────────────────

        private decimal CalcularAdicionalPorRegiao(string cepOrigem, string cepDestino)
        {
            // O primeiro dígito do CEP representa a região do Brasil:
            // 0-1 = SP, 2 = RJ/ES, 3 = MG, 4 = BA/SE, 5 = PE/AL/PB/RN
            // 6 = CE/PI/MA, 7 = DF/GO/TO/MT/MS/RO/AC, 8 = PR/SC, 9 = RS/RS
            if (cepOrigem.Length < 1 || cepDestino.Length < 1)
                return 0m;

            char regiaoOrigem  = cepOrigem[0];
            char regiaoDestino = cepDestino[0];

            if (regiaoOrigem == regiaoDestino) return 0m;          // mesma região

            int distancia = Math.Abs(regiaoOrigem - regiaoDestino);
            if (distancia <= 1) return 8.00m;                       // regiões vizinhas
            if (distancia <= 3) return 15.00m;                      // regiões intermediárias
            return 25.00m;                                           // regiões distantes
        }

        private int CalcularPrazo(string cepOrigem, string cepDestino)
        {
            if (cepOrigem.Length < 1 || cepDestino.Length < 1)
                return 10;

            int distancia = Math.Abs(cepOrigem[0] - cepDestino[0]);
            return distancia == 0 ? 1
                 : distancia <= 1 ? 3
                 : distancia <= 3 ? 5
                 : 8;
        }
    }
}
