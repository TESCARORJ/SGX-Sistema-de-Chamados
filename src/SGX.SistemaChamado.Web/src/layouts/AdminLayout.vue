<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore, obterRotaInicialParaPerfil } from '../stores/authStore'
import NotificacoesBadge from '../components/notificacoes/NotificacoesBadge.vue'
import { permissoes } from '../constants/permissoes'
import type { PerfilEmulado } from '../types/auth'

type MenuItem = {
  label: string
  icon: string
  to?: string
  children?: MenuItem[]
  adminOnly?: boolean
  requiredAnyPermissions?: string[]
}

type GrupoMenuKey =
  | 'atendimento'
  | 'cadastros'
  | 'configuracoes'
  | 'integracoes'
  | 'sla'
  | 'gestao-itsm'
  | 'governanca'
  | 'conhecimento'
  | 'infraestrutura'
  | 'relatorios'

type GrupoMenu = {
  key: GrupoMenuKey
  label: string
  icon: string
  items: MenuItem[]
}

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const drawerOpen = ref(!$q.screen.lt.md)
const drawerMini = ref(false)
const emulacaoSolicitanteCarregando = ref(false)
const emulacaoAtendenteCarregando = ref(false)
const retornoEmulacaoCarregando = ref(false)
const buscaGlobal = ref('')

const gruposMenuDefinicao: Array<Omit<GrupoMenu, 'items'>> = [
  { key: 'atendimento', label: 'Atendimento', icon: 'support_agent' },
  { key: 'cadastros', label: 'Cadastros', icon: 'dataset' },
  { key: 'configuracoes', label: 'Configurações', icon: 'settings' },
  { key: 'integracoes', label: 'Integrações', icon: 'hub' },
  { key: 'sla', label: 'SLA', icon: 'schedule' },
  { key: 'gestao-itsm', label: 'Gestão ITSM', icon: 'insights' },
  { key: 'governanca', label: 'Governança', icon: 'fact_check' },
  { key: 'conhecimento', label: 'Conhecimento', icon: 'menu_book' },
  { key: 'infraestrutura', label: 'Infraestrutura', icon: 'memory' },
  { key: 'relatorios', label: 'Relatórios', icon: 'analytics' },
]

