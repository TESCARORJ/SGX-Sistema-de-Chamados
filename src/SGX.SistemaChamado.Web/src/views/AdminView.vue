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
      <h1>Painel Administrativo</h1>
      <p>Usuário: {{ authStore.usuario?.nome }} ({{ authStore.usuario?.email }})</p>
      <div class="chips">
        <q-chip
          v-for="perfil in authStore.usuario?.perfis ?? []"
          :key="perfil"
          color="primary"
          text-color="white"
          :label="perfil"
        />
      </div>
      <q-btn class="q-mt-md" color="negative" label="Sair" @click="sair" />
    </section>
  </main>
</template>

<style scoped>
.page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1.5rem;
  background: linear-gradient(135deg, #ecfeff 0%, #fff7ed 100%);
}

.card {
  width: min(760px, 100%);
  background: #ffffff;
  border-radius: 14px;
  border: 1px solid #bae6fd;
  box-shadow: 0 18px 44px rgba(15, 23, 42, 0.09);
  padding: 1.5rem;
}

.chips {
  margin-top: 0.7rem;
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}
</style>
