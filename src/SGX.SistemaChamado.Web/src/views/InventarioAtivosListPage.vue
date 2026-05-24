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
import { adminService } from '../services/adminService'
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { usuariosAdminService } from '../services/usuariosAdminService'
import { useAuthStore } from '../stores/authStore'
import type { AtendenteResumo } from '../types/admin'
import type { InventarioAtivoListagem, TipoAtivoInventario } from '../types/inventarioAtivos'
import {
  CriticidadeAtivo,
  StatusOperacionalAtivo,
  StatusPatrimonialAtivo,
  type FiltroInventarioAtivoRequest,
} from '../types/inventarioAtivos'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)
const ativos = ref<InventarioAtivoListagem[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)

const tiposAtivo = ref<TipoAtivoInventario[]>([])
const departamentos = ref<{ id: string; nome: string }[]>([])
const locaisUnidade = ref<{ id: string; nome: string }[]>([])
const usuariosResponsaveis = ref<AtendenteResumo[]>([])

const confirmarAcaoAberto = ref(false)
const executandoAcao = ref(false)
const ativoAcao = ref<InventarioAtivoListagem | null>(null)
const tipoAcao = ref<'inativar' | 'reativar' | null>(null)

const filtros = reactive({
  termo: '',
  tipoAtivoInventarioId: '',
  departamentoId: '',
  localUnidadeId: '',
  usuarioResponsavelId: '',
  statusOperacional: '' as '' | StatusOperacionalAtivo,
  statusPatrimonial: '' as '' | StatusPatrimonialAtivo,
  criticidade: '' as '' | CriticidadeAtivo,
  ativo: 'todos' as 'todos' | 'ativos' | 'inativos',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.inventarioAtivosVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.inventarioAtivosGerenciar))
const podeInativar = computed(() => possuiPermissao(permissoes.inventarioAtivosInativar))

const opcoesStatusOperacional = [
  { label: 'Operacional', value: StatusOperacionalAtivo.Operacional },
  { label: 'Em manutencao', value: StatusOperacionalAtivo.EmManutencao },
  { label: 'Com defeito', value: StatusOperacionalAtivo.ComDefeito },
  { label: 'Reservado', value: StatusOperacionalAtivo.Reservado },
  { label: 'Baixado', value: StatusOperacionalAtivo.Baixado },
]

const opcoesStatusPatrimonial = [
  { label: 'Em uso', value: StatusPatrimonialAtivo.EmUso },
  { label: 'Em estoque', value: StatusPatrimonialAtivo.EmEstoque },
  { label: 'Emprestado', value: StatusPatrimonialAtivo.Emprestado },
  { label: 'Em transferencia', value: StatusPatrimonialAtivo.EmTransferencia },
  { label: 'Descartado', value: StatusPatrimonialAtivo.Descartado },
  { label: 'Extraviado', value: StatusPatrimonialAtivo.Extraviado },
]

const opcoesCriticidade = [
  { label: 'Baixa', value: CriticidadeAtivo.Baixa },
  { label: 'Media', value: CriticidadeAtivo.Media },
  { label: 'Alta', value: CriticidadeAtivo.Alta },
  { label: 'Critica', value: CriticidadeAtivo.Critica },
]

const opcoesAtivo = [
  { label: 'Todos', value: 'todos' },
  { label: 'Ativos', value: 'ativos' },
  { label: 'Inativos', value: 'inativos' },
]

const colunas: QTableColumn<InventarioAtivoListagem>[] = [
  { name: 'codigo', label: 'Codigo', field: 'codigo', align: 'left', sortable: true },
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'tipo', label: 'Tipo', field: 'tipoAtivoInventarioNome', align: 'left' },
  { name: 'patrimonio', label: 'Patrimonio', field: (row) => row.numeroPatrimonio || '-', align: 'left' },
  { name: 'serie', label: 'Serie', field: (row) => row.numeroSerie || '-', align: 'left' },
  { name: 'responsavel', label: 'Responsavel', field: (row) => row.usuarioResponsavelNome || '-', align: 'left' },
  { name: 'departamento', label: 'Departamento', field: (row) => row.departamentoNome || '-', align: 'left' },
  { name: 'local', label: 'Local', field: (row) => row.localUnidadeNome || '-', align: 'left' },
  { name: 'statusOperacional', label: 'Status operacional', field: 'statusOperacionalDescricao', align: 'left' },
  { name: 'statusPatrimonial', label: 'Status patrimonial', field: 'statusPatrimonialDescricao', align: 'left' },
  { name: 'criticidade', label: 'Criticidade', field: 'criticidadeDescricao', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
]

const tituloConfirmacao = computed(() =>
  tipoAcao.value === 'inativar' ? 'Confirmar inativacao' : 'Confirmar reativacao'
)

