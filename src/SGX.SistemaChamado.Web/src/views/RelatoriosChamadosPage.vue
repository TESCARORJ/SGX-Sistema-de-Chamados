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
import { NaturezaChamado } from '../types/portal'
import type {
  FiltroRelatorioChamados,
  RelatorioAtendimentoProdutividade,
  RelatorioChamadosDistribuicao,
  RelatorioChamadosResumo,
  RelatorioChamadosSerieTemporal,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioChamadosResumo | null>(null)
const serieTemporal = ref<RelatorioChamadosSerieTemporal | null>(null)
const distribuicao = ref<RelatorioChamadosDistribuicao | null>(null)
const produtividade = ref<RelatorioAtendimentoProdutividade | null>(null)

const departamentos = ref<{ id: string; nome: string }[]>([])
const categorias = ref<{ id: string; nome: string }[]>([])
const prioridades = ref<{ id: string; nome: string }[]>([])
const status = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioChamados & { naturezaChamado: '' | NaturezaChamado }>({
  dataInicial: '',
  dataFinal: '',
  departamentoId: '',
  categoriaId: '',
  prioridadeId: '',
  statusId: '',
  naturezaChamado: '',
  limiteRanking: 10,
  agruparPor: 1,
  agrupamento: 1,
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosVisualizar))
const podeOperacional = computed(() => possuiPermissao(permissoes.relatoriosAvancadosOperacional))
const podeExportar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosExportar))

const opcoesAgruparPor = [
  { label: 'Status', value: 1 },
  { label: 'Prioridade', value: 2 },
  { label: 'Departamento', value: 3 },
  { label: 'Categoria', value: 4 },
  { label: 'Natureza ITSM', value: 9 },
]

const opcoesAgrupamento = [
  { label: 'Diario', value: 1 },
  { label: 'Semanal', value: 2 },
  { label: 'Mensal', value: 3 },
]

const colunasSerie = [
  { name: 'periodo', label: 'Periodo', field: 'periodo', align: 'left' as const },
  { name: 'abertos', label: 'Abertos', field: 'abertos', align: 'right' as const },
  { name: 'encerrados', label: 'Encerrados', field: 'encerrados', align: 'right' as const },
  { name: 'reabertos', label: 'Reabertos', field: 'reabertos', align: 'right' as const },
]

const colunasDistribuicao = [
  { name: 'nome', label: 'Grupo', field: 'nome', align: 'left' as const },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade', align: 'right' as const },
  { name: 'percentual', label: 'Percentual', field: 'percentual', align: 'right' as const },
]

const colunasProdutividade = [
  { name: 'atendenteNome', label: 'Atendente', field: 'atendenteNome', align: 'left' as const },
  { name: 'chamadosAssumidos', label: 'Assumidos', field: 'chamadosAssumidos', align: 'right' as const },
  { name: 'chamadosConcluidos', label: 'Concluidos', field: 'chamadosConcluidos', align: 'right' as const },
  { name: 'chamadosEmAberto', label: 'Em aberto', field: 'chamadosEmAberto', align: 'right' as const },
  { name: 'percentualConclusao', label: '% conclusao', field: 'percentualConclusao', align: 'right' as const },
]

const colunasNatureza = [
  { name: 'nome', label: 'Natureza', field: 'nome', align: 'left' as const },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade', align: 'right' as const },
  { name: 'percentual', label: 'Percentual', field: 'percentual', align: 'right' as const },
]

const opcoesNatureza = [
  { label: 'Incidente', value: NaturezaChamado.Incidente },
  { label: 'Requisicao', value: NaturezaChamado.Requisicao },
  { label: 'Mudanca', value: NaturezaChamado.Mudanca },
  { label: 'Problema', value: NaturezaChamado.Problema },
  { label: 'Evento/Alerta', value: NaturezaChamado.EventoAlerta },
  { label: 'Tarefa operacional', value: NaturezaChamado.TarefaOperacional },
]

const totalPorNatureza = computed(() => resumo.value?.totalPorNatureza ?? [])

