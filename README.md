# API de PAGAMENTOS E FRETE


📦 Endpoints da API


Base URL: http://pedido.neurosky.com.br/api/pedido

🔍 Consultas de Pedido
- GET /api/pedido/{pedidoId}
Retorna o valor total de um pedido externo.
- Response: objeto com pedidoId e valorTotal.
Exemplo de resposta
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "valorTotal": 250.00
}



📍 Endereço de Entrega
- GET /api/pedido/{pedidoId}/endereco
Retorna o endereço de entrega de um pedido externo.
- Response: objeto com cepDestino e numero.
Exemplo de resposta
{
  "cepDestino": "95000-000",
  "numero": "100"
}







ase URL: http://pagamento.neurosky.com.br/api/frete

🔍 Consultas
- GET /api/frete
Lista todos os fretes cadastrados.
- Response: List<FreteDTO>
- GET /api/frete/{id}
Retorna os detalhes de um frete específico pelo IdFrete.
- Response: FreteDTO
- GET /api/frete/pedido/{pedidoId}
Retorna o frete vinculado a um pedido específico.
- Response: FreteDTO
Exemplo de resposta
{
  "idFrete": "222e3333-e44b-55d6-a777-888999000111",
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "enderecoEntregaId": "123e4567-e89b-12d3-a456-426614174000",
  "transportadoraId": "111e2222-e33b-44d5-a666-777888999000",
  "nomeTransportadora": "Transportadora Rápida",
  "valorFrete": 50.00,
  "codigoRastreio": null,
  "criadoEm": "2026-05-16T21:20:00",
  "dataEnvio": null,
  "dataEntrega": null,
  "prazoEntrega": 5,
  "statusEntrega": "Pendente",
  "logradouro": "Rua Exemplo",
  "numero": "100",
  "complemento": "Apto 202",
  "bairro": "Centro",
  "cidade": "Caxias do Sul",
  "estado": "RS",
  "cepDestino": "95000-000"
}



📦 Cálculo de Frete
- POST /api/frete/calcular
Calcula o frete com base nos dados fornecidos.
- Body: FreteCalculoDTO
- Response: FreteDTO
Exemplo de requisição
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "enderecoEntregaId": "123e4567-e89b-12d3-a456-426614174000",
  "transportadoraId": "111e2222-e33b-44d5-a666-777888999000",
  "cepOrigem": "90000-000",
  "cepDestino": "95000-000",
  "logradouro": "Rua Exemplo",
  "numero": "100",
  "complemento": "Apto 202",
  "bairro": "Centro",
  "cidade": "Caxias do Sul",
  "estado": "RS"
}



📌 Fluxo de Status do Frete
O frete segue o fluxo: Pendente → Preparando → Enviado → EmTransito → Entregue

- POST /api/frete/{id}/confirmar
Confirma o frete e altera status para Preparando.
- Response: mensagem de confirmação.
  
- POST /api/frete/{id}/enviar
Envia o frete e altera status para Enviado.
- Body: FreteEnvioDTO
  
Exemplo de body
{
  "codigoRastreio": "BR123456789"
}


- POST /api/frete/{id}/em-transito
Atualiza status para EmTransito.

- POST /api/frete/{id}/entregar
Atualiza status para Entregue.

❌ Cancelamento
- DELETE /api/frete/{id}
Cancela o frete.
- Response: 204 No Content

Consultas
- GET /api/pagamento
Lista todos os pagamentos cadastrados.
- Response: List<PagamentoDTO>
- GET /api/pagamento/{id}
Retorna os detalhes de um pagamento específico pelo PagamentoId.
- Response: PagamentoDTO (inclui informações de valores, parcelas, status e transações associadas)
Exemplo de resposta
{
  "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "cpfCliente": "12345678900",
  "valorProdutos": 200.00,
  "valorFrete": 50.00,
  "valorTotal": 250.00,
  "numeroParcelas": 2,
  "valorParcela": 125.00,
  "metodoPagamento": "CartaoCredito",
  "statusPagamento": "Pendente",
  "criadoEm": "2026-05-16T21:35:00",
  "dataPagamento": null,
  "transacoes": [
    {
      "idTransacaoPagamento": "555e4444-e33b-44d5-a666-777888999000",
      "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
      "valor": 250.00,
      "retornoGateway": "Aprovado",
      "statusTransacao": true,
      "dataTransacao": "2026-05-16T21:36:00"
    }
  ]
}



