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
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
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

const indicadoresPagina = computed(() => {
  const totalPagina = ativos.value.length
  const ativosOperacionais = ativos.value.filter((item) => item.statusOperacional === StatusOperacionalAtivo.Operacional).length
  const emManutencao = ativos.value.filter((item) => item.statusOperacional === StatusOperacionalAtivo.EmManutencao).length
  const criticidadeAlta = ativos.value.filter((item) => item.criticidade === CriticidadeAtivo.Alta || item.criticidade === CriticidadeAtivo.Critica).length

  return {
    totalPagina,
    ativosOperacionais,
    emManutencao,
    criticidadeAlta,
  }
})

const filtrosAplicados = computed(() => {
  let totalFiltros = 0

  if (filtros.termo.trim()) totalFiltros += 1
  if (filtros.tipoAtivoInventarioId) totalFiltros += 1
  if (filtros.departamentoId) totalFiltros += 1
  if (filtros.localUnidadeId) totalFiltros += 1
  if (filtros.usuarioResponsavelId) totalFiltros += 1
  if (filtros.statusOperacional) totalFiltros += 1
  if (filtros.statusPatrimonial) totalFiltros += 1
  if (filtros.criticidade) totalFiltros += 1
  if (filtros.ativo !== 'todos') totalFiltros += 1

  return totalFiltros
})

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
  <q-page class="sgx-page column q-gutter-md inventario-lista">
    <PageHeader
      titulo="Inventario/Ativos"
      subtitulo="Controle administrativo de ativos com rastreabilidade operacional e patrimonial."
      contexto="Infraestrutura"
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn flat color="primary" icon="refresh" label="Atualizar" :loading="loading" @click="carregarAtivos" />
          <q-btn v-if="podeGerenciar" color="primary" icon="add" label="Novo ativo" unelevated :disable="loading" @click="abrirNovoAtivo" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar o inventario de ativos.
    </q-banner>

    <template v-else>
      <div class="inventario-lista__kpis">
        <MetricCard titulo="Ativos na pagina" :valor="indicadoresPagina.totalPagina" icon="inventory_2" tone="primary" />
        <MetricCard titulo="Operacionais" :valor="indicadoresPagina.ativosOperacionais" icon="check_circle" tone="positive" />
        <MetricCard titulo="Em manutencao" :valor="indicadoresPagina.emManutencao" icon="build" tone="warning" />
        <MetricCard titulo="Criticidade alta/critica" :valor="indicadoresPagina.criticidadeAlta" icon="priority_high" tone="negative" />
      </div>

      <AppSectionCard titulo="Filtros" :subtitulo="`Pesquise por codigo, nome e contexto operacional do ativo. Filtros aplicados: ${filtrosAplicados}.`">
        <FilterBar compact titulo="Refine a consulta" subtitulo="Use combinacoes de filtros para cruzar operacao, patrimonio e responsavel.">
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
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" unelevated />
            <q-btn flat color="primary" label="Limpar filtros" :disable="loading" @click="limparFiltros" />
          </div>
          </q-form>
        </FilterBar>
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
      >
        <template v-if="podeGerenciar" #actions>
          <q-btn color="primary" icon="add" label="Cadastrar ativo" unelevated @click="abrirNovoAtivo" />
        </template>
      </EmptyState>

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
          <template #body-cell-codigo="slotProps">
            <q-td :props="slotProps">
              <div class="column">
                <span class="text-weight-semibold">{{ slotProps.row.codigo }}</span>
                <span class="text-caption sgx-muted">Atualizado: {{ slotProps.row.atualizadoEm ? new Date(slotProps.row.atualizadoEm).toLocaleDateString('pt-BR') : '-' }}</span>
              </div>
            </q-td>
          </template>

          <template #body-cell-statusOperacional="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.statusOperacionalDescricao" />
            </q-td>
          </template>

          <template #body-cell-statusPatrimonial="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.statusPatrimonialDescricao" />
            </q-td>
          </template>

          <template #body-cell-criticidade="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.criticidadeDescricao" />
            </q-td>
          </template>

          <template #body-cell-tipo="slotProps">
            <q-td :props="slotProps">
              <q-chip dense color="blue-1" text-color="primary" icon="category" :label="slotProps.row.tipoAtivoInventarioNome" />
            </q-td>
          </template>

          <template #body-cell-patrimonio="slotProps">
            <q-td :props="slotProps">
              <q-chip
                dense
                square
                color="grey-2"
                text-color="grey-9"
                icon="badge"
                :label="slotProps.row.numeroPatrimonio || '-'"
              />
            </q-td>
          </template>

          <template #body-cell-ativo="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.ativo ? 'Ativo' : 'Inativo'" />
            </q-td>
          </template>

          <template #body-cell-acoes="slotProps">
            <q-td :props="slotProps" class="text-right q-gutter-xs">
              <q-btn flat round dense color="primary" icon="visibility" aria-label="Ver detalhes do ativo" @click="abrirDetalhe(slotProps.row.id)">
                <q-tooltip>Ver detalhes</q-tooltip>
              </q-btn>

              <q-btn
                v-if="podeGerenciar && slotProps.row.ativo"
                flat
                round
                dense
                color="primary"
                icon="edit"
                aria-label="Editar ativo"
                @click="editarAtivo(slotProps.row.id)"
              >
                <q-tooltip>Editar ativo</q-tooltip>
              </q-btn>

              <q-btn
                v-if="podeInativar && slotProps.row.ativo"
                flat
                round
                dense
                color="negative"
                icon="block"
                aria-label="Inativar ativo"
                @click="abrirConfirmacao(slotProps.row, 'inativar')"
              >
                <q-tooltip>Inativar ativo</q-tooltip>
              </q-btn>

              <q-btn
                v-if="podeInativar && !slotProps.row.ativo"
                flat
                round
                dense
                color="primary"
                icon="restart_alt"
                aria-label="Reativar ativo"
                @click="abrirConfirmacao(slotProps.row, 'reativar')"
              >
                <q-tooltip>Reativar ativo</q-tooltip>
              </q-btn>
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
.inventario-lista__kpis {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
}

:deep(.sgx-table .q-chip) {
  max-width: 100%;
}

:deep(.sgx-table tbody tr:hover) {
  background: rgba(11, 94, 215, 0.04);
}

@media (max-width: 1100px) {
  .inventario-lista__kpis {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 768px) {
  .inventario-lista__kpis {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
