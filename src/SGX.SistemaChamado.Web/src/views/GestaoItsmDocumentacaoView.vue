<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import {
  categoriasDocumentosItsm,
  documentosItsm,
  filtrarDocumentosItsm,
  markdownItsmParaHtml,
  type CategoriaDocumentoItsm,
  type DocumentoItsm,
} from '../content/gestaoItsmDocs'

const router = useRouter()

const busca = ref('')
const categoriaSelecionada = ref<CategoriaDocumentoItsm | 'Todas'>('Todas')
const documentoSelecionadoId = ref(documentosItsm[0]?.id ?? '')

const opcoesCategoria = computed(() => [
  { label: 'Todas as categorias', value: 'Todas' },
  ...categoriasDocumentosItsm.map((categoria) => ({
    label: categoria,
    value: categoria,
  })),
])

const documentosFiltrados = computed(() =>
  filtrarDocumentosItsm(documentosItsm, busca.value, categoriaSelecionada.value)
)

const documentoSelecionado = computed<DocumentoItsm | null>(() => {
  return (
    documentosFiltrados.value.find((documento) => documento.id === documentoSelecionadoId.value) ??
    documentosFiltrados.value[0] ??
    null
  )
})

const conteudoDocumentoHtml = computed(() =>
  documentoSelecionado.value ? markdownItsmParaHtml(documentoSelecionado.value.conteudo) : ''
)
const totalTagsSelecionadas = computed(() => documentoSelecionado.value?.tags.length ?? 0)

watch(documentosFiltrados, (documentos) => {
  if (!documentos.length) {
    return
  }

  const documentoAtualEstaVisivel = documentos.some((documento) => documento.id === documentoSelecionadoId.value)
  if (!documentoAtualEstaVisivel) {
    documentoSelecionadoId.value = documentos[0].id
  }
})

function selecionarDocumento(documento: DocumentoItsm): void {
  documentoSelecionadoId.value = documento.id
}

function abrirRoadmap(): void {
  router.push('/admin/gestao-itsm/roadmap')
}
</script>

<template>
  <q-page class="sgx-page gestao-itsm-docs column q-gutter-md">
    <PageHeader
      contexto="Gestao ITSM e conhecimento"
      titulo="Documentacao ITSM"
      subtitulo="Consulte documentacao funcional, tecnica e de homologacao do SGX Sistema de Chamados."
    >
      <template #actions>
        <q-btn color="primary" icon="account_tree" label="Ver Roadmap ITSM" @click="abrirRoadmap" />
      </template>
    </PageHeader>

    <div class="sgx-kpi-grid">
      <MetricCard title="Documentos filtrados" :value="documentosFiltrados.length" icon="article" tone="primary" />
      <MetricCard title="Categoria selecionada" :value="categoriaSelecionada" icon="category" tone="info" />
      <MetricCard title="Tags do documento" :value="totalTagsSelecionadas" icon="sell" tone="warning" />
    </div>

    <AppSectionCard titulo="Filtro de documentacao" subtitulo="Busque por titulo, conteudo, categoria e tags.">
      <FilterBar compact>
        <section class="gestao-itsm-docs__toolbar">
          <q-input
            v-model="busca"
            outlined
            dense
            clearable
            class="gestao-itsm-docs__busca"
            placeholder="Buscar por titulo, conteudo, categoria ou tag"
          >
            <template #prepend>
              <q-icon name="search" />
            </template>
          </q-input>

          <q-select
            v-model="categoriaSelecionada"
            outlined
            dense
            emit-value
            map-options
            class="gestao-itsm-docs__categoria"
            :options="opcoesCategoria"
            label="Categoria"
          />
        </section>
      </FilterBar>
    </AppSectionCard>

    <section class="gestao-itsm-docs__layout">
      <aside class="gestao-itsm-docs__lista" aria-label="Documentos ITSM">
        <div class="gestao-itsm-docs__lista-header">
          <div class="text-subtitle2 text-weight-bold">Documentos</div>
          <q-badge color="primary">{{ documentosFiltrados.length }}</q-badge>
        </div>

        <q-list separator>
          <q-item
            v-for="documento in documentosFiltrados"
            :key="documento.id"
            clickable
            :active="documentoSelecionadoId === documento.id"
            active-class="gestao-itsm-docs__item--ativo"
            @click="selecionarDocumento(documento)"
          >
            <q-item-section>
              <q-item-label class="text-weight-medium">{{ documento.titulo }}</q-item-label>
              <q-item-label caption lines="2">{{ documento.resumo }}</q-item-label>
              <div class="q-mt-sm">
                <q-badge color="blue-grey-7">{{ documento.categoria }}</q-badge>
              </div>
            </q-item-section>
          </q-item>
        </q-list>

        <div v-if="!documentosFiltrados.length" class="gestao-itsm-docs__vazio">
          <q-icon name="search_off" size="32px" />
          <div class="text-subtitle2">Nenhum documento encontrado</div>
          <div class="text-caption sgx-muted">Ajuste a busca ou a categoria selecionada.</div>
        </div>
      </aside>

      <article v-if="documentoSelecionado" class="gestao-itsm-docs__leitor">
        <header class="gestao-itsm-docs__leitor-header">
          <div>
            <q-badge color="primary" class="q-mb-sm">{{ documentoSelecionado.categoria }}</q-badge>
            <h1>{{ documentoSelecionado.titulo }}</h1>
            <p>{{ documentoSelecionado.resumo }}</p>
          </div>
          <div v-if="documentoSelecionado.atualizadoEm" class="gestao-itsm-docs__data">
            Atualizado em {{ documentoSelecionado.atualizadoEm }}
          </div>
        </header>

        <div class="gestao-itsm-docs__tags">
          <q-chip
            v-for="tag in documentoSelecionado.tags"
            :key="tag"
            dense
            square
            color="grey-2"
            text-color="blue-grey-9"
          >
            {{ tag }}
          </q-chip>
        </div>

        <q-separator class="q-my-md" />

        <div class="gestao-itsm-docs__markdown" v-html="conteudoDocumentoHtml" />
      </article>
    </section>
  </q-page>
