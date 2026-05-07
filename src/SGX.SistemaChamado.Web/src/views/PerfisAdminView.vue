<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'tipoPerfilDescricao', label: 'Tipo', field: 'tipoPerfilDescricao', align: 'left', sortable: true },
  { name: 'descricao', label: 'Descricao', field: (row) => row.descricao || '-', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView titulo="Cadastros - Perfis" entidade="perfis" detalhe-path-base="/admin/cadastros/perfis" :colunas="colunas" />
</template>

