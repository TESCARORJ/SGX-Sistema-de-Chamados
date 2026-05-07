<script setup lang="ts">
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

const columns = [
  { name: 'dataRecebimento', label: 'Recebido em', field: 'dataRecebimento', align: 'left' as const },
  { name: 'remetente', label: 'Remetente', field: 'remetente', align: 'left' as const },
  { name: 'assunto', label: 'Assunto', field: 'assunto', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'statusProcessamento', align: 'left' as const },
  { name: 'chamado', label: 'Chamado', field: 'chamadoCodigo', align: 'left' as const },
  { name: 'erro', label: 'Erro resumido', field: 'erroResumido', align: 'left' as const },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' as const },
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
      <q-td :props="slotProps">{{ slotProps.row.erroResumido ?? '-' }}</q-td>
    </template>

    <template #body-cell-acoes="slotProps">
      <q-td :props="slotProps">
        <q-btn flat color="primary" label="Ver detalhe" @click="emit('verDetalhe', slotProps.row.id)" />
      </q-td>
    </template>

    <template #no-data>
      <div class="full-width text-center q-pa-lg text-grey-7">
        Nenhum log encontrado para os filtros aplicados.
      </div>
    </template>
  </q-table>
</template>
