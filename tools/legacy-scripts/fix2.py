import re

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'r', encoding='utf-8-sig') as f:
    c = f.read()

c = re.sub(
    r'PercentualImplementacao = 90,\s*ProximaAcao = "Criar migration de dados ou checklist, se aplicavel"',
    'PercentualImplementacao = 92,\n            ProximaAcao = "Registrar homologacao funcional"',
    c
)

c = re.sub(
    r'(Ordem = 39, Concluido = )false',
    r'\g<1>true',
    c
)

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'w', encoding='utf-8-sig') as f:
    f.write(c)
