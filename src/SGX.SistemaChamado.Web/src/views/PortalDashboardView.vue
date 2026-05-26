<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import CardChamado from '../components/portal/CardChamado.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { baseConhecimentoPortalService } from '../services/baseConhecimentoPortalService'
import { catalogoServicosPortalService } from '../services/catalogoServicosPortalService'
import { portalService } from '../services/portalService'
import type { ChamadoResumoPortal } from '../types/portal'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const chamados = ref<ChamadoResumoPortal[]>([])

const totalServicos = ref<number | null>(null)
const totalArtigos = ref<number | null>(null)

const chamadosAbertos = computed(() => chamados.value.filter((item) => item.status.toLowerCase().includes('aberto')).length)
const chamadosEmAtendimento = computed(() =>
  chamados.value.filter((item) => item.status.toLowerCase().includes('atendimento')).length
)
const chamadosConcluidos = computed(() =>
  chamados.value.filter((item) => ['resolvido', 'encerrado'].some((status) => item.status.toLowerCase().includes(status))).length
)
const chamadosRiscoSla = computed(() => chamados.value.filter((item) => item.slaVencido || item.slaProximoVencimento).length)
const totalChamados = computed(() => chamados.value.length)
const ultimosChamados = computed(() => chamados.value.slice(0, 5))

const atalhoPrincipal = [
  {
    titulo: 'Abrir novo chamado',
    descricao: 'Registre incidentes, requisicoes e duvidas de forma guiada.',
    icon: 'add_circle',
    color: 'secondary',
    rota: '/portal/chamados/novo',
  },
  {
    titulo: 'Meus chamados',
    descricao: 'Acompanhe status, prioridade e historico das solicitacoes.',
    icon: 'receipt_long',
    color: 'primary',
    rota: '/portal/chamados',
  },
  {
    titulo: 'Catalogo de servicos',
    descricao: 'Consulte servicos disponiveis e abra chamados por catalogo.',
    icon: 'inventory_2',
    color: 'primary',
    rota: '/portal/catalogo-servicos',
  },
  {
    titulo: 'Base de conhecimento',
    descricao: 'Encontre orientacoes e procedimentos antes de abrir um chamado.',
    icon: 'menu_book',
    color: 'primary',
    rota: '/portal/base-conhecimento',
  },
]

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null
  totalServicos.value = null
  totalArtigos.value = null

  try {
    const chamadosResponse = await portalService.listarMeusChamados({ pagina: 1, tamanhoPagina: 40 })
    chamados.value = chamadosResponse.items

    const [servicosResult, artigosResult] = await Promise.allSettled([
      catalogoServicosPortalService.listarServicos({ pagina: 1, tamanhoPagina: 1 }),
      baseConhecimentoPortalService.listarArtigos({ pagina: 1, tamanhoPagina: 1 }),
    ])

    if (servicosResult.status === 'fulfilled') {
      totalServicos.value = servicosResult.value.total
    }

    if (artigosResult.status === 'fulfilled') {
      totalArtigos.value = artigosResult.value.total
    }
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os dados do portal.'
  } finally {
    loading.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Portal do solicitante"
      titulo="Visao geral"
      subtitulo="Acesse rapidamente chamados, servicos e conhecimento para resolver demandas com agilidade."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="secondary" icon="add" label="Novo chamado" @click="router.push('/portal/chamados/novo')" />
          <q-btn flat color="primary" icon="receipt_long" label="Meus chamados" @click="router.push('/portal/chamados')" />
        </div>
      </template>
    </PageHeader>

    <div class="sgx-kpi-grid">
      <MetricCard title="Chamados abertos" :value="chamadosAbertos" icon="drafts" tone="primary" :loading="loading" />
      <MetricCard title="Em atendimento" :value="chamadosEmAtendimento" icon="support_agent" tone="info" :loading="loading" />
      <MetricCard title="Concluidos" :value="chamadosConcluidos" icon="task_alt" tone="positive" :loading="loading" />
      <MetricCard
        title="Risco de SLA"
        :value="chamadosRiscoSla"
        icon="warning"
        :tone="chamadosRiscoSla > 0 ? 'negative' : 'warning'"
        :loading="loading"
      />
      <MetricCard
        title="Servicos no catalogo"
        :value="totalServicos ?? '-'"
        :caption="totalServicos === null ? 'Indisponivel no momento' : 'Dados reais do catalogo'"
        icon="inventory"
        tone="primary"
        :loading="loading"
      />
      <MetricCard
        title="Artigos publicados"
        :value="totalArtigos ?? '-'"
        :caption="totalArtigos === null ? 'Indisponivel no momento' : 'Dados reais da base'"
        icon="article"
        tone="info"
        :loading="loading"
      />
    </div>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
    <LoadingState v-else-if="loading" inline mensagem="Carregando visao inicial do portal..." />

    <template v-else>
      <AppSectionCard titulo="Atalhos rapidos" subtitulo="Navegue pelas acoes principais do portal com um clique.">
        <div class="row q-col-gutter-md">
          <div v-for="atalho in atalhoPrincipal" :key="atalho.rota" class="col-12 col-sm-6">
            <q-card flat bordered class="sgx-card portal-atalho cursor-pointer" @click="router.push(atalho.rota)">
              <q-card-section class="row items-start justify-between">
                <div class="col">
                  <div class="text-subtitle1 text-weight-bold">{{ atalho.titulo }}</div>
                  <div class="text-caption text-grey-7 q-mt-xs">{{ atalho.descricao }}</div>
                </div>
                <q-avatar size="40px" :class="atalho.color === 'secondary' ? 'bg-secondary text-white' : 'bg-blue-1 text-primary'">
                  <q-icon :name="atalho.icon" />
                </q-avatar>
              </q-card-section>
            </q-card>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard
        titulo="Meus chamados recentes"
        :subtitulo="totalChamados ? 'Acompanhamento rapido das solicitacoes mais recentes.' : 'Ainda nao ha chamados registrados.'"
      >
        <EmptyState
          v-if="!ultimosChamados.length"
          titulo="Nenhum chamado encontrado"
          mensagem="Voce ainda nao possui chamados. Abra um novo chamado para iniciar o atendimento."
          icon="inbox"
        >
          <template #actions>
            <q-btn color="secondary" icon="add" label="Abrir chamado" @click="router.push('/portal/chamados/novo')" />
          </template>
        </EmptyState>

        <div v-else class="column q-gutter-sm">
          <CardChamado
            v-for="item in ultimosChamados"
            :key="item.id"
            :chamado="item"
            class="cursor-pointer"
            @click="router.push(`/portal/chamados/${item.id}`)"
          />

          <div class="row justify-end q-mt-sm">
            <q-btn flat color="primary" icon="list_alt" label="Ver todos os chamados" @click="router.push('/portal/chamados')" />
          </div>
        </div>
      </AppSectionCard>
    </template>
  </q-page>
</template>

<style scoped>
.portal-atalho {
  min-height: 126px;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.portal-atalho:hover {
  transform: translateY(-1px);
  box-shadow: var(--sgx-shadow-md);
}
</style>
