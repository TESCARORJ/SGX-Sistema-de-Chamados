ALTER TABLE usuarios
    ADD COLUMN IF NOT EXISTS login VARCHAR(120);

UPDATE usuarios
SET login = LOWER(email)
WHERE login IS NULL OR BTRIM(login) = '';

ALTER TABLE usuarios
    ALTER COLUMN login SET NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS idx_usuarios_login
    ON usuarios (login);

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_usuarios_login'
    ) THEN
        ALTER TABLE usuarios
            ADD CONSTRAINT uq_usuarios_login
            UNIQUE USING INDEX idx_usuarios_login;
    END IF;
END $$;
