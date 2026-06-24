<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { HttpRequestError } from '../../services/httpClient'
import { notificacoesService } from '../../services/notificacoesService'
import { useNotificacoesStore } from '../../stores/notificacoesStore'
import type {
  FiltroLeituraNotificacao,
  ListarMinhasNotificacoesResponse,
  MinhaNotificacaoDetalheResponse,
  MinhaNotificacaoResumoResponse,
} from '../../types/notificacoes'
import AppSectionCard from '../../components/ui/AppSectionCard.vue'
import EmptyState from '../../components/ui/EmptyState.vue'
import ErrorState from '../../components/ui/ErrorState.vue'
import LoadingState from '../../components/ui/LoadingState.vue'
import PageHeader from '../../components/ui/PageHeader.vue'
import NotificacaoCard from '../../components/notificacoes/NotificacaoCard.vue'
import NotificacaoDetalheDialog from '../../components/notificacoes/NotificacaoDetalheDialog.vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const $q = useQuasar()
const notificacoesStore = useNotificacoesStore()

const filtroLeitura = ref<FiltroLeituraNotificacao>('todas')
const paginaAtual = ref(1)
const tamanhoPagina = ref(10)
const carregandoLista = ref(false)
const erroLista = ref<string | null>(null)
const notificacoes = ref<MinhaNotificacaoResumoResponse[]>([])
const total = ref(0)
const totalPaginas = ref(0)
const totalNaoLidas = ref(0)
const idsEmMutacao = ref<string[]>([])

const detalheAberto = ref(false)
const detalheIdSelecionado = ref<string | null>(null)
const detalheCarregando = ref(false)
const detalheErro = ref<string | null>(null)
const detalhe = ref<MinhaNotificacaoDetalheResponse | null>(null)

const opcoesFiltro = [
  { label: 'Todas', value: 'todas' },
  { label: 'Não lidas', value: 'nao-lidas' },
  { label: 'Lidas', value: 'lidas' },
] as const

const opcoesTamanhoPagina = [
  { label: '10 por página', value: 10 },
  { label: '20 por página', value: 20 },
  { label: '50 por página', value: 50 },
] as const

const tituloEstadoVazio = computed(() => {
  if (filtroLeitura.value === 'nao-lidas') {
    return 'Nenhuma notificação não lida.'
  }

  if (filtroLeitura.value === 'lidas') {
    return 'Nenhuma notificação lida.'
  }

  return 'Você ainda não possui notificações.'
})

const mensagemEstadoVazio = computed(() => {
  if (filtroLeitura.value === 'nao-lidas') {
    return 'As notificações já lidas deixam de aparecer neste filtro.'
  }

  if (filtroLeitura.value === 'lidas') {
    return 'Quando você marcar notificações como lidas, elas aparecerão aqui.'
  }

  return 'Quando houver notificações do canal Sistema para sua conta, elas serão exibidas aqui.'
})

const contextoPagina = computed(() => (route.path.startsWith('/admin') ? 'Área administrativa' : 'Portal do solicitante'))
const possuiNotificacoes = computed(() => notificacoes.value.length > 0)
const podeVoltarPagina = computed(() => paginaAtual.value > 1)
const podeAvancarPagina = computed(() => totalPaginas.value > 0 && paginaAtual.value < totalPaginas.value)

function obterParametroLida(): boolean | undefined {
  if (filtroLeitura.value === 'nao-lidas') {
    return false
  }

  if (filtroLeitura.value === 'lidas') {
    return true
  }

  return undefined
}

function tipoEventoTexto(tipoEvento: number | string): string {
  if (tipoEvento === 1 || tipoEvento === 'EventoChamado') {
    return 'Evento de chamado'
  }

  if (tipoEvento === 2 || tipoEvento === 'EventoAprovacao') {
    return 'Evento de aprovação'
  }

  if (tipoEvento === 3 || tipoEvento === 'EventoSla') {
    return 'Evento de SLA'
  }

  if (tipoEvento === 4 || tipoEvento === 'EventoAdministrativo') {
    return 'Evento administrativo'
  }

  return 'Notificação do sistema'
}

