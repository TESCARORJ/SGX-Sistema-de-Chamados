<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { relatoriosAvancadosAdminService } from '../services/relatoriosAvancadosAdminService'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroRelatorioAprovacoes,
  RelatorioAprovacoesPorOrigem,
  RelatorioAprovacoesResumo,
  RelatorioAprovacoesTempoMedio,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioAprovacoesResumo | null>(null)
const tempoMedio = ref<RelatorioAprovacoesTempoMedio[]>([])
const porOrigem = ref<RelatorioAprovacoesPorOrigem[]>([])

const departamentos = ref<{ id: string; nome: string }[]>([])
const categorias = ref<{ id: string; nome: string }[]>([])
const prioridades = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioAprovacoes>({
  dataInicial: '',
  dataFinal: '',
  departamentoId: '',
  categoriaId: '',
  prioridadeId: '',
  statusAprovacao: '',
  tipoOrigemAprovacao: '',
  agrupamento: 3,
  agruparPor: 1,
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosVisualizar))
const podeGerencial = computed(() => possuiPermissao(permissoes.relatoriosAvancadosGerencial))
const podeOperacional = computed(() => possuiPermissao(permissoes.relatoriosAvancadosOperacional))
const podeExportar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosExportar))

const opcoesAgrupamento = [
  { label: 'Diario', value: 1 },
  { label: 'Semanal', value: 2 },
  { label: 'Mensal', value: 3 },
]

const opcoesAgruparPor = [
  { label: 'Status aprovacao', value: 1 },
  { label: 'Origem', value: 2 },
  { label: 'Departamento', value: 3 },
]

const colunasTempo = [
  { name: 'grupo', label: 'Grupo', field: 'grupo', align: 'left' as const },
  { name: 'totalDecididas', label: 'Total decididas', field: 'totalDecididas', align: 'right' as const },
  { name: 'tempoMedioDecisaoHoras', label: 'Tempo medio (h)', field: 'tempoMedioDecisaoHoras', align: 'right' as const },
]

const colunasOrigem = [
  { name: 'tipoOrigem', label: 'Tipo de origem', field: 'tipoOrigem', align: 'left' as const },
  { name: 'total', label: 'Total', field: 'total', align: 'right' as const },
  { name: 'pendentes', label: 'Pendentes', field: 'pendentes', align: 'right' as const },
  { name: 'aprovadas', label: 'Aprovadas', field: 'aprovadas', align: 'right' as const },
  { name: 'reprovadas', label: 'Reprovadas', field: 'reprovadas', align: 'right' as const },
  { name: 'canceladas', label: 'Canceladas', field: 'canceladas', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioAprovacoes {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    departamentoId: filtros.departamentoId || undefined,
    categoriaId: filtros.categoriaId || undefined,
    prioridadeId: filtros.prioridadeId || undefined,
    statusAprovacao: filtros.statusAprovacao || undefined,
    tipoOrigemAprovacao: filtros.tipoOrigemAprovacao || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.departamentoId = ''
  filtros.categoriaId = ''
  filtros.prioridadeId = ''
  filtros.statusAprovacao = ''
  filtros.tipoOrigemAprovacao = ''
  filtros.agrupamento = 3
  filtros.agruparPor = 1
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const contexto = await adminService.obterAdminContexto()
    departamentos.value = contexto.departamentos.map((item) => ({ id: item.id, nome: item.nome }))
    categorias.value = contexto.categorias.map((item) => ({ id: item.id, nome: item.nome }))
    prioridades.value = contexto.prioridades.map((item) => ({ id: item.id, nome: item.nome }))
  } finally {
    loadingContexto.value = false
  }
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const request = construirFiltros()
    const [resumoResp, tempoResp, origemResp] = await Promise.all([
      podeGerencial.value ? relatoriosAvancadosAdminService.obterResumoAprovacoes(request) : Promise.resolve(null),
      podeGerencial.value ? relatoriosAvancadosAdminService.obterTempoMedioAprovacoes(request) : Promise.resolve([]),
      podeOperacional.value ? relatoriosAvancadosAdminService.obterAprovacoesPorOrigem(request) : Promise.resolve([]),
    ])

    resumo.value = resumoResp
    tempoMedio.value = tempoResp
    porOrigem.value = origemResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de aprovacoes.'
  } finally {
    loading.value = false
  }
}

