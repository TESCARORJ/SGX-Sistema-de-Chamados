<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import type { PerfilUsuario } from '../types/auth'

const authStore = useAuthStore()
const router = useRouter()

const email = ref(authStore.localDevEmail)
const nome = ref(authStore.localDevNome)
const perfil = ref<PerfilUsuario>(authStore.localDevPerfil)
const erroLocal = ref<string | null>(null)

async function entrarComMicrosoft(): Promise<void> {
  erroLocal.value = null
  try {
    await authStore.loginMicrosoft()
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Falha no login Microsoft.'
  }
}

async function entrarModoLocal(): Promise<void> {
  erroLocal.value = null
  try {
    await authStore.loginLocalDev({
      email: email.value,
      nome: nome.value,
      perfil: perfil.value,
    })
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Falha no modo local.'
  }
}
</script>

<template>
  <main class="login-page">
    <section class="login-card">
      <h1>SGX Sistema de Chamados</h1>
      <p>Autentique com sua conta corporativa Microsoft Entra ID.</p>

      <q-btn
        class="q-mt-sm"
        color="primary"
        icon="login"
        label="Entrar com Microsoft"
        :loading="authStore.carregando"
        @click="entrarComMicrosoft"
      />

      <section v-if="authStore.modoLocal" class="local-mode">
        <h2>Modo local Development</h2>
        <q-input v-model="email" label="E-mail técnico" outlined dense />
        <q-input v-model="nome" label="Nome técnico" outlined dense />
        <q-select
          v-model="perfil"
          :options="['Administrador', 'Atendente', 'Solicitante']"
          label="Perfil simulado"
          outlined
          dense
        />
        <q-btn
          color="secondary"
          label="Entrar em modo local"
          :loading="authStore.carregando"
          @click="entrarModoLocal"
        />
      </section>

      <q-banner v-if="erroLocal || authStore.erro" class="bg-red-1 text-negative q-mt-md rounded-borders">
        {{ erroLocal || authStore.erro }}
      </q-banner>
    </section>
  </main>
</template>

<style scoped>
.login-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1.5rem;
  background: radial-gradient(circle at top right, #cffafe 0%, #ffedd5 48%, #f8fafc 100%);
}

.login-card {
  width: min(560px, 100%);
  background: #ffffff;
  border-radius: 18px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 20px 48px rgba(15, 23, 42, 0.11);
  padding: 1.5rem;
  display: grid;
  gap: 0.8rem;
}

.local-mode {
  display: grid;
  gap: 0.7rem;
  margin-top: 0.6rem;
  padding-top: 0.9rem;
  border-top: 1px dashed #cbd5e1;
}

h1 {
  margin: 0;
  font-size: 1.75rem;
}

h2 {
  margin: 0;
  font-size: 1rem;
}

p {
  margin: 0;
  color: #334155;
}
</style>
