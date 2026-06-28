import re
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

completed = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 15, 16, 17, 18, 19, 20, 21, 25, 26, 31, 32, 33, 35, 36, 30 }

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

# Replace the existing 4 items with the 39 items
content = re.sub(
    r'        new { Id = Guid\.Parse\(\"78787878-7878-7878-7878-000000000109\"\).*?00000112\"\)[^\n]+\n',
    '\n'.join(lines) + '\n',
    content,
    flags=re.DOTALL
)

content = content.replace('PercentualImplementacao = 85,', 'PercentualImplementacao = 87,')
content = content.replace('ProximaAcao = "Testar regressao de aprovacao legada e motor novo"', 'ProximaAcao = "Criar ou revisar migrations estruturais, se necessarias"')

with open('src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs', 'w', encoding='utf-8-sig') as f:
    f.write(content)
