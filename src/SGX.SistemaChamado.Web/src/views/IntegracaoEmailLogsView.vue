<script setup lang="ts">
import { onMounted, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import DetalheLogEmail from '../components/admin/DetalheLogEmail.vue'
import FiltrosLogsEmail from '../components/admin/FiltrosLogsEmail.vue'
import TabelaLogsEmail from '../components/admin/TabelaLogsEmail.vue'
import { integracoesEmailService } from '../services/integracoesEmailService'
import type {
  FiltroLogsEmailRequest,
  ListaLogsIntegracaoEmailResponse,
  LogIntegracaoEmailDetalheResponse,
} from '../types/integracaoEmail'

const loading = ref(false)
const loadingDetalhe = ref(false)
const erro = ref<string | null>(null)
const filtros = ref<FiltroLogsEmailRequest>({
  pagina: 1,
  tamanhoPagina: 20,
})
const lista = ref<ListaLogsIntegracaoEmailResponse>({
  items: [],
  total: 0,
  pagina: 1,
  tamanhoPagina: 20,
})
const modalDetalheAberto = ref(false)
const detalheSelecionado = ref<LogIntegracaoEmailDetalheResponse | null>(null)

async function carregarLogs(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    lista.value = await integracoesEmailService.listarLogs(filtros.value)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar logs de integracao.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(novoFiltro: FiltroLogsEmailRequest): Promise<void> {
  filtros.value = { ...filtros.value, ...novoFiltro, pagina: 1 }
  await carregarLogs()
}

async function alterarPagina(pagina: number): Promise<void> {
  filtros.value = { ...filtros.value, pagina }
  await carregarLogs()
}

async function abrirDetalhe(id: string): Promise<void> {
  modalDetalheAberto.value = true
  loadingDetalhe.value = true
  detalheSelecionado.value = null
  try {
    detalheSelecionado.value = await integracoesEmailService.obterLog(id)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar detalhe do log.'
  } finally {
    loadingDetalhe.value = false
  }
}

onMounted(() => {
  void carregarLogs()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Integracoes de e-mail"
      subtitulo="Acompanhe processamento, correlacao e falhas tecnicas da caixa de entrada"
    />

    <AppSectionCard titulo="Filtros de logs" subtitulo="Defina periodo, status, remetente e busca textual">
      <FiltrosLogsEmail :loading="loading" @filtrar="aplicarFiltros" />
    </AppSectionCard>

    <LoadingState v-if="loading && !lista.items.length" mensagem="Carregando logs de integracao..." />

    <ErrorState
      v-else-if="erro && !lista.items.length"
      titulo="Falha ao carregar logs"
      :mensagem="erro"
      @retry="carregarLogs"
    />

    <EmptyState
      v-else-if="!lista.items.length"
      titulo="Nenhum log encontrado"
      mensagem="Nao existem logs para os filtros atuais."
      icon="mail_lock"
    />

    <AppSectionCard v-else titulo="Resultado dos logs" subtitulo="Lista paginada de processamento de e-mails">
      <TabelaLogsEmail
        :rows="lista.items"
        :total="lista.total"
        :pagina="lista.pagina"
        :tamanho-pagina="lista.tamanhoPagina"
        :loading="loading"
        @alterar-pagina="alterarPagina"
        @ver-detalhe="abrirDetalhe"
      />
    </AppSectionCard>

    <DetalheLogEmail v-model="modalDetalheAberto" :detalhe="detalheSelecionado" :loading="loadingDetalhe" />
  </q-page>
</template>
