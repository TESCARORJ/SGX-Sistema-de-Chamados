<script setup lang="ts">
import { computed } from 'vue'
import type { ChamadoAdminResumo } from '../../types/admin'

const props = defineProps<{
  rows: ChamadoAdminResumo[]
  loading?: boolean
  canForceAssume?: boolean
}>()

const emit = defineEmits<{
  (e: 'detalhar', id: string): void
  (e: 'assumir', id: string): void
}>()

const columns = computed(() => [
  { name: 'codigo', label: 'Codigo', field: 'codigo', align: 'left', sortable: true },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'solicitante', label: 'Solicitante', field: 'solicitanteNome', align: 'left' },
  { name: 'responsavel', label: 'Responsavel', field: 'responsavelNome', align: 'left' },
  { name: 'status', label: 'Status', field: 'status', align: 'left' },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left' },
  { name: 'sla', label: 'SLA', field: 'slaVencido', align: 'left' },
  { name: 'abertoEm', label: 'Aberto em', field: 'abertoEm', align: 'left' },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
])

function fmtDate(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

function fmtPrazo(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

function linhaClasse(row: ChamadoAdminResumo): string {
  if (row.slaVencido) return 'sla-vencido'
  if (row.slaProximoVencimento) return 'sla-proximo'
  return ''
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
    hide-pagination
  >
    <template #body="slotProps">
      <q-tr :props="slotProps" :class="linhaClasse(slotProps.row)">
        <q-td key="codigo" :props="slotProps">{{ slotProps.row.codigo }}</q-td>
        <q-td key="titulo" :props="slotProps">{{ slotProps.row.titulo }}</q-td>
        <q-td key="solicitante" :props="slotProps">{{ slotProps.row.solicitanteNome }}</q-td>
        <q-td key="responsavel" :props="slotProps">{{ slotProps.row.responsavelNome || '-' }}</q-td>
        <q-td key="status" :props="slotProps">
          <q-badge :color="slotProps.row.status.toLowerCase().includes('encerrado') ? 'positive' : 'primary'">{{ slotProps.row.status }}</q-badge>
        </q-td>
        <q-td key="prioridade" :props="slotProps">
          <q-badge :color="slotProps.row.prioridade.toLowerCase().includes('critica') ? 'negative' : 'secondary'">{{ slotProps.row.prioridade }}</q-badge>
        </q-td>
        <q-td key="sla" :props="slotProps">
          <div class="column">
            <q-badge v-if="slotProps.row.slaVencido" color="negative" outline>SLA vencido</q-badge>
            <q-badge v-else-if="slotProps.row.slaProximoVencimento" color="warning" outline>Proximo do vencimento</q-badge>
            <q-badge v-else-if="slotProps.row.estaPausado" color="grey-7" outline>SLA pausado</q-badge>
            <q-badge v-else color="positive" outline>Dentro do prazo</q-badge>
            <small class="text-grey-7">Prazo: {{ fmtPrazo(slotProps.row.prazoResolucaoEm) }}</small>
          </div>
        </q-td>
        <q-td key="abertoEm" :props="slotProps">{{ fmtDate(slotProps.row.abertoEm) }}</q-td>
        <q-td key="acoes" :props="slotProps" class="text-right">
          <div class="row q-gutter-xs justify-end">
            <q-btn size="sm" flat color="primary" label="Detalhe" @click="emit('detalhar', slotProps.row.id)" />
            <q-btn
              size="sm"
              outline
              color="secondary"
              label="Assumir"
              :disable="Boolean(slotProps.row.responsavelNome) && !props.canForceAssume"
              @click="emit('assumir', slotProps.row.id)"
            />
          </div>
        </q-td>
      </q-tr>
    </template>
  </q-table>
</template>

<style scoped>
.sla-vencido {
  background: #fff1f1;
}

.sla-proximo {
  background: #fff9eb;
}
</style>
