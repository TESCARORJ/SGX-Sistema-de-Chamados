CREATE TABLE IF NOT EXISTS logs_integracao_email (
    id UUID NOT NULL,
    caixa_email_id UUID NOT NULL,
    message_id VARCHAR(500) NULL,
    remetente VARCHAR(255) NOT NULL,
    destinatario VARCHAR(255) NOT NULL,
    assunto VARCHAR(500) NOT NULL,
    status_processamento VARCHAR(30) NOT NULL,
    detalhe_processamento TEXT NULL,
    chave_deduplicacao VARCHAR(600) NOT NULL,
    chamado_id UUID NULL,
    data_processamento TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_logs_integracao_email PRIMARY KEY (id),
    CONSTRAINT fk_logs_integracao_email_caixa_email FOREIGN KEY (caixa_email_id)
        REFERENCES caixas_email (id),
    CONSTRAINT fk_logs_integracao_email_chamado FOREIGN KEY (chamado_id)
        REFERENCES chamados (id)
);

CREATE INDEX IF NOT EXISTS idx_logs_integracao_email_caixa_email_id
    ON logs_integracao_email (caixa_email_id);

CREATE INDEX IF NOT EXISTS idx_logs_integracao_email_status_processamento
    ON logs_integracao_email (status_processamento);

CREATE INDEX IF NOT EXISTS idx_logs_integracao_email_data_processamento
    ON logs_integracao_email (data_processamento DESC);

CREATE UNIQUE INDEX IF NOT EXISTS idx_logs_integracao_email_deduplicacao
    ON logs_integracao_email (caixa_email_id, chave_deduplicacao);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_logs_integracao_email_deduplicacao'
    ) THEN
        ALTER TABLE logs_integracao_email
            ADD CONSTRAINT uq_logs_integracao_email_deduplicacao
            UNIQUE USING INDEX idx_logs_integracao_email_deduplicacao;
    END IF;
END $$;

