<script setup lang="ts">
import type { StatusProcessamentoEmail } from '../../types/integracaoEmail'

const props = defineProps<{
  status: StatusProcessamentoEmail
}>()

const colorByStatus: Record<StatusProcessamentoEmail, string> = {
  Pendente: 'grey-7',
  Processado: 'positive',
  Ignorado: 'warning',
  Duplicado: 'orange-8',
  NaoCorrelacionado: 'indigo',
  Erro: 'negative',
}

const labelByStatus: Record<StatusProcessamentoEmail, string> = {
  Pendente: 'Pendente',
  Processado: 'Processado',
  Ignorado: 'Ignorado',
  Duplicado: 'Duplicado',
  NaoCorrelacionado: 'Não correlacionado',
  Erro: 'Erro',
}
</script>

<template>
  <q-badge :color="colorByStatus[props.status]" class="text-weight-medium">
    {{ labelByStatus[props.status] }}
  </q-badge>
</template>
