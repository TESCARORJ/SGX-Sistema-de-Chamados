<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const drawerOpen = ref(true)

const menu = [
  { label: 'Dashboard', icon: 'dashboard', to: '/portal' },
  { label: 'Meus chamados', icon: 'receipt_long', to: '/portal/chamados' },
  { label: 'Novo chamado', icon: 'add_circle', to: '/portal/chamados/novo' },
]

const usuarioNome = computed(() => authStore.usuario?.nome || 'Solicitante')
const usuarioEmail = computed(() => authStore.usuario?.email || '-')

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
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="portal-layout">
    <q-header elevated class="bg-secondary text-white">
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="drawerOpen = !drawerOpen" />
        <q-toolbar-title class="text-weight-semibold">SGX Sistema de Chamados</q-toolbar-title>
        <q-chip color="white" text-color="secondary" icon="person">Portal do solicitante</q-chip>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="drawerOpen"
      show-if-above
      bordered
      :width="280"
      :breakpoint="1024"
      class="portal-drawer"
    >
      <div class="q-pa-md">
        <div class="text-subtitle1 text-weight-medium">{{ usuarioNome }}</div>
        <div class="text-caption text-grey-7">{{ usuarioEmail }}</div>
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
.portal-layout {
  background:
    radial-gradient(circle at 100% 0%, rgba(249, 115, 22, 0.08), transparent 40%),
    linear-gradient(180deg, #f8fafc 0%, #f0f7ff 100%);
}

.portal-drawer {
  background: #fffdfb;
}

:deep(.menu-item-active) {
  background: rgba(249, 115, 22, 0.16);
  color: #c2410c;
  border-radius: 10px;
}
</style>
