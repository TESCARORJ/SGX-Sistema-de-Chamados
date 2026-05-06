<script setup lang="ts">
import { reactive } from 'vue'
import type { AdminContextoResponse, FiltroChamadosAdmin } from '../../types/admin'

const props = defineProps<{
  contexto: AdminContextoResponse | null
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'filtrar', filtros: FiltroChamadosAdmin): void
  (e: 'limpar'): void
}>()

const filtros = reactive<FiltroChamadosAdmin>({
  pagina: 1,
  tamanhoPagina: 20,
  ordenarPor: 'atualizadoEm',
  direcaoOrdenacao: 'desc',
})

function aplicar(): void {
  emit('filtrar', { ...filtros })
}

function limpar(): void {
  filtros.statusId = undefined
  filtros.prioridadeId = undefined
  filtros.categoriaId = undefined
  filtros.departamentoId = undefined
  filtros.responsavelId = undefined
  filtros.solicitanteId = undefined
  filtros.dataInicio = undefined
  filtros.dataFim = undefined
  filtros.slaVencido = undefined
  filtros.texto = undefined
  filtros.pagina = 1
  filtros.tamanhoPagina = 20
  filtros.ordenarPor = 'atualizadoEm'
  filtros.direcaoOrdenacao = 'desc'
  emit('limpar')
  emit('filtrar', { ...filtros })
}
</script>

<template>
  <q-card flat bordered>
    <q-card-section class="row q-col-gutter-sm">
      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.statusId"
          :options="props.contexto?.status.map(s => ({ label: s.nome, value: s.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Status"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.prioridadeId"
          :options="props.contexto?.prioridades.map(p => ({ label: p.nome, value: p.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Prioridade"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.categoriaId"
          :options="props.contexto?.categorias.map(c => ({ label: c.nome, value: c.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Categoria"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.departamentoId"
          :options="props.contexto?.departamentos.map(d => ({ label: d.nome, value: d.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Departamento"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.responsavelId"
          :options="props.contexto?.atendentes.map(a => ({ label: `${a.nome} (${a.perfis.join(', ')})`, value: a.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Responsavel"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-input v-model="filtros.solicitanteId" outlined label="Solicitante ID" />
      </div>
      <div class="col-12 col-md-2">
        <q-input v-model="filtros.dataInicio" type="date" outlined label="Data inicio" />
      </div>
      <div class="col-12 col-md-2">
        <q-input v-model="filtros.dataFim" type="date" outlined label="Data fim" />
      </div>
      <div class="col-12 col-md-2">
        <q-select
          v-model="filtros.slaVencido"
          :options="[{ label: 'Todos', value: undefined }, { label: 'Somente vencidos', value: true }, { label: 'Somente no prazo', value: false }]"
          emit-value
          map-options
          outlined
          label="SLA"
        />
      </div>
      <div class="col-12 col-md-3">
        <q-input v-model="filtros.texto" outlined label="Busca por texto" />
      </div>
      <div class="col-6 col-md-2">
        <q-select
          v-model="filtros.ordenarPor"
          :options="[
            { label: 'Atualizacao', value: 'atualizadoEm' },
            { label: 'Abertura', value: 'abertoEm' },
            { label: 'Codigo', value: 'codigo' },
            { label: 'Titulo', value: 'titulo' },
          ]"
          emit-value
          map-options
          outlined
          label="Ordenar por"
        />
      </div>
      <div class="col-6 col-md-2">
        <q-select
          v-model="filtros.direcaoOrdenacao"
          :options="[{ label: 'Desc', value: 'desc' }, { label: 'Asc', value: 'asc' }]"
          emit-value
          map-options
          outlined
          label="Direcao"
        />
      </div>
      <div class="col-6 col-md-1">
        <q-input v-model.number="filtros.pagina" type="number" min="1" outlined label="Pag" />
      </div>
      <div class="col-6 col-md-1">
        <q-input v-model.number="filtros.tamanhoPagina" type="number" min="1" max="100" outlined label="Tam" />
      </div>
    </q-card-section>

    <q-card-actions align="right">
      <q-btn flat label="Limpar" :disable="props.loading" @click="limpar" />
      <q-btn color="primary" label="Filtrar" :loading="props.loading" @click="aplicar" />
    </q-card-actions>
  </q-card>
</template>
