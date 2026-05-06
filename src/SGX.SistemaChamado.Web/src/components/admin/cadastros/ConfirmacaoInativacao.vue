<script setup lang="ts">
const props = defineProps<{
  modelValue: boolean
  titulo: string
  mensagem: string
  loading?: boolean
  acaoLabel?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar'): void
}>()

function fechar(): void {
  emit('update:modelValue', false)
}
</script>

<template>
  <q-dialog :model-value="modelValue" persistent @update:model-value="(value) => emit('update:modelValue', value)">
    <q-card style="min-width: 360px">
      <q-card-section>
        <div class="text-h6">{{ titulo }}</div>
      </q-card-section>

      <q-card-section class="text-body2">
        {{ mensagem }}
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Cancelar" :disable="loading" @click="fechar" />
        <q-btn color="negative" :loading="loading" :label="acaoLabel || 'Confirmar'" @click="emit('confirmar')" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>
