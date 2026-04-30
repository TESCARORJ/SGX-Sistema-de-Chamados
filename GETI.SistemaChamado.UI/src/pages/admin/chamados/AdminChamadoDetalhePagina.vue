<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Detalhe Administrativo</h1>
        <p v-if="chamado">{{ chamado.numero }} - {{ chamado.titulo }}</p>
      </div>
      <div class="q-gutter-sm">
        <q-btn flat icon="arrow_back" label="Voltar para Fila" :to="{ name: 'admin-chamados-fila' }" />
        <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregarTudo" />
      </div>
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <template v-if="chamado">
      <q-card flat bordered class="q-mb-md">
        <q-card-section>
          <div class="row q-col-gutter-md">
            <div class="col-12 col-md-4"><strong>Situacao:</strong> {{ chamado.situacao }}</div>
            <div class="col-12 col-md-4"><strong>Prioridade:</strong> {{ chamado.prioridade }}</div>
            <div class="col-12 col-md-4"><strong>Origem:</strong> {{ chamado.origem }}</div>
            <div class="col-12 col-md-4"><strong>Solicitante:</strong> {{ chamado.solicitanteNome }}</div>
            <div class="col-12 col-md-4"><strong>Login:</strong> {{ chamado.solicitanteLogin }}</div>
            <div class="col-12 col-md-4"><strong>E-mail:</strong> {{ chamado.solicitanteEmail }}</div>
            <div class="col-12 col-md-4"><strong>Departamento:</strong> {{ chamado.departamentoNome }}</div>
            <div class="col-12 col-md-4"><strong>Categoria:</strong> {{ chamado.categoriaNome }}</div>
            <div class="col-12 col-md-4"><strong>Servico:</strong> {{ chamado.servicoNome }}</div>
            <div class="col-12 col-md-4">
              <strong>Responsavel:</strong> {{ chamado.responsavelNome ?? 'Sem responsavel' }}
            </div>
            <div class="col-12"><strong>Descricao:</strong> {{ chamado.descricao }}</div>
          </div>
        </q-card-section>
      </q-card>

      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Atribuicao</div>
            </q-card-section>
            <q-card-section class="row q-col-gutter-sm items-end">
              <div class="col">
                <q-select
                  v-model="atribuicao.responsavelId"
                  :options="catalogo.responsaveis"
                  option-value="id"
                  option-label="nome"
                  emit-value
                  map-options
                  outlined
                  label="Responsavel"
                  :disable="processando"
                />
              </div>
              <div class="col-auto">
                <q-btn color="primary" label="Atribuir" :loading="processando" @click="salvarAtribuicao" />
              </div>
            </q-card-section>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Alteracao de Situacao</div>
            </q-card-section>
            <q-card-section class="row q-col-gutter-sm items-end">
              <div class="col">
                <q-select
                  v-model="situacao.nova"
                  :options="opcoesSituacao"
                  emit-value
                  map-options
                  outlined
                  label="Nova situacao"
                  :disable="processando"
                />
              </div>
              <div class="col-auto">
                <q-btn color="primary" label="Alterar" :loading="processando" @click="salvarSituacao" />
              </div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <q-card flat bordered class="q-mb-md">
        <q-card-section>
          <div class="text-h6">Encaminhamento</div>
        </q-card-section>
        <q-card-section class="row q-col-gutter-md items-end">
          <div class="col-12 col-md-4">
            <q-select
              v-model="encaminhamento.departamentoId"
              :options="catalogo.departamentos"
              option-value="id"
              option-label="nome"
              emit-value
              map-options
              outlined
              label="Departamento"
              :disable="processando"
            />
          </div>
          <div class="col-12 col-md-4">
            <q-select
              v-model="encaminhamento.categoriaId"
              :options="catalogo.categorias"
              option-value="id"
              option-label="nome"
              emit-value
              map-options
              outlined
              label="Categoria"
              :disable="processando"
            />
          </div>
          <div class="col-12 col-md-4">
            <q-select
              v-model="encaminhamento.servicoId"
              :options="catalogo.servicos"
              option-value="id"
              option-label="nome"
              emit-value
              map-options
              outlined
              label="Servico"
              :disable="processando"
            />
          </div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn color="primary" label="Encaminhar" :loading="processando" @click="salvarEncaminhamento" />
        </q-card-actions>
      </q-card>

      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Comentario Publico</div>
            </q-card-section>
            <q-card-section>
              <q-input
                v-model="comentarioPublico"
                type="textarea"
                autogrow
                outlined
                label="Mensagem visivel ao solicitante"
                :disable="processando"
              />
            </q-card-section>
            <q-card-actions align="right">
              <q-btn color="primary" label="Publicar" :loading="processando" @click="enviarComentarioPublico" />
            </q-card-actions>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Comentario Interno</div>
            </q-card-section>
            <q-card-section>
              <q-input
                v-model="comentarioInterno"
                type="textarea"
                autogrow
                outlined
                label="Mensagem interna da equipe"
                :disable="processando"
              />
            </q-card-section>
            <q-card-actions align="right">
              <q-btn color="primary" label="Registrar" :loading="processando" @click="enviarComentarioInterno" />
            </q-card-actions>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Interacoes</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="interacao in chamado.interacoes" :key="interacao.id">
                <q-item-section>
                  <q-item-label>{{ interacao.mensagem }}</q-item-label>
                  <q-item-label caption>
                    {{ interacao.tipoInteracao }} por {{ interacao.autorNome }} em
                    {{ formatarData(interacao.dataCriacao) }}
                  </q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge
                    :color="interacao.visivelSolicitante ? 'positive' : 'grey-7'"
                    text-color="white"
                  >
                    {{ interacao.visivelSolicitante ? 'Publico' : 'Interno' }}
                  </q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Historico</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="historico in chamado.historicos" :key="historico.id">
                <q-item-section>
                  <q-item-label>{{ historico.descricao }}</q-item-label>
                  <q-item-label caption>
                    {{ historico.situacaoAnterior ?? 'N/A' }} -> {{ historico.situacaoNova }} em
                    {{ formatarData(historico.dataCriacao) }}
                  </q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge
                    :color="historico.visivelSolicitante ? 'positive' : 'grey-7'"
                    text-color="white"
                  >
                    {{ historico.visivelSolicitante ? 'Publico' : 'Interno' }}
                  </q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Anexos</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="anexo in chamado.anexos" :key="anexo.id">
                <q-item-section>
                  <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                  <q-item-label caption>
                    {{ anexo.tipoConteudo }} - {{ formatarBytes(anexo.tamanhoBytes) }} - {{ anexo.autorNome }}
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="chamado.anexos.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum anexo registrado.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>
    </template>
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router';
import {
  alterarSituacaoChamadoAdmin,
  atribuirChamadoAdmin,
  comentarInternamenteChamadoAdmin,
  comentarPublicamenteChamadoAdmin,
  consultarCatalogoChamadoAdmin,
  detalharChamadoAdmin,
  encaminharChamadoAdmin,
  type CatalogoAdminChamado,
  type ChamadoDetalheAdmin
} from '@/services/apiAdmin';

