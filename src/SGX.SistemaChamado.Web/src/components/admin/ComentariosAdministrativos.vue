<script setup lang="ts">
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
    <q-card v-for="comentario in comentarios" :key="comentario.id" flat bordered>
      <q-card-section>
        <div class="row items-center justify-between">
          <strong>{{ comentario.usuario }}</strong>
          <q-badge v-if="comentario.interno" color="orange-8" text-color="white">Interno</q-badge>
        </div>
        <p class="q-mt-sm q-mb-sm">{{ comentario.mensagem }}</p>
        <div class="text-caption text-grey">{{ fmtDate(comentario.criadoEm) }}</div>
      </q-card-section>
    </q-card>

    <q-banner v-if="!comentarios.length" class="bg-blue-1 text-primary">Nenhum comentario administrativo.</q-banner>
  </div>
</template>
