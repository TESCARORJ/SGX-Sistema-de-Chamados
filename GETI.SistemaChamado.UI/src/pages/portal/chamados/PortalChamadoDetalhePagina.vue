<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Detalhe do Chamado</h1>
        <p v-if="chamado">{{ chamado.numero }} - {{ chamado.titulo }}</p>
      </div>
      <div class="q-gutter-sm">
        <q-btn flat icon="arrow_back" label="Voltar" :to="{ name: 'portal-chamados-lista' }" />
        <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregar" />
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
            <div class="col-12 col-md-4"><strong>Departamento:</strong> {{ chamado.departamento }}</div>
            <div class="col-12 col-md-4"><strong>Categoria:</strong> {{ chamado.categoria }}</div>
            <div class="col-12 col-md-4"><strong>Servico:</strong> {{ chamado.servico }}</div>
            <div class="col-12"><strong>Descricao:</strong> {{ chamado.descricao }}</div>
          </div>
        </q-card-section>
      </q-card>

      <q-card flat bordered class="q-mb-md">
        <q-card-section>
          <div class="text-h6">Anexos</div>
        </q-card-section>
        <q-separator />
        <q-card-section>
          <div class="row q-col-gutter-sm items-center q-mb-sm">
            <div class="col">
              <q-file
                v-model="arquivoSelecionado"
                outlined
                label="Selecionar arquivo"
                clearable
                :disable="enviandoAnexo"
                max-file-size="10485760"
              />
            </div>
            <div class="col-auto">
              <q-btn color="primary" label="Enviar" :loading="enviandoAnexo" @click="enviarAnexo" />
            </div>
          </div>

          <q-list bordered separator>
            <q-item v-for="anexo in chamado.anexos" :key="anexo.id">
              <q-item-section>
                <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                <q-item-label caption>
                  {{ anexo.tipoConteudo }} - {{ formatarBytes(anexo.tamanhoBytes) }} - {{ formatarData(anexo.dataCriacao) }}
                </q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="chamado.anexos.length === 0">
              <q-item-section>
                <q-item-label caption>Nenhum anexo enviado.</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </q-card-section>
      </q-card>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
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
                    {{ interacao.tipoInteracao }} por {{ interacao.autor }} em {{ formatarData(interacao.dataCriacao) }}
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="chamado.interacoes.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhuma interacao visivel ao solicitante.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
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
              </q-item>
              <q-item v-if="chamado.historicos.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum historico registrado.</q-item-label>
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
import { onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import {
  anexarArquivoChamadoPortal,
  detalharChamadoPortal,
  type ChamadoPortalDetalhe
} from '@/services/apiPortal';

const route = useRoute();
const carregando = ref(false);
const enviandoAnexo = ref(false);
const erro = ref('');
const chamado = ref<ChamadoPortalDetalhe | null>(null);
const arquivoSelecionado = ref<File | null>(null);

function obterChamadoId(): string {
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

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    chamado.value = await detalharChamadoPortal(obterChamadoId());
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar detalhe do chamado.';
  } finally {
    carregando.value = false;
  }
}

async function enviarAnexo(): Promise<void> {
  if (!arquivoSelecionado.value) {
    erro.value = 'Selecione um arquivo para envio.';
    return;
  }

  enviandoAnexo.value = true;
  erro.value = '';
  try {
    await anexarArquivoChamadoPortal(obterChamadoId(), arquivoSelecionado.value);
    arquivoSelecionado.value = null;
    await carregar();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao enviar anexo.';
  } finally {
    enviandoAnexo.value = false;
  }
}

onMounted(async () => {
  await carregar();
});
</script>
