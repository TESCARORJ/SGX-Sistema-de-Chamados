ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS tipo_autenticacao VARCHAR(30);

UPDATE usuarios
SET tipo_autenticacao = 'CORPORATIVA'
WHERE tipo_autenticacao IS NULL OR BTRIM(tipo_autenticacao) = '';

ALTER TABLE usuarios
    ALTER COLUMN tipo_autenticacao SET NOT NULL;

ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS senha_hash VARCHAR(255);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'ck_usuarios_tipo_autenticacao'
    ) THEN
        ALTER TABLE usuarios
            ADD CONSTRAINT ck_usuarios_tipo_autenticacao
            CHECK (tipo_autenticacao IN ('LOCAL', 'CORPORATIVA'));
    END IF;
END $$;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'ck_usuarios_senha_hash_local'
    ) THEN
        ALTER TABLE usuarios
            ADD CONSTRAINT ck_usuarios_senha_hash_local
            CHECK (
                tipo_autenticacao <> 'LOCAL'
                OR (senha_hash IS NOT NULL AND BTRIM(senha_hash) <> '')
            );
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_usuarios_tipo_autenticacao
    ON usuarios (tipo_autenticacao);