const menu = computed<MenuItem[]>(() => {
  const perfis = authStore.usuario?.perfis ?? []

  if (perfis.includes('Administrador')) {
    return [
      {
        label: 'Minhas pendências',
        icon: 'fact_check',
        to: '/admin/atendimento/aprovacoes-pendentes',
        requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar],
      },
      {
        label: 'Usuários',
        icon: 'group',
        to: '/admin/cadastros/usuarios',
        requiredAnyPermissions: [permissoes.usuariosVisualizar],
      },
      {
        label: 'Perfis e permissões',
        icon: 'badge',
        to: '/admin/cadastros/perfis',
        requiredAnyPermissions: [permissoes.perfisVisualizar],
      },
      {
        label: 'Cadastros administrativos',
        icon: 'dataset',
        requiredAnyPermissions: [permissoes.cadastrosVisualizar],
        children: [
          {
            label: 'Departamentos',
            icon: 'apartment',
            to: '/admin/cadastros/departamentos',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Categorias',
            icon: 'category',
            to: '/admin/cadastros/categorias',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Subcategorias',
            icon: 'account_tree',
            to: '/admin/cadastros/subcategorias',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Prioridades',
            icon: 'priority_high',
            to: '/admin/cadastros/prioridades',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Tipos de Solicitação',
            icon: 'sell',
            to: '/admin/cadastros/tipos-solicitacao',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Locais / Unidades',
            icon: 'location_city',
            to: '/admin/cadastros/locais',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Status',
            icon: 'flag',
            to: '/admin/cadastros/status',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
          {
            label: 'Grupos Tecnicos',
            icon: 'groups',
            to: '/admin/cadastros/grupos-tecnicos',
            requiredAnyPermissions: [permissoes.cadastrosVisualizar],
          },
        ],
      },
      {
        label: 'SLA',
        icon: 'schedule',
        requiredAnyPermissions: [permissoes.slaVisualizar],
        children: [
          {
            label: 'Políticas',
            icon: 'rule',
            to: '/admin/sla/policies',
            requiredAnyPermissions: [permissoes.slaVisualizar],
          },
          {
            label: 'Painel',
            icon: 'monitoring',
            to: '/admin/sla/painel',
            requiredAnyPermissions: [permissoes.slaVisualizar],
          },
          {
            label: 'Alertas',
            icon: 'notifications_active',
            to: '/admin/sla/alertas',
            requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
          },
          {
            label: 'Calendários',
            icon: 'event_available',
            to: '/admin/sla/calendarios',
            requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
          },
        ],
      },
      {
        label: 'Integrações',
        icon: 'hub',
        requiredAnyPermissions: [
          permissoes.integracoesEmailVisualizar,
          permissoes.integracoesMicrosoftVisualizar,
          permissoes.integracoesActiveDirectoryVisualizar,
          permissoes.autenticacaoProvedoresVisualizar,
        ],
        children: [
          {
            label: 'E-mail',
            icon: 'mail',
            to: '/admin/integracoes/email',
            requiredAnyPermissions: [permissoes.integracoesEmailVisualizar],
          },
          {
            label: 'Microsoft Entra ID',
            icon: 'shield',
            to: '/admin/integracoes/microsoft-entra-id',
            requiredAnyPermissions: [permissoes.integracoesMicrosoftVisualizar],
          },
          {
            label: 'Active Directory / LDAP',
            icon: 'domain',
            to: '/admin/integracoes/active-directory',
            requiredAnyPermissions: [permissoes.integracoesActiveDirectoryVisualizar],
          },
          {
            label: 'Métodos de login',
            icon: 'login',
            to: '/admin/integracoes/metodos-login',
            requiredAnyPermissions: [permissoes.autenticacaoProvedoresVisualizar],
          },
        ],
      },
      {
        label: 'Configurações',
        icon: 'settings',
        requiredAnyPermissions: [permissoes.parametrosVisualizar],
        children: [
          {
            label: 'Parâmetros do Sistema',
            icon: 'tune',
            to: '/admin/configuracoes/parametros',
            requiredAnyPermissions: [permissoes.parametrosVisualizar],
          },
          {
            label: 'Regras de Aprovação',
            icon: 'rule',
            to: '/admin/configuracoes/regras-aprovacao',
            requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar, permissoes.aprovacaoChamadosGerenciar],
          },
        ],
      },
      {
        label: 'Roadmap',
        icon: 'insights',
        requiredAnyPermissions: [permissoes.roadmapVisualizar],
        children: [
          {
            label: 'Roadmap ITSM',
            icon: 'account_tree',
            to: '/admin/gestao-itsm/roadmap',
            requiredAnyPermissions: [permissoes.roadmapVisualizar],
          },
          {
            label: 'Documentação',
            icon: 'library_books',
            to: '/admin/gestao-itsm/documentacao',
            requiredAnyPermissions: [permissoes.roadmapVisualizar],
          },
        ],
      },
    ]
  }

  if (perfis.includes('Coordenador Service Desk')) {
    return [
      {
        label: 'Dashboard operacional',
        icon: 'space_dashboard',
        to: '/admin',
        requiredAnyPermissions: [permissoes.dashboardVisualizar],
      },
      {
        label: 'Fila geral',
        icon: 'toc',
        to: '/admin/chamados',
        requiredAnyPermissions: [permissoes.chamadosVisualizarTodos],
      },
      {
        label: 'Minhas pendências',
        icon: 'fact_check',
        to: '/admin/atendimento/aprovacoes-pendentes',
        requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar],
      },
      {
        label: 'Chamados críticos',
        icon: 'priority_high',
        to: '/admin/chamados?prioridade=critica',
        requiredAnyPermissions: [permissoes.chamadosVisualizarTodos],
      },
      {
        label: 'SLA',
        icon: 'schedule',
        requiredAnyPermissions: [permissoes.slaVisualizar],
        children: [
          {
            label: 'Políticas',
            icon: 'rule',
            to: '/admin/sla/policies',
            requiredAnyPermissions: [permissoes.slaVisualizar],
          },
          {
            label: 'Painel',
            icon: 'monitoring',
            to: '/admin/sla/painel',
            requiredAnyPermissions: [permissoes.slaVisualizar],
          },
          {
            label: 'Alertas',
            icon: 'notifications_active',
            to: '/admin/sla/alertas',
            requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
          },
          {
            label: 'Calendários',
            icon: 'event_available',
            to: '/admin/sla/calendarios',
            requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
          },
        ],
      },
      {
        label: 'Atribuições',
        icon: 'group_add',
        to: '/admin/chamados',
        requiredAnyPermissions: [permissoes.chamadosAtribuir],
      },
      {
        label: 'Relatórios operacionais',
        icon: 'analytics',
        to: '/admin/relatorios/chamados',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosOperacional],
      },
    ]
  }

  if (perfis.includes('Técnico N2')) {
    return [
      {
        label: 'Minha fila técnica',
        icon: 'engineering',
        to: '/admin/chamados?filtro=tecnico',
        requiredAnyPermissions: [permissoes.chamadosVisualizar],
      },
      {
        label: 'Minhas pendências',
        icon: 'fact_check',
        to: '/admin/atendimento/aprovacoes-pendentes',
        requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar],
      },
      {
        label: 'Chamados escalados',
        icon: 'arrow_upward',
        to: '/admin/chamados?filtro=escalados',
        requiredAnyPermissions: [permissoes.chamadosVisualizarTodos],
      },
      {
        label: 'Problemas',
        icon: 'report_problem',
        to: '/admin/chamados?tipo=Problema',
        requiredAnyPermissions: [permissoes.problemasVisualizar],
      },
      {
        label: 'Mudanças',
        icon: 'published_with_changes',
        to: '/admin/chamados?tipo=Mudanca',
        requiredAnyPermissions: [permissoes.mudancasVisualizar],
      },
      {
        label: 'Tarefas operacionais',
        icon: 'playlist_add_check',
        to: '/admin/chamados?tipo=Tarefa',
        requiredAnyPermissions: [permissoes.tarefasVisualizar],
      },
      {
        label: 'Base de conhecimento',
        icon: 'menu_book',
        to: '/admin/conhecimento/base-conhecimento',
        requiredAnyPermissions: [permissoes.baseConhecimentoVisualizar],
      },
    ]
  }

  if (perfis.includes('Atendente N1')) {
    return [
      {
        label: 'Fila de atendimento',
        icon: 'list_alt',
        to: '/admin/chamados',
        requiredAnyPermissions: [permissoes.chamadosVisualizarTodos],
      },
      {
        label: 'Minhas pendências',
        icon: 'fact_check',
        to: '/admin/atendimento/aprovacoes-pendentes',
        requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar],
      },
      {
        label: 'Meus atendimentos',
        icon: 'assignment_ind',
        to: '/admin/chamados?filtro=meus',
        requiredAnyPermissions: [permissoes.chamadosVisualizar],
      },
      {
        label: 'Triagem',
        icon: 'assignment',
        to: '/admin/chamados?filtro=triagem',
        requiredAnyPermissions: [permissoes.chamadosAtribuir],
      },
      {
        label: 'Base de conhecimento',
        icon: 'menu_book',
        to: '/admin/conhecimento/base-conhecimento',
        requiredAnyPermissions: [permissoes.baseConhecimentoVisualizar],
      },
    ]
  }

  if (perfis.includes('Gestor TI')) {
    return [
      {
        label: 'Dashboard executivo',
        icon: 'dashboard',
        to: '/admin/relatorios/avancados',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosGerencial],
      },
      {
        label: 'Indicadores ITSM',
        icon: 'trending_up',
        to: '/admin/relatorios/chamados',
        requiredAnyPermissions: [permissoes.dashboardVisualizar],
      },
      {
        label: 'Relatórios',
        icon: 'analytics',
        to: '/admin/relatorios/chamados',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'SLA',
        icon: 'schedule',
        to: '/admin/relatorios/sla',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'Mudanças',
        icon: 'published_with_changes',
        to: '/admin/relatorios/aprovacoes',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'Problemas recorrentes',
        icon: 'sync_problem',
        to: '/admin/relatorios/auditoria',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
    ]
  }

  if (perfis.includes('Auditor Governança')) {
    return [
      {
        label: 'Consulta de chamados',
        icon: 'search',
        to: '/admin/chamados',
        requiredAnyPermissions: [permissoes.chamadosVisualizar],
      },
      {
        label: 'Histórico e auditoria',
        icon: 'manage_search',
        to: '/admin/governanca/auditoria',
        requiredAnyPermissions: [permissoes.auditoriaVisualizar],
      },
      {
        label: 'Relatórios',
        icon: 'analytics',
        to: '/admin/relatorios/chamados',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'SLA',
        icon: 'schedule',
        to: '/admin/relatorios/sla',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'Aprovações',
        icon: 'fact_check',
        to: '/admin/relatorios/aprovacoes',
        requiredAnyPermissions: [permissoes.relatoriosAvancadosVisualizar],
      },
      {
        label: 'Logs/Auditoria',
        icon: 'security',
        to: '/admin/governanca/auditoria-autenticacao',
        requiredAnyPermissions: [permissoes.auditoriaAutenticacaoVisualizar],
      },
    ]
  }

  // Fallback / Atendente padrão legado
  return [
    {
      label: 'Dashboard',
      icon: 'space_dashboard',
      to: '/admin',
      requiredAnyPermissions: [permissoes.dashboardVisualizar],
    },
    {
      label: 'Fila de chamados',
      icon: 'list_alt',
      to: '/admin/chamados',
      requiredAnyPermissions: [permissoes.chamadosVisualizarTodos],
    },
    {
      label: 'Minhas pendências',
      icon: 'fact_check',
      to: '/admin/atendimento/aprovacoes-pendentes',
      requiredAnyPermissions: [permissoes.aprovacaoChamadosVisualizar],
    },
    {
      label: 'Base de conhecimento',
      icon: 'menu_book',
      to: '/admin/conhecimento/base-conhecimento',
      requiredAnyPermissions: [permissoes.baseConhecimentoVisualizar],
    },
    {
      label: 'Grupos Tecnicos',
      icon: 'groups',
      to: '/admin/cadastros/grupos-tecnicos',
    },
  ]
})

