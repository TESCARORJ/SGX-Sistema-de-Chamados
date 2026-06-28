<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FormComentario from '../components/portal/FormComentario.vue'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { StatusAprovacaoChamado } from '../types/aprovacaoChamados'
import { chamadosService } from '../services/chamadosService'
import { portalService } from '../services/portalService'
import { useAuthStore } from '../stores/authStore'
import type { AnexoChamado } from '../types/anexo'
import type { ComentarioChamado } from '../types/comentario'
import type { LinhaTempoChamadoItem } from '../types/linhaTempo'
import type { ChamadoDetalhePortal } from '../types/portal'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const enviandoComentario = ref(false)
const enviandoAnexo = ref(false)

const erro = ref<string | null>(null)
const detalhe = ref<ChamadoDetalhePortal | null>(null)
const comentarios = ref<ComentarioChamado[]>([])
const anexos = ref<AnexoChamado[]>([])
const linhaTempo = ref<LinhaTempoChamadoItem[]>([])

const podeComentarInterno = computed(() =>
  (authStore.usuario?.perfis ?? []).some((perfil) => perfil === 'Administrador' || perfil === 'Atendente')
)

const comentariosVisiveis = computed(() => {
  if (podeComentarInterno.value) {
    return comentarios.value
  }

  return comentarios.value.filter((comentario) => !comentario.interno)
})

const linhaTempoVisivel = computed(() => {
  if (podeComentarInterno.value) {
    return linhaTempo.value
  }

  return linhaTempo.value.filter((item) => !item.interno)
})
const totalComentariosVisiveis = computed(() => comentariosVisiveis.value.length)
const totalAnexos = computed(() => anexos.value.length)
const totalEventosLinhaTempo = computed(() => linhaTempoVisivel.value.length)

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function formatarTamanho(tamanhoBytes: number): string {
  if (tamanhoBytes < 1024) {
    return `${tamanhoBytes} B`
  }

  return `${(tamanhoBytes / 1024).toFixed(1)} KB`
}

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (!(error instanceof Error)) {
    return fallback
  }

  const mensagem = error.message?.trim()
  if (!mensagem) {
    return fallback
  }

  if (mensagem.includes('Request failed with status code') || mensagem.includes('Network Error')) {
    return fallback
  }

  return mensagem
}

async function carregar(): Promise<void> {
  const id = String(route.params.id)

  loading.value = true
  erro.value = null

  try {
    const [detalheResponse, comentariosResponse, anexosResponse, linhaTempoResponse] = await Promise.all([
      portalService.obterChamado(id),
      chamadosService.listarComentariosChamado(id),
      chamadosService.listarAnexosChamado(id),
      chamadosService.listarLinhaTempoChamado(id),
    ])

    detalhe.value = detalheResponse
    comentarios.value = comentariosResponse
    anexos.value = anexosResponse
    linhaTempo.value = linhaTempoResponse.items
  } catch (error) {
    const message = extrairMensagemErro(error, 'Nao foi possivel carregar o detalhe do chamado.')

    if (message.includes('403')) {
      erro.value = 'Voce nao possui permissao para visualizar este chamado.'
    } else if (message.includes('404')) {
      erro.value = 'Chamado nao encontrado.'
    } else {
      erro.value = message
    }
  } finally {
    loading.value = false
  }
}

async function comentar(payload: { mensagem: string; interno: boolean }): Promise<void> {
  if (!detalhe.value) {
    return
  }

  enviandoComentario.value = true
  erro.value = null

  try {
    await chamadosService.adicionarComentarioChamado(detalhe.value.id, {
      mensagem: payload.mensagem,
      interno: podeComentarInterno.value ? payload.interno : false,
    })
    await carregar()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel concluir a acao.')
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
      await chamadosService.enviarAnexoChamado(detalhe.value.id, arquivo)
    }

    await carregar()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel concluir a acao.')
  } finally {
    enviandoAnexo.value = false
  }
}

function onAnexoInvalido(mensagem: string): void {
  erro.value = mensagem
}

async function baixarAnexo(anexo: AnexoChamado): Promise<void> {
  if (!detalhe.value) {
    return
  }

  try {
    const response = await chamadosService.baixarAnexoChamado(detalhe.value.id, anexo.id)
    const blob = response.blob
    const nomeArquivo = response.nomeArquivo || anexo.nomeArquivo

    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = nomeArquivo
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel baixar o anexo.')
  }
}

