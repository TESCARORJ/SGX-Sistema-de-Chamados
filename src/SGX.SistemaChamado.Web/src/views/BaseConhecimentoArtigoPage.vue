<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { HttpRequestError } from '../services/httpClient'
import { baseConhecimentoPortalService } from '../services/baseConhecimentoPortalService'
import type { PortalBaseConhecimentoArtigoDetalhe } from '../types/baseConhecimento'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const artigo = ref<PortalBaseConhecimentoArtigoDetalhe | null>(null)

const slug = computed(() => String(route.params.slug ?? '').trim())

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function extrairTags(tags: string | null): string[] {
  if (!tags?.trim()) {
    return []
  }

  return tags
    .split(',')
    .map((item) => item.trim())
    .filter(Boolean)
}

function extrairMensagemErro(error: unknown): string {
  if (error instanceof HttpRequestError && error.status === 404) {
    return 'Artigo indisponivel ou nao encontrado para o seu perfil de acesso.'
  }

  if (error instanceof Error) {
    const mensagem = error.message
    const jsonStart = mensagem.indexOf('{')

    if (jsonStart >= 0) {
      const trechoJson = mensagem.slice(jsonStart)
      try {
        const parsed = JSON.parse(trechoJson) as { mensagem?: string }
        if (parsed?.mensagem) {
          return parsed.mensagem
        }
      } catch {
        return mensagem
      }
    }

    return mensagem
  }

  return 'Nao foi possivel carregar o artigo selecionado.'
}

async function carregar(): Promise<void> {
  if (!slug.value) {
    erro.value = 'Slug do artigo nao informado.'
    return
  }

  loading.value = true
  erro.value = null

  try {
    artigo.value = await baseConhecimentoPortalService.obterArtigoPorSlug(slug.value)
  } catch (error) {
    artigo.value = null
    erro.value = extrairMensagemErro(error)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Base de conhecimento"
      subtitulo="Detalhe do artigo publicado para consulta no portal."
    >
      <template #actions>
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/portal/base-conhecimento')" />
      </template>
    </PageHeader>

    <LoadingState v-if="loading" inline mensagem="Carregando artigo..." />

    <ErrorState
      v-else-if="erro"
      titulo="Nao foi possivel abrir o artigo"
      :mensagem="erro"
      @retry="carregar"
    />

    <AppSectionCard v-else-if="artigo" :titulo="artigo.titulo" :subtitulo="artigo.categoriaNome || 'Sem categoria'">
      <div class="text-caption text-grey-7 q-mb-md">Publicado em {{ formatarData(artigo.publicadoEm) }}</div>

      <q-banner v-if="artigo.resumo" rounded class="bg-blue-1 text-primary q-mb-md">
        {{ artigo.resumo }}
      </q-banner>

      <div class="conteudo-artigo">{{ artigo.conteudo }}</div>

      <div v-if="extrairTags(artigo.tags).length" class="row q-gutter-xs q-mt-md">
        <q-chip v-for="tag in extrairTags(artigo.tags)" :key="tag" dense square color="grey-3" text-color="grey-9">
          {{ tag }}
        </q-chip>
      </div>
    </AppSectionCard>
  </q-page>
</template>

<style scoped>
.conteudo-artigo {
  white-space: pre-wrap;
  line-height: 1.6;
}

:deep(.conteudo-artigo p) {
  margin: 0 0 0.75rem;
}
</style>
