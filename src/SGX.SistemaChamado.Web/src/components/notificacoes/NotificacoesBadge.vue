<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import { useNotificacoesStore } from '../../stores/notificacoesStore'

const props = withDefaults(
  defineProps<{
    compact?: boolean
  }>(),
  {
    compact: false,
  }
)

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const notificacoesStore = useNotificacoesStore()

const rotaDestino = computed(() =>
  route.path.startsWith('/admin') ? '/admin/notificacoes' : '/portal/notificacoes'
)

const totalFormatado = computed(() => {
  if (notificacoesStore.totalNaoLidas > 99) {
    return '99+'
  }

  return String(notificacoesStore.totalNaoLidas)
})

const ariaLabel = computed(() => {
  const total = notificacoesStore.totalNaoLidas
  if (total === 0) {
    return 'Abrir central de notificacoes. Nenhuma notificacao nao lida.'
  }

  if (total === 1) {
    return 'Abrir central de notificacoes. 1 notificacao nao lida.'
  }

  return `Abrir central de notificacoes. ${total} notificacoes nao lidas.`
})

async function abrirCentral(): Promise<void> {
  if (route.path !== rotaDestino.value) {
    await router.push(rotaDestino.value)
  }
}

onMounted(async () => {
  try {
    await notificacoesStore.carregarContagem()
  } catch {
    // Mantem o layout responsivo mesmo quando a contagem falhar.
  }
})
</script>

<template>
  <q-btn
    :flat="compact"
    :round="compact"
    color="primary"
    icon="notifications"
    :dense="compact"
    :aria-label="ariaLabel"
    :title="ariaLabel"
    @click="abrirCentral"
  >
    <q-badge
      v-if="notificacoesStore.totalNaoLidas > 0"
      color="negative"
      rounded
      floating
      :label="totalFormatado"
    />

    <q-spinner v-else-if="notificacoesStore.carregandoContagem" color="primary" size="18px" />

    <span v-if="!compact" class="q-ml-sm text-weight-medium">Notificações</span>
    <q-tooltip>{{ ariaLabel }}</q-tooltip>
  </q-btn>
</template>
