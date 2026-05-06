<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Relatorios Operacionais</h1>
        <p>Visao consolidada por departamento, situacao, prioridade, responsavel e status de SLA.</p>
      </div>
      <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregar" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <q-card flat bordered class="q-mb-md">
      <q-card-section>
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtro.departamentoId"
              :options="catalogo.departamentos"
              option-value="id"
              option-label="nome"
              emit-value
              map-options
              outlined
              label="Departamento"
              clearable
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.situacao"
              :options="opcoesSituacao"
              emit-value
              map-options
              outlined
              label="Situacao"
              clearable
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.prioridade"
              :options="opcoesPrioridade"
              emit-value
              map-options
              outlined
              label="Prioridade"
              clearable
            />
          </div>
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtro.responsavelId"
              :options="catalogo.responsaveis"
              option-value="id"
              option-label="nome"
              emit-value
              map-options
              outlined
              label="Responsavel"
              clearable
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtro.statusSla"
              :options="opcoesStatusSla"
              emit-value
              map-options
              outlined
              label="Status SLA"
              clearable
            />
          </div>
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Limpar" @click="limparFiltros" />
        <q-btn color="primary" label="Aplicar" :loading="carregando" @click="carregar" />
      </q-card-actions>
    </q-card>

    <template v-if="relatorio">
      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section><div class="text-h6">Por Departamento</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in relatorio.porDepartamento" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side><q-badge color="secondary" text-color="white">{{ indicador.total }}</q-badge></q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section><div class="text-h6">Por Situacao</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in relatorio.porSituacao" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side><q-badge color="primary" text-color="white">{{ indicador.total }}</q-badge></q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section><div class="text-h6">Por Prioridade</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in relatorio.porPrioridade" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge :color="corPrioridade(indicador.chave)" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section><div class="text-h6">Por Responsavel</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in relatorio.porResponsavel" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side><q-badge color="teal" text-color="white">{{ indicador.total }}</q-badge></q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section><div class="text-h6">Por Status SLA</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in relatorio.porStatusSla" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge :color="corStatusSla(indicador.chave)" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section><div class="text-h6 text-negative">Chamados Vencidos</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="chamado in relatorio.chamadosVencidosSla" :key="chamado.id" clickable :to="{ name: 'admin-chamados-detalhe', params: { id: chamado.id } }">
                <q-item-section>
                  <q-item-label>{{ chamado.numero }} - {{ chamado.titulo }}</q-item-label>
                  <q-item-label caption>
                    {{ chamado.departamento }} | {{ chamado.responsavel }} | Atraso: {{ chamado.minutosAtrasoSla }} min
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="relatorio.chamadosVencidosSla.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum chamado vencido para os filtros.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section><div class="text-h6 text-warning">Proximos do Vencimento</div></q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="chamado in relatorio.chamadosProximosVencimentoSla" :key="chamado.id" clickable :to="{ name: 'admin-chamados-detalhe', params: { id: chamado.id } }">
                <q-item-section>
                  <q-item-label>{{ chamado.numero }} - {{ chamado.titulo }}</q-item-label>
                  <q-item-label caption>
                    {{ chamado.departamento }} | {{ chamado.responsavel }} | Limite: {{ formatarData(chamado.dataLimiteSla) }}
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="relatorio.chamadosProximosVencimentoSla.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum chamado proximo do vencimento para os filtros.</q-item-label>
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
import {
  consultarCatalogoChamadoAdmin,
  consultarRelatoriosOperacionaisChamadoAdmin,
  type CatalogoAdminChamado,
  type RelatorioOperacionalChamadoAdmin
} from '@/services/apiAdmin';

const carregando = ref(false);
const erro = ref('');
const relatorio = ref<RelatorioOperacionalChamadoAdmin | null>(null);
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
  departamentoId: string | null;
  situacao: string | null;
  prioridade: string | null;
  responsavelId: string | null;
  statusSla: string | null;
}>({
  departamentoId: null,
  situacao: null,
  prioridade: null,
  responsavelId: null,
  statusSla: null
});

const opcoesSituacao = ref<Array<{ label: string; value: string }>>([]);
const opcoesPrioridade = ref<Array<{ label: string; value: string }>>([]);
const opcoesStatusSla = [
  { label: 'Dentro do prazo', value: 'DENTRO_DO_PRAZO' },
  { label: 'Proximo do vencimento', value: 'PROXIMO_DO_VENCIMENTO' },
  { label: 'Vencido', value: 'VENCIDO' }
];

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

function corStatusSla(statusSla: string): string {
  if (statusSla === 'VENCIDO') {
    return 'negative';
  }
  if (statusSla === 'PROXIMO_DO_VENCIMENTO') {
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
}

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    relatorio.value = await consultarRelatoriosOperacionaisChamadoAdmin({
      departamentoId: filtro.departamentoId,
      situacao: filtro.situacao,
      prioridade: filtro.prioridade,
      responsavelId: filtro.responsavelId,
      statusSla: filtro.statusSla
    });
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar relatorios operacionais.';
  } finally {
    carregando.value = false;
  }
}

async function limparFiltros(): Promise<void> {
  filtro.departamentoId = null;
  filtro.situacao = null;
  filtro.prioridade = null;
  filtro.responsavelId = null;
  filtro.statusSla = null;
  await carregar();
}

onMounted(async () => {
  carregando.value = true;
  erro.value = '';
  try {
    await carregarCatalogo();
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao inicializar relatorios operacionais.';
  } finally {
    carregando.value = false;
  }
});
</script>
