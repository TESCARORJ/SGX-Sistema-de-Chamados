<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { catalogoServicosPortalService } from '../services/catalogoServicosPortalService'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, DepartamentoPortal } from '../types/portal'
import type { PortalCatalogoServicoListagem } from '../types/catalogoServicos'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const servicos = ref<PortalCatalogoServicoListagem[]>([])
const departamentos = ref<DepartamentoPortal[]>([])
const categorias = ref<CategoriaPortal[]>([])

const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(12)

const filtros = reactive({
  termo: '',
  departamentoResponsavelId: '',
  categoriaId: '',
  permiteAberturaChamado: 'todos' as 'todos' | 'sim' | 'nao',
})

const totalPaginas = computed(() => Math.max(1, Math.ceil(total.value / tamanhoPagina.value)))

const opcoesPermiteAbertura = [
  { label: 'Todos', value: 'todos' },
  { label: 'Permite abertura', value: 'sim' },
  { label: 'Nao permite abertura', value: 'nao' },
]

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

async function carregarContexto(): Promise<void> {
  const contexto = await portalService.getPortalContexto()
  departamentos.value = contexto.departamentos
  categorias.value = contexto.categorias
}

async function carregarServicos(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await catalogoServicosPortalService.listarServicos({
      termo: filtros.termo.trim() || undefined,
      departamentoResponsavelId: filtros.departamentoResponsavelId || undefined,
      categoriaId: filtros.categoriaId || undefined,
      permiteAberturaChamado:
        filtros.permiteAberturaChamado === 'todos' ? undefined : filtros.permiteAberturaChamado === 'sim',
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    })

    servicos.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar o catalogo de servicos.')
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  pagina.value = 1
  await carregarServicos()
}

async function limparFiltros(): Promise<void> {
  filtros.termo = ''
  filtros.departamentoResponsavelId = ''
  filtros.categoriaId = ''
  filtros.permiteAberturaChamado = 'todos'
  pagina.value = 1
  await carregarServicos()
}

async function alterarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarServicos()
}

function abrirDetalhe(slug: string): void {
  router.push(`/portal/catalogo-servicos/${slug}`)
}

function abrirChamado(servico: PortalCatalogoServicoListagem): void {
  if (!servico.permiteAberturaChamado) {
    return
  }

  router.push(`/portal/catalogo-servicos/${servico.slug}?acao=abrir-chamado`)
}

onMounted(async () => {
  loading.value = true
  erro.value = null

  try {
    await carregarContexto()
    await carregarServicos()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar dados do catalogo de servicos.')
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Catalogo de Servicos"
      subtitulo="Consulte os servicos institucionais por departamento e escolha o atendimento que melhor atende sua necessidade."
    />

    <AppSectionCard titulo="Busca" subtitulo="Pesquise por nome, descricao e filtros principais do servico.">
      <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltros">
        <div class="col-12 col-md-4">
          <q-input
            v-model="filtros.termo"
            outlined
            label="Buscar servico"
            placeholder="Nome, descricao ou instrucoes"
            :disable="loading"
          />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.departamentoResponsavelId"
            outlined
            clearable
            emit-value
            map-options
            label="Departamento responsavel"
            :disable="loading"
            :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
          />
        </div>

        <div class="col-12 col-md-3">
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

        <div class="col-12 col-md-2">
          <q-select
            v-model="filtros.permiteAberturaChamado"
            outlined
            emit-value
            map-options
            label="Abertura"
            :disable="loading"
            :options="opcoesPermiteAbertura"
          />
        </div>

        <div class="col-12 row justify-end q-gutter-sm">
          <q-btn type="submit" color="primary" icon="search" label="Buscar" :loading="loading" />
          <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
        </div>
      </q-form>
    </AppSectionCard>

    <ErrorState v-if="erro" titulo="Nao foi possivel consultar o catalogo" :mensagem="erro" @retry="carregarServicos" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando servicos publicados..." />

    <AppSectionCard v-else titulo="Servicos disponiveis" :subtitulo="`Total encontrado: ${total}`">
      <EmptyState
        v-if="!servicos.length"
        titulo="Nenhum servico encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="inventory_2"
      />

      <template v-else>
        <div class="row q-col-gutter-md">
          <div v-for="servico in servicos" :key="servico.id" class="col-12 col-md-6 col-lg-4">
            <q-card flat bordered class="sgx-card card-servico full-height">
              <q-card-section>
                <div class="text-h6 ellipsis-2-lines">{{ servico.nome }}</div>
                <div class="text-caption text-grey-7 q-mt-xs">
                  {{ servico.departamentoResponsavelNome || 'Departamento nao informado' }}
                </div>
                <div class="text-caption text-grey-7">
                  {{ servico.categoriaNome || 'Sem categoria' }}
                </div>
              </q-card-section>

              <q-card-section class="q-pt-none">
                <div class="text-body2 text-grey-8 ellipsis-3-lines">
                  {{ servico.descricao || 'Servico sem descricao complementar.' }}
                </div>
              </q-card-section>

              <q-card-section class="q-pt-none row items-center q-gutter-xs">
                <q-badge :color="servico.permiteAberturaChamado ? 'positive' : 'grey-6'" text-color="white">
                  {{ servico.permiteAberturaChamado ? 'Permite abertura de chamado' : 'Consulta informativa' }}
                </q-badge>
                <q-badge v-if="servico.requerAprovacao" color="orange-8" text-color="white">Requer aprovacao</q-badge>
              </q-card-section>

              <q-card-section class="q-pt-none text-caption text-grey-7">
                Publicado em {{ formatarData(servico.publicadoEm) }}
              </q-card-section>

              <q-space />

              <q-card-actions align="right">
                <q-btn flat color="primary" icon="visibility" label="Ver detalhes" @click="abrirDetalhe(servico.slug)" />
                <q-btn
                  flat
                  color="secondary"
                  icon="add_circle"
                  label="Abrir chamado"
                  :disable="!servico.permiteAberturaChamado"
                  @click="abrirChamado(servico)"
                />
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
.card-servico {
  display: flex;
  flex-direction: column;
  min-height: 300px;
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
