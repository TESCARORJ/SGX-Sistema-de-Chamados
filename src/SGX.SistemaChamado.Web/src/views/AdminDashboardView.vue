<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import FiltrosDashboardAdmin from '../components/admin/FiltrosDashboardAdmin.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { adminService } from '../services/adminService'
import { dashboardAdminService } from '../services/dashboardAdminService'
import type { AdminContextoResponse } from '../types/admin'
import type { DashboardAdminResponse } from '../types/dashboard'
import type { FiltroIndicadoresRequest } from '../types/indicadores'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const contexto = ref<AdminContextoResponse | null>(null)
const dashboard = ref<DashboardAdminResponse | null>(null)
const filtros = ref<FiltroIndicadoresRequest>({})

const produtividadeColumns = [
  { name: 'responsavel', label: 'Responsavel', field: 'responsavelNome', align: 'left' as const },
  { name: 'atendidos', label: 'Atendidos', field: 'totalAtendidos', align: 'right' as const },
  { name: 'encerrados', label: 'Encerrados', field: 'totalEncerrados', align: 'right' as const },
  { name: 'vencidos', label: 'Vencidos', field: 'totalVencidos', align: 'right' as const },
  { name: 'media', label: 'Media resolucao', field: 'mediaHorasResolucao', align: 'right' as const },
]

function formatHoras(valor: number | null): string {
  if (valor === null || Number.isNaN(valor)) return '-'
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
      subtitulo="Visao consolidada de chamados, SLA e produtividade da equipe"
    >
      <template #actions>
        <q-btn color="primary" icon="list_alt" label="Ir para fila" @click="router.push('/admin/chamados')" />
      </template>
    </PageHeader>

    <FiltrosDashboardAdmin :contexto="contexto" :loading="loading" @filtrar="aplicarFiltros" />

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
    </div>

    <template v-if="dashboard && !loading">
      <div class="row q-col-gutter-md">
        <div v-for="card in dashboard.cards" :key="card.chave" class="col-12 col-sm-6 col-lg-3">
          <q-card flat bordered class="sgx-card">
            <q-card-section>
              <div class="text-caption text-grey-7">{{ card.titulo }}</div>
              <div class="text-h5 text-weight-bold q-mt-xs">{{ card.valor }}</div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Chamados por status</q-card-section>
            <q-separator />
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorStatus" :key="item.status">
                <q-item-section>{{ item.status }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Chamados por prioridade</q-card-section>
            <q-separator />
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorPrioridade" :key="item.prioridade">
                <q-item-section>{{ item.prioridade }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Chamados por categoria</q-card-section>
            <q-separator />
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorCategoria" :key="item.categoria">
                <q-item-section>{{ item.categoria }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Indicadores de SLA</q-card-section>
            <q-separator />
            <q-list separator>
              <q-item>
                <q-item-section>Total chamados</q-item-section>
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
                <q-item-section>Cumprimento</q-item-section>
                <q-item-section side>{{ dashboard.indicadoresSla.percentualCumprimento }}%</q-item-section>
              </q-item>
              <q-item>
                <q-item-section>Media resolucao</q-item-section>
                <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasResolucao) }}</q-item-section>
              </q-item>
              <q-item>
                <q-item-section>Media primeira resposta</q-item-section>
                <q-item-section side>{{ formatHoras(dashboard.indicadoresSla.mediaHorasPrimeiraResposta) }}</q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-8">
          <q-card flat bordered class="sgx-card">
            <q-card-section class="text-subtitle1 text-weight-medium">Produtividade por atendente</q-card-section>
            <q-separator />
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
          </q-card>
        </div>
      </div>
    </template>

    <q-banner v-if="!loading && dashboard && !dashboard.cards.length" rounded class="bg-blue-1 text-primary">
      Nenhum dado encontrado para os filtros informados.
    </q-banner>
  </q-page>
</template>
