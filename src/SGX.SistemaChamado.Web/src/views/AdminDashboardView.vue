<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { QTableColumn } from 'quasar'
import FiltrosDashboardAdmin from '../components/admin/FiltrosDashboardAdmin.vue'
import StatusProcessamentoEmailBadge from '../components/admin/StatusProcessamentoEmailBadge.vue'
import TabelaChamados from '../components/admin/TabelaChamados.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { adminService } from '../services/adminService'
import { dashboardAdminService } from '../services/dashboardAdminService'
import { integracoesEmailService } from '../services/integracoesEmailService'
import type { AdminContextoResponse, ChamadoAdminResumo } from '../types/admin'
import type { DashboardAdminResponse } from '../types/dashboard'
import type { LogIntegracaoEmailResumoResponse } from '../types/integracaoEmail'
import type { ChamadosPorNatureza, FiltroIndicadoresRequest, ProdutividadeAtendente } from '../types/indicadores'
import { NaturezaChamado } from '../types/portal'

const router = useRouter()

const loadingDashboard = ref(false)
const loadingFila = ref(false)
const loadingEmail = ref(false)
const processandoAssumir = ref(false)

const erroDashboard = ref<string | null>(null)
const erroFila = ref<string | null>(null)
const erroEmail = ref<string | null>(null)
const sucessoFila = ref<string | null>(null)

const contexto = ref<AdminContextoResponse | null>(null)
const dashboard = ref<DashboardAdminResponse | null>(null)
const filtrosDashboard = ref<FiltroIndicadoresRequest>({})

const filaChamados = ref<ChamadoAdminResumo[]>([])
const totalFila = ref(0)
const paginaFila = ref(1)
const tamanhoPaginaFila = ref(10)
const filtrosFila = reactive({
  texto: '',
  statusId: '',
  naturezaChamado: '' as '' | NaturezaChamado,
})

const logsEmail = ref<LogIntegracaoEmailResumoResponse[]>([])

const produtividadeColumns: QTableColumn<ProdutividadeAtendente>[] = [
  { name: 'responsavel', label: 'Atendente', field: 'responsavelNome', align: 'left', sortable: true },
  { name: 'atendidos', label: 'Total atendidos', field: 'totalAtendidos', align: 'right', sortable: true },
  { name: 'encerrados', label: 'Total encerrados', field: 'totalEncerrados', align: 'right', sortable: true },
  { name: 'vencidos', label: 'Total vencidos', field: 'totalVencidos', align: 'right', sortable: true },
  { name: 'media', label: 'Média de resolução', field: 'mediaHorasResolucao', align: 'right', sortable: true },
]

const totalPaginasFila = computed(() => Math.max(1, Math.ceil(totalFila.value / tamanhoPaginaFila.value)))

