<script setup lang="ts">
import { reactive } from 'vue'
import type { FiltroLogsEmailRequest, StatusProcessamentoEmail } from '../../types/integracaoEmail'

const props = defineProps<{
  loading?: boolean
}>()

const emit = defineEmits<{
  filtrar: [filtro: FiltroLogsEmailRequest]
}>()

const form = reactive<FiltroLogsEmailRequest>({
  pagina: 1,
  tamanhoPagina: 20,
})

const statusOptions: StatusProcessamentoEmail[] = ['Pendente', 'Processado', 'IgnoradoDuplicado', 'Erro']

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
  <q-card flat bordered>
    <q-card-section class="row q-col-gutter-md items-end">
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="form.dataInicio" label="Data inicio" type="date" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model="form.dataFim" label="Data fim" type="date" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-select v-model="form.status" :options="statusOptions" label="Status" clearable dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-input v-model="form.remetente" label="Remetente" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-input v-model="form.chamadoId" label="Chamado (GUID)" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-4">
        <q-input v-model="form.texto" label="Busca livre" dense outlined />
      </div>
      <div class="col-12 col-sm-6 col-md-2">
        <q-input v-model.number="form.tamanhoPagina" label="Tamanho pagina" type="number" min="1" max="200" dense outlined />
      </div>
      <div class="col-12 col-md-6 row justify-end q-gutter-sm">
        <q-btn color="primary" label="Filtrar" :loading="props.loading" @click="aplicarFiltros" />
        <q-btn flat color="primary" label="Limpar" :disable="props.loading" @click="limparFiltros" />
      </div>
    </q-card-section>
  </q-card>
</template>
