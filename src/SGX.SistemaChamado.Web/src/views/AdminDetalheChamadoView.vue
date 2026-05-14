<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import ComentariosAdministrativos from '../components/admin/ComentariosAdministrativos.vue'
import ModalAlterarCategoria from '../components/admin/ModalAlterarCategoria.vue'
import ModalAlterarPrioridade from '../components/admin/ModalAlterarPrioridade.vue'
import ModalAlterarStatus from '../components/admin/ModalAlterarStatus.vue'
import ModalAtribuirResponsavel from '../components/admin/ModalAtribuirResponsavel.vue'
import ModalEncerrarChamado from '../components/admin/ModalEncerrarChamado.vue'
import ModalReabrirChamado from '../components/admin/ModalReabrirChamado.vue'
import PainelAtendimento from '../components/admin/PainelAtendimento.vue'
import TimelineAdministrativa from '../components/admin/TimelineAdministrativa.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { useAuthStore } from '../stores/authStore'
import type { AdminContextoResponse, ChamadoAdminDetalhe } from '../types/admin'

const $q = useQuasar()
const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const chamadoId = route.params.id as string

const loading = ref(false)
const processing = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const contexto = ref<AdminContextoResponse | null>(null)
const detalhe = ref<ChamadoAdminDetalhe | null>(null)

const showAtribuir = ref(false)
const showStatus = ref(false)
const showPrioridade = ref(false)
const showCategoria = ref(false)
const showEncerrar = ref(false)
const showReabrir = ref(false)
const showComentar = ref(false)

const comentarioMensagem = ref('')
const comentarioInterno = ref(false)

const isAdministrador = computed(() => contexto.value?.usuario.perfis.includes('Administrador') ?? false)
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeAssumirPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.chamadosAssumir)
)
const podeAtribuirPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.chamadosAtribuir)
)
const podeEncerrarPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.chamadosEncerrar)
)

const podeAssumir = computed(() => {
  if (!podeAssumirPermissao.value) {
    return false
  }

  if (!detalhe.value) return false
  return isAdministrador.value || !detalhe.value.responsavel
})

const chamadoEncerrado = computed(() => detalhe.value?.status.toLowerCase().includes('encerrado') ?? false)

const chamadoReabrivel = computed(() => {
  const status = detalhe.value?.status.toLowerCase() ?? ''
  return status.includes('encerrado') || status.includes('resolvido')
})

const slaProximo = computed(() => detalhe.value?.sla?.situacao === 'ProximoDoVencimento')

const atualizadoEm = computed(() => {
  if (!detalhe.value?.historico.length) {
    return null
  }

  const maisRecente = detalhe.value.historico
    .map((evento) => new Date(evento.criadoEm).getTime())
    .filter((valor) => !Number.isNaN(valor))
    .sort((a, b) => b - a)[0]

  return Number.isFinite(maisRecente) ? new Date(maisRecente).toISOString() : null
})

function formatarData(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

function registrarSucesso(mensagem: string): void {
  sucesso.value = mensagem
  $q.notify({ type: 'positive', message: mensagem })
}

function registrarErro(error: unknown, fallback: string): void {
  const mensagem = error instanceof Error ? error.message : fallback
  erro.value = mensagem
  $q.notify({ type: 'negative', message: mensagem })
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const [ctx, det] = await Promise.all([
      adminService.obterAdminContexto(),
      adminService.obterChamadoAdmin(chamadoId),
    ])

    contexto.value = ctx
    detalhe.value = det
  } catch (error) {
    registrarErro(error, 'Não foi possível carregar o detalhe do chamado.')
  } finally {
    loading.value = false
  }
}

async function recarregarDetalhe(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  detalhe.value = await adminService.obterChamadoAdmin(detalhe.value.id)
}

