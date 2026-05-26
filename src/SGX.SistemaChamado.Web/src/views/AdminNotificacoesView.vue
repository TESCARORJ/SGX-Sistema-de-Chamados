<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { type QTableColumn, useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { useAuthStore } from '../stores/authStore'
import { useNotificacoesStore } from '../stores/notificacoesStore'
import type { Notificacao, TipoNotificacao } from '../types/notificacao'

type FiltroLeitura = 'Todas' | 'NÃ£o lidas' | 'Lidas'
type FiltroPeriodo = 'Hoje' | 'Ãšltimos 7 dias' | 'Ãšltimos 30 dias'

const $q = useQuasar()
const router = useRouter()
const authStore = useAuthStore()
const notificacoesStore = useNotificacoesStore()

const filtros = reactive({
  texto: '',
  tipo: 'Todos',
  leitura: 'Todas' as FiltroLeitura,
  periodo: 'Ãšltimos 30 dias' as FiltroPeriodo,
})

const dialogDetalheAberto = ref(false)
const notificacaoDetalheId = ref<string | null>(null)
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeVisualizarNotificacoes = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.notificacoesVisualizar)
)
const podeGerenciarNotificacoes = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.notificacoesGerenciar)
)

const opcoesTipo = [
  { label: 'Todos', value: 'Todos' },
  { label: 'Info', value: 'info' },
  { label: 'Warning', value: 'warning' },
  { label: 'Negative', value: 'negative' },
  { label: 'Positive', value: 'positive' },
  { label: 'Message', value: 'message' },
  { label: 'Email', value: 'email' },
  { label: 'SLA', value: 'sla' },
  { label: 'Assignment', value: 'assignment' },
]

const opcoesLeitura = [
  { label: 'Todas', value: 'Todas' },
  { label: 'NÃ£o lidas', value: 'NÃ£o lidas' },
  { label: 'Lidas', value: 'Lidas' },
]

const opcoesPeriodo = [
  { label: 'Hoje', value: 'Hoje' },
  { label: 'Ãšltimos 7 dias', value: 'Ãšltimos 7 dias' },
  { label: 'Ãšltimos 30 dias', value: 'Ãšltimos 30 dias' },
]

