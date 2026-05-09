<script setup lang="ts">
import { computed, ref } from 'vue'

const props = withDefaults(
  defineProps<{
    loading?: boolean
    titulo?: string
    multiple?: boolean
    extensoesPermitidas?: string[]
    tamanhoMaximoBytes?: number | null
  }>(),
  {
    loading: false,
    titulo: 'Anexar arquivo',
    multiple: true,
    extensoesPermitidas: () => ['.pdf', '.png', '.jpg', '.jpeg', '.txt', '.doc', '.docx', '.xls', '.xlsx'],
    tamanhoMaximoBytes: null,
  }
)

const emit = defineEmits<{
  upload: [files: File[]]
  invalid: [message: string]
}>()

const arquivosSelecionados = ref<File[] | null>(null)
const mensagemErro = ref<string | null>(null)

const accept = computed(() =>
  props.extensoesPermitidas
    .map((extensao) => extensao.trim().toLowerCase())
    .filter((extensao) => extensao.startsWith('.'))
    .join(',')
)

function obterExtensao(nomeArquivo: string): string {
  const indice = nomeArquivo.lastIndexOf('.')
  return indice < 0 ? '' : nomeArquivo.slice(indice).toLowerCase()
}

function enviarArquivos(): void {
  const arquivos = arquivosSelecionados.value

  if (!arquivos?.length || props.loading) {
    return
  }

  const extensoes = new Set(props.extensoesPermitidas.map((item) => item.trim().toLowerCase()))
  const validos: File[] = []

  for (const arquivo of arquivos) {
    const extensao = obterExtensao(arquivo.name)
    if (!extensao || !extensoes.has(extensao)) {
      mensagemErro.value = `Arquivo invalido (${arquivo.name}). Extensao nao permitida.`
      emit('invalid', mensagemErro.value)
      continue
    }

    if (props.tamanhoMaximoBytes && arquivo.size > props.tamanhoMaximoBytes) {
      mensagemErro.value = `Arquivo invalido (${arquivo.name}). Tamanho excede o limite permitido.`
      emit('invalid', mensagemErro.value)
      continue
    }

    validos.push(arquivo)
  }

  if (!validos.length) {
    return
  }

  mensagemErro.value = null
  emit('upload', validos)
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
      :accept="accept"
      :rules="[(v) => (v && v.length > 0) || 'Selecione ao menos um arquivo']"
    >
      <template #prepend>
        <q-icon name="attach_file" />
      </template>
    </q-file>

    <q-banner v-if="mensagemErro" dense rounded class="bg-amber-2 text-dark">
      {{ mensagemErro }}
    </q-banner>

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