const usuarioNome = computed(() => authStore.usuario?.nome || 'Administrador SGX')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')
const usuarioPerfil = computed(() => authStore.usuario?.perfis?.[0] || 'Perfil não identificado')
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const quantidadePermissoes = computed(() => authStore.usuario?.permissoes?.length ?? 0)
const fallbackAdminSemPermissoes = computed(() => usuarioEhAdministrador.value && quantidadePermissoes.value === 0)
const podeEmularQualquerPerfil = computed(() => {
  if (!authStore.modoLocal) {
    return false
  }
  const originalEhAdmin = authStore.obterUsuarioOriginalPersistido()?.perfil === 'Administrador'
  const atualEhAdmin = (authStore.usuario?.perfis ?? []).includes('Administrador')
  return originalEhAdmin || atualEhAdmin
})

const opcoesEmulacao = computed(() => {
  const opcoes = [
    { label: 'Administrador', value: 'Administrador' },
    { label: 'Solicitante', value: 'Solicitante' },
    { label: 'Atendente', value: 'Atendente' },
    { label: 'Atendente N1', value: 'Atendente N1' },
    { label: 'Técnico N2', value: 'Técnico N2' },
    { label: 'Coordenador Service Desk', value: 'Coordenador Service Desk' },
    { label: 'Gestor TI', value: 'Gestor TI' },
    { label: 'Auditor Governança', value: 'Auditor Governança' }
  ]

  if (authStore.emulandoPerfil) {
    opcoes.unshift({ label: 'Perfil original', value: 'Original' })
  }

  return opcoes
})