const semResultados = computed(() => {
  return (
    !resumo.value ||
    (resumo.value.totalChamados === 0 &&
      (serieTemporal.value?.itens.length ?? 0) === 0 &&
      (distribuicao.value?.itens.length ?? 0) === 0 &&
      (produtividade.value?.ranking.length ?? 0) === 0)
  )
})

function construirFiltros(): FiltroRelatorioChamados {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    departamentoId: filtros.departamentoId || undefined,
    categoriaId: filtros.categoriaId || undefined,
    prioridadeId: filtros.prioridadeId || undefined,
    statusId: filtros.statusId || undefined,
    naturezaChamado: filtros.naturezaChamado || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
    agruparPor: filtros.agruparPor || undefined,
    agrupamento: filtros.agrupamento || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.departamentoId = ''
  filtros.categoriaId = ''
  filtros.prioridadeId = ''
  filtros.statusId = ''
  filtros.naturezaChamado = ''
  filtros.limiteRanking = 10
  filtros.agruparPor = 1
  filtros.agrupamento = 1
}

function formatarHoras(valor: number | null): string {
  if (valor === null || valor === undefined) {
    return '-'
  }

  return `${Number(valor).toFixed(2)}h`
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const contexto = await adminService.obterAdminContexto()
    departamentos.value = contexto.departamentos.map((item) => ({ id: item.id, nome: item.nome }))
    categorias.value = contexto.categorias.map((item) => ({ id: item.id, nome: item.nome }))
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
    const [resumoResp, serieResp, distribuicaoResp, produtividadeResp] = await Promise.all([
      relatoriosAvancadosAdminService.obterResumoChamados(request),
      relatoriosAvancadosAdminService.obterSerieTemporalChamados(request),
      relatoriosAvancadosAdminService.obterDistribuicaoChamados(request),
      podeOperacional.value
        ? relatoriosAvancadosAdminService.obterProdutividadeAtendimento(request)
        : Promise.resolve(null),
    ])

    resumo.value = resumoResp
    serieTemporal.value = serieResp
    distribuicao.value = distribuicaoResp
    produtividade.value = produtividadeResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar o relatorio de chamados.'
  } finally {
    loading.value = false
  }
}

