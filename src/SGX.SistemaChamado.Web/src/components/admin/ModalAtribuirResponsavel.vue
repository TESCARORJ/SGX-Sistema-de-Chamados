<script setup lang="ts">
import { ref, watch } from 'vue'
import type { AtendenteResumo } from '../../types/admin'

const props = defineProps<{
  modelValue: boolean
  atendentes: AtendenteResumo[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', responsavelId: string): void
}>()

const responsavelId = ref<string>('')

watch(
  () => props.modelValue,
  (opened) => {
    if (opened) {
      responsavelId.value = ''
    }
  }
)

function confirmar(): void {
  if (!responsavelId.value) return
  emit('confirmar', responsavelId.value)
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="modal-card">
      <q-card-section>
        <div class="text-h6">Atribuir responsável</div>
      </q-card-section>
      <q-card-section>
        <q-select
          v-model="responsavelId"
          :options="props.atendentes.map(a => ({ label: `${a.nome} (${a.perfis.join(', ')})`, value: a.id }))"
          emit-value
          map-options
          outlined
          label="Responsável"
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="primary" label="Confirmar" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.modal-card {
  width: min(460px, 92vw);
}
</style>