const perfilSelecionado = computed({
  get() {
    if (authStore.emulandoPerfil && authStore.perfilEmulado) {
      return authStore.perfilEmulado
    }
    return authStore.usuario?.perfis?.[0] || 'Administrador'
  },
  async set(val: string) {
    if (val === 'Original') {
      await voltarParaAdministrador()
    } else {
      await alterarVisaoHomologacao(val as PerfilEmulado)
    }
  }
})

const emulacaoDisponivel = computed(
  () => authStore.podeEmularSolicitante && authStore.podeEmularAtendente && !authStore.emulandoPerfil
)
const emulandoAtendente = computed(
  () => authStore.emulandoPerfil && authStore.perfilEmulado === 'Atendente'
)

function podeExibirItemMenu(item: MenuItem): boolean {
  if (item.adminOnly && !usuarioEhAdministrador.value) {
    return false
  }

  if (!item.requiredAnyPermissions?.length) {
    return true
  }

  if (fallbackAdminSemPermissoes.value) {
    return true
  }

  return authStore.possuiAlgumaPermissao(item.requiredAnyPermissions)
}

const menuVisivel = computed<MenuItem[]>(() =>
  menu.value
    .filter((item) => podeExibirItemMenu(item))
    .map((item) => {
      if (!item.children?.length) {
        return item
      }

      const filhos = item.children.filter((child) => podeExibirItemMenu(child))
      if (!filhos.length && !item.to) {
        return null
      }

      return {
        ...item,
        children: filhos,
      }
    })
    .filter((item): item is MenuItem => Boolean(item))
)

function resolverGrupoMenu(item: MenuItem): GrupoMenuKey {
  switch (item.label) {
    case 'Cadastros':
    case 'Usuários':
    case 'Perfis e permissões':
    case 'Cadastros administrativos':
    case 'Grupos Tecnicos':
      return 'cadastros'
    case 'Configurações':
      return 'configuracoes'
    case 'Integrações':
      return 'integracoes'
    case 'SLA':
      return 'sla'
    case 'Roadmap':
    case 'Gestão ITSM':
      return 'gestao-itsm'
    case 'Governança':
    case 'Histórico e auditoria':
    case 'Logs/Auditoria':
      return 'governanca'
    case 'Conhecimento':
    case 'Base de conhecimento':
      return 'conhecimento'
    case 'Infraestrutura':
      return 'infraestrutura'
    case 'Relatórios':
    case 'Dashboard executivo':
    case 'Relatórios operacionais':
    case 'Problemas recorrentes':
    case 'Aprovações':
      return 'relatorios'
    case 'Dashboard':
    case 'Atendimento':
    case 'Fila de atendimento':
    case 'Meus atendimentos':
    case 'Triagem':
    case 'Minha fila técnica':
    case 'Chamados escalados':
    case 'Problemas':
    case 'Mudanças':
    case 'Tarefas operacionais':
    case 'Dashboard operacional':
    case 'Fila geral':
    case 'Chamados críticos':
    case 'Atribuições':
    case 'Consulta de chamados':
    case 'Notificações':
    default:
      return 'atendimento'
  }
}

function normalizarLabelMenu(valor: string): string {
  return valor
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .replace(/\s+/g, ' ')
    .trim()
}

