<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'codigo', label: 'Codigo', field: 'codigo', align: 'left', sortable: true },
  { name: 'ehStatusFinal', label: 'Final', field: (row) => (row.ehStatusFinal ? 'Sim' : 'Nao'), align: 'left' },
  { name: 'pausaSla', label: 'Pausa SLA', field: (row) => (row.pausaSla ? 'Sim' : 'Nao'), align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView titulo="Cadastros - Status" entidade="status" detalhe-path-base="/admin/cadastros/status" :colunas="colunas" />
</template>

