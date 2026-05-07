<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  vencido?: boolean
  proximo?: boolean
  pausado?: boolean
}>()

const estilo = computed(() => {
  if (props.vencido) {
    return { color: 'red-1', textColor: 'red-9', icon: 'warning', label: 'SLA vencido' }
  }

  if (props.proximo) {
    return { color: 'orange-1', textColor: 'orange-9', icon: 'schedule', label: 'Proximo do vencimento' }
  }

  if (props.pausado) {
    return { color: 'purple-1', textColor: 'purple-8', icon: 'pause_circle', label: 'SLA pausado' }
  }

  return { color: 'green-1', textColor: 'green-9', icon: 'task_alt', label: 'Dentro do prazo' }
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
    :label="estilo.label"
  />
</template>