const cardsExecutivos = computed(() => {
  if (!dashboard.value) {
    return []
  }

  const totalCriticos = dashboard.value.totalVencidos + dashboard.value.totalProximosDoVencimento

  return [
    {
      chave: 'abertos',
      titulo: 'Chamados abertos',
      valor: dashboard.value.totalAbertos,
      subtitulo: `${dashboard.value.totalSemResponsavel} sem responsável`,
      icon: 'drafts',
      tone: 'primary' as const,
    },
    {
      chave: 'atendimento',
      titulo: 'Em atendimento',
      valor: dashboard.value.totalEmAtendimento,
      subtitulo: 'Tratativas em andamento',
      icon: 'support_agent',
      tone: 'info' as const,
    },
    {
      chave: 'criticos',
      titulo: 'Chamados críticos',
      valor: totalCriticos,
      subtitulo: 'Vencidos + em risco de SLA',
      icon: 'priority_high',
      tone: totalCriticos > 0 ? ('negative' as const) : ('warning' as const),
    },
    {
      chave: 'vencidos',
      titulo: 'SLA vencido',
      valor: dashboard.value.totalVencidos,
      subtitulo: 'Exigem ação imediata',
      icon: 'warning',
      tone: 'negative' as const,
    },
    {
      chave: 'risco-sla',
      titulo: 'SLA em risco',
      valor: dashboard.value.totalProximosDoVencimento,
      subtitulo: 'Próximos do vencimento',
      icon: 'schedule',
      tone: 'warning' as const,
    },
    {
      chave: 'concluidos',
      titulo: 'Concluídos no período',
      valor: dashboard.value.totalEncerradosPeriodo,
      subtitulo: 'Encerramentos realizados',
      icon: 'task_alt',
      tone: 'positive' as const,
    },
    {
      chave: 'aguardando',
      titulo: 'Aguardando solicitante',
      valor: dashboard.value.totalAguardandoSolicitante,
      subtitulo: 'Dependentes de retorno',
      icon: 'hourglass_top',
      tone: 'warning' as const,
    },
    {
      chave: 'cumprimento-sla',
      titulo: 'Cumprimento de SLA',
      valor: `${dashboard.value.indicadoresSla.percentualCumprimento}%`,
      subtitulo: 'Indicador consolidado',
      icon: 'query_stats',
      tone: 'primary' as const,
    },
  ]
})

const semDadosDashboard = computed(() => {
  if (!dashboard.value) {
    return false
  }

  return cardsExecutivos.value.every((card) => {
    if (typeof card.valor === 'string') {
      return card.valor === '0%'
    }

    return card.valor === 0
  })
})

