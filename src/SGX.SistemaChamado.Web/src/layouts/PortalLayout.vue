<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const drawerOpen = ref(!$q.screen.lt.md)
const retornoEmulacaoCarregando = ref(false)

const menu = [
  { label: 'Dashboard', icon: 'space_dashboard', to: '/portal' },
  { label: 'Meus chamados', icon: 'receipt_long', to: '/portal/chamados' },
  { label: 'Novo chamado', icon: 'add_circle', to: '/portal/chamados/novo' },
  { label: 'Catálogo de serviços', icon: 'inventory_2', to: '/portal/catalogo-servicos' },
  { label: 'Base de conhecimento', icon: 'menu_book', to: '/portal/base-conhecimento' },
]

const usuarioNome = computed(() => authStore.usuario?.nome || 'Solicitante')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')
const emulandoSolicitante = computed(
  () => authStore.emulandoPerfil && authStore.perfilEmulado === 'Solicitante'
)
const iniciaisUsuario = computed(() => {
  const nome = usuarioNome.value.trim()

  if (!nome) {
    return 'S'
  }

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
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="portal-layout">
    <q-header elevated class="portal-header text-dark">
      <q-toolbar class="q-px-md q-py-sm">
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="drawerOpen = !drawerOpen" />

        <q-toolbar-title class="text-weight-bold">SGX Sistema de Chamados</q-toolbar-title>

        <q-chip color="blue-1" text-color="primary" icon="person" square>Portal do solicitante</q-chip>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :width="290"
      :breakpoint="1024"
      :behavior="$q.screen.lt.md ? 'mobile' : 'desktop'"
      class="portal-drawer"
    >
      <div class="q-pa-md drawer-user">
        <q-avatar size="42px" color="secondary" text-color="white">
          {{ iniciaisUsuario }}
        </q-avatar>

        <div>
          <div class="text-weight-medium">{{ usuarioNome }}</div>
          <div class="text-caption text-grey-7">{{ usuarioEmail }}</div>
        </div>
      </div>

      <q-separator />

      <q-list padding>
        <q-item
          v-for="item in menu"
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
          <div class="text-caption">Você está visualizando o sistema como Solicitante Demo.</div>

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
    linear-gradient(180deg, #f7faff 0%, #edf3fb 100%);
}

.portal-header {
  background: #ffffff;
  border-bottom: 1px solid var(--sgx-border);
}

.portal-drawer {
  background: #f8fbff;
}

.drawer-user {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 12px;
  align-items: center;
}

:deep(.menu-item-active) {
  background: rgba(11, 94, 215, 0.12);
  color: #0b5ed7;
  border-radius: 10px;
}

.emulacao-banner {
  border: 1px solid rgba(146, 64, 14, 0.18);
}
</style>

