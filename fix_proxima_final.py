import re

# 1. Fix SeedData.cs
SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'

with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

def update_item18(text):
    parts = text.split('RoadmapItsmItem18Id,')
    if len(parts) > 1:
        sub_part = parts[1]
        sub_part = re.sub(
            r'ProximaAcao = "Registrar passagem de conhecimento da governanca"',
            r'ProximaAcao = "Registrar homologacao visual responsiva"',
            sub_part,
            count=1
        )
        parts[1] = sub_part
    return 'RoadmapItsmItem18Id,'.join(parts)

c = update_item18(c)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)


# 2. Fix Test File
TEST_FILE = 'tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs'
with open(TEST_FILE, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

tc = re.sub(
    r'Assert\.Equal\(\n            "Registrar passagem de conhecimento da governanca",\n            RemoverAcentos\(item\.ProximaAcao\)\);',
    r'Assert.Equal(\n            "Registrar homologacao visual responsiva",\n            RemoverAcentos(item.ProximaAcao));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(\n            "Registrar passagem de conhecimento da governanca",\n            RemoverAcentos\(detalhe\.ProximaAcao\)\);',
    r'Assert.Equal(\n            "Registrar homologacao visual responsiva",\n            RemoverAcentos(detalhe.ProximaAcao));',
    tc
)

# Remove the extra line from the array
tc = tc.replace(
    '                "Registrar homologacao funcional",\n                "Registrar passagem de conhecimento da governanca",\n                "Registrar homologacao visual responsiva",',
    '                "Registrar homologacao funcional",\n                "Registrar homologacao visual responsiva",'
)

with open(TEST_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(tc)

