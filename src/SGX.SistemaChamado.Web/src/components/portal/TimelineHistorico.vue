<script setup lang="ts">
import type { HistoricoChamado } from '../../types/portal'

defineProps<{
  itens: HistoricoChamado[]
}>()
</script>

<template>
  <q-timeline color="primary" side="right" layout="comfortable">
    <q-timeline-entry v-for="item in itens" :key="item.id" :title="item.tipoDescricao" :subtitle="new Date(item.criadoEm).toLocaleString()">
      <div>{{ item.descricao }}</div>
      <div class="text-caption text-grey-7" v-if="item.usuario">por {{ item.usuario }}</div>
    </q-timeline-entry>
  </q-timeline>
</template>
