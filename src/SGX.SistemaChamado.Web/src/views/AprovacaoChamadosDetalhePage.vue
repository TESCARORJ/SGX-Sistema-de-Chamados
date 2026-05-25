<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { aprovacaoChamadosAdminService } from '../services/aprovacaoChamadosAdminService'
import { useAuthStore } from '../stores/authStore'
import { StatusAprovacaoChamado, type AprovacaoChamadoDetalhe } from '../types/aprovacaoChamados'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const aprovacaoId = String(route.params.id ?? '')

const loading = ref(false)
const erro = ref<string | null>(null)
const processandoAcao = ref(false)
const detalhe = ref<AprovacaoChamadoDetalhe | null>(null)

const dialogAcaoAberto = ref(false)
const acaoSelecionada = ref<'aprovar' | 'reprovar' | 'cancelar' | null>(null)
const justificativaAcao = ref('')

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeAprovarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosAprovar))
const podeReprovarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosReprovar))
const podeCancelarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosCancelar))

const statusPendente = computed(() => detalhe.value?.status === StatusAprovacaoChamado.Pendente)
const podeAprovar = computed(() => statusPendente.value && podeAprovarPermissao.value)
const podeReprovar = computed(() => statusPendente.value && podeReprovarPermissao.value)
const podeCancelar = computed(() => statusPendente.value && podeCancelarPermissao.value)

const tituloDialogAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'Confirmar aprovacao'
  if (acaoSelecionada.value === 'reprovar') return 'Confirmar reprovacao'
  return 'Confirmar cancelamento'
})

const labelBotaoAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'Aprovar'
  if (acaoSelecionada.value === 'reprovar') return 'Reprovar'
  return 'Cancelar aprovacao'
})

const corBotaoAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'positive'
  if (acaoSelecionada.value === 'reprovar') return 'negative'
  return 'warning'
})

const justificativaObrigatoria = computed(() =>
  acaoSelecionada.value === 'reprovar' || acaoSelecionada.value === 'cancelar'
)

function formatarData(data: string | null): string {
  if (!data) return '-'
  return new Date(data).toLocaleString('pt-BR')
}

function corStatus(status: StatusAprovacaoChamado): string {
  switch (status) {
    case StatusAprovacaoChamado.Pendente:
      return 'warning'
    case StatusAprovacaoChamado.Aprovado:
      return 'positive'
    case StatusAprovacaoChamado.Reprovado:
      return 'negative'
    case StatusAprovacaoChamado.Cancelado:
      return 'grey-7'
    default:
      return 'grey-7'
  }
}

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (!(error instanceof Error)) {
    return fallback
  }

  const mensagem = error.message
  const jsonStart = mensagem.indexOf('{')
  if (jsonStart >= 0) {
    const trechoJson = mensagem.slice(jsonStart)
    try {
      const parsed = JSON.parse(trechoJson) as { mensagem?: string }
      if (parsed?.mensagem) {
        return parsed.mensagem
      }
    } catch {
      return mensagem
    }
  }

  return mensagem
}

async function carregarDetalhe(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    detalhe.value = await aprovacaoChamadosAdminService.obterPorId(aprovacaoId)
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar o detalhe da aprovacao.')
  } finally {
    loading.value = false
  }
}

function abrirDialogAcao(acao: 'aprovar' | 'reprovar' | 'cancelar'): void {
  acaoSelecionada.value = acao
  justificativaAcao.value = ''
  dialogAcaoAberto.value = true
}

function fecharDialogAcao(): void {
  dialogAcaoAberto.value = false
  acaoSelecionada.value = null
  justificativaAcao.value = ''
}

async function confirmarAcao(): Promise<void> {
  if (!detalhe.value || !acaoSelecionada.value) {
    return
  }

  if (justificativaObrigatoria.value && !justificativaAcao.value.trim()) {
    $q.notify({ type: 'warning', message: 'Informe a justificativa para continuar.' })
    return
  }

  processandoAcao.value = true

  try {
    const justificativa = justificativaAcao.value.trim()

    if (acaoSelecionada.value === 'aprovar') {
      await aprovacaoChamadosAdminService.aprovar(detalhe.value.id, {
        justificativaDecisao: justificativa || undefined,
      })
      $q.notify({ type: 'positive', message: 'Aprovacao registrada com sucesso.' })
    }

    if (acaoSelecionada.value === 'reprovar') {
      await aprovacaoChamadosAdminService.reprovar(detalhe.value.id, {
        justificativaDecisao: justificativa,
      })
      $q.notify({ type: 'positive', message: 'Reprovacao registrada com sucesso.' })
    }

    if (acaoSelecionada.value === 'cancelar') {
      await aprovacaoChamadosAdminService.cancelar(detalhe.value.id, {
        justificativaDecisao: justificativa,
      })
      $q.notify({ type: 'positive', message: 'Cancelamento registrado com sucesso.' })
    }

    fecharDialogAcao()
    await carregarDetalhe()
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel concluir a acao desta aprovacao.')
    erro.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    processandoAcao.value = false
  }
}

