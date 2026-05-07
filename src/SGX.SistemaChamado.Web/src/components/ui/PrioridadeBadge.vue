<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  texto: string
}>()

const estilo = computed(() => {
  const valor = props.texto.toLowerCase()

  if (valor.includes('critica') || valor.includes('cri')) {
    return { color: 'red-1', textColor: 'red-9', icon: 'priority_high' }
  }

  if (valor.includes('alta')) {
    return { color: 'deep-orange-1', textColor: 'deep-orange-9', icon: 'keyboard_double_arrow_up' }
  }

  if (valor.includes('media')) {
    return { color: 'amber-1', textColor: 'amber-9', icon: 'drag_handle' }
  }

  if (valor.includes('baixa')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'keyboard_double_arrow_down' }
  }

  return { color: 'blue-grey-1', textColor: 'blue-grey-9', icon: 'flag' }
})
</script>

<template>
  <q-chip
    dense
    rounded
    size="12px"
    :color="estilo.color"
    :text-color="estilo.textColor"
    :icon="estilo.icon"
    :label="texto"
  />
</template>
