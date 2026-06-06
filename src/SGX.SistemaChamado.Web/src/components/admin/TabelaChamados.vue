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
  canAssumir?: boolean
  canForceAssume?: boolean
}>()

const emit = defineEmits<{
  (e: 'detalhar', id: string): void
  (e: 'assumir', id: string): void
}>()

const columns: QTableColumn<ChamadoAdminResumo>[] = [
  { name: 'codigo', label: 'Código', field: 'codigo', align: 'left', sortable: true },
  { name: 'titulo', label: 'Título', field: 'titulo', align: 'left', sortable: true },
  { name: 'solicitante', label: 'Solicitante', field: 'solicitanteNome', align: 'left', sortable: true },
  { name: 'itsm', label: 'Classificação ITSM', field: 'naturezaChamado', align: 'left' },
  { name: 'atendimento', label: 'Atendimento', field: 'grupoTecnicoNome', align: 'left' },
  { name: 'status', label: 'Status', field: 'status', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'sla', label: 'SLA', field: 'slaVencido', align: 'left' },
  { name: 'responsavel', label: 'Responsável', field: 'responsavelNome', align: 'left', sortable: true },
  { name: 'abertoEm', label: 'Aberto em', field: 'abertoEm', align: 'left', sortable: true },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'id', align: 'right' },
]

function formatarData(value: string | null): string {
  if (!value) {
    return '-'
  }

  return new Date(value).toLocaleString('pt-BR')
}

function labelNaturezaChamado(value: number): string {
  switch (value) {
    case 1: return 'Incidente'
    case 2: return 'Requisicao'
    case 3: return 'Mudanca'
    case 4: return 'Problema'
    case 5: return 'Evento/Alerta'
    case 6: return 'Tarefa Operacional'
    default: return `#${value}`
  }
}

function labelImpactoChamado(value: number): string {
  switch (value) {
    case 1: return 'Baixo'
    case 2: return 'Medio'
    case 3: return 'Alto'
    default: return `#${value}`
  }
}

function labelUrgenciaChamado(value: number): string {
  switch (value) {
    case 1: return 'Baixa'
    case 2: return 'Media'
    case 3: return 'Alta'
    default: return `#${value}`
  }
}

function podeAssumir(row: ChamadoAdminResumo): boolean {
  if (props.canAssumir === false) {
    return false
  }

  if (!row.responsavelNome) {
    return true
  }

  return !!props.canForceAssume
}
</script>

<template>
  <q-table
    class="sgx-table"
    flat
    bordered
    :rows="props.rows"
    :columns="columns"
    row-key="id"
    :loading="props.loading"
    hide-pagination
    hide-bottom
    :grid="$q.screen.lt.md"
    separator="horizontal"
  >
    <template #body-cell-codigo="slotProps">
      <q-td :props="slotProps">
        <div class="column q-gutter-xs">
          <q-btn
            flat
            dense
            color="primary"
            class="q-pa-none text-weight-bold chamada-codigo"
            :label="slotProps.row.codigo"
            @click="emit('detalhar', slotProps.row.id)"
          />
          <div class="text-caption text-grey-7 ellipsis">{{ slotProps.row.categoria }}</div>
        </div>
      </q-td>
    </template>

    <template #body-cell-status="slotProps">
      <q-td :props="slotProps">
        <StatusBadge :texto="slotProps.row.status" />
      </q-td>
    </template>

    <template #body-cell-itsm="slotProps">
      <q-td :props="slotProps">
        <div class="column q-gutter-xs">
          <q-chip dense square color="blue-1" text-color="primary">
            {{ labelNaturezaChamado(slotProps.row.naturezaChamado) }}
          </q-chip>
          <div class="text-caption text-grey-8">
            Impacto: {{ labelImpactoChamado(slotProps.row.impactoChamado) }} | Urgencia:
            {{ labelUrgenciaChamado(slotProps.row.urgenciaChamado) }}
          </div>
        </div>
      </q-td>
    </template>

    <template #body-cell-atendimento="slotProps">
      <q-td :props="slotProps">
        <div class="column q-gutter-xs">
          <div class="text-body2 text-weight-medium">
            {{ slotProps.row.grupoTecnicoNome || 'Sem grupo' }}
          </div>
          <div class="text-caption text-grey-7">
            Fila: {{ slotProps.row.filaAtendimentoNome || 'Sem fila' }}
          </div>
        </div>
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
          :situacao="slotProps.row.situacaoSla"
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
        <div class="row justify-end q-gutter-xs no-wrap">
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

    <template #no-data>
      <div class="full-width row flex-center q-pa-lg text-grey-7">
        Nenhum chamado disponível para os filtros atuais.
      </div>
    </template>

    <template #item="slotProps">
      <div class="col-12 q-mb-sm">
        <q-card flat bordered class="sgx-card">
          <q-card-section class="row items-start justify-between q-col-gutter-sm">
            <div class="col">
              <div class="text-caption text-grey-7">{{ slotProps.row.codigo }}</div>
              <div class="text-subtitle1 text-weight-medium">{{ slotProps.row.titulo }}</div>
              <div class="text-caption text-grey-7 q-mt-xs">Categoria: {{ slotProps.row.categoria }}</div>
              <div class="text-caption text-grey-7">
                Natureza: {{ labelNaturezaChamado(slotProps.row.naturezaChamado) }}
              </div>
              <div class="text-caption text-grey-7">
                Impacto: {{ labelImpactoChamado(slotProps.row.impactoChamado) }} | Urgencia:
                {{ labelUrgenciaChamado(slotProps.row.urgenciaChamado) }}
              </div>
              <div class="text-caption text-grey-7">Solicitante: {{ slotProps.row.solicitanteNome }}</div>
              <div class="text-caption text-grey-7">
                Grupo: {{ slotProps.row.grupoTecnicoNome || 'Sem grupo' }}
              </div>
              <div class="text-caption text-grey-7">
                Fila: {{ slotProps.row.filaAtendimentoNome || 'Sem fila' }}
              </div>
              <div class="text-caption text-grey-7">Responsável: {{ slotProps.row.responsavelNome || '-' }}</div>
              <div class="text-caption text-grey-7">Aberto em: {{ formatarData(slotProps.row.abertoEm) }}</div>
            </div>

            <div class="col-auto column items-end q-gutter-xs">
              <StatusBadge :texto="slotProps.row.status" />
              <PrioridadeBadge :texto="slotProps.row.prioridade" />
              <SlaBadge
                :vencido="slotProps.row.slaVencido"
                :proximo="slotProps.row.slaProximoVencimento"
                :pausado="slotProps.row.estaPausado"
                :situacao="slotProps.row.situacaoSla"
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

<style scoped>
:deep(.sgx-table .q-table__top),
:deep(.sgx-table .q-table__bottom) {
  padding-left: 0;
  padding-right: 0;
}

:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
}

:deep(.sgx-table tbody tr:hover) {
  background: rgba(11, 94, 215, 0.04);
}

.chamada-codigo {
  letter-spacing: 0.01em;
}
</style>
