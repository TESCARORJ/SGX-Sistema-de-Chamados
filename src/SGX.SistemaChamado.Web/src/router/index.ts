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
const CatalogoServicosPortalPage = () => import('../views/CatalogoServicosPortalPage.vue')
const CatalogoServicoDetalhePage = () => import('../views/CatalogoServicoDetalhePage.vue')

const AdminDashboardView = () => import('../views/AdminDashboardView.vue')
const AdminChamadosView = () => import('../views/AdminChamadosView.vue')
const AdminNotificacoesView = () => import('../views/AdminNotificacoesView.vue')
const AdminDetalheChamadoView = () => import('../views/AdminDetalheChamadoView.vue')
const AprovacaoChamadosListPage = () => import('../views/AprovacaoChamadosListPage.vue')
const AprovacaoChamadosDetalhePage = () => import('../views/AprovacaoChamadosDetalhePage.vue')
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
const ActiveDirectoryIntegracaoAdminView = () => import('../views/ActiveDirectoryIntegracaoAdminView.vue')
const MetodosLoginAdminView = () => import('../views/MetodosLoginAdminView.vue')
const RoadmapItsmView = () => import('../views/RoadmapItsmView.vue')
const GestaoItsmDocumentacaoView = () => import('../views/GestaoItsmDocumentacaoView.vue')
const SlaPoliciesAdminView = () => import('../views/SlaPoliciesAdminView.vue')
const SlaAlertasAdminView = () => import('../views/SlaAlertasAdminView.vue')
const SlaDashboardAdminView = () => import('../views/SlaDashboardAdminView.vue')
const SlaCalendariosAdminView = () => import('../views/SlaCalendariosAdminView.vue')
const AuditoriaAdminView = () => import('../views/AuditoriaAdminView.vue')
const AuditoriaAutenticacaoAdminView = () => import('../views/AuditoriaAutenticacaoAdminView.vue')
const BaseConhecimentoListPage = () => import('../views/BaseConhecimentoListPage.vue')
const BaseConhecimentoFormPage = () => import('../views/BaseConhecimentoFormPage.vue')
const CatalogoServicosListPage = () => import('../views/CatalogoServicosListPage.vue')
const CatalogoServicosFormPage = () => import('../views/CatalogoServicosFormPage.vue')
const InventarioAtivosListPage = () => import('../views/InventarioAtivosListPage.vue')
const InventarioAtivosFormPage = () => import('../views/InventarioAtivosFormPage.vue')
const InventarioAtivosDetalhePage = () => import('../views/InventarioAtivosDetalhePage.vue')
const RelatoriosAvancadosDashboardPage = () => import('../views/RelatoriosAvancadosDashboardPage.vue')
const RelatoriosChamadosPage = () => import('../views/RelatoriosChamadosPage.vue')
const RelatoriosSlaPage = () => import('../views/RelatoriosSlaPage.vue')
const RelatoriosAprovacoesPage = () => import('../views/RelatoriosAprovacoesPage.vue')
const RelatoriosCatalogoServicosPage = () => import('../views/RelatoriosCatalogoServicosPage.vue')
const RelatoriosInventarioAtivosPage = () => import('../views/RelatoriosInventarioAtivosPage.vue')
const RelatoriosBaseConhecimentoPage = () => import('../views/RelatoriosBaseConhecimentoPage.vue')
const RelatoriosAuditoriaPage = () => import('../views/RelatoriosAuditoriaPage.vue')

declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    perfisPermitidos?: PerfilUsuario[]
    requiredAnyPermissions?: string[]
  }
}

const perfisPortal: PerfilUsuario[] = [
  'Solicitante',
  'Atendente',
  'Administrador',
  'Atendente N1',
  'Técnico N2',
  'Coordenador Service Desk',
  'Gestor TI',
  'Auditor Governança'
]

