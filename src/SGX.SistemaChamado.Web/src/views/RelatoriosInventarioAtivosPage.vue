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
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { relatoriosAvancadosAdminService } from '../services/relatoriosAvancadosAdminService'
import { usuariosAdminService } from '../services/usuariosAdminService'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroRelatorioInventario,
  RelatorioInventarioAtivosChamadosRecorrentes,
  RelatorioInventarioAtivosPorDepartamento,
  RelatorioInventarioAtivosPorStatus,
  RelatorioInventarioAtivosResumo,
} from '../types/relatoriosAvancados'
import { CriticidadeAtivo, StatusOperacionalAtivo, StatusPatrimonialAtivo } from '../types/inventarioAtivos'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioInventarioAtivosResumo | null>(null)
const porStatus = ref<RelatorioInventarioAtivosPorStatus | null>(null)
const recorrentes = ref<RelatorioInventarioAtivosChamadosRecorrentes[]>([])
const porDepartamento = ref<RelatorioInventarioAtivosPorDepartamento[]>([])

const departamentos = ref<{ id: string; nome: string }[]>([])
const locaisUnidade = ref<{ id: string; nome: string }[]>([])
const usuarios = ref<{ id: string; nome: string }[]>([])
const tiposAtivo = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioInventario>({
  dataInicial: '',
  dataFinal: '',
  departamentoId: '',
  localUnidadeId: '',
  usuarioResponsavelId: '',
  tipoAtivoInventarioId: '',
  statusOperacional: '',
  statusPatrimonial: '',
  criticidade: '',
  limiteRanking: 20,
})

const opcoesStatusOperacional = [
  { label: 'Operacional', value: String(StatusOperacionalAtivo.Operacional) },
  { label: 'Em manutencao', value: String(StatusOperacionalAtivo.EmManutencao) },
  { label: 'Com defeito', value: String(StatusOperacionalAtivo.ComDefeito) },
  { label: 'Reservado', value: String(StatusOperacionalAtivo.Reservado) },
  { label: 'Baixado', value: String(StatusOperacionalAtivo.Baixado) },
]

const opcoesStatusPatrimonial = [
  { label: 'Em uso', value: String(StatusPatrimonialAtivo.EmUso) },
  { label: 'Em estoque', value: String(StatusPatrimonialAtivo.EmEstoque) },
  { label: 'Emprestado', value: String(StatusPatrimonialAtivo.Emprestado) },
  { label: 'Em transferencia', value: String(StatusPatrimonialAtivo.EmTransferencia) },
  { label: 'Descartado', value: String(StatusPatrimonialAtivo.Descartado) },
  { label: 'Extraviado', value: String(StatusPatrimonialAtivo.Extraviado) },
]

const opcoesCriticidade = [
  { label: 'Baixa', value: String(CriticidadeAtivo.Baixa) },
  { label: 'Media', value: String(CriticidadeAtivo.Media) },
  { label: 'Alta', value: String(CriticidadeAtivo.Alta) },
  { label: 'Critica', value: String(CriticidadeAtivo.Critica) },
]

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
  { name: 'departamento', label: 'Departamento', field: 'departamento', align: 'left' as const },
  { name: 'usuarioResponsavel', label: 'Responsavel', field: 'usuarioResponsavel', align: 'left' as const },
  { name: 'totalChamados', label: 'Total chamados', field: 'totalChamados', align: 'right' as const },
  { name: 'chamadosAbertos', label: 'Abertos', field: 'chamadosAbertos', align: 'right' as const },
]

const colunasDepartamento = [
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left' as const },
  { name: 'totalAtivos', label: 'Total ativos', field: 'totalAtivos', align: 'right' as const },
  { name: 'ativosAtivos', label: 'Ativos', field: 'ativosAtivos', align: 'right' as const },
  { name: 'ativosInativos', label: 'Inativos', field: 'ativosInativos', align: 'right' as const },
  { name: 'totalComChamados', label: 'Com chamados', field: 'totalComChamados', align: 'right' as const },
  { name: 'criticos', label: 'Criticos', field: 'criticos', align: 'right' as const },
]

function construirFiltros(): FiltroRelatorioInventario {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    departamentoId: filtros.departamentoId || undefined,
    localUnidadeId: filtros.localUnidadeId || undefined,
    usuarioResponsavelId: filtros.usuarioResponsavelId || undefined,
    tipoAtivoInventarioId: filtros.tipoAtivoInventarioId || undefined,
    statusOperacional: filtros.statusOperacional || undefined,
    statusPatrimonial: filtros.statusPatrimonial || undefined,
    criticidade: filtros.criticidade || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.departamentoId = ''
  filtros.localUnidadeId = ''
  filtros.usuarioResponsavelId = ''
  filtros.tipoAtivoInventarioId = ''
  filtros.statusOperacional = ''
  filtros.statusPatrimonial = ''
  filtros.criticidade = ''
  filtros.limiteRanking = 20
}