</template>

<style scoped>
.gestao-itsm-docs {
  color: var(--sgx-text);
}

.gestao-itsm-docs__toolbar {
  display: grid;
  grid-template-columns: minmax(0, 1fr) minmax(220px, 320px);
  gap: 12px;
}

.gestao-itsm-docs__layout {
  display: grid;
  grid-template-columns: minmax(280px, 360px) minmax(0, 1fr);
  gap: 16px;
  align-items: start;
}

.gestao-itsm-docs__lista,
.gestao-itsm-docs__leitor {
  background: #ffffff;
  border: 1px solid var(--sgx-border);
  border-radius: 8px;
  box-shadow: 0 8px 22px rgba(15, 23, 42, 0.06);
}

.gestao-itsm-docs__lista {
  overflow: hidden;
}

.gestao-itsm-docs__lista-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  padding: 16px;
  border-bottom: 1px solid var(--sgx-border);
}

.gestao-itsm-docs__item--ativo {
  background: rgba(11, 94, 215, 0.1);
  color: var(--sgx-primary);
}

.gestao-itsm-docs__vazio {
  display: grid;
  gap: 8px;
  justify-items: center;
  padding: 28px 16px;
  text-align: center;
}

.gestao-itsm-docs__leitor {
  padding: 22px;
}

.gestao-itsm-docs__leitor-header {
  display: grid;
  grid-template-columns: minmax(0, 1fr) auto;
  gap: 16px;
  align-items: start;
}

.gestao-itsm-docs__leitor-header h1 {
  margin: 0;
  font-size: 1.35rem;
  line-height: 1.25;
  font-weight: 800;
  letter-spacing: 0;
}

.gestao-itsm-docs__leitor-header p {
  margin: 8px 0 0;
  color: var(--sgx-muted);
}

.gestao-itsm-docs__data {
  color: var(--sgx-muted);
  font-size: 0.82rem;
  white-space: nowrap;
}

.gestao-itsm-docs__tags {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-top: 14px;
}

.gestao-itsm-docs__markdown {
  color: #111827;
  font-size: 0.98rem;
  line-height: 1.7;
}

.gestao-itsm-docs__markdown :deep(h1) {
  margin: 0 0 14px;
  font-size: 1.25rem;
  line-height: 1.3;
  font-weight: 800;
  letter-spacing: 0;
}

.gestao-itsm-docs__markdown :deep(h2) {
  margin: 22px 0 8px;
  font-size: 1rem;
  line-height: 1.35;
  font-weight: 800;
  letter-spacing: 0;
}

.gestao-itsm-docs__markdown :deep(p) {
  margin: 0 0 12px;
}

.gestao-itsm-docs__markdown :deep(ul) {
  margin: 0 0 14px;
  padding-left: 22px;
}

.gestao-itsm-docs__markdown :deep(li) {
  margin: 5px 0;
}

.gestao-itsm-docs__markdown :deep(code) {
  padding: 2px 5px;
  border-radius: 4px;
  background: #edf2f7;
  color: #0f172a;
  font-size: 0.9em;
}

@media (max-width: 1024px) {
  .gestao-itsm-docs__layout,
  .gestao-itsm-docs__toolbar {
    grid-template-columns: minmax(0, 1fr);
  }

  .gestao-itsm-docs__leitor-header {
    grid-template-columns: minmax(0, 1fr);
  }

  .gestao-itsm-docs__data {
    white-space: normal;
  }
}
</style>
