import re
import os

SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'
TEST_FILE = 'tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs'

# --- 1. Fix SeedData.cs ---
with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

# Make sure 1..36 are true, and 37..39 are false for RoadmapItsmItem18Id
for i in range(1, 40):
    val = 'true' if i <= 36 else 'false'
    # Use a positive lookahead to ensure we are matching within RoadmapItsmItem18Id
    # This is slightly tricky with regex. Instead of regex, let's just do a string replacement safely.
    # We will find the exact lines from our python script logic.
    pass

lines = c.split('\n')
for j in range(len(lines)):
    if 'RoadmapItemId = RoadmapItsmItem18Id' in lines[j] and 'Ordem =' in lines[j]:
        m = re.search(r'Ordem = (\d+)', lines[j])
        if m:
            ordem = int(m.group(1))
            if ordem <= 36:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = true', lines[j])
            else:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = false', lines[j])

c = '\n'.join(lines)

# And percentual to 92%, and ProximaAcao
c = re.sub(
    r'PercentualImplementacao = \d+,\s*PendenciasTecnicas =',
    r'PercentualImplementacao = 92,\n            PendenciasTecnicas =',
    c
)

c = re.sub(
    r'ProximaAcao = "[^"]*",\s*Observacao =',
    r'ProximaAcao = "Registrar homologacao funcional",\n            Observacao =',
    c
)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)


# --- 2. Fix Test Array ---
with open(TEST_FILE, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

tc = re.sub(
    r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => x\.Concluido\)\);',
    r'Assert.Equal(36, checklistAtivo.Count(x => x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => !x\.Concluido\)\);',
    r'Assert.Equal(3, checklistAtivo.Count(x => !x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(\d+, detalhe\.QuantidadeChecklistConcluido\);',
    r'Assert.Equal(36, detalhe.QuantidadeChecklistConcluido);',
    tc
)

# We must replace the hardcoded array to be 1..36
arr_str = 'new[] { ' + ', '.join(str(x) for x in range(1, 37)) + ' }'
tc = re.sub(
    r'new\[\] \{ [0-9, ]+ \}',
    arr_str,
    tc
)

with open(TEST_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(tc)

