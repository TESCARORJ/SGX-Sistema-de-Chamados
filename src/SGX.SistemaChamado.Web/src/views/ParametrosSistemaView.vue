<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'chave', label: 'Chave', field: 'chave', align: 'left', sortable: true },
  { name: 'valor', label: 'Valor', field: (row) => (row.sensivel ? '********' : row.valor), align: 'left' },
  { name: 'sensivel', label: 'Sensível', field: 'sensivel', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView
    titulo="Configurações - Parâmetros do Sistema"
    entidade="parametros"
    detalhe-path-base="/admin/configuracoes/parametros"
    :colunas="colunas"
  />
</template>

