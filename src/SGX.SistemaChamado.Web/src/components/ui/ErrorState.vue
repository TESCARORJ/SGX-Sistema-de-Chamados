<script setup lang="ts">
const props = defineProps<{
  titulo?: string
  mensagem: string
  botaoLabel?: string
}>()

const emit = defineEmits<{
  retry: []
}>()

function onRetry(): void {
  emit('retry')
}
</script>

<template>
  <q-card flat bordered class="sgx-card">
    <q-card-section class="column items-center text-center q-gutter-sm q-py-xl">
      <q-icon name="error_outline" color="negative" size="42px" />
      <div class="text-subtitle1 text-weight-medium">{{ titulo || 'Não foi possível carregar os dados.' }}</div>
      <div class="text-body2 text-grey-8">{{ mensagem }}</div>
      <q-btn color="negative" outline icon="refresh" :label="props.botaoLabel || 'Tentar novamente'" @click="onRetry" />
    </q-card-section>
  </q-card>
</template>
