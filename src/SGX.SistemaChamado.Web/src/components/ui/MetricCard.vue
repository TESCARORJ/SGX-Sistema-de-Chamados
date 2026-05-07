<script setup lang="ts">
defineProps<{
  titulo: string
  valor: string | number
  subtitulo?: string
  icon?: string
  color?: string
}>()
</script>

<template>
  <q-card flat bordered class="sgx-card metric-card">
    <q-card-section class="row items-start no-wrap">
      <div class="col">
        <div class="text-caption text-grey-7">{{ titulo }}</div>
        <div class="text-h5 text-weight-bold q-mt-xs">{{ valor }}</div>
        <div v-if="subtitulo" class="text-caption text-grey-7 q-mt-xs">{{ subtitulo }}</div>
      </div>

      <q-avatar v-if="icon" :color="color || 'primary'" text-color="white" size="34px">
        <q-icon :name="icon" />
      </q-avatar>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.metric-card {
  height: 100%;
}
</style>
