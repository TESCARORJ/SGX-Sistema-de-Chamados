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
  FiltroRelatorioSla,
  RelatorioSlaPorDepartamento,
  RelatorioSlaPorPrioridade,
  RelatorioSlaResumo,
  RelatorioSlaViolacao,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioSlaResumo | null>(null)
const violacoes = ref<RelatorioSlaViolacao[]>([])
const porDepartamento = ref<RelatorioSlaPorDepartamento[]>([])
const porPrioridade = ref<RelatorioSlaPorPrioridade[]>([])

const departamentos = ref<{ id: string; nome: string }[]>([])
const prioridades = ref<{ id: string; nome: string }[]>([])
const status = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioSla>({
  dataInicial: '',
  dataFinal: '',
  departamentoId: '',
  prioridadeId: '',
  statusId: '',
  limiteRanking: 20,
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

const colunasViolacoes = [
  { name: 'numeroProtocolo', label: 'Protocolo', field: 'numeroProtocolo', align: 'left' as const },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left' as const },
  { name: 'departamento', label: 'Departamento', field: 'departamento', align: 'left' as const },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'status', align: 'left' as const },
  { name: 'dataLimiteSla', label: 'Limite SLA', field: 'dataLimiteSla', align: 'left' as const },
  { name: 'horasExcedidas', label: 'Horas excedidas', field: 'horasExcedidas', align: 'right' as const },
]

const colunasCumprimento = [
  { name: 'nome', label: 'Grupo', field: 'nome', align: 'left' as const },
  { name: 'totalComSla', label: 'Total com SLA', field: 'totalComSla', align: 'right' as const },
  { name: 'dentroSla', label: 'Dentro SLA', field: 'dentroSla', align: 'right' as const },
  { name: 'foraSla', label: 'Fora SLA', field: 'foraSla', align: 'right' as const },
  { name: 'percentualCumprimento', label: '% de cumprimento', field: 'percentualCumprimento', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioSla {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    departamentoId: filtros.departamentoId || undefined,
    prioridadeId: filtros.prioridadeId || undefined,
    statusId: filtros.statusId || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.departamentoId = ''
  filtros.prioridadeId = ''
  filtros.statusId = ''
  filtros.limiteRanking = 20
}

function formatarData(valor: string | null): string {
  if (!valor) {
    return '-'
  }

  const data = new Date(valor)
  return Number.isNaN(data.getTime()) ? valor : data.toLocaleString('pt-BR')
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const contexto = await adminService.obterAdminContexto()
    departamentos.value = contexto.departamentos.map((item) => ({ id: item.id, nome: item.nome }))
    prioridades.value = contexto.prioridades.map((item) => ({ id: item.id, nome: item.nome }))
    status.value = contexto.status.map((item) => ({ id: item.id, nome: item.nome }))
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

    const [resumoResp, violacoesResp, departamentoResp, prioridadeResp] = await Promise.all([
      podeGerencial.value ? relatoriosAvancadosAdminService.obterResumoSla(request) : Promise.resolve(null),
      podeOperacional.value ? relatoriosAvancadosAdminService.obterViolacoesSla(request) : Promise.resolve([]),
      podeGerencial.value ? relatoriosAvancadosAdminService.obterSlaPorDepartamento(request) : Promise.resolve([]),
      podeOperacional.value ? relatoriosAvancadosAdminService.obterSlaPorPrioridade(request) : Promise.resolve([]),
    ])

    resumo.value = resumoResp
    violacoes.value = violacoesResp
    porDepartamento.value = departamentoResp
    porPrioridade.value = prioridadeResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de SLA.'
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
          { grupo: 'Resumo', indicador: 'Total com SLA', valor: resumo.value.totalChamadosComSla },
          { grupo: 'Resumo', indicador: 'Dentro SLA', valor: resumo.value.totalDentroSla },
          { grupo: 'Resumo', indicador: 'Fora SLA', valor: resumo.value.totalForaSla },
          { grupo: 'Resumo', indicador: 'Total sem SLA', valor: resumo.value.totalSemSla ?? 0 },
        ]
      : []),
    ...violacoes.value.map((item) => ({
      grupo: 'Violacao',
      indicador: item.numeroProtocolo,
      valor: item.horasExcedidas ?? 0,
    })),
  ]

  exportarCsv('relatorio-sla.csv', linhas)
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
    <PageHeader titulo="Relatorios - SLA" subtitulo="Cumprimento de prazos, violacoes e desempenho por grupo." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de SLA.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Recorte de periodo, departamento, prioridade e status para os indicadores de SLA.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
            <div class="col-12 col-md-3">
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
            <div class="col-12 col-md-3">
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
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.statusId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Status"
                :loading="loadingContexto"
                :options="status.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo && !violacoes.length" mensagem="Carregando relatorios de SLA..." />

      <template v-else>
        <div v-if="resumo" class="sgx-kpi-grid">
          <MetricCard titulo="Total com SLA" :valor="resumo.totalChamadosComSla" icon="schedule" tone="primary" />
          <MetricCard titulo="Dentro SLA" :valor="resumo.totalDentroSla" icon="task_alt" tone="positive" />
          <MetricCard titulo="Fora SLA" :valor="resumo.totalForaSla" icon="warning" tone="negative" />
          <MetricCard titulo="Cumprimento" :valor="resumo.percentualCumprimento == null ? '-' : `${Number(resumo.percentualCumprimento).toFixed(2)}%`" icon="percent" tone="info" />
          <MetricCard titulo="Tempo medio resolucao" :valor="resumo.tempoMedioResolucaoHoras == null ? '-' : `${Number(resumo.tempoMedioResolucaoHoras).toFixed(2)}h`" icon="timer" tone="warning" />
          <MetricCard titulo="Proximos do vencimento" :valor="resumo.chamadosProximosVencimento ?? '-'" icon="alarm" tone="warning" />
        </div>

        <AppSectionCard titulo="SLA por departamento" :subtitulo="`Departamentos analisados: ${porDepartamento.length}`">
          <q-table v-if="porDepartamento.length" class="sgx-table" flat bordered :rows="porDepartamento.map((x) => ({ ...x, nome: x.departamentoNome }))" :columns="colunasCumprimento" row-key="departamentoNome" hide-pagination>
            <template #body-cell-percentualCumprimento="props">
              <q-td :props="props" class="text-right">{{ props.row.percentualCumprimento == null ? '-' : `${Number(props.row.percentualCumprimento).toFixed(2)}%` }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem distribuicao por departamento"
            mensagem="Nao ha dados de SLA por departamento para os filtros aplicados."
            icon="apartment"
          />
        </AppSectionCard>

        <AppSectionCard v-if="podeOperacional" titulo="SLA por prioridade" :subtitulo="`Prioridades analisadas: ${porPrioridade.length}`">
          <q-table v-if="porPrioridade.length" class="sgx-table" flat bordered :rows="porPrioridade.map((x) => ({ ...x, nome: x.prioridadeNome }))" :columns="colunasCumprimento" row-key="prioridadeId" hide-pagination>
            <template #body-cell-percentualCumprimento="props">
              <q-td :props="props" class="text-right">{{ props.row.percentualCumprimento == null ? '-' : `${Number(props.row.percentualCumprimento).toFixed(2)}%` }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem distribuicao por prioridade"
            mensagem="Nao ha dados de SLA por prioridade para os filtros aplicados."
            icon="flag"
          />
        </AppSectionCard>

        <AppSectionCard v-if="podeOperacional" titulo="Violacoes de SLA" :subtitulo="`Chamados com violacao: ${violacoes.length}`">
          <q-table v-if="violacoes.length" class="sgx-table" flat bordered :rows="violacoes" :columns="colunasViolacoes" row-key="chamadoId" hide-pagination>
            <template #body-cell-prioridade="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.prioridade" />
              </q-td>
            </template>

            <template #body-cell-status="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.status" />
              </q-td>
            </template>

            <template #body-cell-dataLimiteSla="props">
              <q-td :props="props">{{ formatarData(props.row.dataLimiteSla) }}</q-td>
            </template>

            <template #body-cell-horasExcedidas="props">
              <q-td :props="props" class="text-right">{{ props.row.horasExcedidas == null ? '-' : Number(props.row.horasExcedidas).toFixed(2) }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem violacoes de SLA"
            mensagem="Nao ha violacoes de SLA para os filtros informados."
            icon="task_alt"
          />
        </AppSectionCard>

        <EmptyState
          v-if="!resumo && !porDepartamento.length && !porPrioridade.length && !violacoes.length"
          titulo="Sem dados de SLA"
          mensagem="Nao ha resultados para os filtros informados."
          icon="schedule"
        />
      </template>
    </template>
  </q-page>
</template>