const route = useRoute();
const carregando = ref(false);
const processando = ref(false);
const erro = ref('');
const chamado = ref<ChamadoDetalheAdmin | null>(null);
const catalogo = reactive<CatalogoAdminChamado>({
  departamentos: [],
  categorias: [],
  servicos: [],
  responsaveis: [],
  situacoes: [],
  prioridades: [],
  origens: []
});

const atribuicao = reactive({
  responsavelId: '' as string | null
});

const situacao = reactive({
  nova: '' as string | null
});

const encaminhamento = reactive({
  departamentoId: '' as string | null,
  categoriaId: '' as string | null,
  servicoId: '' as string | null
});

const comentarioPublico = ref('');
const comentarioInterno = ref('');
const opcoesSituacao = ref<Array<{ label: string; value: string }>>([]);

function obterIdChamado(): string {
  const id = route.params.id;
  if (typeof id !== 'string' || !id) {
    throw new Error('Identificador do chamado invalido.');
  }
  return id;
}

function formatarData(valor: string): string {
  return new Date(valor).toLocaleString('pt-BR');
}

function formatarBytes(valor: number): string {
  if (valor < 1024) {
    return `${valor} B`;
  }
  if (valor < 1024 * 1024) {
    return `${(valor / 1024).toFixed(1)} KB`;
  }
  return `${(valor / (1024 * 1024)).toFixed(1)} MB`;
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
}

