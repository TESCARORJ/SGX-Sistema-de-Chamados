<script setup lang="ts">
import { reactive } from 'vue'
import type { FiltroLogsEmailRequest, StatusProcessamentoEmail } from '../../types/integracaoEmail'

defineProps<{
  loading?: boolean
}>()

const emit = defineEmits<{
  filtrar: [filtro: FiltroLogsEmailRequest]
}>()

const form = reactive<FiltroLogsEmailRequest>({
  pagina: 1,
  tamanhoPagina: 20,
})

const statusOptions: { label: string; value: StatusProcessamentoEmail }[] = [
  { label: 'Pendente', value: 'Pendente' },
  { label: 'Processado', value: 'Processado' },
  { label: 'Ignorado duplicado', value: 'IgnoradoDuplicado' },
  { label: 'Erro', value: 'Erro' },
]

function aplicarFiltros(): void {
  emit('filtrar', { ...form, pagina: 1 })
}

function limparFiltros(): void {
  form.dataInicio = undefined
  form.dataFim = undefined
  form.status = undefined
  form.remetente = undefined
  form.chamadoId = undefined
  form.texto = undefined
  form.pagina = 1
  form.tamanhoPagina = 20
  emit('filtrar', { ...form })
}
</script>

<template>
  <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
    <div class="row q-col-gutter-md">
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="form.dataInicio" label="Data inicio" type="date" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="form.dataFim" label="Data fim" type="date" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-select
          v-model="form.status"
          :options="statusOptions"
          option-label="label"
          option-value="value"
          emit-value
          map-options
          label="Status"
          clearable
          dense
          outlined
        />
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-input v-model="form.remetente" label="Remetente" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-input v-model="form.chamadoId" label="Chamado (ID)" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-4">
        <q-input v-model="form.texto" label="Busca livre" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-input
          v-model.number="form.tamanhoPagina"
          label="Tamanho da pagina"
          type="number"
          min="1"
          max="200"
          dense
          outlined
        />
      </div>
    </div>

    <div class="row justify-end q-gutter-sm">
      <q-btn type="submit" color="primary" label="Filtrar" :loading="loading" />
      <q-btn flat color="primary" label="Limpar" :disable="loading" @click="limparFiltros" />
    </div>
  </q-form>
</template>
