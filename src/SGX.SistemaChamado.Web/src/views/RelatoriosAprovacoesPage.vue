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
  FiltroRelatorioAprovacoes,
  RelatorioAprovacoesPorOrigem,
  RelatorioAprovacoesResumo,
  RelatorioAprovacoesTempoMedio,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioAprovacoesResumo | null>(null)
const tempoMedio = ref<RelatorioAprovacoesTempoMedio[]>([])
const porOrigem = ref<RelatorioAprovacoesPorOrigem[]>([])

const filtros = reactive<FiltroRelatorioAprovacoes>({
  dataInicial: '',
  dataFinal: '',
  agrupamento: 3,
  agruparPor: 1,
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
const podeOperacional = computed(() => possuiPermissao(permissoes.relatoriosAvancadosOperacional))
const podeExportar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosExportar))

const colunasTempo = [
  { name: 'grupo', label: 'Grupo', field: 'grupo', align: 'left' as const },
  { name: 'totalDecididas', label: 'Total decididas', field: 'totalDecididas', align: 'right' as const },
  { name: 'tempoMedioDecisaoHoras', label: 'Tempo medio (h)', field: 'tempoMedioDecisaoHoras', align: 'right' as const },
]

const colunasOrigem = [
  { name: 'tipoOrigem', label: 'Tipo origem', field: 'tipoOrigem', align: 'left' as const },
  { name: 'total', label: 'Total', field: 'total', align: 'right' as const },
  { name: 'pendentes', label: 'Pendentes', field: 'pendentes', align: 'right' as const },
  { name: 'aprovadas', label: 'Aprovadas', field: 'aprovadas', align: 'right' as const },
  { name: 'reprovadas', label: 'Reprovadas', field: 'reprovadas', align: 'right' as const },
  { name: 'canceladas', label: 'Canceladas', field: 'canceladas', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioAprovacoes {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
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
    const [resumoResp, tempoResp, origemResp] = await Promise.all([
      podeGerencial.value ? relatoriosAvancadosAdminService.obterResumoAprovacoes(request) : Promise.resolve(null),
      podeGerencial.value ? relatoriosAvancadosAdminService.obterTempoMedioAprovacoes(request) : Promise.resolve([]),
      podeOperacional.value ? relatoriosAvancadosAdminService.obterAprovacoesPorOrigem(request) : Promise.resolve([]),
    ])

    resumo.value = resumoResp
    tempoMedio.value = tempoResp
    porOrigem.value = origemResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de aprovacoes.'
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
          { grupo: 'Resumo', indicador: 'Total aprovacoes', valor: resumo.value.totalAprovacoes },
          { grupo: 'Resumo', indicador: 'Pendentes', valor: resumo.value.pendentes },
          { grupo: 'Resumo', indicador: 'Aprovadas', valor: resumo.value.aprovadas },
          { grupo: 'Resumo', indicador: 'Reprovadas', valor: resumo.value.reprovadas },
        ]
      : []),
    ...porOrigem.value.map((item) => ({ grupo: 'Origem', indicador: item.tipoOrigem, valor: item.total })),
  ]

  exportarCsv('relatorio-aprovacoes.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Aprovacoes" subtitulo="Acompanhe gargalos, tempo medio e distribuicao por origem.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de aprovacoes.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo e filtros simples de aprovacao.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.statusAprovacao" outlined dense label="Status aprovacao" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.tipoOrigemAprovacao" outlined dense label="Tipo origem" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo && !tempoMedio.length && !porOrigem.length" mensagem="Carregando relatorios de aprovacoes..." />

      <template v-else>
        <div v-if="resumo" class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total" :valor="resumo.totalAprovacoes" icon="fact_check" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Pendentes" :valor="resumo.pendentes" icon="hourglass_top" color="warning" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Aprovadas" :valor="resumo.aprovadas" icon="task_alt" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Reprovadas" :valor="resumo.reprovadas" icon="cancel" color="negative" /></div>
        </div>

        <AppSectionCard v-if="tempoMedio.length" titulo="Tempo medio de decisao">
          <q-table flat :rows="tempoMedio" :columns="colunasTempo" row-key="grupo" hide-pagination>
            <template #body-cell-tempoMedioDecisaoHoras="props">
              <q-td :props="props" class="text-right">{{ props.row.tempoMedioDecisaoHoras == null ? '-' : Number(props.row.tempoMedioDecisaoHoras).toFixed(2) }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <AppSectionCard v-if="porOrigem.length" titulo="Aprovacoes por origem">
          <q-table flat :rows="porOrigem" :columns="colunasOrigem" row-key="tipoOrigem" hide-pagination />
        </AppSectionCard>

        <EmptyState
          v-if="!resumo && !tempoMedio.length && !porOrigem.length"
          titulo="Sem dados de aprovacoes"
          mensagem="Nao ha resultados para os filtros informados."
          icon="fact_check"
        />
      </template>
    </template>
  </q-page>
</template>
