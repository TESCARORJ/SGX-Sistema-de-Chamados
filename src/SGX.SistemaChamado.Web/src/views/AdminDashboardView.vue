<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import FiltrosDashboardAdmin from '../components/admin/FiltrosDashboardAdmin.vue'
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
  <div class="column q-gutter-md">
    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">Dashboard administrativo</h1>
      <q-btn color="primary" label="Ir para fila" @click="router.push('/admin/chamados')" />
    </div>

    <FiltrosDashboardAdmin :contexto="contexto" :loading="loading" @filtrar="aplicarFiltros" />

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-spinner v-if="loading" size="2rem" color="primary" />

    <template v-if="dashboard && !loading">
      <div class="row q-col-gutter-md">
        <div class="col-12 col-sm-6 col-md-3" v-for="card in dashboard.cards" :key="card.chave">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-caption text-grey-8">{{ card.titulo }}</div>
              <div class="text-h6">{{ card.valor }}</div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Chamados por status</div>
            </q-card-section>
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
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Chamados por prioridade</div>
            </q-card-section>
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
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Chamados por categoria</div>
            </q-card-section>
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
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Indicadores de SLA</div>
            </q-card-section>
            <q-separator />
            <q-card-section class="column q-gutter-xs">
              <div><strong>Total chamados:</strong> {{ dashboard.indicadoresSla.totalChamados }}</div>
              <div><strong>Dentro do prazo:</strong> {{ dashboard.indicadoresSla.totalDentroDoPrazo }}</div>
              <div><strong>Vencidos:</strong> {{ dashboard.indicadoresSla.totalVencidos }}</div>
              <div><strong>Proximos do vencimento:</strong> {{ dashboard.indicadoresSla.totalProximosDoVencimento }}</div>
              <div><strong>Cumprimento:</strong> {{ dashboard.indicadoresSla.percentualCumprimento }}%</div>
              <div><strong>Media resolucao:</strong> {{ formatHoras(dashboard.indicadoresSla.mediaHorasResolucao) }}</div>
              <div><strong>Media 1a resposta:</strong> {{ formatHoras(dashboard.indicadoresSla.mediaHorasPrimeiraResposta) }}</div>
            </q-card-section>
          </q-card>
        </div>

        <div class="col-12 col-lg-8">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Produtividade por atendente</div>
            </q-card-section>
            <q-separator />
            <q-table
              flat
              :rows="dashboard.produtividadePorAtendente"
              :columns="[
                { name: 'responsavel', label: 'Responsavel', field: 'responsavelNome', align: 'left' },
                { name: 'atendidos', label: 'Atendidos', field: 'totalAtendidos', align: 'right' },
                { name: 'encerrados', label: 'Encerrados', field: 'totalEncerrados', align: 'right' },
                { name: 'vencidos', label: 'Vencidos', field: 'totalVencidos', align: 'right' },
                { name: 'media', label: 'Media resolucao', field: 'mediaHorasResolucao', align: 'right' }
              ]"
              row-key="responsavelId"
              hide-pagination
            >
              <template #body-cell-media="slotProps">
                <q-td :props="slotProps">{{ formatHoras(slotProps.row.mediaHorasResolucao) }}</q-td>
              </template>
            </q-table>
          </q-card>
        </div>
      </div>
    </template>

    <q-banner v-if="!loading && dashboard && !dashboard.cards.length" class="bg-blue-1 text-primary">
      Nenhum dado encontrado para os filtros informados.
    </q-banner>
  </div>
</template>
