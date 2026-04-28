<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Servicos</h1>
        <p>Catalogo administrativo de servicos vinculado a categoria e departamento.</p>
      </div>
      <q-btn color="primary" icon="add" label="Novo Servico" @click="abrirCriacao" />
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
      no-data-label="Nenhum servico cadastrado."
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
      <q-card style="min-width: 580px; max-width: 92vw;">
        <q-card-section>
          <div class="text-h6">{{ modoEdicao ? 'Editar Servico' : 'Novo Servico' }}</div>
        </q-card-section>

        <q-card-section class="q-gutter-md">
          <q-input
            v-model="formulario.nome"
            label="Nome"
            outlined
            maxlength="150"
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
            v-model="formulario.categoriaId"
            :options="opcoesCategoria"
            emit-value
            map-options
            outlined
            label="Categoria"
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
  atualizarServico,
  criarServico,
  inativarServico,
  listarCategorias,
  listarDepartamentos,
  listarServicos,
  type CategoriaAdministrativa,
  type DepartamentoAdministrativo,
  type ServicoAdministrativo
} from '@/services/apiAdmin';

const carregando = ref(false);
const salvando = ref(false);
const erro = ref('');
const registros = ref<ServicoAdministrativo[]>([]);
const categorias = ref<CategoriaAdministrativa[]>([]);
const departamentos = ref<DepartamentoAdministrativo[]>([]);

const dialogoAberto = ref(false);
const modoEdicao = ref(false);
const registroEdicaoId = ref<string | null>(null);
const formulario = reactive({
  nome: '',
  descricao: '',
  categoriaId: '',
  departamentoId: ''
});

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'categoriaNome', label: 'Categoria', field: 'categoriaNome', align: 'left', sortable: true },
  { name: 'departamentoNome', label: 'Departamento', field: 'departamentoNome', align: 'left', sortable: true },
  { name: 'ativo', label: 'Situacao', field: 'ativo', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

const opcoesCategoria = computed(() =>
  categorias.value
    .filter((categoria) => categoria.ativo)
    .map((categoria) => ({ label: categoria.nome, value: categoria.id }))
);

const opcoesDepartamento = computed(() =>
  departamentos.value
    .filter((departamento) => departamento.ativo)
    .map((departamento) => ({ label: departamento.nome, value: departamento.id }))
);

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    const [servicos, categoriasCadastradas, departamentosCadastrados] = await Promise.all([
      listarServicos(),
      listarCategorias(),
      listarDepartamentos()
    ]);
    registros.value = servicos;
    categorias.value = categoriasCadastradas;
    departamentos.value = departamentosCadastrados;
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar servicos.';
  } finally {
    carregando.value = false;
  }
}

function abrirCriacao(): void {
  modoEdicao.value = false;
  registroEdicaoId.value = null;
  formulario.nome = '';
  formulario.descricao = '';
  formulario.categoriaId = opcoesCategoria.value[0]?.value ?? '';
  formulario.departamentoId = opcoesDepartamento.value[0]?.value ?? '';
  dialogoAberto.value = true;
}

function abrirEdicao(registro: ServicoAdministrativo): void {
  modoEdicao.value = true;
  registroEdicaoId.value = registro.id;
  formulario.nome = registro.nome;
  formulario.descricao = registro.descricao ?? '';
  formulario.categoriaId = registro.categoriaId;
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
  const categoriaId = formulario.categoriaId;
  const departamentoId = formulario.departamentoId;

  if (!nome || !categoriaId || !departamentoId) {
    erro.value = 'Informe nome, categoria e departamento do servico.';
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    if (modoEdicao.value && registroEdicaoId.value) {
      await atualizarServico(registroEdicaoId.value, nome, descricao, categoriaId, departamentoId);
    } else {
      await criarServico(nome, descricao, categoriaId, departamentoId);
    }
    dialogoAberto.value = false;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao salvar servico.';
  } finally {
    salvando.value = false;
  }
}

async function inativar(id: string): Promise<void> {
  if (!window.confirm('Confirma a inativacao deste servico?')) {
    return;
  }

  erro.value = '';
  try {
    await inativarServico(id);
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inativar servico.';
  }
}

onMounted(async () => {
  await carregar();
});
</script>
