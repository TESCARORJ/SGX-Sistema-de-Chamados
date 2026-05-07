<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  modelValue: boolean
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', payload: { solucao: string; comentarioInterno: boolean }): void
}>()

const solucao = ref('')
const comentarioInterno = ref(false)

watch(() => props.modelValue, (opened) => {
  if (opened) {
    solucao.value = ''
    comentarioInterno.value = false
  }
})

function confirmar(): void {
  if (!solucao.value.trim()) return
  emit('confirmar', { solucao: solucao.value.trim(), comentarioInterno: comentarioInterno.value })
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="modal-card">
      <q-card-section><div class="text-h6">Encerrar chamado</div></q-card-section>
      <q-card-section class="column q-gutter-sm">
        <q-input v-model="solucao" type="textarea" outlined label="Solucao/comentario final" autogrow />
        <q-toggle v-model="comentarioInterno" label="Comentario interno" />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="negative" label="Encerrar" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.modal-card {
  width: min(520px, 92vw);
}
</style>
