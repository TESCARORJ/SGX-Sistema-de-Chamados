ALTER TABLE chamados
    ADD COLUMN IF NOT EXISTS responsavel_usuario_id UUID NULL;

ALTER TABLE historicos_chamado
    ADD COLUMN IF NOT EXISTS visivel_solicitante BOOLEAN NOT NULL DEFAULT TRUE;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'fk_chamados_responsavel_usuario'
    ) THEN
        ALTER TABLE chamados
            ADD CONSTRAINT fk_chamados_responsavel_usuario
            FOREIGN KEY (responsavel_usuario_id)
            REFERENCES usuarios (id);
    END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_chamados_responsavel_usuario_id
    ON chamados (responsavel_usuario_id);
