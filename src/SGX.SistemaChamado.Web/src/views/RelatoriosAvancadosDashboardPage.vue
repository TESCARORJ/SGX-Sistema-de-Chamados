<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { relatoriosAvancadosAdminService } from '../services/relatoriosAvancadosAdminService'
import { HttpRequestError } from '../services/httpClient'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroRelatorioAprovacoes,
  FiltroRelatorioAuditoria,
  FiltroRelatorioCatalogo,
  FiltroRelatorioChamados,
  FiltroRelatorioInventario,
  FiltroRelatorioSla,
  RelatorioAprovacoesResumo,
  RelatorioAuditoriaResumo,
  RelatorioChamadosResumo,
  RelatorioInventarioAtivosChamadosRecorrentes,
  RelatoriosAvancadosMetadados,
  RelatorioSlaResumo,
} from '../types/relatoriosAvancados'

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const metadados = ref<RelatoriosAvancadosMetadados | null>(null)
const houveFalhaParcial = ref(false)

const resumoChamados = ref<RelatorioChamadosResumo | null>(null)
const resumoSla = ref<RelatorioSlaResumo | null>(null)
const resumoAprovacoes = ref<RelatorioAprovacoesResumo | null>(null)
const ativosRecorrentes = ref<RelatorioInventarioAtivosChamadosRecorrentes[]>([])
const resumoAuditoria = ref<RelatorioAuditoriaResumo | null>(null)
const rankingCatalogoMaisSolicitados = ref<readonly { nomeServico: string; totalChamados: number }[]>([])

