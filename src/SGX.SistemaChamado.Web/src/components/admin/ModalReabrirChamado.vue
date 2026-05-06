<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  modelValue: boolean
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', mensagem: string): void
}>()

const mensagem = ref('')

watch(() => props.modelValue, (opened) => {
  if (opened) {
    mensagem.value = ''
  }
})

function confirmar(): void {
  if (!mensagem.value.trim()) return
  emit('confirmar', mensagem.value.trim())
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card style="min-width: 420px">
      <q-card-section><div class="text-h6">Reabrir chamado</div></q-card-section>
      <q-card-section>
        <q-input v-model="mensagem" type="textarea" outlined label="Motivo da reabertura" autogrow />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="warning" text-color="black" label="Reabrir" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>
