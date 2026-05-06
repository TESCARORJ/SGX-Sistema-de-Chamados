<script setup lang="ts">
defineProps<{
  titulo: string
  loading?: boolean
  somenteLeitura?: boolean
  botaoSalvarLabel?: string
}>()

const emit = defineEmits<{
  (e: 'salvar'): void
  (e: 'cancelar'): void
}>()
</script>

<template>
  <q-card flat bordered>
    <q-card-section>
      <div class="text-h6">{{ titulo }}</div>
    </q-card-section>

    <q-separator />

    <q-card-section>
      <slot />
    </q-card-section>

    <q-separator />

    <q-card-actions align="right">
      <q-btn flat label="Voltar" :disable="loading" @click="emit('cancelar')" />
      <q-btn
        v-if="!somenteLeitura"
        color="primary"
        :loading="loading"
        :label="botaoSalvarLabel || 'Salvar'"
        @click="emit('salvar')"
      />
    </q-card-actions>
  </q-card>
</template>
