<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import { useNotificacoesStore } from '../../stores/notificacoesStore'
import type { Notificacao, TipoNotificacao } from '../../types/notificacao'

const $q = useQuasar()
const router = useRouter()
const notificacoesStore = useNotificacoesStore()

const menuAberto = ref(false)
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

const notificacoesRecentes = computed(() => notificacoesStore.notificacoesOrdenadas.slice(0, 6))
const totalNaoLidas = computed(() => notificacoesStore.totalNaoLidas)

function obterCor(tipo: TipoNotificacao): string {
  return corPorTipo[tipo] ?? 'primary'
}

function obterIcone(tipo: TipoNotificacao): string {
  return iconePorTipo[tipo] ?? 'notifications'
}

async function selecionarNotificacao(notificacao: Notificacao): Promise<void> {
  notificacoesStore.marcarComoLida(notificacao.id)

  if (notificacao.chamadoId) {
    menuAberto.value = false
    await router.push(`/admin/chamados/${notificacao.chamadoId}`)
    return
  }

  $q.notify({
    type: 'info',
    message: 'Notificação selecionada.',
  })
}

function marcarTodasComoLidas(): void {
  notificacoesStore.marcarTodasComoLidas()

  $q.notify({
    type: 'positive',
    message: 'Todas as notificações foram marcadas como lidas.',
  })
}

async function verTodas(): Promise<void> {
  menuAberto.value = false
  await router.push('/admin/notificacoes')
}

onMounted(async () => {
  await notificacoesStore.carregarNotificacoes()
})
</script>

<template>
  <q-btn flat round dense icon="notifications" class="q-ml-sm" aria-label="Abrir notificações">
    <q-badge v-if="totalNaoLidas > 0" color="negative" rounded floating :label="totalNaoLidas" />
    <q-tooltip>Notificações</q-tooltip>

    <q-menu
      v-model="menuAberto"
      anchor="bottom right"
      self="top right"
      :offset="[0, 10]"
      transition-show="jump-down"
      transition-hide="jump-up"
    >
      <q-card class="notifications-menu">
        <div class="q-pa-md">
          <div class="text-subtitle1 text-weight-bold">Notificações</div>
          <div class="text-caption text-grey-7">Atualizações recentes dos chamados</div>
        </div>

        <q-separator />

        <q-list class="notifications-menu__list">
          <q-item v-if="notificacoesStore.loading" dense>
            <q-item-section class="items-center">
              <q-spinner color="primary" size="24px" />
            </q-item-section>
          </q-item>

          <q-item
            v-for="notificacao in notificacoesRecentes"
            :key="notificacao.id"
            clickable
            class="notifications-menu__item"
            :class="{ 'notifications-menu__item--nao-lida': !notificacao.lida }"
            @click="selecionarNotificacao(notificacao)"
          >
            <q-item-section avatar>
              <q-icon :name="obterIcone(notificacao.tipo)" :color="obterCor(notificacao.tipo)" />
            </q-item-section>

            <q-item-section>
              <div class="row items-start no-wrap justify-between q-gutter-sm">
                <div class="column">
                  <span class="text-body2 text-weight-medium">{{ notificacao.titulo }}</span>
                  <span class="text-caption text-grey-8">{{ notificacao.descricao }}</span>
                </div>
                <q-badge v-if="!notificacao.lida" color="primary" rounded />
              </div>
              <span class="text-caption text-grey-6 q-mt-xs">{{ notificacao.tempoRelativo }}</span>
            </q-item-section>
          </q-item>

          <q-item v-if="!notificacoesStore.loading && !notificacoesRecentes.length">
            <q-item-section class="text-caption text-grey-7">
              Nenhuma notificação encontrada.
            </q-item-section>
          </q-item>
        </q-list>

        <q-separator />

        <div class="row items-center justify-between q-pa-sm q-gutter-sm">
          <q-btn flat no-caps color="primary" icon="done_all" label="Marcar todas como lidas" @click="marcarTodasComoLidas" />
          <q-btn flat no-caps color="secondary" icon="open_in_new" label="Ver todas" @click="verTodas" />
        </div>
      </q-card>
    </q-menu>
  </q-btn>
</template>

<style scoped>
.notifications-menu {
  width: min(360px, calc(100vw - 24px));
  max-width: calc(100vw - 24px);
}

.notifications-menu__list {
  max-height: 420px;
  overflow-y: auto;
}

.notifications-menu__item {
  align-items: flex-start;
}

.notifications-menu__item--nao-lida {
  background: rgba(11, 94, 215, 0.06);
}
</style>
