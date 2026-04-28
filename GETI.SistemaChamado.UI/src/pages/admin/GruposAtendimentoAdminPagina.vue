<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Grupos de Atendimento</h1>
        <p>Estrutura administrativa de atendimento vinculada a um departamento.</p>
      </div>
      <q-btn color="primary" icon="add" label="Novo Grupo" @click="abrirCriacao" />
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
      no-data-label="Nenhum grupo de atendimento cadastrado."
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
      <q-card style="min-width: 520px; max-width: 90vw;">
        <q-card-section>
          <div class="text-h6">{{ modoEdicao ? 'Editar Grupo de Atendimento' : 'Novo Grupo de Atendimento' }}</div>
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
          <q-select
            v-model="formulario.departamentoId"
            :options="opcoesDepartamento"
            emit-value
            map-options
            outlined
            label="Departamento"
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
import { computed, onMounted, reactive, ref } from 'vue';
import {
  atualizarGrupoAtendimento,
  criarGrupoAtendimento,
  inativarGrupoAtendimento,
  listarDepartamentos,
  listarGruposAtendimento,
  type DepartamentoAdministrativo,
  type GrupoAtendimentoAdministrativo
} from '@/services/apiAdmin';

const carregando = ref(false);
const salvando = ref(false);
const erro = ref('');
const registros = ref<GrupoAtendimentoAdministrativo[]>([]);
const departamentos = ref<DepartamentoAdministrativo[]>([]);

const dialogoAberto = ref(false);
const modoEdicao = ref(false);
const registroEdicaoId = ref<string | null>(null);
const formulario = reactive({
  nome: '',
  descricao: '',
  departamentoId: ''
});

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left', sortable: true },
  { name: 'descricao', label: 'Descricao', field: 'descricao', align: 'left' },
  { name: 'ativo', label: 'Situacao', field: 'ativo', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

const opcoesDepartamento = computed(() =>
  departamentos.value
    .filter((departamento) => departamento.ativo)
    .map((departamento) => ({ label: departamento.nome, value: departamento.id }))
);

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    const [grupos, departamentosCadastrados] = await Promise.all([
      listarGruposAtendimento(),
      listarDepartamentos()
    ]);
    registros.value = grupos;
    departamentos.value = departamentosCadastrados;
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar grupos de atendimento.';
  } finally {
    carregando.value = false;
  }
}

function abrirCriacao(): void {
  modoEdicao.value = false;
  registroEdicaoId.value = null;
  formulario.nome = '';
  formulario.descricao = '';
  formulario.departamentoId = opcoesDepartamento.value[0]?.value ?? '';
  dialogoAberto.value = true;
}

function abrirEdicao(registro: GrupoAtendimentoAdministrativo): void {
  modoEdicao.value = true;
  registroEdicaoId.value = registro.id;
  formulario.nome = registro.nome;
  formulario.descricao = registro.descricao ?? '';
  formulario.departamentoId = registro.departamentoId;
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
  const departamentoId = formulario.departamentoId;

  if (!nome || !departamentoId) {
    erro.value = 'Informe nome e departamento do grupo de atendimento.';
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    if (modoEdicao.value && registroEdicaoId.value) {
      await atualizarGrupoAtendimento(registroEdicaoId.value, nome, descricao, departamentoId);
    } else {
      await criarGrupoAtendimento(nome, descricao, departamentoId);
    }
    dialogoAberto.value = false;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao salvar grupo de atendimento.';
  } finally {
    salvando.value = false;
  }
}

async function inativar(id: string): Promise<void> {
  if (!window.confirm('Confirma a inativacao deste grupo de atendimento?')) {
    return;
  }

  erro.value = '';
  try {
    await inativarGrupoAtendimento(id);
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inativar grupo de atendimento.';
  }
}

onMounted(async () => {
  await carregar();
});
</script>
