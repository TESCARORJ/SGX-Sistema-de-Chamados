<script setup lang="ts">
import type { HistoricoAdmin } from '../../types/admin'

defineProps<{
  historico: HistoricoAdmin[]
}>()

function fmtDate(value: string): string {
  return new Date(value).toLocaleString('pt-BR')
}

function corPorTipo(tipoDescricao: string): string {
  const valor = tipoDescricao.toLowerCase()

  if (valor.includes('encerr')) return 'positive'
  if (valor.includes('reabert')) return 'warning'
  if (valor.includes('coment')) return 'primary'
  if (valor.includes('anexo')) return 'deep-orange'

  return 'secondary'
}
</script>

<template>
  <q-timeline v-if="historico.length" color="primary" layout="dense">
    <q-timeline-entry
      v-for="item in historico"
      :key="item.id"
      :color="corPorTipo(item.tipoDescricao)"
      :title="item.tipoDescricao"
      :subtitle="fmtDate(item.criadoEm)"
    >
      <div>{{ item.descricao }}</div>
      <div class="text-caption text-grey">{{ item.usuario ?? 'Sistema' }}</div>
    </q-timeline-entry>
  </q-timeline>

  <q-banner v-else rounded class="bg-blue-1 text-primary">
    Nenhum evento de historico para este chamado.
  </q-banner>
</template>
