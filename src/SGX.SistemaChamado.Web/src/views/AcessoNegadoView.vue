<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const rotaDestino = computed(() => {
  if (!authStore.autenticado) {
    return '/login'
  }

  return authStore.rotaInicial
})

const labelDestino = computed(() => (authStore.autenticado ? 'Ir para meu painel' : 'Ir para login'))

function voltar(): void {
  router.back()
}

function irParaDestino(): void {
  router.push(rotaDestino.value)
}
</script>

<template>
  <q-page class="auth-page-shell">
    <q-card flat bordered class="sgx-card auth-card-shell denied-card">
      <q-card-section class="text-center auth-card-header">
        <q-avatar size="64px" color="red-1" text-color="negative">
          <q-icon name="gpp_bad" size="34px" />
        </q-avatar>
        <div class="auth-card-title q-mt-md">Acesso negado</div>
        <div class="auth-card-subtitle">
          Seu usuário está autenticado, mas não possui autorização para esta área.
          Se necessário, solicite a liberação ao administrador do SGX.
        </div>
      </q-card-section>

      <q-card-section class="q-pt-none q-px-lg q-pb-lg">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-sm-6">
            <q-btn flat color="primary" icon="arrow_back" label="Voltar" class="full-width" @click="voltar" />
          </div>
          <div class="col-12 col-sm-6">
            <q-btn color="primary" unelevated icon="login" :label="labelDestino" class="full-width" @click="irParaDestino" />
          </div>
        </div>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.denied-card {
  max-width: 560px;
}
</style>
