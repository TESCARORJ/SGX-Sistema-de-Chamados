<script setup lang="ts">
import { ref, watch } from 'vue'
import type { CategoriaAdmin } from '../../types/admin'

const props = defineProps<{
  modelValue: boolean
  categorias: CategoriaAdmin[]
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', categoriaId: string): void
}>()

const categoriaId = ref<string>('')
watch(() => props.modelValue, (opened) => { if (opened) categoriaId.value = '' })

function confirmar(): void {
  if (!categoriaId.value) return
  emit('confirmar', categoriaId.value)
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card style="min-width: 360px">
      <q-card-section><div class="text-h6">Alterar categoria</div></q-card-section>
      <q-card-section>
        <q-select
          v-model="categoriaId"
          :options="props.categorias.map(c => ({ label: c.nome, value: c.id }))"
          emit-value
          map-options
          outlined
          label="Categoria"
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="primary" label="Salvar" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>
