<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
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
import { baseConhecimentoAdminService } from '../services/baseConhecimentoAdminService'
import { cadastrosAdminService } from '../services/cadastrosAdminService'
import { useAuthStore } from '../stores/authStore'
import type { CategoriaChamadoResumoResponse } from '../types/adminCadastros'
import {
  StatusArtigoConhecimento,
  VisibilidadeArtigoConhecimento,
  type BaseConhecimentoArtigoListagem,
  type FiltroBaseConhecimentoArtigoRequest,
} from '../types/baseConhecimento'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)
const artigos = ref<BaseConhecimentoArtigoListagem[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)
const categorias = ref<CategoriaChamadoResumoResponse[]>([])

const confirmarAcaoAberto = ref(false)
const executandoAcao = ref(false)
const artigoAcao = ref<BaseConhecimentoArtigoListagem | null>(null)
const tipoAcao = ref<'publicar' | 'arquivar' | 'reativar' | null>(null)

const filtros = reactive({
  termo: '',
  status: '' as '' | StatusArtigoConhecimento,
  visibilidade: '' as '' | VisibilidadeArtigoConhecimento,
  categoriaId: '',
  ativo: 'todos' as 'todos' | 'ativos' | 'inativos',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.baseConhecimentoVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.baseConhecimentoGerenciar))
const podePublicar = computed(() => possuiPermissao(permissoes.baseConhecimentoPublicar))
const podeArquivar = computed(() => possuiPermissao(permissoes.baseConhecimentoArquivar))

const opcoesStatus = [
  { label: 'Rascunho', value: StatusArtigoConhecimento.Rascunho },
  { label: 'Em revisao', value: StatusArtigoConhecimento.EmRevisao },
  { label: 'Publicado', value: StatusArtigoConhecimento.Publicado },
  { label: 'Arquivado', value: StatusArtigoConhecimento.Arquivado },
]

const opcoesVisibilidade = [
  { label: 'Solicitante', value: VisibilidadeArtigoConhecimento.Solicitante },
  { label: 'Atendente', value: VisibilidadeArtigoConhecimento.Atendente },
  { label: 'Administrador', value: VisibilidadeArtigoConhecimento.Administrador },
]

const opcoesAtivo = [
  { label: 'Todos', value: 'todos' },
  { label: 'Ativos', value: 'ativos' },
  { label: 'Inativos', value: 'inativos' },
]

const colunas: QTableColumn<BaseConhecimentoArtigoListagem>[] = [
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'categoria', label: 'Categoria', field: (row) => row.categoriaNome || '-', align: 'left' },
  { name: 'status', label: 'Status', field: 'statusDescricao', align: 'left' },
  { name: 'visibilidade', label: 'Visibilidade', field: 'visibilidadeDescricao', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left' },
  { name: 'publicadoEm', label: 'Publicado em', field: 'publicadoEm', align: 'left' },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
]

const tituloConfirmacao = computed(() => {
  if (tipoAcao.value === 'publicar') return 'Confirmar publicacao'
  if (tipoAcao.value === 'arquivar') return 'Confirmar arquivamento'
  return 'Confirmar reativacao'
})

const mensagemConfirmacao = computed(() => {
  const tituloArtigo = artigoAcao.value?.titulo ?? 'este artigo'
  if (tipoAcao.value === 'publicar') return `Deseja publicar o artigo "${tituloArtigo}"?`
  if (tipoAcao.value === 'arquivar') return `Deseja arquivar o artigo "${tituloArtigo}"?`
  return `Deseja reativar o artigo "${tituloArtigo}"?`
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

function statusColor(status: StatusArtigoConhecimento): string {
  switch (status) {
    case StatusArtigoConhecimento.Publicado:
      return 'positive'
    case StatusArtigoConhecimento.Arquivado:
      return 'negative'
    case StatusArtigoConhecimento.EmRevisao:
      return 'warning'
    default:
      return 'grey-7'
  }
}

function visibilidadeColor(visibilidade: VisibilidadeArtigoConhecimento): string {
  switch (visibilidade) {
    case VisibilidadeArtigoConhecimento.Administrador:
      return 'deep-orange'
    case VisibilidadeArtigoConhecimento.Atendente:
      return 'indigo'
    default:
      return 'teal'
  }
}

function podeMostrarAcaoPublicar(artigo: BaseConhecimentoArtigoListagem): boolean {
  if (!podePublicar.value) {
    return false
  }

  return artigo.status !== StatusArtigoConhecimento.Publicado && artigo.status !== StatusArtigoConhecimento.Arquivado
}

function podeMostrarAcaoArquivar(artigo: BaseConhecimentoArtigoListagem): boolean {
  if (!podeArquivar.value) {
    return false
  }

  return artigo.status !== StatusArtigoConhecimento.Arquivado
}

function podeMostrarAcaoReativar(artigo: BaseConhecimentoArtigoListagem): boolean {
  return podeArquivar.value && artigo.status === StatusArtigoConhecimento.Arquivado
}

function abrirNovoArtigo(): void {
  router.push('/admin/conhecimento/base-conhecimento/novo')
}

function editarArtigo(artigoId: string): void {
  router.push(`/admin/conhecimento/base-conhecimento/${artigoId}`)
}

function montarFiltroRequest(): FiltroBaseConhecimentoArtigoRequest {
  return {
    termo: filtros.termo.trim() || undefined,
    status: filtros.status || undefined,
    visibilidade: filtros.visibilidade || undefined,
    categoriaId: filtros.categoriaId || undefined,
    ativo: filtros.ativo === 'todos' ? undefined : filtros.ativo === 'ativos',
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
    ordenarPor: 'atualizadoEm',
    direcaoOrdenacao: 'desc',
  }
}

async function carregarCategorias(): Promise<void> {
  const response = await cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 })
  categorias.value = response.items
}