const menuAgrupado = computed<GrupoMenu[]>(() =>
  gruposMenuDefinicao
    .map((grupo) => {
      const itensDoGrupo = menuVisivel.value.filter((item) => resolverGrupoMenu(item) === grupo.key)

      const itensNormalizados = itensDoGrupo.flatMap((item) => {
        const itemEhAgrupadorRedundante =
          !item.to &&
          Boolean(item.children?.length) &&
          normalizarLabelMenu(item.label) === normalizarLabelMenu(grupo.label)

        if (itemEhAgrupadorRedundante) {
          return item.children ?? []
        }

        return [item]
      })

      return {
        ...grupo,
        items: itensNormalizados,
      }
    })
    .filter((grupo) => grupo.items.length > 0)
)
const statusUsuario = computed(() => (authStore.autenticado ? 'Online' : 'Offline'))
const tituloPagina = computed(() => {
  if (route.path === '/admin') return 'Dashboard'
  if (route.path.startsWith('/admin/relatorios/avancados')) return 'Relatórios avançados'
  if (route.path.startsWith('/admin/relatorios/chamados')) return 'Relatórios - Chamados'
  if (route.path.startsWith('/admin/relatorios/sla')) return 'Relatórios - SLA'
  if (route.path.startsWith('/admin/relatorios/aprovacoes')) return 'Relatórios - Aprovações'
  if (route.path.startsWith('/admin/relatorios/catalogo-servicos')) return 'Relatórios - Catálogo de serviços'
  if (route.path.startsWith('/admin/relatorios/inventario-ativos')) return 'Relatórios - Inventário/Ativos'
  if (route.path.startsWith('/admin/relatorios/base-conhecimento')) return 'Relatórios - Base de conhecimento'
  if (route.path.startsWith('/admin/relatorios/auditoria')) return 'Relatórios - Auditoria'
  if (route.path === '/admin/chamados') return 'Fila de Chamados'
  if (route.path.startsWith('/admin/atendimento/aprovacoes-pendentes')) return 'Minhas pendências de aprovação'
  if (route.path.startsWith('/admin/atendimento/aprovacao-chamados')) return 'Aprovação de chamados'
  if (route.path === '/admin/notificacoes') return 'Central de Notificações'
  if (route.path.startsWith('/admin/cadastros/grupos-tecnicos')) return 'Grupos Tecnicos'
  if (route.path.startsWith('/admin/sla/alertas')) return 'Alertas de SLA'
  if (route.path.startsWith('/admin/sla/calendarios')) return 'Calendários de SLA'
  if (route.path.startsWith('/admin/sla/painel')) return 'Painel de SLA'
  if (route.path.startsWith('/admin/sla')) return 'Políticas de SLA'
  if (route.path.startsWith('/admin/chamados/')) return 'Detalhe do Chamado'
  if (route.path.startsWith('/admin/cadastros')) return 'Cadastros'
  if (route.path.startsWith('/admin/conhecimento/base-conhecimento')) return 'Base de conhecimento'
  if (route.path.startsWith('/admin/conhecimento/catalogo-servicos')) return 'Catálogo de serviços'
  if (route.path.startsWith('/admin/infraestrutura/inventario-ativos')) return 'Inventário/Ativos'
  if (route.path.startsWith('/admin/configuracoes')) return 'Configurações'
  if (route.path.startsWith('/admin/integracoes/active-directory')) return 'Active Directory / LDAP'
  if (route.path.startsWith('/admin/integracoes/metodos-login')) return 'Métodos de login'
  if (route.path.startsWith('/admin/integracoes/autenticacao/metodos-login')) return 'Métodos de login'
  if (route.path.startsWith('/admin/integracoes')) return 'Integrações'
  if (route.path.startsWith('/admin/governanca/auditoria-autenticacao')) return 'Auditoria de autenticação'
  if (route.path.startsWith('/admin/governanca/auditoria')) return 'Histórico / Auditoria'
  if (route.path.startsWith('/admin/gestao-itsm/documentacao')) return 'Documentação ITSM'
  if (route.path.startsWith('/admin/gestao-itsm')) return 'Gestão ITSM'
  if (route.path.startsWith('/admin/roadmap-itsm')) return 'Roadmap ITSM'
  return 'Área Administrativa'
})
const iniciaisUsuario = computed(() => {
  const nome = usuarioNome.value.trim()
  if (!nome) return 'AS'
  return nome
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((parte) => parte[0]?.toUpperCase() || '')
    .join('')
})

function rotaAtiva(path: string): boolean {
  if (path === '/admin') {
    return route.path === '/admin'
  }

  return route.path === path || route.path.startsWith(`${path}/`)
}

function grupoAberto(children: MenuItem[] | undefined): boolean {
  if (!children?.length) {
    return false
  }

  return children.some((item) => item.to && rotaAtiva(item.to))
}

function alternarMenu(): void {
  if ($q.screen.lt.md) {
    drawerOpen.value = !drawerOpen.value
    return
  }

  drawerMini.value = !drawerMini.value
}

function acionarBuscaGlobal(): void {
  const texto = buscaGlobal.value.trim()
  const possuiBuscaAtual = Boolean(route.query.texto ?? route.query.busca)

  if (!texto && route.path !== '/admin/chamados' && !possuiBuscaAtual) {
    return
  }

  const query = texto ? { texto } : {}

  router.push({
    path: '/admin/chamados',
    query,
  })
}

function extrairTextoBuscaQuery(): string {
  const valor = route.query.texto ?? route.query.busca

  if (Array.isArray(valor)) {
    return (valor[0] ?? '').trim()
  }

  return typeof valor === 'string' ? valor.trim() : ''
}

async function logout(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}

function obterRotaPadraoPerfil(perfil: PerfilEmulado): string {
  return obterRotaInicialParaPerfil(perfil)
}

