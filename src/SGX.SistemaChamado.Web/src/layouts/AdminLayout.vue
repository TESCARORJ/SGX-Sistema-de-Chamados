<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

type MenuItem = {
  label: string
  icon: string
  to?: string
  children?: MenuItem[]
}

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const drawerOpen = ref(true)

const menu: MenuItem[] = [
  { label: 'Dashboard', icon: 'space_dashboard', to: '/admin' },
  { label: 'Chamados', icon: 'support_agent', to: '/admin/chamados' },
  {
    label: 'Cadastros',
    icon: 'dataset',
    children: [
      { label: 'Usuarios', icon: 'group', to: '/admin/cadastros/usuarios' },
      { label: 'Perfis', icon: 'badge', to: '/admin/cadastros/perfis' },
      { label: 'Departamentos', icon: 'apartment', to: '/admin/cadastros/departamentos' },
      { label: 'Categorias', icon: 'category', to: '/admin/cadastros/categorias' },
      { label: 'Prioridades', icon: 'priority_high', to: '/admin/cadastros/prioridades' },
      { label: 'Status', icon: 'flag', to: '/admin/cadastros/status' },
    ],
  },
  {
    label: 'Configuracoes',
    icon: 'settings',
    children: [{ label: 'Parametros do Sistema', icon: 'tune', to: '/admin/configuracoes/parametros' }],
  },
  {
    label: 'Integracoes',
    icon: 'hub',
    children: [{ label: 'E-mail', icon: 'mail', to: '/admin/integracoes/email' }],
  },
]

const usuarioNome = computed(() => authStore.usuario?.nome || 'Usuario')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')
const iniciaisUsuario = computed(() => {
  const nome = usuarioNome.value.trim()
  if (!nome) return 'U'
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

async function logout(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}

function navegarPara(path: string): void {
  if (route.path !== path) {
    router.push(path)
  }

  if ($q.screen.lt.md) {
    drawerOpen.value = false
  }
}
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="admin-layout">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="drawerOpen = !drawerOpen" />
        <q-toolbar-title class="text-weight-semibold">SGX Sistema de Chamados</q-toolbar-title>

        <q-chip color="white" text-color="primary" icon="admin_panel_settings" square>
          Area administrativa
        </q-chip>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :width="290"
      :breakpoint="1024"
      class="admin-drawer"
    >
      <div class="q-pa-md drawer-user">
        <q-avatar size="44px" color="primary" text-color="white">{{ iniciaisUsuario }}</q-avatar>
        <div>
          <div class="text-weight-medium">{{ usuarioNome }}</div>
          <div class="text-caption text-grey-7">{{ usuarioEmail }}</div>
        </div>
      </div>

      <q-separator />

      <q-list padding>
        <template v-for="item in menu" :key="item.label">
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
          </q-item>

          <q-expansion-item
            v-else
            dense-toggle
            expand-separator
            :icon="item.icon"
            :label="item.label"
            :default-opened="grupoAberto(item.children)"
            header-class="menu-group"
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

      <q-separator />

      <div class="q-pa-md">
        <q-btn
          color="negative"
          outline
          icon="logout"
          label="Sair"
          class="full-width"
          @click="logout"
        />
      </div>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.admin-layout {
  background:
    radial-gradient(circle at 0% 0%, rgba(14, 116, 144, 0.08), transparent 36%),
    linear-gradient(180deg, #f3f6fb 0%, #ecf1f7 100%);
}

.admin-drawer {
  background: #fbfdff;
}

.drawer-user {
  display: grid;
  grid-template-columns: auto 1fr;
  gap: 12px;
  align-items: center;
}

:deep(.menu-item-active) {
  background: rgba(15, 118, 110, 0.12);
  color: #0f766e;
  border-radius: 10px;
}

:deep(.menu-group) {
  border-radius: 10px;
}
</style>
