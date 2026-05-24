<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import PaginacaoTabela from '../components/admin/cadastros/PaginacaoTabela.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { catalogoServicosAdminService } from '../services/catalogoServicosAdminService'
import { cadastrosAdminService } from '../services/cadastrosAdminService'
import { useAuthStore } from '../stores/authStore'
import type {
  CategoriaChamadoResumoResponse,
  DepartamentoResumoResponse,
  SubcategoriaChamadoResumoResponse,
} from '../types/adminCadastros'
import {
  StatusCatalogoServico,
  VisibilidadeCatalogoServico,
  type CatalogoServicoListagem,
  type FiltroCatalogoServicoRequest,
} from '../types/catalogoServicos'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)
const servicos = ref<CatalogoServicoListagem[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)

const departamentos = ref<DepartamentoResumoResponse[]>([])
const categorias = ref<CategoriaChamadoResumoResponse[]>([])
const subcategorias = ref<SubcategoriaChamadoResumoResponse[]>([])

const confirmarAcaoAberto = ref(false)
const executandoAcao = ref(false)
const servicoAcao = ref<CatalogoServicoListagem | null>(null)
const tipoAcao = ref<'publicar' | 'arquivar' | 'reativar' | null>(null)

const filtros = reactive({
  termo: '',
  departamentoResponsavelId: '',
  categoriaId: '',
  subcategoriaId: '',
  status: '' as '' | StatusCatalogoServico,
  visibilidade: '' as '' | VisibilidadeCatalogoServico,
  ativo: 'todos' as 'todos' | 'ativos' | 'inativos',
  permiteAberturaChamado: 'todos' as 'todos' | 'sim' | 'nao',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.catalogoServicosVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.catalogoServicosGerenciar))
const podePublicar = computed(() => possuiPermissao(permissoes.catalogoServicosPublicar))
const podeArquivar = computed(() => possuiPermissao(permissoes.catalogoServicosArquivar))

const opcoesStatus = [
  { label: 'Rascunho', value: StatusCatalogoServico.Rascunho },
  { label: 'Publicado', value: StatusCatalogoServico.Publicado },
  { label: 'Arquivado', value: StatusCatalogoServico.Arquivado },
]

const opcoesVisibilidade = [
  { label: 'Interno', value: VisibilidadeCatalogoServico.Interno },
  { label: 'Solicitante', value: VisibilidadeCatalogoServico.Solicitante },
  { label: 'Atendente', value: VisibilidadeCatalogoServico.Atendente },
  { label: 'Administrador', value: VisibilidadeCatalogoServico.Administrador },
]

const opcoesAtivo = [
  { label: 'Todos', value: 'todos' },
  { label: 'Ativos', value: 'ativos' },
  { label: 'Inativos', value: 'inativos' },
]

const opcoesPermiteAbertura = [
  { label: 'Todos', value: 'todos' },
  { label: 'Permite', value: 'sim' },
  { label: 'Nao permite', value: 'nao' },
]

const colunas: QTableColumn<CatalogoServicoListagem>[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'departamento', label: 'Departamento responsavel', field: (row) => row.departamentoResponsavelNome || '-', align: 'left' },
  { name: 'categoria', label: 'Categoria', field: (row) => row.categoriaNome || '-', align: 'left' },
  { name: 'status', label: 'Status', field: 'statusDescricao', align: 'left' },
  { name: 'visibilidade', label: 'Visibilidade', field: 'visibilidadeDescricao', align: 'left' },
  { name: 'permite', label: 'Permite abertura', field: 'permiteAberturaChamado', align: 'center' },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left' },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
]

const tituloConfirmacao = computed(() => {
  if (tipoAcao.value === 'publicar') return 'Confirmar publicacao'
  if (tipoAcao.value === 'arquivar') return 'Confirmar arquivamento'
  return 'Confirmar reativacao'
})

const mensagemConfirmacao = computed(() => {
  const nomeServico = servicoAcao.value?.nome ?? 'este servico'
  if (tipoAcao.value === 'publicar') return `Deseja publicar o servico "${nomeServico}"?`
  if (tipoAcao.value === 'arquivar') return `Deseja arquivar o servico "${nomeServico}"?`
  return `Deseja reativar o servico "${nomeServico}"?`
})

const labelConfirmacao = computed(() => {
  if (tipoAcao.value === 'publicar') return 'Publicar'
  if (tipoAcao.value === 'arquivar') return 'Arquivar'
  return 'Reativar'
})

const corConfirmacao = computed(() => {
  if (tipoAcao.value === 'publicar') return 'positive'
  if (tipoAcao.value === 'arquivar') return 'negative'
  return 'primary'
})

const subcategoriasFiltradas = computed(() => {
  if (!filtros.categoriaId) {
    return subcategorias.value
  }

  return subcategorias.value.filter((item) => item.categoriaChamadoId === filtros.categoriaId)
})

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

  return new Date(data).toLocaleString('pt-BR')
}

