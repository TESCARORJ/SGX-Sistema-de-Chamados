<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import NotificacoesBadge from '../components/notificacoes/NotificacoesBadge.vue'

type PortalMenuItem = {
  label: string
  icon: string
  to: string
  destaque?: boolean
}

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const drawerOpen = ref(!$q.screen.lt.md)
const retornoEmulacaoCarregando = ref(false)

const menuNavegacao: PortalMenuItem[] = [
  { label: 'Meus chamados', icon: 'receipt_long', to: '/portal/chamados' },
  { label: 'Abrir chamado', icon: 'add_circle', to: '/portal/chamados/novo', destaque: true },
  { label: 'Notificações', icon: 'notifications', to: '/portal/notificacoes' },
  { label: 'Base de conhecimento', icon: 'menu_book', to: '/portal/base-conhecimento' },
  { label: 'Minha conta', icon: 'person', to: '/alterar-senha' },
]

const atalhoHeader = computed(() => menuNavegacao.filter((item) => item.to !== '/portal'))
const usuarioNome = computed(() => authStore.usuario?.nome || 'Solicitante')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')
const emulandoSolicitante = computed(() => authStore.emulandoPerfil && authStore.perfilEmulado === 'Solicitante')
const iniciaisUsuario = computed(() => {
  const nome = usuarioNome.value.trim()
  if (!nome) return 'S'

  return nome
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((parte) => parte[0]?.toUpperCase() || '')
    .join('')
})

function rotaAtiva(path: string): boolean {
  if (path === '/portal') {
    return route.path === '/portal'
  }

  return route.path === path || route.path.startsWith(`${path}/`)
}

function navegarPara(path: string): void {
  if (route.path !== path) {
    router.push(path)
  }

  if ($q.screen.lt.md) {
    drawerOpen.value = false
  }
}

async function logout(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}

