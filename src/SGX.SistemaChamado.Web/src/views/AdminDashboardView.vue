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
import type { FiltroIndicadoresRequest, ProdutividadeAtendente } from '../types/indicadores'

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
})

const logsEmail = ref<LogIntegracaoEmailResumoResponse[]>([])

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
    {
      chave: 'abertos',
      titulo: 'Chamados Abertos',
      valor: dashboard.value.totalAbertos,
      subtitulo: `${dashboard.value.totalSemResponsavel} sem responsavel`,
      icon: 'drafts',
      color: 'primary',
    },
    {
      chave: 'atendimento',
      titulo: 'Em Atendimento',
      valor: dashboard.value.totalEmAtendimento,
      subtitulo: 'Atendimento ativo',
      icon: 'support_agent',
      color: 'warning',
    },
    {
      chave: 'aguardando',
      titulo: 'Aguardando Solicitante',
      valor: dashboard.value.totalAguardandoSolicitante,
      subtitulo: 'Aguardando retorno',
      icon: 'hourglass_top',
      color: 'deep-orange',
    },
    {
      chave: 'sla-vencido',
      titulo: 'SLA Vencido',
      valor: dashboard.value.totalVencidos,
      subtitulo: 'Prioridade de atuacao',
      icon: 'warning',
      color: 'negative',
    },
    {
      chave: 'sla-proximo',
      titulo: 'Proximos do Vencimento',
      valor: dashboard.value.totalProximosDoVencimento,
      subtitulo: 'Risco de ruptura',
      icon: 'schedule',
      color: 'orange-8',
    },
    {
      chave: 'resolvidos-periodo',
      titulo: 'Resolvidos no Periodo',
      valor: dashboard.value.totalResolvidosPeriodo,
      subtitulo: 'Volume entregue',
      icon: 'task_alt',
      color: 'positive',
    },
  ]
})

const semDadosDashboard = computed(() => {
  if (!dashboard.value) {
    return false
  }

  return cardsObrigatorios.value.every((card) => card.valor === 0)
})

const isAdministrador = computed(() => contexto.value?.usuario.perfis.includes('Administrador') ?? false)
const totalStatus = computed(
  () => dashboard.value?.chamadosPorStatus.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalPrioridade = computed(
  () => dashboard.value?.chamadosPorPrioridade.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalCategoria = computed(
  () => dashboard.value?.chamadosPorCategoria.reduce((acc, item) => acc + item.total, 0) ?? 0
)
const totalPaginasFila = computed(() => Math.max(1, Math.ceil(totalFila.value / tamanhoPaginaFila.value)))

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
      error instanceof Error ? error.message : 'Falha ao carregar dashboard administrativo.'
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
      pagina: paginaFila.value,
      tamanhoPagina: tamanhoPaginaFila.value,
      ordenarPor: 'atualizadoEm',
      direcaoOrdenacao: 'desc',
    })

    filaChamados.value = response.items
    totalFila.value = response.total
  } catch (error) {
    erroFila.value = error instanceof Error ? error.message : 'Falha ao carregar fila de chamados.'
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
    erroEmail.value = error instanceof Error ? error.message : 'Falha ao carregar integracao de e-mail.'
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
    erroFila.value = error instanceof Error ? error.message : 'Falha ao assumir chamado.'
  } finally {
    processandoAssumir.value = false
  }
}

