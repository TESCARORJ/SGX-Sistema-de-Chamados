<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'peso', label: 'Peso', field: 'peso', align: 'left', sortable: true },
  { name: 'cor', label: 'Cor', field: 'cor', align: 'left' },
  { name: 'descricao', label: 'Descrição', field: (row) => row.descricao || '-', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView
    titulo="Cadastros - Prioridades"
    entidade="prioridades"
    detalhe-path-base="/admin/cadastros/prioridades"
    :colunas="colunas"
  />
</template>

