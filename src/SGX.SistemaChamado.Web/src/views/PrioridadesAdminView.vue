<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'nivel', label: 'Nivel', field: 'nivel', align: 'left', sortable: true },
  { name: 'prazoResolucaoHoras', label: 'Prazo Resolucao (h)', field: 'prazoResolucaoHoras', align: 'left', sortable: true },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
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

