import re
test_file = 'tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs'
with open(test_file, 'r', encoding='utf-8-sig') as f:
    tc = f.read()

tc = re.sub(
    r'"Registrar homologacao funcional"',
    r'"Registrar passagem de conhecimento da governanca"',
    tc
)

with open(test_file, 'w', encoding='utf-8-sig') as f:
    f.write(tc)