function exportarDados(): void {
  if (!podeExportar.value) {
    return
  }

  const linhas = [
    ...(resumo.value
      ? [
          { grupo: 'Resumo', indicador: 'Total de aprovacoes', valor: resumo.value.totalAprovacoes },
          { grupo: 'Resumo', indicador: 'Pendentes', valor: resumo.value.pendentes },
          { grupo: 'Resumo', indicador: 'Aprovadas', valor: resumo.value.aprovadas },
          { grupo: 'Resumo', indicador: 'Reprovadas', valor: resumo.value.reprovadas },
          { grupo: 'Resumo', indicador: 'Canceladas', valor: resumo.value.canceladas },
        ]
      : []),
    ...porOrigem.value.map((item) => ({ grupo: 'Origem', indicador: item.tipoOrigem, valor: item.total })),
  ]

  exportarCsv('relatorio-aprovacoes.csv', linhas)
}

onMounted(async () => {
  if (!podeVisualizar.value) {
    return
  }

  await Promise.all([carregarContexto(), carregar()])
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Aprovacoes" subtitulo="Acompanhe gargalos, tempo medio e distribuicao por origem." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de aprovacoes.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo, classificacao e origem de aprovacoes para leitura gerencial e operacional.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.departamentoId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Departamento"
                :loading="loadingContexto"
                :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.categoriaId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Categoria"
                :loading="loadingContexto"
                :options="categorias.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.prioridadeId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Prioridade"
                :loading="loadingContexto"
                :options="prioridades.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.statusAprovacao" outlined dense label="Status aprovacao" /></div>
            <div class="col-12 col-md-3"><q-input v-model="filtros.tipoOrigemAprovacao" outlined dense label="Tipo de origem" /></div>
            <div class="col-12 col-md-2">
              <q-select v-model="filtros.agruparPor" outlined dense emit-value map-options label="Tempo medio por" :options="opcoesAgruparPor" />
            </div>
            <div class="col-12 col-md-2">
              <q-select v-model="filtros.agrupamento" outlined dense emit-value map-options label="Agrupamento" :options="opcoesAgrupamento" />
            </div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo && !tempoMedio.length && !porOrigem.length" mensagem="Carregando relatorios de aprovacoes..." />

      <template v-else>
        <div v-if="resumo" class="sgx-kpi-grid">
          <MetricCard titulo="Total" :valor="resumo.totalAprovacoes" icon="fact_check" tone="primary" />
          <MetricCard titulo="Pendentes" :valor="resumo.pendentes" icon="hourglass_top" tone="warning" />
          <MetricCard titulo="Aprovadas" :valor="resumo.aprovadas" icon="task_alt" tone="positive" />
          <MetricCard titulo="Reprovadas" :valor="resumo.reprovadas" icon="cancel" tone="negative" />
          <MetricCard titulo="Taxa aprovacao" :valor="resumo.taxaAprovacao == null ? '-' : `${Number(resumo.taxaAprovacao).toFixed(2)}%`" icon="trending_up" tone="info" />
          <MetricCard titulo="Tempo medio decisao" :valor="resumo.tempoMedioDecisaoHoras == null ? '-' : `${Number(resumo.tempoMedioDecisaoHoras).toFixed(2)}h`" icon="timer" tone="primary" />
        </div>

        <AppSectionCard titulo="Tempo medio de decisao" :subtitulo="`Grupos analisados: ${tempoMedio.length}`">
          <q-table v-if="tempoMedio.length" class="sgx-table" flat bordered :rows="tempoMedio" :columns="colunasTempo" row-key="grupo" hide-pagination>
            <template #body-cell-tempoMedioDecisaoHoras="props">
              <q-td :props="props" class="text-right">{{ props.row.tempoMedioDecisaoHoras == null ? '-' : Number(props.row.tempoMedioDecisaoHoras).toFixed(2) }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem tempo medio"
            mensagem="Nao ha dados de tempo medio para os filtros informados."
            icon="timer"
          />
        </AppSectionCard>

        <AppSectionCard v-if="podeOperacional" titulo="Aprovacoes por origem" :subtitulo="`Origens encontradas: ${porOrigem.length}`">
          <q-table v-if="porOrigem.length" class="sgx-table" flat bordered :rows="porOrigem" :columns="colunasOrigem" row-key="tipoOrigem" hide-pagination>
            <template #body-cell-tipoOrigem="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.tipoOrigem" />
              </q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem dados por origem"
            mensagem="Nao ha aprovacoes por origem para os filtros informados."
            icon="account_tree"
          />
        </AppSectionCard>

        <EmptyState
          v-if="!resumo && !tempoMedio.length && !porOrigem.length"
          titulo="Sem dados de aprovacoes"
          mensagem="Nao ha resultados para os filtros informados."
          icon="fact_check"
        />
      </template>
    </template>
  </q-page>
</template>
