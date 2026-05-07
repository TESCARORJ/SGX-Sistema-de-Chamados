<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'email', label: 'E-mail', field: 'email', align: 'left', sortable: true },
  { name: 'login', label: 'Login', field: 'login', align: 'left', sortable: true },
  { name: 'departamento', label: 'Departamento', field: (row) => row.departamento || '-', align: 'left', sortable: true },
  {
    name: 'perfis',
    label: 'Perfis',
    field: (row) => row.perfis?.map((perfil: { nome: string }) => perfil.nome).join(', ') || '-',
    align: 'left',
  },
  { name: 'situacao', label: 'Situacao', field: 'situacao', align: 'left', sortable: true },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
]
</script>

<template>
  <CadastroListaBaseView titulo="Cadastros - Usuarios" entidade="usuarios" detalhe-path-base="/admin/cadastros/usuarios" :colunas="colunas" />
</template>

