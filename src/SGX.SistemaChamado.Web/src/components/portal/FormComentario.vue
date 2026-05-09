<script setup lang="ts">
import { ref } from 'vue'

const props = withDefaults(
  defineProps<{
    loading?: boolean
  }>(),
  {
    loading: false,
  }
)

const emit = defineEmits<{
  submit: [mensagem: string]
}>()

const mensagem = ref('')

function enviar(): void {
  const texto = mensagem.value.trim()
  if (!texto || props.loading) {
    return
  }

  emit('submit', texto)
  mensagem.value = ''
}
</script>

<template>
  <q-form class="column q-gutter-sm" @submit.prevent="enviar">
    <q-input
      v-model="mensagem"
      type="textarea"
      outlined
      autogrow
      maxlength="2000"
      counter
      label="Novo comentário"
      :disable="props.loading"
      :rules="[(v) => !!String(v || '').trim() || 'Informe um comentário']"
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
