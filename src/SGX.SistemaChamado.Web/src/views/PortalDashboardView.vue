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
import { portalService } from '../services/portalService'
import type { ChamadoResumoPortal } from '../types/portal'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)
const chamados = ref<ChamadoResumoPortal[]>([])

const chamadosAbertos = computed(() => chamados.value.filter((item) => item.status.toLowerCase().includes('aberto')).length)
const chamadosEmAtendimento = computed(() =>
  chamados.value.filter((item) => item.status.toLowerCase().includes('atendimento')).length
)
const chamadosAguardandoSolicitante = computed(() =>
  chamados.value.filter((item) => item.status.toLowerCase().includes('aguardando')).length
)
const chamadosResolvidosEncerrados = computed(() =>
  chamados.value.filter((item) => ['resolvido', 'encerrado'].some((status) => item.status.toLowerCase().includes(status))).length
)

const ultimosChamados = computed(() => chamados.value.slice(0, 6))

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await portalService.listarMeusChamados({ pagina: 1, tamanhoPagina: 30 })
    chamados.value = response.items
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar o portal do solicitante.'
  } finally {
    loading.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Meu portal" subtitulo="Acompanhe seus chamados e solicitacoes.">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="secondary" icon="add" label="Abrir chamado" @click="router.push('/portal/chamados/novo')" />
          <q-btn flat color="primary" icon="list_alt" label="Meus chamados" @click="router.push('/portal/chamados')" />
        </div>
      </template>
    </PageHeader>

    <div class="row q-col-gutter-md">
      <div class="col-12 col-sm-6 col-lg-3">
        <MetricCard titulo="Chamados abertos" :valor="chamadosAbertos" icon="drafts" color="primary" />
      </div>
      <div class="col-12 col-sm-6 col-lg-3">
        <MetricCard titulo="Em atendimento" :valor="chamadosEmAtendimento" icon="support_agent" color="warning" />
      </div>
      <div class="col-12 col-sm-6 col-lg-3">
        <MetricCard
          titulo="Aguardando solicitante"
          :valor="chamadosAguardandoSolicitante"
          icon="hourglass_top"
          color="deep-orange"
        />
      </div>
      <div class="col-12 col-sm-6 col-lg-3">
        <MetricCard
          titulo="Resolvidos/encerrados"
          :valor="chamadosResolvidosEncerrados"
          icon="task_alt"
          color="positive"
        />
      </div>
    </div>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando informacoes do seu portal..." />

    <AppSectionCard
      v-else
      titulo="Ultimos chamados"
      subtitulo="Historico recente para acompanhar o andamento das solicitacoes"
    >
      <div class="column q-gutter-sm">
        <CardChamado
          v-for="item in ultimosChamados"
          :key="item.id"
          :chamado="item"
          class="cursor-pointer"
          @click="router.push(`/portal/chamados/${item.id}`)"
        />

        <EmptyState
          v-if="!ultimosChamados.length"
          titulo="Sem chamados recentes"
          mensagem="Quando houver novos chamados, eles aparecerao aqui para acompanhamento rapido."
        >
          <template #actions>
            <q-btn color="secondary" icon="add" label="Abrir chamado" @click="router.push('/portal/chamados/novo')" />
          </template>
        </EmptyState>
      </div>
    </AppSectionCard>
  </q-page>
</template>
