<script setup lang="ts">
import { useQuasar } from 'quasar'
import EmptyState from '../ui/EmptyState.vue'
import LoadingState from '../ui/LoadingState.vue'
import StatusProcessamentoEmailBadge from './StatusProcessamentoEmailBadge.vue'
import type { LogIntegracaoEmailDetalheResponse } from '../../types/integracaoEmail'

const props = defineProps<{
  modelValue: boolean
  detalhe: LogIntegracaoEmailDetalheResponse | null
  loading?: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  abrirChamado: [id: string]
}>()

const $q = useQuasar()

function fechar(): void {
  emit('update:modelValue', false)
}

function abrirChamado(): void {
  if (!props.detalhe?.chamadoId) {
    return
  }

  emit('abrirChamado', props.detalhe.chamadoId)
}

function formatarData(valor: string | null): string {
  if (!valor) return '-'
  return new Date(valor).toLocaleString('pt-BR')
}
</script>

<template>
  <q-dialog
    :model-value="props.modelValue"
    :maximized="$q.screen.lt.md"
    @update:model-value="(v) => emit('update:modelValue', v)"
  >
    <q-card class="sgx-card" style="width: min(960px, 96vw); max-width: 96vw;">
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Detalhe do log de integracao</div>
        <q-btn flat icon="close" round aria-label="Fechar detalhe do log" @click="fechar" />
      </q-card-section>

      <q-separator />

      <q-card-section v-if="props.loading">
        <LoadingState inline mensagem="Carregando detalhe do log..." />
      </q-card-section>

      <q-card-section v-else-if="props.detalhe">
        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Status</q-item-label>
              <q-item-label>
                <StatusProcessamentoEmailBadge :status="props.detalhe.statusProcessamento" />
              </q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Chamado vinculado</q-item-label>
              <q-item-label>{{ props.detalhe.chamadoCodigo ?? 'Sem vinculo' }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>MessageId</q-item-label>
              <q-item-label>{{ props.detalhe.messageId ?? '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>InReplyTo</q-item-label>
              <q-item-label>{{ props.detalhe.inReplyTo ?? '-' }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>References</q-item-label>
              <q-item-label>{{ props.detalhe.references ?? '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Fingerprint</q-item-label>
              <q-item-label>{{ props.detalhe.fingerprint }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>Remetente</q-item-label>
              <q-item-label>{{ props.detalhe.nomeRemetente ?? '-' }} ({{ props.detalhe.remetente }})</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Destinatario</q-item-label>
              <q-item-label>{{ props.detalhe.destinatario ?? '-' }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>Assunto</q-item-label>
              <q-item-label>{{ props.detalhe.assunto ?? '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Titulo do chamado</q-item-label>
              <q-item-label>{{ props.detalhe.chamadoTitulo ?? '-' }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>Data recebimento</q-item-label>
              <q-item-label>{{ formatarData(props.detalhe.dataRecebimento) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Data processamento</q-item-label>
              <q-item-label>{{ formatarData(props.detalhe.dataProcessamento) }}</q-item-label>
            </q-item-section>
          </q-item>
          <q-item>
            <q-item-section>
              <q-item-label caption>Criado em</q-item-label>
              <q-item-label>{{ formatarData(props.detalhe.criadoEm) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Atualizado em</q-item-label>
              <q-item-label>{{ formatarData(props.detalhe.atualizadoEm) }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <q-expansion-item
          dense
          dense-toggle
          expand-separator
          icon="error"
          label="Erro tecnico"
          header-class="text-negative"
          class="q-mt-md"
        >
          <q-card flat bordered>
            <q-card-section class="text-negative text-body2">
              {{ props.detalhe.erro ?? 'Nenhum erro registrado.' }}
            </q-card-section>
          </q-card>
        </q-expansion-item>
      </q-card-section>

      <q-card-section v-else>
        <EmptyState titulo="Detalhe indisponivel" mensagem="Nao foi possivel carregar os dados." icon="report" />
      </q-card-section>

      <q-separator />

      <q-card-actions align="right">
        <q-btn flat color="primary" label="Fechar" @click="fechar" />
        <q-btn v-if="props.detalhe?.chamadoId" color="secondary" label="Abrir chamado" @click="abrirChamado" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>
