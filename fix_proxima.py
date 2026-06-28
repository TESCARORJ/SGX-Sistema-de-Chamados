import re
with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'r', encoding='utf-8-sig') as f:
    c = f.read()

m = re.search(r'Id = RoadmapItsmItem18Id.*?ProximaAcao = "([^"]+)"', c, re.DOTALL)
if m:
    print('Current:', m.group(1))
    c = c.replace(m.group(1), "Criar ou revisar migrations estruturais, se necessarias")
    with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'w', encoding='utf-8-sig') as f:
        f.write(c)
    print('Replaced!')
