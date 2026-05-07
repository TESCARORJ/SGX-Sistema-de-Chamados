<script setup lang="ts">
import { ref } from 'vue'
import type { QForm } from 'quasar'

defineProps<{
  titulo: string
  loading?: boolean
  somenteLeitura?: boolean
  botaoSalvarLabel?: string
}>()

const emit = defineEmits<{
  (e: 'salvar'): void
  (e: 'cancelar'): void
}>()

const formRef = ref<QForm | null>(null)

async function onSubmit(): Promise<void> {
  const form = formRef.value
  if (!form) {
    emit('salvar')
    return
  }

  const valido = await form.validate()
  if (valido) {
    emit('salvar')
  }
}
</script>

<template>
  <q-form ref="formRef" class="fit" @submit.prevent="onSubmit">
    <q-card flat bordered class="sgx-card">
      <q-card-section>
        <div class="text-h6">{{ titulo }}</div>
      </q-card-section>

      <q-separator />

      <q-card-section>
        <slot />
      </q-card-section>

      <q-separator />

      <q-card-actions align="right">
        <q-btn flat label="Voltar" :disable="loading" @click="emit('cancelar')" />
        <q-btn
          v-if="!somenteLeitura"
          type="submit"
          color="primary"
          :loading="loading"
          :label="botaoSalvarLabel || 'Salvar'"
        />
      </q-card-actions>
    </q-card>
  </q-form>
</template>
