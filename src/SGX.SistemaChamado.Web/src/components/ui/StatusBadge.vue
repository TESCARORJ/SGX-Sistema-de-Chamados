<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    texto?: string
    status?: string
  }>(),
  {
    texto: '',
    status: '',
  }
)

const textoExibido = computed(() => props.texto || props.status || '-')

const estilo = computed(() => {
  const valor = textoExibido.value.toLowerCase()

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

  if (valor.includes('encerrado') || valor.includes('concluido')) {
    return { color: 'blue-grey-1', textColor: 'blue-grey-9', icon: 'check_circle' }
  }

  if (valor.includes('cancelado')) {
    return { color: 'grey-3', textColor: 'grey-8', icon: 'block' }
  }

  if (valor.includes('operacional')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'memory' }
  }

  if (valor.includes('manutencao') || valor.includes('manutenção')) {
    return { color: 'amber-1', textColor: 'amber-10', icon: 'build' }
  }

  if (valor.includes('defeito')) {
    return { color: 'red-1', textColor: 'negative', icon: 'warning' }
  }

  if (valor.includes('reservado')) {
    return { color: 'indigo-1', textColor: 'indigo-9', icon: 'bookmark' }
  }

  if (valor.includes('em uso')) {
    return { color: 'blue-1', textColor: 'primary', icon: 'work' }
  }

  if (valor.includes('estoque')) {
    return { color: 'teal-1', textColor: 'teal-9', icon: 'inventory_2' }
  }

  if (valor.includes('emprestado')) {
    return { color: 'purple-1', textColor: 'purple-9', icon: 'swap_horiz' }
  }

  if (valor.includes('transferencia') || valor.includes('transferência')) {
    return { color: 'orange-1', textColor: 'orange-9', icon: 'move_down' }
  }

  if (valor.includes('extraviado')) {
    return { color: 'deep-orange-1', textColor: 'deep-orange-9', icon: 'report_problem' }
  }

  if (valor.includes('alta') || valor.includes('critica') || valor.includes('crítica')) {
    return { color: 'red-1', textColor: 'negative', icon: 'priority_high' }
  }

  if (valor.includes('media') || valor.includes('média')) {
    return { color: 'orange-1', textColor: 'orange-9', icon: 'trending_flat' }
  }

  if (valor.includes('baixa')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'south' }
  }

  if (valor.includes('pendente')) {
    return { color: 'amber-1', textColor: 'amber-10', icon: 'hourglass_top' }
  }

  if (valor.includes('aprovad')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'verified' }
  }

  if (valor.includes('reprovad')) {
    return { color: 'red-1', textColor: 'negative', icon: 'gpp_bad' }
  }

  if (valor.includes('publicado')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'published_with_changes' }
  }

  if (valor.includes('rascunho')) {
    return { color: 'amber-1', textColor: 'amber-10', icon: 'draft' }
  }

  if (valor.includes('revisao') || valor.includes('revisão')) {
    return { color: 'orange-1', textColor: 'orange-9', icon: 'rule' }
  }

  if (valor.includes('arquivado')) {
    return { color: 'blue-grey-1', textColor: 'blue-grey-8', icon: 'archive' }
  }

  if (valor.includes('inativo')) {
    return { color: 'grey-3', textColor: 'grey-8', icon: 'pause_circle' }
  }

  if (valor.includes('ativo')) {
    return { color: 'green-1', textColor: 'green-9', icon: 'check_circle' }
  }

  return { color: 'blue-grey-1', textColor: 'blue-grey-8', icon: 'flag' }
})
</script>

<template>
  <q-chip
    dense
    rounded
    class="status-badge text-weight-medium"
    size="12px"
    :color="estilo.color"
    :text-color="estilo.textColor"
    :icon="estilo.icon"
    :label="textoExibido"
    :title="textoExibido"
  />
</template>

<style scoped>
.status-badge {
  border: 1px solid rgba(11, 94, 215, 0.08);
}
</style>
