<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

async function sair(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}
</script>

<template>
  <main class="page">
    <section class="card">
      <h1>Portal do Solicitante</h1>
      <p>Usuário: {{ authStore.usuario?.nome }} ({{ authStore.usuario?.email }})</p>
      <q-chip color="secondary" text-color="white" :label="authStore.usuario?.autenticadoPor" />
      <q-btn class="q-mt-md" color="primary" label="Sair" @click="sair" />
    </section>
  </main>
</template>

<style scoped>
.page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1.5rem;
  background: linear-gradient(135deg, #f0fdf4 0%, #eff6ff 100%);
}

.card {
  width: min(640px, 100%);
  background: #ffffff;
  border-radius: 14px;
  border: 1px solid #d1fae5;
  box-shadow: 0 18px 40px rgba(15, 23, 42, 0.08);
  padding: 1.5rem;
}
</style>
