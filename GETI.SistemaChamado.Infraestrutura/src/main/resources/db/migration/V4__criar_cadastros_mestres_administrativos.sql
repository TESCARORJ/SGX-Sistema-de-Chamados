CREATE TABLE IF NOT EXISTS categorias (
    id UUID NOT NULL,
    nome VARCHAR(120) NOT NULL,
    descricao VARCHAR(255) NULL,
    ativo BOOLEAN NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_categorias PRIMARY KEY (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_categorias_nome
    ON categorias (nome);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_categorias_nome'
    ) THEN
        ALTER TABLE categorias
            ADD CONSTRAINT uq_categorias_nome
            UNIQUE USING INDEX idx_categorias_nome;
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS grupos_atendimento (
    id UUID NOT NULL,
    nome VARCHAR(120) NOT NULL,
    descricao VARCHAR(255) NULL,
    ativo BOOLEAN NOT NULL,
    departamento_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_grupos_atendimento PRIMARY KEY (id),
    CONSTRAINT fk_grupos_atendimento_departamento FOREIGN KEY (departamento_id)
        REFERENCES departamentos (id)
);

CREATE INDEX IF NOT EXISTS idx_grupos_atendimento_departamento_id
    ON grupos_atendimento (departamento_id);

CREATE UNIQUE INDEX IF NOT EXISTS idx_grupos_atendimento_nome_departamento
    ON grupos_atendimento (nome, departamento_id);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_grupos_atendimento_nome_departamento'
    ) THEN
        ALTER TABLE grupos_atendimento
            ADD CONSTRAINT uq_grupos_atendimento_nome_departamento
            UNIQUE USING INDEX idx_grupos_atendimento_nome_departamento;
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS servicos (
    id UUID NOT NULL,
    nome VARCHAR(150) NOT NULL,
    descricao VARCHAR(255) NULL,
    ativo BOOLEAN NOT NULL,
    categoria_id UUID NOT NULL,
    departamento_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_servicos PRIMARY KEY (id),
    CONSTRAINT fk_servicos_categoria FOREIGN KEY (categoria_id)
        REFERENCES categorias (id),
    CONSTRAINT fk_servicos_departamento FOREIGN KEY (departamento_id)
        REFERENCES departamentos (id)
);

CREATE INDEX IF NOT EXISTS idx_servicos_categoria_id
    ON servicos (categoria_id);

CREATE INDEX IF NOT EXISTS idx_servicos_departamento_id
    ON servicos (departamento_id);

CREATE UNIQUE INDEX IF NOT EXISTS idx_servicos_nome_departamento
    ON servicos (nome, departamento_id);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_servicos_nome_departamento'
    ) THEN
        ALTER TABLE servicos
            ADD CONSTRAINT uq_servicos_nome_departamento
            UNIQUE USING INDEX idx_servicos_nome_departamento;
    END IF;
END $$;
