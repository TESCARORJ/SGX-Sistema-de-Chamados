<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Departamentos</h1>
        <p>Cadastro mestre organizacional para vinculo de caixas e grupos de atendimento.</p>
      </div>
      <q-btn color="primary" icon="add" label="Novo Departamento" @click="abrirCriacao" />
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
      no-data-label="Nenhum departamento cadastrado."
    >
      <template #body-cell-ativo="props">
        <q-td :props="props">
          <q-badge :color="props.row.ativo ? 'positive' : 'grey-6'" text-color="white">
            {{ props.row.ativo ? 'Ativo' : 'Inativo' }}
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
      <q-card style="min-width: 420px; max-width: 90vw;">
        <q-card-section>
          <div class="text-h6">{{ modoEdicao ? 'Editar Departamento' : 'Novo Departamento' }}</div>
        </q-card-section>

        <q-card-section>
          <q-input
            v-model="formulario.nome"
            label="Nome"
            outlined
            maxlength="150"
            autofocus
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
  atualizarDepartamento,
  criarDepartamento,
  inativarDepartamento,
  listarDepartamentos,
  type DepartamentoAdministrativo
} from '@/services/apiAdmin';

const carregando = ref(false);
const salvando = ref(false);
const erro = ref('');
const registros = ref<DepartamentoAdministrativo[]>([]);

const dialogoAberto = ref(false);
const modoEdicao = ref(false);
const registroEdicaoId = ref<string | null>(null);
const formulario = reactive({
  nome: ''
});

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'ativo', label: 'Situacao', field: 'ativo', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    registros.value = await listarDepartamentos();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar departamentos.';
  } finally {
    carregando.value = false;
  }
}

function abrirCriacao(): void {
  modoEdicao.value = false;
  registroEdicaoId.value = null;
  formulario.nome = '';
  dialogoAberto.value = true;
}

function abrirEdicao(registro: DepartamentoAdministrativo): void {
  modoEdicao.value = true;
  registroEdicaoId.value = registro.id;
  formulario.nome = registro.nome;
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
  if (!nome) {
    erro.value = 'Informe o nome do departamento.';
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    if (modoEdicao.value && registroEdicaoId.value) {
      await atualizarDepartamento(registroEdicaoId.value, nome);
    } else {
      await criarDepartamento(nome);
    }
    dialogoAberto.value = false;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao salvar departamento.';
  } finally {
    salvando.value = false;
  }
}

async function inativar(id: string): Promise<void> {
  if (!window.confirm('Confirma a inativacao deste departamento?')) {
    return;
  }

  erro.value = '';
  try {
    await inativarDepartamento(id);
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inativar departamento.';
  }
}

onMounted(async () => {
  await carregar();
});
</script>
