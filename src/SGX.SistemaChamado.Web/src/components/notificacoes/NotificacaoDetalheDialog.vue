<script setup lang="ts">
import type { MinhaNotificacaoDetalheResponse } from '../../types/notificacoes'

defineProps<{
  modelValue: boolean
  detalhe: MinhaNotificacaoDetalheResponse | null
  carregando: boolean
  erro: string | null
  acaoCarregando: boolean
  tipoEventoTexto: string
  enviadaEmTexto: string
  lidaEmTexto: string | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  retry: []
  marcarComoLida: [id: string]
  marcarComoNaoLida: [id: string]
  abrirChamado: [id: string]
}>()

function fechar(): void {
  emit('update:modelValue', false)
}
</script>

<template>
  <q-dialog :model-value="modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="sgx-card notificacao-detalhe-dialog">
      <q-card-section class="row items-start justify-between q-col-gutter-md">
        <div class="col">
          <div class="text-overline text-grey-7">Detalhe da notificacao</div>
          <h2 class="text-h6 q-ma-none notificacao-detalhe-dialog__titulo">
            {{ detalhe?.assunto || 'Notificacao do sistema' }}
          </h2>
        </div>

        <div class="col-auto">
          <q-btn flat round dense icon="close" aria-label="Fechar detalhe da notificacao" @click="fechar" />
        </div>
      </q-card-section>

      <q-separator />

      <q-card-section
        v-if="carregando"
        class="column items-center q-gutter-sm q-py-xl"
        role="status"
        aria-live="polite"
      >
        <q-spinner color="primary" size="2rem" />
        <div class="text-body2 text-grey-7">Carregando detalhe da notificacao...</div>
      </q-card-section>

      <q-card-section
        v-else-if="erro"
        class="column items-center text-center q-gutter-sm q-py-xl"
        role="alert"
        aria-live="assertive"
      >
        <q-icon name="error_outline" color="negative" size="42px" />
        <div class="text-subtitle2 text-weight-medium">Nao foi possivel carregar o detalhe.</div>
        <div class="text-body2 text-grey-8">{{ erro }}</div>
        <q-btn color="negative" outline icon="refresh" label="Tentar novamente" @click="emit('retry')" />
      </q-card-section>

      <q-card-section v-else-if="detalhe" class="column q-gutter-md">
        <div class="row items-center q-gutter-sm">
          <q-badge :color="detalhe.lida ? 'grey-6' : 'primary'" :label="detalhe.lida ? 'Lida' : 'Nao lida'" />
          <q-chip dense square color="blue-1" text-color="primary" icon="campaign">
            {{ tipoEventoTexto }}
          </q-chip>
        </div>

        <div class="text-body2 text-grey-8">
          <strong>Enviada em:</strong> {{ enviadaEmTexto }}
        </div>

        <div class="text-body2 text-grey-8">
          <strong>Leitura:</strong> {{ detalhe.lida ? 'Lida' : 'Nao lida' }}
          <span v-if="lidaEmTexto"> em {{ lidaEmTexto }}</span>
        </div>

        <div v-if="detalhe.chamadoId" class="text-body2 text-grey-8">
          <strong>Chamado relacionado:</strong> disponivel para navegacao
        </div>

        <q-separator />

        <div class="text-body1 notificacao-detalhe-dialog__conteudo">
          {{ detalhe.conteudo }}
        </div>
      </q-card-section>

      <q-separator v-if="detalhe && !carregando && !erro" />

      <q-card-actions v-if="detalhe && !carregando && !erro" align="right" class="q-pa-md q-gutter-sm">
        <q-btn
          v-if="!detalhe.lida"
          color="secondary"
          outline
          icon="done"
          label="Marcar como lida"
          :loading="acaoCarregando"
          :disable="acaoCarregando"
          @click="emit('marcarComoLida', detalhe.id)"
        />

        <q-btn
          v-else
          flat
          color="primary"
          icon="mark_email_unread"
          label="Marcar como nao lida"
          :loading="acaoCarregando"
          :disable="acaoCarregando"
          @click="emit('marcarComoNaoLida', detalhe.id)"
        />

        <q-btn
          v-if="detalhe.chamadoId"
          flat
          color="primary"
          icon="open_in_new"
          label="Abrir chamado"
          @click="emit('abrirChamado', detalhe.chamadoId)"
        />

        <q-btn flat color="grey-8" icon="close" label="Fechar" @click="fechar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.notificacao-detalhe-dialog {
  width: min(760px, 96vw);
  max-width: 96vw;
}

.notificacao-detalhe-dialog__titulo,
.notificacao-detalhe-dialog__conteudo {
  overflow-wrap: anywhere;
}

.notificacao-detalhe-dialog__conteudo {
  white-space: pre-wrap;
  line-height: 1.6;
}
</style>