function corStatus(status: StatusCatalogoServico): string {
  switch (status) {
    case StatusCatalogoServico.Publicado:
      return 'positive'
    case StatusCatalogoServico.Arquivado:
      return 'negative'
    default:
      return 'grey-7'
  }
}

function corVisibilidade(visibilidade: VisibilidadeCatalogoServico): string {
  switch (visibilidade) {
    case VisibilidadeCatalogoServico.Administrador:
      return 'deep-orange'
    case VisibilidadeCatalogoServico.Atendente:
      return 'indigo'
    case VisibilidadeCatalogoServico.Solicitante:
      return 'teal'
    default:
      return 'blue-grey'
  }
}

function podeMostrarAcaoPublicar(servico: CatalogoServicoListagem): boolean {
  if (!podePublicar.value) {
    return false
  }

  return servico.status !== StatusCatalogoServico.Publicado && servico.status !== StatusCatalogoServico.Arquivado
}

function podeMostrarAcaoArquivar(servico: CatalogoServicoListagem): boolean {
  if (!podeArquivar.value) {
    return false
  }

  return servico.status !== StatusCatalogoServico.Arquivado
}

function podeMostrarAcaoReativar(servico: CatalogoServicoListagem): boolean {
  return podeArquivar.value && servico.status === StatusCatalogoServico.Arquivado
}

function abrirNovoServico(): void {
  router.push('/admin/conhecimento/catalogo-servicos/novo')
}

function editarServico(servicoId: string): void {
  router.push(`/admin/conhecimento/catalogo-servicos/${servicoId}`)
}

function montarFiltroRequest(): FiltroCatalogoServicoRequest {
  return {
    termo: filtros.termo.trim() || undefined,
    departamentoResponsavelId: filtros.departamentoResponsavelId || undefined,
    categoriaId: filtros.categoriaId || undefined,
    subcategoriaId: filtros.subcategoriaId || undefined,
    status: filtros.status || undefined,
    visibilidade: filtros.visibilidade || undefined,
    ativo: filtros.ativo === 'todos' ? undefined : filtros.ativo === 'ativos',
    permiteAberturaChamado:
      filtros.permiteAberturaChamado === 'todos' ? undefined : filtros.permiteAberturaChamado === 'sim',
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
    ordenarPor: 'atualizadoEm',
    direcaoOrdenacao: 'desc',
  }
}

