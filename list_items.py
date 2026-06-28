import re
SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'
with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

parts = c.split('RoadmapItsmItem18Id,')
if len(parts) > 1:
    sub = parts[1]
    matches = re.findall(r'Titulo = "([^"]+)"', sub)
    for i, m in enumerate(matches):
        print(f'{i+1}: {m}')