async function voltarParaAdministrador(): Promise<void> {
  if (!emulandoSolicitante.value || retornoEmulacaoCarregando.value) {
    return
  }

  retornoEmulacaoCarregando.value = true
  try {
    await authStore.encerrarEmulacao()
    await router.replace('/admin')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Nao foi possivel concluir a acao.'

    $q.notify({
      type: 'negative',
      message,
    })

    if (
      message.includes('Contexto original da emulacao nao encontrado') ||
      message.includes('Contexto original da emulacao nao encontrado')
    ) {
      await router.replace('/login')
    }
  } finally {
    retornoEmulacaoCarregando.value = false
  }
}
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="portal-layout">
    <q-header class="portal-header text-dark">
      <q-toolbar class="portal-toolbar">
        <div class="row items-center q-gutter-sm no-wrap">
          <q-btn flat dense round icon="menu" aria-label="Menu" @click="drawerOpen = !drawerOpen" />

          <div
            class="portal-brand cursor-pointer"
            role="button"
            tabindex="0"
            aria-label="Ir para dashboard do portal"
            @click="navegarPara('/portal')"
            @keydown.enter.prevent="navegarPara('/portal')"
            @keydown.space.prevent="navegarPara('/portal')"
          >
            <div class="portal-brand__title">SGX Sistema de Chamados</div>
            <div class="portal-brand__subtitle">Portal do solicitante</div>
          </div>
        </div>

        <q-space />

        <div class="portal-toolbar__actions gt-sm">
          <NotificacoesBadge compact class="q-mr-sm" />

          <q-btn
            v-for="item in atalhoHeader"
            :key="item.to"
            :flat="!item.destaque"
            :unelevated="item.destaque"
            :color="item.destaque ? 'secondary' : 'primary'"
            :icon="item.icon"
            :label="item.label"
            size="sm"
            @click="navegarPara(item.to)"
          />
        </div>

        <q-btn round flat class="q-ml-sm" aria-label="Abrir menu de usuário">
          <q-avatar size="32px" color="secondary" text-color="white">{{ iniciaisUsuario }}</q-avatar>
          <q-menu anchor="bottom right" self="top right">
            <q-list style="min-width: 230px">
              <q-item>
                <q-item-section avatar>
                  <q-avatar size="34px" color="blue-1" text-color="primary">{{ iniciaisUsuario }}</q-avatar>
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-weight-medium">{{ usuarioNome }}</q-item-label>
                  <q-item-label caption>{{ usuarioEmail }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-separator />
              <q-item clickable v-close-popup @click="logout">
                <q-item-section avatar><q-icon name="logout" /></q-item-section>
                <q-item-section>Sair</q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :width="298"
      :breakpoint="1024"
      :behavior="$q.screen.lt.md ? 'mobile' : 'desktop'"
      class="portal-drawer"
    >
      <div class="q-pa-md drawer-user">
        <q-avatar size="44px" color="secondary" text-color="white">
          {{ iniciaisUsuario }}
        </q-avatar>

        <div>
          <div class="text-weight-medium">{{ usuarioNome }}</div>
          <div class="text-caption text-grey-7">{{ usuarioEmail }}</div>
        </div>
      </div>

      <div class="q-px-md q-pb-sm">
        <q-btn
          color="secondary"
          icon="add"
          label="Novo chamado"
          class="full-width"
          unelevated
          @click="navegarPara('/portal/chamados/novo')"
        />
      </div>

      <q-separator />

      <q-list padding>
        <q-item
          v-for="item in menuNavegacao"
          :key="item.label"
          clickable
          :active="rotaAtiva(item.to)"
          active-class="menu-item-active"
          @click="navegarPara(item.to)"
        >
          <q-item-section avatar>
            <q-icon :name="item.icon" />
          </q-item-section>
          <q-item-section>{{ item.label }}</q-item-section>
        </q-item>
      </q-list>

      <q-separator />

      <div class="q-pa-md">
        <q-btn color="negative" outline icon="logout" label="Sair" class="full-width" @click="logout" />
      </div>
    </q-drawer>

    <q-page-container>
      <div v-if="emulandoSolicitante" class="q-pa-sm q-pa-md">
        <q-banner rounded class="bg-amber-2 text-dark emulacao-banner">
          <template #avatar>
            <q-icon name="person_search" />
          </template>

          <div class="text-weight-medium">Visualizando como Solicitante Demo</div>
          <div class="text-caption">Voce esta visualizando o sistema como Solicitante Demo.</div>

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
.portal-layout {
  background:
    radial-gradient(circle at 100% 0%, rgba(14, 165, 233, 0.12), transparent 42%),
    linear-gradient(180deg, var(--sgx-page-bg) 0%, var(--sgx-page-bg-alt) 100%);
}

.portal-header {
  background: rgba(255, 255, 255, 0.94);
  border-bottom: 1px solid var(--sgx-border);
  backdrop-filter: blur(10px);
}

.portal-toolbar {
  min-height: 64px;
  padding: 8px 14px;
}

.portal-brand {
  min-width: 0;
}

.portal-brand__title {
  font-size: 0.94rem;
  font-weight: 800;
  line-height: 1.15;
  color: var(--sgx-text);
}

.portal-brand__subtitle {
  font-size: 0.76rem;
  color: var(--sgx-muted);
}

.portal-toolbar__actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.portal-drawer {
  background: var(--sgx-surface-soft);
}

.drawer-user {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 12px;
  align-items: center;
}

:deep(.menu-item-active) {
  background: rgba(11, 94, 215, 0.12);
  color: var(--sgx-primary);
  border-radius: var(--sgx-radius-sm);
}

.emulacao-banner {
  border: 1px solid rgba(146, 64, 14, 0.18);
}

@media (max-width: 1023px) {
  .portal-toolbar {
    min-height: 58px;
    padding: 8px 10px;
  }

  .portal-brand__title {
    font-size: 0.88rem;
  }
}
</style>
