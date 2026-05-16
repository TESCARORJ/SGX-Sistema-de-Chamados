<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type {
  CategoriaAdmin,
  DepartamentoAdmin,
  LocalUnidadeAdmin,
  SubcategoriaAdmin,
  TipoSolicitacaoAdmin,
} from '../../types/admin'

const props = defineProps<{
  modelValue: boolean
  categorias: CategoriaAdmin[]
  subcategorias: SubcategoriaAdmin[]
  tiposSolicitacao: TipoSolicitacaoAdmin[]
  locaisUnidade: LocalUnidadeAdmin[]
  departamentos: DepartamentoAdmin[]
  valoresIniciais?: {
    categoriaId?: string | null
    subcategoriaId?: string | null
    tipoSolicitacaoId?: string | null
    localUnidadeId?: string | null
    departamentoId?: string | null
  }
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'confirmar', payload: {
    categoriaId: string
    subcategoriaId?: string
    tipoSolicitacaoId?: string
    localUnidadeId?: string
    departamentoId?: string
  }): void
}>()

const categoriaId = ref<string>('')
const subcategoriaId = ref<string | null>(null)
const tipoSolicitacaoId = ref<string | null>(null)
const localUnidadeId = ref<string | null>(null)
const departamentoId = ref<string | null>(null)

const subcategoriasFiltradas = computed(() => {
  if (!categoriaId.value) {
    return []
  }

  return props.subcategorias
    .filter((subcategoria) => subcategoria.categoriaChamadoId === categoriaId.value)
    .map((subcategoria) => ({ label: subcategoria.nome, value: subcategoria.id }))
})

watch(
  () => props.modelValue,
  (opened) => {
    if (!opened) {
      return
    }

    categoriaId.value = props.valoresIniciais?.categoriaId ?? ''
    subcategoriaId.value = props.valoresIniciais?.subcategoriaId ?? null
    tipoSolicitacaoId.value = props.valoresIniciais?.tipoSolicitacaoId ?? null
    localUnidadeId.value = props.valoresIniciais?.localUnidadeId ?? null
    departamentoId.value = props.valoresIniciais?.departamentoId ?? null
  }
)

watch(categoriaId, (novaCategoria) => {
  if (!novaCategoria) {
    subcategoriaId.value = null
    return
  }

  const subcategoriaValida = props.subcategorias.some(
    (subcategoria) =>
      subcategoria.id === subcategoriaId.value && subcategoria.categoriaChamadoId === novaCategoria
  )

  if (!subcategoriaValida) {
    subcategoriaId.value = null
  }
})

function confirmar(): void {
  if (!categoriaId.value) return

  emit('confirmar', {
    categoriaId: categoriaId.value,
    subcategoriaId: subcategoriaId.value ?? undefined,
    tipoSolicitacaoId: tipoSolicitacaoId.value ?? undefined,
    localUnidadeId: localUnidadeId.value ?? undefined,
    departamentoId: departamentoId.value ?? undefined,
  })
}
</script>

<template>
  <q-dialog :model-value="props.modelValue" @update:model-value="emit('update:modelValue', $event)">
    <q-card class="modal-card">
      <q-card-section><div class="text-h6">Alterar classificação</div></q-card-section>
      <q-card-section>
        <q-select
          v-model="categoriaId"
          :options="props.categorias.map(c => ({ label: c.nome, value: c.id }))"
          emit-value
          map-options
          outlined
          label="Categoria"
        />

        <q-select
          v-model="subcategoriaId"
          class="q-mt-md"
          :options="subcategoriasFiltradas"
          emit-value
          map-options
          clearable
          outlined
          :disable="!categoriaId"
          label="Subcategoria"
        />

        <q-select
          v-model="tipoSolicitacaoId"
          class="q-mt-md"
          :options="props.tiposSolicitacao.map((tipo) => ({ label: tipo.nome, value: tipo.id }))"
          emit-value
          map-options
          clearable
          outlined
          label="Tipo de solicitação"
        />

        <q-select
          v-model="localUnidadeId"
          class="q-mt-md"
          :options="props.locaisUnidade.map((local) => ({ label: local.nome, value: local.id }))"
          emit-value
          map-options
          clearable
          outlined
          label="Local / Unidade"
        />

        <q-select
          v-model="departamentoId"
          class="q-mt-md"
          :options="props.departamentos.map((departamento) => ({ label: departamento.nome, value: departamento.id }))"
          emit-value
          map-options
          clearable
          outlined
          label="Departamento"
        />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="emit('update:modelValue', false)" />
        <q-btn color="primary" label="Salvar" :loading="props.loading" @click="confirmar" />
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<style scoped>
.modal-card {
  width: min(560px, 92vw);
}
</style>
