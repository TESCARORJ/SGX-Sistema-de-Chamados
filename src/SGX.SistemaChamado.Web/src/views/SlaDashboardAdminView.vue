<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
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
    { titulo: 'Com SLA', valor: data.totalComSlaAplicado, icon: 'assignment_turned_in', color: 'primary' },
    { titulo: 'Vencidos', valor: data.totalVencidos, icon: 'warning', color: 'negative' },
    { titulo: 'Próximos', valor: data.totalProximosDoVencimento, icon: 'schedule', color: 'warning' },
    { titulo: 'Cumprimento', valor: `${data.percentualCumprimento}%`, icon: 'verified', color: 'positive' },
  ]
})

function formatarMedia(valor: number | null): string {
  return valor === null ? '-' : `${Math.round(valor)} min`
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    dashboard.value = await slaPoliciesService.obterDashboard({ ...filtros })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar o painel de SLA.'
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
    <PageHeader titulo="Painel de SLA" subtitulo="Acompanhe vencimentos, cumprimento e médias operacionais." />

    <AppSectionCard titulo="Filtros" subtitulo="Refine os indicadores por período e situação.">
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
            label="Situação"
            :options="[
              { label: 'Dentro do prazo', value: 'DentroDoPrazo' },
              { label: 'Próximo do vencimento', value: 'ProximoDoVencimento' },
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
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
    <LoadingState v-else-if="loading" inline mensagem="Carregando painel de SLA..." />

    <template v-else-if="dashboard">
      <div class="row q-col-gutter-md">
        <div v-for="card in cards" :key="card.titulo" class="col-12 col-sm-6 col-lg-3">
          <MetricCard :titulo="card.titulo" :valor="String(card.valor)" :icon="card.icon" :color="card.color" />
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-md-6">
          <AppSectionCard titulo="Médias" subtitulo="Tempos médios registrados nos SLAs concluídos.">
            <q-list separator>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Primeira resposta</q-item-label>
                  <q-item-label>{{ formatarMedia(dashboard.tempoMedioPrimeiraRespostaMinutos) }}</q-item-label>
                </q-item-section>
              </q-item>
              <q-item>
                <q-item-section>
                  <q-item-label caption>Resolução</q-item-label>
                  <q-item-label>{{ formatarMedia(dashboard.tempoMedioResolucaoMinutos) }}</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
        <div class="col-12 col-md-6">
          <AppSectionCard titulo="Situação" subtitulo="Distribuição consolidada do SLA.">
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
        <div v-for="grupo in [
          { titulo: 'Por prioridade', itens: dashboard.porPrioridade },
          { titulo: 'Por categoria', itens: dashboard.porCategoria },
          { titulo: 'Por departamento', itens: dashboard.porDepartamento },
        ]" :key="grupo.titulo" class="col-12 col-lg-4">
          <AppSectionCard :titulo="grupo.titulo" subtitulo="Total, vencidos e próximos.">
            <q-list separator>
              <q-item v-for="item in (grupo.itens as SlaAgrupamentoResponse[])" :key="`${grupo.titulo}-${item.id ?? item.nome}`">
                <q-item-section>
                  <q-item-label>{{ item.nome }}</q-item-label>
                  <q-item-label caption>{{ item.vencidos }} vencidos · {{ item.proximos }} próximos</q-item-label>
                </q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
      </div>
    </template>
  </q-page>
</template>
