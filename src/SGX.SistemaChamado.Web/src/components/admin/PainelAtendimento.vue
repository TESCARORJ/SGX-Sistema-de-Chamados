<script setup lang="ts">
import type { ChamadoAdminDetalhe } from '../../types/admin'

defineProps<{
  chamado: ChamadoAdminDetalhe
  loading?: boolean
  canAssumir?: boolean
  canAtribuir?: boolean
  canEncerrar?: boolean
  canReabrir?: boolean
}>()

const emit = defineEmits<{
  (e: 'assumir'): void
  (e: 'atribuir'): void
  (e: 'alterar-status'): void
  (e: 'alterar-prioridade'): void
  (e: 'alterar-categoria'): void
  (e: 'comentar'): void
  (e: 'encerrar'): void
  (e: 'reabrir'): void
}>()
</script>

<template>
  <q-card flat bordered>
    <q-card-section>
      <div class="text-subtitle1">Acoes administrativas</div>
      <div class="row q-gutter-sm q-mt-sm">
        <q-btn color="secondary" label="Assumir" :loading="loading" :disable="canAssumir === false" @click="emit('assumir')" />
        <q-btn v-if="canAtribuir !== false" outline color="primary" label="Atribuir" :loading="loading" @click="emit('atribuir')" />
        <q-btn outline color="primary" label="Status" :loading="loading" @click="emit('alterar-status')" />
        <q-btn outline color="primary" label="Prioridade" :loading="loading" @click="emit('alterar-prioridade')" />
        <q-btn outline color="primary" label="Categoria" :loading="loading" @click="emit('alterar-categoria')" />
        <q-btn outline color="primary" label="Comentar" :loading="loading" @click="emit('comentar')" />
        <q-btn v-if="canEncerrar !== false" color="negative" label="Encerrar" :loading="loading" @click="emit('encerrar')" />
        <q-btn v-if="canReabrir !== false" color="warning" text-color="black" label="Reabrir" :loading="loading" @click="emit('reabrir')" />
      </div>
    </q-card-section>
  </q-card>
</template>
