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
  FiltroRelatorioInventario,
  RelatorioInventarioAtivosChamadosRecorrentes,
  RelatorioInventarioAtivosPorDepartamento,
  RelatorioInventarioAtivosPorStatus,
  RelatorioInventarioAtivosResumo,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioInventarioAtivosResumo | null>(null)
const porStatus = ref<RelatorioInventarioAtivosPorStatus | null>(null)
const recorrentes = ref<RelatorioInventarioAtivosChamadosRecorrentes[]>([])
const porDepartamento = ref<RelatorioInventarioAtivosPorDepartamento[]>([])

const filtros = reactive<FiltroRelatorioInventario>({
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

const colunasDistribuicao = [
  { name: 'nome', label: 'Grupo', field: 'nome', align: 'left' as const },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade', align: 'right' as const },
  { name: 'percentual', label: '%', field: 'percentual', align: 'right' as const },
]

const colunasRecorrentes = [
  { name: 'codigo', label: 'Codigo', field: 'codigo', align: 'left' as const },
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const },
  { name: 'tipoAtivo', label: 'Tipo', field: 'tipoAtivo', align: 'left' as const },
  { name: 'totalChamados', label: 'Total chamados', field: 'totalChamados', align: 'right' as const },
  { name: 'chamadosAbertos', label: 'Abertos', field: 'chamadosAbertos', align: 'right' as const },
]

const colunasDepartamento = [
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left' as const },
  { name: 'totalAtivos', label: 'Total ativos', field: 'totalAtivos', align: 'right' as const },
  { name: 'ativosAtivos', label: 'Ativos', field: 'ativosAtivos', align: 'right' as const },
  { name: 'ativosInativos', label: 'Inativos', field: 'ativosInativos', align: 'right' as const },
  { name: 'totalComChamados', label: 'Com chamados', field: 'totalComChamados', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioInventario {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value || !podeGerencial.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const request = construirFiltros()
    const [resumoResp, statusResp, recorrentesResp, departamentoResp] = await Promise.all([
      relatoriosAvancadosAdminService.obterResumoInventarioAtivos(request),
      relatoriosAvancadosAdminService.obterInventarioAtivosPorStatus(request),
      relatoriosAvancadosAdminService.obterInventarioAtivosChamadosRecorrentes(request),
      relatoriosAvancadosAdminService.obterInventarioAtivosPorDepartamento(request),
    ])

    resumo.value = resumoResp
    porStatus.value = statusResp
    recorrentes.value = recorrentesResp
    porDepartamento.value = departamentoResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de inventario.'
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
          { grupo: 'Resumo', indicador: 'Total ativos', valor: resumo.value.totalAtivos },
          { grupo: 'Resumo', indicador: 'Ativos ativos', valor: resumo.value.ativosAtivos },
          { grupo: 'Resumo', indicador: 'Ativos inativos', valor: resumo.value.ativosInativos },
        ]
      : []),
    ...recorrentes.value.map((item) => ({ grupo: 'Recorrencia', indicador: `${item.codigo} - ${item.nome}`, valor: item.totalChamados })),
  ]

  exportarCsv('relatorio-inventario-ativos.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Inventario/Ativos" subtitulo="Visao consolidada de ativos, criticidade e recorrencia de chamados.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeGerencial" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar os relatorios de inventario.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Recorte de periodo e dimensoes de inventario.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.departamentoId" outlined dense label="Departamento ID" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.tipoAtivoInventarioId" outlined dense label="Tipo ativo ID" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorios de inventario..." />

      <template v-else-if="resumo">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total ativos" :valor="resumo.totalAtivos" icon="inventory" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Ativos ativos" :valor="resumo.ativosAtivos" icon="check_circle" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Em manutencao" :valor="resumo.totalEmManutencao" icon="build" color="warning" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Com defeito" :valor="resumo.totalComDefeito" icon="error" color="negative" /></div>
        </div>

        <AppSectionCard v-if="porStatus" titulo="Distribuicao por status">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Operacional</div>
              <q-table flat :rows="porStatus.porStatusOperacional" :columns="colunasDistribuicao" row-key="chave" hide-pagination />
            </div>
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Patrimonial</div>
              <q-table flat :rows="porStatus.porStatusPatrimonial" :columns="colunasDistribuicao" row-key="chave" hide-pagination />
            </div>
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Criticidade</div>
              <q-table flat :rows="porStatus.porCriticidade" :columns="colunasDistribuicao" row-key="chave" hide-pagination />
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard v-if="recorrentes.length" titulo="Ativos com chamados recorrentes">
          <q-table flat :rows="recorrentes" :columns="colunasRecorrentes" row-key="inventarioAtivoId" hide-pagination />
        </AppSectionCard>

        <AppSectionCard v-if="porDepartamento.length" titulo="Inventario por departamento">
          <q-table flat :rows="porDepartamento" :columns="colunasDepartamento" row-key="departamentoNome" hide-pagination />
        </AppSectionCard>
      </template>

      <EmptyState
        v-else
        titulo="Sem dados de inventario"
        mensagem="Nao ha resultados para os filtros informados."
        icon="inventory"
      />
    </template>
  </q-page>
</template>
