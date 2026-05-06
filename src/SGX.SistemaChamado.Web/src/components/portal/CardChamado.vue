<script setup lang="ts">
import type { ChamadoResumoPortal } from '../../types/portal'
import StatusBadge from './StatusBadge.vue'
import PrioridadeBadge from './PrioridadeBadge.vue'

defineProps<{
  chamado: ChamadoResumoPortal
}>()

function fmtPrazo(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}
</script>

<template>
  <q-card flat bordered class="card-chamado" :class="{ 'sla-vencido': chamado.slaVencido, 'sla-proximo': chamado.slaProximoVencimento && !chamado.slaVencido }">
    <q-card-section class="row items-start justify-between q-gutter-sm">
      <div>
        <div class="text-caption text-grey-7">{{ chamado.codigo }}</div>
        <div class="text-subtitle1 text-weight-medium">{{ chamado.titulo }}</div>
        <div class="text-caption text-grey-8">
          {{ chamado.categoria }} • {{ chamado.departamento || 'Sem departamento' }}
        </div>
        <div class="q-mt-xs">
          <q-badge v-if="chamado.slaVencido" color="negative" outline>SLA vencido</q-badge>
          <q-badge v-else-if="chamado.slaProximoVencimento" color="warning" outline>Proximo do vencimento</q-badge>
          <q-badge v-else-if="chamado.estaPausado" color="grey-7" outline>SLA pausado</q-badge>
          <q-badge v-else color="positive" outline>Dentro do prazo</q-badge>
          <div class="text-caption text-grey-7 q-mt-xs">Prazo resolucao: {{ fmtPrazo(chamado.prazoResolucaoEm) }}</div>
        </div>
      </div>
      <div class="column items-end q-gutter-xs">
        <StatusBadge :status="chamado.status" />
        <PrioridadeBadge :prioridade="chamado.prioridade" />
      </div>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.card-chamado {
  border-radius: 12px;
}

.sla-vencido {
  border-left: 4px solid #d32f2f;
}

.sla-proximo {
  border-left: 4px solid #f57c00;
}
</style>
