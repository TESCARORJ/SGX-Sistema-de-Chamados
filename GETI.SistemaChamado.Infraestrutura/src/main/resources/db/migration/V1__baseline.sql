CREATE TABLE IF NOT EXISTS controle_baseline_sistema (
    id BIGSERIAL PRIMARY KEY,
    descricao VARCHAR(120) NOT NULL,
    criado_em TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

INSERT INTO controle_baseline_sistema (descricao)
VALUES ('Baseline estrutural inicial')
ON CONFLICT DO NOTHING;
