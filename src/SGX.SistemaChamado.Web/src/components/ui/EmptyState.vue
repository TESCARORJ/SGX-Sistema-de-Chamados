<script setup lang="ts">
defineProps<{
  titulo?: string
  mensagem: string
  icon?: string
}>()
</script>

<template>
  <q-card flat bordered class="sgx-card">
    <q-card-section class="column items-center text-center q-gutter-sm q-py-xl">
      <q-icon :name="icon || 'inbox'" color="grey-6" size="42px" />
      <div class="text-subtitle1 text-weight-medium">{{ titulo || 'Nenhum dado encontrado' }}</div>
      <div class="text-body2 text-grey-7">{{ mensagem }}</div>
      <slot name="actions" />
    </q-card-section>
  </q-card>
</template>
