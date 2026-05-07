<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import PrioridadeBadge from '../ui/PrioridadeBadge.vue'
import SlaBadge from '../ui/SlaBadge.vue'
import StatusBadge from '../ui/StatusBadge.vue'
import type { ChamadoAdminResumo } from '../../types/admin'

const $q = useQuasar()

const props = defineProps<{
  rows: ChamadoAdminResumo[]
  loading?: boolean
  canForceAssume?: boolean
}>()

const emit = defineEmits<{
  (e: 'detalhar', id: string): void
  (e: 'assumir', id: string): void
}>()

const columns: QTableColumn<ChamadoAdminResumo>[] = [
  { name: 'codigo', label: 'Codigo', field: 'codigo', align: 'left', sortable: true },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'solicitante', label: 'Solicitante', field: 'solicitanteNome', align: 'left', sortable: true },
  { name: 'status', label: 'Status', field: 'status', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'sla', label: 'SLA', field: 'slaVencido', align: 'left' },
  { name: 'responsavel', label: 'Responsavel', field: 'responsavelNome', align: 'left', sortable: true },
  { name: 'abertoEm', label: 'Aberto em', field: 'abertoEm', align: 'left', sortable: true },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
]

function formatarData(value: string | null): string {
  if (!value) {
    return '-'
  }

  return new Date(value).toLocaleString('pt-BR')
}

function podeAssumir(row: ChamadoAdminResumo): boolean {
  if (!row.responsavelNome) {
    return true
  }

  return !!props.canForceAssume
}
</script>

<template>
  <q-table
    flat
    :rows="props.rows"
    :columns="columns"
    row-key="id"
    :loading="props.loading"
    hide-pagination
    hide-bottom
    :grid="$q.screen.lt.md"
  >
    <template #body-cell-status="slotProps">
      <q-td :props="slotProps">
        <StatusBadge :texto="slotProps.row.status" />
      </q-td>
    </template>

    <template #body-cell-prioridade="slotProps">
      <q-td :props="slotProps">
        <PrioridadeBadge :texto="slotProps.row.prioridade" />
      </q-td>
    </template>

    <template #body-cell-sla="slotProps">
      <q-td :props="slotProps">
        <SlaBadge
          :vencido="slotProps.row.slaVencido"
          :proximo="slotProps.row.slaProximoVencimento"
          :pausado="slotProps.row.estaPausado"
        />
      </q-td>
    </template>

    <template #body-cell-abertoEm="slotProps">
      <q-td :props="slotProps">{{ formatarData(slotProps.row.abertoEm) }}</q-td>
    </template>

    <template #body-cell-atualizadoEm="slotProps">
      <q-td :props="slotProps">{{ formatarData(slotProps.row.atualizadoEm) }}</q-td>
    </template>

    <template #body-cell-acoes="slotProps">
      <q-td :props="slotProps" class="text-right">
        <div class="row justify-end q-gutter-xs">
          <q-btn flat dense color="primary" icon="visibility" label="Ver detalhe" @click="emit('detalhar', slotProps.row.id)" />
          <q-btn
            outline
            dense
            color="secondary"
            icon="person_add"
            label="Assumir"
            :disable="!podeAssumir(slotProps.row)"
            @click="emit('assumir', slotProps.row.id)"
          />
        </div>
      </q-td>
    </template>

    <template #item="slotProps">
      <div class="col-12 q-mb-sm">
        <q-card flat bordered class="sgx-card">
          <q-card-section class="row items-start justify-between q-col-gutter-sm">
            <div class="col">
              <div class="text-caption text-grey-7">{{ slotProps.row.codigo }}</div>
              <div class="text-subtitle1 text-weight-medium">{{ slotProps.row.titulo }}</div>
              <div class="text-caption text-grey-7 q-mt-xs">Solicitante: {{ slotProps.row.solicitanteNome }}</div>
              <div class="text-caption text-grey-7">Responsavel: {{ slotProps.row.responsavelNome || '-' }}</div>
              <div class="text-caption text-grey-7">Aberto em: {{ formatarData(slotProps.row.abertoEm) }}</div>
            </div>

            <div class="col-auto column items-end q-gutter-xs">
              <StatusBadge :texto="slotProps.row.status" />
              <PrioridadeBadge :texto="slotProps.row.prioridade" />
              <SlaBadge
                :vencido="slotProps.row.slaVencido"
                :proximo="slotProps.row.slaProximoVencimento"
                :pausado="slotProps.row.estaPausado"
              />
            </div>
          </q-card-section>

          <q-separator />

          <q-card-actions align="right">
            <q-btn flat dense color="primary" icon="visibility" label="Ver detalhe" @click="emit('detalhar', slotProps.row.id)" />
            <q-btn
              outline
              dense
              color="secondary"
              icon="person_add"
              label="Assumir"
              :disable="!podeAssumir(slotProps.row)"
              @click="emit('assumir', slotProps.row.id)"
            />
          </q-card-actions>
        </q-card>
      </div>
    </template>
  </q-table>
</template>