➕ Criação de Pagamento
- POST /api/pagamento
Registra um novo pagamento.
- Body: PagamentoDTO (sem PagamentoId, que é gerado pelo sistema)
- Response: PagamentoDTO (com PagamentoId preenchido)
Exemplo de requisição
{
  "pedidoId": "123e4567-e89b-12d3-a456-426614174000",
  "cpfCliente": "12345678900",
  "valorProdutos": 200.00,
  "valorFrete": 50.00,
  "valorTotal": 250.00,
  "numeroParcelas": 2,
  "valorParcela": 125.00,
  "metodoPagamento": "CartaoCredito",
  "statusPagamento": "Pendente"
}



🔄 Processamento de Transação
- POST /api/pagamento/transacao
Processa uma transação de pagamento via gateway.
- Body: TransacaoPagamentoDTO
- Response: TransacaoPagamentoDTO (resultado da operação)
Exemplo de requisição
{
  "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
  "valor": 250.00,
  "retornoGateway": "Aprovado",
  "statusTransacao": true
}


Exemplo de resposta
{
  "idTransacaoPagamento": "555e4444-e33b-44d5-a666-777888999000",
  "pagamentoId": "987e6543-e21b-12d3-a456-426614174999",
  "valor": 250.00,
  "retornoGateway": "Aprovado",
  "statusTransacao": true,
  "dataTransacao": "2026-05-16T21:37:00"
}



❌ Cancelamento
- DELETE /api/pagamento/{id}
Cancela um pagamento existente.
- Response: 204 No Content




- GET /api/transportadora
Lista todas as transportadoras cadastradas.
- Response: List<TransportadoraDTO>
- GET /api/transportadora/{id}
Retorna os detalhes de uma transportadora específica pelo TransportadoraId.
- Response: TransportadoraDTO
Exemplo de resposta
{
  "transportadoraId": "111e2222-e33b-44d5-a666-777888999000",
  "nome": "Transportadora Rápida",
  "codigoServico": "EXPRESSO",
  "valorBase": 30.00,
  "prazoMinDias": 2,
  "prazoMaxDias": 5,
  "ativo": true
}



➕ Criação de Transportadora
- POST /api/transportadora
Cadastra uma nova transportadora.
- Body: TransportadoraDTO (sem TransportadoraId, que é gerado pelo sistema)
- Response: TransportadoraDTO (com TransportadoraId preenchido)
Exemplo de requisição
{
  "nome": "Transportadora Econômica",
  "codigoServico": "PADRAO",
  "valorBase": 20.00,
  "prazoMinDias": 5,
  "prazoMaxDias": 10,
  "ativo": true
}


Exemplo de resposta
{
  "transportadoraId": "222e3333-e44b-55d6-a777-888999000111",
  "nome": "Transportadora Econômica",
  "codigoServico": "PADRAO",
  "valorBase": 20.00,
  "prazoMinDias": 5,
  "prazoMaxDias": 10,
  "ativo": true
}



⚙️ Ativação e Desativação
- POST /api/transportadora/{id}/ativar
Ativa uma transportadora.
- Response: mensagem de confirmação.
- POST /api/transportadora/{id}/desativar
Desativa uma transportadora.
- Response: mensagem de confirmação.
Exemplo de resposta (ativar)
{
  "mensagem": "Transportadora ativada com sucesso."
}


Exemplo de resposta (desativar)
{
  "mensagem": "Transportadora desativada com sucesso."
}








