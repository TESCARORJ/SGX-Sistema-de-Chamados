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
import { relatoriosAvancadosAdminService } from '../services/relatoriosAvancadosAdminService'
import { usuariosAdminService } from '../services/usuariosAdminService'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroRelatorioAuditoria,
  RelatorioAuditoriaPorEntidade,
  RelatorioAuditoriaPorUsuario,
  RelatorioAuditoriaResumo,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const loadingContexto = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioAuditoriaResumo | null>(null)
const porUsuario = ref<RelatorioAuditoriaPorUsuario[]>([])
const porEntidade = ref<RelatorioAuditoriaPorEntidade[]>([])
const usuarios = ref<{ id: string; nome: string }[]>([])

const filtros = reactive<FiltroRelatorioAuditoria>({
  dataInicial: '',
  dataFinal: '',
  usuarioId: '',
  termo: '',
  entidade: '',
  tipoAcao: '',
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
const podeAuditoria = computed(() => possuiPermissao(permissoes.relatoriosAvancadosAuditoria))
const podeExportar = computed(() => possuiPermissao(permissoes.relatoriosAvancadosExportar))

const colunasUsuario = [
  { name: 'usuarioNome', label: 'Usuario', field: 'usuarioNome', align: 'left' as const },
  { name: 'totalAcoes', label: 'Total de acoes', field: 'totalAcoes', align: 'right' as const },
  { name: 'ultimaAcaoEm', label: 'Ultima acao', field: 'ultimaAcaoEm', align: 'left' as const },
]

const colunasEntidade = [
  { name: 'entidade', label: 'Entidade', field: 'entidade', align: 'left' as const },
  { name: 'totalAcoes', label: 'Total de acoes', field: 'totalAcoes', align: 'right' as const },
  { name: 'usuariosDistintos', label: 'Usuarios distintos', field: 'usuariosDistintos', align: 'right' as const },
  { name: 'ultimaAcaoEm', label: 'Ultima acao', field: 'ultimaAcaoEm', align: 'left' as const },
]

function construirFiltros(): FiltroRelatorioAuditoria {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    usuarioId: filtros.usuarioId || undefined,
    termo: filtros.termo || undefined,
    entidade: filtros.entidade || undefined,
    tipoAcao: filtros.tipoAcao || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

function limparFiltros(): void {
  filtros.dataInicial = ''
  filtros.dataFinal = ''
  filtros.usuarioId = ''
  filtros.termo = ''
  filtros.entidade = ''
  filtros.tipoAcao = ''
  filtros.limiteRanking = 20
}

function formatarData(valor: string | null): string {
  if (!valor) {
    return '-'
  }

  const data = new Date(valor)
  return Number.isNaN(data.getTime()) ? valor : data.toLocaleString('pt-BR')
}

function formatarAcoesPorTipo(acoes: Array<{ nome: string; quantidade: number }>): string {
  if (!acoes.length) {
    return '-'
  }

  return acoes.slice(0, 3).map((item) => `${item.nome}: ${item.quantidade}`).join(' | ')
}

async function carregarContexto(): Promise<void> {
  loadingContexto.value = true

  try {
    const response = await usuariosAdminService.listar({ ativo: true, tamanhoPagina: 200, ordenarPor: 'nome', direcaoOrdenacao: 'asc' })
    usuarios.value = response.items.map((item) => ({ id: item.id, nome: item.nome }))
  } finally {
    loadingContexto.value = false
  }
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value || !podeAuditoria.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const request = construirFiltros()
    const [resumoResp, usuarioResp, entidadeResp] = await Promise.all([
      relatoriosAvancadosAdminService.obterResumoAuditoria(request),
      relatoriosAvancadosAdminService.obterAuditoriaPorUsuario(request),
      relatoriosAvancadosAdminService.obterAuditoriaPorEntidade(request),
    ])

    resumo.value = resumoResp
    porUsuario.value = usuarioResp
    porEntidade.value = entidadeResp
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os relatorios de auditoria.'
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
          { grupo: 'Resumo', indicador: 'Total de acoes auditadas', valor: resumo.value.totalAcoesAuditadas },
          { grupo: 'Resumo', indicador: 'Usuarios com acoes', valor: resumo.value.usuariosComAcoes },
          { grupo: 'Resumo', indicador: 'Entidades afetadas', valor: resumo.value.entidadesAfetadas },
        ]
      : []),
    ...porEntidade.value.map((item) => ({ grupo: 'Entidade', indicador: item.entidade, valor: item.totalAcoes })),
  ]

  exportarCsv('relatorio-auditoria.csv', linhas)
}

