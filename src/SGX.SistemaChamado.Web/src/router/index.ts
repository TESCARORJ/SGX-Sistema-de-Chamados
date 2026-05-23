import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import type { PerfilUsuario } from '../types/auth'

const AuthLayout = () => import('../layouts/AuthLayout.vue')
const PortalLayout = () => import('../layouts/PortalLayout.vue')
const AdminLayout = () => import('../layouts/AdminLayout.vue')

const LoginView = () => import('../views/LoginView.vue')
const AlterarSenhaView = () => import('../views/AlterarSenhaView.vue')
const RecuperarSenhaView = () => import('../views/RecuperarSenhaView.vue')
const AcessoNegadoView = () => import('../views/AcessoNegadoView.vue')

const PortalDashboardView = () => import('../views/PortalDashboardView.vue')
const PortalChamadosView = () => import('../views/PortalChamadosView.vue')
const NovoChamadoView = () => import('../views/NovoChamadoView.vue')
const DetalheChamadoView = () => import('../views/DetalheChamadoView.vue')
const BaseConhecimentoPortalPage = () => import('../views/BaseConhecimentoPortalPage.vue')
const BaseConhecimentoArtigoPage = () => import('../views/BaseConhecimentoArtigoPage.vue')

const AdminDashboardView = () => import('../views/AdminDashboardView.vue')
const AdminChamadosView = () => import('../views/AdminChamadosView.vue')
const AdminNotificacoesView = () => import('../views/AdminNotificacoesView.vue')
const AdminDetalheChamadoView = () => import('../views/AdminDetalheChamadoView.vue')
const UsuariosAdminView = () => import('../views/UsuariosAdminView.vue')
const UsuarioAdminDetalheView = () => import('../views/UsuarioAdminDetalheView.vue')
const PerfisAdminView = () => import('../views/PerfisAdminView.vue')
const PerfilAdminDetalheView = () => import('../views/PerfilAdminDetalheView.vue')
const DepartamentosAdminView = () => import('../views/DepartamentosAdminView.vue')
const DepartamentoDetalheView = () => import('../views/DepartamentoDetalheView.vue')
const CategoriasAdminView = () => import('../views/CategoriasAdminView.vue')
const CategoriaDetalheView = () => import('../views/CategoriaDetalheView.vue')
const SubcategoriasAdminView = () => import('../views/SubcategoriasAdminView.vue')
const SubcategoriaDetalheView = () => import('../views/SubcategoriaDetalheView.vue')
const PrioridadesAdminView = () => import('../views/PrioridadesAdminView.vue')
const PrioridadeDetalheView = () => import('../views/PrioridadeDetalheView.vue')
const TiposSolicitacaoAdminView = () => import('../views/TiposSolicitacaoAdminView.vue')
const TipoSolicitacaoDetalheView = () => import('../views/TipoSolicitacaoDetalheView.vue')
const LocaisUnidadesAdminView = () => import('../views/LocaisUnidadesAdminView.vue')
const LocalUnidadeDetalheView = () => import('../views/LocalUnidadeDetalheView.vue')
const StatusAdminView = () => import('../views/StatusAdminView.vue')
const StatusDetalheView = () => import('../views/StatusDetalheView.vue')
const ParametrosSistemaView = () => import('../views/ParametrosSistemaView.vue')
const ParametroSistemaDetalheView = () => import('../views/ParametroSistemaDetalheView.vue')
const IntegracaoEmailLogsView = () => import('../views/IntegracaoEmailLogsView.vue')
const IntegracaoMicrosoftEntraIdView = () => import('../views/IntegracaoMicrosoftEntraIdView.vue')
const RoadmapItsmView = () => import('../views/RoadmapItsmView.vue')
const GestaoItsmDocumentacaoView = () => import('../views/GestaoItsmDocumentacaoView.vue')
const SlaPoliciesAdminView = () => import('../views/SlaPoliciesAdminView.vue')
const SlaAlertasAdminView = () => import('../views/SlaAlertasAdminView.vue')
const SlaDashboardAdminView = () => import('../views/SlaDashboardAdminView.vue')
const SlaCalendariosAdminView = () => import('../views/SlaCalendariosAdminView.vue')
const AuditoriaAdminView = () => import('../views/AuditoriaAdminView.vue')
const BaseConhecimentoListPage = () => import('../views/BaseConhecimentoListPage.vue')
const BaseConhecimentoFormPage = () => import('../views/BaseConhecimentoFormPage.vue')

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    perfisPermitidos?: PerfilUsuario[]
  }
}

