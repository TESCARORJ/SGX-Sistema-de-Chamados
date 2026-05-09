<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import DetalheLogEmail from '../components/admin/DetalheLogEmail.vue'
import FiltrosLogsEmail from '../components/admin/FiltrosLogsEmail.vue'
import TabelaLogsEmail from '../components/admin/TabelaLogsEmail.vue'
import { permissoes } from '../constants/permissoes'
import { integracoesEmailService } from '../services/integracoesEmailService'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroLogsEmailRequest,
  ListaLogsIntegracaoEmailResponse,
  LogIntegracaoEmailDetalheResponse,
} from '../types/integracaoEmail'

const authStore = useAuthStore()
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
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeVisualizarLogsEmail = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.integracoesEmailVisualizar)
)

async function carregarLogs(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    lista.value = await integracoesEmailService.listarLogs(filtros.value)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar os dados.'
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
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar os dados.'
  } finally {
    loadingDetalhe.value = false
  }
}

onMounted(() => {
  if (podeVisualizarLogsEmail.value) {
    void carregarLogs()
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="IntegraÃ§Ã£o de e-mail"
      subtitulo="Acompanhe processamento, correlaÃ§Ã£o e eventos tÃ©cnicos da caixa de entrada"
    />

    <q-banner v-if="!podeVisualizarLogsEmail" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar os logs de integração de e-mail.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros de logs" subtitulo="Defina período, status, remetente e busca textual">
        <FiltrosLogsEmail :loading="loading" @filtrar="aplicarFiltros" />
      </AppSectionCard>

      <LoadingState v-if="loading && !lista.items.length" mensagem="Carregando logs de e-mail..." />

      <ErrorState
        v-else-if="erro && !lista.items.length"
        titulo="Não foi possível carregar os dados."
        :mensagem="erro"
        @retry="carregarLogs"
      />

      <EmptyState
        v-else-if="!lista.items.length"
        titulo="Nenhum log de e-mail encontrado."
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
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
    </template>
  </q-page>
</template>

