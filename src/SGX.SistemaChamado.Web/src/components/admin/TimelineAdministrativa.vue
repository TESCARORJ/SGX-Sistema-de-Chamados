<script setup lang="ts">
import type { HistoricoAdmin } from '../../types/admin'

defineProps<{
  historico: HistoricoAdmin[]
}>()

function fmtDate(value: string): string {
  return new Date(value).toLocaleString('pt-BR')
}
</script>

<template>
  <q-timeline color="primary" layout="dense">
    <q-timeline-entry
      v-for="item in historico"
      :key="item.id"
      :title="item.tipoDescricao"
      :subtitle="fmtDate(item.criadoEm)"
    >
      <div>{{ item.descricao }}</div>
      <div class="text-caption text-grey">{{ item.usuario ?? 'Sistema' }}</div>
    </q-timeline-entry>
  </q-timeline>
</template>
