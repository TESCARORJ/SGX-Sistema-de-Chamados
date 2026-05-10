<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
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

const router = useRouter()
const authStore = useAuthStore()
const loading = ref(false)
const loadingDetalhe = ref(false)
const erro = ref<string | null>(null)
const filtrosIniciais: FiltroLogsEmailRequest = {
  pagina: 1,
  tamanhoPagina: 20,
}
const filtros = ref<FiltroLogsEmailRequest>({ ...filtrosIniciais })
const filtrosKey = ref(0)
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
    lista.value = await integracoesEmailService.listarLogsEmail(filtros.value)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os logs de integracao de e-mail.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(novoFiltro: FiltroLogsEmailRequest): Promise<void> {
  filtros.value = { ...filtros.value, ...novoFiltro, pagina: 1 }
  await carregarLogs()
}

async function limparFiltros(): Promise<void> {
  filtros.value = { ...filtrosIniciais }
  filtrosKey.value += 1
  await carregarLogs()
}

async function alterarPagina(pagina: number): Promise<void> {
  filtros.value = { ...filtros.value, pagina }
  await carregarLogs()
}

function abrirChamado(id: string): void {
  void router.push(`/admin/chamados/${id}`)
}

async function abrirDetalhe(id: string): Promise<void> {
  modalDetalheAberto.value = true
  loadingDetalhe.value = true
  detalheSelecionado.value = null
  try {
    detalheSelecionado.value = await integracoesEmailService.obterLogEmail(id)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os logs de integracao de e-mail.'
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
      titulo="Logs de integração de e-mail"
      subtitulo="Acompanhe mensagens processadas pelo Worker de e-mail, falhas, duplicidades e chamados vinculados."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" label="Atualizar" :loading="loading" @click="carregarLogs" />
          <q-btn flat color="primary" label="Limpar filtros" :disable="loading" @click="limparFiltros" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizarLogsEmail" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar os logs de integracao de e-mail.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Use periodo, status, remetente, chamado, assunto e MessageId.">
        <FiltrosLogsEmail :key="filtrosKey" :loading="loading" @filtrar="aplicarFiltros" />
      </AppSectionCard>

      <LoadingState v-if="loading && !lista.items.length" mensagem="Carregando logs de e-mail..." />

      <ErrorState
        v-else-if="erro && !lista.items.length"
        titulo="Nao foi possivel carregar os logs de integracao de e-mail."
        :mensagem="erro"
        @retry="carregarLogs"
      />

      <EmptyState
        v-else-if="!lista.items.length"
        titulo="Nenhum log de e-mail encontrado."
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
        icon="mail_lock"
      />

      <AppSectionCard v-else titulo="Lista de logs" subtitulo="Resultado paginado do processamento de e-mails.">
        <TabelaLogsEmail
          :rows="lista.items"
          :total="lista.total"
          :pagina="lista.pagina"
          :tamanho-pagina="lista.tamanhoPagina"
          :loading="loading"
          @alterar-pagina="alterarPagina"
          @ver-detalhe="abrirDetalhe"
          @abrir-chamado="abrirChamado"
        />
      </AppSectionCard>

      <DetalheLogEmail
        v-model="modalDetalheAberto"
        :detalhe="detalheSelecionado"
        :loading="loadingDetalhe"
        @abrir-chamado="abrirChamado"
      />
    </template>
  </q-page>
</template>