async function alterarVisaoHomologacao(perfil: PerfilEmulado): Promise<void> {
  try {
    await authStore.iniciarEmulacaoPerfil(perfil)

    const currentRoute = router.currentRoute.value
    
    if (perfil === 'Solicitante') {
      await router.replace('/portal/chamados')
      return
    }

    if (!currentRoute.path.startsWith('/admin')) {
      await router.replace(obterRotaPadraoPerfil(perfil))
      return
    }

    const perfisPermitidos = currentRoute.meta.perfisPermitidos
    const requiredAnyPermissions = currentRoute.meta.requiredAnyPermissions
    
    let permitido = true
    if (perfisPermitidos?.length) {
      const perfisUsuario = authStore.usuario?.perfis ?? []
      permitido = perfisPermitidos.some((p) => perfisUsuario.includes(p))
    }
    
    if (permitido && requiredAnyPermissions?.length) {
      const usuarioEhAdministrador = (authStore.usuario?.perfis ?? []).includes('Administrador')
      const fallbackAdminSemPermissoes = usuarioEhAdministrador && (authStore.usuario?.permissoes?.length ?? 0) === 0
      permitido = fallbackAdminSemPermissoes || authStore.possuiAlgumaPermissao(requiredAnyPermissions)
    }

    if (!permitido) {
      await router.replace(obterRotaPadraoPerfil(perfil))
    } else {
      await router.replace(currentRoute.fullPath)
    }
    
    $q.notify({
      type: 'success',
      message: `Visão alterada para ${perfil}`,
      position: 'top',
      timeout: 1000
    })
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
    $q.notify({
      type: 'negative',
      message,
    })
  }
}

async function visualizarComoSolicitante(): Promise<void> {
  await alterarVisaoHomologacao('Solicitante')
}

async function visualizarComoAtendente(): Promise<void> {
  await alterarVisaoHomologacao('Atendente')
}

async function voltarParaAdministrador(): Promise<void> {
  if (!authStore.emulandoPerfil || retornoEmulacaoCarregando.value) {
    return
  }

  retornoEmulacaoCarregando.value = true

  try {
    await authStore.encerrarEmulacao()
    await router.replace(authStore.rotaInicial)
    $q.notify({
      type: 'success',
      message: 'Voltou para o perfil original',
      position: 'top',
      timeout: 1000
    })
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
    $q.notify({
      type: 'negative',
      message,
    })

    if (
      message.includes('Contexto original da emulacao nao encontrado') ||
      message.includes('Contexto original da emulação não encontrado')
    ) {
      await router.replace('/login')
    }
  } finally {
    retornoEmulacaoCarregando.value = false
  }
}

function navegarPara(path: string): void {
  if (route.path !== path) {
    router.push(path)
  }

  if ($q.screen.lt.md) {
    drawerOpen.value = false
  }
}

