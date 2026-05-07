<script setup lang="ts">
const props = defineProps<{
  vencido?: boolean
  proximo?: boolean
  pausado?: boolean
}>()
</script>

<template>
  <q-chip
    v-if="props.vencido"
    dense
    square
    color="negative"
    text-color="white"
    icon="warning"
    label="SLA vencido"
  />
  <q-chip
    v-else-if="props.proximo"
    dense
    square
    color="warning"
    text-color="black"
    icon="schedule"
    label="SLA proximo"
  />
  <q-chip
    v-else-if="props.pausado"
    dense
    square
    color="grey-7"
    text-color="white"
    icon="pause_circle"
    label="SLA pausado"
  />
  <q-chip
    v-else
    dense
    square
    color="positive"
    text-color="white"
    icon="task_alt"
    label="Dentro do prazo"
  />
</template>
