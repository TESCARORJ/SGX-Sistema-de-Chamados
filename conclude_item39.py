import re
import os

SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'
TEST_FILE = 'tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs'
DOC_FILE = 'docs/roadmap/sprint-7-gerenciamento-requisicoes.md'
ROADMAP_MD = 'docs/ROADMAP.md'
ROADMAP_ITSM = 'docs/ROADMAP-ITSM.md'

# --- 1. Fix SeedData.cs ---
with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

lines = c.split('\n')
for j in range(len(lines)):
    if 'RoadmapItemId = RoadmapItsmItem18Id' in lines[j] and 'Ordem =' in lines[j]:
        m = re.search(r'Ordem = (\d+)', lines[j])
        if m:
            ordem = int(m.group(1))
            if ordem == 39:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = true', lines[j])

c = '\n'.join(lines)

def update_item18(text):
    parts = text.split('RoadmapItsmItem18Id,')
    if len(parts) > 1:
        sub_part = parts[1]
        sub_part = re.sub(
            r'PercentualImplementacao = 90',
            r'PercentualImplementacao = 92',
            sub_part,
            count=1
        )
        sub_part = re.sub(
            r'ProximaAcao = "Registrar aceite formal somente com evidencia"',
            r'ProximaAcao = "Planejar modelagem estrutural futura para grupo responsavel por catalogo, formulario por servico e persistencia das respostas."',
            sub_part,
            count=1
        )
        parts[1] = sub_part
    return 'RoadmapItsmItem18Id,'.join(parts)

c = update_item18(c)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)


# --- 2. Fix Test Array ---
with open(TEST_FILE, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

tc = re.sub(
    r'Assert\.Equal\(35, checklistAtivo\.Count\(x => x\.Concluido\)\);',
    r'Assert.Equal(36, checklistAtivo.Count(x => x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(4, checklistAtivo\.Count\(x => !x\.Concluido\)\);',
    r'Assert.Equal(3, checklistAtivo.Count(x => !x.Concluido));',
    tc
)

tc = re.sub(
    r'Assert\.Equal\(35, detalhe\.QuantidadeChecklistConcluido\);',
    r'Assert.Equal(36, detalhe.QuantidadeChecklistConcluido);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(90, detalhe\.PercentualImplementacao\);',
    r'Assert.Equal(92, detalhe.PercentualImplementacao);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(90, item\.PercentualImplementacao\);',
    r'Assert.Equal(92, item.PercentualImplementacao);',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(\n            "Registrar aceite formal somente com evidencia",\n            RemoverAcentos\(item\.ProximaAcao\)\);',
    r'Assert.Equal(\n            "Planejar modelagem estrutural futura para grupo responsavel por catalogo, formulario por servico e persistencia das respostas.",\n            RemoverAcentos(item.ProximaAcao));',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(\n            "Registrar aceite formal somente com evidencia",\n            RemoverAcentos\(detalhe\.ProximaAcao\)\);',
    r'Assert.Equal(\n            "Planejar modelagem estrutural futura para grupo responsavel por catalogo, formulario por servico e persistencia das respostas.",\n            RemoverAcentos(detalhe.ProximaAcao));',
    tc
)
tc = re.sub(
    r'Assert\.Equal\(90, percentualEsperado\);',
    r'Assert.Equal(92, percentualEsperado);',
    tc
)
tc = re.sub(
    r'Math\.Round\(\(35 \* 100\.0\) \/ 39',
    r'Math.Round((36 * 100.0) / 39',
    tc
)

arr = [x for x in range(1, 40) if x not in [10, 13, 14]]
arr_str = 'new[] { ' + ', '.join(str(x) for x in arr) + ' }'
tc = re.sub(
    r'new\[\] \{ [0-9, ]+ \}',
    arr_str,
    tc,
    count=1
)

with open(TEST_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(tc)


# --- 3. Fix Markdown Docs ---
with open(DOC_FILE, 'r', encoding='utf-8') as f:
    doc = f.read()

doc = re.sub(r'- \[ \] 39\.', r'- [x] 39.', doc)

doc = re.sub(r'90% \(', r'92% (', doc)
doc = re.sub(r'35/39 itens', r'36/39 itens', doc)
with open(DOC_FILE, 'w', encoding='utf-8') as f:
    f.write(doc)

with open(ROADMAP_MD, 'r', encoding='utf-8') as f:
    r1 = f.read()
r1 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(90%', r'Sprint 7 - Gerenciamento de Requisicoes (92%', r1)
with open(ROADMAP_MD, 'w', encoding='utf-8') as f:
    f.write(r1)

with open(ROADMAP_ITSM, 'r', encoding='utf-8') as f:
    r2 = f.read()
r2 = re.sub(r'Sprint 7 - Gerenciamento de Requisicoes \(90%', r'Sprint 7 - Gerenciamento de Requisicoes (92%', r2)
with open(ROADMAP_ITSM, 'w', encoding='utf-8') as f:
    f.write(r2)