const perfisPortal: PerfilUsuario[] = ['Solicitante', 'Atendente', 'Administrador']

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/login',
  },
  {
    path: '/',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        name: 'login',
        component: LoginView,
      },
      {
        path: 'alterar-senha',
        name: 'alterar-senha',
        component: AlterarSenhaView,
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: 'recuperar-senha',
        name: 'recuperar-senha',
        component: RecuperarSenhaView,
      },
      {
        path: 'acesso-negado',
        name: 'acesso-negado',
        component: AcessoNegadoView,
      },
    ],
  },
  {
    path: '/portal',
    component: PortalLayout,
    meta: {
      requiresAuth: true,
      perfisPermitidos: perfisPortal,
    },
    children: [
      {
        path: '',
        name: 'portal-dashboard',
        component: PortalDashboardView,
      },
      {
        path: 'chamados',
        name: 'portal-chamados',
        component: PortalChamadosView,
      },
      {
        path: 'chamados/novo',
        name: 'portal-chamados-novo',
        component: NovoChamadoView,
      },
      {
        path: 'chamados/:id',
        name: 'portal-chamados-detalhe',
        component: DetalheChamadoView,
      },
      {
        path: 'base-conhecimento',
        name: 'portal-base-conhecimento',
        component: BaseConhecimentoPortalPage,
      },
      {
        path: 'base-conhecimento/:slug',
        name: 'portal-base-conhecimento-artigo',
        component: BaseConhecimentoArtigoPage,
      },
    ],
  },
  {
    path: '/admin',
    component: AdminLayout,
    meta: {
      requiresAuth: true,
      perfisPermitidos: ['Administrador', 'Atendente'],
    },
    children: [
      {
        path: '',
        name: 'admin-dashboard',
        component: AdminDashboardView,
      },
      {
        path: 'chamados',
        name: 'admin-chamados',
        component: AdminChamadosView,
      },
      {
        path: 'notificacoes',
        name: 'admin-notificacoes',
        component: AdminNotificacoesView,
      },
      {
        path: 'chamados/:id',
        name: 'admin-chamados-detalhe',
        component: AdminDetalheChamadoView,
      },
      {
        path: 'cadastros/usuarios',
        name: 'admin-cadastros-usuarios',
        component: UsuariosAdminView,
      },
      {
        path: 'cadastros/usuarios/:id',
        name: 'admin-cadastros-usuarios-detalhe',
        component: UsuarioAdminDetalheView,
      },
      {
        path: 'cadastros/perfis',
        name: 'admin-cadastros-perfis',
        component: PerfisAdminView,
      },
      {
        path: 'cadastros/perfis/:id',
        name: 'admin-cadastros-perfis-detalhe',
        component: PerfilAdminDetalheView,
      },
      {
        path: 'cadastros/departamentos',
        name: 'admin-cadastros-departamentos',
        component: DepartamentosAdminView,
      },
      {
        path: 'cadastros/departamentos/:id',
        name: 'admin-cadastros-departamentos-detalhe',
        component: DepartamentoDetalheView,
      },
      {
        path: 'cadastros/categorias',
        name: 'admin-cadastros-categorias',
        component: CategoriasAdminView,
      },
      {
        path: 'cadastros/categorias/:id',
        name: 'admin-cadastros-categorias-detalhe',
        component: CategoriaDetalheView,
      },
      {
        path: 'cadastros/subcategorias',
        name: 'admin-cadastros-subcategorias',
        component: SubcategoriasAdminView,
      },
      {
        path: 'cadastros/subcategorias/:id',
        name: 'admin-cadastros-subcategorias-detalhe',
        component: SubcategoriaDetalheView,
      },
      {
        path: 'cadastros/prioridades',
        name: 'admin-cadastros-prioridades',
        component: PrioridadesAdminView,
      },
      {
        path: 'cadastros/prioridades/:id',
        name: 'admin-cadastros-prioridades-detalhe',
        component: PrioridadeDetalheView,
      },
      {
        path: 'cadastros/tipos-solicitacao',
        name: 'admin-cadastros-tipos-solicitacao',
        component: TiposSolicitacaoAdminView,
      },
      {
        path: 'cadastros/tipos-solicitacao/:id',
        name: 'admin-cadastros-tipos-solicitacao-detalhe',
        component: TipoSolicitacaoDetalheView,
      },
      {
        path: 'cadastros/locais',
        name: 'admin-cadastros-locais',
        component: LocaisUnidadesAdminView,
      },
      {
        path: 'cadastros/locais/:id',
        name: 'admin-cadastros-locais-detalhe',
        component: LocalUnidadeDetalheView,
      },
      {
        path: 'cadastros/status',
        name: 'admin-cadastros-status',
        component: StatusAdminView,
      },
      {
        path: 'cadastros/status/:id',
        name: 'admin-cadastros-status-detalhe',
        component: StatusDetalheView,
      },
      {
        path: 'configuracoes/parametros',
        name: 'admin-configuracoes-parametros',
        component: ParametrosSistemaView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador'],
        },
      },
      {
        path: 'configuracoes/parametros/:id',
        name: 'admin-configuracoes-parametros-detalhe',
        component: ParametroSistemaDetalheView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador'],
        },
      },
      {
        path: 'integracoes/email',
        name: 'admin-integracoes-email',
        component: IntegracaoEmailLogsView,
      },
      {
        path: 'integracoes/microsoft-entra-id',
        name: 'admin-integracoes-microsoft-entra-id',
        component: IntegracaoMicrosoftEntraIdView,
      },
      {
        path: 'sla/policies',
        name: 'admin-sla-policies',
        component: SlaPoliciesAdminView,
      },
      {
        path: 'sla/alertas',
        name: 'admin-sla-alertas',
        component: SlaAlertasAdminView,
      },
      {
        path: 'sla/calendarios',
        name: 'admin-sla-calendarios',
        component: SlaCalendariosAdminView,
      },
      {
        path: 'sla/painel',
        name: 'admin-sla-painel',
        component: SlaDashboardAdminView,
      },
      {
        path: 'roadmap-itsm',
        name: 'admin-roadmap-itsm',
        component: RoadmapItsmView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador', 'Atendente'],
        },
      },
      {
        path: 'gestao-itsm/roadmap',
        name: 'admin-gestao-itsm-roadmap',
        component: RoadmapItsmView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador', 'Atendente'],
        },
      },
      {
        path: 'gestao-itsm/documentacao',
        name: 'admin-gestao-itsm-documentacao',
        component: GestaoItsmDocumentacaoView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador', 'Atendente'],
        },
      },
      {
        path: 'governanca/auditoria',
        name: 'admin-governanca-auditoria',
        component: AuditoriaAdminView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: ['Administrador', 'Atendente'],
        },
      },
      {
        path: 'conhecimento/base-conhecimento',
        name: 'admin-conhecimento-base-conhecimento',
        component: BaseConhecimentoListPage,
      },
      {
        path: 'conhecimento/base-conhecimento/:id',
        name: 'admin-conhecimento-base-conhecimento-detalhe',
        component: BaseConhecimentoFormPage,
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach(async (to) => {
  const authStore = useAuthStore()
  if (!to.meta.requiresAuth && to.path !== '/login' && to.path !== '/alterar-senha' && to.path !== '/recuperar-senha') {
    return true
  }

  const autenticado = await authStore.inicializarSessao()

  if (to.path === '/login' && autenticado) {
    if (authStore.deveAlterarSenha) {
      return '/alterar-senha'
    }

    return authStore.rotaInicial
  }

  if (to.path === '/alterar-senha' && !autenticado) {
    return '/login'
  }

  if (to.path === '/alterar-senha' && autenticado && !authStore.deveAlterarSenha) {
    return authStore.rotaInicial
  }

  if (to.path === '/recuperar-senha' && autenticado && authStore.deveAlterarSenha) {
    return '/alterar-senha'
  }

  if (to.meta.requiresAuth && !autenticado) {
    return '/login'
  }

  if (to.path !== '/alterar-senha' && to.path !== '/login' && autenticado && authStore.deveAlterarSenha) {
    return '/alterar-senha'
  }

  const perfisPermitidos = to.meta.perfisPermitidos
  if (to.meta.requiresAuth && perfisPermitidos?.length) {
    const perfisUsuario = authStore.usuario?.perfis ?? []
    const permitido = perfisPermitidos.some((perfil) => perfisUsuario.includes(perfil))
    if (!permitido) {
      return '/acesso-negado'
    }
  }

  return true
})

export default router
