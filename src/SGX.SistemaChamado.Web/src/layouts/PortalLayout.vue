<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

async function logout() {
  await authStore.logout()
  await router.replace('/login')
}
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>SGX.SistemaChamado - Portal</q-toolbar-title>
        <div class="q-mr-md text-caption">{{ authStore.usuario?.nome }}</div>
        <q-btn flat dense icon="logout" label="Sair" @click="logout" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <q-page class="q-pa-md portal-page">
        <router-view />
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.portal-page {
  background: linear-gradient(150deg, #f8fafc 0%, #e0f2fe 100%);
}
</style>