async function carregarArtigos(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const response = await baseConhecimentoAdminService.listarArtigos(montarFiltroRequest())
    artigos.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar artigos da base de conhecimento.')
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
  filtros.status = ''
  filtros.visibilidade = ''
  filtros.categoriaId = ''
  filtros.ativo = 'todos'
  pagina.value = 1
  await carregarArtigos()
}

async function atualizarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarArtigos()
}

async function atualizarTamanhoPagina(value: number): Promise<void> {
  tamanhoPagina.value = value
  pagina.value = 1
  await carregarArtigos()
}

function abrirConfirmacao(artigo: BaseConhecimentoArtigoListagem, acao: 'publicar' | 'arquivar' | 'reativar'): void {
  artigoAcao.value = artigo
  tipoAcao.value = acao
  confirmarAcaoAberto.value = true
}

async function executarAcao(): Promise<void> {
  if (!artigoAcao.value || !tipoAcao.value) {
    return
  }

  executandoAcao.value = true

  try {
    if (tipoAcao.value === 'publicar') {
      await baseConhecimentoAdminService.publicarArtigo(artigoAcao.value.id)
      $q.notify({ type: 'positive', message: 'Artigo publicado com sucesso.' })
    }

    if (tipoAcao.value === 'arquivar') {
      const response = await baseConhecimentoAdminService.arquivarArtigo(artigoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Artigo arquivado com sucesso.' })
    }

    if (tipoAcao.value === 'reativar') {
      const response = await baseConhecimentoAdminService.reativarArtigo(artigoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Artigo reativado com sucesso.' })
    }

    confirmarAcaoAberto.value = false
    artigoAcao.value = null
    tipoAcao.value = null
    await carregarArtigos()
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel executar a acao selecionada.')
    erro.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    executandoAcao.value = false
  }
}

onMounted(async () => {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    await carregarCategorias()
    await carregarArtigos()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados da base de conhecimento.')
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Base de conhecimento" subtitulo="Gerencie artigos, publicacoes e visibilidade interna do conhecimento.">
      <template #actions>
        <q-btn v-if="podeGerenciar" color="primary" icon="add" label="Novo artigo" :disable="loading" @click="abrirNovoArtigo" />
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar a base de conhecimento.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Busca por titulo, resumo, conteudo e tags, com filtros administrativos.">
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-4">
              <q-input
                v-model="filtros.termo"
                outlined
                label="Busca"
                placeholder="Titulo, resumo, conteudo ou tags"
                :disable="loading"
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
                v-model="filtros.ativo"
                outlined
                emit-value
                map-options
                label="Ativo"
                :disable="loading"
                :options="opcoesAtivo"
              />
            </div>
          </div>

          <div class="row justify-end q-gutter-sm">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </AppSectionCard>

      <q-banner v-if="erro && artigos.length" rounded class="bg-red-1 text-negative">
        {{ erro }}
      </q-banner>

      <LoadingState v-if="loading && !artigos.length" mensagem="Carregando artigos da base de conhecimento..." />

      <ErrorState
        v-else-if="erro && !artigos.length"
        titulo="Nao foi possivel carregar os artigos"
        :mensagem="erro"
        @retry="carregarArtigos"
      />

      <EmptyState
        v-else-if="!artigos.length"
        titulo="Nenhum artigo encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="article"
      />

      <AppSectionCard v-else titulo="Artigos" :subtitulo="`Total de artigos: ${total}`">
        <q-table
          class="sgx-table"
          flat
          bordered
          :rows="artigos"
          :columns="colunas"
          row-key="id"
          :rows-per-page-options="[0]"
          hide-bottom
        >
          <template #body-cell-titulo="slotProps">
            <q-td :props="slotProps">
              <div class="text-weight-medium">{{ slotProps.row.titulo }}</div>
              <div class="text-caption text-grey-7">{{ slotProps.row.slug }}</div>
            </q-td>
          </template>

          <template #body-cell-status="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="statusColor(slotProps.row.status)">
                {{ slotProps.row.statusDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-visibilidade="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="visibilidadeColor(slotProps.row.visibilidade)">
                {{ slotProps.row.visibilidadeDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-ativo="slotProps">
            <q-td :props="slotProps">
              <q-badge :color="slotProps.row.ativo ? 'positive' : 'grey-6'" text-color="white">
                {{ slotProps.row.ativo ? 'Ativo' : 'Inativo' }}
              </q-badge>
            </q-td>
          </template>

          <template #body-cell-atualizadoEm="slotProps">
            <q-td :props="slotProps">{{ formatarData(slotProps.row.atualizadoEm || slotProps.row.criadoEm) }}</q-td>
          </template>

          <template #body-cell-publicadoEm="slotProps">
            <q-td :props="slotProps">{{ formatarData(slotProps.row.publicadoEm) }}</q-td>
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
                @click="editarArtigo(slotProps.row.id)"
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
