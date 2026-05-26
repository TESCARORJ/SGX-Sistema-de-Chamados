<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    titulo?: string
    title?: string
    mensagem?: string
    descricao?: string
    description?: string
    icon?: string
  }>(),
  {
    titulo: '',
    title: '',
    mensagem: '',
    descricao: '',
    description: '',
    icon: 'inbox',
  }
)

const tituloExibido = computed(() => props.titulo || props.title || 'Nenhum dado encontrado')
const descricaoExibida = computed(() => props.mensagem || props.descricao || props.description || '')
</script>

<template>
  <q-card flat bordered class="sgx-card empty-state" role="status" aria-live="polite">
    <q-card-section class="column items-center text-center q-gutter-sm q-py-xl">
      <q-avatar size="54px" class="empty-state__avatar">
        <q-icon :name="icon" size="28px" color="primary" />
      </q-avatar>
      <div class="text-subtitle1 text-weight-bold">{{ tituloExibido }}</div>
      <div v-if="descricaoExibida" class="text-body2 text-grey-7 empty-state__descricao">{{ descricaoExibida }}</div>
      <div v-if="$slots.actions" class="q-mt-sm">
        <slot name="actions" />
      </div>
    </q-card-section>
  </q-card>
</template>

<style scoped>
.empty-state__avatar {
  background: #eef4ff;
}

.empty-state__descricao {
  max-width: 560px;
}
</style>
