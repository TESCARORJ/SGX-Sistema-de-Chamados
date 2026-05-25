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
  FiltroRelatorioCatalogo,
  RelatorioCatalogoServicosMaisSolicitados,
  RelatorioCatalogoServicosPorDepartamento,
  RelatorioCatalogoServicosResumo,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioCatalogoServicosResumo | null>(null)
const maisSolicitados = ref<RelatorioCatalogoServicosMaisSolicitados[]>([])
const porDepartamento = ref<RelatorioCatalogoServicosPorDepartamento[]>([])

const filtros = reactive<FiltroRelatorioCatalogo>({
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
const podeExportar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosExportar))

const colunasMaisSolicitados = [
  { name: 'nomeServico', label: 'Servico', field: 'nomeServico', align: 'left' as const },
  { name: 'departamentoResponsavel', label: 'Departamento', field: 'departamentoResponsavel', align: 'left' as const },
  { name: 'totalChamados', label: 'Total chamados', field: 'totalChamados', align: 'right' as const },
  { name: 'totalComAprovacao', label: 'Com aprovacao', field: 'totalComAprovacao', align: 'right' as const },
]

const colunasDepartamento = [
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left' as const },
  { name: 'totalServicos', label: 'Total servicos', field: 'totalServicos', align: 'right' as const },
  { name: 'servicosPublicados', label: 'Publicados', field: 'servicosPublicados', align: 'right' as const },
  { name: 'chamadosAbertos', label: 'Chamados abertos', field: 'chamadosAbertos', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioCatalogo {
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
    const [resumoResp, solicitadosResp, departamentoResp] = await Promise.all([
      podeGerencial.value ? relatoriosAvancadosAdminService.obterResumoCatalogoServicos(request) : Promise.resolve(null),
      podeGerencial.value ? relatoriosAvancadosAdminService.obterCatalogoServicosMaisSolicitados(request) : Promise.resolve([]),
      relatoriosAvancadosAdminService.obterCatalogoServicosPorDepartamento(request),
    ])

    resumo.value = resumoResp
    maisSolicitados.value = solicitadosResp
    porDepartamento.value = departamentoResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de catalogo de servicos.'
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
          { grupo: 'Resumo', indicador: 'Total servicos', valor: resumo.value.totalServicos },
          { grupo: 'Resumo', indicador: 'Servicos publicados', valor: resumo.value.servicosPublicados },
          { grupo: 'Resumo', indicador: 'Chamados por catalogo', valor: resumo.value.chamadosAbertosPorCatalogo },
        ]
      : []),
    ...maisSolicitados.value.map((item) => ({ grupo: 'Mais solicitados', indicador: item.nomeServico, valor: item.totalChamados })),
  ]

  exportarCsv('relatorio-catalogo-servicos.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Catalogo de servicos" subtitulo="Analise de demanda por servico e distribuicao por departamento.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de catalogo de servicos.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo e departamento para leitura do catalogo.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-4"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-4"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-4"><q-input v-model="filtros.departamentoId" outlined dense label="Departamento ID" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo && !porDepartamento.length" mensagem="Carregando relatorios de catalogo..." />

      <template v-else>
        <div v-if="resumo" class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total servicos" :valor="resumo.totalServicos" icon="inventory_2" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Publicados" :valor="resumo.servicosPublicados" icon="task_alt" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Ativos" :valor="resumo.servicosAtivos" icon="check_circle" color="info" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Chamados por catalogo" :valor="resumo.chamadosAbertosPorCatalogo" icon="support_agent" color="warning" /></div>
        </div>

        <AppSectionCard v-if="maisSolicitados.length" titulo="Servicos mais solicitados">
          <q-table flat :rows="maisSolicitados" :columns="colunasMaisSolicitados" row-key="catalogoServicoId" hide-pagination />
        </AppSectionCard>

        <AppSectionCard v-if="porDepartamento.length" titulo="Catalogo por departamento">
          <q-table flat :rows="porDepartamento" :columns="colunasDepartamento" row-key="departamentoId" hide-pagination />
        </AppSectionCard>

        <EmptyState
          v-if="!resumo && !maisSolicitados.length && !porDepartamento.length"
          titulo="Sem dados de catalogo"
          mensagem="Nao ha resultados para os filtros informados."
          icon="inventory_2"
        />
      </template>
    </template>
  </q-page>
</template>