async function baixarAnexoDaLinhaTempo(item: LinhaTempoChamadoItem): Promise<void> {
  if (!detalhe.value || item.tipo !== 'anexo' || !item.referenciaId) {
    return
  }

  try {
    const response = await chamadosService.baixarAnexoChamado(detalhe.value.id, item.referenciaId)
    const blob = response.blob
    const nomeArquivo = response.nomeArquivo || item.nomeArquivo || 'anexo'

    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = nomeArquivo
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel baixar o anexo.')
  }
}

function corLinhaTempo(tipo: string, interno: boolean): string {
  if (interno) {
    return 'orange-8'
  }

  if (tipo === 'abertura') return 'primary'
  if (tipo === 'comentario') return 'blue'
  if (tipo === 'anexo') return 'deep-orange'
  if (tipo === 'status') return 'teal'
  if (tipo === 'encerramento') return 'positive'
  if (tipo === 'reabertura') return 'warning'

  return 'secondary'
}

function iconeLinhaTempo(tipo: string): string {
  if (tipo === 'abertura') return 'play_circle'
  if (tipo === 'comentario') return 'comment'
  if (tipo === 'anexo') return 'attach_file'
  if (tipo === 'status') return 'sync'
  if (tipo === 'responsavel') return 'person'
  if (tipo === 'prioridade') return 'flag'
  if (tipo === 'categoria') return 'category'
  if (tipo === 'encerramento') return 'check_circle'
  if (tipo === 'reabertura') return 'autorenew'
  if (tipo === 'sla') return 'schedule'

  return 'history'
}

const statusAprovacaoTexto = computed(() => {
  if (!detalhe.value?.requerAprovacao) return 'Nao requer aprovacao'
  if (detalhe.value.aprovacaoPendente || detalhe.value.statusAprovacao === StatusAprovacaoChamado.Pendente) return 'Aguardando aprovacao'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Aprovado) return 'Aprovado'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Reprovado) return 'Reprovado'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Cancelado) return 'Aprovacao cancelada'
  return 'Nao informado'
})

