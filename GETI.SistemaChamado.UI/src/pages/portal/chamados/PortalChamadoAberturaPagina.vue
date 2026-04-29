<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Abertura de Chamado</h1>
      <p>Registre uma nova solicitacao no portal com categoria e servico adequados.</p>
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <q-card flat bordered>
      <q-card-section class="q-gutter-md">
        <q-input
          v-model="formulario.titulo"
          label="Titulo"
          outlined
          maxlength="150"
          :disable="salvando || carregandoCatalogo"
        />

        <q-input
          v-model="formulario.descricao"
          label="Descricao"
          type="textarea"
          outlined
          autogrow
          :disable="salvando || carregandoCatalogo"
        />

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-4">
            <q-select
              v-model="formulario.prioridade"
              :options="prioridades"
              label="Prioridade"
              outlined
              emit-value
              map-options
              :disable="salvando || carregandoCatalogo"
            />
          </div>
          <div class="col-12 col-md-4">
            <q-select
              v-model="formulario.departamentoId"
              :options="catalogo.departamentos"
              label="Departamento"
              option-value="id"
              option-label="nome"
              outlined
              emit-value
              map-options
              :disable="salvando || carregandoCatalogo"
            />
          </div>
          <div class="col-12 col-md-4">
            <q-select
              v-model="formulario.categoriaId"
              :options="catalogo.categorias"
              label="Categoria"
              option-value="id"
              option-label="nome"
              outlined
              emit-value
              map-options
              :disable="salvando || carregandoCatalogo"
            />
          </div>
        </div>

        <q-select
          v-model="formulario.servicoId"
          :options="catalogo.servicos"
          label="Servico"
          option-value="id"
          option-label="nome"
          outlined
          emit-value
          map-options
          :disable="salvando || carregandoCatalogo"
        />
      </q-card-section>

      <q-card-actions align="right">
        <q-btn flat label="Voltar" :to="{ name: 'portal-chamados-lista' }" :disable="salvando" />
        <q-btn color="primary" label="Abrir Chamado" :loading="salvando" @click="salvar" />
      </q-card-actions>
    </q-card>

    <q-banner class="bg-blue-1 text-primary q-mt-md" rounded>
      O envio de anexos ocorre apos a abertura, na tela de detalhe do chamado.
    </q-banner>
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import {
  abrirChamadoPortal,
  consultarCatalogoAberturaChamado,
  type CatalogoAberturaChamadoPortal,
  type AberturaChamadoPortalEntrada
} from '@/services/apiPortal';

type Prioridade = AberturaChamadoPortalEntrada['prioridade'];

const router = useRouter();
const carregandoCatalogo = ref(false);
const salvando = ref(false);
const erro = ref('');
const catalogo = reactive<CatalogoAberturaChamadoPortal>({
  departamentos: [],
  categorias: [],
  servicos: []
});

const formulario = reactive<{
  titulo: string;
  descricao: string;
  prioridade: Prioridade;
  departamentoId: string;
  categoriaId: string;
  servicoId: string;
}>({
  titulo: '',
  descricao: '',
  prioridade: 'MEDIA',
  departamentoId: '',
  categoriaId: '',
  servicoId: ''
});

const prioridades: Array<{ label: string; value: Prioridade }> = [
  { label: 'Baixa', value: 'BAIXA' },
  { label: 'Media', value: 'MEDIA' },
  { label: 'Alta', value: 'ALTA' },
  { label: 'Critica', value: 'CRITICA' }
];

async function carregarCatalogo(): Promise<void> {
  carregandoCatalogo.value = true;
  erro.value = '';
  try {
    const resposta = await consultarCatalogoAberturaChamado();
    catalogo.departamentos = resposta.departamentos;
    catalogo.categorias = resposta.categorias;
    catalogo.servicos = resposta.servicos;
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar catalogos de abertura.';
  } finally {
    carregandoCatalogo.value = false;
  }
}

function validarFormulario(): string | null {
  if (!formulario.titulo.trim()) {
    return 'Informe o titulo do chamado.';
  }
  if (!formulario.descricao.trim()) {
    return 'Informe a descricao do chamado.';
  }
  if (!formulario.departamentoId) {
    return 'Selecione o departamento.';
  }
  if (!formulario.categoriaId) {
    return 'Selecione a categoria.';
  }
  if (!formulario.servicoId) {
    return 'Selecione o servico.';
  }

  return null;
}

async function salvar(): Promise<void> {
  const validacao = validarFormulario();
  if (validacao) {
    erro.value = validacao;
    return;
  }

  salvando.value = true;
  erro.value = '';
  try {
    const chamado = await abrirChamadoPortal({
      titulo: formulario.titulo.trim(),
      descricao: formulario.descricao.trim(),
      prioridade: formulario.prioridade,
      departamentoId: formulario.departamentoId,
      categoriaId: formulario.categoriaId,
      servicoId: formulario.servicoId
    });

    await router.push({ name: 'portal-chamados-detalhe', params: { id: chamado.id } });
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao abrir chamado.';
  } finally {
    salvando.value = false;
  }
}

onMounted(async () => {
  await carregarCatalogo();
});
</script>