const mensagemConfirmacao = computed(() => {
  const nome = ativoAcao.value?.nome ?? 'este ativo'
  if (tipoAcao.value === 'inativar') {
    return `Deseja inativar o ativo "${nome}"?`
  }

  return `Deseja reativar o ativo "${nome}"?`
})

const labelConfirmacao = computed(() => (tipoAcao.value === 'inativar' ? 'Inativar' : 'Reativar'))
const corConfirmacao = computed(() => (tipoAcao.value === 'inativar' ? 'negative' : 'primary'))

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

function corStatusOperacional(value: StatusOperacionalAtivo): string {
  switch (value) {
    case StatusOperacionalAtivo.Operacional:
      return 'positive'
    case StatusOperacionalAtivo.EmManutencao:
      return 'warning'
    case StatusOperacionalAtivo.ComDefeito:
      return 'negative'
    case StatusOperacionalAtivo.Reservado:
      return 'blue'
    default:
      return 'grey-7'
  }
}

function corStatusPatrimonial(value: StatusPatrimonialAtivo): string {
  switch (value) {
    case StatusPatrimonialAtivo.EmUso:
      return 'primary'
    case StatusPatrimonialAtivo.EmEstoque:
      return 'teal'
    case StatusPatrimonialAtivo.Emprestado:
      return 'indigo'
    case StatusPatrimonialAtivo.EmTransferencia:
      return 'warning'
    case StatusPatrimonialAtivo.Descartado:
      return 'negative'
    case StatusPatrimonialAtivo.Extraviado:
      return 'deep-orange'
    default:
      return 'grey-7'
  }
}

function corCriticidade(value: CriticidadeAtivo): string {
  switch (value) {
    case CriticidadeAtivo.Baixa:
      return 'positive'
    case CriticidadeAtivo.Media:
      return 'blue'
    case CriticidadeAtivo.Alta:
      return 'warning'
    default:
      return 'negative'
  }
}

function abrirNovoAtivo(): void {
  router.push('/admin/infraestrutura/inventario-ativos/novo')
}

function abrirDetalhe(ativoId: string): void {
  router.push(`/admin/infraestrutura/inventario-ativos/${ativoId}`)
}

function editarAtivo(ativoId: string): void {
  router.push(`/admin/infraestrutura/inventario-ativos/${ativoId}/editar`)
}

function montarFiltroRequest(): FiltroInventarioAtivoRequest {
  return {
    termo: filtros.termo.trim() || undefined,
    tipoAtivoInventarioId: filtros.tipoAtivoInventarioId || undefined,
    departamentoId: filtros.departamentoId || undefined,
    localUnidadeId: filtros.localUnidadeId || undefined,
    usuarioResponsavelId: filtros.usuarioResponsavelId || undefined,
    statusOperacional: filtros.statusOperacional || undefined,
    statusPatrimonial: filtros.statusPatrimonial || undefined,
    criticidade: filtros.criticidade || undefined,
    ativo: filtros.ativo === 'todos' ? undefined : filtros.ativo === 'ativos',
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
    ordenarPor: 'atualizadoEm',
    direcaoOrdenacao: 'desc',
  }
}

async function carregarReferencias(): Promise<void> {
  const [tiposResponse, contextoResponse, usuariosResponse] = await Promise.all([
    inventarioAtivosAdminService.listarTipos(),
    adminService.obterAdminContexto(),
    usuariosAdminService.listar({ ativo: true, tamanhoPagina: 200, ordenarPor: 'nome', direcaoOrdenacao: 'asc' }),
  ])

  tiposAtivo.value = tiposResponse
  departamentos.value = contextoResponse.departamentos
  locaisUnidade.value = contextoResponse.locaisUnidade
  usuariosResponsaveis.value = usuariosResponse.items.map((item) => ({
    id: item.id,
    nome: item.nome,
    email: item.email,
    perfis: item.perfis.map((perfil) => perfil.nome),
  }))
}

async function carregarAtivos(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const response = await inventarioAtivosAdminService.listar(montarFiltroRequest())
    ativos.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os ativos de inventario.')
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  pagina.value = 1
  await carregarAtivos()
}

async function limparFiltros(): Promise<void> {
  filtros.termo = ''
  filtros.tipoAtivoInventarioId = ''
  filtros.departamentoId = ''
  filtros.localUnidadeId = ''
  filtros.usuarioResponsavelId = ''
  filtros.statusOperacional = ''
  filtros.statusPatrimonial = ''
  filtros.criticidade = ''
  filtros.ativo = 'todos'
  pagina.value = 1
  await carregarAtivos()
}

async function atualizarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarAtivos()
}

async function atualizarTamanhoPagina(value: number): Promise<void> {
  tamanhoPagina.value = value
  pagina.value = 1
  await carregarAtivos()
}

