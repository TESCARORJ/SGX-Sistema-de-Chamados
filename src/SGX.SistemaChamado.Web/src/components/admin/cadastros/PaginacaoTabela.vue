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
  <div class="paginacao-tabela row items-center justify-between q-gutter-sm">
    <div class="text-caption text-grey-8 paginacao-tabela__total">
      Total: {{ total }} | Pagina {{ pagina }} de {{ Math.max(1, Math.ceil(total / tamanhoPagina)) }}
    </div>

    <div class="row items-center q-gutter-sm paginacao-tabela__controles">
      <q-select
        :model-value="tamanhoPagina"
        dense
        outlined
        emit-value
        map-options
        :disable="loading"
        :options="tamanhos.map((item) => ({ label: `${item} / pagina`, value: item }))"
        label="Itens por pagina"
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

<style scoped>
@media (max-width: 768px) {
  .paginacao-tabela {
    flex-direction: column;
    align-items: stretch;
  }

  .paginacao-tabela__controles {
    width: 100%;
    justify-content: space-between;
  }

  .paginacao-tabela__controles :deep(.q-field),
  .paginacao-tabela__controles :deep(.q-pagination) {
    max-width: 100%;
  }
}
</style>

