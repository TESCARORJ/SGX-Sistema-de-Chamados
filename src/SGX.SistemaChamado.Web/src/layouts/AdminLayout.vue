<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

async function logout(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-dark text-white">
      <q-toolbar>
        <q-toolbar-title>SGX.SistemaChamado - Atendimento Administrativo</q-toolbar-title>
        <q-tabs dense inline-label class="text-white">
          <q-route-tab to="/admin" label="Dashboard" icon="space_dashboard" exact />
          <q-route-tab to="/admin/chamados" label="Chamados" icon="support_agent" exact />
          <q-btn-dropdown flat dense label="Cadastros" icon="dataset">
            <q-list dense>
              <q-item clickable v-close-popup to="/admin/cadastros/usuarios"><q-item-section>Usuários</q-item-section></q-item>
              <q-item clickable v-close-popup to="/admin/cadastros/perfis"><q-item-section>Perfis</q-item-section></q-item>
              <q-item clickable v-close-popup to="/admin/cadastros/departamentos"><q-item-section>Departamentos</q-item-section></q-item>
              <q-item clickable v-close-popup to="/admin/cadastros/categorias"><q-item-section>Categorias</q-item-section></q-item>
              <q-item clickable v-close-popup to="/admin/cadastros/prioridades"><q-item-section>Prioridades</q-item-section></q-item>
              <q-item clickable v-close-popup to="/admin/cadastros/status"><q-item-section>Status</q-item-section></q-item>
            </q-list>
          </q-btn-dropdown>
          <q-btn-dropdown flat dense label="Configurações" icon="settings">
            <q-list dense>
              <q-item clickable v-close-popup to="/admin/configuracoes/parametros"><q-item-section>Parâmetros do Sistema</q-item-section></q-item>
            </q-list>
          </q-btn-dropdown>
          <q-btn-dropdown flat dense label="Integrações" icon="hub">
            <q-list dense>
              <q-item clickable v-close-popup to="/admin/integracoes/email"><q-item-section>E-mail</q-item-section></q-item>
            </q-list>
          </q-btn-dropdown>
        </q-tabs>
        <div class="q-ml-md text-caption">{{ authStore.usuario?.nome }}</div>
        <q-btn flat dense icon="logout" label="Sair" @click="logout" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-md admin-page">
        <router-view />
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.admin-page {
  background: linear-gradient(130deg, #f8fafc 0%, #e2e8f0 100%);
}
</style>