function abrirChamadoRelacionado(): void {
  if (!detalhe.value) {
    return
  }

  router.push(`/admin/chamados/${detalhe.value.chamadoId}`)
}

onMounted(carregarDetalhe)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `Aprovacao ${detalhe.numeroProtocoloChamado}` : 'Detalhe da aprovacao'"
      subtitulo="Visualize o ciclo de aprovacao do chamado e execute decisoes quando permitido."
    >
      <template #actions>
        <div class="row q-gutter-xs">
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/admin/atendimento/aprovacao-chamados')" />
          <q-btn
            v-if="detalhe"
            flat
            color="secondary"
            icon="open_in_new"
            label="Abrir chamado"
            @click="abrirChamadoRelacionado"
          />
        </div>
      </template>
    </PageHeader>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregarDetalhe" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando detalhe da aprovacao..." />

    <template v-else-if="detalhe">
      <AppSectionCard titulo="Situacao da aprovacao" subtitulo="Estado atual, origem e vinculo com o chamado.">
        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Status</q-item-label>
              <q-item-label>
                <q-chip dense square text-color="white" :color="corStatus(detalhe.status)">
                  {{ detalhe.statusDescricao }}
                </q-chip>
              </q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Origem</q-item-label>
              <q-item-label>{{ detalhe.tipoOrigemDescricao }}</q-item-label>
              <q-item-label caption>{{ detalhe.origemDescricao || '-' }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Solicitada em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.solicitadaEm) }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Decidida em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.decididaEm) }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Chamado</q-item-label>
              <q-item-label>{{ detalhe.numeroProtocoloChamado }} - {{ detalhe.tituloChamado }}</q-item-label>
              <q-item-label caption>{{ detalhe.descricaoChamado || 'Sem descricao informada.' }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>

      <AppSectionCard titulo="Participantes" subtitulo="Solicitante, aprovador e trilha da decisao.">
        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitante</q-item-label>
              <q-item-label>{{ detalhe.solicitanteNome || '-' }}</q-item-label>
              <q-item-label caption>{{ detalhe.solicitanteId || '-' }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Aprovador</q-item-label>
              <q-item-label>{{ detalhe.aprovadorNome || '-' }}</q-item-label>
              <q-item-label caption>{{ detalhe.aprovadorId || '-' }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Criado em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.criadoEm) }}</q-item-label>
            </q-item-section>

            <q-item-section>
              <q-item-label caption>Atualizado em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.atualizadoEm) }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Justificativa da solicitacao</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.justificativaSolicitacao || '-' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Justificativa da decisao</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.justificativaDecisao || '-' }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>

      <AppSectionCard titulo="Acoes" subtitulo="Aprove, reprove ou cancele quando a aprovacao estiver pendente.">
        <q-banner v-if="!statusPendente" rounded class="bg-blue-1 text-primary">
          Esta aprovacao ja foi decidida e nao permite novas acoes.
        </q-banner>

        <div v-else class="row q-gutter-sm">
          <q-btn v-if="podeAprovar" color="positive" icon="check" label="Aprovar" @click="abrirDialogAcao('aprovar')" />
          <q-btn v-if="podeReprovar" color="negative" icon="close" label="Reprovar" @click="abrirDialogAcao('reprovar')" />
          <q-btn v-if="podeCancelar" color="warning" text-color="black" icon="cancel" label="Cancelar" @click="abrirDialogAcao('cancelar')" />
        </div>
      </AppSectionCard>
    </template>

    <EmptyState
      v-else
      titulo="Aprovacao nao encontrada"
      mensagem="Nao foi possivel carregar a aprovacao solicitada."
    />

    <q-dialog v-model="dialogAcaoAberto">
      <q-card class="sgx-card" style="width: min(640px, 94vw)">
        <q-card-section class="text-h6">{{ tituloDialogAcao }}</q-card-section>

        <q-card-section class="column q-gutter-sm">
          <div class="text-body2 text-grey-8">
            Esta acao atualiza o status da aprovacao e sera registrada no historico do chamado.
          </div>

          <q-input
            v-model="justificativaAcao"
            outlined
            autogrow
            type="textarea"
            :label="justificativaObrigatoria ? 'Justificativa (obrigatoria)' : 'Justificativa (opcional)'"
            :rules="justificativaObrigatoria ? [(v) => !!String(v || '').trim() || 'Informe a justificativa'] : []"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Voltar" :disable="processandoAcao" @click="fecharDialogAcao" />
          <q-btn :color="corBotaoAcao" :label="labelBotaoAcao" :loading="processandoAcao" @click="confirmarAcao" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
