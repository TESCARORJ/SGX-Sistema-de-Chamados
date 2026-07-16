import re

SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'

with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

def replace_proxima_acao_for_item18(text):
    parts = text.split('RoadmapItsmItem18Id,')
    if len(parts) > 1:
        sub_part = parts[1]
        sub_part = re.sub(
            r'ProximaAcao = "[^"]*"',
            r'ProximaAcao = "Registrar homologacao funcional"',
            sub_part,
            count=1
        )
        parts[1] = sub_part
    return 'RoadmapItsmItem18Id,'.join(parts)

c = replace_proxima_acao_for_item18(c)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)
