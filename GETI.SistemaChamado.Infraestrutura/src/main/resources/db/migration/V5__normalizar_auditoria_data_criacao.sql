UPDATE departamentos
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE perfis_acesso
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE caixas_email
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE usuarios
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE categorias
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE grupos_atendimento
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

UPDATE servicos
SET data_criacao = CURRENT_TIMESTAMP
WHERE data_criacao IS NULL;

ALTER TABLE departamentos
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE perfis_acesso
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE caixas_email
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE usuarios
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE categorias
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE grupos_atendimento
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;

ALTER TABLE servicos
    ALTER COLUMN data_criacao SET DEFAULT CURRENT_TIMESTAMP,
    ALTER COLUMN data_criacao SET NOT NULL;
