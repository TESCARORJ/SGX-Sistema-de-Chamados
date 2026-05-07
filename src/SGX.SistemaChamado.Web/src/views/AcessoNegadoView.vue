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
  <q-page class="row items-center justify-center q-pa-md">
    <q-card flat bordered class="sgx-card denied-card">
      <q-card-section class="text-center">
        <q-icon name="gpp_bad" color="negative" size="52px" />
        <div class="text-h5 text-weight-bold text-negative q-mt-sm">Acesso negado</div>
        <div class="text-body2 text-grey-8 q-mt-sm">
          Seu usuario esta autenticado, mas nao possui permissao para acessar esta area do sistema.
        </div>
      </q-card-section>

      <q-card-actions align="right" class="q-pa-md q-gutter-sm">
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="voltar" />
        <q-btn color="secondary" unelevated icon="login" :label="labelDestino" @click="irParaDestino" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>

<style scoped>
.denied-card {
  width: min(560px, 100%);
}
</style>