const corStatusAprovacao = computed(() => {
  if (!detalhe.value?.requerAprovacao) return 'grey-7'
  if (detalhe.value.aprovacaoPendente || detalhe.value.statusAprovacao === StatusAprovacaoChamado.Pendente) return 'orange-8'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Aprovado) return 'positive'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Reprovado) return 'negative'
  if (detalhe.value.statusAprovacao === StatusAprovacaoChamado.Cancelado) return 'grey-7'
  return 'grey-7'
})

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Portal do solicitante"
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.titulo}` : 'Detalhe do chamado'"
      subtitulo="Acompanhe status, priorizacao, prazos de SLA e andamento completo do atendimento."
    >
      <template #actions>
        <div class="row items-center q-gutter-sm">
          <q-btn flat color="secondary" icon="add" label="Novo chamado" @click="router.push('/portal/chamados/novo')" />
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/portal/chamados')" />
          <StatusBadge v-if="detalhe" :texto="detalhe.status" />
          <PrioridadeBadge v-if="detalhe" :texto="detalhe.prioridade" />
          <SlaBadge
            v-if="detalhe"
            :vencido="detalhe.sla?.estaVencido"
            :proximo="detalhe.sla?.situacao === 'ProximoDoVencimento'"
            :pausado="detalhe.sla?.estaPausado"
            :situacao="detalhe.sla?.situacao ?? 'NaoAplicavel'"
          />
        </div>
      </template>
    </PageHeader>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando detalhes do chamado..." />

    <template v-else-if="detalhe">
      <div class="sgx-kpi-grid">
        <MetricCard title="Comentarios visiveis" :value="totalComentariosVisiveis" icon="forum" tone="info" />
        <MetricCard title="Anexos vinculados" :value="totalAnexos" icon="attach_file" tone="primary" />
        <MetricCard title="Eventos da linha do tempo" :value="totalEventosLinhaTempo" icon="timeline" tone="warning" />
        <MetricCard
          title="Aprovacao"
          :value="detalhe.aprovacaoPendente ? 'Pendente' : 'Concluida'"
          :caption="statusAprovacaoTexto"
          icon="fact_check"
          :tone="detalhe.aprovacaoPendente ? 'warning' : 'positive'"
        />
      </div>

      <AppSectionCard titulo="Resumo do chamado" subtitulo="Informações principais da solicitação.">
        <q-list separator class="resumo-list">
          <q-item v-if="detalhe.catalogoServicoNome">
            <q-item-section>
              <q-item-label caption>Serviço solicitado (Catálogo)</q-item-label>
              <q-item-label class="text-primary text-weight-medium">
                <q-icon name="miscellaneous_services" size="xs" class="q-mr-xs" />
                {{ detalhe.catalogoServicoNome }}
              </q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Código</q-item-label>
              <q-item-label>{{ detalhe.codigo }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Categoria</q-item-label>
              <q-item-label>{{ detalhe.categoria }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Subcategoria</q-item-label>
              <q-item-label>{{ detalhe.subcategoria || 'Não informada' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Departamento</q-item-label>
              <q-item-label>{{ detalhe.departamento || 'Não informado' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitante</q-item-label>
              <q-item-label>{{ detalhe.solicitante }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Responsável</q-item-label>
              <q-item-label>{{ detalhe.responsavel || 'Não atribuído' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Aberto em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.abertoEm) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Tipo de solicitação</q-item-label>
              <q-item-label>{{ detalhe.tipoSolicitacao || 'Não informado' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Local / Unidade</q-item-label>
              <q-item-label>{{ detalhe.localUnidade || 'Não informado' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Descrição</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.descricao }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <div class="q-mt-md row q-gutter-sm">
          <SlaBadge
            :vencido="detalhe.sla?.estaVencido"
            :proximo="detalhe.sla?.situacao === 'ProximoDoVencimento'"
            :pausado="detalhe.sla?.estaPausado"
            :situacao="detalhe.sla?.situacao ?? 'NaoAplicavel'"
          />

          <q-chip dense square color="grey-3" text-color="grey-9" icon="schedule">
            Prazo resolução: {{ detalhe.sla ? formatarData(detalhe.sla.prazoResolucaoEm) : '-' }}
          </q-chip>
          <q-chip v-if="detalhe.sla" dense square color="grey-3" text-color="grey-9" icon="event_available">
            {{ detalhe.sla.usarHorarioComercial ? 'Horário comercial' : 'Minutos corridos' }}
          </q-chip>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Aprovacao" subtitulo="Acompanhe o status de aprovacao deste chamado.">
        <q-banner
          rounded
          :class="{
            'bg-blue-1 text-primary': !detalhe.requerAprovacao,
            'bg-orange-1 text-orange-10': detalhe.aprovacaoPendente || detalhe.statusAprovacao === StatusAprovacaoChamado.Pendente,
            'bg-green-1 text-positive': detalhe.statusAprovacao === StatusAprovacaoChamado.Aprovado,
            'bg-red-1 text-negative': detalhe.statusAprovacao === StatusAprovacaoChamado.Reprovado,
            'bg-grey-2 text-grey-8': detalhe.statusAprovacao === StatusAprovacaoChamado.Cancelado,
          }"
          class="q-mb-sm"
        >
          {{ detalhe.mensagemOrientativaAprovacao }}
        </q-banner>

        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Status da aprovacao</q-item-label>
              <q-item-label>
                <q-chip dense square text-color="white" :color="corStatusAprovacao">
                  {{ statusAprovacaoTexto }}
                </q-chip>
              </q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Requer aprovacao</q-item-label>
              <q-item-label>{{ detalhe.requerAprovacao ? 'Sim' : 'Nao' }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Aprovacao pendente</q-item-label>
              <q-item-label>{{ detalhe.aprovacaoPendente ? 'Sim' : 'Nao' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitada em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.aprovacaoSolicitadaEm) }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Decidida em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.aprovacaoDecididaEm) }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item v-if="detalhe.statusAprovacao === StatusAprovacaoChamado.Reprovado && detalhe.justificativaReprovacao">
            <q-item-section>
              <q-item-label caption>Justificativa da reprovacao</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.justificativaReprovacao }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item v-else-if="detalhe.statusAprovacao === StatusAprovacaoChamado.Aprovado && detalhe.justificativaAprovacao">
            <q-item-section>
              <q-item-label caption>Justificativa da aprovacao</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.justificativaAprovacao }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-7">
          <AppSectionCard titulo="Comentários" subtitulo="Histórico de comunicação do chamado.">
            <div class="column q-gutter-sm">
              <EmptyState
                v-if="!comentariosVisiveis.length"
                titulo="Sem comentários"
                mensagem="Nenhum comentário encontrado."
              />

              <q-card v-for="comentario in comentariosVisiveis" :key="comentario.id" flat bordered class="sgx-card">
                <q-card-section>
                  <div class="row items-center justify-between q-gutter-sm">
                    <div class="text-caption text-grey-7">{{ comentario.usuario }} - {{ formatarData(comentario.criadoEm) }}</div>
                    <q-badge v-if="comentario.interno" color="orange-8" text-color="white">Interno</q-badge>
                  </div>
                  <div class="text-body2 q-mt-xs">{{ comentario.mensagem }}</div>
                </q-card-section>
              </q-card>

              <FormComentario
                :loading="enviandoComentario"
                :pode-comentar-interno="podeComentarInterno"
                @submit="comentar"
              />
            </div>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-5">
          <AppSectionCard titulo="Atendimento / Anexos" subtitulo="Evidências enviadas no atendimento do chamado.">
            <div class="column q-gutter-sm">
              <EmptyState
                v-if="!anexos.length"
                titulo="Sem anexos"
                mensagem="Nenhum arquivo foi anexado a este chamado."
              />

              <q-list v-else bordered separator>
                <q-item v-for="anexo in anexos" :key="anexo.id">
                  <q-item-section>
                    <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                    <q-item-label caption>
                      {{ anexo.usuario }} - {{ formatarTamanho(anexo.tamanhoBytes) }} - {{ formatarData(anexo.criadoEm) }}
                    </q-item-label>
                  </q-item-section>
                  <q-item-section side>
                    <q-btn flat color="primary" icon="download" label="Baixar" @click="baixarAnexo(anexo)" />
                  </q-item-section>
                </q-item>
              </q-list>

              <UploadAnexo :loading="enviandoAnexo" @upload="anexar" @invalid="onAnexoInvalido" />
            </div>
          </AppSectionCard>
        </div>
      </div>

      <AppSectionCard titulo="Linha do tempo" subtitulo="Visão consolidada dos eventos do atendimento.">
        <q-timeline v-if="linhaTempoVisivel.length" color="primary" side="right" layout="comfortable">
          <q-timeline-entry
            v-for="item in linhaTempoVisivel"
            :key="item.id"
            :color="corLinhaTempo(item.tipo, item.interno)"
            :icon="iconeLinhaTempo(item.tipo)"
            :title="item.titulo"
            :subtitle="formatarData(item.dataHora)"
          >
            <div class="row items-center q-gutter-sm">
              <div class="text-caption text-grey-7">{{ item.tipoDescricao }}</div>
              <q-badge v-if="item.interno && podeComentarInterno" color="orange-8" text-color="white">Interno</q-badge>
            </div>
            <div class="text-body2 q-mt-xs">{{ item.descricao }}</div>
            <div v-if="item.usuario" class="text-caption text-grey-7 q-mt-xs">por {{ item.usuario }}</div>

            <div v-if="item.tipo === 'anexo'" class="q-mt-sm">
              <q-btn
                flat
                color="primary"
                icon="download"
                label="Baixar anexo"
                @click="baixarAnexoDaLinhaTempo(item)"
              />
            </div>
          </q-timeline-entry>
        </q-timeline>

        <EmptyState
          v-else
          titulo="Sem eventos na linha do tempo"
          mensagem="Ainda nao ha atualizacoes adicionais para este chamado."
          icon="timeline"
        />

        <q-banner v-if="linhaTempoVisivel.length <= 1" rounded class="bg-grey-2 text-grey-9 q-mt-sm">
          Ainda não há atualizações além da abertura do chamado.
        </q-banner>
      </AppSectionCard>
    </template>

    <EmptyState
      v-else
      titulo="Chamado não encontrado"
      mensagem="Não foi possível localizar o chamado informado ou você não possui acesso."
    />
  </q-page>
</template>

<style scoped>
.resumo-list :deep(.q-item) {
  padding-left: 0;
  padding-right: 0;
}
</style>
