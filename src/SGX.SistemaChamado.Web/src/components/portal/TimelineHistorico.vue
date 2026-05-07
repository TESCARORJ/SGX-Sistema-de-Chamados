<script setup lang="ts">
import type { HistoricoChamado } from '../../types/portal'

const props = defineProps<{
  itens: HistoricoChamado[]
}>()

function corPorTipo(tipoDescricao: string): string {
  const valor = tipoDescricao.toLowerCase()

  if (valor.includes('encerr')) return 'positive'
  if (valor.includes('reabert')) return 'warning'
  if (valor.includes('coment')) return 'primary'
  if (valor.includes('anexo')) return 'deep-orange'

  return 'secondary'
}

function iconePorTipo(tipoDescricao: string): string {
  const valor = tipoDescricao.toLowerCase()

  if (valor.includes('encerr')) return 'check_circle'
  if (valor.includes('reabert')) return 'autorenew'
  if (valor.includes('coment')) return 'comment'
  if (valor.includes('anexo')) return 'attach_file'

  return 'history'
}
</script>

<template>
  <q-timeline v-if="props.itens.length" color="primary" side="right" layout="comfortable">
    <q-timeline-entry
      v-for="item in props.itens"
      :key="item.id"
      :color="corPorTipo(item.tipoDescricao)"
      :icon="iconePorTipo(item.tipoDescricao)"
      :title="item.tipoDescricao"
      :subtitle="new Date(item.criadoEm).toLocaleString('pt-BR')"
    >
      <div class="text-body2">{{ item.descricao }}</div>
      <div v-if="item.usuario" class="text-caption text-grey-7 q-mt-xs">por {{ item.usuario }}</div>
    </q-timeline-entry>
  </q-timeline>

  <q-banner v-else rounded class="bg-blue-1 text-primary">
    Nenhum evento de historico encontrado para este chamado.
  </q-banner>
</template>
