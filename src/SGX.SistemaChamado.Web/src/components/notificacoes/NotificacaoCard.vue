<script setup lang="ts">
import { computed } from 'vue'
import type { MinhaNotificacaoResumoResponse } from '../../types/notificacoes'

const props = defineProps<{
  notificacao: MinhaNotificacaoResumoResponse
  tipoEventoTexto: string
  enviadaEmTexto: string
  acaoCarregando: boolean
  podeAbrirChamado: boolean
}>()

const emit = defineEmits<{
  abrirDetalhe: [id: string]
  marcarComoLida: [id: string]
  marcarComoNaoLida: [id: string]
  abrirChamado: [id: string]
}>()

const rotuloLeitura = computed(() => (props.notificacao.lida ? 'Lida' : 'Não lida'))
</script>

<template>
  <q-card flat bordered class="sgx-card notificacao-card" :class="{ 'notificacao-card--nao-lida': !notificacao.lida }">
    <q-card-section class="row items-start justify-between q-col-gutter-md">
      <div class="col">
        <div class="row items-center q-gutter-sm q-mb-sm">
          <q-badge
            :color="notificacao.lida ? 'grey-6' : 'primary'"
            :label="rotuloLeitura"
            :aria-label="`Estado da notificacao: ${rotuloLeitura}`"
          />
          <q-chip dense square color="blue-1" text-color="primary" icon="campaign">
            {{ tipoEventoTexto }}
          </q-chip>
        </div>

        <h2 class="text-subtitle1 text-weight-bold q-ma-none notificacao-card__titulo">
          {{ notificacao.assunto || 'Notificação do sistema' }}
        </h2>

        <div class="text-body2 text-grey-8 q-mt-sm notificacao-card__resumo">
          {{ notificacao.conteudoResumo }}
        </div>

        <div class="row items-center q-col-gutter-md q-mt-md text-caption text-grey-7">
          <div class="col-auto">
            <q-icon name="schedule" size="16px" class="q-mr-xs" />
            {{ enviadaEmTexto }}
          </div>
          <div v-if="notificacao.chamadoId" class="col-auto">
            <q-icon name="confirmation_number" size="16px" class="q-mr-xs" />
            Chamado relacionado
          </div>
        </div>
      </div>

      <div class="col-12 col-md-auto">
        <div class="row q-col-gutter-sm justify-end notificacao-card__acoes">
          <div class="col-12 col-sm-auto">
            <q-btn
              flat
              color="primary"
              icon="visibility"
              label="Abrir detalhe"
              class="full-width"
              @click="emit('abrirDetalhe', notificacao.id)"
            />
          </div>

          <div class="col-12 col-sm-auto">
            <q-btn
              v-if="!notificacao.lida"
              color="secondary"
              outline
              icon="done"
              label="Marcar como lida"
              class="full-width"
              :loading="acaoCarregando"
              :disable="acaoCarregando"
              :aria-label="`Marcar notificação ${notificacao.assunto || notificacao.id} como lida`"
              @click="emit('marcarComoLida', notificacao.id)"
            />

            <q-btn
              v-else
              flat
              color="primary"
              icon="mark_email_unread"
              label="Marcar como não lida"
              class="full-width"
              :loading="acaoCarregando"
              :disable="acaoCarregando"
              :aria-label="`Marcar notificação ${notificacao.assunto || notificacao.id} como não lida`"
              @click="emit('marcarComoNaoLida', notificacao.id)"
            />
          </div>

          <div v-if="podeAbrirChamado" class="col-12 col-sm-auto">
            <q-btn
              flat
              color="primary"
              icon="open_in_new"
              label="Abrir chamado"
              class="full-width"
              @click="notificacao.chamadoId && emit('abrirChamado', notificacao.chamadoId)"
            />
          </div>
        </div>
      </div>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.notificacao-card {
  border-radius: var(--sgx-radius-md);
}

.notificacao-card--nao-lida {
  border-color: rgba(11, 94, 215, 0.28);
  box-shadow: inset 4px 0 0 var(--sgx-primary);
}

.notificacao-card__titulo,
.notificacao-card__resumo {
  overflow-wrap: anywhere;
}

.notificacao-card__acoes {
  min-width: min(100%, 280px);
}

@media (max-width: 767px) {
  .notificacao-card__acoes {
    width: 100%;
  }
}
</style>