const erroCards = ref<Record<string, string | null>>({
  chamados: null,
  sla: null,
  aprovacoes: null,
  catalogo: null,
  inventario: null,
  auditoria: null,
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
const podeAuditoria = computed(() => possuiPermissao(permissoes.relatoriosAvancadosAuditoria))
const podeVerSecaoTecnica = computed(() => usuarioEhAdministrador.value || podeGerencial.value)

const periodosAmigaveis: Record<string, string> = {
  Hoje: 'Hoje',
  Ontem: 'Ontem',
  Ultimos7Dias: 'Ultimos 7 dias',
  Ultimos30Dias: 'Ultimos 30 dias',
  MesAtual: 'Mes atual',
  MesAnterior: 'Mes anterior',
  TrimestreAtual: 'Trimestre atual',
  AnoAtual: 'Ano atual',
  Personalizado: 'Personalizado',
}

const filtrosAmigaveis: Record<string, string> = {
  DataInicial: 'Periodo',
  DataFinal: 'Periodo',
  DepartamentoId: 'Departamento',
  LocalUnidadeId: 'Local/Unidade',
  UsuarioResponsavelId: 'Responsavel',
  TipoAtivoInventarioId: 'Tipo de ativo',
  StatusOperacional: 'Status operacional',
  StatusPatrimonial: 'Status patrimonial',
  Criticidade: 'Criticidade',
  CategoriaId: 'Categoria',
  SubcategoriaId: 'Subcategoria',
  PrioridadeId: 'Prioridade',
  Status: 'Status do chamado',
  StatusId: 'Status do chamado',
  StatusArtigo: 'Status do artigo',
  VisibilidadeArtigo: 'Visibilidade do artigo',
  AtendenteId: 'Atendente',
  SolicitanteId: 'Solicitante',
  CatalogoServicoId: 'Servico do catalogo',
  UsuarioId: 'Usuario',
  Entidade: 'Entidade auditada',
  TipoAcao: 'Tipo de acao',
  Termo: 'Busca textual',
  TipoOrigemAprovacao: 'Origem da aprovacao',
  StatusAprovacao: 'Status da aprovacao',
  InventarioAtivoId: 'Ativo relacionado',
  Origem: 'Origem',
  Ativo: 'Apenas registros ativos',
  ApenasAtivos: 'Apenas registros ativos',
  Agrupamento: 'Agrupamento',
  AgruparPor: 'Agrupamento',
}

const permissoesAmigaveis: Record<string, string> = {
  'RelatoriosAvancados.Visualizar': 'Visualizar relatorios',
  'RelatoriosAvancados.Exportar': 'Exportar dados',
  'RelatoriosAvancados.Gerencial': 'Acessar relatorios gerenciais',
  'RelatoriosAvancados.Operacional': 'Acessar relatorios operacionais',
  'RelatoriosAvancados.Auditoria': 'Acessar relatorios de auditoria',
}

function normalizarListaUnica(itens: string[]): string[] {
  return Array.from(new Set(itens)).sort((a, b) => a.localeCompare(b))
}

const periodosSuportadosAmigaveis = computed(() => {
  const itens = metadados.value?.periodosSuportados ?? []
  return normalizarListaUnica(itens.map((item) => periodosAmigaveis[item] ?? item))
})

const filtrosDisponiveisAmigaveis = computed(() => {
  const itens = metadados.value?.filtrosDisponiveis ?? []
  return normalizarListaUnica(itens.map((item) => filtrosAmigaveis[item] ?? item))
})

const permissoesRelevantesAmigaveis = computed(() => {
  const itens = metadados.value?.permissoesRelevantes ?? []
  return normalizarListaUnica(itens.map((item) => permissoesAmigaveis[item] ?? item))
})

function formatarDataIsoLocal(data: Date): string {
  const ano = data.getFullYear()
  const mes = String(data.getMonth() + 1).padStart(2, '0')
  const dia = String(data.getDate()).padStart(2, '0')
  return `${ano}-${mes}-${dia}`
}

function formatarDataHoraPadrao(data: Date, fimDoDia: boolean): string {
  const dataBase = formatarDataIsoLocal(data)
  return fimDoDia ? `${dataBase}T23:59:59` : `${dataBase}T00:00:00`
}

function criarFiltroPeriodoPadrao(): Pick<FiltroRelatorioChamados, 'dataInicial' | 'dataFinal'> {
  const hoje = new Date()
  const dataInicial = new Date(hoje)
  dataInicial.setDate(dataInicial.getDate() - 30)

  return {
    dataInicial: formatarDataHoraPadrao(dataInicial, false),
    dataFinal: formatarDataHoraPadrao(hoje, true),
  }
}

function formatarCardServicoMaisSolicitado(): { valor: string | number; subtitulo?: string } {
  if (erroCards.value.catalogo) {
    return { valor: erroCards.value.catalogo }
  }

  if (!podeGerencial.value) {
    return { valor: 'Sem permissao' }
  }

  const item = rankingCatalogoMaisSolicitados.value[0]
  if (!item) {
    return { valor: 'Sem dados no periodo' }
  }

  return {
    valor: item.nomeServico,
    subtitulo: `${item.totalChamados} chamados`,
  }
}

function formatarCardAtivoRecorrente(): { valor: string | number; subtitulo?: string } {
  if (!podeGerencial.value) {
    return { valor: 'Sem permissao' }
  }

  if (erroCards.value.inventario) {
    return { valor: erroCards.value.inventario }
  }

  const item = ativosRecorrentes.value[0]
  if (!item) {
    return { valor: 'Sem dados no periodo' }
  }

  return {
    valor: item.nome,
    subtitulo: `${item.totalChamados} chamados`,
  }
}

function mapearMensagemErroDashboard(error: unknown): string {
  if (error instanceof HttpRequestError) {
    if (error.status === 400) return 'Filtro invalido'
    if (error.status === 401 || error.status === 403) return 'Sem permissao'
    if (error.status === 404) return 'Endpoint nao encontrado'
    if (error.status >= 500) return 'Erro interno ao carregar'
    return 'Erro ao carregar'
  }

  if (error instanceof Error) {
    const mensagem = error.message.toLowerCase()
    if (mensagem.includes('failed to fetch') || mensagem.includes('network')) {
      return 'API indisponivel'
    }
  }

  return 'Erro ao carregar'
}

const cardsResumo = computed(() => {
  const cards: Array<{ titulo: string; valor: string | number; icon: string; color: string; subtitulo?: string }> = []

  cards.push({
    titulo: 'Total de chamados',
    valor: erroCards.value.chamados ?? (resumoChamados.value?.totalChamados ?? 'Sem dados no periodo'),
    icon: 'support_agent',
    color: 'primary',
  })

  if (podeGerencial.value) {
    cards.push({
      titulo: 'Cumprimento de SLA',
      valor: erroCards.value.sla
        ? erroCards.value.sla
        : resumoSla.value?.percentualCumprimento !== null && resumoSla.value?.percentualCumprimento !== undefined
          ? `${Number(resumoSla.value.percentualCumprimento).toFixed(1)}%`
          : 'Sem dados no periodo',
      icon: 'schedule',
      color: 'positive',
    })
  }

  cards.push({
    titulo: 'Aprovacoes pendentes',
    valor: !podeGerencial.value
      ? 'Sem permissao'
      : erroCards.value.aprovacoes
        ? erroCards.value.aprovacoes
        : (resumoAprovacoes.value?.pendentes ?? 'Sem dados no periodo'),
    icon: 'fact_check',
    color: 'warning',
  })

  const servicoMaisSolicitado = formatarCardServicoMaisSolicitado()
  cards.push({
    titulo: 'Servicos mais usados',
    valor: servicoMaisSolicitado.valor,
    subtitulo: servicoMaisSolicitado.subtitulo,
    icon: 'inventory_2',
    color: 'info',
  })

  if (podeGerencial.value) {
    const ativoRecorrente = formatarCardAtivoRecorrente()
    cards.push({
      titulo: 'Ativos com chamados',
      valor: ativoRecorrente.valor,
      subtitulo: ativoRecorrente.subtitulo,
      icon: 'memory',
      color: 'purple',
    })
  }

  if (podeAuditoria.value) {
    cards.push({
      titulo: 'Acoes de auditoria',
      valor: erroCards.value.auditoria
        ? erroCards.value.auditoria
        : (resumoAuditoria.value?.totalAcoesAuditadas ?? 'Sem dados no periodo'),
      icon: 'manage_search',
      color: 'negative',
    })
  }

  return cards
})

const atalhos = computed(() => [
  { titulo: 'Relatorios avancados', rota: '/admin/relatorios/avancados', icon: 'analytics', visivel: podeVisualizar.value },
  { titulo: 'Chamados', rota: '/admin/relatorios/chamados', icon: 'support_agent', visivel: podeVisualizar.value },
  { titulo: 'SLA', rota: '/admin/relatorios/sla', icon: 'schedule', visivel: podeVisualizar.value },
  { titulo: 'Aprovacoes', rota: '/admin/relatorios/aprovacoes', icon: 'fact_check', visivel: podeVisualizar.value },
  { titulo: 'Catalogo de servicos', rota: '/admin/relatorios/catalogo-servicos', icon: 'inventory_2', visivel: podeVisualizar.value },
  { titulo: 'Inventario/Ativos', rota: '/admin/relatorios/inventario-ativos', icon: 'memory', visivel: podeVisualizar.value },
  { titulo: 'Base de conhecimento', rota: '/admin/relatorios/base-conhecimento', icon: 'menu_book', visivel: podeVisualizar.value },
  { titulo: 'Auditoria', rota: '/admin/relatorios/auditoria', icon: 'manage_search', visivel: podeAuditoria.value },
])

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null
  houveFalhaParcial.value = false
  erroCards.value = {
    chamados: null,
    sla: null,
    aprovacoes: null,
    catalogo: null,
    inventario: null,
    auditoria: null,
  }

  try {
    const filtroPeriodoPadrao = criarFiltroPeriodoPadrao()

    const chamadasFiltro: FiltroRelatorioChamados = {
      ...filtroPeriodoPadrao,
      apenasAtivos: false,
    }
    const slaFiltro: FiltroRelatorioSla = {
      ...filtroPeriodoPadrao,
      apenasAtivos: false,
    }
    const aprovacoesFiltro: FiltroRelatorioAprovacoes = {
      ...filtroPeriodoPadrao,
      apenasAtivos: false,
    }
    const catalogoFiltro: FiltroRelatorioCatalogo = {
      ...filtroPeriodoPadrao,
      limiteRanking: 5,
      apenasAtivos: false,
    }
    const inventarioFiltro: FiltroRelatorioInventario = {
      ...filtroPeriodoPadrao,
      limiteRanking: 5,
    }
    const auditoriaFiltro: FiltroRelatorioAuditoria = {
      ...filtroPeriodoPadrao,
      limiteRanking: 5,
    }

    const metadadosPromise = relatoriosAvancadosAdminService.obterMetadados()
    const chamadosPromise = relatoriosAvancadosAdminService.obterResumoChamados(chamadasFiltro)
    const slaPromise = podeGerencial.value
      ? relatoriosAvancadosAdminService.obterResumoSla(slaFiltro)
      : Promise.resolve(null)
    const aprovacoesPromise = podeGerencial.value
      ? relatoriosAvancadosAdminService.obterResumoAprovacoes(aprovacoesFiltro)
      : Promise.resolve(null)
    const catalogoMaisSolicitadosPromise = podeGerencial.value
      ? relatoriosAvancadosAdminService.obterCatalogoServicosMaisSolicitados(catalogoFiltro)
      : Promise.resolve([])
    const inventarioPromise = podeGerencial.value
      ? relatoriosAvancadosAdminService.obterInventarioAtivosChamadosRecorrentes(inventarioFiltro)
      : Promise.resolve([])
    const auditoriaPromise = podeAuditoria.value
      ? relatoriosAvancadosAdminService.obterResumoAuditoria(auditoriaFiltro)
      : Promise.resolve(null)

    const resultados = await Promise.allSettled([
      metadadosPromise,
      chamadosPromise,
      slaPromise,
      aprovacoesPromise,
      catalogoMaisSolicitadosPromise,
      inventarioPromise,
      auditoriaPromise,
    ])

    if (resultados[0].status === 'fulfilled') {
      metadados.value = resultados[0].value as RelatoriosAvancadosMetadados
    }

    if (resultados[1].status === 'fulfilled') {
      resumoChamados.value = resultados[1].value as RelatorioChamadosResumo
    } else {
      erroCards.value.chamados = mapearMensagemErroDashboard(resultados[1].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[2].status === 'fulfilled') {
      resumoSla.value = resultados[2].value as RelatorioSlaResumo | null
    } else if (podeGerencial.value) {
      erroCards.value.sla = mapearMensagemErroDashboard(resultados[2].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[3].status === 'fulfilled') {
      resumoAprovacoes.value = resultados[3].value as RelatorioAprovacoesResumo | null
    } else if (podeGerencial.value) {
      erroCards.value.aprovacoes = mapearMensagemErroDashboard(resultados[3].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[4].status === 'fulfilled') {
      rankingCatalogoMaisSolicitados.value = (resultados[4].value as Array<{ nomeServico: string; totalChamados: number }>) ?? []
    } else if (podeGerencial.value) {
      erroCards.value.catalogo = mapearMensagemErroDashboard(resultados[4].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[5].status === 'fulfilled') {
      ativosRecorrentes.value = (resultados[5].value as RelatorioInventarioAtivosChamadosRecorrentes[]) ?? []
    } else if (podeGerencial.value) {
      erroCards.value.inventario = mapearMensagemErroDashboard(resultados[5].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[6].status === 'fulfilled') {
      resumoAuditoria.value = resultados[6].value as RelatorioAuditoriaResumo | null
    } else if (podeAuditoria.value) {
      erroCards.value.auditoria = mapearMensagemErroDashboard(resultados[6].reason)
      houveFalhaParcial.value = true
    }

    if (resultados[0].status === 'rejected' && resultados[1].status === 'rejected') {
      throw resultados[0].reason ?? resultados[1].reason
    }
  } catch (error) {
    const mensagemAmigavel = mapearMensagemErroDashboard(error)
    erro.value = mensagemAmigavel === 'API indisponivel'
      ? 'API indisponivel. Verifique se a API backend esta em execucao e acessivel.'
      : mensagemAmigavel
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Relatorios avancados" subtitulo="Painel administrativo consolidado de relatorios operacionais, gerenciais e institucionais.">
      <template #actions>
        <q-btn color="primary" icon="refresh" label="Recarregar" :loading="loading" @click="carregar" />
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar os relatorios avancados.
    </q-banner>

    <template v-else>
      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
      <LoadingState v-else-if="loading && !metadados" mensagem="Carregando dashboard de relatorios..." />

      <template v-else>
        <q-banner v-if="houveFalhaParcial" rounded class="bg-amber-1 text-amber-10 q-mb-md">
          Alguns indicadores nao puderam ser carregados. Os demais dados permanecem disponiveis.
        </q-banner>

        <div v-if="cardsResumo.length" class="row q-col-gutter-md">
          <div v-for="card in cardsResumo" :key="card.titulo" class="col-12 col-sm-6 col-lg-4">
            <MetricCard :titulo="card.titulo" :valor="card.valor" :subtitulo="card.subtitulo" :icon="card.icon" :color="card.color" />
          </div>
        </div>

        <AppSectionCard titulo="Acesso rapido" subtitulo="Escolha um relatorio para detalhar filtros, distribuicoes e rankings.">
          <div class="row q-col-gutter-sm">
            <div v-for="item in atalhos.filter((x) => x.visivel)" :key="item.rota" class="col-12 col-sm-6 col-lg-3">
              <q-btn outline color="primary" class="full-width" :icon="item.icon" :label="item.titulo" @click="router.push(item.rota)" />
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard
          v-if="metadados && podeVerSecaoTecnica"
          titulo="Informacoes tecnicas dos relatorios"
          subtitulo="Capacidades informadas pelo backend para montagem dos relatorios."
        >
          <q-expansion-item
            icon="build"
            label="Detalhes tecnicos"
            caption="Visualizacao de apoio para diagnostico e validacao de integracao."
            header-class="text-primary"
            expand-separator
          >
            <div class="row q-col-gutter-md q-pa-sm">
              <div class="col-12 col-lg-4">
                <div class="text-subtitle2 q-mb-sm">Periodos suportados</div>
                <q-list dense bordered>
                  <q-item v-for="periodo in periodosSuportadosAmigaveis" :key="periodo"><q-item-section>{{ periodo }}</q-item-section></q-item>
                </q-list>
              </div>
              <div class="col-12 col-lg-4">
                <div class="text-subtitle2 q-mb-sm">Filtros disponiveis</div>
                <q-list dense bordered>
                  <q-item v-for="filtro in filtrosDisponiveisAmigaveis" :key="filtro"><q-item-section>{{ filtro }}</q-item-section></q-item>
                </q-list>
              </div>
              <div class="col-12 col-lg-4">
                <div class="text-subtitle2 q-mb-sm">Permissoes relevantes</div>
                <q-list dense bordered>
                  <q-item v-for="permissao in permissoesRelevantesAmigaveis" :key="permissao"><q-item-section>{{ permissao }}</q-item-section></q-item>
                </q-list>
              </div>
            </div>
          </q-expansion-item>
        </AppSectionCard>

        <EmptyState
          v-if="!cardsResumo.length && !metadados"
          titulo="Sem dados para dashboard"
          mensagem="Nenhum dado de resumo foi retornado para o perfil atual."
          icon="analytics"
        />
      </template>
    </template>
  </q-page>
</template>
