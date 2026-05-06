<script setup lang="ts">
import type { StatusProcessamentoEmail } from '../../types/integracaoEmail'

const props = defineProps<{
  status: StatusProcessamentoEmail
}>()

const colorByStatus: Record<StatusProcessamentoEmail, string> = {
  Pendente: 'grey-7',
  Processado: 'positive',
  IgnoradoDuplicado: 'warning',
  Erro: 'negative',
}

const labelByStatus: Record<StatusProcessamentoEmail, string> = {
  Pendente: 'Pendente',
  Processado: 'Processado',
  IgnoradoDuplicado: 'Duplicado',
  Erro: 'Erro',
}
</script>

<template>
  <q-badge :color="colorByStatus[props.status]" class="text-weight-medium">
    {{ labelByStatus[props.status] }}
  </q-badge>
</template>