async function carregarReferencias(): Promise<void> {
  const [departamentosResponse, categoriasResponse, subcategoriasResponse] = await Promise.all([
    cadastrosAdminService.listarDepartamentos({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarSubcategorias({ ativo: true, tamanhoPagina: 200 }),
  ])

  departamentos.value = departamentosResponse.items
  categorias.value = categoriasResponse.items
  subcategorias.value = subcategoriasResponse.items
}

async function carregarServicos(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const response = await catalogoServicosAdminService.listarServicos(montarFiltroRequest())
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
  filtros.subcategoriaId = ''
  filtros.status = ''
  filtros.visibilidade = ''
  filtros.ativo = 'todos'
  filtros.permiteAberturaChamado = 'todos'
  pagina.value = 1
  await carregarServicos()
}

async function atualizarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarServicos()
}

async function atualizarTamanhoPagina(value: number): Promise<void> {
  tamanhoPagina.value = value
  pagina.value = 1
  await carregarServicos()
}

function abrirConfirmacao(servico: CatalogoServicoListagem, acao: 'publicar' | 'arquivar' | 'reativar'): void {
  servicoAcao.value = servico
  tipoAcao.value = acao
  confirmarAcaoAberto.value = true
}

async function executarAcao(): Promise<void> {
  if (!servicoAcao.value || !tipoAcao.value) {
    return
  }

  executandoAcao.value = true

  try {
    if (tipoAcao.value === 'publicar') {
      await catalogoServicosAdminService.publicarServico(servicoAcao.value.id)
      $q.notify({ type: 'positive', message: 'Servico publicado com sucesso.' })
    }

    if (tipoAcao.value === 'arquivar') {
      const response = await catalogoServicosAdminService.arquivarServico(servicoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Servico arquivado com sucesso.' })
    }

    if (tipoAcao.value === 'reativar') {
      const response = await catalogoServicosAdminService.reativarServico(servicoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Servico reativado com sucesso.' })
    }

    confirmarAcaoAberto.value = false
    servicoAcao.value = null
    tipoAcao.value = null
    await carregarServicos()
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel executar a acao selecionada.')
    erro.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    executandoAcao.value = false
  }
}

watch(
  () => filtros.categoriaId,
  () => {
    if (!filtros.categoriaId) {
      return
    }

    if (!subcategoriasFiltradas.value.some((item) => item.id === filtros.subcategoriaId)) {
      filtros.subcategoriaId = ''
    }
  }
)

onMounted(async () => {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    await carregarReferencias()
    await carregarServicos()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados do catalogo de servicos.')
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Catalogo de Servicos"
      subtitulo="Catalogo institucional organizado por departamento responsavel e visibilidade de atendimento."
    >
      <template #actions>
        <q-btn v-if="podeGerenciar" color="primary" icon="add" label="Novo servico" :disable="loading" @click="abrirNovoServico" />
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar o catalogo de servicos.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Busque por nome, descricao e escopo institucional do servico.">
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.termo"
                outlined
                label="Busca"
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

            <div class="col-12 col-md-2">
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
                v-model="filtros.subcategoriaId"
                outlined
                clearable
                emit-value
                map-options
                label="Subcategoria"
                :disable="loading"
                :options="subcategoriasFiltradas.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.status"
                outlined
                clearable
                emit-value
                map-options
                label="Status"
                :disable="loading"
                :options="opcoesStatus"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.visibilidade"
                outlined
                clearable
                emit-value
                map-options
                label="Visibilidade"
                :disable="loading"
                :options="opcoesVisibilidade"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.ativo"
                outlined
                emit-value
                map-options
                label="Ativo"
                :disable="loading"
                :options="opcoesAtivo"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.permiteAberturaChamado"
                outlined
                emit-value
                map-options
                label="Permite abertura"
                :disable="loading"
                :options="opcoesPermiteAbertura"
              />
            </div>
          </div>

          <div class="row justify-end q-gutter-sm">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </AppSectionCard>

      <q-banner v-if="erro && servicos.length" rounded class="bg-red-1 text-negative">
        {{ erro }}
      </q-banner>

      <LoadingState v-if="loading && !servicos.length" mensagem="Carregando catalogo de servicos..." />

      <ErrorState
        v-else-if="erro && !servicos.length"
        titulo="Nao foi possivel carregar os servicos"
        :mensagem="erro"
        @retry="carregarServicos"
      />

      <EmptyState
        v-else-if="!servicos.length"
        titulo="Nenhum servico encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="menu_book"
      />

      <AppSectionCard v-else titulo="Servicos" :subtitulo="`Total de servicos: ${total}`">
        <q-table
          class="sgx-table"
          flat
          bordered
          :rows="servicos"
          :columns="colunas"
          row-key="id"
          :rows-per-page-options="[0]"
          hide-bottom
        >
          <template #body-cell-nome="slotProps">
            <q-td :props="slotProps">
              <div class="text-weight-medium">{{ slotProps.row.nome }}</div>
              <div class="text-caption text-grey-7">{{ slotProps.row.slug }}</div>
            </q-td>
          </template>

          <template #body-cell-status="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="corStatus(slotProps.row.status)">
                {{ slotProps.row.statusDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-visibilidade="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="corVisibilidade(slotProps.row.visibilidade)">
                {{ slotProps.row.visibilidadeDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-permite="slotProps">
            <q-td :props="slotProps">
              <q-badge :color="slotProps.row.permiteAberturaChamado ? 'positive' : 'grey-6'" text-color="white">
                {{ slotProps.row.permiteAberturaChamado ? 'Sim' : 'Nao' }}
              </q-badge>
            </q-td>
          </template>

          <template #body-cell-atualizadoEm="slotProps">
            <q-td :props="slotProps">{{ formatarData(slotProps.row.atualizadoEm || slotProps.row.criadoEm) }}</q-td>
          </template>

          <template #body-cell-acoes="slotProps">
            <q-td :props="slotProps" class="text-right q-gutter-xs">
              <q-btn
                v-if="podeGerenciar"
                flat
                dense
                color="primary"
                icon="edit"
                label="Editar"
                @click="editarServico(slotProps.row.id)"
              />

              <q-btn
                v-if="podeMostrarAcaoPublicar(slotProps.row)"
                flat
                dense
                color="positive"
                icon="publish"
                label="Publicar"
                @click="abrirConfirmacao(slotProps.row, 'publicar')"
              />

              <q-btn
                v-if="podeMostrarAcaoArquivar(slotProps.row)"
                flat
                dense
                color="negative"
                icon="archive"
                label="Arquivar"
                @click="abrirConfirmacao(slotProps.row, 'arquivar')"
              />

              <q-btn
                v-if="podeMostrarAcaoReativar(slotProps.row)"
                flat
                dense
                color="primary"
                icon="restart_alt"
                label="Reativar"
                @click="abrirConfirmacao(slotProps.row, 'reativar')"
              />
            </q-td>
          </template>
        </q-table>

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
    </template>

    <ConfirmDialog
      v-model="confirmarAcaoAberto"
      :titulo="tituloConfirmacao"
      :mensagem="mensagemConfirmacao"
      :confirmar-label="labelConfirmacao"
      :color="corConfirmacao"
      :loading="executandoAcao"
      @confirm="executarAcao"
    />
  </q-page>
</template>

<style scoped>
:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
}

:deep(.sgx-table tbody tr:hover) {
  background: rgba(11, 94, 215, 0.04);
}
</style>
