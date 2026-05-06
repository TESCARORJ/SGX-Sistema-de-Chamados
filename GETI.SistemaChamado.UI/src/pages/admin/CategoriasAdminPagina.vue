<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Categorias</h1>
        <p>Classificacao administrativa base para organizacao do catalogo de servicos.</p>
      </div>
      <q-btn color="primary" icon="add" label="Nova Categoria" @click="abrirCriacao" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <q-table
      flat
      bordered
      :rows="registros"
      :columns="colunas"
      row-key="id"
      :loading="carregando"
      no-data-label="Nenhuma categoria cadastrada."
    >
      <template #body-cell-ativo="props">
        <q-td :props="props">
          <q-badge :color="props.row.ativo ? 'positive' : 'grey-6'" text-color="white">
            {{ props.row.ativo ? 'Ativa' : 'Inativa' }}
          </q-badge>
        </q-td>
      </template>

      <template #body-cell-acoes="props">
        <q-td :props="props" class="q-gutter-xs">
          <q-btn flat dense color="primary" icon="edit" @click="abrirEdicao(props.row)" />
          <q-btn
            v-if="props.row.ativo"
            flat
            dense
            color="negative"
            icon="block"
            @click="inativar(props.row.id)"
          />
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="dialogoAberto" persistent>
      <q-card style="min-width: 520px; max-width: 90vw;">
        <q-card-section>
          <div class="text-h6">{{ modoEdicao ? 'Editar Categoria' : 'Nova Categoria' }}</div>
        </q-card-section>

        <q-card-section class="q-gutter-md">
          <q-input
            v-model="formulario.nome"
            label="Nome"
            outlined
            maxlength="120"
            :disable="salvando"
          />
          <q-input
            v-model="formulario.descricao"
            label="Descricao"
            outlined
            maxlength="255"
            type="textarea"
            autogrow
            :disable="salvando"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" :disable="salvando" @click="fecharDialogo" />
          <q-btn color="primary" label="Salvar" :loading="salvando" @click="salvar" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup lang="ts">
import type { QTableColumn } from 'quasar';
import { onMounted, reactive, ref } from 'vue';
import {
  atualizarCategoria,
  criarCategoria,
  inativarCategoria,
  listarCategorias,
  type CategoriaAdministrativa
} from '@/services/apiAdmin';

const carregando = ref(false);
const salvando = ref(false);
const erro = ref('');
const registros = ref<CategoriaAdministrativa[]>([]);

const dialogoAberto = ref(false);
const modoEdicao = ref(false);
const registroEdicaoId = ref<string | null>(null);
const formulario = reactive({
  nome: '',
  descricao: ''
});

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'descricao', label: 'Descricao', field: 'descricao', align: 'left' },
  { name: 'ativo', label: 'Situacao', field: 'ativo', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    registros.value = await listarCategorias();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar categorias.';
  } finally {
    carregando.value = false;
  }
}

function abrirCriacao(): void {
  modoEdicao.value = false;
  registroEdicaoId.value = null;
  formulario.nome = '';
  formulario.descricao = '';
  dialogoAberto.value = true;
}

function abrirEdicao(registro: CategoriaAdministrativa): void {
  modoEdicao.value = true;
  registroEdicaoId.value = registro.id;
  formulario.nome = registro.nome;
  formulario.descricao = registro.descricao ?? '';
  dialogoAberto.value = true;
}

function fecharDialogo(): void {
  if (salvando.value) {
    return;
  }
  dialogoAberto.value = false;
}

async function salvar(): Promise<void> {
  const nome = formulario.nome.trim();
  const descricao = formulario.descricao.trim() || null;
  if (!nome) {
    erro.value = 'Informe o nome da categoria.';
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    if (modoEdicao.value && registroEdicaoId.value) {
      await atualizarCategoria(registroEdicaoId.value, nome, descricao);
    } else {
      await criarCategoria(nome, descricao);
    }
    dialogoAberto.value = false;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao salvar categoria.';
  } finally {
    salvando.value = false;
  }
}

async function inativar(id: string): Promise<void> {
  if (!window.confirm('Confirma a inativacao desta categoria?')) {
    return;
  }

  erro.value = '';
  try {
    await inativarCategoria(id);
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inativar categoria.';
  }
}

onMounted(async () => {
  await carregar();
});
</script>