watch(
  () => [route.path, route.query.texto, route.query.busca],
  () => {
    if (!route.path.startsWith('/admin')) {
      return
    }

    buscaGlobal.value = extrairTextoBuscaQuery()
  },
  { immediate: true }
)
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="admin-layout">
    <q-header elevated class="admin-header text-dark">
      <q-toolbar class="q-px-md q-py-sm header-toolbar">
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="alternarMenu" />

        <div class="q-ml-sm header-title">
          <div class="text-overline text-grey-7">SGX Sistema de Chamados</div>
          <q-toolbar-title class="text-subtitle1 text-weight-bold header-title__value">
            {{ tituloPagina }}
          </q-toolbar-title>
        </div>

        <q-space />

        <q-input
          v-model="buscaGlobal"
          class="header-search gt-sm"
          dense
          outlined
          aria-label="Buscar chamados"
          placeholder="Buscar chamados, solicitantes, categorias..."
          @keyup.enter="acionarBuscaGlobal"
        >
          <template #prepend>
            <q-icon name="search" />
          </template>
          <template #append>
            <q-btn flat round dense icon="arrow_forward" aria-label="Executar busca global" @click="acionarBuscaGlobal" />
          </template>
        </q-input>

        <NotificacoesBadge compact />

        <q-chip square class="q-ml-sm user-chip">
          <q-avatar color="primary" text-color="white">{{ iniciaisUsuario }}</q-avatar>
          <div class="column">
            <div class="text-caption text-weight-medium ellipsis user-chip__name">{{ usuarioNome }}</div>
            <div class="row items-center q-gutter-xs">
              <span class="status-dot" />
              <span class="text-caption text-grey-7">{{ statusUsuario }}</span>
            </div>
            <div class="text-caption text-grey-7 ellipsis user-chip__perfil">{{ usuarioPerfil }}</div>
          </div>
          <q-tooltip>
            <div>{{ usuarioNome }}</div>
            <div>{{ usuarioEmail }}</div>
            <div>{{ usuarioPerfil }}</div>
          </q-tooltip>
        </q-chip>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :mini="drawerMini && !$q.screen.lt.md"
      :mini-width="78"
      :width="296"
      :breakpoint="1024"
      :behavior="$q.screen.lt.md ? 'mobile' : 'desktop'"
      class="admin-drawer"
      content-class="admin-drawer__content"
    >
      <div class="admin-sidebar">
        <div class="q-pa-md drawer-brand">
          <q-avatar size="42px" color="secondary" text-color="white" icon="support_agent" />
          <div v-if="!drawerMini">
            <div class="drawer-brand__title">SGX</div>
            <div class="drawer-brand__name">Sistema de Chamados</div>
            <div class="text-caption drawer-brand__subtitle">Painel administrativo</div>
          </div>
        </div>

        <q-separator />

        <div v-if="podeEmularQualquerPerfil" class="q-px-md q-py-sm emulacao-acoes">
          <q-select
            v-model="perfilSelecionado"
            :options="opcoesEmulacao"
            label="Visão de homologação"
            outlined
            dense
            options-dense
            emit-value
            map-options
            bg-color="white"
            class="full-width q-mb-sm drawer-emulacao-select"
          >
            <template #prepend>
              <q-icon name="visibility" size="20px" color="primary" />
            </template>
            <q-tooltip>Simula a experiência de qualquer perfil em ambiente local.</q-tooltip>
          </q-select>
        </div>

        <q-separator />

        <div class="admin-sidebar__nav-wrap">
          <q-list class="admin-sidebar__nav" padding>
            <template v-for="grupo in menuAgrupado" :key="grupo.key">
              <q-item-label v-if="!drawerMini" header class="menu-group-label row items-center q-gutter-xs">
                <q-icon :name="grupo.icon" size="16px" />
                <span>{{ grupo.label }}</span>
              </q-item-label>

              <template v-for="item in grupo.items" :key="item.label">
                <q-item
                  v-if="item.to"
                  clickable
                  class="menu-entry"
                  :active="rotaAtiva(item.to)"
                  active-class="menu-item-active"
                  @click="navegarPara(item.to)"
                >
                  <q-item-section avatar>
                    <q-icon :name="item.icon" />
                  </q-item-section>
                  <q-item-section>{{ item.label }}</q-item-section>
                  <q-tooltip v-if="drawerMini">{{ item.label }}</q-tooltip>
                </q-item>

                <q-expansion-item
                  v-else
                  dense-toggle
                  expand-separator
                  class="menu-entry menu-entry--group"
                  :icon="item.icon"
                  :label="item.label"
                  :default-opened="grupoAberto(item.children)"
                  header-class="menu-group"
                  :disable="drawerMini"
                >
                  <q-list dense>
                    <q-item
                      v-for="child in item.children"
                      :key="child.label"
                      clickable
                      class="menu-entry menu-entry--child"
                      :inset-level="0.5"
                      :active="Boolean(child.to && rotaAtiva(child.to))"
                      active-class="menu-item-active"
                      @click="child.to && navegarPara(child.to)"
                    >
                      <q-item-section avatar>
                        <q-icon :name="child.icon" size="18px" />
                      </q-item-section>
                      <q-item-section>{{ child.label }}</q-item-section>
                    </q-item>
                  </q-list>
                </q-expansion-item>
              </template>
            </template>
          </q-list>
        </div>

        <q-separator />

        <div class="q-pa-md column q-gutter-sm admin-sidebar__footer">
          <q-btn
            flat
            class="text-grey-8"
            :icon="drawerMini ? 'chevron_right' : 'chevron_left'"
            :label="drawerMini ? '' : 'Recolher menu'"
            :aria-label="drawerMini ? 'Expandir menu lateral' : 'Recolher menu lateral'"
            :disable="$q.screen.lt.md"
            @click="drawerMini = !drawerMini"
          >
            <q-tooltip>{{ drawerMini ? 'Expandir menu' : 'Recolher menu' }}</q-tooltip>
          </q-btn>

          <q-btn
            color="negative"
            unelevated
            icon="logout"
            :label="drawerMini ? '' : 'Sair'"
            aria-label="Sair da conta"
            class="full-width drawer-exit-btn"
            @click="logout"
          >
            <q-tooltip>Sair</q-tooltip>
          </q-btn>
        </div>
      </div>
    </q-drawer>

    <q-page-container class="admin-page-container">
      <div v-if="authStore.emulandoPerfil" class="q-pa-sm q-pa-md">
        <q-banner rounded class="bg-amber-2 text-dark emulacao-banner">
          <template #avatar>
            <q-icon name="badge" />
          </template>

          <div class="text-weight-medium">Visualizando como {{ authStore.perfilEmulado }}</div>
          <div class="text-caption">Você está simulando a visão de {{ authStore.perfilEmulado }}.</div>

          <template #action>
            <q-btn
              color="primary"
              flat
              icon="undo"
              label="Voltar para Perfil original"
              :loading="retornoEmulacaoCarregando"
              @click="voltarParaAdministrador"
            />
          </template>
        </q-banner>
      </div>

      <router-view />
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.admin-layout {
  background: linear-gradient(180deg, var(--sgx-page-bg) 0%, var(--sgx-page-bg-alt) 100%);
}

.admin-header {
  background: rgba(253, 254, 255, 0.92);
  backdrop-filter: blur(6px);
  border-bottom: 1px solid var(--sgx-border-soft);
}

.header-toolbar {
  min-width: 0;
  gap: var(--sgx-space-2);
}

.header-title {
  min-width: 0;
}

.header-title__value {
  color: var(--sgx-text);
}

.header-title :deep(.q-toolbar__title) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.header-search {
  width: clamp(220px, 28vw, 430px);
}

.header-search :deep(.q-field__control) {
  background: #ffffff;
}

.user-chip {
  background: #ffffff;
  border: 1px solid var(--sgx-border);
  border-radius: var(--sgx-radius-md);
  box-shadow: var(--sgx-shadow-sm);
  max-width: 260px;
  min-width: 0;
}

.user-chip__name {
  max-width: 140px;
}

.user-chip__perfil {
  max-width: 160px;
}

