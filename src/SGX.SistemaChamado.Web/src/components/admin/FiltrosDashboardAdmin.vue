<script setup lang="ts">
import { reactive } from 'vue'
import type { AdminContextoResponse } from '../../types/admin'
import type { FiltroIndicadoresRequest } from '../../types/indicadores'

const props = defineProps<{
  contexto: AdminContextoResponse | null
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'filtrar', filtros: FiltroIndicadoresRequest): void
}>()

const state = reactive({
  dataInicio: '',
  dataFim: '',
  departamentoId: '',
  categoriaId: '',
  responsavelId: '',
})

function emitirFiltros(): void {
  emit('filtrar', {
    dataInicio: state.dataInicio || undefined,
    dataFim: state.dataFim || undefined,
    departamentoId: state.departamentoId || undefined,
    categoriaId: state.categoriaId || undefined,
    responsavelId: state.responsavelId || undefined,
  })
}

function limpar(): void {
  state.dataInicio = ''
  state.dataFim = ''
  state.departamentoId = ''
  state.categoriaId = ''
  state.responsavelId = ''
  emitirFiltros()
}
</script>

<template>
  <q-card flat bordered>
    <q-card-section class="row q-col-gutter-sm">
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="state.dataInicio" outlined type="date" label="Data inicio" :disable="props.loading" />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="state.dataFim" outlined type="date" label="Data fim" :disable="props.loading" />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="state.departamentoId"
          outlined
          clearable
          emit-value
          map-options
          option-label="label"
          option-value="value"
          :disable="props.loading"
          :options="props.contexto?.departamentos.map(d => ({ label: d.nome, value: d.id })) ?? []"
          label="Departamento"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="state.categoriaId"
          outlined
          clearable
          emit-value
          map-options
          option-label="label"
          option-value="value"
          :disable="props.loading"
          :options="props.contexto?.categorias.map(c => ({ label: c.nome, value: c.id })) ?? []"
          label="Categoria"
        />
      </div>
      <div class="col-12 col-md-2">
        <q-select
          v-model="state.responsavelId"
          outlined
          clearable
          emit-value
          map-options
          option-label="label"
          option-value="value"
          :disable="props.loading"
          :options="props.contexto?.atendentes.map(a => ({ label: a.nome, value: a.id })) ?? []"
          label="Responsavel"
        />
      </div>
    </q-card-section>
    <q-card-actions align="right">
      <q-btn flat label="Limpar" :disable="props.loading" @click="limpar" />
      <q-btn color="primary" label="Aplicar filtros" :loading="props.loading" @click="emitirFiltros" />
    </q-card-actions>
  </q-card>
</template>
