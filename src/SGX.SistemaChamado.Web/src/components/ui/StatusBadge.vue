<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  texto: string
}>()

const color = computed(() => {
  const valor = props.texto.toLowerCase()
  if (valor.includes('encerrado') || valor.includes('resolvido')) return 'positive'
  if (valor.includes('aguardando')) return 'warning'
  if (valor.includes('cancelado')) return 'grey-7'
  if (valor.includes('aberto')) return 'primary'
  return 'secondary'
})
</script>

<template>
  <q-chip dense square :color="color" text-color="white" :label="texto" />
</template>
