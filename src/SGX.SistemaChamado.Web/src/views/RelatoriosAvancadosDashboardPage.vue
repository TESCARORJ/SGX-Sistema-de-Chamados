<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { relatoriosAvancadosAdminService } from '../services/relatoriosAvancadosAdminService'
import { useAuthStore } from '../stores/authStore'
import { criarFiltrosResumoDashboard, mapearMensagemErroDashboard } from './relatoriosAvancadosDashboard.helpers'
import type {
  RelatorioAprovacoesResumo,
  RelatorioAuditoriaResumo,
  RelatorioChamadosResumo,
  RelatorioInventarioAtivosChamadosRecorrentes,
  RelatoriosAvancadosMetadados,
  RelatorioSlaResumo,
} from '../types/relatoriosAvancados'

const router = useRouter()
const route = useRoute()
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
const podeConsultarSla = computed(() => podeVisualizar.value && podeGerencial.value)
const podeConsultarAprovacoes = computed(() => podeVisualizar.value && podeGerencial.value)
const podeConsultarCatalogo = computed(() => podeVisualizar.value && podeGerencial.value)
const podeConsultarInventario = computed(() => podeVisualizar.value && podeGerencial.value)
const podeConsultarAuditoria = computed(() => podeVisualizar.value && podeAuditoria.value)

function formatarCardServicoMaisSolicitado(): { valor: string | number; subtitulo?: string } {
  if (erroCards.value.catalogo) {
    return { valor: erroCards.value.catalogo }
  }

  if (!podeConsultarCatalogo.value) {
    return { valor: 'Sem permissao' }
  }

  const item = rankingCatalogoMaisSolicitados.value[0]
  if (!item) {
    return { valor: 'Sem dados no periodo' }
  }

  return {
    valor: `${item.nomeServico} - ${item.totalChamados} chamados`,
  }
}

function formatarCardAtivoRecorrente(): { valor: string | number; subtitulo?: string } {
  if (!podeConsultarInventario.value) {
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
    valor: `${item.codigo} - ${item.totalChamados} chamados`,
  }
}