onMounted(async () => {
  if (!podeVisualizar.value || !podeAuditoria.value) {
    return
  }

  await Promise.all([carregarContexto(), carregar()])
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios - Auditoria" subtitulo="Rastreabilidade por usuario, entidade e tipo de acao." contexto="Relatorios">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" unelevated :loading="loading" @click="carregar" />
          <q-btn flat color="primary" icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeAuditoria" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar relatorios de auditoria.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Periodo e recortes de entidade, tipo de acao e usuario para trilha de auditoria.">
        <FilterBar compact>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
            <div class="col-12 col-md-3">
              <q-select
                v-model="filtros.usuarioId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Usuario"
                :loading="loadingContexto"
                :options="usuarios.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.entidade" outlined dense label="Entidade" /></div>
            <div class="col-12 col-md-2"><q-input v-model="filtros.tipoAcao" outlined dense label="Tipo de acao" /></div>
            <div class="col-12 col-md-1"><q-input v-model.number="filtros.limiteRanking" type="number" min="1" max="100" outlined dense label="Limite" /></div>
            <div class="col-12"><q-input v-model="filtros.termo" outlined dense label="Termo" /></div>
          </div>
          <template #actions>
            <q-btn color="primary" icon="search" label="Aplicar" unelevated :loading="loading" @click="carregar" />
            <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
          </template>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatorios de auditoria..." />

      <template v-else-if="resumo">
        <div class="sgx-kpi-grid">
          <MetricCard titulo="Total de acoes" :valor="resumo.totalAcoesAuditadas" icon="manage_search" tone="primary" />
          <MetricCard titulo="Usuarios com acoes" :valor="resumo.usuariosComAcoes" icon="group" tone="info" />
          <MetricCard titulo="Entidades afetadas" :valor="resumo.entidadesAfetadas" icon="fact_check" tone="negative" />
          <MetricCard titulo="Tipos de acao" :valor="resumo.totalPorTipoAcao.length" icon="schema" tone="warning" />
          <MetricCard titulo="Registros diarios" :valor="resumo.totalPorDia.length" icon="today" tone="primary" />
          <MetricCard titulo="Entidades no resumo" :valor="resumo.totalPorEntidade.length" icon="account_tree" tone="warning" />
        </div>

        <AppSectionCard titulo="Auditoria por usuario" :subtitulo="`Usuarios no ranking: ${porUsuario.length}`">
          <q-table v-if="porUsuario.length" class="sgx-table" flat bordered :rows="porUsuario" :columns="colunasUsuario" row-key="usuarioNome" hide-pagination>
            <template #body-cell-usuarioNome="props">
              <q-td :props="props">
                <div class="column">
                  <span class="text-weight-medium">{{ props.row.usuarioNome }}</span>
                  <span class="text-caption sgx-muted">{{ formatarAcoesPorTipo(props.row.acoesPorTipo) }}</span>
                </div>
              </q-td>
            </template>

            <template #body-cell-ultimaAcaoEm="props">
              <q-td :props="props">{{ formatarData(props.row.ultimaAcaoEm) }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem dados por usuario"
            mensagem="Nao ha eventos de auditoria por usuario para os filtros atuais."
            icon="person_search"
          />
        </AppSectionCard>

        <AppSectionCard titulo="Auditoria por entidade" :subtitulo="`Entidades no ranking: ${porEntidade.length}`">
          <q-table v-if="porEntidade.length" class="sgx-table" flat bordered :rows="porEntidade" :columns="colunasEntidade" row-key="entidade" hide-pagination>
            <template #body-cell-entidade="props">
              <q-td :props="props">
                <div class="column">
                  <StatusBadge :texto="props.row.entidade" />
                  <span class="text-caption sgx-muted">{{ formatarAcoesPorTipo(props.row.acoesPorTipo) }}</span>
                </div>
              </q-td>
            </template>

            <template #body-cell-ultimaAcaoEm="props">
              <q-td :props="props">{{ formatarData(props.row.ultimaAcaoEm) }}</q-td>
            </template>
          </q-table>
          <EmptyState
            v-else
            titulo="Sem dados por entidade"
            mensagem="Nao ha eventos de auditoria por entidade para os filtros atuais."
            icon="domain"
          />
        </AppSectionCard>
      </template>

      <EmptyState
        v-else
        titulo="Sem dados de auditoria"
        mensagem="Nao ha resultados para os filtros informados."
        icon="manage_search"
      />
    </template>
  </q-page>
</template>