const totalStatus = computed(
  () => dashboard.value?.chamadosPorStatus.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalPrioridade = computed(
  () => dashboard.value?.chamadosPorPrioridade.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalCategoria = computed(
  () => dashboard.value?.chamadosPorCategoria.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalNatureza = computed(
  () => dashboard.value?.chamadosPorNatureza.reduce((acc, item) => acc + item.total, 0) ?? 0
)

const naturezasOrdenadas = [
  { codigo: NaturezaChamado.Incidente, nome: 'Incidente' },
  { codigo: NaturezaChamado.Requisicao, nome: 'Requisicao' },
  { codigo: NaturezaChamado.Mudanca, nome: 'Mudanca' },
  { codigo: NaturezaChamado.Problema, nome: 'Problema' },
  { codigo: NaturezaChamado.EventoAlerta, nome: 'Evento/Alerta' },
  { codigo: NaturezaChamado.TarefaOperacional, nome: 'Tarefa operacional' },
] as const

const chamadosPorNaturezaOrdenado = computed<ChamadosPorNatureza[]>(() => {
  const atual = dashboard.value?.chamadosPorNatureza ?? []
  const totais = new Map<number, number>(atual.map((item) => [item.codigo, item.total]))

  return naturezasOrdenadas.map((item) => ({
    codigo: item.codigo,
    natureza: item.nome,
    total: totais.get(item.codigo) ?? 0,
  }))
})

const aprovacoesPendentesFila = computed(() => filaChamados.value.filter((item) => item.aprovacaoPendente).length)
const chamadosCriticosFila = computed(() =>
  filaChamados.value.filter((item) => item.slaVencido || item.slaProximoVencimento).length
)
const chamadosSemResponsavelFila = computed(() =>
  filaChamados.value.filter((item) => !item.responsavelNome).length
)
const logsComErro = computed(() => logsEmail.value.filter((item) => Boolean(item.erroResumido)).length)

const isAdministrador = computed(() => contexto.value?.usuario.perfis.includes('Administrador') ?? false)

const atalhosRapidos = [
  {
    titulo: 'Fila de chamados',
    descricao: 'Acompanhar backlog e distribuição.',
    icon: 'support_agent',
    rota: '/admin/chamados',
  },
  {
    titulo: 'Painel de SLA',
    descricao: 'Monitorar riscos, médias e vencimentos.',
    icon: 'monitoring',
    rota: '/admin/sla/painel',
  },
  {
    titulo: 'Roadmap ITSM',
    descricao: 'Evolução de iniciativas estratégicas.',
    icon: 'account_tree',
    rota: '/admin/gestao-itsm/roadmap',
  },
  {
    titulo: 'Relatórios avançados',
    descricao: 'Análises executivas e operacionais.',
    icon: 'analytics',
    rota: '/admin/relatorios/avancados',
  },
  {
    titulo: 'Aprovações de chamados',
    descricao: 'Pendências para decisão e liberação.',
    icon: 'fact_check',
    rota: '/admin/atendimento/aprovacao-chamados',
  },
  {
    titulo: 'Auditoria',
    descricao: 'Rastreabilidade e governança da operação.',
    icon: 'manage_search',
    rota: '/admin/governanca/auditoria',
  },
]

function formatarData(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

function formatHoras(valor: number | null): string {
  if (valor === null || Number.isNaN(valor)) {
    return '-'
  }

  return `${valor.toFixed(2)}h`
}

function percentual(parte: number, total: number): number {
  if (total <= 0) {
    return 0
  }

  return parte / total
}

async function carregarContexto(): Promise<void> {
  if (contexto.value) {
    return
  }

  contexto.value = await adminService.obterAdminContexto()
}

async function carregarDashboard(): Promise<void> {
  loadingDashboard.value = true
  erroDashboard.value = null

  try {
    await carregarContexto()
    dashboard.value = await dashboardAdminService.obterDashboard(filtrosDashboard.value)
  } catch (error) {
    erroDashboard.value =
      error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  } finally {
    loadingDashboard.value = false
  }
}

async function carregarFilaChamados(): Promise<void> {
  loadingFila.value = true
  erroFila.value = null

  try {
    const response = await adminService.listarChamadosAdmin({
      texto: filtrosFila.texto || undefined,
      statusId: filtrosFila.statusId || undefined,
      naturezaChamado: filtrosFila.naturezaChamado || undefined,
      pagina: paginaFila.value,
      tamanhoPagina: tamanhoPaginaFila.value,
      ordenarPor: 'atualizadoEm',
      direcaoOrdenacao: 'desc',
    })

    filaChamados.value = response.items
    totalFila.value = response.total
  } catch (error) {
    erroFila.value = error instanceof Error ? error.message : 'Não foi possível carregar os chamados.'
  } finally {
    loadingFila.value = false
  }
}

async function carregarResumoEmail(): Promise<void> {
  loadingEmail.value = true
  erroEmail.value = null

  try {
    const response = await integracoesEmailService.listarLogs({
      pagina: 1,
      tamanhoPagina: 5,
    })
    logsEmail.value = response.items
  } catch (error) {
    erroEmail.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  } finally {
    loadingEmail.value = false
  }
}

async function carregarTudo(): Promise<void> {
  await Promise.all([carregarDashboard(), carregarFilaChamados(), carregarResumoEmail()])
}

async function aplicarFiltrosDashboard(novosFiltros: FiltroIndicadoresRequest): Promise<void> {
  filtrosDashboard.value = { ...novosFiltros }
  await carregarDashboard()
}

async function aplicarFiltroFila(): Promise<void> {
  paginaFila.value = 1
  sucessoFila.value = null
  await carregarFilaChamados()
}

async function limparFiltroFila(): Promise<void> {
  filtrosFila.texto = ''
  filtrosFila.statusId = ''
  filtrosFila.naturezaChamado = ''
  paginaFila.value = 1
  sucessoFila.value = null
  await carregarFilaChamados()
}

async function alterarPaginaFila(novaPagina: number): Promise<void> {
  paginaFila.value = novaPagina
  await carregarFilaChamados()
}

async function assumirChamado(id: string): Promise<void> {
  processandoAssumir.value = true
  erroFila.value = null

  try {
    await adminService.assumirChamado(id)
    sucessoFila.value = 'Chamado assumido com sucesso.'
    await Promise.all([carregarFilaChamados(), carregarDashboard()])
  } catch (error) {
    erroFila.value = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
  } finally {
    processandoAssumir.value = false
  }
}

onMounted(() => {
  void carregarTudo()
})
</script>

<template>
  <q-page class="sgx-page dashboard-page column q-gutter-md">
    <PageHeader
      titulo="Dashboard Executivo"
      subtitulo="Visão consolidada da operação de atendimento, SLA e produtividade do service desk."
    >
      <template #actions>
        <div class="row q-gutter-sm items-center wrap">
          <q-chip color="blue-1" text-color="primary" icon="assignment" square>
            Fila atual: {{ totalFila }}
          </q-chip>
          <q-btn
            color="primary"
            icon="support_agent"
            label="Abrir fila"
            @click="router.push('/admin/chamados')"
          />
          <q-btn
            outline
            color="primary"
            icon="analytics"
            label="Relatórios"
            @click="router.push('/admin/relatorios/avancados')"
          />
        </div>
      </template>
    </PageHeader>

    <AppSectionCard
      titulo="Visão geral operacional"
      subtitulo="Ajuste o período e os recortes para atualizar os indicadores executivos."
    >
      <FilterBar compact>
        <FiltrosDashboardAdmin :contexto="contexto" :loading="loadingDashboard" @filtrar="aplicarFiltrosDashboard" />
      </FilterBar>
    </AppSectionCard>

    <ErrorState v-if="erroDashboard" :mensagem="erroDashboard" @retry="carregarDashboard" />

    <LoadingState
      v-else-if="loadingDashboard"
      inline
      mensagem="Carregando dashboard administrativo..."
    />

    <template v-else-if="dashboard">
      <q-banner rounded class="bg-blue-1 text-primary dashboard-banner">
        <div class="text-weight-bold">Resumo operacional do período</div>
        <div class="text-caption">
          Total monitorado: {{ dashboard.indicadoresSla.totalChamados }} chamados com SLA,
          {{ dashboard.totalVencidos }} vencidos e {{ dashboard.totalProximosDoVencimento }} em risco.
        </div>
      </q-banner>

      <div class="sgx-kpi-grid">
        <div v-for="card in cardsExecutivos" :key="card.chave">
          <MetricCard
            :titulo="card.titulo"
            :valor="card.valor"
            :subtitulo="card.subtitulo"
            :icon="card.icon"
            :tone="card.tone"
          />
        </div>
      </div>

      <div class="dashboard-triple-grid">
        <AppSectionCard titulo="Distribuição por status" subtitulo="Volume e participação por etapa do atendimento.">
          <EmptyState
            v-if="!dashboard.chamadosPorStatus.length"
            titulo="Sem dados de status"
            mensagem="Não há dados para o período selecionado."
          />

          <q-list v-else separator>
            <q-item v-for="item in dashboard.chamadosPorStatus" :key="item.status">
              <q-item-section>
                <StatusBadge :texto="item.status" />
                <q-linear-progress
                  class="q-mt-xs"
                  rounded
                  size="8px"
                  color="primary"
                  :value="percentual(item.total, totalStatus)"
                />
              </q-item-section>
              <q-item-section side>{{ item.total }}</q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>

        <AppSectionCard titulo="Distribuição por prioridade" subtitulo="Classificação dos chamados por criticidade.">
          <EmptyState
            v-if="!dashboard.chamadosPorPrioridade.length"
            titulo="Sem dados de prioridade"
            mensagem="Não há dados para o período selecionado."
          />

          <q-list v-else separator>
            <q-item v-for="item in dashboard.chamadosPorPrioridade" :key="item.prioridade">
              <q-item-section>
                <PrioridadeBadge :texto="item.prioridade" />
                <q-linear-progress
                  class="q-mt-xs"
                  rounded
                  size="8px"
                  color="orange-8"
                  :value="percentual(item.total, totalPrioridade)"
                />
              </q-item-section>
              <q-item-section side>{{ item.total }}</q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>

        <AppSectionCard titulo="Distribuição por categoria" subtitulo="Principais frentes de demanda na operação.">
          <EmptyState
            v-if="!dashboard.chamadosPorCategoria.length"
            titulo="Sem dados por categoria"
            mensagem="Não há dados para o período selecionado."
          />

          <q-list v-else separator>
            <q-item v-for="item in dashboard.chamadosPorCategoria" :key="item.categoria">
              <q-item-section>
                <div class="text-body2 text-weight-medium">{{ item.categoria }}</div>
                <q-linear-progress
                  class="q-mt-xs"
                  rounded
                  size="8px"
                  color="teal"
                  :value="percentual(item.total, totalCategoria)"
                />
              </q-item-section>
              <q-item-section side>{{ item.total }}</q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>

        <AppSectionCard titulo="Distribuicao por natureza ITSM" subtitulo="Volume por tipo de chamado no periodo filtrado.">
          <EmptyState
            v-if="!chamadosPorNaturezaOrdenado.length"
            titulo="Sem dados por natureza"
            mensagem="Nao ha dados para o periodo selecionado."
          />

          <q-list v-else separator>
            <q-item v-for="item in chamadosPorNaturezaOrdenado" :key="item.codigo">
              <q-item-section>
                <div class="text-body2 text-weight-medium">{{ item.natureza }}</div>
                <q-linear-progress
                  class="q-mt-xs"
                  rounded
                  size="8px"
                  color="indigo"
                  :value="percentual(item.total, totalNatureza)"
                />
              </q-item-section>
              <q-item-section side>{{ item.total }}</q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>
      </div>

      <div class="dashboard-main-grid">
        <AppSectionCard titulo="Indicadores de SLA" subtitulo="Cumprimento, risco e tempos médios de resposta.">
          <div class="q-mb-sm">
            <SlaBadge
              :vencido="dashboard.indicadoresSla.totalVencidos > 0"
              :proximo="dashboard.indicadoresSla.totalVencidos === 0 && dashboard.indicadoresSla.totalProximosDoVencimento > 0"
              :pausado="false"
            />
          </div>

          <q-list separator>
            <q-item>
              <q-item-section>Total de chamados</q-item-section>
              <q-item-section side>{{ dashboard.indicadoresSla.totalChamados }}</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Dentro do prazo</q-item-section>
              <q-item-section side>{{ dashboard.indicadoresSla.totalDentroDoPrazo }}</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Vencidos</q-item-section>
              <q-item-section side>{{ dashboard.indicadoresSla.totalVencidos }}</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Próximos do vencimento</q-item-section>
              <q-item-section side>{{ dashboard.indicadoresSla.totalProximosDoVencimento }}</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Percentual de cumprimento</q-item-section>
              <q-item-section side>{{ dashboard.indicadoresSla.percentualCumprimento }}%</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Média de resolução</q-item-section>
              <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasResolucao) }}</q-item-section>
            </q-item>
            <q-item>
              <q-item-section>Média de primeira resposta</q-item-section>
              <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasPrimeiraResposta) }}</q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>

        <AppSectionCard titulo="Produtividade por atendente" subtitulo="Ranking de atendimento e eficiência operacional.">
          <EmptyState
            v-if="!dashboard.produtividadePorAtendente.length"
            titulo="Sem produtividade registrada"
            mensagem="Ainda não há dados de produtividade para o período selecionado."
          />

          <q-table
            v-else
            class="produtividade-table"
            flat
            :rows="dashboard.produtividadePorAtendente"
            :columns="produtividadeColumns"
            row-key="responsavelId"
            :rows-per-page-options="[10, 20, 50]"
          >
            <template #body-cell-media="slotProps">
              <q-td :props="slotProps">{{ formatHoras(slotProps.row.mediaHorasResolucao) }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>
      </div>

      <div class="dashboard-bottom-grid">
        <AppSectionCard
          titulo="Fila de atendimento"
          subtitulo="Triagem operacional com filtros e priorização do backlog."
        >
          <template #actions>
            <q-btn flat color="primary" icon="open_in_new" label="Ver todas" @click="router.push('/admin/chamados')" />
          </template>

          <FilterBar class="q-mb-md" compact>
            <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltroFila">
              <div class="col-12 col-md-5">
                <q-input
                  v-model="filtrosFila.texto"
                  outlined
                  dense
                  label="Buscar na fila"
                  placeholder="Código, título ou descrição"
                />
              </div>
              <div class="col-12 col-md-4">
                <q-select
                  v-model="filtrosFila.statusId"
                  outlined
                  dense
                  clearable
                  emit-value
                  map-options
                  label="Status"
                  :options="contexto?.status.map((item) => ({ label: item.nome, value: item.id })) ?? []"
                />
              </div>
              <div class="col-12 col-md-3">
                <q-select
                  v-model="filtrosFila.naturezaChamado"
                  outlined
                  dense
                  clearable
                  emit-value
                  map-options
                  label="Natureza ITSM"
                  :options="[
                    { label: 'Todos', value: undefined },
                    { label: 'Incidente', value: NaturezaChamado.Incidente },
                    { label: 'Requisicao', value: NaturezaChamado.Requisicao },
                    { label: 'Mudanca', value: NaturezaChamado.Mudanca },
                    { label: 'Problema', value: NaturezaChamado.Problema },
                    { label: 'Evento/Alerta', value: NaturezaChamado.EventoAlerta },
                    { label: 'Tarefa operacional', value: NaturezaChamado.TarefaOperacional },
                  ]"
                />
              </div>
              <div class="col-12 col-md-12 row justify-end q-gutter-sm">
                <q-btn flat color="primary" label="Limpar" :disable="loadingFila" @click="limparFiltroFila" />
                <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loadingFila" />
              </div>
            </q-form>
          </FilterBar>

          <q-banner v-if="sucessoFila" rounded class="bg-green-1 text-positive q-mb-sm">
            {{ sucessoFila }}
          </q-banner>

          <ErrorState v-if="erroFila" :mensagem="erroFila" @retry="carregarFilaChamados" />

          <LoadingState
            v-else-if="loadingFila && !filaChamados.length"
            inline
            mensagem="Carregando fila de chamados..."
          />

          <EmptyState
            v-else-if="!filaChamados.length"
            titulo="Fila sem chamados"
            mensagem="Nenhum resultado corresponde aos filtros aplicados."
          />

          <template v-else>
            <TabelaChamados
              :rows="filaChamados"
              :loading="loadingFila || processandoAssumir"
              :can-force-assume="isAdministrador"
              @detalhar="(id) => router.push(`/admin/chamados/${id}`)"
              @assumir="assumirChamado"
            />

            <div class="row justify-end q-mt-md">
              <q-pagination
                v-model="paginaFila"
                color="primary"
                boundary-numbers
                direction-links
                :max="totalPaginasFila"
                :max-pages="7"
                @update:model-value="alterarPaginaFila"
              />
            </div>
          </template>
        </AppSectionCard>

        <div class="dashboard-side-stack">
          <AppSectionCard titulo="Atalhos rápidos" subtitulo="Navegação executiva para módulos estratégicos.">
            <q-list separator>
              <q-item
                v-for="atalho in atalhosRapidos"
                :key="atalho.rota"
                clickable
                class="atalho-item"
                @click="router.push(atalho.rota)"
              >
                <q-item-section avatar>
                  <q-avatar size="34px" color="blue-1" text-color="primary">
                    <q-icon :name="atalho.icon" />
                  </q-avatar>
                </q-item-section>
                <q-item-section>
                  <q-item-label class="text-body2 text-weight-medium">{{ atalho.titulo }}</q-item-label>
                  <q-item-label caption>{{ atalho.descricao }}</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-icon name="arrow_forward" color="primary" />
                </q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>

          <AppSectionCard titulo="Aprovações e alertas" subtitulo="Visão baseada nos dados carregados da fila e integração.">
            <q-list separator>
              <q-item>
                <q-item-section>
                  <q-item-label>Aprovações pendentes (fila atual)</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge color="warning" text-color="dark">{{ aprovacoesPendentesFila }}</q-badge>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label>Chamados críticos (fila atual)</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge color="negative">{{ chamadosCriticosFila }}</q-badge>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label>Sem responsável (fila atual)</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge color="primary">{{ chamadosSemResponsavelFila }}</q-badge>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label>Erros na integração de e-mail (amostra)</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge color="orange-8">{{ logsComErro }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>

            <div class="text-caption text-grey-7 q-mt-sm">
              Indicadores desta seção usam os dados reais já carregados na tela atual.
            </div>
          </AppSectionCard>

          <AppSectionCard
            titulo="Integração de e-mail"
            subtitulo="Últimos processamentos para monitorar ingestão automática."
          >
            <template #actions>
              <q-btn
                flat
                color="primary"
                icon="mail"
                label="Ver todas"
                @click="router.push('/admin/integracoes/email')"
              />
            </template>

            <ErrorState v-if="erroEmail" :mensagem="erroEmail" @retry="carregarResumoEmail" />

            <LoadingState
              v-else-if="loadingEmail"
              inline
              mensagem="Carregando resumo da integração de e-mail..."
            />

            <EmptyState
              v-else-if="!logsEmail.length"
              titulo="Sem logs recentes"
              mensagem="Nenhum log de e-mail encontrado."
              icon="mail_lock"
            />

            <q-list v-else separator>
              <q-item v-for="log in logsEmail" :key="log.id" clickable @click="router.push('/admin/integracoes/email')">
                <q-item-section>
                  <q-item-label class="text-body2 text-weight-medium">{{ log.remetente }}</q-item-label>
                  <q-item-label caption class="ellipsis">
                    {{ log.assunto || '(sem assunto)' }}
                  </q-item-label>
                  <q-item-label v-if="log.erroResumido" caption class="text-negative">
                    {{ log.erroResumido }}
                  </q-item-label>
                </q-item-section>

                <q-item-section side class="items-end q-gutter-xs">
                  <StatusProcessamentoEmailBadge :status="log.statusProcessamento" />
                  <span class="text-caption text-grey-7">{{ formatarData(log.dataRecebimento) }}</span>
                </q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
      </div>

      <EmptyState
        v-if="semDadosDashboard"
        titulo="Sem indicadores no período"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="analytics"
      />
    </template>
  </q-page>
</template>

<style scoped>
.dashboard-page {
  padding-bottom: 24px;
}

.dashboard-banner {
  border: 1px solid rgba(11, 94, 215, 0.16);
}

.dashboard-triple-grid {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: 16px;
}

.dashboard-main-grid {
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 16px;
}

.dashboard-bottom-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 16px;
}

.dashboard-side-stack {
  display: grid;
  gap: 16px;
  align-content: start;
}

.atalho-item {
  border-radius: var(--sgx-radius-sm);
}

.atalho-item:hover {
  background: rgba(11, 94, 215, 0.06);
}

:deep(.produtividade-table .q-table__middle) {
  overflow-x: auto;
}

@media (max-width: 1280px) {
  .dashboard-triple-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .dashboard-bottom-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 1024px) {
  .dashboard-triple-grid,
  .dashboard-main-grid,
  .dashboard-bottom-grid {
    grid-template-columns: minmax(0, 1fr);
  }

  .dashboard-side-stack {
    order: 2;
  }
}
</style>