function formatarDataHora(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(data))
}

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (error instanceof HttpRequestError) {
    if (error.status === 404) {
      return 'A notificação solicitada não foi encontrada.'
    }

    if (error.status === 401) {
      return 'Sua sessão expirou. Entre novamente para continuar.'
    }

    if (error.status === 403) {
      return 'Você não possui acesso à central de notificações.'
    }
  }

  if (error instanceof Error) {
    const mensagem = error.message?.trim()
    if (mensagem) {
      return mensagem
    }
  }

  return fallback
}

function rotaChamado(chamadoId: string): string {
  return route.path.startsWith('/admin')
    ? `/admin/chamados/${chamadoId}`
    : `/portal/chamados/${chamadoId}`
}

function idEstaEmMutacao(id: string): boolean {
  return idsEmMutacao.value.includes(id)
}

function adicionarMutacao(id: string): void {
  if (idsEmMutacao.value.includes(id)) {
    return
  }

  idsEmMutacao.value = [...idsEmMutacao.value, id]
}

function removerMutacao(id: string): void {
  idsEmMutacao.value = idsEmMutacao.value.filter((itemId) => itemId !== id)
}

function atualizarTotalNaoLidasLocal(novoTotal: number): void {
  totalNaoLidas.value = Math.max(0, novoTotal)
  notificacoesStore.sincronizarTotal(totalNaoLidas.value)
}

function atualizarResumoLocal(id: string, lida: boolean, lidaEm: string | null): void {
  notificacoes.value = notificacoes.value
    .map((item) => {
      if (item.id !== id) {
        return item
      }

      return {
        ...item,
        lida,
        lidaEm,
      }
    })
    .filter((item) => {
      if (filtroLeitura.value === 'nao-lidas') {
        return !item.lida
      }

      if (filtroLeitura.value === 'lidas') {
        return item.lida
      }

      return true
    })

  if (detalhe.value?.id === id) {
    detalhe.value = {
      ...detalhe.value,
      lida,
      lidaEm,
    }
  }
}

async function carregarLista(): Promise<void> {
  carregandoLista.value = true
  erroLista.value = null

  try {
    const response = await notificacoesService.listarMinhasNotificacoes({
      pagina: paginaAtual.value,
      tamanhoPagina: tamanhoPagina.value,
      lida: obterParametroLida(),
    })

    aplicarListagem(response)
  } catch (error) {
    erroLista.value = extrairMensagemErro(error, 'Não foi possível carregar suas notificações.')
    notificacoes.value = []
    total.value = 0
    totalPaginas.value = 0
  } finally {
    carregandoLista.value = false
  }
}

function aplicarListagem(response: ListarMinhasNotificacoesResponse): void {
  notificacoes.value = response.itens
  total.value = response.total
  totalPaginas.value = response.totalPaginas
  paginaAtual.value = response.pagina
  tamanhoPagina.value = response.tamanhoPagina
  atualizarTotalNaoLidasLocal(response.totalNaoLidas)
}

async function atualizarLista(): Promise<void> {
  await carregarLista()

  try {
    await notificacoesStore.carregarContagem(true)
  } catch {
    // A listagem já foi carregada; a contagem global é sincronizada novamente na próxima navegação.
  }
}

async function abrirDetalhe(id: string): Promise<void> {
  detalheIdSelecionado.value = id
  detalheAberto.value = true
  detalheCarregando.value = true
  detalheErro.value = null
  detalhe.value = null

  try {
    detalhe.value = await notificacoesService.obterMinhaNotificacao(id)
  } catch (error) {
    detalheErro.value = extrairMensagemErro(error, 'Não foi possível carregar o detalhe da notificação.')
  } finally {
    detalheCarregando.value = false
  }
}