.status-dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: var(--sgx-success);
}

.admin-drawer {
  background: #f8fbff !important;
  color: #0f172a;
}

:deep(.admin-drawer .q-drawer__content) {
  background: #f8fbff !important;
  color: #0f172a !important;
}

.drawer-brand {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 12px;
  align-items: center;
  padding: 14px;
  margin: 12px;
  border: 1px solid var(--sgx-border);
  border-radius: var(--sgx-radius-md);
  background: linear-gradient(135deg, #ffffff 0%, #eef4ff 100%);
  box-shadow: var(--sgx-shadow-sm);
}

.admin-sidebar {
  min-height: 100%;
  display: flex;
  flex-direction: column;
}

.admin-sidebar__nav-wrap {
  flex: 1 1 auto;
  min-height: 220px;
  overflow-y: auto;
  padding: 10px;
}

.admin-sidebar__nav {
  padding: 0 !important;
}

.menu-group-label {
  padding: 12px 8px 6px;
  margin-top: 6px;
  color: var(--sgx-muted);
  font-size: 0.72rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  font-weight: 700;
}

.drawer-brand__title {
  font-size: 1.05rem;
  line-height: 1;
  font-weight: 800;
  letter-spacing: 0.06em;
  color: var(--sgx-primary);
}

.drawer-brand__name {
  font-size: 0.94rem;
  line-height: 1.2;
  font-weight: 700;
  color: #0f172a;
  display: block;
}

.drawer-brand__subtitle {
  color: var(--sgx-muted);
}

:deep(.menu-entry) {
  margin-bottom: 4px;
  border-radius: var(--sgx-radius-sm);
}

:deep(.menu-entry .q-item) {
  border-radius: var(--sgx-radius-sm);
}

:deep(.menu-entry.q-item),
:deep(.menu-entry .q-item),
:deep(.menu-group) {
  transition: background-color 0.2s ease, color 0.2s ease, transform 0.2s ease;
}

:deep(.menu-item-active),
:deep(.menu-entry .menu-item-active) {
  background: rgba(11, 94, 215, 0.12);
  color: var(--sgx-primary);
  border-radius: var(--sgx-radius-sm);
  box-shadow: inset 3px 0 0 var(--sgx-primary);
}

:deep(.menu-group) {
  border-radius: var(--sgx-radius-sm);
  color: #0f172a;
}

:deep(.q-item) {
  color: #0f172a;
  border-radius: var(--sgx-radius-sm);
}

:deep(.q-item.q-item--clickable:hover),
:deep(.q-expansion-item__container > .q-item.q-item--clickable:hover) {
  background: rgba(11, 94, 215, 0.08);
}

:deep(.q-item__section--avatar .q-icon),
:deep(.q-expansion-item__toggle-icon) {
  color: #1e3a8a;
}

:deep(.menu-item-active .q-icon) {
  color: var(--sgx-primary);
}

:deep(.q-expansion-item__container > .q-item .q-item__label) {
  color: #0f172a;
}

:deep(.q-expansion-item__container > .q-item .q-item__label--caption) {
  color: #64748b;
}

:deep(.q-item__label) {
  color: #0f172a;
}

:deep(.q-separator) {
  background: var(--sgx-border-soft);
}

:deep(.q-drawer--left.q-drawer--bordered) {
  border-right: 1px solid var(--sgx-border-soft);
}

:deep(.q-expansion-item__container .q-item) {
  min-height: 44px;
}

.drawer-emulacao-btn {
  background: var(--sgx-primary);
  color: #ffffff !important;
  border: 1px solid #0a50b8;
  min-height: 40px;
  border-radius: var(--sgx-radius-sm);
}

.emulacao-acoes {
  display: grid;
  gap: 8px;
  padding-top: 2px;
}

.drawer-emulacao-btn :deep(.q-icon) {
  color: #ffffff;
}

.emulacao-banner {
  border: 1px solid rgba(146, 64, 14, 0.18);
}

.drawer-exit-btn {
  background: transparent;
  color: var(--sgx-danger) !important;
  border: 1px solid var(--sgx-danger);
  border-radius: var(--sgx-radius-sm);
  min-height: 40px;
}

.drawer-exit-btn:hover {
  background: rgba(220, 38, 38, 0.08) !important;
}

.admin-page-container {
  background: rgba(244, 247, 251, 0.6);
}

@media (max-width: 1430px) {
  .user-chip {
    max-width: 215px;
  }

  .user-chip__name {
    max-width: 106px;
  }

  .user-chip__perfil {
    max-width: 106px;
  }
}

@media (max-width: 1023px) {
  .user-chip {
    max-width: 180px;
  }

  .user-chip__name {
    max-width: 90px;
  }

  .user-chip__perfil {
    max-width: 90px;
  }
}

@media (max-width: 768px) {
  .header-title :deep(.text-overline) {
    display: none;
  }

  .header-search {
    width: 100%;
  }

  .user-chip {
    padding-right: 6px;
    min-width: 0;
  }

  .user-chip__perfil {
    display: none;
  }

  .drawer-brand {
    margin: 10px;
  }

  .menu-group-label {
    padding-top: 10px;
  }
}
</style>





