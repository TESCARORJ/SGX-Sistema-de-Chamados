CREATE TABLE IF NOT EXISTS departamentos (
    id UUID NOT NULL,
    nome VARCHAR(150) NOT NULL,
    ativo BOOLEAN NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_departamentos PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS perfis_acesso (
    id UUID NOT NULL,
    nome VARCHAR(80) NOT NULL,
    descricao VARCHAR(255) NULL,
    ativo BOOLEAN NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_perfis_acesso PRIMARY KEY (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_perfis_acesso_nome
    ON perfis_acesso (nome);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_perfis_acesso_nome'
    ) THEN
        ALTER TABLE perfis_acesso
            ADD CONSTRAINT uq_perfis_acesso_nome
            UNIQUE USING INDEX idx_perfis_acesso_nome;
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS caixas_email (
    id UUID NOT NULL,
    endereco_email VARCHAR(255) NOT NULL,
    nome_exibicao VARCHAR(150) NOT NULL,
    ativa BOOLEAN NOT NULL,
    departamento_id UUID NOT NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_caixas_email PRIMARY KEY (id),
    CONSTRAINT fk_caixas_email_departamento FOREIGN KEY (departamento_id)
        REFERENCES departamentos (id)
);

CREATE INDEX IF NOT EXISTS idx_caixas_email_departamento_id
    ON caixas_email (departamento_id);

CREATE TABLE IF NOT EXISTS usuarios (
    id UUID NOT NULL,
    nome VARCHAR(150) NOT NULL,
    email VARCHAR(255) NOT NULL,
    ativo BOOLEAN NOT NULL,
    perfil_acesso_id UUID NOT NULL,
    departamento_id UUID NULL,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao TIMESTAMP NULL,
    CONSTRAINT pk_usuarios PRIMARY KEY (id),
    CONSTRAINT fk_usuarios_perfil_acesso FOREIGN KEY (perfil_acesso_id)
        REFERENCES perfis_acesso (id),
    CONSTRAINT fk_usuarios_departamento FOREIGN KEY (departamento_id)
        REFERENCES departamentos (id)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_email
    ON usuarios (email);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_usuarios_email'
    ) THEN
        ALTER TABLE usuarios
            ADD CONSTRAINT uq_usuarios_email
            UNIQUE USING INDEX idx_usuarios_email;
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_usuarios_perfil_acesso_id
    ON usuarios (perfil_acesso_id);

CREATE INDEX IF NOT EXISTS idx_usuarios_departamento_id
    ON usuarios (departamento_id);

INSERT INTO perfis_acesso (id, nome, descricao, ativo)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Solicitante', 'Usuario que abre e acompanha chamados no portal do solicitante.', true),
    ('22222222-2222-2222-2222-222222222222', 'Atendente', 'Usuario responsavel pelo atendimento e tratamento inicial dos chamados.', true),
    ('33333333-3333-3333-3333-333333333333', 'Supervisor', 'Usuario responsavel por acompanhar filas, equipes e indicadores operacionais.', true),
    ('44444444-4444-4444-4444-444444444444', 'Administrador', 'Usuario responsavel pela configuracao e administracao geral do sistema.', true)
ON CONFLICT ON CONSTRAINT uq_perfis_acesso_nome DO UPDATE
SET
    descricao = EXCLUDED.descricao,
    ativo = EXCLUDED.ativo,
    data_atualizacao = CURRENT_TIMESTAMP;