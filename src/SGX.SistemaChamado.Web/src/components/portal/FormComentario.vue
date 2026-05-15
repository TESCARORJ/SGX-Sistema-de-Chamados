<script setup lang="ts">
import { ref } from 'vue'

const props = withDefaults(
  defineProps<{
    loading?: boolean
    podeComentarInterno?: boolean
  }>(),
  {
    loading: false,
    podeComentarInterno: false,
  }
)

const emit = defineEmits<{
  submit: [payload: { mensagem: string; interno: boolean }]
}>()

const mensagem = ref('')
const interno = ref(false)

function enviar(): void {
  const texto = mensagem.value.trim()
  if (!texto || props.loading) {
    return
  }

  emit('submit', { mensagem: texto, interno: props.podeComentarInterno ? interno.value : false })
  mensagem.value = ''
  interno.value = false
}
</script>

<template>
  <q-form class="column q-gutter-sm" @submit.prevent="enviar">
    <q-input
      v-model="mensagem"
      type="textarea"
      outlined
      autogrow
      maxlength="4000"
      counter
      label="Novo comentário"
      :disable="props.loading"
      :rules="[(v) => !!String(v || '').trim() || 'Informe um comentário']"
    />

    <q-toggle
      v-if="props.podeComentarInterno"
      v-model="interno"
      label="Comentário interno"
      :disable="props.loading"
    />

    <div class="row justify-end">
      <q-btn
        type="submit"
        color="primary"
        icon="send"
        label="Adicionar comentário"
        :loading="props.loading"
        :disable="props.loading"
      />
    </div>
  </q-form>
</template>
