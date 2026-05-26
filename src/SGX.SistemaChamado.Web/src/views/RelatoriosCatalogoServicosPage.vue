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
import type {
  FiltroRelatorioCatalogo,
  RelatorioCatalogoServicosMaisSolicitados,
  RelatorioCatalogoServicosPorDepartamento,
  RelatorioCatalogoServicosResumo,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioCatalogoServicosResumo | null>(null)
const maisSolicitados = ref<RelatorioCatalogoServicosMaisSolicitados[]>([])
const porDepartamento = ref<RelatorioCatalogoServicosPorDepartamento[]>([])

const departamentos = ref<{ id: string; nome: string }[]>([])
const categorias = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioCatalogo>({
  dataInicial: '',
  dataFinal: '',
  departamentoId: '',
  categoriaId: '',
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
  { name: 'totalChamados', label: 'Total de chamados', field: 'totalChamados', align: 'right' as const },
  { name: 'totalComAprovacao', label: 'Com aprovacao', field: 'totalComAprovacao', align: 'right' as const },
  { name: 'totalForaSla', label: 'Fora SLA', field: 'totalForaSla', align: 'right' as const },
]

const colunasDepartamento = [
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left' as const },
  { name: 'totalServicos', label: 'Total de servicos', field: 'totalServicos', align: 'right' as const },
  { name: 'servicosPublicados', label: 'Publicados', field: 'servicosPublicados', align: 'right' as const },
  { name: 'chamadosAbertos', label: 'Chamados abertos', field: 'chamadosAbertos', align: 'right' as const },
  { name: 'servicosQueRequeremAprovacao', label: 'Servicos com aprovacao', field: 'servicosQueRequeremAprovacao', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioCatalogo {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    departamentoId: filtros.departamentoId || undefined,
    categoriaId: filtros.categoriaId || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.departamentoId = ''
  filtros.categoriaId = ''
  filtros.limiteRanking = 20
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const contexto = await adminService.obterAdminContexto()
    departamentos.value = contexto.departamentos.map((item) => ({ id: item.id, nome: item.nome }))
    categorias.value = contexto.categorias.map((item) => ({ id: item.id, nome: item.nome }))
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
          { grupo: 'Resumo', indicador: 'Total de servicos', valor: resumo.value.totalServicos },
          { grupo: 'Resumo', indicador: 'Servicos publicados', valor: resumo.value.servicosPublicados },
          { grupo: 'Resumo', indicador: 'Chamados por catalogo', valor: resumo.value.chamadosAbertosPorCatalogo },
          { grupo: 'Resumo', indicador: 'Servicos com aprovacao', valor: resumo.value.servicosQueRequeremAprovacao },
        ]
      : []),
    ...maisSolicitados.value.map((item) => ({ grupo: 'Mais solicitados', indicador: item.nomeServico, valor: item.totalChamados })),
  ]

  exportarCsv('relatorio-catalogo-servicos.csv', linhas)
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
    <PageHeader titulo="Relatorios - Catalogo de servicos" subtitulo="Analise de demanda por servico e distribuicao por departamento." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de catalogo de servicos.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo, departamento e categoria para leitura da demanda de catalogo.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
            <div class="col-12 col-md-3">
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
            <div class="col-12 col-md-3">
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
            <div class="col-12 col-md-3"><q-input v-model.number="filtros.limiteRanking" type="number" min="1" max="100" outlined dense label="Limite ranking" /></div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo && !porDepartamento.length" mensagem="Carregando relatorios de catalogo..." />

      <template v-else>
        <div v-if="resumo" class="sgx-kpi-grid">
          <MetricCard titulo="Total de servicos" :valor="resumo.totalServicos" icon="inventory_2" tone="primary" />
          <MetricCard titulo="Publicados" :valor="resumo.servicosPublicados" icon="task_alt" tone="positive" />
          <MetricCard titulo="Ativos" :valor="resumo.servicosAtivos" icon="check_circle" tone="info" />
          <MetricCard titulo="Com abertura" :valor="resumo.servicosQuePermitemAbertura" icon="app_registration" tone="warning" />
          <MetricCard titulo="Com aprovacao" :valor="resumo.servicosQueRequeremAprovacao" icon="verified_user" tone="warning" />
          <MetricCard titulo="Chamados por catalogo" :valor="resumo.chamadosAbertosPorCatalogo" icon="support_agent" tone="primary" />
        </div>

        <AppSectionCard titulo="Servicos mais solicitados" :subtitulo="`Servicos no ranking: ${maisSolicitados.length}`">
          <q-table v-if="maisSolicitados.length" class="sgx-table" flat bordered :rows="maisSolicitados" :columns="colunasMaisSolicitados" row-key="catalogoServicoId" hide-pagination>
            <template #body-cell-nomeServico="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.nomeServico" />
              </q-td>
            </template>

            <template #body-cell-totalForaSla="props">
              <q-td :props="props" class="text-right">{{ props.row.totalForaSla ?? '-' }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem ranking de servicos"
            mensagem="Nao ha servicos mais solicitados para os filtros informados."
            icon="leaderboard"
          />
        </AppSectionCard>

        <AppSectionCard titulo="Catalogo por departamento" :subtitulo="`Departamentos analisados: ${porDepartamento.length}`">
          <q-table v-if="porDepartamento.length" class="sgx-table" flat bordered :rows="porDepartamento" :columns="colunasDepartamento" row-key="departamentoId" hide-pagination />
          <EmptyState
            v-else
            titulo="Sem dados por departamento"
            mensagem="Nao ha distribuicao de catalogo por departamento para os filtros aplicados."
            icon="apartment"
          />
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
