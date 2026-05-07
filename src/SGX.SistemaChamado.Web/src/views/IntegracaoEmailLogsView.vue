<script setup lang="ts">
import { onMounted, ref } from 'vue'
import DetalheLogEmail from '../components/admin/DetalheLogEmail.vue'
import FiltrosLogsEmail from '../components/admin/FiltrosLogsEmail.vue'
import TabelaLogsEmail from '../components/admin/TabelaLogsEmail.vue'
import PageHeader from '../components/ui/PageHeader.vue'
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

onMounted(carregarLogs)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Integracoes de e-mail"
      subtitulo="Acompanhe processamento, correlacao e falhas tecnicas da caixa de entrada"
    />

    <FiltrosLogsEmail :loading="loading" @filtrar="aplicarFiltros" />

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <q-card flat bordered class="sgx-card">
      <q-card-section>
        <TabelaLogsEmail
          :rows="lista.items"
          :total="lista.total"
          :pagina="lista.pagina"
          :tamanho-pagina="lista.tamanhoPagina"
          :loading="loading"
          @alterar-pagina="alterarPagina"
          @ver-detalhe="abrirDetalhe"
        />
      </q-card-section>
    </q-card>

    <q-banner v-if="!loading && !lista.items.length" rounded class="bg-blue-1 text-primary">
      Nenhum log de integracao encontrado para os filtros aplicados.
    </q-banner>

    <DetalheLogEmail v-model="modalDetalheAberto" :detalhe="detalheSelecionado" :loading="loadingDetalhe" />
  </q-page>
</template>
