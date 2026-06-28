with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'r', encoding='utf-8') as f:
    c = f.read()

c = c.replace('"Testar regressao de aprovacao legada e motor novo"', '"Criar ou revisar migrations estruturais, se necessarias"')

with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'w', encoding='utf-8') as f:
    f.write(c)
