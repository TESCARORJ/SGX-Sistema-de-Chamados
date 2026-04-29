CREATE SEQUENCE IF NOT EXISTS seq_numero_chamado
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 10;

CREATE TABLE IF NOT EXISTS chamados (
    id UUID NOT NULL,
    numero VARCHAR(30) NOT NULL,
    titulo VARCHAR(150) NOT NULL,
    descricao TEXT NOT NULL,
    situacao VARCHAR(40) NOT NULL,
    prioridade VARCHAR(20) NOT NULL,
    origem VARCHAR(20) NOT NULL,
    solicitante_id UUID NOT NULL,
    departamento_id UUID NOT NULL,
    categoria_id UUID NOT NULL,
    servico_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_chamados PRIMARY KEY (id),
    CONSTRAINT fk_chamados_solicitante FOREIGN KEY (solicitante_id)
        REFERENCES usuarios (id),
    CONSTRAINT fk_chamados_departamento FOREIGN KEY (departamento_id)
        REFERENCES departamentos (id),
    CONSTRAINT fk_chamados_categoria FOREIGN KEY (categoria_id)
        REFERENCES categorias (id),
    CONSTRAINT fk_chamados_servico FOREIGN KEY (servico_id)
        REFERENCES servicos (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_chamados_numero
    ON chamados (numero);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_chamados_numero'
    ) THEN
        ALTER TABLE chamados
            ADD CONSTRAINT uq_chamados_numero
            UNIQUE USING INDEX idx_chamados_numero;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_chamados_solicitante_id
    ON chamados (solicitante_id);

CREATE INDEX IF NOT EXISTS idx_chamados_departamento_id
    ON chamados (departamento_id);

CREATE INDEX IF NOT EXISTS idx_chamados_categoria_id
    ON chamados (categoria_id);

CREATE INDEX IF NOT EXISTS idx_chamados_servico_id
    ON chamados (servico_id);

CREATE INDEX IF NOT EXISTS idx_chamados_data_criacao
    ON chamados (data_criacao DESC);

CREATE TABLE IF NOT EXISTS interacoes_chamado (
    id UUID NOT NULL,
    chamado_id UUID NOT NULL,
    tipo_interacao VARCHAR(40) NOT NULL,
    mensagem TEXT NOT NULL,
    visivel_solicitante BOOLEAN NOT NULL,
    autor_usuario_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_interacoes_chamado PRIMARY KEY (id),
    CONSTRAINT fk_interacoes_chamado_chamado FOREIGN KEY (chamado_id)
        REFERENCES chamados (id),
    CONSTRAINT fk_interacoes_chamado_autor FOREIGN KEY (autor_usuario_id)
        REFERENCES usuarios (id)
);

CREATE INDEX IF NOT EXISTS idx_interacoes_chamado_chamado_id
    ON interacoes_chamado (chamado_id);

CREATE INDEX IF NOT EXISTS idx_interacoes_chamado_data_criacao
    ON interacoes_chamado (data_criacao);

CREATE TABLE IF NOT EXISTS historicos_chamado (
    id UUID NOT NULL,
    chamado_id UUID NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    situacao_anterior VARCHAR(40) NULL,
    situacao_nova VARCHAR(40) NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_historicos_chamado PRIMARY KEY (id),
    CONSTRAINT fk_historicos_chamado_chamado FOREIGN KEY (chamado_id)
        REFERENCES chamados (id)
);

CREATE INDEX IF NOT EXISTS idx_historicos_chamado_chamado_id
    ON historicos_chamado (chamado_id);

CREATE INDEX IF NOT EXISTS idx_historicos_chamado_data_criacao
    ON historicos_chamado (data_criacao);

CREATE TABLE IF NOT EXISTS anexos_chamado (
    id UUID NOT NULL,
    chamado_id UUID NOT NULL,
    nome_arquivo VARCHAR(255) NOT NULL,
    nome_armazenado VARCHAR(255) NOT NULL,
    caminho_armazenamento VARCHAR(500) NOT NULL,
    tipo_conteudo VARCHAR(150) NOT NULL,
    tamanho_bytes BIGINT NOT NULL,
    autor_usuario_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_anexos_chamado PRIMARY KEY (id),
    CONSTRAINT fk_anexos_chamado_chamado FOREIGN KEY (chamado_id)
        REFERENCES chamados (id),
    CONSTRAINT fk_anexos_chamado_autor FOREIGN KEY (autor_usuario_id)
        REFERENCES usuarios (id)
);

CREATE INDEX IF NOT EXISTS idx_anexos_chamado_chamado_id
    ON anexos_chamado (chamado_id);

CREATE INDEX IF NOT EXISTS idx_anexos_chamado_data_criacao
    ON anexos_chamado (data_criacao);
