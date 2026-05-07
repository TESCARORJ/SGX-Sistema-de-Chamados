<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'chave', label: 'Chave', field: 'chave', align: 'left', sortable: true },
  { name: 'valor', label: 'Valor', field: 'valor', align: 'left' },
  { name: 'sensivel', label: 'Sensivel', field: 'sensivel', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView
    titulo="ConfigurAcoes - Parametros do Sistema"
    entidade="parametros"
    detalhe-path-base="/admin/configuracoes/parametros"
    :colunas="colunas"
  />
</template>

