<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  modelValue: string
  loading?: boolean
  label?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

const localValue = computed({
  get: () => props.modelValue,
  set: (value: string) => emit('update:modelValue', value),
})
</script>

<template>
  <q-input
    v-model="localValue"
    dense
    outlined
    clearable
    :disable="loading"
    :label="label || 'Buscar por texto'"
  >
    <template #prepend>
      <q-icon name="search" />
    </template>
  </q-input>
</template>
