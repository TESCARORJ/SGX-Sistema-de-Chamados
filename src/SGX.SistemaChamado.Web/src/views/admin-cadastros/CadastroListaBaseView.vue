<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { QTableColumn } from 'quasar'
import CampoAtivoInativo from '../../components/admin/cadastros/CampoAtivoInativo.vue'
import CampoBuscaCadastro from '../../components/admin/cadastros/CampoBuscaCadastro.vue'
import PaginacaoTabela from '../../components/admin/cadastros/PaginacaoTabela.vue'
import TabelaAdministrativa from '../../components/admin/cadastros/TabelaAdministrativa.vue'
import AppSectionCard from '../../components/ui/AppSectionCard.vue'
import EmptyState from '../../components/ui/EmptyState.vue'
import ErrorState from '../../components/ui/ErrorState.vue'
import LoadingState from '../../components/ui/LoadingState.vue'
import PageHeader from '../../components/ui/PageHeader.vue'
import { permissoes } from '../../constants/permissoes'
import { cadastrosAdminService } from '../../services/cadastrosAdminService'
import { parametrosSistemaService } from '../../services/parametrosSistemaService'
import { usuariosAdminService } from '../../services/usuariosAdminService'
import { useAuthStore } from '../../stores/authStore'
import type { FiltroCadastroRequest, PagedResultResponse } from '../../types/adminCadastros'

type Entidade = 'usuarios' | 'perfis' | 'departamentos' | 'categorias' | 'prioridades' | 'status' | 'parametros'

const props = defineProps<{
  titulo: string
  entidade: Entidade
  detalhePathBase: string
  colunas: QTableColumn[]
}>()

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const texto = ref('')
const filtroAtivo = ref<'todos' | 'ativos' | 'inativos'>('ativos')
const pagina = ref(1)
const tamanhoPagina = ref(20)
const total = ref(0)
const rows = ref<unknown[]>([])

const isAdmin = computed(() => authStore.usuario?.perfis.includes('Administrador') ?? false)
const temRegistros = computed(() => rows.value.length > 0)
const filtrosAplicados = computed(() => Boolean(texto.value.trim()) || filtroAtivo.value !== 'ativos')
const podeCriar = computed(() => {
  switch (props.entidade) {
    case 'usuarios':
      return authStore.possuiPermissao(permissoes.usuariosGerenciar)
    case 'perfis':
      return authStore.possuiPermissao(permissoes.perfisGerenciar)
    case 'parametros':
      return authStore.possuiPermissao(permissoes.parametrosGerenciar)
    default:
      return isAdmin.value
  }
})
const podeDetalhar = computed(() => {
  switch (props.entidade) {
    case 'usuarios':
      return authStore.possuiAlgumaPermissao([permissoes.usuariosVisualizar, permissoes.usuariosGerenciar])
    case 'perfis':
      return authStore.possuiAlgumaPermissao([permissoes.perfisVisualizar, permissoes.perfisGerenciar])
    case 'parametros':
      return authStore.possuiAlgumaPermissao([permissoes.parametrosVisualizar, permissoes.parametrosGerenciar])
    default:
      return true
  }
})

function montarFiltro(): FiltroCadastroRequest {
  return {
    texto: texto.value || undefined,
    ativo: filtroAtivo.value === 'todos' ? undefined : filtroAtivo.value === 'ativos',
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
  }
}

async function listarComServico(filtro: FiltroCadastroRequest): Promise<PagedResultResponse<unknown>> {
  switch (props.entidade) {
    case 'usuarios':
      return usuariosAdminService.listar(filtro)
    case 'perfis':
      return cadastrosAdminService.listarPerfis(filtro)
    case 'departamentos':
      return cadastrosAdminService.listarDepartamentos(filtro)
    case 'categorias':
      return cadastrosAdminService.listarCategorias(filtro)
    case 'prioridades':
      return cadastrosAdminService.listarPrioridades(filtro)
    case 'status':
      return cadastrosAdminService.listarStatus(filtro)
    case 'parametros':
      return parametrosSistemaService.listar(filtro)
  }
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await listarComServico(montarFiltro())
    rows.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  } finally {
    loading.value = false
  }
}

function abrirDetalhe(row: unknown): void {
  const id = (row as { id?: string }).id
  if (!id) {
    return
  }

  router.push(`${props.detalhePathBase}/${id}`)
}

function novo(): void {
  router.push(`${props.detalhePathBase}/novo`)
}

function aplicarFiltros(): void {
  pagina.value = 1
  void carregar()
}

function limparFiltros(): void {
  texto.value = ''
  filtroAtivo.value = 'ativos'
  pagina.value = 1
  void carregar()
}

function atualizarPagina(value: number): void {
  pagina.value = value
  void carregar()
}

function atualizarTamanhoPagina(value: number): void {
  tamanhoPagina.value = value
  pagina.value = 1
  void carregar()
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader :titulo="titulo" subtitulo="Lista administrativa com filtros, status e paginação">
      <template #actions>
        <q-btn
          v-if="podeCriar"
          color="primary"
          icon="add"
          label="Novo"
          :disable="loading"
          @click="novo"
        />
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros" subtitulo="Refine os resultados por busca textual e situação">
      <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-7">
            <CampoBuscaCadastro v-model="texto" :loading="loading" />
          </div>
          <div class="col-12 col-md-5">
            <CampoAtivoInativo v-model="filtroAtivo" :loading="loading" />
          </div>
        </div>

        <div class="row justify-end q-gutter-sm">
          <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" />
          <q-btn flat label="Limpar" :disable="loading" @click="limparFiltros" />
        </div>
      </q-form>
    </AppSectionCard>

    <q-banner v-if="erro && temRegistros" rounded class="bg-red-1 text-negative">
      {{ erro }}
    </q-banner>

    <LoadingState v-if="loading && !temRegistros" mensagem="Carregando lista administrativa..." />

    <q-banner v-else-if="!podeDetalhar" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar esta listagem.
    </q-banner>

    <ErrorState
      v-else-if="erro && !temRegistros"
      titulo="Não foi possível carregar a listagem"
      :mensagem="erro"
      @retry="carregar"
    />

    <EmptyState
      v-else-if="!temRegistros"
      titulo="Nenhum registro encontrado"
      mensagem="Nenhum resultado corresponde aos filtros aplicados."
      icon="search_off"
    >
      <template #actions>
        <q-btn
          v-if="filtrosAplicados"
          flat
          color="primary"
          icon="filter_alt_off"
          label="Limpar filtros"
          @click="limparFiltros"
        />
      </template>
    </EmptyState>

    <AppSectionCard v-else :titulo="titulo" subtitulo="Resultados da listagem administrativa">
      <TabelaAdministrativa :title="titulo" :rows="rows" :columns="colunas" :loading="loading">
        <template #acoes="{ row }">
          <q-btn v-if="podeDetalhar" flat dense icon="visibility" label="Detalhar" @click="abrirDetalhe(row)" />
        </template>
      </TabelaAdministrativa>

      <q-separator class="q-my-md" />

      <PaginacaoTabela
        :pagina="pagina"
        :tamanho-pagina="tamanhoPagina"
        :total="total"
        :loading="loading"
        @update:pagina="atualizarPagina"
        @update:tamanho-pagina="atualizarTamanhoPagina"
      />
    </AppSectionCard>
  </q-page>
</template>
