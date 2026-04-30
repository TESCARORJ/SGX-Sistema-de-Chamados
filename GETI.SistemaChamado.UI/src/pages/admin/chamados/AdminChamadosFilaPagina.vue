<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Fila Administrativa</h1>
        <p>Operacao dos chamados com filtros por situacao, prioridade, origem, departamento e responsavel.</p>
      </div>
      <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregarFila" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <q-card flat bordered class="q-mb-md">
      <q-card-section>
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.situacao"
              :options="opcoesSituacao"
              label="Situacao"
              outlined
              clearable
              emit-value
              map-options
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.prioridade"
              :options="opcoesPrioridade"
              label="Prioridade"
              outlined
              clearable
              emit-value
              map-options
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.origem"
              :options="opcoesOrigem"
              label="Origem"
              outlined
              clearable
              emit-value
              map-options
            />
          </div>
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtro.departamentoId"
              :options="catalogo.departamentos"
              label="Departamento"
              option-value="id"
              option-label="nome"
              outlined
              clearable
              emit-value
              map-options
            />
          </div>
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtro.responsavelId"
              :options="catalogo.responsaveis"
              label="Responsavel"
              option-value="id"
              option-label="nome"
              outlined
              clearable
              emit-value
              map-options
            />
          </div>
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Limpar Filtros" @click="limparFiltros" />
        <q-btn color="primary" label="Aplicar Filtros" :loading="carregando" @click="carregarFila" />
      </q-card-actions>
    </q-card>

    <q-table
      flat
      bordered
      :rows="chamados"
      :columns="colunas"
      row-key="id"
      :loading="carregando"
      no-data-label="Nenhum chamado encontrado para os filtros informados."
    >
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
            :to="{ name: 'admin-chamados-detalhe', params: { id: props.row.id } }"
          />
        </q-td>
      </template>
    </q-table>
  </q-page>
</template>

<script setup lang="ts">
import type { QTableColumn } from 'quasar';
import { onMounted, reactive, ref } from 'vue';
import {
  consultarCatalogoChamadoAdmin,
  listarFilaChamadoAdmin,
  type CatalogoAdminChamado,
  type ChamadoFilaAdmin
} from '@/services/apiAdmin';

const carregando = ref(false);
const erro = ref('');
const chamados = ref<ChamadoFilaAdmin[]>([]);
const catalogo = reactive<CatalogoAdminChamado>({
  departamentos: [],
  categorias: [],
  servicos: [],
  responsaveis: [],
  situacoes: [],
  prioridades: [],
  origens: []
});

const filtro = reactive<{
  situacao: string | null;
  prioridade: string | null;
  departamentoId: string | null;
  origem: string | null;
  responsavelId: string | null;
}>({
  situacao: null,
  prioridade: null,
  departamentoId: null,
  origem: null,
  responsavelId: null
});

const colunas: QTableColumn[] = [
  { name: 'numero', label: 'Numero', field: 'numero', align: 'left', sortable: true },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'situacao', label: 'Situacao', field: 'situacao', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'origem', label: 'Origem', field: 'origem', align: 'left', sortable: true },
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left', sortable: true },
  { name: 'responsavelNome', label: 'Responsavel', field: 'responsavelNome', align: 'left', sortable: true },
  { name: 'dataCriacao', label: 'Abertura', field: 'dataCriacao', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

const opcoesSituacao = ref<Array<{ label: string; value: string }>>([]);
const opcoesPrioridade = ref<Array<{ label: string; value: string }>>([]);
const opcoesOrigem = ref<Array<{ label: string; value: string }>>([]);

function formatarData(valor: string): string {
  return new Date(valor).toLocaleString('pt-BR');
}

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

async function carregarCatalogo(): Promise<void> {
  const resposta = await consultarCatalogoChamadoAdmin();
  catalogo.departamentos = resposta.departamentos;
  catalogo.categorias = resposta.categorias;
  catalogo.servicos = resposta.servicos;
  catalogo.responsaveis = resposta.responsaveis;
  catalogo.situacoes = resposta.situacoes;
  catalogo.prioridades = resposta.prioridades;
  catalogo.origens = resposta.origens;

  opcoesSituacao.value = resposta.situacoes.map((item) => ({ label: item, value: item }));
  opcoesPrioridade.value = resposta.prioridades.map((item) => ({ label: item, value: item }));
  opcoesOrigem.value = resposta.origens.map((item) => ({ label: item, value: item }));
}

async function carregarFila(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    chamados.value = await listarFilaChamadoAdmin({
      situacao: filtro.situacao,
      prioridade: filtro.prioridade,
      departamentoId: filtro.departamentoId,
      origem: filtro.origem,
      responsavelId: filtro.responsavelId
    });
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar fila administrativa.';
  } finally {
    carregando.value = false;
  }
}

async function limparFiltros(): Promise<void> {
  filtro.situacao = null;
  filtro.prioridade = null;
  filtro.departamentoId = null;
  filtro.origem = null;
  filtro.responsavelId = null;
  await carregarFila();
}

onMounted(async () => {
  carregando.value = true;
  erro.value = '';
  try {
    await carregarCatalogo();
    await carregarFila();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar dados administrativos de chamados.';
  } finally {
    carregando.value = false;
  }
});
</script>
