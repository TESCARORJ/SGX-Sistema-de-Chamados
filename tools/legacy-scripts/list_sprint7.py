import re

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'r', encoding='utf-8-sig') as f:
    for line in f:
        if 'RoadmapItemId = RoadmapItsmItem18Id' in line:
            m1 = re.search(r'Ordem = (\d+)', line)
            m2 = re.search(r'Concluido = (true|false)', line)
            if m1 and m2:
                print(f'{m1.group(1)}: {m2.group(1)}')