async function assumir(): Promise<void> {
  if (!detalhe.value) return
  if (!podeAssumirPermissao.value) {
    registrarErro(new Error('Você não possui permissão para assumir chamados.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.assumirChamado(detalhe.value.id)
    registrarSucesso('Chamado assumido com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function atribuir(responsavelId: string): Promise<void> {
  if (!detalhe.value) return
  if (!podeAtribuirPermissao.value) {
    registrarErro(new Error('Você não possui permissão para atribuir chamados.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.atribuirChamado(detalhe.value.id, { responsavelId })
    showAtribuir.value = false
    registrarSucesso('Informações salvas com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function alterarStatus(statusId: string): Promise<void> {
  if (!detalhe.value) return

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.alterarStatus(detalhe.value.id, { statusId })
    showStatus.value = false
    registrarSucesso('Status alterado com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function alterarPrioridade(prioridadeId: string): Promise<void> {
  if (!detalhe.value) return

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.alterarPrioridade(detalhe.value.id, { prioridadeId })
    showPrioridade.value = false
    registrarSucesso('Prioridade alterada com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function alterarCategoria(categoriaId: string): Promise<void> {
  if (!detalhe.value) return

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.alterarCategoria(detalhe.value.id, { categoriaId })
    showCategoria.value = false
    registrarSucesso('Categoria alterada com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function comentar(): Promise<void> {
  if (!detalhe.value || !comentarioMensagem.value.trim()) return

  processing.value = true
  erro.value = null

  try {
    await adminService.comentarChamadoAdmin(detalhe.value.id, {
      mensagem: comentarioMensagem.value.trim(),
      interno: comentarioInterno.value,
    })

    comentarioMensagem.value = ''
    comentarioInterno.value = false
    showComentar.value = false

    await recarregarDetalhe()
    registrarSucesso('Comentário enviado com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function encerrar(payload: { solucao: string; comentarioInterno: boolean }): Promise<void> {
  if (!detalhe.value) return
  if (!podeEncerrarPermissao.value) {
    registrarErro(new Error('Você não possui permissão para encerrar chamados.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.encerrarChamado(detalhe.value.id, payload)
    showEncerrar.value = false
    registrarSucesso('Chamado encerrado com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function reabrir(mensagem: string): Promise<void> {
  if (!detalhe.value) return

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.reabrirChamado(detalhe.value.id, { mensagem })
    showReabrir.value = false
    registrarSucesso('Chamado reaberto com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.titulo}` : 'Detalhe administrativo do chamado'"
      subtitulo="Gerencie atendimento, atualize status e acompanhe histórico completo."
    >
      <template #actions>
        <div class="row q-gutter-xs">
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/admin/chamados')" />
          <StatusBadge v-if="detalhe" :texto="detalhe.status" />
          <PrioridadeBadge v-if="detalhe" :texto="detalhe.prioridade" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">{{ sucesso }}</q-banner>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando detalhe do chamado..." />

    <template v-else-if="detalhe">
      <AppSectionCard titulo="Resumo do chamado" subtitulo="Dados principais para triagem e acompanhamento.">
        <q-list separator>
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
              <q-item-label caption>Departamento</q-item-label>
              <q-item-label>{{ detalhe.departamento || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Origem</q-item-label>
              <q-item-label>{{ detalhe.origem }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitante</q-item-label>
              <q-item-label>{{ detalhe.solicitante.nome }} ({{ detalhe.solicitante.email }})</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Responsável</q-item-label>
              <q-item-label>{{ detalhe.responsavel?.nome || 'Não atribuído' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Aberto em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.abertoEm) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Atualizado em</q-item-label>
              <q-item-label>{{ formatarData(atualizadoEm) }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Encerrado em</q-item-label>
              <q-item-label>{{ formatarData(detalhe.encerradoEm) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Prazo primeira resposta</q-item-label>
              <q-item-label>{{ formatarData(detalhe.sla?.prazoPrimeiraRespostaEm ?? null) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Prazo resolução</q-item-label>
              <q-item-label>{{ formatarData(detalhe.sla?.prazoResolucaoEm ?? null) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Total pausado</q-item-label>
              <q-item-label>{{ detalhe.sla?.totalMinutosPausado ?? 0 }} min</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Descrição</q-item-label>
              <q-item-label class="text-body2">{{ detalhe.descricao }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <div class="q-mt-sm">
          <SlaBadge
            :vencido="detalhe.sla?.estaVencido"
            :proximo="slaProximo"
            :pausado="detalhe.sla?.estaPausado"
            :situacao="detalhe.sla?.situacao ?? 'NaoAplicavel'"
          />
        </div>
      </AppSectionCard>

      <div class="detalhe-top-grid">
        <AppSectionCard titulo="SLA em destaque" subtitulo="Indicadores operacionais de prazo e risco.">
          <q-list separator>
            <q-item>
              <q-item-section>
                <q-item-label caption>Status atual</q-item-label>
                <q-item-label>
                  <SlaBadge
                    :vencido="detalhe.sla?.estaVencido"
                    :proximo="slaProximo"
                    :pausado="detalhe.sla?.estaPausado"
                    :situacao="detalhe.sla?.situacao ?? 'NaoAplicavel'"
                  />
                </q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Prazo primeira resposta</q-item-label>
                <q-item-label>{{ formatarData(detalhe.sla?.prazoPrimeiraRespostaEm ?? null) }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Prazo resolução</q-item-label>
                <q-item-label>{{ formatarData(detalhe.sla?.prazoResolucaoEm ?? null) }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Total pausado</q-item-label>
                <q-item-label>{{ detalhe.sla?.totalMinutosPausado ?? 0 }} min</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Tipo de cálculo</q-item-label>
                <q-item-label>
                  {{ detalhe.sla?.usarHorarioComercial ? 'Cálculo em horário comercial' : 'Cálculo em minutos corridos' }}
                </q-item-label>
                <q-item-label v-if="detalhe.sla?.usarHorarioComercial" caption>
                  {{ detalhe.sla?.calendarioCorporativoNome || 'Calendário padrão' }}
                </q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>

        <AppSectionCard titulo="Solicitante e contato" subtitulo="Dados de origem para retorno e acompanhamento.">
          <q-list separator>
            <q-item>
              <q-item-section>
                <q-item-label caption>Nome</q-item-label>
                <q-item-label>{{ detalhe.solicitante.nome }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>E-mail</q-item-label>
                <q-item-label>{{ detalhe.solicitante.email }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Origem</q-item-label>
                <q-item-label>{{ detalhe.origem }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item>
              <q-item-section>
                <q-item-label caption>Abertura</q-item-label>
                <q-item-label>{{ formatarData(detalhe.abertoEm) }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </AppSectionCard>
      </div>

      <AppSectionCard titulo="Ações administrativas" subtitulo="Assumir, atribuir e atualizar ciclo do chamado.">
        <PainelAtendimento
          :chamado="detalhe"
          :loading="processing"
          :can-assumir="podeAssumir"
          :can-atribuir="podeAtribuirPermissao"
          :can-encerrar="podeEncerrarPermissao && !chamadoEncerrado"
          :can-reabrir="chamadoReabrivel"
          @assumir="assumir"
          @atribuir="showAtribuir = true"
          @alterar-status="showStatus = true"
          @alterar-prioridade="showPrioridade = true"
          @alterar-categoria="showCategoria = true"
          @comentar="showComentar = true"
          @encerrar="showEncerrar = true"
          @reabrir="showReabrir = true"
        />
      </AppSectionCard>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Comentários" subtitulo="Comentários públicos e internos da equipe.">
            <ComentariosAdministrativos :comentarios="detalhe.comentarios" />
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Histórico" subtitulo="Linha do tempo de alterações administrativas.">
            <TimelineAdministrativa :historico="detalhe.historico" />
          </AppSectionCard>
        </div>
      </div>

      <AppSectionCard titulo="Histórico de SLA" subtitulo="Eventos de aplicação, alertas, pausa, resposta e resolução.">
        <EmptyState
          v-if="!detalhe.historicoSla.length"
          titulo="Sem eventos de SLA"
          mensagem="Nenhum evento de SLA foi registrado para este chamado."
        />

        <q-list v-else bordered separator>
          <q-item v-for="evento in detalhe.historicoSla" :key="evento.id">
            <q-item-section>
              <q-item-label>{{ evento.tipoEventoDescricao }}</q-item-label>
              <q-item-label caption>{{ evento.descricao }}</q-item-label>
            </q-item-section>
            <q-item-section side>
              <q-item-label caption>{{ formatarData(evento.dataEvento) }}</q-item-label>
              <q-item-label caption>{{ evento.usuario || '-' }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>

      <AppSectionCard titulo="Anexos" subtitulo="Arquivos relacionados ao chamado.">
        <EmptyState v-if="!detalhe.anexos.length" titulo="Sem anexos" mensagem="Nenhum anexo foi enviado para este chamado." />

        <q-list v-else bordered separator>
          <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
            <q-item-section>
              <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
              <q-item-label caption>
                {{ anexo.contentType }} - {{ (anexo.tamanhoBytes / 1024).toFixed(1) }} KB - {{ anexo.usuario }} - {{ formatarData(anexo.criadoEm) }}
              </q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>
    </template>

    <EmptyState
      v-else
      titulo="Chamado não encontrado"
      mensagem="Não foi possível carregar o chamado solicitado ou ele não está disponível."
    />

    <ModalAtribuirResponsavel
      v-model="showAtribuir"
      :atendentes="contexto?.atendentes ?? []"
      :loading="processing"
      @confirmar="atribuir"
    />

    <ModalAlterarStatus v-model="showStatus" :status="contexto?.status ?? []" :loading="processing" @confirmar="alterarStatus" />

    <ModalAlterarPrioridade
      v-model="showPrioridade"
      :prioridades="contexto?.prioridades ?? []"
      :loading="processing"
      @confirmar="alterarPrioridade"
    />

    <ModalAlterarCategoria
      v-model="showCategoria"
      :categorias="contexto?.categorias ?? []"
      :loading="processing"
      @confirmar="alterarCategoria"
    />

    <ModalEncerrarChamado v-model="showEncerrar" :loading="processing" @confirmar="encerrar" />

    <ModalReabrirChamado v-model="showReabrir" :loading="processing" @confirmar="reabrir" />

    <q-dialog v-model="showComentar">
      <q-card class="sgx-card comment-dialog-card">
        <q-card-section>
          <div class="text-h6">Novo comentário administrativo</div>
        </q-card-section>

        <q-card-section class="column q-gutter-sm">
          <q-input
            v-model="comentarioMensagem"
            outlined
            type="textarea"
            autogrow
            label="Mensagem"
            :rules="[(v) => !!String(v || '').trim() || 'Informe a mensagem']"
          />
          <q-toggle v-model="comentarioInterno" label="Comentário interno" />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Enviar comentário" :loading="processing" @click="comentar" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
.comment-dialog-card {
  width: min(560px, 92vw);
}

.detalhe-top-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 16px;
}

@media (max-width: 1024px) {
  .detalhe-top-grid {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