async function carregarDetalhe(): Promise<void> {
  chamado.value = await detalharChamadoAdmin(obterIdChamado());
  atribuicao.responsavelId = chamado.value.responsavelId;
  situacao.nova = chamado.value.situacao;
  encaminhamento.departamentoId = chamado.value.departamentoId;
  encaminhamento.categoriaId = chamado.value.categoriaId;
  encaminhamento.servicoId = chamado.value.servicoId;
}

async function carregarTudo(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    await carregarCatalogo();
    await carregarDetalhe();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar detalhe administrativo do chamado.';
  } finally {
    carregando.value = false;
  }
}

async function salvarAtribuicao(): Promise<void> {
  if (!atribuicao.responsavelId) {
    erro.value = 'Selecione um responsavel para atribuir.';
    return;
  }
  processando.value = true;
  erro.value = '';
  try {
    chamado.value = await atribuirChamadoAdmin(obterIdChamado(), atribuicao.responsavelId);
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao atribuir chamado.';
  } finally {
    processando.value = false;
  }
}

async function salvarSituacao(): Promise<void> {
  if (!situacao.nova) {
    erro.value = 'Selecione a nova situacao.';
    return;
  }
  processando.value = true;
  erro.value = '';
  try {
    chamado.value = await alterarSituacaoChamadoAdmin(obterIdChamado(), situacao.nova);
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao alterar situacao do chamado.';
  } finally {
    processando.value = false;
  }
}

async function salvarEncaminhamento(): Promise<void> {
  if (!encaminhamento.departamentoId || !encaminhamento.categoriaId || !encaminhamento.servicoId) {
    erro.value = 'Informe departamento, categoria e servico para encaminhamento.';
    return;
  }
  processando.value = true;
  erro.value = '';
  try {
    chamado.value = await encaminharChamadoAdmin(
      obterIdChamado(),
      encaminhamento.departamentoId,
      encaminhamento.categoriaId,
      encaminhamento.servicoId
    );
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao encaminhar chamado.';
  } finally {
    processando.value = false;
  }
}

async function enviarComentarioPublico(): Promise<void> {
  const mensagem = comentarioPublico.value.trim();
  if (!mensagem) {
    erro.value = 'Informe a mensagem do comentario publico.';
    return;
  }
  processando.value = true;
  erro.value = '';
  try {
    chamado.value = await comentarPublicamenteChamadoAdmin(obterIdChamado(), mensagem);
    comentarioPublico.value = '';
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao registrar comentario publico.';
  } finally {
    processando.value = false;
  }
}

async function enviarComentarioInterno(): Promise<void> {
  const mensagem = comentarioInterno.value.trim();
  if (!mensagem) {
    erro.value = 'Informe a mensagem do comentario interno.';
    return;
  }
  processando.value = true;
  erro.value = '';
  try {
    chamado.value = await comentarInternamenteChamadoAdmin(obterIdChamado(), mensagem);
    comentarioInterno.value = '';
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao registrar comentario interno.';
  } finally {
    processando.value = false;
  }
}

onMounted(async () => {
  await carregarTudo();
});
</script>
