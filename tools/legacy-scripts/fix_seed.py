import re

SEED_FILE = 'src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs'

with open(SEED_FILE, 'r', encoding='utf-8-sig') as f:
    c = f.read()

lines = c.split('\n')

in_item18_block = False
for j in range(len(lines)):
    if 'RoadmapItemId = RoadmapItsmItem18Id' in lines[j]:
        m = re.search(r'Ordem = (\d+)', lines[j])
        if m:
            ordem = int(m.group(1))
            if ordem in [10, 13, 14, 37, 38, 39]:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = false', lines[j])
            else:
                lines[j] = re.sub(r'Concluido = (false|true)', 'Concluido = true', lines[j])

c = '\n'.join(lines)

# Fix percent for item 18 only. The PercentualImplementacao is set when the new anonymous object is created for RoadmapItsmItens.
# We need to find the specific block for RoadmapItsmItem18Id in the RoadmapItsmItens array.
# The format is typically:
# new {
#     Id = RoadmapItsmItem18Id,
#     ...
#     PercentualImplementacao = 85,
#     PendenciasTecnicas = ...
# }

def replace_percent_for_item18(text):
    # Regex to find the block starting with Id = RoadmapItsmItem18Id and containing PercentualImplementacao
    # Since it might span multiple lines, we'll do this carefully.
    parts = text.split('RoadmapItsmItem18Id,')
    if len(parts) > 1:
        # The part AFTER RoadmapItsmItem18Id, contains the properties for this item until the next "new {" or similar.
        sub_part = parts[1]
        sub_part = re.sub(
            r'PercentualImplementacao = \d+',
            r'PercentualImplementacao = 85',
            sub_part,
            count=1 # only replace the first occurrence after Item18Id
        )
        parts[1] = sub_part
    return 'RoadmapItsmItem18Id,'.join(parts)

c = replace_percent_for_item18(c)

with open(SEED_FILE, 'w', encoding='utf-8-sig') as f:
    f.write(c)
