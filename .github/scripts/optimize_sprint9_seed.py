from pathlib import Path
import re

path = Path('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs')
raw = path.read_bytes()
has_bom = raw.startswith(b'\xef\xbb\xbf')
text = raw.decode('utf-8-sig')

pattern = re.compile(
    r'^[ \t]*new \{ Id = Guid\.Parse\("([^"]+)"\), '
    r'RoadmapItemId = RoadmapItsmItem17Id, Titulo = "([^"]+)", '
    r'Descricao = "Sprint 9 Gerenciamento de Incidentes", '
    r'Grupo = GrupoRoadmapChecklist\.([A-Za-z]+), Ordem = (\d+), '
    r'Concluido = (true|false),.*$\n?',
    re.MULTILINE,
)
matches = list(pattern.finditer(text))
if len(matches) != 59:
    raise SystemExit(f'Esperados 59 itens detalhados da Sprint 9; encontrados {len(matches)}.')

orders = [int(match.group(4)) for match in matches]
if orders != list(range(1, 60)):
    raise SystemExit(f'Ordenacao invalida da Sprint 9: {orders}')

tuple_lines = []
for match in matches:
    item_id, title, group, _, completed = match.groups()
    tuple_lines.append(
        f'            (Guid.Parse("{item_id}"), "{title}", '
        f'GrupoRoadmapChecklist.{group}, {completed}),' 
    )

first_start = matches[0].start()
last_end = matches[-1].end()
text = text[:first_start] + '        .. CriarChecklistSprint9GerenciamentoIncidentes(),\n' + text[last_end:]

helper_name = 'CriarChecklistSprint9GerenciamentoIncidentes'
if f'private static object[] {helper_name}()' in text:
    raise SystemExit('Helper da Sprint 9 ja existe antes da otimizacao.')

field_start = text.index('public static readonly object[] RoadmapChecklistItens')
field_end = text.index('\n    ];', field_start) + len('\n    ];')
helper = f'''\n\n    private static object[] {helper_name}()\n    {{\n        var itens = new (Guid Id, string Titulo, GrupoRoadmapChecklist Grupo, bool Concluido)[]\n        {{\n{chr(10).join(tuple_lines)}\n        }};\n\n        return itens\n            .Select((item, index) => (object)new\n            {{\n                item.Id,\n                RoadmapItemId = RoadmapItsmItem17Id,\n                item.Titulo,\n                Descricao = "Sprint 9 Gerenciamento de Incidentes",\n                item.Grupo,\n                Ordem = index + 1,\n                item.Concluido,\n                Obrigatorio = true,\n                Ativo = true,\n                CriadoEm = DataBase,\n                CriadoPor = UsuarioSistema,\n                AtualizadoEm = item.Concluido ? DataBase : (DateTime?)null,\n                AtualizadoPor = item.Concluido ? UsuarioSistema : null\n            }})\n            .ToArray();\n    }}'''
text = text[:field_end] + helper + text[field_end:]

encoding = 'utf-8-sig' if has_bom else 'utf-8'
path.write_text(text, encoding=encoding, newline='\n')
print('SeedData otimizado com helper tipado para os 59 itens da Sprint 9.')
