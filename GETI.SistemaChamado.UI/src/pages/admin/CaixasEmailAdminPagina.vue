<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Caixas de E-mail</h1>
        <p>Canal institucional de entrada vinculado obrigatoriamente a um departamento.</p>
      </div>
      <q-btn color="primary" icon="add" label="Nova Caixa" @click="abrirCriacao" />
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
      no-data-label="Nenhuma caixa de e-mail cadastrada."
    >
      <template #body-cell-ativa="props">
        <q-td :props="props">
          <q-badge :color="props.row.ativa ? 'positive' : 'grey-6'" text-color="white">
            {{ props.row.ativa ? 'Ativa' : 'Inativa' }}
          </q-badge>
        </q-td>
      </template>

      <template #body-cell-acoes="props">
        <q-td :props="props" class="q-gutter-xs">
          <q-btn flat dense color="primary" icon="edit" @click="abrirEdicao(props.row)" />
          <q-btn
            v-if="props.row.ativa"
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
          <div class="text-h6">{{ modoEdicao ? 'Editar Caixa de E-mail' : 'Nova Caixa de E-mail' }}</div>
        </q-card-section>

        <q-card-section class="q-gutter-md">
          <q-input
            v-model="formulario.enderecoEmail"
            label="Endereco de e-mail"
            outlined
            maxlength="255"
            :disable="salvando"
          />
          <q-input
            v-model="formulario.nomeExibicao"
            label="Nome de exibicao"
            outlined
            maxlength="150"
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
  atualizarCaixaEmail,
  criarCaixaEmail,
  inativarCaixaEmail,
  listarCaixasEmail,
  listarDepartamentos,
  type CaixaDeEmailAdministrativa,
  type DepartamentoAdministrativo
} from '@/services/apiAdmin';

const carregando = ref(false);
const salvando = ref(false);
const erro = ref('');
const registros = ref<CaixaDeEmailAdministrativa[]>([]);
const departamentos = ref<DepartamentoAdministrativo[]>([]);

const dialogoAberto = ref(false);
const modoEdicao = ref(false);
const registroEdicaoId = ref<string | null>(null);
const formulario = reactive({
  enderecoEmail: '',
  nomeExibicao: '',
  departamentoId: ''
});

const colunas: QTableColumn[] = [
  { name: 'nomeExibicao', label: 'Nome de exibicao', field: 'nomeExibicao', align: 'left', sortable: true },
  { name: 'enderecoEmail', label: 'E-mail', field: 'enderecoEmail', align: 'left', sortable: true },
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left', sortable: true },
  { name: 'ativa', label: 'Situacao', field: 'ativa', align: 'left', sortable: true },
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
    const [caixas, departamentosCadastrados] = await Promise.all([
      listarCaixasEmail(),
      listarDepartamentos()
    ]);
    registros.value = caixas;
    departamentos.value = departamentosCadastrados;
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar caixas de e-mail.';
  } finally {
    carregando.value = false;
  }
}

function abrirCriacao(): void {
  modoEdicao.value = false;
  registroEdicaoId.value = null;
  formulario.enderecoEmail = '';
  formulario.nomeExibicao = '';
  formulario.departamentoId = opcoesDepartamento.value[0]?.value ?? '';
  dialogoAberto.value = true;
}

function abrirEdicao(registro: CaixaDeEmailAdministrativa): void {
  modoEdicao.value = true;
  registroEdicaoId.value = registro.id;
  formulario.enderecoEmail = registro.enderecoEmail;
  formulario.nomeExibicao = registro.nomeExibicao;
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
  const enderecoEmail = formulario.enderecoEmail.trim();
  const nomeExibicao = formulario.nomeExibicao.trim();
  const departamentoId = formulario.departamentoId;

  if (!enderecoEmail || !nomeExibicao || !departamentoId) {
    erro.value = 'Preencha endereco de e-mail, nome de exibicao e departamento.';
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    if (modoEdicao.value && registroEdicaoId.value) {
      await atualizarCaixaEmail(registroEdicaoId.value, enderecoEmail, nomeExibicao, departamentoId);
    } else {
      await criarCaixaEmail(enderecoEmail, nomeExibicao, departamentoId);
    }
    dialogoAberto.value = false;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao salvar caixa de e-mail.';
  } finally {
    salvando.value = false;
  }
}

async function inativar(id: string): Promise<void> {
  if (!window.confirm('Confirma a inativacao desta caixa de e-mail?')) {
    return;
  }

  erro.value = '';
  try {
    await inativarCaixaEmail(id);
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inativar caixa de e-mail.';
  }
}

onMounted(async () => {
  await carregar();
});
</script>