async function alterarLeitura(
  id: string,
  operacao: 'lida' | 'nao-lida'
): Promise<void> {
  if (idEstaEmMutacao(id)) {
    return
  }

  adicionarMutacao(id)

  try {
    const response = operacao === 'lida'
      ? await notificacoesService.marcarMinhaNotificacaoComoLida(id)
      : await notificacoesService.marcarMinhaNotificacaoComoNaoLida(id)

    atualizarResumoLocal(id, response.lida, response.lidaEm)

    if (response.estadoAlterado) {
      const delta = response.lida ? -1 : 1
      atualizarTotalNaoLidasLocal(totalNaoLidas.value + delta)

      if (filtroLeitura.value !== 'todas') {
        total.value = Math.max(0, total.value - 1)
      }

      if (total.value === 0) {
        totalPaginas.value = 0
      } else {
        totalPaginas.value = Math.ceil(total.value / tamanhoPagina.value)
      }

      if (!notificacoes.value.length && paginaAtual.value > 1) {
        paginaAtual.value -= 1
        await carregarLista()
      }

      if (detalheIdSelecionado.value === id && detalhe.value) {
        detalhe.value = {
          ...detalhe.value,
          lida: response.lida,
          lidaEm: response.lidaEm,
        }
      }
    }

    try {
      await notificacoesStore.carregarContagem(true)
      totalNaoLidas.value = notificacoesStore.totalNaoLidas
    } catch {
      // Mantem o estado local ajustado se a contagem global falhar.
    }
  } catch (error) {
    const mensagem = extrairMensagemErro(
      error,
      operacao === 'lida'
        ? 'Não foi possível marcar a notificação como lida.'
        : 'Não foi possível marcar a notificação como não lida.'
    )

    $q.notify({
      type: 'negative',
      message: mensagem,
      position: 'top',
    })
  } finally {
    removerMutacao(id)
  }
}

async function abrirChamado(chamadoId: string): Promise<void> {
  await router.push(rotaChamado(chamadoId))
}

async function irParaPagina(pagina: number): Promise<void> {
  if (pagina === paginaAtual.value || pagina < 1 || (totalPaginas.value > 0 && pagina > totalPaginas.value)) {
    return
  }

  paginaAtual.value = pagina
  await carregarLista()
}

watch(filtroLeitura, async () => {
  paginaAtual.value = 1
  await carregarLista()
})

watch(tamanhoPagina, async () => {
  paginaAtual.value = 1
  await carregarLista()
})

