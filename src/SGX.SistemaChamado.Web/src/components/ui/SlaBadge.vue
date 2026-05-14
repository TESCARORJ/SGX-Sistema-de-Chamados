<script setup lang="ts">
import { computed } from 'vue'

type SituacaoSla =
  | 'NaoAplicavel'
  | 'DentroDoPrazo'
  | 'ProximoDoVencimento'
  | 'Vencido'
  | 'Cumprido'
  | 'Violado'
  | 'Pausado'

const props = defineProps<{
  vencido?: boolean
  proximo?: boolean
  pausado?: boolean
  situacao?: SituacaoSla | null
}>()

const estilo = computed(() => {
  switch (props.situacao) {
    case 'NaoAplicavel':
      return { color: 'grey-3', textColor: 'grey-8', icon: 'remove_circle_outline', label: 'SLA não aplicável' }
    case 'Pausado':
      return { color: 'purple-1', textColor: 'purple-8', icon: 'pause_circle', label: 'SLA pausado' }
    case 'Violado':
    case 'Vencido':
      return { color: 'red-1', textColor: 'red-9', icon: 'warning', label: 'SLA vencido' }
    case 'ProximoDoVencimento':
      return { color: 'orange-1', textColor: 'orange-9', icon: 'schedule', label: 'Próximo do vencimento' }
    case 'Cumprido':
      return { color: 'teal-1', textColor: 'teal-9', icon: 'verified', label: 'SLA cumprido' }
    case 'DentroDoPrazo':
      return { color: 'green-1', textColor: 'green-9', icon: 'task_alt', label: 'Dentro do prazo' }
    default:
      if (props.vencido) {
        return { color: 'red-1', textColor: 'red-9', icon: 'warning', label: 'SLA vencido' }
      }

      if (props.proximo) {
        return { color: 'orange-1', textColor: 'orange-9', icon: 'schedule', label: 'Próximo do vencimento' }
      }

      if (props.pausado) {
        return { color: 'purple-1', textColor: 'purple-8', icon: 'pause_circle', label: 'SLA pausado' }
      }

      return { color: 'green-1', textColor: 'green-9', icon: 'task_alt', label: 'Dentro do prazo' }
  }
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
