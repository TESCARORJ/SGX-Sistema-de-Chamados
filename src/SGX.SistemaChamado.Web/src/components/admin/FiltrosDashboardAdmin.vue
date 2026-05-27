<script setup lang="ts">
import { reactive } from 'vue'
import type { AdminContextoResponse } from '../../types/admin'
import type { FiltroIndicadoresRequest } from '../../types/indicadores'
import { NaturezaChamado } from '../../types/portal'

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
  naturezaChamado: '' as '' | NaturezaChamado,
})

const opcoesNatureza = [
  { label: 'Todos', value: undefined },
  { label: 'Incidente', value: NaturezaChamado.Incidente },
  { label: 'Requisicao', value: NaturezaChamado.Requisicao },
  { label: 'Mudanca', value: NaturezaChamado.Mudanca },
  { label: 'Problema', value: NaturezaChamado.Problema },
  { label: 'Evento/Alerta', value: NaturezaChamado.EventoAlerta },
  { label: 'Tarefa operacional', value: NaturezaChamado.TarefaOperacional },
]

function emitirFiltros(): void {
  emit('filtrar', {
    dataInicio: state.dataInicio || undefined,
    dataFim: state.dataFim || undefined,
    departamentoId: state.departamentoId || undefined,
    categoriaId: state.categoriaId || undefined,
    responsavelId: state.responsavelId || undefined,
    naturezaChamado: state.naturezaChamado || undefined,
  })
}

function limpar(): void {
  state.dataInicio = ''
  state.dataFim = ''
  state.departamentoId = ''
  state.categoriaId = ''
  state.responsavelId = ''
  state.naturezaChamado = ''
  emitirFiltros()
}
</script>

<template>
  <q-form class="row q-col-gutter-sm" @submit.prevent="emitirFiltros">
    <div class="col-12 col-sm-6 col-md-2">
      <q-input v-model="state.dataInicio" outlined type="date" label="Período inicial" :disable="props.loading" />
    </div>

    <div class="col-12 col-sm-6 col-md-2">
      <q-input v-model="state.dataFim" outlined type="date" label="Período final" :disable="props.loading" />
    </div>

    <div class="col-12 col-md-3">
      <q-select
        v-model="state.departamentoId"
        outlined
        clearable
        emit-value
        map-options
        :disable="props.loading"
        :options="props.contexto?.departamentos.map((d) => ({ label: d.nome, value: d.id })) ?? []"
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
        :disable="props.loading"
        :options="props.contexto?.categorias.map((c) => ({ label: c.nome, value: c.id })) ?? []"
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
        :disable="props.loading"
        :options="props.contexto?.atendentes.map((a) => ({ label: a.nome, value: a.id })) ?? []"
        label="Responsável"
      />
    </div>

    <div class="col-12 col-md-3">
      <q-select
        v-model="state.naturezaChamado"
        outlined
        clearable
        emit-value
        map-options
        :disable="props.loading"
        :options="opcoesNatureza"
        label="Natureza ITSM"
      />
    </div>

    <div class="col-12 row justify-end q-gutter-sm">
      <q-btn flat label="Limpar" :disable="props.loading" @click="limpar" />
      <q-btn type="submit" color="primary" label="Aplicar filtros" icon="filter_list" :loading="props.loading" />
    </div>
  </q-form>
</template>
