with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'r', encoding='utf-8') as f:
    c = f.read()

# I want to find the first instance of "Criar ou revisar migrations estruturais, se necessarias"
# which is the array one, and replace it with "Testar regressao de aprovacao legada e motor novo"
# wait, there are two instances now. 
# One at line 20: Assert.Equal("Criar ou revisar...", item.ProximaAcao)
# One at line ~60 in the array!

c = c.replace('"Criar ou revisar migrations estruturais, se necessarias",\r\n                "Executar build backend e testes direcionados"', '"Testar regressao de aprovacao legada e motor novo",\r\n                "Executar build backend e testes direcionados"')

c = c.replace('"Criar ou revisar migrations estruturais, se necessarias",\n                "Executar build backend e testes direcionados"', '"Testar regressao de aprovacao legada e motor novo",\n                "Executar build backend e testes direcionados"')


with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'w', encoding='utf-8') as f:
    f.write(c)
