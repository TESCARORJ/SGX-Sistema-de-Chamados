<script setup lang="ts">
import { ref, watch } from 'vue'
import type { PrioridadeAdmin } from '../../types/admin'

const props = defineProps<{
  modelValue: boolean
  prioridades: PrioridadeAdmin[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', prioridadeId: string): void
}>()

const prioridadeId = ref<string>('')
watch(() => props.modelValue, (opened) => { if (opened) prioridadeId.value = '' })

function confirmar(): void {
  if (!prioridadeId.value) return
  emit('confirmar', prioridadeId.value)
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="modal-card">
      <q-card-section><div class="text-h6">Alterar prioridade</div></q-card-section>
      <q-card-section>
        <q-select
          v-model="prioridadeId"
          :options="props.prioridades.map(p => ({ label: p.nome, value: p.id }))"
          emit-value
          map-options
          outlined
          label="Prioridade"
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="primary" label="Salvar" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.modal-card {
  width: min(420px, 92vw);
}
</style>
