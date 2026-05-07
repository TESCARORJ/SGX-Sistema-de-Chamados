<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  texto: string
}>()

const color = computed(() => {
  const valor = props.texto.toLowerCase()
  if (valor.includes('critica') || valor.includes('alta')) return 'negative'
  if (valor.includes('media')) return 'warning'
  if (valor.includes('baixa')) return 'positive'
  return 'secondary'
})
</script>

<template>
  <q-chip dense square :color="color" text-color="white" :label="texto" />
</template>
