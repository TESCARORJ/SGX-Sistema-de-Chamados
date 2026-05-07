<script setup lang="ts">
import StatusProcessamentoEmailBadge from './StatusProcessamentoEmailBadge.vue'
import type { LogIntegracaoEmailDetalheResponse } from '../../types/integracaoEmail'

const props = defineProps<{
  modelValue: boolean
  detalhe: LogIntegracaoEmailDetalheResponse | null
  loading?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

function fechar(): void {
  emit('update:modelValue', false)
}

function formatarData(valor: string | null): string {
  if (!valor) return '-'
  return new Date(valor).toLocaleString('pt-BR')
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="(v) => emit('update:modelValue', v)">
    <q-card class="sgx-card" style="min-width: 65vw; max-width: 90vw;">
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Detalhe do log de integracao</div>
        <q-btn flat icon="close" round @click="fechar" />
      </q-card-section>

      <q-separator />
      <q-card-section v-if="props.loading" class="row justify-center q-py-lg">
        <q-spinner color="primary" size="2rem" />
      </q-card-section>

      <q-card-section v-else-if="props.detalhe" class="column q-gutter-sm">
        <div><strong>Status:</strong> <StatusProcessamentoEmailBadge :status="props.detalhe.statusProcessamento" /></div>
        <div><strong>MessageId:</strong> {{ props.detalhe.messageId ?? '-' }}</div>
        <div><strong>Fingerprint:</strong> {{ props.detalhe.fingerprint }}</div>
        <div><strong>Remetente:</strong> {{ props.detalhe.nomeRemetente ?? '-' }} ({{ props.detalhe.remetente }})</div>
        <div><strong>Assunto:</strong> {{ props.detalhe.assunto ?? '-' }}</div>
        <div><strong>Chamado vinculado:</strong> {{ props.detalhe.chamadoCodigo ?? '-' }}</div>
        <div><strong>Data recebimento:</strong> {{ formatarData(props.detalhe.dataRecebimento) }}</div>
        <div><strong>Data processamento:</strong> {{ formatarData(props.detalhe.dataProcessamento) }}</div>
        <div><strong>Tentativas:</strong> {{ props.detalhe.tentativas }}</div>
        <div><strong>Criado por:</strong> {{ props.detalhe.criadoPor }}</div>

        <q-expansion-item
          dense
          dense-toggle
          expand-separator
          icon="error"
          label="Erro tecnico"
          header-class="text-negative"
        >
          <q-card flat bordered>
            <q-card-section class="text-negative text-body2">
              {{ props.detalhe.erro ?? 'Sem erro tecnico registrado.' }}
            </q-card-section>
          </q-card>
        </q-expansion-item>
      </q-card-section>
    </q-card>
  </q-dialog>
</template>
