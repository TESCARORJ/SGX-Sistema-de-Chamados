<script setup lang="ts">
import EmptyState from '../ui/EmptyState.vue'
import type { ComentarioChamado } from '../../types/comentario'

defineProps<{
  comentarios: ComentarioChamado[]
}>()

function fmtDate(value: string): string {
  return new Date(value).toLocaleString('pt-BR')
}
</script>

<template>
  <div class="column q-gutter-sm">
    <EmptyState
      v-if="!comentarios.length"
      titulo="Sem comentários"
      mensagem="Nenhum comentário encontrado."
    />

    <q-card
      v-for="comentario in comentarios"
      :key="comentario.id"
      flat
      bordered
      class="sgx-card"
      :class="{ 'comentario-interno': comentario.interno }"
    >
      <q-card-section>
        <div class="row items-center justify-between">
          <strong>{{ comentario.usuario }}</strong>
          <q-badge v-if="comentario.interno" color="orange-8" text-color="white">Interno</q-badge>
        </div>
        <div class="q-mt-sm q-mb-sm text-body2">{{ comentario.mensagem }}</div>
        <div class="text-caption text-grey">{{ fmtDate(comentario.criadoEm) }}</div>
      </q-card-section>
    </q-card>
  </div>
</template>

<style scoped>
.comentario-interno {
  border-left: 3px solid #f59e0b;
  background: #fff9eb;
}
</style>
