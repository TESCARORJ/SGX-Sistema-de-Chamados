<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import NotificationsMenu from '../components/ui/NotificationsMenu.vue'
import { permissoes } from '../constants/permissoes'

type MenuItem = {
  label: string
  icon: string
  to?: string
  children?: MenuItem[]
  adminOnly?: boolean
  requiredAnyPermissions?: string[]
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

const menu: MenuItem[] = [
  {
    label: 'Dashboard',
    icon: 'space_dashboard',
    to: '/admin',
    requiredAnyPermissions: [permissoes.dashboardVisualizar],
  },
  {
    label: 'Chamados',
    icon: 'support_agent',
    to: '/admin/chamados',
    requiredAnyPermissions: [permissoes.chamadosVisualizar, permissoes.chamadosVisualizarTodos],
  },
  {
    label: 'SLA',
    icon: 'schedule',
    requiredAnyPermissions: [
      permissoes.slaVisualizar,
      permissoes.slaCriar,
      permissoes.slaEditar,
      permissoes.slaAtivarDesativar,
    ],
    children: [
      {
        label: 'PolÃ­ticas',
        icon: 'rule',
        to: '/admin/sla/policies',
        requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaCriar, permissoes.slaEditar],
      },
      {
        label: 'Alertas',
        icon: 'notifications_active',
        to: '/admin/sla/alertas',
        requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
      },
      {
        label: 'CalendÃ¡rios',
        icon: 'event_available',
        to: '/admin/sla/calendarios',
        requiredAnyPermissions: [permissoes.slaVisualizar, permissoes.slaEditar],
      },
      {
        label: 'Painel',
        icon: 'monitoring',
        to: '/admin/sla/painel',
        requiredAnyPermissions: [permissoes.slaVisualizar],
      },
    ],
  },
  {
    label: 'NotificaÃ§Ãµes',
    icon: 'notifications_active',
    to: '/admin/notificacoes',
    requiredAnyPermissions: [permissoes.notificacoesVisualizar],
  },
  {
    label: 'Cadastros',
    icon: 'dataset',
    requiredAnyPermissions: [
      permissoes.cadastrosVisualizar,
      permissoes.usuariosVisualizar,
      permissoes.usuariosGerenciar,
      permissoes.perfisVisualizar,
      permissoes.perfisGerenciar,
    ],
    children: [
      {
        label: 'UsuÃ¡rios',
        icon: 'group',
        to: '/admin/cadastros/usuarios',
        requiredAnyPermissions: [permissoes.usuariosVisualizar, permissoes.usuariosGerenciar],
      },
      {
        label: 'Perfis',
        icon: 'badge',
        to: '/admin/cadastros/perfis',
        requiredAnyPermissions: [permissoes.perfisVisualizar, permissoes.perfisGerenciar],
      },
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
        label: 'Tipos de Solicitacao',
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
    ],
  },
  {
    label: 'Conhecimento',
    icon: 'menu_book',
    requiredAnyPermissions: [
      permissoes.baseConhecimentoVisualizar,
      permissoes.baseConhecimentoGerenciar,
      permissoes.baseConhecimentoPublicar,
      permissoes.baseConhecimentoArquivar,
    ],
    children: [
      {
        label: 'Base de conhecimento',
        icon: 'article',
        to: '/admin/conhecimento/base-conhecimento',
        requiredAnyPermissions: [
          permissoes.baseConhecimentoVisualizar,
          permissoes.baseConhecimentoGerenciar,
          permissoes.baseConhecimentoPublicar,
          permissoes.baseConhecimentoArquivar,
        ],
      },
    ],
  },
  {
    label: 'ConfiguraÃ§Ãµes',
    icon: 'settings',
    requiredAnyPermissions: [permissoes.parametrosVisualizar, permissoes.parametrosGerenciar],
    children: [
      {
        label: 'ParÃ¢metros do Sistema',
        icon: 'tune',
        to: '/admin/configuracoes/parametros',
        requiredAnyPermissions: [permissoes.parametrosVisualizar, permissoes.parametrosGerenciar],
      },
    ],
  },
  {
    label: 'IntegraÃ§Ãµes',
    icon: 'hub',
    requiredAnyPermissions: [
      permissoes.integracoesEmailVisualizar,
      permissoes.integracoesMicrosoftVisualizar,
      permissoes.integracoesMicrosoftGerenciar,
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
        requiredAnyPermissions: [permissoes.integracoesMicrosoftVisualizar, permissoes.integracoesMicrosoftGerenciar],
      },
    ],
  },
  {
    label: 'GestÃ£o ITSM',
    icon: 'insights',
    requiredAnyPermissions: [permissoes.roadmapVisualizar, permissoes.roadmapGerenciar],
    children: [
      {
        label: 'Roadmap',
        icon: 'account_tree',
        to: '/admin/gestao-itsm/roadmap',
        requiredAnyPermissions: [permissoes.roadmapVisualizar, permissoes.roadmapGerenciar],
      },
      {
        label: 'DocumentaÃ§Ã£o',
        icon: 'library_books',
        to: '/admin/gestao-itsm/documentacao',
        requiredAnyPermissions: [permissoes.roadmapVisualizar, permissoes.roadmapGerenciar],
      },
    ],
  },
  {
    label: 'GovernanÃ§a',
    icon: 'fact_check',
    requiredAnyPermissions: [permissoes.auditoriaVisualizar, permissoes.auditoriaGerenciar],
    children: [
      {
        label: 'Auditoria',
        icon: 'manage_search',
        to: '/admin/governanca/auditoria',
        requiredAnyPermissions: [permissoes.auditoriaVisualizar, permissoes.auditoriaGerenciar],
      },
    ],
  },
]

