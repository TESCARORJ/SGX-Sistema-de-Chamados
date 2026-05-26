<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { slaPoliciesService } from '../services/slaPoliciesService'
import type { FiltroDashboardSlaRequest, SlaAgrupamentoResponse, SlaDashboardResponse } from '../types/slaPolicies'

const loading = ref(false)
const erro = ref<string | null>(null)
const dashboard = ref<SlaDashboardResponse | null>(null)

const filtros = reactive<FiltroDashboardSlaRequest>({
  dataInicio: undefined,
  dataFim: undefined,
  situacaoSla: undefined,
})

const cards = computed(() => {
  const data = dashboard.value
  if (!data) return []
  return [
    { titulo: 'Total com SLA', valor: data.totalComSlaAplicado, icon: 'assignment_turned_in', tone: 'primary' as const },
    { titulo: 'SLA cumprido', valor: data.totalCumpridos, icon: 'task_alt', tone: 'positive' as const },
    { titulo: 'SLA em risco', valor: data.totalProximosDoVencimento, icon: 'schedule', tone: 'warning' as const },
    { titulo: 'SLA violado', valor: data.totalViolados, icon: 'warning', tone: 'negative' as const },
    { titulo: 'Vencidos', valor: data.totalVencidos, icon: 'priority_high', tone: 'negative' as const },
    { titulo: 'Dentro do prazo', valor: data.totalDentroDoPrazo, icon: 'verified', tone: 'info' as const },
    { titulo: 'Cumprimento', valor: `${data.percentualCumprimento}%`, icon: 'query_stats', tone: 'primary' as const },
  ]
})

const semDados = computed(() => {
  if (!dashboard.value) return true
  return dashboard.value.totalComSlaAplicado === 0
})

function formatarMedia(valor: number | null): string {
  return valor === null ? '-' : `${Math.round(valor)} min`
}

function grupoVazio(itens: SlaAgrupamentoResponse[]): boolean {
  return !itens.length
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    dashboard.value = await slaPoliciesService.obterDashboard({ ...filtros })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar o painel de SLA.'
  } finally {
    loading.value = false
  }
}

function limpar(): void {
  filtros.dataInicio = undefined
  filtros.dataFim = undefined
  filtros.situacaoSla = undefined
  void carregar()
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="SLA e monitoramento"
      titulo="Painel de SLA"
      subtitulo="Acompanhe cumprimento, risco, violacoes e medias operacionais com base em dados reais."
    />

    <AppSectionCard titulo="Filtros" subtitulo="Refine os indicadores por periodo e situacao de SLA.">
      <FilterBar compact>
        <q-form class="row q-col-gutter-sm" @submit.prevent="carregar">
          <div class="col-12 col-md-3">
            <q-input v-model="filtros.dataInicio" type="date" outlined label="Data inicial" />
          </div>
          <div class="col-12 col-md-3">
            <q-input v-model="filtros.dataFim" type="date" outlined label="Data final" />
          </div>
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtros.situacaoSla"
              clearable
              emit-value
              map-options
              outlined
              label="Situacao"
              :options="[
                { label: 'Dentro do prazo', value: 'DentroDoPrazo' },
                { label: 'Proximo do vencimento', value: 'ProximoDoVencimento' },
                { label: 'Vencido', value: 'Vencido' },
                { label: 'Cumprido', value: 'Cumprido' },
                { label: 'Violado', value: 'Violado' },
                { label: 'Pausado', value: 'Pausado' },
              ]"
            />
          </div>
          <div class="col-12 col-md-3 row items-center justify-end q-gutter-sm">
            <q-btn flat label="Limpar" @click="limpar" />
            <q-btn color="primary" icon="search" label="Filtrar" type="submit" :loading="loading" />
          </div>
        </q-form>
      </FilterBar>
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
    <LoadingState v-else-if="loading" inline mensagem="Carregando painel de SLA..." />

    <template v-else-if="dashboard">
      <div class="sgx-kpi-grid">
        <MetricCard
          v-for="card in cards"
          :key="card.titulo"
          :title="card.titulo"
          :value="card.valor"
          :icon="card.icon"
          :tone="card.tone"
        />
      </div>

      <EmptyState
        v-if="semDados"
        titulo="Sem dados para o periodo selecionado"
        mensagem="Nao ha chamados com SLA aplicado para os filtros atuais."
        icon="monitoring"
      />

      <template v-else>
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <AppSectionCard titulo="Medias operacionais" subtitulo="Tempos medios registrados nos ciclos de SLA.">
              <q-list separator>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Primeira resposta</q-item-label>
                    <q-item-label>{{ formatarMedia(dashboard.tempoMedioPrimeiraRespostaMinutos) }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Resolucao</q-item-label>
                    <q-item-label>{{ formatarMedia(dashboard.tempoMedioResolucaoMinutos) }}</q-item-label>
                  </q-item-section>
                </q-item>
              </q-list>
            </AppSectionCard>
          </div>
          <div class="col-12 col-md-6">
            <AppSectionCard titulo="Distribuicao de situacao" subtitulo="Resumo consolidado por estado de prazo.">
              <q-list separator>
                <q-item>
                  <q-item-section>Dentro do prazo</q-item-section>
                  <q-item-section side>{{ dashboard.totalDentroDoPrazo }}</q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>Cumpridos</q-item-section>
                  <q-item-section side>{{ dashboard.totalCumpridos }}</q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>Violados</q-item-section>
                  <q-item-section side>{{ dashboard.totalViolados }}</q-item-section>
                </q-item>
              </q-list>
            </AppSectionCard>
          </div>
        </div>

        <div class="row q-col-gutter-md">
          <div
            v-for="grupo in [
              { titulo: 'Por prioridade', itens: dashboard.porPrioridade },
              { titulo: 'Por categoria', itens: dashboard.porCategoria },
              { titulo: 'Por departamento', itens: dashboard.porDepartamento },
            ]"
            :key="grupo.titulo"
            class="col-12 col-lg-4"
          >
            <AppSectionCard :titulo="grupo.titulo" subtitulo="Total, vencidos e proximos do vencimento.">
              <EmptyState
                v-if="grupoVazio(grupo.itens as SlaAgrupamentoResponse[])"
                titulo="Sem dados no agrupamento"
                mensagem="Nenhum registro para os filtros atuais."
                icon="inbox"
              />

              <q-list v-else separator>
                <q-item
                  v-for="item in (grupo.itens as SlaAgrupamentoResponse[])"
                  :key="`${grupo.titulo}-${item.id ?? item.nome}`"
                >
                  <q-item-section>
                    <q-item-label>{{ item.nome }}</q-item-label>
                    <q-item-label caption>{{ item.vencidos }} vencidos · {{ item.proximos }} proximos</q-item-label>
                  </q-item-section>
                  <q-item-section side>{{ item.total }}</q-item-section>
                </q-item>
              </q-list>
            </AppSectionCard>
          </div>
        </div>
      </template>
    </template>
  </q-page>
</template>
