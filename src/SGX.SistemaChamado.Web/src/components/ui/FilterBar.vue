<script setup lang="ts">
withDefaults(
  defineProps<{
    titulo?: string
    subtitulo?: string
    compact?: boolean
  }>(),
  {
    titulo: '',
    subtitulo: '',
    compact: false,
  }
)
</script>

<template>
  <div class="filter-bar" :class="{ 'filter-bar--compact': compact }">
    <div v-if="titulo || subtitulo" class="filter-bar__header q-mb-sm">
      <div v-if="titulo" class="text-subtitle2 text-weight-bold">{{ titulo }}</div>
      <div v-if="subtitulo" class="text-caption sgx-muted">{{ subtitulo }}</div>
    </div>

    <div class="filter-bar__content">
      <slot />
    </div>

    <div v-if="$slots.actions" class="filter-bar__actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<style scoped>
.filter-bar {
  width: 100%;
  border: 1px solid var(--sgx-border-soft);
  border-radius: var(--sgx-radius-sm);
  background: var(--sgx-surface-soft);
  padding: 12px;
  overflow: hidden;
}

.filter-bar--compact {
  padding: 10px;
}

.filter-bar__actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 10px;
  flex-wrap: wrap;
}

.filter-bar__content {
  min-width: 0;
}

.filter-bar__content :deep(.row) {
  min-width: 0;
}

@media (max-width: 768px) {
  .filter-bar__actions {
    justify-content: flex-start;
  }
}
</style>