const perfisAdministrativos: PerfilUsuario[] = [
  'Administrador',
  'Atendente',
  'Atendente N1',
  'Técnico N2',
  'Coordenador Service Desk',
  'Gestor TI',
  'Auditor Governança'
]

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
      {
        path: 'catalogo-servicos',
        name: 'portal-catalogo-servicos',
        component: CatalogoServicosPortalPage,
      },
      {
        path: 'catalogo-servicos/:slug',
        name: 'portal-catalogo-servicos-detalhe',
        component: CatalogoServicoDetalhePage,
      },
    ],
  },
  {
    path: '/admin',
    component: AdminLayout,
    meta: {
      requiresAuth: true,
      perfisPermitidos: perfisAdministrativos,
    },
    children: [
      {
        path: '',
        name: 'admin-dashboard',
        component: AdminDashboardView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Dashboard.Visualizar', 'RelatoriosAvancados.Gerencial', 'RelatoriosAvancados.Operacional'],
        },
      },
      {
        path: 'chamados',
        name: 'admin-chamados',
        component: AdminChamadosView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Chamados.Visualizar', 'Chamados.VisualizarTodos'],
        },
      },
      {
        path: 'notificacoes',
        name: 'admin-notificacoes',
        component: AdminNotificacoesView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Notificacoes.Visualizar'],
        },
      },
      {
        path: 'chamados/:id',
        name: 'admin-chamados-detalhe',
        component: AdminDetalheChamadoView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Chamados.Visualizar', 'Chamados.VisualizarTodos'],
        },
      },
      {
        path: 'atendimento/aprovacao-chamados',
        name: 'admin-atendimento-aprovacao-chamados',
        component: AprovacaoChamadosListPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['AprovacaoChamados.Visualizar', 'AprovacaoChamados.Gerenciar'],
        },
      },
      {
        path: 'atendimento/aprovacao-chamados/:id',
        name: 'admin-atendimento-aprovacao-chamados-detalhe',
        component: AprovacaoChamadosDetalhePage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['AprovacaoChamados.Visualizar', 'AprovacaoChamados.Gerenciar'],
        },
      },
      {
        path: 'cadastros/usuarios',
        name: 'admin-cadastros-usuarios',
        component: UsuariosAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Usuarios.Visualizar', 'Usuarios.Gerenciar'],
        },
      },
      {
        path: 'cadastros/usuarios/:id',
        name: 'admin-cadastros-usuarios-detalhe',
        component: UsuarioAdminDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Usuarios.Visualizar', 'Usuarios.Gerenciar'],
        },
      },
      {
        path: 'cadastros/perfis',
        name: 'admin-cadastros-perfis',
        component: PerfisAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Perfis.Visualizar', 'Perfis.Gerenciar'],
        },
      },
      {
        path: 'cadastros/perfis/:id',
        name: 'admin-cadastros-perfis-detalhe',
        component: PerfilAdminDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Perfis.Visualizar', 'Perfis.Gerenciar'],
        },
      },
      {
        path: 'cadastros/departamentos',
        name: 'admin-cadastros-departamentos',
        component: DepartamentosAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/departamentos/:id',
        name: 'admin-cadastros-departamentos-detalhe',
        component: DepartamentoDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/categorias',
        name: 'admin-cadastros-categorias',
        component: CategoriasAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/categorias/:id',
        name: 'admin-cadastros-categorias-detalhe',
        component: CategoriaDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/subcategorias',
        name: 'admin-cadastros-subcategorias',
        component: SubcategoriasAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/subcategorias/:id',
        name: 'admin-cadastros-subcategorias-detalhe',
        component: SubcategoriaDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/prioridades',
        name: 'admin-cadastros-prioridades',
        component: PrioridadesAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/prioridades/:id',
        name: 'admin-cadastros-prioridades-detalhe',
        component: PrioridadeDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/tipos-solicitacao',
        name: 'admin-cadastros-tipos-solicitacao',
        component: TiposSolicitacaoAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/tipos-solicitacao/:id',
        name: 'admin-cadastros-tipos-solicitacao-detalhe',
        component: TipoSolicitacaoDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/locais',
        name: 'admin-cadastros-locais',
        component: LocaisUnidadesAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/locais/:id',
        name: 'admin-cadastros-locais-detalhe',
        component: LocalUnidadeDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/status',
        name: 'admin-cadastros-status',
        component: StatusAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'cadastros/status/:id',
        name: 'admin-cadastros-status-detalhe',
        component: StatusDetalheView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Cadastros.Visualizar'],
        },
      },
      {
        path: 'configuracoes/parametros',
        name: 'admin-configuracoes-parametros',
        component: ParametrosSistemaView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Parametros.Visualizar', 'Parametros.Gerenciar'],
        },
      },
      {
        path: 'configuracoes/parametros/:id',
        name: 'admin-configuracoes-parametros-detalhe',
        component: ParametroSistemaDetalheView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Parametros.Visualizar', 'Parametros.Gerenciar'],
        },
      },
      {
        path: 'integracoes/email',
        name: 'admin-integracoes-email',
        component: IntegracaoEmailLogsView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['IntegracoesEmail.Visualizar'],
        },
      },
      {
        path: 'integracoes/microsoft-entra-id',
        name: 'admin-integracoes-microsoft-entra-id',
        component: IntegracaoMicrosoftEntraIdView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['IntegracoesMicrosoft.Visualizar', 'IntegracoesMicrosoft.Gerenciar'],
        },
      },
      {
        path: 'integracoes/active-directory',
        name: 'admin-integracoes-active-directory',
        component: ActiveDirectoryIntegracaoAdminView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['IntegracoesActiveDirectory.Visualizar', 'IntegracoesActiveDirectory.Gerenciar'],
        },
      },
      {
        path: 'integracoes/metodos-login',
        name: 'admin-integracoes-metodos-login',
        component: MetodosLoginAdminView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['AutenticacaoProvedores.Visualizar', 'AutenticacaoProvedores.Gerenciar'],
        },
      },
      {
        path: 'integracoes/autenticacao/metodos-login',
        redirect: '/admin/integracoes/metodos-login',
      },
      {
        path: 'sla/policies',
        name: 'admin-sla-policies',
        component: SlaPoliciesAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Sla.Visualizar', 'Sla.Criar', 'Sla.Editar', 'Sla.Excluir', 'Sla.AtivarDesativar'],
        },
      },
      {
        path: 'sla/alertas',
        name: 'admin-sla-alertas',
        component: SlaAlertasAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Sla.Visualizar', 'Sla.Editar'],
        },
      },
      {
        path: 'sla/calendarios',
        name: 'admin-sla-calendarios',
        component: SlaCalendariosAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Sla.Visualizar', 'Sla.Editar'],
        },
      },
      {
        path: 'sla/painel',
        name: 'admin-sla-painel',
        component: SlaDashboardAdminView,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['Sla.Visualizar'],
        },
      },
      {
        path: 'roadmap-itsm',
        name: 'admin-roadmap-itsm',
        component: RoadmapItsmView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Roadmap.Visualizar', 'Roadmap.Gerenciar'],
        },
      },
      {
        path: 'gestao-itsm/roadmap',
        name: 'admin-gestao-itsm-roadmap',
        component: RoadmapItsmView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Roadmap.Visualizar', 'Roadmap.Gerenciar'],
        },
      },
      {
        path: 'gestao-itsm/documentacao',
        name: 'admin-gestao-itsm-documentacao',
        component: GestaoItsmDocumentacaoView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Roadmap.Visualizar'],
        },
      },
      {
        path: 'governanca/auditoria',
        name: 'admin-governanca-auditoria',
        component: AuditoriaAdminView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['Auditoria.Visualizar', 'Auditoria.Gerenciar'],
        },
      },
      {
        path: 'governanca/auditoria-autenticacao',
        name: 'admin-governanca-auditoria-autenticacao',
        component: AuditoriaAutenticacaoAdminView,
        meta: {
          requiresAuth: true,
          perfisPermitidos: perfisAdministrativos,
          requiredAnyPermissions: ['AuditoriaAutenticacao.Visualizar'],
        },
      },
      {
        path: 'conhecimento/base-conhecimento',
        name: 'admin-conhecimento-base-conhecimento',
        component: BaseConhecimentoListPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['BaseConhecimento.Visualizar'],
        },
      },
      {
        path: 'conhecimento/base-conhecimento/:id',
        name: 'admin-conhecimento-base-conhecimento-detalhe',
        component: BaseConhecimentoFormPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['BaseConhecimento.Visualizar', 'BaseConhecimento.Gerenciar'],
        },
      },
      {
        path: 'conhecimento/catalogo-servicos',
        name: 'admin-conhecimento-catalogo-servicos',
        component: CatalogoServicosListPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['CatalogoServicos.Visualizar'],
        },
      },
      {
        path: 'conhecimento/catalogo-servicos/novo',
        name: 'admin-conhecimento-catalogo-servicos-novo',
        component: CatalogoServicosFormPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['CatalogoServicos.Gerenciar'],
        },
      },
      {
        path: 'conhecimento/catalogo-servicos/:id',
        name: 'admin-conhecimento-catalogo-servicos-detalhe',
        component: CatalogoServicosFormPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['CatalogoServicos.Visualizar', 'CatalogoServicos.Gerenciar'],
        },
      },
      {
        path: 'infraestrutura/inventario-ativos',
        name: 'admin-infraestrutura-inventario-ativos',
        component: InventarioAtivosListPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['InventarioAtivos.Visualizar'],
        },
      },
      {
        path: 'infraestrutura/inventario-ativos/novo',
        name: 'admin-infraestrutura-inventario-ativos-novo',
        component: InventarioAtivosFormPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['InventarioAtivos.Gerenciar'],
        },
      },
      {
        path: 'infraestrutura/inventario-ativos/:id',
        name: 'admin-infraestrutura-inventario-ativos-detalhe',
        component: InventarioAtivosDetalhePage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['InventarioAtivos.Visualizar'],
        },
      },
      {
        path: 'infraestrutura/inventario-ativos/:id/editar',
        name: 'admin-infraestrutura-inventario-ativos-editar',
        component: InventarioAtivosFormPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['InventarioAtivos.Gerenciar'],
        },
      },
      {
        path: 'relatorios/avancados',
        name: 'admin-relatorios-avancados-dashboard',
        component: RelatoriosAvancadosDashboardPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar', 'RelatoriosAvancados.Gerencial'],
        },
      },
      {
        path: 'relatorios/chamados',
        name: 'admin-relatorios-chamados',
        component: RelatoriosChamadosPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar', 'RelatoriosAvancados.Operacional'],
        },
      },
      {
        path: 'relatorios/sla',
        name: 'admin-relatorios-sla',
        component: RelatoriosSlaPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar'],
        },
      },
      {
        path: 'relatorios/aprovacoes',
        name: 'admin-relatorios-aprovacoes',
        component: RelatoriosAprovacoesPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar'],
        },
      },
      {
        path: 'relatorios/catalogo-servicos',
        name: 'admin-relatorios-catalogo-servicos',
        component: RelatoriosCatalogoServicosPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar'],
        },
      },
      {
        path: 'relatorios/inventario-ativos',
        name: 'admin-relatorios-inventario-ativos',
        component: RelatoriosInventarioAtivosPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar'],
        },
      },
      {
        path: 'relatorios/base-conhecimento',
        name: 'admin-relatorios-base-conhecimento',
        component: RelatoriosBaseConhecimentoPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar'],
        },
      },
      {
        path: 'relatorios/auditoria',
        name: 'admin-relatorios-auditoria',
        component: RelatoriosAuditoriaPage,
        meta: {
          requiresAuth: true,
          requiredAnyPermissions: ['RelatoriosAvancados.Visualizar', 'RelatoriosAvancados.Auditoria'],
        },
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

  const requiredAnyPermissions = to.meta.requiredAnyPermissions
  if (to.meta.requiresAuth && requiredAnyPermissions?.length) {
    const usuarioEhAdministrador = (authStore.usuario?.perfis ?? []).includes('Administrador')
    const fallbackAdminSemPermissoes = usuarioEhAdministrador && (authStore.usuario?.permissoes?.length ?? 0) === 0
    const permitido = fallbackAdminSemPermissoes || authStore.possuiAlgumaPermissao(requiredAnyPermissions)
    if (!permitido) {
      return '/acesso-negado'
    }
  }

  return true
})

export default router
