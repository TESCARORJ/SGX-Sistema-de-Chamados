<script setup lang="ts">
defineProps<{
  mensagem?: string
  inline?: boolean
}>()
</script>

<template>
  <q-card v-if="!inline" flat bordered class="sgx-card" role="status" aria-live="polite">
    <q-card-section class="column items-center text-center q-gutter-sm q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
      <div class="text-body2 text-grey-7">{{ mensagem || 'Carregando dados...' }}</div>
    </q-card-section>
  </q-card>

  <div v-else class="row justify-center items-center q-gutter-sm q-py-xl" role="status" aria-live="polite">
    <q-spinner color="primary" size="2rem" />
    <span class="text-body2 text-grey-7">{{ mensagem || 'Carregando dados...' }}</span>
  </div>
</template>
