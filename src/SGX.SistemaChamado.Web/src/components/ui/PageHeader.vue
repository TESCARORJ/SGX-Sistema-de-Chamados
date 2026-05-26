<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    titulo?: string
    subtitulo?: string
    title?: string
    subtitle?: string
    contexto?: string
  }>(),
  {
    titulo: '',
    subtitulo: '',
    title: '',
    subtitle: '',
    contexto: '',
  }
)

const tituloExibido = computed(() => props.titulo || props.title)
const subtituloExibido = computed(() => props.subtitulo || props.subtitle)
</script>

<template>
  <div class="page-header row items-start justify-between q-col-gutter-md q-mb-md">
    <div class="col page-header__content">
      <div v-if="contexto" class="page-header__contexto">{{ contexto }}</div>
      <h1 class="sgx-section-title page-header__title">{{ tituloExibido }}</h1>
      <div v-if="subtituloExibido" class="sgx-section-subtitle q-mt-xs">{{ subtituloExibido }}</div>
    </div>
    <div v-if="$slots.actions" class="col-auto page-header__actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<style scoped>
.page-header {
  padding: 4px 2px;
}

.page-header__contexto {
  font-size: 0.74rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: var(--sgx-muted);
  font-weight: 700;
}

.page-header__title {
  font-size: clamp(1.15rem, 0.95rem + 0.85vw, 1.8rem);
  margin: 0;
  overflow-wrap: anywhere;
}

.page-header__actions {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: 8px;
}

@media (max-width: 768px) {
  .page-header {
    flex-direction: column;
    align-items: stretch;
  }

  .page-header__actions {
    width: 100%;
    justify-content: flex-start;
    flex-wrap: wrap;
  }
}
</style>
