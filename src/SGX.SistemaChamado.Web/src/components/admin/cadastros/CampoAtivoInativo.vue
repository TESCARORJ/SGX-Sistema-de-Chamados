<script setup lang="ts">
import { computed } from 'vue'

type OptionValue = 'todos' | 'ativos' | 'inativos'

const props = defineProps<{
  modelValue: OptionValue
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: OptionValue): void
}>()

const localValue = computed({
  get: () => props.modelValue,
  set: (value: OptionValue) => emit('update:modelValue', value),
})

const options: { label: string; value: OptionValue }[] = [
  { label: 'Todos', value: 'todos' },
  { label: 'Apenas ativos', value: 'ativos' },
  { label: 'Apenas inativos', value: 'inativos' },
]
</script>

<template>
  <q-select
    v-model="localValue"
    dense
    outlined
    emit-value
    map-options
    :disable="loading"
    :options="options"
    label="Situacao"
  />
</template>
