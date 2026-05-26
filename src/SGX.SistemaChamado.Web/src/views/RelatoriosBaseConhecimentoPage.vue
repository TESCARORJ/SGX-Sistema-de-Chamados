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
  FiltroRelatorioBaseConhecimento,
  RelatorioBaseConhecimentoPorStatus,
  RelatorioBaseConhecimentoResumo,
  RelatorioBaseConhecimentoVinculosChamados,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioBaseConhecimentoResumo | null>(null)
const porStatus = ref<RelatorioBaseConhecimentoPorStatus | null>(null)
const vinculos = ref<RelatorioBaseConhecimentoVinculosChamados[]>([])
const categorias = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioBaseConhecimento>({
  dataInicial: '',
  dataFinal: '',
  categoriaId: '',
  statusArtigo: '',
  visibilidadeArtigo: '',
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
  { name: 'ultimoVinculoEm', label: 'Ultimo vinculo', field: 'ultimoVinculoEm', align: 'left' as const },
]

function construirFiltros(): FiltroRelatorioBaseConhecimento {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    categoriaId: filtros.categoriaId || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
    statusArtigo: filtros.statusArtigo || undefined,
    visibilidadeArtigo: filtros.visibilidadeArtigo || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.categoriaId = ''
  filtros.statusArtigo = ''
  filtros.visibilidadeArtigo = ''
  filtros.limiteRanking = 20
}

function formatarData(valor: string | null): string {
  if (!valor) {
    return '-'
  }

  const data = new Date(valor)
  return Number.isNaN(data.getTime()) ? valor : data.toLocaleString('pt-BR')
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const contexto = await adminService.obterAdminContexto()
    categorias.value = contexto.categorias.map((item) => ({ id: item.id, nome: item.nome }))
  } finally {
    loadingContexto.value = false
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
          { grupo: 'Resumo', indicador: 'Com vinculo', valor: resumo.value.artigosVinculadosChamados },
        ]
      : []),
    ...vinculos.value.map((item) => ({ grupo: 'Vinculos', indicador: item.titulo, valor: item.totalChamadosVinculados })),
  ]

  exportarCsv('relatorio-base-conhecimento.csv', linhas)
}

onMounted(async () => {
  if (!podeVisualizar.value || !podeGerencial.value) {
    return
  }

  await Promise.all([carregarContexto(), carregar()])
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Base de conhecimento" subtitulo="Status, visibilidade e vinculos de artigos com chamados." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeGerencial" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios da base de conhecimento.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo, categoria, status e visibilidade para leitura da base de conhecimento.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
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
            <div class="col-12 col-md-2"><q-input v-model="filtros.statusArtigo" outlined dense label="Status artigo" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.visibilidadeArtigo" outlined dense label="Visibilidade" /></div>
            <div class="col-12 col-md-1"><q-input v-model.number="filtros.limiteRanking" type="number" min="1" max="100" outlined dense label="Limite" /></div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorios da base de conhecimento..." />

      <template v-else-if="resumo">
        <div class="sgx-kpi-grid">
          <MetricCard titulo="Total artigos" :valor="resumo.totalArtigos" icon="menu_book" tone="primary" />
          <MetricCard titulo="Publicados" :valor="resumo.artigosPublicados" icon="task_alt" tone="positive" />
          <MetricCard titulo="Rascunho" :valor="resumo.artigosRascunho" icon="edit_note" tone="warning" />
          <MetricCard titulo="Arquivados" :valor="resumo.artigosArquivados" icon="archive" tone="info" />
          <MetricCard titulo="Com vinculo" :valor="resumo.artigosVinculadosChamados" icon="link" tone="primary" />
          <MetricCard titulo="Chamados com artigo" :valor="resumo.chamadosComArtigoVinculado" icon="support_agent" tone="warning" />
        </div>

        <AppSectionCard v-if="porStatus" titulo="Distribuicao por status e visibilidade">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-lg-6">
              <div class="text-subtitle2 q-mb-sm">Status</div>
              <q-table class="sgx-table" flat bordered :rows="porStatus.porStatus" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
                <template #body-cell-nome="props">
                  <q-td :props="props"><StatusBadge :texto="props.row.nome" /></q-td>
                </template>
              </q-table>
            </div>
            <div class="col-12 col-lg-6">
              <div class="text-subtitle2 q-mb-sm">Visibilidade</div>
              <q-table class="sgx-table" flat bordered :rows="porStatus.porVisibilidade" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
                <template #body-cell-nome="props">
                  <q-td :props="props"><StatusBadge :texto="props.row.nome" /></q-td>
                </template>
              </q-table>
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard titulo="Artigos vinculados a chamados" :subtitulo="`Artigos no ranking: ${vinculos.length}`">
          <q-table v-if="vinculos.length" class="sgx-table" flat bordered :rows="vinculos" :columns="colunasVinculos" row-key="artigoId" hide-pagination>
            <template #body-cell-status="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.status" />
              </q-td>
            </template>

            <template #body-cell-visibilidade="props">
              <q-td :props="props">
                <StatusBadge :texto="props.row.visibilidade" />
              </q-td>
            </template>

            <template #body-cell-ultimoVinculoEm="props">
              <q-td :props="props">{{ formatarData(props.row.ultimoVinculoEm) }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem artigos vinculados"
            mensagem="Nao ha vinculos entre artigos e chamados para os filtros informados."
            icon="menu_book"
          />
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
