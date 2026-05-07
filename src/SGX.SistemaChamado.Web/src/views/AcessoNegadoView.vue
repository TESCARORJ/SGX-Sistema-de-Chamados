<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()
</script>

<template>
  <q-page class="row items-center justify-center sgx-page">
    <q-card class="sgx-card denied-card" flat bordered>
      <q-card-section>
        <div class="text-h5 text-negative">Acesso negado</div>
        <div class="text-body2 text-grey-8 q-mt-sm">
          Seu usuario esta autenticado, mas nao possui permissao para esta area.
        </div>
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat color="primary" label="Voltar para login" @click="router.push('/login')" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>

<style scoped>
.denied-card {
  width: min(540px, 100%);
}
</style>
