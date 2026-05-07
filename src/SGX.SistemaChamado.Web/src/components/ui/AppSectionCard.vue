<script setup lang="ts">
defineProps<{
  titulo: string
  subtitulo?: string
}>()
</script>

<template>
  <q-card flat bordered class="sgx-card app-section-card">
    <q-card-section class="app-section-card__header row items-start justify-between q-col-gutter-md">
      <div class="col app-section-card__content">
        <div class="text-subtitle1 text-weight-bold">{{ titulo }}</div>
        <div v-if="subtitulo" class="text-caption sgx-muted q-mt-xs">{{ subtitulo }}</div>
      </div>
      <div class="col-auto app-section-card__actions">
        <slot name="actions" />
      </div>
    </q-card-section>

    <q-separator />

    <q-card-section class="app-section-card__body">
      <slot />
    </q-card-section>
  </q-card>
</template>

<style scoped>
.app-section-card {
  height: 100%;
}

.app-section-card__header {
  padding: 18px 20px 14px;
}

.app-section-card__body {
  padding: 18px 20px 20px;
}

.app-section-card__actions {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 8px;
}

@media (max-width: 768px) {
  .app-section-card__header {
    flex-direction: column;
    align-items: stretch;
  }

  .app-section-card__actions {
    width: 100%;
    justify-content: flex-start;
    flex-wrap: wrap;
  }

  .app-section-card__actions :deep(.q-btn),
  .app-section-card__actions :deep(.q-chip),
  .app-section-card__actions :deep(.q-badge) {
    max-width: 100%;
  }
}
</style>