function exportarDados(): void {
  if (!podeExportar.value || !resumo.value) {
    return
  }

  const linhas = [
    { grupo: 'Resumo', indicador: 'Total chamados', valor: resumo.value.totalChamados },
    { grupo: 'Resumo', indicador: 'Total abertos', valor: resumo.value.totalAbertos },
    { grupo: 'Resumo', indicador: 'Total em atendimento', valor: resumo.value.totalEmAtendimento },
    { grupo: 'Resumo', indicador: 'Total encerrados', valor: resumo.value.totalEncerradosOuConcluidos },
    { grupo: 'Resumo', indicador: 'Total cancelados', valor: resumo.value.totalCancelados },
    { grupo: 'Resumo', indicador: 'Total reabertos', valor: resumo.value.totalReabertos },
    ...resumo.value.totalPorNatureza.map((item) => ({ grupo: 'Natureza ITSM', indicador: item.nome, valor: item.quantidade })),
    ...((distribuicao.value?.itens ?? []).map((item) => ({ grupo: 'Distribuicao', indicador: item.nome, valor: item.quantidade }))),
    ...((serieTemporal.value?.itens ?? []).map((item) => ({ grupo: 'Serie temporal', indicador: item.periodo, valor: item.abertos + item.encerrados + item.reabertos }))),
  ]

  exportarCsv('relatorio-chamados.csv', linhas)
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
    <PageHeader titulo="Relatorios - Chamados" subtitulo="Resumo executivo, distribuicoes e produtividade de atendimento." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn
            v-if="podeExportar"
            color="secondary"
            icon="download"
            label="Exportar CSV"
            :disable="!resumo"
            @click="exportarDados"
          />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de chamados.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Ajuste periodo e recortes de chamados para analise operacional e gerencial.">
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
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.naturezaChamado"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Natureza ITSM"
                :options="opcoesNatureza"
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
            <div class="col-12 col-md-2">
              <q-select v-model="filtros.agruparPor" outlined dense emit-value map-options label="Distribuicao por" :options="opcoesAgruparPor" />
            </div>
            <div class="col-12 col-md-2">
              <q-select v-model="filtros.agrupamento" outlined dense emit-value map-options label="Serie temporal" :options="opcoesAgrupamento" />
            </div>
            <div class="col-12 col-md-2">
              <q-input v-model.number="filtros.limiteRanking" type="number" min="1" max="100" outlined dense label="Limite ranking" />
            </div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorio de chamados..." />

      <template v-else-if="resumo">
        <div class="sgx-kpi-grid">
          <MetricCard titulo="Total chamados" :valor="resumo.totalChamados" icon="support_agent" tone="primary" />
          <MetricCard titulo="Abertos" :valor="resumo.totalAbertos" icon="inbox" tone="warning" />
          <MetricCard titulo="Em atendimento" :valor="resumo.totalEmAtendimento" icon="support_agent" tone="info" />
          <MetricCard titulo="Encerrados" :valor="resumo.totalEncerradosOuConcluidos" icon="task_alt" tone="positive" />
          <MetricCard titulo="Reabertos" :valor="resumo.totalReabertos" icon="restart_alt" tone="negative" />
          <MetricCard titulo="Tempo medio atendimento" :valor="formatarHoras(resumo.tempoMedioAtendimentoHoras)" icon="timer" tone="primary" />
        </div>

        <AppSectionCard titulo="Serie temporal" :subtitulo="`Pontos gerados: ${serieTemporal?.itens.length ?? 0}`">
          <q-table v-if="serieTemporal?.itens?.length" class="sgx-table" flat bordered :rows="serieTemporal.itens" :columns="colunasSerie" row-key="periodo" hide-pagination />
          <EmptyState
            v-else
            titulo="Sem serie temporal"
            mensagem="Nao ha pontos de serie temporal para os filtros aplicados."
            icon="timeline"
          />
        </AppSectionCard>

        <AppSectionCard titulo="Consolidado por natureza ITSM" :subtitulo="`Naturezas monitoradas: ${totalPorNatureza.length}`">
          <q-table v-if="totalPorNatureza.length" class="sgx-table" flat bordered :rows="totalPorNatureza" :columns="colunasNatureza" row-key="chave" hide-pagination>
            <template #body-cell-nome="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.nome" />
              </q-td>
            </template>
            <template #body-cell-percentual="props">
              <q-td :props="props" class="text-right">{{ props.row.percentual === null ? '-' : `${Number(props.row.percentual).toFixed(2)}%` }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem consolidado por natureza"
            mensagem="Nao ha dados por natureza ITSM para os filtros aplicados."
            icon="category"
          />
        </AppSectionCard>

        <AppSectionCard titulo="Distribuicao" :subtitulo="`Grupos encontrados: ${distribuicao?.itens.length ?? 0}`">
          <q-table v-if="distribuicao?.itens?.length" class="sgx-table" flat bordered :rows="distribuicao.itens" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
            <template #body-cell-nome="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.nome" />
              </q-td>
            </template>

            <template #body-cell-percentual="props">
              <q-td :props="props" class="text-right">{{ Number(props.row.percentual).toFixed(2) }}%</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem distribuicao"
            mensagem="Nenhum grupo de distribuicao foi retornado para os filtros atuais."
            icon="donut_large"
          />
        </AppSectionCard>

        <AppSectionCard v-if="podeOperacional" titulo="Produtividade de atendimento" :subtitulo="`Atendentes no ranking: ${produtividade?.ranking.length ?? 0}`">
          <q-table v-if="produtividade?.ranking?.length" class="sgx-table" flat bordered :rows="produtividade.ranking" :columns="colunasProdutividade" row-key="atendenteId" hide-pagination>
            <template #body-cell-percentualConclusao="props">
              <q-td :props="props" class="text-right">{{ Number(props.row.percentualConclusao).toFixed(2) }}%</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem produtividade"
            mensagem="Nao ha dados de produtividade para os filtros aplicados."
            icon="trending_up"
          />
        </AppSectionCard>

        <EmptyState
          v-if="semResultados"
          titulo="Sem dados de chamados"
          mensagem="Nao ha resultados para os filtros informados."
          icon="support_agent"
        />
      </template>
    </template>
  </q-page>
</template>

<style scoped>
:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}
</style>
