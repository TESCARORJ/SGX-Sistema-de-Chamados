<script setup lang="ts">
const props = defineProps<{
  prioridade: string
}>()

function cor(nome: string): string {
  const lower = nome.toLowerCase()
  if (lower.includes('critica')) return 'negative'
  if (lower.includes('alta')) return 'deep-orange'
  if (lower.includes('media')) return 'warning'
  return 'positive'
}
</script>

<template>
  <q-badge :color="cor(props.prioridade)" text-color="white" :label="prioridade" />
</template>
