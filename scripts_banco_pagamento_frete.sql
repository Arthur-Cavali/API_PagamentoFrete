-- =====================================================================
-- Script de criacao das tabelas de Pagamento e Frete
-- Banco: Pagamento (servidor 191.242.230.255)
-- Ordem obrigatoria: transportadora → pagamento → transacao_pagamento → frete
-- =====================================================================

-- ---------------------------------------------------------------------
-- PASSO 1: Remover tabelas existentes (ordem inversa das FKs)
-- ---------------------------------------------------------------------
DROP TABLE IF EXISTS public.frete               CASCADE;
DROP TABLE IF EXISTS public.transacao_pagamento CASCADE;
DROP TABLE IF EXISTS public.pagamento           CASCADE;
DROP TABLE IF EXISTS public.transportadora      CASCADE;

-- ---------------------------------------------------------------------
-- PASSO 2: Tabela transportadora
-- ---------------------------------------------------------------------
CREATE TABLE public.transportadora (
    transportadora_id  UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    nome               VARCHAR(100)  NOT NULL,
    codigo_servico     VARCHAR(20)   NOT NULL,
    valor_base         NUMERIC(10,2) NOT NULL DEFAULT 0,
    valor_por_kg       NUMERIC(10,2) NOT NULL DEFAULT 0,
    prazo_min_dias     INTEGER       NOT NULL DEFAULT 1,
    prazo_max_dias     INTEGER       NOT NULL DEFAULT 30,
    ativo              BOOLEAN       NOT NULL DEFAULT TRUE
);

INSERT INTO public.transportadora (nome, codigo_servico, valor_base, valor_por_kg, prazo_min_dias, prazo_max_dias, ativo) VALUES
    ('Correios PAC',   'PAC',   15.00, 2.00, 5,  10, TRUE),
    ('Correios SEDEX', 'SEDEX', 30.00, 4.00, 1,  3,  TRUE);

-- ---------------------------------------------------------------------
-- PASSO 3: Tabela pagamento
-- ---------------------------------------------------------------------
CREATE TABLE public.pagamento (
    pagamento_id      UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id         UUID          NOT NULL,
    cpf_cliente       VARCHAR(20)   NOT NULL,
    valor_produtos    NUMERIC(12,2) NOT NULL,
    valor_frete       NUMERIC(12,2) NOT NULL,
    valor_total       NUMERIC(12,2) NOT NULL,
    numero_parcelas   INTEGER       NOT NULL DEFAULT 1,
    valor_parcela     NUMERIC(12,2) NOT NULL DEFAULT 0,
    metodo_pagamento  INTEGER       NOT NULL,
    status_pagamento  INTEGER       NOT NULL DEFAULT 0,
    criado_em         TIMESTAMP     NOT NULL DEFAULT NOW(),
    data_pagamento    TIMESTAMP     NULL
);

-- ---------------------------------------------------------------------
-- PASSO 4: Tabela transacao_pagamento
-- ---------------------------------------------------------------------
CREATE TABLE public.transacao_pagamento (
    id_transacao_pagamento UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    pagamento_id           UUID          NOT NULL,
    valor                  NUMERIC(12,2) NOT NULL,
    retorno_gateway        VARCHAR(200)  NOT NULL,
    status_transacao       BOOLEAN       NOT NULL,
    data_transacao         TIMESTAMP     NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_transacao_pagamento FOREIGN KEY (pagamento_id)
        REFERENCES public.pagamento (pagamento_id)
);

-- ---------------------------------------------------------------------
-- PASSO 5: Tabela frete
-- ---------------------------------------------------------------------
CREATE TABLE public.frete (
    id_frete            UUID          PRIMARY KEY DEFAULT gen_random_uuid(),
    pedido_id           UUID          NOT NULL,
    endereco_entrega_id UUID          NOT NULL,
    transportadora_id   UUID          NOT NULL,
    valor_frete         NUMERIC(12,2) NOT NULL,
    codigo_rastreio     VARCHAR(100)  NULL,
    prazo_entrega       INTEGER       NOT NULL,
    status_entrega      INTEGER       NOT NULL DEFAULT 0,
    criado_em           TIMESTAMP     NOT NULL DEFAULT NOW(),
    data_envio          TIMESTAMP     NULL,
    data_entrega        TIMESTAMP     NULL,
    logradouro          VARCHAR(200)  NOT NULL DEFAULT '',
    numero              VARCHAR(20)   NOT NULL DEFAULT '',
    complemento         VARCHAR(100)  NULL,
    bairro              VARCHAR(100)  NOT NULL DEFAULT '',
    cidade              VARCHAR(100)  NOT NULL DEFAULT '',
    estado              CHAR(2)       NOT NULL DEFAULT '',
    cep_destino         VARCHAR(10)   NOT NULL DEFAULT '',
    CONSTRAINT fk_frete_transportadora FOREIGN KEY (transportadora_id)
        REFERENCES public.transportadora (transportadora_id)
);

-- ---------------------------------------------------------------------
-- PASSO 6: Conferir tabelas criadas
-- ---------------------------------------------------------------------
SELECT table_name
  FROM information_schema.tables
 WHERE table_schema = 'public'
   AND table_name IN ('transportadora', 'pagamento', 'transacao_pagamento', 'frete')
 ORDER BY table_name;
-- Esperado: 4 linhas