const cardsResumo = computed(() => {
  const cards: Array<{ titulo: string; valor: string | number; icon: string; color: string; subtitulo?: string }> = []

  cards.push({
    titulo: 'Total de chamados',
    valor: erroCards.value.chamados ?? (resumoChamados.value?.totalChamados ?? 'Sem dados no periodo'),
    icon: 'support_agent',
    color: 'primary',
  })

  cards.push({
    titulo: 'Cumprimento de SLA',
    valor: !podeConsultarSla.value
      ? 'Sem permissao'
      : erroCards.value.sla
        ? erroCards.value.sla
        : resumoSla.value?.percentualCumprimento !== null && resumoSla.value?.percentualCumprimento !== undefined
          ? `${Number(resumoSla.value.percentualCumprimento).toFixed(1)}%`
          : resumoSla.value?.totalChamadosComSla === 0
            ? '0%'
            : 'Sem dados no periodo',
    icon: 'schedule',
    color: 'positive',
  })

  cards.push({
    titulo: 'Aprovacoes pendentes',
    valor: !podeConsultarAprovacoes.value
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

  const ativoRecorrente = formatarCardAtivoRecorrente()
  cards.push({
    titulo: 'Ativos com chamados',
    valor: ativoRecorrente.valor,
    subtitulo: ativoRecorrente.subtitulo,
    icon: 'memory',
    color: 'purple',
  })

  cards.push({
    titulo: 'Acoes de auditoria',
    valor: !podeConsultarAuditoria.value
      ? 'Sem permissao'
      : erroCards.value.auditoria
        ? erroCards.value.auditoria
        : (resumoAuditoria.value?.totalAcoesAuditadas ?? 'Sem dados no periodo'),
    icon: 'manage_search',
    color: 'negative',
  })

  return cards
})

const possuiIndicadoresCarregados = computed(() => {
  return (
    Boolean(resumoChamados.value) ||
    Boolean(resumoSla.value) ||
    Boolean(resumoAprovacoes.value) ||
    Boolean(resumoAuditoria.value) ||
    rankingCatalogoMaisSolicitados.value.length > 0 ||
    ativosRecorrentes.value.length > 0
  )
})

const rotaAtualRelatoriosAvancados = '/admin/relatorios/avancados'

const atalhosDetalhados = [
  { titulo: 'Chamados', rota: '/admin/relatorios/chamados', icon: 'support_agent', visivel: () => podeVisualizar.value },
  { titulo: 'SLA', rota: '/admin/relatorios/sla', icon: 'schedule', visivel: () => podeVisualizar.value },
  { titulo: 'Aprovacoes', rota: '/admin/relatorios/aprovacoes', icon: 'fact_check', visivel: () => podeVisualizar.value },
  { titulo: 'Catalogo de servicos', rota: '/admin/relatorios/catalogo-servicos', icon: 'inventory_2', visivel: () => podeVisualizar.value },
  { titulo: 'Inventario/Ativos', rota: '/admin/relatorios/inventario-ativos', icon: 'memory', visivel: () => podeVisualizar.value },
  { titulo: 'Base de conhecimento', rota: '/admin/relatorios/base-conhecimento', icon: 'menu_book', visivel: () => podeVisualizar.value },
  { titulo: 'Auditoria', rota: '/admin/relatorios/auditoria', icon: 'manage_search', visivel: () => podeAuditoria.value },
]

function normalizarRota(rota: string): string {
  return rota.endsWith('/') && rota.length > 1 ? rota.slice(0, -1) : rota
}

const atalhos = computed(() => {
  const rotaAtualNormalizada = normalizarRota(route.path)
  const rotaDashboardNormalizada = normalizarRota(rotaAtualRelatoriosAvancados)

  return atalhosDetalhados.filter((item) => {
    if (!item.visivel()) {
      return false
    }

    const rotaItemNormalizada = normalizarRota(item.rota)
    if (rotaAtualNormalizada === rotaDashboardNormalizada && rotaItemNormalizada === rotaDashboardNormalizada) {
      return false
    }

    return true
  })
})

function registrarFalhaCard(card: keyof typeof erroCards.value, motivo: unknown): void {
  const mensagem = mapearMensagemErroDashboard(motivo)
  erroCards.value[card] = mensagem
  houveFalhaParcial.value = true

  if (import.meta.env.DEV) {
    console.warn(`[RelatoriosAvancadosDashboard] Falha ao carregar card "${card}"`, motivo)
  }
}

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
    const filtrosResumo = criarFiltrosResumoDashboard()

    const metadadosPromise = relatoriosAvancadosAdminService.obterMetadados()
    const chamadosPromise = relatoriosAvancadosAdminService.obterResumoChamados(filtrosResumo.chamados)
    const slaPromise = podeConsultarSla.value
      ? relatoriosAvancadosAdminService.obterResumoSla(filtrosResumo.sla)
      : Promise.resolve(null)
    const aprovacoesPromise = podeConsultarAprovacoes.value
      ? relatoriosAvancadosAdminService.obterResumoAprovacoes(filtrosResumo.aprovacoes)
      : Promise.resolve(null)
    const catalogoMaisSolicitadosPromise = podeConsultarCatalogo.value
      ? relatoriosAvancadosAdminService.obterCatalogoServicosMaisSolicitados(filtrosResumo.catalogo)
      : Promise.resolve([])
    const inventarioPromise = podeConsultarInventario.value
      ? relatoriosAvancadosAdminService.obterInventarioAtivosChamadosRecorrentes(filtrosResumo.inventario)
      : Promise.resolve([])
    const auditoriaPromise = podeConsultarAuditoria.value
      ? relatoriosAvancadosAdminService.obterResumoAuditoria(filtrosResumo.auditoria)
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
      registrarFalhaCard('chamados', resultados[1].reason)
    }

    if (resultados[2].status === 'fulfilled') {
      resumoSla.value = resultados[2].value as RelatorioSlaResumo | null
    } else if (podeConsultarSla.value) {
      registrarFalhaCard('sla', resultados[2].reason)
    }

    if (resultados[3].status === 'fulfilled') {
      resumoAprovacoes.value = resultados[3].value as RelatorioAprovacoesResumo | null
    } else if (podeConsultarAprovacoes.value) {
      registrarFalhaCard('aprovacoes', resultados[3].reason)
    }

    if (resultados[4].status === 'fulfilled') {
      rankingCatalogoMaisSolicitados.value = (resultados[4].value as Array<{ nomeServico: string; totalChamados: number }>) ?? []
    } else if (podeConsultarCatalogo.value) {
      registrarFalhaCard('catalogo', resultados[4].reason)
    }

    if (resultados[5].status === 'fulfilled') {
      ativosRecorrentes.value = (resultados[5].value as RelatorioInventarioAtivosChamadosRecorrentes[]) ?? []
    } else if (podeConsultarInventario.value) {
      registrarFalhaCard('inventario', resultados[5].reason)
    }

    if (resultados[6].status === 'fulfilled') {
      resumoAuditoria.value = resultados[6].value as RelatorioAuditoriaResumo | null
    } else if (podeConsultarAuditoria.value) {
      registrarFalhaCard('auditoria', resultados[6].reason)
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
    <PageHeader
      titulo="Relatorios avancados"
      subtitulo="Painel gerencial consolidado para analise operacional, gerencial e de auditoria."
      contexto="Relatorios"
    >
      <template #actions>
        <q-btn color="primary" icon="refresh" label="Recarregar painel" unelevated :loading="loading" @click="carregar" />
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

        <div v-if="cardsResumo.length" class="sgx-kpi-grid">
          <MetricCard
            v-for="card in cardsResumo"
            :key="card.titulo"
            :titulo="card.titulo"
            :valor="card.valor"
            :subtitulo="card.subtitulo"
            :icon="card.icon"
            :color="card.color"
          />
        </div>

        <AppSectionCard titulo="Acesso rapido aos relatorios" subtitulo="Navegue para analises especificas por dominio funcional.">
          <div v-if="atalhos.length" class="row q-col-gutter-md">
            <div v-for="item in atalhos" :key="item.rota" class="col-12 col-sm-6 col-lg-3">
              <q-card flat bordered class="sgx-card dashboard-atalho cursor-pointer" @click="router.push(item.rota)">
                <q-card-section class="row items-center no-wrap q-gutter-sm">
                  <q-avatar color="blue-1" text-color="primary" icon="insights" />
                  <div class="col">
                    <div class="text-subtitle2 text-weight-bold">{{ item.titulo }}</div>
                    <div class="text-caption sgx-muted">Abrir relatorio detalhado</div>
                  </div>
                  <q-icon :name="item.icon" color="primary" size="20px" />
                </q-card-section>
              </q-card>
            </div>
          </div>
          <EmptyState
            v-else
            titulo="Nenhum relatorio disponivel"
            mensagem="Nao ha relatorios acessiveis para o perfil atual."
            icon="analytics"
          />
        </AppSectionCard>

        <AppSectionCard
          v-if="metadados"
          titulo="Contexto dos dados"
          subtitulo="Metadados recebidos da API para orientar leitura do dashboard."
        >
          <div class="row q-col-gutter-md">
            <div class="col-12 col-lg-6">
              <div class="text-caption sgx-muted q-mb-xs">Periodos suportados</div>
              <div class="row q-gutter-xs">
                <q-chip v-for="item in metadados.periodosSuportados" :key="item" dense color="grey-2" text-color="grey-9" :label="item" />
                <q-chip v-if="!metadados.periodosSuportados.length" dense color="grey-2" text-color="grey-9" label="Sem periodos informados" />
              </div>
            </div>
            <div class="col-12 col-lg-6">
              <div class="text-caption sgx-muted q-mb-xs">Filtros disponiveis</div>
              <div class="row q-gutter-xs">
                <q-chip v-for="item in metadados.filtrosDisponiveis" :key="item" dense color="blue-1" text-color="primary" :label="item" />
                <q-chip v-if="!metadados.filtrosDisponiveis.length" dense color="grey-2" text-color="grey-9" label="Sem filtros catalogados" />
              </div>
            </div>
          </div>
        </AppSectionCard>

        <EmptyState
          v-if="!possuiIndicadoresCarregados && !metadados"
          titulo="Sem dados para dashboard"
          mensagem="Nenhum dado de resumo foi retornado para o perfil atual."
          icon="analytics"
        />
      </template>
    </template>
  </q-page>
</template>

<style scoped>
.dashboard-atalho {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.dashboard-atalho:hover {
  transform: translateY(-2px);
  box-shadow: var(--sgx-shadow-md);
}
</style>