function formatarData(valor: string | null): string {
  if (!valor) {
    return '-'
  }

  const data = new Date(valor)
  return Number.isNaN(data.getTime()) ? valor : data.toLocaleDateString('pt-BR')
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const [contexto, tipos, usuariosResponse] = await Promise.all([
      adminService.obterAdminContexto(),
      inventarioAtivosAdminService.listarTipos(),
      usuariosAdminService.listar({ ativo: true, tamanhoPagina: 200, ordenarPor: 'nome', direcaoOrdenacao: 'asc' }),
    ])

    departamentos.value = contexto.departamentos.map((item) => ({ id: item.id, nome: item.nome }))
    locaisUnidade.value = contexto.locaisUnidade.map((item) => ({ id: item.id, nome: item.nome }))
    tiposAtivo.value = tipos.filter((item) => item.ativo).map((item) => ({ id: item.id, nome: item.nome }))
    usuarios.value = usuariosResponse.items.map((item) => ({ id: item.id, nome: item.nome }))
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
          { grupo: 'Resumo', indicador: 'Com chamados relacionados', valor: resumo.value.totalComChamadosRelacionados },
        ]
      : []),
    ...recorrentes.value.map((item) => ({ grupo: 'Recorrencia', indicador: `${item.codigo} - ${item.nome}`, valor: item.totalChamados })),
  ]

  exportarCsv('relatorio-inventario-ativos.csv', linhas)
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
    <PageHeader titulo="Relatorios - Inventario/Ativos" subtitulo="Visao consolidada de ativos, criticidade e recorrencia de chamados." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeGerencial" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar os relatorios de inventario.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Recorte de periodo, estrutura organizacional e classificacao dos ativos.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
            <div class="col-12 col-md-2">
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
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.localUnidadeId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Local / Unidade"
                :loading="loadingContexto"
                :options="locaisUnidade.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.usuarioResponsavelId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Responsavel"
                :loading="loadingContexto"
                :options="usuarios.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.tipoAtivoInventarioId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Tipo de ativo"
                :loading="loadingContexto"
                :options="tiposAtivo.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2"><q-select v-model="filtros.statusOperacional" outlined dense clearable emit-value map-options label="Status operacional" :options="opcoesStatusOperacional" /></div>
            <div class="col-12 col-md-2"><q-select v-model="filtros.statusPatrimonial" outlined dense clearable emit-value map-options label="Status patrimonial" :options="opcoesStatusPatrimonial" /></div>
            <div class="col-12 col-md-2"><q-select v-model="filtros.criticidade" outlined dense clearable emit-value map-options label="Criticidade" :options="opcoesCriticidade" /></div>
            <div class="col-12 col-md-2"><q-input v-model.number="filtros.limiteRanking" type="number" min="1" max="100" outlined dense label="Limite ranking" /></div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorios de inventario..." />

      <template v-else-if="resumo">
        <div class="sgx-kpi-grid">
          <MetricCard titulo="Total ativos" :valor="resumo.totalAtivos" icon="inventory" tone="primary" />
          <MetricCard titulo="Ativos ativos" :valor="resumo.ativosAtivos" icon="check_circle" tone="positive" />
          <MetricCard titulo="Em manutencao" :valor="resumo.totalEmManutencao" icon="build" tone="warning" />
          <MetricCard titulo="Com defeito" :valor="resumo.totalComDefeito" icon="error" tone="negative" />
          <MetricCard titulo="Com chamados" :valor="resumo.totalComChamadosRelacionados" icon="support_agent" tone="info" />
          <MetricCard titulo="Inativos" :valor="resumo.ativosInativos" icon="pause_circle" tone="warning" />
        </div>

        <AppSectionCard v-if="porStatus" titulo="Distribuicao por status" subtitulo="Composicao operacional, patrimonial e de criticidade.">
          <div class="row q-col-gutter-md">
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Operacional</div>
              <q-table class="sgx-table" flat bordered :rows="porStatus.porStatusOperacional" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
                <template #body-cell-nome="props">
                  <q-td :props="props"><StatusBadge :texto="props.row.nome" /></q-td>
                </template>
              </q-table>
            </div>
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Patrimonial</div>
              <q-table class="sgx-table" flat bordered :rows="porStatus.porStatusPatrimonial" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
                <template #body-cell-nome="props">
                  <q-td :props="props"><StatusBadge :texto="props.row.nome" /></q-td>
                </template>
              </q-table>
            </div>
            <div class="col-12 col-lg-4">
              <div class="text-subtitle2 q-mb-sm">Criticidade</div>
              <q-table class="sgx-table" flat bordered :rows="porStatus.porCriticidade" :columns="colunasDistribuicao" row-key="chave" hide-pagination>
                <template #body-cell-nome="props">
                  <q-td :props="props"><StatusBadge :texto="props.row.nome" /></q-td>
                </template>
              </q-table>
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard titulo="Ativos com chamados recorrentes" :subtitulo="`Ativos no ranking: ${recorrentes.length}`">
          <q-table v-if="recorrentes.length" class="sgx-table" flat bordered :rows="recorrentes" :columns="colunasRecorrentes" row-key="inventarioAtivoId" hide-pagination>
            <template #body-cell-codigo="props">
              <q-td :props="props">
                <div class="column">
                  <span class="text-weight-medium">{{ props.row.codigo }}</span>
                  <span class="text-caption sgx-muted">Ultimo chamado: {{ formatarData(props.row.ultimoChamadoEm) }}</span>
                </div>
              </q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem recorrencia de chamados"
            mensagem="Nao ha ativos recorrentes para os filtros aplicados."
            icon="memory"
          />
        </AppSectionCard>

        <AppSectionCard titulo="Inventario por departamento" :subtitulo="`Departamentos com ativos: ${porDepartamento.length}`">
          <q-table v-if="porDepartamento.length" class="sgx-table" flat bordered :rows="porDepartamento" :columns="colunasDepartamento" row-key="departamentoNome" hide-pagination />
          <EmptyState
            v-else
            titulo="Sem distribuicao por departamento"
            mensagem="Nao ha dados de inventario por departamento para os filtros atuais."
            icon="apartment"
          />
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
