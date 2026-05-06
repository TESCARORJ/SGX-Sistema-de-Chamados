<script setup lang="ts">
defineProps<{
  status: string
}>()
</script>

<template>
  <q-badge color="primary" text-color="white" :label="status" />
</template>