const colunas: QTableColumn<Notificacao>[] = [
  { name: 'status', label: 'Status', field: 'lida', align: 'left', sortable: true },
  { name: 'tipo', label: 'Tipo', field: 'tipo', align: 'left', sortable: true },
  { name: 'titulo', label: 'TÃ­tulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'descricao', label: 'DescriÃ§Ã£o', field: 'descricao', align: 'left', sortable: false },
  { name: 'chamado', label: 'Chamado', field: 'chamadoCodigo', align: 'left', sortable: true },
  { name: 'dataHora', label: 'Data/Hora', field: 'dataHora', align: 'left', sortable: true },
  { name: 'acoes', label: 'AÃ§Ãµes', field: 'id', align: 'right' },
]

const corPorTipo: Record<TipoNotificacao, string> = {
  info: 'primary',
  warning: 'warning',
  negative: 'negative',
  positive: 'positive',
  message: 'teal',
  email: 'indigo',
  sla: 'warning',
  assignment: 'deep-purple',
}

const iconePorTipo: Record<TipoNotificacao, string> = {
  info: 'confirmation_number',
  warning: 'schedule',
  negative: 'report_problem',
  positive: 'check_circle',
  message: 'chat_bubble',
  email: 'mail',
  sla: 'alarm',
  assignment: 'assignment_ind',
}

const notificacaoDetalhe = computed(() => {
  if (!notificacaoDetalheId.value) {
    return null
  }

  return notificacoesStore.obterPorId(notificacaoDetalheId.value) ?? null
})

const notificacoesFiltradas = computed(() => {
  const termo = filtros.texto.trim().toLowerCase()
  const agora = new Date()
  const inicioHoje = new Date(agora)
  inicioHoje.setHours(0, 0, 0, 0)

  const limite7Dias = new Date(agora)
  limite7Dias.setDate(limite7Dias.getDate() - 7)

  const limite30Dias = new Date(agora)
  limite30Dias.setDate(limite30Dias.getDate() - 30)

  return notificacoesStore.notificacoesOrdenadas.filter((notificacao) => {
    if (termo) {
      const alvo = [
        notificacao.titulo,
        notificacao.descricao,
        notificacao.chamadoCodigo ?? '',
        notificacao.remetente ?? '',
        notificacao.detalheTecnico ?? '',
      ]
        .join(' ')
        .toLowerCase()

      if (!alvo.includes(termo)) {
        return false
      }
    }

    if (filtros.tipo !== 'Todos' && notificacao.tipo !== filtros.tipo) {
      return false
    }

    if (filtros.leitura === 'NÃ£o lidas' && notificacao.lida) {
      return false
    }

    if (filtros.leitura === 'Lidas' && !notificacao.lida) {
      return false
    }

    const dataNotificacao = new Date(notificacao.dataHora)
    if (filtros.periodo === 'Hoje' && dataNotificacao < inicioHoje) {
      return false
    }

    if (filtros.periodo === 'Ãšltimos 7 dias' && dataNotificacao < limite7Dias) {
      return false
    }

    if (filtros.periodo === 'Ãšltimos 30 dias' && dataNotificacao < limite30Dias) {
      return false
    }

    return true
  })
})

const tituloCardLista = computed(() => `Registros encontrados: ${notificacoesFiltradas.value.length}`)
const possuiDados = computed(() => notificacoesStore.notificacoes.length > 0)

function obterCor(tipo: TipoNotificacao): string {
  return corPorTipo[tipo] ?? 'primary'
}

function obterIcone(tipo: TipoNotificacao): string {
  return iconePorTipo[tipo] ?? 'notifications'
}

function rotuloTipo(tipo: TipoNotificacao): string {
  if (tipo === 'assignment') return 'Assignment'
  if (tipo === 'message') return 'Message'
  if (tipo === 'negative') return 'Negative'
  if (tipo === 'positive') return 'Positive'
  if (tipo === 'warning') return 'Warning'
  if (tipo === 'email') return 'Email'
  if (tipo === 'sla') return 'SLA'
  return 'Info'
}

function formatarDataHora(data: string): string {
  return new Date(data).toLocaleString('pt-BR')
}

function limparFiltros(): void {
  filtros.texto = ''
  filtros.tipo = 'Todos'
  filtros.leitura = 'Todas'
  filtros.periodo = 'Ãšltimos 30 dias'
}

async function atualizar(): Promise<void> {
  await notificacoesStore.atualizar()
  $q.notify({
    type: 'info',
    message: 'NotificaÃ§Ãµes atualizadas.',
  })
}

function abrirDetalhe(notificacao: Notificacao): void {
  notificacaoDetalheId.value = notificacao.id
  dialogDetalheAberto.value = true
}

function marcarComoLida(id: string): void {
  if (!podeGerenciarNotificacoes.value) {
    return
  }

  const notificacao = notificacoesStore.obterPorId(id)
  if (!notificacao || notificacao.lida) {
    return
  }

  notificacoesStore.marcarComoLida(id)
  $q.notify({
    type: 'positive',
    message: 'NotificaÃ§Ã£o marcada como lida.',
  })
}

function marcarTodasComoLidas(): void {
  if (!podeGerenciarNotificacoes.value) {
    $q.notify({
      type: 'warning',
      message: 'VocÃª nÃ£o possui permissÃ£o para gerenciar notificaÃ§Ãµes.',
    })
    return
  }

  if (notificacoesStore.totalNaoLidas === 0) {
    $q.notify({
      type: 'info',
      message: 'Todas as notificaÃ§Ãµes jÃ¡ estÃ£o lidas.',
    })
    return
  }

  notificacoesStore.marcarTodasComoLidas()
  $q.notify({
    type: 'positive',
    message: 'Todas as notificaÃ§Ãµes foram marcadas como lidas.',
  })
}

async function abrirChamado(notificacao: Notificacao): Promise<void> {
  if (!notificacao.chamadoId) {
    $q.notify({
      type: 'info',
      message: 'Chamado associado ainda nÃ£o disponÃ­vel nesta demonstraÃ§Ã£o.',
    })
    return
  }

  await router.push(`/admin/chamados/${notificacao.chamadoId}`)
}

async function carregarNotificacoes(): Promise<void> {
  if (!podeVisualizarNotificacoes.value) {
    return
  }

  await notificacoesStore.carregarNotificacoes()
}

onMounted(async () => {
  await carregarNotificacoes()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Central de NotificaÃ§Ãµes"
      subtitulo="Acompanhe alertas, eventos de SLA, atribuiÃ§Ãµes e atualizaÃ§Ãµes de chamados."
    >
      <template #actions>
        <div class="row q-col-gutter-sm notifications-header-actions">
          <div v-if="podeGerenciarNotificacoes" class="col-auto">
            <q-btn color="primary" icon="done_all" label="Marcar todas como lidas" @click="marcarTodasComoLidas" />
          </div>
          <div class="col-auto">
            <q-btn color="secondary" icon="refresh" label="Atualizar" :loading="notificacoesStore.loading" @click="atualizar" />
          </div>
          <div class="col-auto">
            <q-btn flat color="primary" icon="filter_alt_off" label="Limpar filtros" @click="limparFiltros" />
          </div>
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizarNotificacoes" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar a central de notificações.
    </q-banner>

    <AppSectionCard v-if="podeVisualizarNotificacoes" titulo="Filtros" subtitulo="Refine por texto, tipo, leitura e perÃ­odo.">
      <div class="row q-col-gutter-sm">
        <div class="col-12 col-md-4">
          <q-input v-model="filtros.texto" outlined dense label="Texto" placeholder="TÃ­tulo, descriÃ§Ã£o, chamado ou remetente" />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.tipo"
            outlined
            dense
            emit-value
            map-options
            label="Tipo"
            :options="opcoesTipo"
          />
        </div>

        <div class="col-12 col-md-2">
          <q-select
            v-model="filtros.leitura"
            outlined
            dense
            emit-value
            map-options
            label="Leitura"
            :options="opcoesLeitura"
          />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.periodo"
            outlined
            dense
            emit-value
            map-options
            label="PerÃ­odo"
            :options="opcoesPeriodo"
          />
        </div>
      </div>
    </AppSectionCard>

    <ErrorState
      v-if="podeVisualizarNotificacoes && notificacoesStore.erro"
      :mensagem="notificacoesStore.erro"
      @retry="carregarNotificacoes"
    />

    <LoadingState
      v-else-if="podeVisualizarNotificacoes && notificacoesStore.loading && !notificacoesStore.notificacoes.length"
      inline
      mensagem="Carregando central de notificaÃ§Ãµes..."
    />

    <AppSectionCard v-else-if="podeVisualizarNotificacoes" titulo="NotificaÃ§Ãµes" :subtitulo="tituloCardLista">
      <template #actions>
        <div class="row q-gutter-sm">
          <q-chip color="blue-1" text-color="primary" icon="notifications" square>
            Total: {{ notificacoesStore.notificacoes.length }}
          </q-chip>
          <q-chip color="orange-1" text-color="warning" icon="mark_email_unread" square>
            NÃ£o lidas: {{ notificacoesStore.totalNaoLidas }}
          </q-chip>
        </div>
      </template>

      <EmptyState
        v-if="!possuiDados"
        titulo="Nenhuma notificaÃ§Ã£o encontrada."
        mensagem="Nenhuma notificaÃ§Ã£o encontrada."
      />

      <EmptyState
        v-else-if="!notificacoesFiltradas.length"
        titulo="Sem resultados para os filtros"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
      />

      <q-table
        v-else
        class="sgx-table"
        flat
        bordered
        :rows="notificacoesFiltradas"
        :columns="colunas"
        row-key="id"
        :grid="$q.screen.lt.md"
        :pagination="{ rowsPerPage: 10 }"
        :rows-per-page-options="[10, 20, 50]"
        separator="horizontal"
      >
        <template #body-cell-status="slotProps">
          <q-td :props="slotProps">
            <q-badge
              rounded
              :color="slotProps.row.lida ? 'grey-6' : 'primary'"
              :label="slotProps.row.lida ? 'Lida' : 'NÃ£o lida'"
            />
          </q-td>
        </template>

        <template #body-cell-tipo="slotProps">
          <q-td :props="slotProps">
            <q-chip dense square :color="obterCor(slotProps.row.tipo)" text-color="white" :icon="obterIcone(slotProps.row.tipo)">
              {{ rotuloTipo(slotProps.row.tipo) }}
            </q-chip>
          </q-td>
        </template>

        <template #body-cell-chamado="slotProps">
          <q-td :props="slotProps">
            {{ slotProps.row.chamadoCodigo || '-' }}
          </q-td>
        </template>

        <template #body-cell-dataHora="slotProps">
          <q-td :props="slotProps">
            <div>{{ formatarDataHora(slotProps.row.dataHora) }}</div>
            <div class="text-caption text-grey-7">{{ slotProps.row.tempoRelativo }}</div>
          </q-td>
        </template>

        <template #body-cell-acoes="slotProps">
          <q-td :props="slotProps" class="text-right">
            <div class="row justify-end q-gutter-xs">
              <q-btn
                flat
                dense
                color="primary"
                icon="visibility"
                label="Ver detalhe"
                @click="abrirDetalhe(slotProps.row)"
              />
              <q-btn
                v-if="!slotProps.row.lida"
                flat
                dense
                color="secondary"
                icon="done"
                label="Marcar como lida"
                :disable="!podeGerenciarNotificacoes"
                @click="marcarComoLida(slotProps.row.id)"
              />
              <q-btn
                flat
                dense
                color="primary"
                icon="open_in_new"
                label="Abrir chamado"
                @click="abrirChamado(slotProps.row)"
              />
            </div>
          </q-td>
        </template>

        <template #item="slotProps">
          <div class="col-12 q-mb-sm">
            <q-card flat bordered class="sgx-card">
              <q-card-section class="row items-start justify-between q-col-gutter-sm">
                <div class="col">
                  <div class="row items-center q-gutter-sm">
                    <q-badge
                      rounded
                      :color="slotProps.row.lida ? 'grey-6' : 'primary'"
                      :label="slotProps.row.lida ? 'Lida' : 'NÃ£o lida'"
                    />
                    <q-chip dense square :color="obterCor(slotProps.row.tipo)" text-color="white" :icon="obterIcone(slotProps.row.tipo)">
                      {{ rotuloTipo(slotProps.row.tipo) }}
                    </q-chip>
                  </div>
                  <div class="text-subtitle2 text-weight-bold q-mt-sm">{{ slotProps.row.titulo }}</div>
                  <div class="text-body2 text-grey-8">{{ slotProps.row.descricao }}</div>
                  <div class="text-caption text-grey-7 q-mt-xs">
                    Chamado: {{ slotProps.row.chamadoCodigo || '-' }}
                  </div>
                  <div class="text-caption text-grey-7">
                    {{ formatarDataHora(slotProps.row.dataHora) }} ({{ slotProps.row.tempoRelativo }})
                  </div>
                </div>
              </q-card-section>

              <q-separator />

              <q-card-actions align="between" class="q-pa-sm">
                <q-btn flat dense color="primary" icon="visibility" label="Ver detalhe" @click="abrirDetalhe(slotProps.row)" />
                <div class="row q-gutter-xs">
                  <q-btn
                    v-if="!slotProps.row.lida"
                    flat
                    dense
                    color="secondary"
                    icon="done"
                    label="Marcar lida"
                    :disable="!podeGerenciarNotificacoes"
                    @click="marcarComoLida(slotProps.row.id)"
                  />
                  <q-btn
                    flat
                    dense
                    color="primary"
                    icon="open_in_new"
                    label="Abrir chamado"
                    @click="abrirChamado(slotProps.row)"
                  />
                </div>
              </q-card-actions>
            </q-card>
          </div>
        </template>
      </q-table>
    </AppSectionCard>

    <q-dialog v-model="dialogDetalheAberto" :maximized="$q.screen.lt.md">
      <q-card class="sgx-card dialog-detalhe">
        <q-card-section class="row items-start justify-between q-col-gutter-sm">
          <div class="col">
            <div class="text-h6 text-weight-bold">{{ notificacaoDetalhe?.titulo || 'Detalhe da notificaÃ§Ã£o' }}</div>
            <div class="text-caption text-grey-7">
              {{ notificacaoDetalhe ? formatarDataHora(notificacaoDetalhe.dataHora) : '-' }}
            </div>
          </div>
          <div class="col-auto">
            <q-btn flat round dense icon="close" aria-label="Fechar detalhe da notificação" @click="dialogDetalheAberto = false" />
          </div>
        </q-card-section>

        <q-separator />

        <q-card-section v-if="notificacaoDetalhe" class="column q-gutter-md">
          <div class="row q-gutter-sm">
            <q-badge
              rounded
              :color="notificacaoDetalhe.lida ? 'grey-6' : 'primary'"
              :label="notificacaoDetalhe.lida ? 'Lida' : 'NÃ£o lida'"
            />
            <q-chip dense square :color="obterCor(notificacaoDetalhe.tipo)" text-color="white" :icon="obterIcone(notificacaoDetalhe.tipo)">
              {{ rotuloTipo(notificacaoDetalhe.tipo) }}
            </q-chip>
          </div>

          <div>
            <div class="text-body1 text-weight-medium">{{ notificacaoDetalhe.descricao }}</div>
            <div class="text-caption text-grey-7 q-mt-xs">{{ notificacaoDetalhe.tempoRelativo }}</div>
          </div>

          <div class="text-body2">
            <strong>Chamado associado:</strong>
            {{ notificacaoDetalhe.chamadoCodigo || 'NÃ£o informado' }}
          </div>

          <div v-if="notificacaoDetalhe.remetente" class="text-body2">
            <strong>Remetente:</strong>
            {{ notificacaoDetalhe.remetente }}
          </div>

          <q-expansion-item
            v-if="notificacaoDetalhe.detalheTecnico"
            icon="terminal"
            label="Detalhe tÃ©cnico"
            header-class="text-negative"
          >
            <div class="q-pa-sm text-body2">
              {{ notificacaoDetalhe.detalheTecnico }}
            </div>
          </q-expansion-item>
        </q-card-section>

        <q-separator />

        <q-card-actions align="right" class="q-pa-md q-gutter-sm">
          <q-btn
            v-if="notificacaoDetalhe && !notificacaoDetalhe.lida"
            color="secondary"
            icon="done"
            label="Marcar como lida"
            :disable="!podeGerenciarNotificacoes"
            @click="notificacaoDetalhe && marcarComoLida(notificacaoDetalhe.id)"
          />
          <q-btn
            v-if="notificacaoDetalhe"
            flat
            color="primary"
            icon="open_in_new"
            label="Abrir chamado"
            @click="notificacaoDetalhe && abrirChamado(notificacaoDetalhe)"
          />
          <q-btn flat color="grey-8" icon="close" label="Fechar" @click="dialogDetalheAberto = false" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
.notifications-header-actions {
  align-items: center;
}

.dialog-detalhe {
  width: min(760px, 96vw);
  max-width: 96vw;
}

:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
}

:deep(.sgx-table tbody tr:hover) {
  background: rgba(11, 94, 215, 0.04);
}

@media (max-width: 768px) {
  .notifications-header-actions {
    width: 100%;
  }

  .notifications-header-actions .col-auto {
    width: 100%;
  }

  .notifications-header-actions :deep(.q-btn) {
    width: 100%;
  }
}
</style>