const usuarioNome = computed(() => authStore.usuario?.nome || 'Administrador SGX')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const quantidadePermissoes = computed(() => authStore.usuario?.permissoes?.length ?? 0)
const fallbackAdminSemPermissoes = computed(() => usuarioEhAdministrador.value && quantidadePermissoes.value === 0)
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
  menu
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
const podeVisualizarNotificacoes = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.notificacoesVisualizar)
)
const statusUsuario = computed(() => (authStore.autenticado ? 'Online' : 'Offline'))
const tituloPagina = computed(() => {
  if (route.path === '/admin') return 'Dashboard'
  if (route.path === '/admin/chamados') return 'Fila de Chamados'
  if (route.path === '/admin/notificacoes') return 'Central de NotificaÃƒÂ§ÃƒÂµes'
  if (route.path.startsWith('/admin/sla/alertas')) return 'Alertas de SLA'
  if (route.path.startsWith('/admin/sla/calendarios')) return 'CalendÃ¡rios de SLA'
  if (route.path.startsWith('/admin/sla/painel')) return 'Painel de SLA'
  if (route.path.startsWith('/admin/sla')) return 'PolÃ­ticas de SLA'
  if (route.path.startsWith('/admin/chamados/')) return 'Detalhe do Chamado'
  if (route.path.startsWith('/admin/cadastros')) return 'Cadastros'
  if (route.path.startsWith('/admin/conhecimento/base-conhecimento')) return 'Base de conhecimento'
  if (route.path.startsWith('/admin/configuracoes')) return 'ConfiguraÃƒÂ§ÃƒÂµes'
  if (route.path.startsWith('/admin/integracoes')) return 'IntegraÃƒÂ§ÃƒÂµes'
  if (route.path.startsWith('/admin/governanca/auditoria')) return 'HistÃ³rico / Auditoria'
  if (route.path.startsWith('/admin/gestao-itsm/documentacao')) return 'DocumentaÃ§Ã£o ITSM'
  if (route.path.startsWith('/admin/gestao-itsm')) return 'GestÃ£o ITSM'
  if (route.path.startsWith('/admin/roadmap-itsm')) return 'Roadmap ITSM'
  return 'ÃƒÂrea Administrativa'
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

async function visualizarComoSolicitante(): Promise<void> {
  if (!emulacaoDisponivel.value || emulacaoSolicitanteCarregando.value || emulacaoAtendenteCarregando.value) {
    return
  }

  emulacaoSolicitanteCarregando.value = true

  try {
    await authStore.iniciarEmulacaoSolicitante()
    await router.replace('/portal')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'NÃƒÂ£o foi possÃƒÂ­vel concluir a aÃƒÂ§ÃƒÂ£o.'
    $q.notify({
      type: 'negative',
      message,
    })
  } finally {
    emulacaoSolicitanteCarregando.value = false
  }
}

async function visualizarComoAtendente(): Promise<void> {
  if (!emulacaoDisponivel.value || emulacaoAtendenteCarregando.value || emulacaoSolicitanteCarregando.value) {
    return
  }

  emulacaoAtendenteCarregando.value = true

  try {
    await authStore.iniciarEmulacaoAtendente()
    await router.replace('/admin/chamados')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'NÃƒÂ£o foi possÃƒÂ­vel concluir a aÃƒÂ§ÃƒÂ£o.'
    $q.notify({
      type: 'negative',
      message,
    })
  } finally {
    emulacaoAtendenteCarregando.value = false
  }
}

async function voltarParaAdministrador(): Promise<void> {
  if (!emulandoAtendente.value || retornoEmulacaoCarregando.value) {
    return
  }

  retornoEmulacaoCarregando.value = true

  try {
    await authStore.encerrarEmulacao()
    await router.replace('/admin')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'NÃƒÂ£o foi possÃƒÂ­vel concluir a aÃƒÂ§ÃƒÂ£o.'
    $q.notify({
      type: 'negative',
      message,
    })

    if (
      message.includes('Contexto original da emulacao nao encontrado') ||
      message.includes('Contexto original da emulaÃƒÂ§ÃƒÂ£o nÃƒÂ£o encontrado')
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
          <q-toolbar-title class="text-subtitle1 text-weight-bold">
            {{ tituloPagina }}
          </q-toolbar-title>
        </div>

        <q-space />

        <q-input
          v-model="buscaGlobal"
          class="header-search gt-sm"
          dense
          outlined
          placeholder="Buscar chamados, solicitantes, categorias..."
          @keyup.enter="acionarBuscaGlobal"
        >
          <template #prepend>
            <q-icon name="search" />
          </template>
          <template #append>
            <q-btn flat round dense icon="arrow_forward" @click="acionarBuscaGlobal" />
          </template>
        </q-input>

        <NotificationsMenu v-if="podeVisualizarNotificacoes" />

        <q-chip square class="q-ml-sm user-chip">
          <q-avatar color="primary" text-color="white">{{ iniciaisUsuario }}</q-avatar>
          <div class="column">
            <div class="text-caption text-weight-medium ellipsis user-chip__name">{{ usuarioNome }}</div>
            <div class="row items-center q-gutter-xs">
              <span class="status-dot" />
              <span class="text-caption text-grey-7">{{ statusUsuario }}</span>
            </div>
          </div>
          <q-tooltip>{{ usuarioEmail }}</q-tooltip>
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
          <div>
            <div class="drawer-brand__title">SGX</div>
            <div class="drawer-brand__name">Sistema de Chamados</div>
            <div class="text-caption drawer-brand__subtitle">Painel administrativo</div>
          </div>
        </div>

        <q-separator />

        <div v-if="emulacaoDisponivel" class="q-px-md q-py-sm emulacao-acoes">
          <q-btn
            color="primary"
            unelevated
            icon="visibility"
            :label="drawerMini ? '' : 'Visualizar como Solicitante'"
            class="full-width drawer-emulacao-btn"
            :loading="emulacaoSolicitanteCarregando"
            @click="visualizarComoSolicitante"
          >
            <q-tooltip>Simula a experiÃƒÂªncia do Solicitante em ambiente local.</q-tooltip>
          </q-btn>

          <q-btn
            color="primary"
            unelevated
            icon="support_agent"
            :label="drawerMini ? '' : 'Visualizar como Atendente'"
            class="full-width drawer-emulacao-btn"
            :loading="emulacaoAtendenteCarregando"
            @click="visualizarComoAtendente"
          >
            <q-tooltip>Simula a experiÃƒÂªncia do Atendente em ambiente local.</q-tooltip>
          </q-btn>
        </div>

        <q-separator />

        <div class="admin-sidebar__nav-wrap">
          <q-list class="admin-sidebar__nav" padding>
            <template v-for="item in menuVisivel" :key="item.label">
              <q-item
                v-if="item.to"
                clickable
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
                    inset-level="0.5"
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
          </q-list>
        </div>

        <q-separator />

        <div class="q-pa-md column q-gutter-sm admin-sidebar__footer">
          <q-btn
            flat
            class="text-grey-8"
            :icon="drawerMini ? 'chevron_right' : 'chevron_left'"
            :label="drawerMini ? '' : 'Recolher menu'"
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
            class="full-width drawer-exit-btn"
            @click="logout"
          >
            <q-tooltip>Sair</q-tooltip>
          </q-btn>
        </div>
      </div>
    </q-drawer>

    <q-page-container>
      <div v-if="emulandoAtendente" class="q-pa-sm q-pa-md">
        <q-banner rounded class="bg-amber-2 text-dark emulacao-banner">
          <template #avatar>
            <q-icon name="badge" />
          </template>

          <div class="text-weight-medium">Visualizando como Atendente Demo</div>
          <div class="text-caption">VocÃƒÂª estÃƒÂ¡ visualizando como Atendente Demo.</div>

          <template #action>
            <q-btn
              color="primary"
              flat
              icon="undo"
              label="Voltar para Administrador"
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
  background: linear-gradient(180deg, var(--sgx-page-bg) 0%, #eaf0f9 100%);
}

.admin-header {
  background: #fdfefe;
  border-bottom: 1px solid #dde4f0;
}

.header-toolbar {
  min-width: 0;
}

.header-title {
  min-width: 0;
}

.header-title :deep(.q-toolbar__title) {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.header-search {
  width: clamp(220px, 28vw, 430px);
}

.user-chip {
  background: #f1f5f9;
  max-width: 260px;
  min-width: 0;
}

.user-chip__name {
  max-width: 140px;
}

.status-dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  background: #22c55e;
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
  padding: 8px 10px;
}

.admin-sidebar__nav {
  padding: 0 !important;
}

.drawer-brand__title {
  font-size: 1.05rem;
  line-height: 1;
  font-weight: 800;
  letter-spacing: 0.06em;
  color: #0b5ed7;
}

.drawer-brand__name {
  font-size: 0.94rem;
  line-height: 1.2;
  font-weight: 700;
  color: #0f172a;
  display: block;
}

.drawer-brand__subtitle {
  color: #64748b;
}

:deep(.menu-item-active) {
  background: rgba(11, 94, 215, 0.12);
  color: #0b5ed7;
  border-radius: 10px;
  box-shadow: none;
}

:deep(.menu-group) {
  border-radius: 10px;
  color: #0f172a;
}

:deep(.q-item) {
  color: #0f172a;
  border-radius: 10px;
}

:deep(.q-item.q-item--clickable:hover),
:deep(.q-expansion-item__container > .q-item.q-item--clickable:hover) {
  background: rgba(11, 94, 215, 0.08);
}

:deep(.q-item__section--avatar .q-icon),
:deep(.q-expansion-item__toggle-icon) {
  color: #0b2f66;
}

:deep(.menu-item-active .q-icon) {
  color: #0b5ed7;
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
  background: #dbe4f1;
}

:deep(.q-drawer--left.q-drawer--bordered) {
  border-right: 1px solid #dbe4f1;
}

:deep(.q-expansion-item__container .q-item) {
  min-height: 44px;
}

.drawer-emulacao-btn {
  background: #1f63d1;
  color: #ffffff !important;
  border: 1px solid #1a55b2;
  min-height: 40px;
  border-radius: 10px;
}

.emulacao-acoes {
  display: grid;
  gap: 8px;
}

.drawer-emulacao-btn :deep(.q-icon) {
  color: #ffffff;
}

.emulacao-banner {
  border: 1px solid rgba(146, 64, 14, 0.18);
}

.drawer-exit-btn {
  background: transparent;
  color: #dc2626 !important;
  border: 1px solid #dc2626;
  border-radius: 10px;
  min-height: 40px;
}

.drawer-exit-btn:hover {
  background: rgba(220, 38, 38, 0.08) !important;
}

@media (max-width: 1430px) {
  .user-chip {
    max-width: 215px;
  }

  .user-chip__name {
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
}

@media (max-width: 768px) {
  .header-title :deep(.text-overline) {
    display: none;
  }

  .user-chip {
    padding-right: 6px;
  }
}
</style>





