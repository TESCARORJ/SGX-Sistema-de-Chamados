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
  FiltroRelatorioAuditoria,
  RelatorioAuditoriaPorEntidade,
  RelatorioAuditoriaPorUsuario,
  RelatorioAuditoriaResumo,
} from '../types/relatoriosAvancados'
import { exportarCsv } from '../utils/exportCsv'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const resumo = ref<RelatorioAuditoriaResumo | null>(null)
const porUsuario = ref<RelatorioAuditoriaPorUsuario[]>([])
const porEntidade = ref<RelatorioAuditoriaPorEntidade[]>([])

const filtros = reactive<FiltroRelatorioAuditoria>({
  dataInicial: '',
  dataFinal: '',
  termo: '',
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
  { name: 'usuarioNome', label: 'Usuário', field: 'usuarioNome', align: 'left' as const },
  { name: 'totalAcoes', label: 'Total de ações', field: 'totalAcoes', align: 'right' as const },
  { name: 'ultimaAcaoEm', label: 'Última ação', field: 'ultimaAcaoEm', align: 'left' as const },
]

const colunasEntidade = [
  { name: 'entidade', label: 'Entidade', field: 'entidade', align: 'left' as const },
  { name: 'totalAcoes', label: 'Total de ações', field: 'totalAcoes', align: 'right' as const },
  { name: 'usuariosDistintos', label: 'Usuários distintos', field: 'usuariosDistintos', align: 'right' as const },
  { name: 'ultimaAcaoEm', label: 'Última ação', field: 'ultimaAcaoEm', align: 'left' as const },
]

function construirFiltros(): FiltroRelatorioAuditoria {
  return {
    ...filtros,
    dataInicial: filtros.dataInicial || undefined,
    dataFinal: filtros.dataFinal || undefined,
    termo: filtros.termo || undefined,
    entidade: filtros.entidade || undefined,
    tipoAcao: filtros.tipoAcao || undefined,
    limiteRanking: filtros.limiteRanking || undefined,
  }
}

function formatarData(valor: string | null): string {
  if (!valor) {
    return '-'
  }

  const data = new Date(valor)
  return Number.isNaN(data.getTime()) ? valor : data.toLocaleString('pt-BR')
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os relatórios de auditoria.'
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
          { grupo: 'Resumo', indicador: 'Total de ações auditadas', valor: resumo.value.totalAcoesAuditadas },
          { grupo: 'Resumo', indicador: 'Usuários com ações', valor: resumo.value.usuariosComAcoes },
          { grupo: 'Resumo', indicador: 'Entidades afetadas', valor: resumo.value.entidadesAfetadas },
        ]
      : []),
    ...porEntidade.value.map((item) => ({ grupo: 'Entidade', indicador: item.entidade, valor: item.totalAcoes })),
  ]

  exportarCsv('relatorio-auditoria.csv', linhas)
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatórios - Auditoria" subtitulo="Rastreabilidade por usuário, entidade e tipo de ação.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="search" label="Aplicar filtros" :loading="loading" @click="carregar" />
          <q-btn v-if="podeExportar" color="secondary" icon="download" label="Exportar CSV" @click="exportarDados" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar || !podeAuditoria" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar relatórios de auditoria.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Período e filtros de entidade/ação para trilha de auditoria.">
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataInicial" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.dataFinal" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.entidade" outlined dense label="Entidade" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.tipoAcao" outlined dense label="Tipo de ação" /></div>
          <div class="col-12"><q-input v-model="filtros.termo" outlined dense label="Termo" /></div>
        </div>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !resumo" mensagem="Carregando relatórios de auditoria..." />

      <template v-else-if="resumo">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-sm-6 col-lg-4"><MetricCard titulo="Total de ações" :valor="resumo.totalAcoesAuditadas" icon="manage_search" color="primary" /></div>
          <div class="col-12 col-sm-6 col-lg-4"><MetricCard titulo="Usuários com ações" :valor="resumo.usuariosComAcoes" icon="group" color="info" /></div>
          <div class="col-12 col-sm-6 col-lg-4"><MetricCard titulo="Entidades afetadas" :valor="resumo.entidadesAfetadas" icon="fact_check" color="negative" /></div>
        </div>

        <AppSectionCard v-if="porUsuario.length" titulo="Auditoria por usuário">
          <q-table flat :rows="porUsuario" :columns="colunasUsuario" row-key="usuarioNome" hide-pagination>
            <template #body-cell-ultimaAcaoEm="props">
              <q-td :props="props">{{ formatarData(props.row.ultimaAcaoEm) }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>

        <AppSectionCard v-if="porEntidade.length" titulo="Auditoria por entidade">
          <q-table flat :rows="porEntidade" :columns="colunasEntidade" row-key="entidade" hide-pagination>
            <template #body-cell-ultimaAcaoEm="props">
              <q-td :props="props">{{ formatarData(props.row.ultimaAcaoEm) }}</q-td>
            </template>
          </q-table>
        </AppSectionCard>
      </template>

        <EmptyState
        v-else
        titulo="Sem dados de auditoria"
        mensagem="Não há resultados para os filtros informados."
        icon="manage_search"
      />
    </template>
  </q-page>
</template>
