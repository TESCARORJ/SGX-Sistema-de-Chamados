<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
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
const erro = ref<string | null>(null)
const resumo = ref<RelatorioSlaResumo | null>(null)
const violacoes = ref<RelatorioSlaViolacao[]>([])
const porDepartamento = ref<RelatorioSlaPorDepartamento[]>([])
const porPrioridade = ref<RelatorioSlaPorPrioridade[]>([])

const filtros = reactive<FiltroRelatorioSla>({
  dataInicial: '',
  dataFinal: '',
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
  { name: 'horasExcedidas', label: 'Horas excedidas', field: 'horasExcedidas', align: 'right' as const },
]

const colunasCumprimento = [
  { name: 'nome', label: 'Grupo', field: 'nome', align: 'left' as const },
  { name: 'totalComSla', label: 'Total com SLA', field: 'totalComSla', align: 'right' as const },
  { name: 'dentroSla', label: 'Dentro SLA', field: 'dentroSla', align: 'right' as const },
  { name: 'foraSla', label: 'Fora SLA', field: 'foraSla', align: 'right' as const },
  { name: 'percentualCumprimento', label: '% cumprimento', field: 'percentualCumprimento', align: 'right' as const },
]

const vazio = computed(() => {
  const semResumo = !resumo.value
  return semResumo && !violacoes.value.length && !porDepartamento.value.length && !porPrioridade.value.length
})

function construirFiltros(): FiltroRelatorioSla {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
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

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - SLA" subtitulo="Cumprimento de prazos, violacoes e desempenho por grupo.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de SLA.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Recorte de periodo e dimensoes para os indicadores de SLA.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.departamentoId" outlined dense label="Departamento ID" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.prioridadeId" outlined dense label="Prioridade ID" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && vazio" mensagem="Carregando relatorios de SLA..." />

      <template v-else>
        <div v-if="resumo" class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total com SLA" :valor="resumo.totalChamadosComSla" icon="schedule" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Dentro SLA" :valor="resumo.totalDentroSla" icon="task_alt" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Fora SLA" :valor="resumo.totalForaSla" icon="warning" color="negative" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="% cumprimento" :valor="resumo.percentualCumprimento ?? '-'" icon="percent" color="info" /></div>
        </div>

        <AppSectionCard v-if="porDepartamento.length" titulo="SLA por departamento">
          <q-table flat :rows="porDepartamento.map((x) => ({ ...x, nome: x.departamentoNome }))" :columns="colunasCumprimento" row-key="departamentoNome" hide-pagination>
            <template #body-cell-percentualCumprimento="props">
              <q-td :props="props" class="text-right">{{ props.row.percentualCumprimento == null ? '-' : `${Number(props.row.percentualCumprimento).toFixed(2)}%` }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <AppSectionCard v-if="porPrioridade.length" titulo="SLA por prioridade">
          <q-table flat :rows="porPrioridade.map((x) => ({ ...x, nome: x.prioridadeNome }))" :columns="colunasCumprimento" row-key="prioridadeId" hide-pagination>
            <template #body-cell-percentualCumprimento="props">
              <q-td :props="props" class="text-right">{{ props.row.percentualCumprimento == null ? '-' : `${Number(props.row.percentualCumprimento).toFixed(2)}%` }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <AppSectionCard v-if="violacoes.length" titulo="Violacoes de SLA">
          <q-table flat :rows="violacoes" :columns="colunasViolacoes" row-key="chamadoId" hide-pagination>
            <template #body-cell-horasExcedidas="props">
              <q-td :props="props" class="text-right">{{ props.row.horasExcedidas == null ? '-' : Number(props.row.horasExcedidas).toFixed(2) }}</q-td>
            </template>
          </q-table>
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
