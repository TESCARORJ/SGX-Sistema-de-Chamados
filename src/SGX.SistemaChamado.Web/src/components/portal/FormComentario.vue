<script setup lang="ts">
import { ref } from 'vue'

const emit = defineEmits<{
  submit: [mensagem: string]
}>()

const mensagem = ref('')

function enviar() {
  const texto = mensagem.value.trim()
  if (!texto) {
    return
  }

  emit('submit', texto)
  mensagem.value = ''
}
</script>

<template>
  <div class="column q-gutter-sm">
    <q-input v-model="mensagem" type="textarea" outlined autogrow label="Novo comentário" />
    <div>
      <q-btn color="primary" label="Adicionar comentário" @click="enviar" />
    </div>
  </div>
</template>
