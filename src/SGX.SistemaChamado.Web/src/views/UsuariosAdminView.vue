<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'email', label: 'E-mail', field: 'email', align: 'left', sortable: true },
  { name: 'login', label: 'Login', field: 'login', align: 'left', sortable: true },
  { name: 'situacao', label: 'Situação', field: 'situacao', align: 'left', sortable: true },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView titulo="Cadastros - Usuários" entidade="usuarios" detalhe-path-base="/admin/cadastros/usuarios" :colunas="colunas" />
</template>
