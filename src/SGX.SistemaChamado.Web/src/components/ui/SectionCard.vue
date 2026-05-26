<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    titulo?: string
    subtitulo?: string
    title?: string
    subtitle?: string
    icon?: string
    semSeparador?: boolean
  }>(),
  {
    titulo: '',
    subtitulo: '',
    title: '',
    subtitle: '',
    icon: '',
    semSeparador: false,
  }
)

const tituloExibido = computed(() => props.titulo || props.title)
const subtituloExibido = computed(() => props.subtitulo || props.subtitle)
</script>

<template>
  <q-card flat bordered class="sgx-card section-card">
    <q-card-section
      v-if="tituloExibido || subtituloExibido || icon || $slots.actions"
      class="section-card__header row items-start justify-between q-col-gutter-md"
    >
      <div class="col section-card__content">
        <div class="row items-center q-gutter-sm">
          <q-icon v-if="icon" :name="icon" size="20px" color="primary" />
          <h2 class="text-subtitle1 text-weight-bold section-card__title">{{ tituloExibido }}</h2>
        </div>
        <div v-if="subtituloExibido" class="text-caption sgx-muted q-mt-xs">{{ subtituloExibido }}</div>
      </div>
      <div v-if="$slots.actions" class="col-auto section-card__actions">
        <slot name="actions" />
      </div>
    </q-card-section>

    <q-separator v-if="!semSeparador && (tituloExibido || subtituloExibido || icon || $slots.actions)" />

    <q-card-section class="section-card__body">
      <slot />
    </q-card-section>
  </q-card>
</template>

<style scoped>
.section-card {
  height: 100%;
}

.section-card__header {
  padding: 18px 20px 14px;
}

.section-card__body {
  padding: 18px 20px 20px;
}

.section-card__actions {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 8px;
}

.section-card__title {
  margin: 0;
  overflow-wrap: anywhere;
}

@media (max-width: 768px) {
  .section-card__header {
    flex-direction: column;
    align-items: stretch;
  }

  .section-card__actions {
    width: 100%;
    justify-content: flex-start;
    flex-wrap: wrap;
  }
}
</style>
