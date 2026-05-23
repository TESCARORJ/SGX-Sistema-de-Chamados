<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { baseConhecimentoPortalService } from '../services/baseConhecimentoPortalService'
import { portalService } from '../services/portalService'
import type { CategoriaPortal } from '../types/portal'
import type { PortalBaseConhecimentoArtigoListagem } from '../types/baseConhecimento'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const artigos = ref<PortalBaseConhecimentoArtigoListagem[]>([])
const categorias = ref<CategoriaPortal[]>([])

const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(12)

const filtros = reactive({
  termo: '',
  categoriaId: '',
})

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamanhoPagina.value)))

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (!(error instanceof Error)) {
    return fallback
  }

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

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleDateString('pt-BR')
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

async function carregarCategorias(): Promise<void> {
  const contexto = await portalService.getPortalContexto()
  categorias.value = contexto.categorias
}

async function carregarArtigos(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await baseConhecimentoPortalService.listarArtigos({
      termo: filtros.termo.trim() || undefined,
      categoriaId: filtros.categoriaId || undefined,
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    })

    artigos.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar a base de conhecimento.')
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  pagina.value = 1
  await carregarArtigos()
}

async function limparFiltros(): Promise<void> {
  filtros.termo = ''
  filtros.categoriaId = ''
  pagina.value = 1
  await carregarArtigos()
}

async function alterarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarArtigos()
}

function abrirArtigo(slug: string): void {
  router.push(`/portal/base-conhecimento/${slug}`)
}

onMounted(async () => {
  loading.value = true
  erro.value = null

  try {
    await carregarCategorias()
    await carregarArtigos()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar dados da base de conhecimento.')
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Base de conhecimento"
      subtitulo="Consulte orientacoes e artigos publicados para resolver duvidas comuns com mais rapidez."
    />

    <AppSectionCard titulo="Busca" subtitulo="Use termo e categoria para encontrar o artigo certo.">
      <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltros">
        <div class="col-12 col-md-6">
          <q-input
            v-model="filtros.termo"
            outlined
            label="Buscar"
            placeholder="Titulo, resumo, conteudo ou tags"
            :disable="loading"
          />
        </div>

        <div class="col-12 col-md-4">
          <q-select
            v-model="filtros.categoriaId"
            outlined
            clearable
            emit-value
            map-options
            label="Categoria"
            :disable="loading"
            :options="categorias.map((item) => ({ label: item.nome, value: item.id }))"
          />
        </div>

        <div class="col-12 col-md-2 row items-center justify-end q-gutter-sm">
          <q-btn type="submit" color="primary" icon="search" label="Buscar" :loading="loading" />
          <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
        </div>
      </q-form>
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregarArtigos" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando artigos publicados..." />

    <AppSectionCard v-else titulo="Artigos disponiveis" :subtitulo="`Total encontrado: ${total}`">
      <EmptyState
        v-if="!artigos.length"
        titulo="Nenhum artigo encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="article"
      />

      <template v-else>
        <div class="row q-col-gutter-md">
          <div v-for="artigo in artigos" :key="artigo.id" class="col-12 col-md-6 col-lg-4">
            <q-card flat bordered class="sgx-card card-artigo full-height">
              <q-card-section>
                <div class="text-h6 ellipsis-2-lines">{{ artigo.titulo }}</div>
                <div class="text-caption text-grey-7 q-mt-xs">
                  {{ artigo.categoriaNome || 'Sem categoria' }}
                </div>
                <div class="text-caption text-grey-7">Publicado em {{ formatarData(artigo.publicadoEm) }}</div>
              </q-card-section>

              <q-card-section class="q-pt-none">
                <div class="text-body2 text-grey-8 ellipsis-3-lines">
                  {{ artigo.resumo || 'Sem resumo cadastrado para este artigo.' }}
                </div>
              </q-card-section>

              <q-card-section v-if="extrairTags(artigo.tags).length" class="q-pt-none">
                <div class="row q-gutter-xs">
                  <q-chip
                    v-for="tag in extrairTags(artigo.tags)"
                    :key="tag"
                    dense
                    square
                    color="blue-1"
                    text-color="primary"
                  >
                    {{ tag }}
                  </q-chip>
                </div>
              </q-card-section>

              <q-space />

              <q-card-actions align="right">
                <q-btn flat color="primary" icon="article" label="Ler artigo" @click="abrirArtigo(artigo.slug)" />
              </q-card-actions>
            </q-card>
          </div>
        </div>

        <div class="row justify-end q-mt-md">
          <q-pagination
            v-model="pagina"
            color="primary"
            :max="totalPaginas"
            :max-pages="8"
            boundary-links
            direction-links
            @update:model-value="alterarPagina"
          />
        </div>
      </template>
    </AppSectionCard>
  </q-page>
</template>

<style scoped>
.card-artigo {
  display: flex;
  flex-direction: column;
  min-height: 280px;
}

.ellipsis-2-lines {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.ellipsis-3-lines {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
