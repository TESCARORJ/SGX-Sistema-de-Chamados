<script setup lang="ts">
import type { ChamadoResumoPortal } from '../../types/portal'
import PrioridadeBadge from '../ui/PrioridadeBadge.vue'
import SlaBadge from '../ui/SlaBadge.vue'
import StatusBadge from '../ui/StatusBadge.vue'

const props = defineProps<{
  chamado: ChamadoResumoPortal
}>()

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}
</script>

<template>
  <q-card
    flat
    bordered
    class="sgx-card card-chamado"
    :class="{
      'card-chamado--vencido': props.chamado.slaVencido,
      'card-chamado--proximo': props.chamado.slaProximoVencimento && !props.chamado.slaVencido,
    }"
  >
    <q-card-section class="card-chamado__header row items-start justify-between q-col-gutter-md">
      <div class="col">
        <div class="text-caption text-grey-7">{{ props.chamado.codigo }}</div>
        <div class="text-subtitle1 text-weight-medium q-mt-xs">{{ props.chamado.titulo }}</div>

        <div class="row q-col-gutter-md q-mt-sm text-caption text-grey-8">
          <div class="col-auto">Categoria: {{ props.chamado.categoria }}</div>
          <div class="col-auto">Departamento: {{ props.chamado.departamento || 'Nao informado' }}</div>
        </div>

        <div class="row q-col-gutter-md q-mt-xs text-caption text-grey-7">
          <div class="col-auto">Aberto em: {{ formatarData(props.chamado.abertoEm) }}</div>
          <div class="col-auto">Atualizado em: {{ formatarData(props.chamado.atualizadoEm) }}</div>
        </div>
      </div>

      <div class="col-auto column items-end q-gutter-xs badges-coluna">
        <StatusBadge :texto="props.chamado.status" />
        <PrioridadeBadge :texto="props.chamado.prioridade" />
        <SlaBadge
          :vencido="props.chamado.slaVencido"
          :proximo="props.chamado.slaProximoVencimento"
          :pausado="props.chamado.estaPausado"
        />
      </div>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.card-chamado {
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.card-chamado:hover {
  box-shadow: 0 10px 28px rgba(15, 23, 42, 0.12);
  transform: translateY(-1px);
}

.card-chamado--vencido {
  border-left: 4px solid #c62828;
}

.card-chamado--proximo {
  border-left: 4px solid #ef6c00;
}

.badges-coluna {
  min-width: 172px;
}

@media (max-width: 768px) {
  .badges-coluna {
    min-width: 100%;
    align-items: flex-start;
  }
}
</style>
