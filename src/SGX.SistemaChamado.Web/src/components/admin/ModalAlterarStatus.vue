<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { StatusAdmin } from '../../types/admin'

const props = defineProps<{
  modelValue: boolean
  status: StatusAdmin[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', statusId: string): void
}>()

const statusId = ref<string>('')
const opcoesStatus = computed(() => props.status.map((item) => ({ label: item.nome, value: item.id })))

watch(() => props.modelValue, (opened) => { if (opened) statusId.value = '' })

function confirmar(): void {
  if (!statusId.value || opcoesStatus.value.length === 0) return
  emit('confirmar', statusId.value)
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="modal-card">
      <q-card-section><div class="text-h6">Alterar status</div></q-card-section>
      <q-card-section>
        <q-banner v-if="opcoesStatus.length === 0" rounded class="bg-amber-1 text-dark q-mb-sm">
          Nao ha status compativeis para a natureza deste chamado.
        </q-banner>
        <q-select
          v-model="statusId"
          :options="opcoesStatus"
          emit-value
          map-options
          outlined
          label="Status"
          :disable="opcoesStatus.length === 0"
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="primary" label="Salvar" :loading="props.loading" :disable="opcoesStatus.length === 0" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.modal-card {
  width: min(420px, 92vw);
}
</style>
