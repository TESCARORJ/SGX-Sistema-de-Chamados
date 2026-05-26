<script setup lang="ts">
import { ref } from 'vue'
import type { QForm } from 'quasar'

defineProps<{
  titulo: string
  loading?: boolean
  somenteLeitura?: boolean
  botaoSalvarLabel?: string
  subtitulo?: string
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
    <q-card flat bordered class="sgx-card form-cadastro">
      <q-card-section class="form-cadastro__header">
        <div class="text-h6">{{ titulo }}</div>
        <div v-if="subtitulo" class="text-caption sgx-muted q-mt-xs">{{ subtitulo }}</div>
      </q-card-section>

      <q-separator />

      <q-card-section>
        <q-banner v-if="somenteLeitura" rounded class="bg-blue-1 text-blue-10 q-mb-md">
          Modo somente leitura. Voce pode visualizar os dados, mas nao alterar este cadastro.
        </q-banner>

        <slot />
      </q-card-section>

      <q-separator />

      <q-card-actions align="right" class="form-cadastro__actions">
        <q-btn flat label="Voltar" :disable="loading" @click="emit('cancelar')" />
        <q-btn
          v-if="!somenteLeitura"
          type="submit"
          color="primary"
          unelevated
          icon="save"
          :loading="loading"
          :label="botaoSalvarLabel || 'Salvar'"
        />
      </q-card-actions>
    </q-card>
  </q-form>
</template>

<style scoped>
.form-cadastro__header {
  padding-bottom: 14px;
}

.form-cadastro__actions {
  padding: 12px 16px 16px;
}
</style>

