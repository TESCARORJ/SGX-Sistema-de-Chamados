<script setup lang="ts">
import { ref } from 'vue'

const props = withDefaults(
  defineProps<{
    loading?: boolean
    titulo?: string
    multiple?: boolean
  }>(),
  {
    loading: false,
    titulo: 'Anexar arquivo',
    multiple: true,
  }
)

const emit = defineEmits<{
  upload: [files: File[]]
}>()

const arquivosSelecionados = ref<File[] | null>(null)

function enviarArquivos(): void {
  const arquivos = arquivosSelecionados.value

  if (!arquivos?.length || props.loading) {
    return
  }

  emit('upload', [...arquivos])
  arquivosSelecionados.value = null
}
</script>

<template>
  <q-form class="column q-gutter-sm" @submit.prevent="enviarArquivos">
    <q-file
      v-model="arquivosSelecionados"
      :label="props.titulo"
      outlined
      use-chips
      clearable
      counter
      :multiple="props.multiple"
      :disable="props.loading"
      accept=".pdf,.png,.jpg,.jpeg,.doc,.docx,.xls,.xlsx,.txt,.zip"
      :rules="[(v) => (v && v.length > 0) || 'Selecione ao menos um arquivo']"
    >
      <template #prepend>
        <q-icon name="attach_file" />
      </template>
    </q-file>

    <div class="row justify-end">
      <q-btn
        type="submit"
        color="secondary"
        icon="cloud_upload"
        label="Enviar anexo"
        :loading="props.loading"
        :disable="props.loading || !(arquivosSelecionados && arquivosSelecionados.length)"
      />
    </div>
  </q-form>
</template>
