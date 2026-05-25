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
  FiltroRelatorioBaseConhecimento,
  RelatorioBaseConhecimentoPorStatus,
  RelatorioBaseConhecimentoResumo,
  RelatorioBaseConhecimentoVinculosChamados,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioBaseConhecimentoResumo | null>(null)
const porStatus = ref<RelatorioBaseConhecimentoPorStatus | null>(null)
const vinculos = ref<RelatorioBaseConhecimentoVinculosChamados[]>([])

const filtros = reactive<FiltroRelatorioBaseConhecimento>({
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
  { name: 'percentual', label: 'Percentual', field: 'percentual', align: 'right' as const },
]

const colunasVinculos = [
  { name: 'titulo', label: 'Artigo', field: 'titulo', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'status', align: 'left' as const },
  { name: 'visibilidade', label: 'Visibilidade', field: 'visibilidade', align: 'left' as const },
  { name: 'totalChamadosVinculados', label: 'Chamados vinculados', field: 'totalChamadosVinculados', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioBaseConhecimento {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
    statusArtigo: filtros.statusArtigo || undefined,
    visibilidadeArtigo: filtros.visibilidadeArtigo || undefined,
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
    const [resumoResp, statusResp, vinculosResp] = await Promise.all([
      relatoriosAvancadosAdminService.obterResumoBaseConhecimento(request),
      relatoriosAvancadosAdminService.obterBaseConhecimentoPorStatus(request),
      relatoriosAvancadosAdminService.obterBaseConhecimentoVinculosChamados(request),
    ])

    resumo.value = resumoResp
    porStatus.value = statusResp
    vinculos.value = vinculosResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios da base de conhecimento.'
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
          { grupo: 'Resumo', indicador: 'Total artigos', valor: resumo.value.totalArtigos },
          { grupo: 'Resumo', indicador: 'Publicados', valor: resumo.value.artigosPublicados },
          { grupo: 'Resumo', indicador: 'Arquivados', valor: resumo.value.artigosArquivados },
        ]
      : []),
    ...vinculos.value.map((item) => ({ grupo: 'Vinculos', indicador: item.titulo, valor: item.totalChamadosVinculados })),
  ]

  exportarCsv('relatorio-base-conhecimento.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Base de conhecimento" subtitulo="Status, visibilidade e vinculos de artigos com chamados.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeGerencial" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios da base de conhecimento.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo e status para recorte da base de conhecimento.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.statusArtigo" outlined dense label="Status artigo" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.visibilidadeArtigo" outlined dense label="Visibilidade" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorios da base de conhecimento..." />

      <template v-else-if="resumo">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Total artigos" :valor="resumo.totalArtigos" icon="menu_book" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Publicados" :valor="resumo.artigosPublicados" icon="task_alt" color="positive" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Rascunho" :valor="resumo.artigosRascunho" icon="edit_note" color="warning" /></div>
          <div class="col-12 col-sm-6 col-lg-3"><MetricCard titulo="Com vinculo" :valor="resumo.artigosVinculadosChamados" icon="link" color="info" /></div>
        </div>

        <AppSectionCard v-if="porStatus" titulo="Distribuicao por status e visibilidade">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-lg-6">
              <div class="text-subtitle2 q-mb-sm">Status</div>
              <q-table flat :rows="porStatus.porStatus" :columns="colunasDistribuicao" row-key="chave" hide-pagination />
            </div>
            <div class="col-12 col-lg-6">
              <div class="text-subtitle2 q-mb-sm">Visibilidade</div>
              <q-table flat :rows="porStatus.porVisibilidade" :columns="colunasDistribuicao" row-key="chave" hide-pagination />
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard v-if="vinculos.length" titulo="Artigos vinculados a chamados">
          <q-table flat :rows="vinculos" :columns="colunasVinculos" row-key="artigoId" hide-pagination />
        </AppSectionCard>
      </template>

      <EmptyState
        v-else
        titulo="Sem dados de base de conhecimento"
        mensagem="Nao ha resultados para os filtros informados."
        icon="menu_book"
      />
    </template>
  </q-page>
</template>
