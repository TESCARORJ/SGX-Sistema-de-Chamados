ALTER TABLE chamados
    ADD COLUMN IF NOT EXISTS prazo_sla_minutos INTEGER NULL;

ALTER TABLE chamados
    ADD COLUMN IF NOT EXISTS data_limite_sla TIMESTAMP NULL;

UPDATE chamados
SET prazo_sla_minutos = CASE prioridade
    WHEN 'CRITICA' THEN 240
    WHEN 'ALTA' THEN 480
    WHEN 'MEDIA' THEN 1440
    WHEN 'BAIXA' THEN 4320
    ELSE 1440
END
WHERE prazo_sla_minutos IS NULL;

UPDATE chamados
SET data_limite_sla = data_criacao + (prazo_sla_minutos || ' minutes')::interval
WHERE data_limite_sla IS NULL;

ALTER TABLE chamados
    ALTER COLUMN prazo_sla_minutos SET NOT NULL;

ALTER TABLE chamados
    ALTER COLUMN data_limite_sla SET NOT NULL;

CREATE INDEX IF NOT EXISTS idx_chamados_data_limite_sla
    ON chamados (data_limite_sla);

CREATE INDEX IF NOT EXISTS idx_chamados_situacao_data_limite_sla
    ON chamados (situacao, data_limite_sla);
