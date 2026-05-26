<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
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
      contexto="Autoatendimento"
      titulo="Base de conhecimento"
      subtitulo="Detalhe do artigo publicado para consulta no portal."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/portal/base-conhecimento')" />
          <q-btn flat color="primary" icon="inventory_2" label="Catalogo" @click="router.push('/portal/catalogo-servicos')" />
        </div>
      </template>
    </PageHeader>

    <LoadingState v-if="loading" inline mensagem="Carregando artigo..." />

    <ErrorState
      v-else-if="erro"
      titulo="Nao foi possivel abrir o artigo"
      :mensagem="erro"
      @retry="carregar"
    />

    <template v-else-if="artigo">
      <AppSectionCard :titulo="artigo.titulo" :subtitulo="artigo.categoriaNome || 'Sem categoria'">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-7">
            <q-banner v-if="artigo.resumo" rounded class="bg-blue-1 text-primary">
              {{ artigo.resumo }}
            </q-banner>
            <q-banner v-else rounded class="bg-grey-2 text-grey-8">
              Este artigo nao possui resumo cadastrado.
            </q-banner>
          </div>
          <div class="col-12 col-md-5">
            <q-list bordered separator class="rounded-borders">
              <q-item>
                <q-item-section>
                  <q-item-label caption>Publicado em</q-item-label>
                  <q-item-label>{{ formatarData(artigo.publicadoEm) }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Ultima atualizacao</q-item-label>
                  <q-item-label>{{ formatarData(artigo.atualizadoEm) }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Categoria</q-item-label>
                  <q-item-label>{{ artigo.categoriaNome || 'Sem categoria' }}</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </div>
        </div>

        <div v-if="extrairTags(artigo.tags).length" class="row q-gutter-xs q-mt-md">
          <q-chip v-for="tag in extrairTags(artigo.tags)" :key="tag" dense square color="grey-3" text-color="grey-9">
            {{ tag }}
          </q-chip>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Conteudo do artigo" subtitulo="Orientacoes detalhadas para autoatendimento.">
        <EmptyState
          v-if="!artigo.conteudo?.trim()"
          titulo="Conteudo indisponivel"
          mensagem="Este artigo ainda nao possui conteudo publicado."
          icon="article"
        />
        <div v-else class="conteudo-artigo">{{ artigo.conteudo }}</div>
      </AppSectionCard>
    </template>
  </q-page>
</template>

<style scoped>
.conteudo-artigo {
  white-space: pre-wrap;
  line-height: 1.72;
  color: #1f2937;
}

:deep(.conteudo-artigo p) {
  margin: 0 0 0.75rem;
}
</style>
