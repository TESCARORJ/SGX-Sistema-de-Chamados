<script setup lang="ts">
import type { QTableColumn } from 'quasar'
import CadastroListaBaseView from './admin-cadastros/CadastroListaBaseView.vue'

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'nivel', label: 'Nível', field: 'nivel', align: 'left', sortable: true },
  { name: 'prazoPrimeiraRespostaHoras', label: 'Prazo 1ª resposta (h)', field: 'prazoPrimeiraRespostaHoras', align: 'left', sortable: true },
  { name: 'prazoResolucaoHoras', label: 'Prazo resolução (h)', field: 'prazoResolucaoHoras', align: 'left', sortable: true },
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

