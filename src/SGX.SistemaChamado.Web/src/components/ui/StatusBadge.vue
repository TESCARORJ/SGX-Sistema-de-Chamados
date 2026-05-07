<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  texto: string
}>()

const estilo = computed(() => {
  const valor = props.texto.toLowerCase()

  if (valor.includes('aberto')) {
    return { color: 'blue-1', textColor: 'primary', icon: 'mark_email_unread' }
  }

  if (valor.includes('atendimento')) {
    return { color: 'teal-1', textColor: 'teal-9', icon: 'support_agent' }
  }

  if (valor.includes('aguardando')) {
    return { color: 'orange-1', textColor: 'orange-9', icon: 'hourglass_top' }
  }

  if (valor.includes('resolvido')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'task_alt' }
  }

  if (valor.includes('encerrado')) {
    return { color: 'blue-grey-1', textColor: 'blue-grey-9', icon: 'check_circle' }
  }

  if (valor.includes('cancelado')) {
    return { color: 'grey-3', textColor: 'grey-8', icon: 'block' }
  }

  return { color: 'blue-grey-1', textColor: 'blue-grey-8', icon: 'flag' }
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
