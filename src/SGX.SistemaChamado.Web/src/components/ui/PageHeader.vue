<script setup lang="ts">
defineProps<{
  titulo: string
  subtitulo?: string
}>()
</script>

<template>
  <div class="page-header row items-start justify-between q-col-gutter-sm q-mb-md">
    <div class="col page-header__content">
      <div class="sgx-section-title">{{ titulo }}</div>
      <div v-if="subtitulo" class="sgx-section-subtitle q-mt-xs">{{ subtitulo }}</div>
    </div>
    <div class="col-auto page-header__actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<style scoped>
.page-header__actions {
  display: flex;
  justify-content: flex-end;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .page-header__actions {
    width: 100%;
    justify-content: flex-start;
  }

  .page-header__actions :deep(.q-btn),
  .page-header__actions :deep(.q-chip),
  .page-header__actions :deep(.q-badge) {
    max-width: 100%;
  }
}
</style>