onMounted(async () => {
  await carregarLista()

  try {
    await notificacoesStore.carregarContagem(true)
  } catch {
    // O badge do layout continua funcional mesmo se a primeira carga falhar.
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :contexto="contextoPagina"
      titulo="Notificações"
      subtitulo="Consulte sua caixa interna, acompanhe o que ainda não foi lido e controle o estado de leitura sem efeitos colaterais em GET."
    >
      <template #actions>
        <div class="row items-center q-col-gutter-sm notificacoes-header-actions">
          <div class="col-auto">
            <q-chip color="orange-1" text-color="warning" icon="mark_email_unread" square>
              Não lidas: {{ totalNaoLidas }}
            </q-chip>
          </div>
          <div class="col-auto">
            <q-btn color="secondary" icon="refresh" label="Atualizar" :loading="carregandoLista" @click="atualizarLista" />
          </div>
        </div>
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros" subtitulo="Alterne entre todas, não lidas e lidas. A paginação sempre usa a resposta da API.">
      <div class="row q-col-gutter-md items-center">
        <div class="col-12 col-md">
          <q-btn-toggle
            v-model="filtroLeitura"
            spread
            unelevated
            toggle-color="primary"
            color="white"
            text-color="primary"
            :options="opcoesFiltro"
            aria-label="Filtro de leitura das notificações"
          />
        </div>

        <div class="col-12 col-md-auto">
          <q-select
            v-model="tamanhoPagina"
            outlined
            dense
            emit-value
            map-options
            label="Tamanho da página"
            :options="opcoesTamanhoPagina"
          />
        </div>
      </div>
    </AppSectionCard>

    <ErrorState v-if="erroLista" :mensagem="erroLista" @retry="carregarLista" />

    <LoadingState
      v-else-if="carregandoLista && !possuiNotificacoes"
      inline
      mensagem="Carregando suas notificações..."
    />

    <AppSectionCard
      v-else
      titulo="Central de notificações"
      :subtitulo="`Página ${paginaAtual} de ${totalPaginas || 1} • total listado: ${total}`"
    >
      <EmptyState
        v-if="!possuiNotificacoes"
        :titulo="tituloEstadoVazio"
        :mensagem="mensagemEstadoVazio"
        icon="notifications_off"
      />

      <div v-else class="column q-gutter-md">
        <NotificacaoCard
          v-for="item in notificacoes"
          :key="item.id"
          :notificacao="item"
          :tipo-evento-texto="tipoEventoTexto(item.tipoEvento)"
          :enviada-em-texto="formatarDataHora(item.enviadaEm)"
          :acao-carregando="idEstaEmMutacao(item.id)"
          :pode-abrir-chamado="Boolean(item.chamadoId)"
          @abrir-detalhe="abrirDetalhe"
          @marcar-como-lida="alterarLeitura($event, 'lida')"
          @marcar-como-nao-lida="alterarLeitura($event, 'nao-lida')"
          @abrir-chamado="abrirChamado"
        />

        <div class="row items-center justify-between q-col-gutter-md q-pt-sm notificacoes-paginacao">
          <div class="col-12 col-md-auto text-caption text-grey-7">
            Mostrando {{ notificacoes.length }} item(ns) nesta página.
          </div>

          <div class="col-12 col-md-auto row items-center q-gutter-sm">
            <q-btn flat color="primary" icon="chevron_left" label="Anterior" :disable="!podeVoltarPagina" @click="irParaPagina(paginaAtual - 1)" />

            <q-pagination
              v-model="paginaAtual"
              color="primary"
              :max="Math.max(totalPaginas, 1)"
              :max-pages="6"
              boundary-numbers
              direction-links
              @update:model-value="irParaPagina"
            />

            <q-btn flat color="primary" icon-right="chevron_right" label="Próxima" :disable="!podeAvancarPagina" @click="irParaPagina(paginaAtual + 1)" />
          </div>
        </div>
      </div>
    </AppSectionCard>

    <NotificacaoDetalheDialog
      v-model="detalheAberto"
      :detalhe="detalhe"
      :carregando="detalheCarregando"
      :erro="detalheErro"
      :acao-carregando="Boolean(detalhe?.id && idEstaEmMutacao(detalhe.id))"
      :tipo-evento-texto="detalhe ? tipoEventoTexto(detalhe.tipoEvento) : 'Notificação do sistema'"
      :enviada-em-texto="formatarDataHora(detalhe?.enviadaEm ?? null)"
      :lida-em-texto="detalhe?.lidaEm ? formatarDataHora(detalhe.lidaEm) : null"
      @retry="detalheIdSelecionado && abrirDetalhe(detalheIdSelecionado)"
      @marcar-como-lida="alterarLeitura($event, 'lida')"
      @marcar-como-nao-lida="alterarLeitura($event, 'nao-lida')"
      @abrir-chamado="abrirChamado"
    />
  </q-page>
</template>

<style scoped>
.notificacoes-header-actions {
  align-items: center;
}

.notificacoes-paginacao {
  border-top: 1px solid var(--sgx-border-soft);
}

@media (max-width: 768px) {
  .notificacoes-header-actions,
  .notificacoes-paginacao {
    width: 100%;
  }

  .notificacoes-header-actions .col-auto,
  .notificacoes-paginacao .col-12,
  .notificacoes-paginacao .col-md-auto {
    width: 100%;
  }
}
</style>
