<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FormComentario from '../components/portal/FormComentario.vue'
import TimelineHistorico from '../components/portal/TimelineHistorico.vue'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { portalService } from '../services/portalService'
import type { ChamadoDetalhePortal } from '../types/portal'

const route = useRoute()
const router = useRouter()

const loading = ref(false)
const enviandoComentario = ref(false)
const enviandoAnexo = ref(false)

const erro = ref<string | null>(null)
const detalhe = ref<ChamadoDetalhePortal | null>(null)

const comentariosVisiveis = computed(() =>
  (detalhe.value?.comentarios || []).filter((comentario) => !comentario.interno)
)

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function slaProximoVencimento(): boolean {
  if (!detalhe.value?.sla) {
    return false
  }

  const sla = detalhe.value.sla
  if (sla.estaVencido || sla.estaPausado || sla.resolvidoEm) {
    return false
  }

  return new Date(sla.prazoResolucaoEm).getTime() <= Date.now() + 4 * 60 * 60 * 1000
}

async function carregar(): Promise<void> {
  const id = String(route.params.id)

  loading.value = true
  erro.value = null

  try {
    detalhe.value = await portalService.obterChamado(id)
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Falha ao carregar chamado.'

    if (message.includes('403')) {
      erro.value = 'Voce nao possui permissao para visualizar este chamado.'
    } else {
      erro.value = message
    }
  } finally {
    loading.value = false
  }
}

async function comentar(mensagem: string): Promise<void> {
  if (!detalhe.value) {
    return
  }

  enviandoComentario.value = true
  erro.value = null

  try {
    await portalService.comentarChamado(detalhe.value.id, { mensagem })
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao enviar comentario.'
  } finally {
    enviandoComentario.value = false
  }
}

async function anexar(arquivos: File[]): Promise<void> {
  if (!detalhe.value || !arquivos.length) {
    return
  }

  enviandoAnexo.value = true
  erro.value = null

  try {
    for (const arquivo of arquivos) {
      await portalService.anexarArquivo(detalhe.value.id, arquivo)
    }

    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao anexar arquivo.'
  } finally {
    enviandoAnexo.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.titulo}` : 'Detalhe do chamado'"
      subtitulo="Acompanhe status, comentarios, anexos e historico do atendimento"
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/portal/chamados')" />
          <StatusBadge v-if="detalhe" :texto="detalhe.status" />
          <PrioridadeBadge v-if="detalhe" :texto="detalhe.prioridade" />
        </div>
      </template>
    </PageHeader>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando detalhes do chamado..." />

    <template v-else-if="detalhe">
      <AppSectionCard titulo="Resumo do chamado" subtitulo="Informacoes principais da solicitacao.">
        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Codigo</q-item-label>
              <q-item-label>{{ detalhe.codigo }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Categoria</q-item-label>
              <q-item-label>{{ detalhe.categoria }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Departamento</q-item-label>
              <q-item-label>{{ detalhe.departamento || 'Nao informado' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitante</q-item-label>
              <q-item-label>{{ detalhe.solicitante }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Responsavel</q-item-label>
              <q-item-label>{{ detalhe.responsavel || 'Nao atribuido' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Aberto em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.abertoEm) }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Descricao</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.descricao }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <div class="q-mt-md row q-gutter-sm">
          <SlaBadge
            :vencido="detalhe.sla?.estaVencido"
            :proximo="slaProximoVencimento()"
            :pausado="detalhe.sla?.estaPausado"
          />

          <q-chip dense square color="grey-3" text-color="grey-9" icon="schedule">
            Prazo resolucao: {{ detalhe.sla ? formatarData(detalhe.sla.prazoResolucaoEm) : '-' }}
          </q-chip>
        </div>
      </AppSectionCard>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-7">
          <AppSectionCard titulo="Comentarios" subtitulo="Historico de comunicacao do chamado.">
            <div class="column q-gutter-sm">
              <EmptyState
                v-if="!comentariosVisiveis.length"
                titulo="Sem comentarios"
                mensagem="Nao ha comentarios visiveis neste chamado ate o momento."
              />

              <q-card v-for="comentario in comentariosVisiveis" :key="comentario.id" flat bordered class="sgx-card">
                <q-card-section>
                  <div class="text-caption text-grey-7">{{ comentario.usuario }} - {{ formatarData(comentario.criadoEm) }}</div>
                  <div class="text-body2 q-mt-xs">{{ comentario.mensagem }}</div>
                </q-card-section>
              </q-card>

              <FormComentario :loading="enviandoComentario" @submit="comentar" />
            </div>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-5">
          <AppSectionCard titulo="Anexos" subtitulo="Arquivos enviados para apoiar o atendimento.">
            <div class="column q-gutter-sm">
              <EmptyState
                v-if="!detalhe.anexos.length"
                titulo="Sem anexos"
                mensagem="Nenhum arquivo foi anexado a este chamado."
              />

              <q-list v-else bordered separator>
                <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
                  <q-item-section>
                    <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                    <q-item-label caption>
                      {{ anexo.usuario }} - {{ (anexo.tamanhoBytes / 1024).toFixed(1) }} KB - {{ formatarData(anexo.criadoEm) }}
                    </q-item-label>
                  </q-item-section>
                </q-item>
              </q-list>

              <UploadAnexo :loading="enviandoAnexo" @upload="anexar" />
            </div>
          </AppSectionCard>
        </div>
      </div>

      <AppSectionCard titulo="Historico" subtitulo="Linha do tempo das atualizacoes do chamado.">
        <TimelineHistorico :itens="detalhe.historico" />
      </AppSectionCard>
    </template>

    <EmptyState
      v-else
      titulo="Chamado nao encontrado"
      mensagem="Nao foi possivel localizar o chamado informado ou voce nao possui acesso."
    />
  </q-page>
</template>
