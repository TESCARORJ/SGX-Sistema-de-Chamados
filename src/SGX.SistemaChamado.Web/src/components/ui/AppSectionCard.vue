<script setup lang="ts">
import SectionCard from './SectionCard.vue'

withDefaults(
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
</script>

<template>
  <SectionCard
    :titulo="titulo"
    :subtitulo="subtitulo"
    :title="title"
    :subtitle="subtitle"
    :icon="icon"
    :sem-separador="semSeparador"
  >
    <template v-if="$slots.actions" #actions>
      <slot name="actions" />
    </template>
    <slot />
  </SectionCard>
</template>
