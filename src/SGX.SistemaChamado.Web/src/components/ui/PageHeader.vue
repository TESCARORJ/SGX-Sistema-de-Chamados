<script setup lang="ts">
defineProps<{
  titulo: string
  subtitulo?: string
}>()
</script>

<template>
  <div class="row items-start justify-between q-col-gutter-sm q-mb-md">
    <div class="col">
      <div class="sgx-section-title">{{ titulo }}</div>
      <div v-if="subtitulo" class="sgx-section-subtitle q-mt-xs">{{ subtitulo }}</div>
    </div>
    <div class="col-auto">
      <slot name="actions" />
    </div>
  </div>
</template>
