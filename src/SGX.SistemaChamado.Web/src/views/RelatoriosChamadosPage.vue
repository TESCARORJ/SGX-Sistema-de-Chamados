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
  FiltroRelatorioChamados,
  RelatorioAtendimentoProdutividade,
  RelatorioChamadosDistribuicao,
  RelatorioChamadosResumo,
  RelatorioChamadosSerieTemporal,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioChamadosResumo | null>(null)
const serieTemporal = ref<RelatorioChamadosSerieTemporal | null>(null)
const distribuicao = ref<RelatorioChamadosDistribuicao | null>(null)
const produtividade = ref<RelatorioAtendimentoProdutividade | null>(null)

const filtros = reactive<FiltroRelatorioChamados>({
  dataInicial: '',
  dataFinal: '',
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

const colunasSerie = [
  { name: 'periodo', label: 'Período', field: 'periodo', align: 'left' as const },
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
  { name: 'chamadosConcluidos', label: 'Concluídos', field: 'chamadosConcluidos', align: 'right' as const },
  { name: 'chamadosEmAberto', label: 'Em aberto', field: 'chamadosEmAberto', align: 'right' as const },
  { name: 'percentualConclusao', label: '% conclusão', field: 'percentualConclusao', align: 'right' as const },
]

const vazio = computed(() => {
  return (
    !resumo.value ||
    (resumo.value.totalChamados === 0 &&
      (serieTemporal.value?.itens.length ?? 0) === 0 &&
      (distribuicao.value?.itens.length ?? 0) === 0)
  )
})

function construirFiltros(): FiltroRelatorioChamados {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
    agruparPor: filtros.agruparPor || undefined,
    agrupamento: filtros.agrupamento || undefined,
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar o relatório de chamados.'
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
    { grupo: 'Resumo', indicador: 'Total reabertos', valor: resumo.value.totalReabertos },
    ...((distribuicao.value?.itens ?? []).map((item) => ({ grupo: 'Distribuição', indicador: item.nome, valor: item.quantidade }))),
    ...((serieTemporal.value?.itens ?? []).map((item) => ({ grupo: 'Série temporal', indicador: item.periodo, valor: item.abertos + item.encerrados + item.reabertos }))),
  ]

  exportarCsv('relatorio-chamados.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatórios - Chamados" subtitulo="Resumo, série temporal, distribuição e produtividade de atendimento.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
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
      Você não possui permissão para visualizar relatórios de chamados.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Período e recortes básicos para o relatório.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.departamentoId" outlined dense label="Departamento ID" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.categoriaId" outlined dense label="Categoria ID" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.prioridadeId" outlined dense label="Prioridade ID" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.status" outlined dense label="Status" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatório de chamados..." />

      <template v-else-if="resumo">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total de chamados" :valor="resumo.totalChamados" icon="support_agent" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Abertos" :valor="resumo.totalAbertos" icon="inbox" color="warning" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Encerrados" :valor="resumo.totalEncerradosOuConcluidos" icon="task_alt" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Reabertos" :valor="resumo.totalReabertos" icon="restart_alt" color="info" /></div>
        </div>

        <AppSectionCard v-if="serieTemporal?.itens?.length" titulo="Série temporal">
          <q-table flat :rows="serieTemporal.itens" :columns="colunasSerie" row-key="periodo" hide-pagination />
        </AppSectionCard>

        <AppSectionCard v-if="distribuicao?.itens?.length" titulo="Distribuição">
          <q-table flat :rows="distribuicao.itens" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
            <template #body-cell-percentual="props">
              <q-td :props="props" class="text-right">{{ Number(props.row.percentual).toFixed(2) }}%</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <AppSectionCard v-if="podeOperacional && produtividade?.ranking?.length" titulo="Produtividade de atendimento">
          <q-table flat :rows="produtividade.ranking" :columns="colunasProdutividade" row-key="atendenteId" hide-pagination>
            <template #body-cell-percentualConclusao="props">
              <q-td :props="props" class="text-right">{{ Number(props.row.percentualConclusao).toFixed(2) }}%</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <EmptyState
          v-if="vazio"
          titulo="Sem dados de chamados"
          mensagem="Não há resultados para os filtros informados."
          icon="support_agent"
        />
      </template>
    </template>
  </q-page>
</template>