function abrirConfirmacao(ativo: InventarioAtivoListagem, acao: 'inativar' | 'reativar'): void {
  ativoAcao.value = ativo
  tipoAcao.value = acao
  confirmarAcaoAberto.value = true
}

async function executarAcao(): Promise<void> {
  if (!ativoAcao.value || !tipoAcao.value) {
    return
  }

  executandoAcao.value = true

  try {
    if (tipoAcao.value === 'inativar') {
      const response = await inventarioAtivosAdminService.inativar(ativoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Ativo inativado com sucesso.' })
    } else {
      const response = await inventarioAtivosAdminService.reativar(ativoAcao.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Ativo reativado com sucesso.' })
    }

    confirmarAcaoAberto.value = false
    ativoAcao.value = null
    tipoAcao.value = null
    await carregarAtivos()
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
    await carregarReferencias()
    await carregarAtivos()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados de inventario.')
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Inventario/Ativos" subtitulo="Controle administrativo de ativos com rastreabilidade operacional.">
      <template #actions>
        <q-btn v-if="podeGerenciar" color="primary" icon="add" label="Novo ativo" :disable="loading" @click="abrirNovoAtivo" />
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar o inventario de ativos.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Pesquise por codigo, nome e contexto operacional do ativo.">
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.termo"
                outlined
                label="Busca"
                placeholder="Codigo, nome, patrimonio, serie..."
                :disable="loading"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-select
                v-model="filtros.tipoAtivoInventarioId"
                outlined
                clearable
                emit-value
                map-options
                label="Tipo"
                :disable="loading"
                :options="tiposAtivo.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.departamentoId"
                outlined
                clearable
                emit-value
                map-options
                label="Departamento"
                :disable="loading"
                :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.localUnidadeId"
                outlined
                clearable
                emit-value
                map-options
                label="Local / Unidade"
                :disable="loading"
                :options="locaisUnidade.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.usuarioResponsavelId"
                outlined
                clearable
                emit-value
                map-options
                label="Responsavel"
                :disable="loading"
                :options="usuariosResponsaveis.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.statusOperacional"
                outlined
                clearable
                emit-value
                map-options
                label="Status operacional"
                :disable="loading"
                :options="opcoesStatusOperacional"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.statusPatrimonial"
                outlined
                clearable
                emit-value
                map-options
                label="Status patrimonial"
                :disable="loading"
                :options="opcoesStatusPatrimonial"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.criticidade"
                outlined
                clearable
                emit-value
                map-options
                label="Criticidade"
                :disable="loading"
                :options="opcoesCriticidade"
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

      <q-banner v-if="erro && ativos.length" rounded class="bg-red-1 text-negative">
        {{ erro }}
      </q-banner>

      <LoadingState v-if="loading && !ativos.length" mensagem="Carregando inventario de ativos..." />

      <ErrorState
        v-else-if="erro && !ativos.length"
        titulo="Nao foi possivel carregar os ativos"
        :mensagem="erro"
        @retry="carregarAtivos"
      />

      <EmptyState
        v-else-if="!ativos.length"
        titulo="Nenhum ativo encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="inventory_2"
      />

      <AppSectionCard v-else titulo="Ativos" :subtitulo="`Total de ativos: ${total}`">
        <q-table
          class="sgx-table"
          flat
          bordered
          :rows="ativos"
          :columns="colunas"
          row-key="id"
          :rows-per-page-options="[0]"
          hide-bottom
        >
          <template #body-cell-statusOperacional="slotProps">
            <q-td :props="slotProps">
              <q-chip
                dense
                square
                text-color="white"
                :color="corStatusOperacional(slotProps.row.statusOperacional)"
              >
                {{ slotProps.row.statusOperacionalDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-statusPatrimonial="slotProps">
            <q-td :props="slotProps">
              <q-chip
                dense
                square
                text-color="white"
                :color="corStatusPatrimonial(slotProps.row.statusPatrimonial)"
              >
                {{ slotProps.row.statusPatrimonialDescricao }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-criticidade="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="corCriticidade(slotProps.row.criticidade)">
                {{ slotProps.row.criticidadeDescricao }}
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

          <template #body-cell-acoes="slotProps">
            <q-td :props="slotProps" class="text-right q-gutter-xs">
              <q-btn flat dense color="primary" icon="visibility" label="Ver detalhes" @click="abrirDetalhe(slotProps.row.id)" />

              <q-btn
                v-if="podeGerenciar && slotProps.row.ativo"
                flat
                dense
                color="primary"
                icon="edit"
                label="Editar"
                @click="editarAtivo(slotProps.row.id)"
              />

              <q-btn
                v-if="podeInativar && slotProps.row.ativo"
                flat
                dense
                color="negative"
                icon="block"
                label="Inativar"
                @click="abrirConfirmacao(slotProps.row, 'inativar')"
              />

              <q-btn
                v-if="podeInativar && !slotProps.row.ativo"
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
