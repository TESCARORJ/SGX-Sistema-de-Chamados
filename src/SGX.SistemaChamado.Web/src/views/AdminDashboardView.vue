<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { QTableColumn } from 'quasar'
import FiltrosDashboardAdmin from '../components/admin/FiltrosDashboardAdmin.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { adminService } from '../services/adminService'
import { dashboardAdminService } from '../services/dashboardAdminService'
import type { AdminContextoResponse } from '../types/admin'
import type { DashboardAdminResponse } from '../types/dashboard'
import type { FiltroIndicadoresRequest, ProdutividadeAtendente } from '../types/indicadores'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const contexto = ref<AdminContextoResponse | null>(null)
const dashboard = ref<DashboardAdminResponse | null>(null)
const filtros = ref<FiltroIndicadoresRequest>({})

const produtividadeColumns: QTableColumn<ProdutividadeAtendente>[] = [
  { name: 'responsavel', label: 'Atendente', field: 'responsavelNome', align: 'left', sortable: true },
  { name: 'atendidos', label: 'Total atendidos', field: 'totalAtendidos', align: 'right', sortable: true },
  { name: 'encerrados', label: 'Total encerrados', field: 'totalEncerrados', align: 'right', sortable: true },
  { name: 'vencidos', label: 'Total vencidos', field: 'totalVencidos', align: 'right', sortable: true },
  { name: 'media', label: 'Media de resolucao', field: 'mediaHorasResolucao', align: 'right', sortable: true },
]

const cardsObrigatorios = computed(() => {
  if (!dashboard.value) {
    return []
  }

  return [
    { chave: 'abertos', titulo: 'Chamados abertos', valor: dashboard.value.totalAbertos, icon: 'drafts', color: 'primary' },
    {
      chave: 'atendimento',
      titulo: 'Em atendimento',
      valor: dashboard.value.totalEmAtendimento,
      icon: 'support_agent',
      color: 'warning',
    },
    {
      chave: 'aguardando',
      titulo: 'Aguardando solicitante',
      valor: dashboard.value.totalAguardandoSolicitante,
      icon: 'hourglass_top',
      color: 'deep-orange',
    },
    { chave: 'sla-vencido', titulo: 'SLA vencido', valor: dashboard.value.totalVencidos, icon: 'warning', color: 'negative' },
    {
      chave: 'sla-proximo',
      titulo: 'Proximos do vencimento',
      valor: dashboard.value.totalProximosDoVencimento,
      icon: 'schedule',
      color: 'orange-8',
    },
    {
      chave: 'resolvidos-periodo',
      titulo: 'Resolvidos no periodo',
      valor: dashboard.value.totalResolvidosPeriodo,
      icon: 'task_alt',
      color: 'positive',
    },
    {
      chave: 'sem-responsavel',
      titulo: 'Sem responsavel',
      valor: dashboard.value.totalSemResponsavel,
      icon: 'person_off',
      color: 'grey-7',
    },
  ]
})

const semDadosDashboard = computed(() => {
  if (!dashboard.value) {
    return false
  }

  return cardsObrigatorios.value.every((card) => card.valor === 0)
})

function formatHoras(valor: number | null): string {
  if (valor === null || Number.isNaN(valor)) {
    return '-'
  }

  return `${valor.toFixed(2)}h`
}

async function carregarDashboard(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    if (!contexto.value) {
      contexto.value = await adminService.obterAdminContexto()
    }

    dashboard.value = await dashboardAdminService.obterDashboard(filtros.value)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar dashboard administrativo.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(novosFiltros: FiltroIndicadoresRequest): Promise<void> {
  filtros.value = { ...novosFiltros }
  await carregarDashboard()
}

onMounted(carregarDashboard)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Dashboard administrativo"
      subtitulo="Acompanhe a operacao de atendimento e os indicadores de SLA."
    >
      <template #actions>
        <q-btn color="primary" icon="list_alt" label="Ir para fila" @click="router.push('/admin/chamados')" />
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros" subtitulo="Periodo, departamento, categoria e responsavel.">
      <FiltrosDashboardAdmin :contexto="contexto" :loading="loading" @filtrar="aplicarFiltros" />
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregarDashboard" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando dashboard administrativo..." />

    <template v-else-if="dashboard">
      <div class="row q-col-gutter-md">
        <div v-for="card in cardsObrigatorios" :key="card.chave" class="col-12 col-sm-6 col-lg-3">
          <MetricCard :titulo="card.titulo" :valor="card.valor" :icon="card.icon" :color="card.color" />
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Chamados por status" class="full-height">
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorStatus" :key="item.status">
                <q-item-section>
                  <StatusBadge :texto="item.status" />
                </q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Chamados por prioridade" class="full-height">
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorPrioridade" :key="item.prioridade">
                <q-item-section>
                  <PrioridadeBadge :texto="item.prioridade" />
                </q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Chamados por categoria" class="full-height">
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorCategoria" :key="item.categoria">
                <q-item-section>{{ item.categoria }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Indicadores de SLA" class="full-height">
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
                <q-item-section>Proximos do vencimento</q-item-section>
                <q-item-section side>{{ dashboard.indicadoresSla.totalProximosDoVencimento }}</q-item-section>
              </q-item>
              <q-item>
                <q-item-section>Percentual de cumprimento</q-item-section>
                <q-item-section side>{{ dashboard.indicadoresSla.percentualCumprimento }}%</q-item-section>
              </q-item>
              <q-item>
                <q-item-section>Media de resolucao</q-item-section>
                <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasResolucao) }}</q-item-section>
              </q-item>
              <q-item>
                <q-item-section>Media de primeira resposta</q-item-section>
                <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasPrimeiraResposta) }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-8">
          <AppSectionCard titulo="Produtividade por atendente">
            <q-table
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
      </div>

      <EmptyState
        v-if="semDadosDashboard"
        titulo="Sem indicadores no periodo"
        mensagem="Nao foram encontrados dados para os filtros aplicados."
        icon="analytics"
      />
    </template>
  </q-page>
</template>
