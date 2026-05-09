<script setup lang="ts">
import { useQuasar } from 'quasar'
import type { LogIntegracaoEmailResumoResponse } from '../../types/integracaoEmail'
import StatusProcessamentoEmailBadge from './StatusProcessamentoEmailBadge.vue'

const props = defineProps<{
  rows: LogIntegracaoEmailResumoResponse[]
  total: number
  pagina: number
  tamanhoPagina: number
  loading?: boolean
}>()

const emit = defineEmits<{
  alterarPagina: [pagina: number]
  verDetalhe: [id: string]
}>()

const $q = useQuasar()

const columns = [
  { name: 'dataRecebimento', label: 'Data recebimento', field: 'dataRecebimento', align: 'left' as const },
  { name: 'remetente', label: 'Remetente', field: 'remetente', align: 'left' as const },
  { name: 'assunto', label: 'Assunto', field: 'assunto', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'statusProcessamento', align: 'left' as const },
  { name: 'chamado', label: 'Chamado vinculado', field: 'chamadoCodigo', align: 'left' as const },
  { name: 'erro', label: 'Erro', field: 'erroResumido', align: 'left' as const },
  { name: 'acoes', label: 'Ações', field: 'id', align: 'right' as const },
]

function formatarData(valor: string | null): string {
  if (!valor) return '-'
  return new Date(valor).toLocaleString('pt-BR')
}
</script>

<template>
  <q-table
    flat
    bordered
    row-key="id"
    :rows="props.rows"
    :columns="columns"
    :loading="props.loading"
    :grid="$q.screen.lt.md"
    :pagination="{ page: props.pagina, rowsPerPage: props.tamanhoPagina, rowsNumber: props.total }"
    @update:pagination="(p) => emit('alterarPagina', p.page)"
  >
    <template #body-cell-dataRecebimento="slotProps">
      <q-td :props="slotProps">{{ formatarData(slotProps.row.dataRecebimento) }}</q-td>
    </template>

    <template #body-cell-status="slotProps">
      <q-td :props="slotProps">
        <StatusProcessamentoEmailBadge :status="slotProps.row.statusProcessamento" />
      </q-td>
    </template>

    <template #body-cell-chamado="slotProps">
      <q-td :props="slotProps">{{ slotProps.row.chamadoCodigo ?? '-' }}</q-td>
    </template>

    <template #body-cell-erro="slotProps">
      <q-td :props="slotProps">
        <span class="text-caption">{{ slotProps.row.erroResumido ?? '-' }}</span>
      </q-td>
    </template>

    <template #body-cell-acoes="slotProps">
      <q-td :props="slotProps">
        <q-btn flat color="primary" label="Ver detalhe" @click="emit('verDetalhe', slotProps.row.id)" />
      </q-td>
    </template>
  </q-table>
</template>
