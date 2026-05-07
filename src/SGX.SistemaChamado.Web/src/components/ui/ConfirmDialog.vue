<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    modelValue: boolean
    titulo?: string
    mensagem: string
    confirmarLabel?: string
    cancelarLabel?: string
    loading?: boolean
    persistent?: boolean
    color?: string
  }>(),
  {
    titulo: 'Confirmacao',
    confirmarLabel: 'Confirmar',
    cancelarLabel: 'Cancelar',
    loading: false,
    persistent: false,
    color: 'primary',
  }
)

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  confirm: []
  cancel: []
}>()

const aberto = computed({
  get: () => props.modelValue,
  set: (value: boolean) => emit('update:modelValue', value),
})

function confirmar(): void {
  emit('confirm')
}

function cancelar(): void {
  emit('cancel')
  emit('update:modelValue', false)
}
</script>

<template>
  <q-dialog v-model="aberto" :persistent="persistent">
    <q-card class="confirm-dialog-card">
      <q-card-section class="text-h6">{{ titulo }}</q-card-section>

      <q-card-section class="q-pt-none text-body2 text-grey-8">
        {{ mensagem }}
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat :label="cancelarLabel" :disable="loading" @click="cancelar" />
        <q-btn :color="color" unelevated :label="confirmarLabel" :loading="loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.confirm-dialog-card {
  width: min(420px, 92vw);
}
</style>
