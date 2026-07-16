import re

# 1. Generate SeedData.cs
titles = [
    'Diagnosticar estado atual da Sprint 7 e inconsistencias do roadmap',
    'Confirmar representacao da requisicao de servico como Chamado com NaturezaChamadoEnum.Requisicao',
    'Validar vinculo existente entre Chamado e Catalogo de Servicos',
    'Definir menor escopo seguro da abertura guiada por catalogo',
    'Implementar ou ajustar contrato de consulta do servico para abertura',
    'Implementar ou ajustar contrato de abertura guiada por catalogo com semantica de requisicao',
    'Criar validator dedicado para abertura guiada por catalogo',
    'Implementar use case dedicado de abertura de requisicao de servico via catalogo',
    'Aplicar classificacao vinda do catalogo no backend',
    'Aplicar grupo responsavel configurado no catalogo',
    'Aplicar SLA configurado ou fallback existente',
    'Persistir vinculo entre chamado e servico do catalogo',
    'Implementar ou reutilizar formulario por servico',
    'Validar e persistir respostas do formulario',
    'Gerar aprovacao obrigatoria quando a regra aplicavel exigir',
    'Preservar aprovacao legada sem duplicidade',
    'Preservar abertura de incidentes e chamados sem catalogo',
    'Criar ou ajustar endpoints do portal para catalogo e abertura guiada',
    'Implementar tela de catalogo no portal',
    'Implementar detalhe do servico no portal',
    'Implementar formulario guiado de abertura',
    'Implementar confirmacao e acompanhamento da requisicao aberta',
    'Garantir seguranca, autorizacao e ownership dos endpoints',
    'Registrar historico e auditoria dos eventos relevantes',
    'Testar abertura por catalogo sem aprovacao',
    'Testar abertura por catalogo com aprovacao obrigatoria',
    'Testar formulario obrigatorio e respostas invalidas',
    'Testar grupo responsavel e SLA',
    'Testar regressao de abertura legada, incidente e atendimento',
    'Testar regressao de aprovacao legada e motor novo',
    'Executar build backend e testes direcionados',
    'Executar build frontend e validacao TypeScript',
    'Verificar EF pending model changes',
    'Criar ou revisar migrations estruturais, se necessarias',
    'Criar migration de dados ou checklist, se aplicavel',
    'Atualizar documentacao principal da Sprint 7',
    'Registrar homologacao funcional',
    'Registrar homologacao visual responsiva',
    'Registrar aceite formal somente com evidencia'
]

not_completed = { 10, 13, 14, 34, 39 }
completed = [i for i in range(1, 40) if i not in not_completed]

lines = []
for i, title in enumerate(titles, 1):
    c = 'true' if i in completed else 'false'
    up = 'DataBase, AtualizadoPor = UsuarioSistema' if c == 'true' else '(DateTime?)null, AtualizadoPor = (string?)null'
    grupo = 'GrupoRoadmapChecklist.Desenvolvimento'
    if 'Planejar' in title or 'Diagnosticar' in title: grupo = 'GrupoRoadmapChecklist.Planejamento'
    if 'Testar' in title or 'Executar' in title: grupo = 'GrupoRoadmapChecklist.Testes'
    if 'Homologacao' in title or 'Registrar homologacao' in title or 'aceite' in title: grupo = 'GrupoRoadmapChecklist.Homologacao'
    if 'migration' in title.lower() or 'documentacao' in title.lower(): grupo = 'GrupoRoadmapChecklist.Documentacao'
    if i == 33 or i == 34 or i == 35: grupo = 'GrupoRoadmapChecklist.Desenvolvimento'
    if i == 36: grupo = 'GrupoRoadmapChecklist.Documentacao'
    if i == 23 or i == 24: grupo = 'GrupoRoadmapChecklist.Governanca'
    uid = '78787878-7878-7878-7878-00000001' + str(i).zfill(4)
    line = f'        new {{ Id = Guid.Parse("{uid}"), RoadmapItemId = RoadmapItsmItem18Id, Titulo = "{title}", Descricao = "Sprint 7 Gerenciamento de Requisicoes", Grupo = {grupo}, Ordem = {i}, Concluido = {c}, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = {up} }},'
    lines.append(line)

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'r', encoding='utf-8-sig') as f:
    content = f.read()

content = re.sub(
    r'        new { Id = Guid\.Parse\(\"78787878-7878-7878-7878-000000010001\"\).*?00010039\"\)[^\n]+\n',
    '\n'.join(lines) + '\n',
    content,
    flags=re.DOTALL
)

content = content.replace('PercentualImplementacao = 85,', 'PercentualImplementacao = 87,')
content = content.replace('PercentualImplementacao = 51,', 'PercentualImplementacao = 87,')

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'w', encoding='utf-8-sig') as f:
    f.write(content)


# 2. Fix Tests
with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'r', encoding='utf-8') as f:
    t_content = f.read()

arr_str = ', '.join(str(x) for x in completed)

t_content = re.sub(r'Assert\.Equal\(\d+, item\.PercentualImplementacao\);', 'Assert.Equal(87, item.PercentualImplementacao);', t_content)
t_content = re.sub(r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => x\.Concluido\)\);', 'Assert.Equal(34, checklistAtivo.Count(x => x.Concluido));', t_content)
t_content = re.sub(r'Assert\.Equal\(\d+, checklistAtivo\.Count\(x => !x\.Concluido\)\);', 'Assert.Equal(5, checklistAtivo.Count(x => !x.Concluido));', t_content)

t_content = re.sub(r'new\[\] \{[^\}]+\},\s*checklistAtivo\.Where\(x => x\.Concluido\)\.Select\(x => x\.Ordem\)\.ToArray\(\)\);', f'new[] {{ {arr_str} }},\n            checklistAtivo.Where(x => x.Concluido).Select(x => x.Ordem).ToArray());', t_content)

t_content = re.sub(r'Assert\.Equal\(\d+, detalhe\.QuantidadeChecklistConcluido\);', 'Assert.Equal(34, detalhe.QuantidadeChecklistConcluido);', t_content)
t_content = re.sub(r'Assert\.Equal\(\d+, detalhe\.PercentualImplementacao\);', 'Assert.Equal(87, detalhe.PercentualImplementacao);', t_content)

with open('tests/SGX.SistemaChamado.Tests/RoadmapSprint7GerenciamentoRequisicoesChecklistTests.cs', 'w', encoding='utf-8') as f:
    f.write(t_content)

