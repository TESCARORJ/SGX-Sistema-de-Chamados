<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Meus Chamados</h1>
        <p>Acompanhe os chamados abertos no portal do solicitante.</p>
      </div>
      <q-btn color="primary" icon="add" label="Abrir Chamado" :to="{ name: 'portal-chamados-abertura' }" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <q-table
      flat
      bordered
      :rows="chamados"
      :columns="colunas"
      row-key="id"
      :loading="carregando"
      no-data-label="Nenhum chamado aberto para o solicitante."
    >
      <template #top-right>
        <q-btn flat dense icon="refresh" label="Atualizar" @click="carregar" />
      </template>

      <template #body-cell-situacao="props">
        <q-td :props="props">
          <q-badge color="primary" text-color="white">{{ props.row.situacao }}</q-badge>
        </q-td>
      </template>

      <template #body-cell-prioridade="props">
        <q-td :props="props">
          <q-badge :color="corPrioridade(props.row.prioridade)" text-color="white">
            {{ props.row.prioridade }}
          </q-badge>
        </q-td>
      </template>

      <template #body-cell-dataCriacao="props">
        <q-td :props="props">{{ formatarData(props.row.dataCriacao) }}</q-td>
      </template>

      <template #body-cell-acoes="props">
        <q-td :props="props" class="text-right">
          <q-btn
            flat
            dense
            color="primary"
            icon="visibility"
            label="Detalhar"
            :to="{ name: 'portal-chamados-detalhe', params: { id: props.row.id } }"
          />
        </q-td>
      </template>
    </q-table>
  </q-page>
</template>

<script setup lang="ts">
import type { QTableColumn } from 'quasar';
import { onMounted, ref } from 'vue';
import { listarChamadosPortal, type ChamadoPortalResumo } from '@/services/apiPortal';

const carregando = ref(false);
const erro = ref('');
const chamados = ref<ChamadoPortalResumo[]>([]);

const colunas: QTableColumn[] = [
  { name: 'numero', label: 'Numero', field: 'numero', align: 'left', sortable: true },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'situacao', label: 'Situacao', field: 'situacao', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'categoria', label: 'Categoria', field: 'categoria', align: 'left', sortable: true },
  { name: 'servico', label: 'Servico', field: 'servico', align: 'left', sortable: true },
  { name: 'dataCriacao', label: 'Abertura', field: 'dataCriacao', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

function corPrioridade(prioridade: string): string {
  if (prioridade === 'CRITICA') {
    return 'negative';
  }
  if (prioridade === 'ALTA') {
    return 'deep-orange';
  }
  if (prioridade === 'MEDIA') {
    return 'warning';
  }
  return 'positive';
}

function formatarData(valor: string): string {
  return new Date(valor).toLocaleString('pt-BR');
}

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    chamados.value = await listarChamadosPortal();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar chamados do solicitante.';
  } finally {
    carregando.value = false;
  }
}

onMounted(async () => {
  await carregar();
});
</script>
