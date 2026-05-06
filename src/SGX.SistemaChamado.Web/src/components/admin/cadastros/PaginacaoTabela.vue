<script setup lang="ts">
const props = defineProps<{
  pagina: number
  tamanhoPagina: number
  total: number
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:pagina', value: number): void
  (e: 'update:tamanhoPagina', value: number): void
}>()

const tamanhos = [10, 20, 50, 100]
</script>

<template>
  <div class="row items-center justify-between q-gutter-sm">
    <div class="text-caption text-grey-8">Total: {{ total }}</div>

    <div class="row items-center q-gutter-sm">
      <q-select
        :model-value="tamanhoPagina"
        dense
        outlined
        emit-value
        map-options
        :disable="loading"
        :options="tamanhos.map((item) => ({ label: `${item} / página`, value: item }))"
        label="Página"
        @update:model-value="(value) => emit('update:tamanhoPagina', Number(value))"
      />

      <q-pagination
        :model-value="pagina"
        :max="Math.max(1, Math.ceil(total / tamanhoPagina))"
        direction-links
        boundary-links
        color="primary"
        :disable="loading"
        @update:model-value="(value) => emit('update:pagina', value)"
      />
    </div>
  </div>
</template>