onMounted(() => {
  void carregarTudo()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Dashboard" subtitulo="Visao geral da operacao de atendimento.">
      <template #actions>
        <div class="row q-gutter-sm items-center">
          <q-btn
            color="primary"
            icon="filter_list"
            label="Filtros"
            @click="router.push('/admin/chamados')"
          />
          <q-btn
            outline
            color="primary"
            icon="list_alt"
            label="Ver fila completa"
            @click="router.push('/admin/chamados')"
          />
        </div>
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros do dashboard" subtitulo="Periodo, departamento, categoria e responsavel.">
      <FiltrosDashboardAdmin :contexto="contexto" :loading="loadingDashboard" @filtrar="aplicarFiltrosDashboard" />
    </AppSectionCard>

    <ErrorState v-if="erroDashboard" :mensagem="erroDashboard" @retry="carregarDashboard" />

    <LoadingState
      v-else-if="loadingDashboard"
      inline
      mensagem="Carregando dashboard administrativo..."
    />

    <template v-else-if="dashboard">
      <div class="sgx-kpi-grid">
        <div v-for="card in cardsObrigatorios" :key="card.chave">
          <MetricCard
            :titulo="card.titulo"
            :valor="card.valor"
            :subtitulo="card.subtitulo"
            :icon="card.icon"
            :tone="
              card.chave === 'sla-vencido'
                ? 'negative'
                : card.chave === 'resolvidos-periodo'
                  ? 'positive'
                  : card.chave === 'atendimento'
                    ? 'info'
                    : card.chave.includes('proximo') || card.chave.includes('aguardando')
                      ? 'warning'
                      : 'primary'
            "
          />
        </div>
      </div>

      <div class="dashboard-triple-grid">
        <div>
          <AppSectionCard titulo="Chamados por Status" class="full-height">
            <q-list separator>
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
        </div>

        <div>
          <AppSectionCard titulo="Chamados por Prioridade" class="full-height">
            <q-list separator>
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
        </div>

        <div>
          <AppSectionCard titulo="Chamados por Categoria" class="full-height">
            <q-list separator>
              <q-item v-for="item in dashboard.chamadosPorCategoria" :key="item.categoria">
                <q-item-section>
                  <div class="text-body2">{{ item.categoria }}</div>
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
        </div>
      </div>

      <div class="dashboard-main-grid">
        <div>
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

        <div>
          <AppSectionCard titulo="Produtividade por Atendente" class="full-height">
            <q-table
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
      </div>

      <AppSectionCard
        titulo="Fila de Chamados"
        subtitulo="Atendimentos mais recentes para triagem e distribuicao da operacao."
      >
        <template #actions>
          <q-btn flat color="primary" icon="open_in_new" label="Ver todas" @click="router.push('/admin/chamados')" />
        </template>

        <q-form class="row q-col-gutter-sm q-mb-md" @submit.prevent="aplicarFiltroFila">
          <div class="col-12 col-md-5">
            <q-input
              v-model="filtrosFila.texto"
              outlined
              dense
              label="Buscar na fila"
              placeholder="Codigo, titulo ou descricao"
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
          <div class="col-12 col-md-3 row justify-end q-gutter-sm">
            <q-btn flat color="primary" label="Limpar" :disable="loadingFila" @click="limparFiltroFila" />
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loadingFila" />
          </div>
        </q-form>

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
          mensagem="Nao ha chamados para os filtros aplicados neste momento."
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

      <div class="dashboard-bottom-grid">
        <div>
          <AppSectionCard
            titulo="Integracao de E-mail"
            subtitulo="Ultimos processamentos para monitorar ingestao automatica."
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
              mensagem="Carregando resumo da integracao de e-mail..."
            />

            <EmptyState
              v-else-if="!logsEmail.length"
              titulo="Sem logs recentes"
              mensagem="Nenhum processamento de e-mail foi encontrado no periodo atual."
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

        <div>
          <AppSectionCard titulo="Resumo operacional" subtitulo="Navegacao rapida para o time administrativo.">
            <div class="column q-gutter-sm">
              <q-btn
                unelevated
                color="primary"
                icon="assignment"
                label="Gerenciar chamados"
                class="full-width"
                @click="router.push('/admin/chamados')"
              />
              <q-btn
                outline
                color="primary"
                icon="group"
                label="Abrir cadastros"
                class="full-width"
                @click="router.push('/admin/cadastros/usuarios')"
              />
              <q-btn
                outline
                color="primary"
                icon="mail"
                label="Monitorar e-mail"
                class="full-width"
                @click="router.push('/admin/integracoes/email')"
              />
            </div>
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

<style scoped>
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
}
</style>
