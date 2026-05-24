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
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { chamadoBaseConhecimentoService } from '../services/chamadoBaseConhecimentoService'
import { chamadoInventarioAtivoService } from '../services/chamadoInventarioAtivoService'
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { useAuthStore } from '../stores/authStore'
import type {
  AdminContextoResponse,
  ArtigoConhecimentoDisponivelParaVinculo,
  ChamadoAdminDetalhe,
  ChamadoArtigoConhecimento,
} from '../types/admin'
import type { InventarioAtivoDetalhe, InventarioAtivoListagem } from '../types/inventarioAtivos'

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
const showVincularArtigo = ref(false)
const showConfirmarRemocaoVinculo = ref(false)
const showVincularAtivo = ref(false)
const showConfirmarRemocaoAtivo = ref(false)

const comentarioMensagem = ref('')
const comentarioInterno = ref(false)
const artigosConhecimento = ref<ChamadoArtigoConhecimento[]>([])
const erroBaseConhecimento = ref<string | null>(null)
const loadingArtigosConhecimento = ref(false)
const loadingArtigosDisponiveis = ref(false)
const vinculandoArtigoId = ref<string | null>(null)
const removendoArtigoId = ref<string | null>(null)
const artigoVinculoSelecionado = ref<ChamadoArtigoConhecimento | null>(null)
const artigosDisponiveis = ref<ArtigoConhecimentoDisponivelParaVinculo[]>([])
const totalArtigosDisponiveis = ref(0)
const paginaArtigosDisponiveis = ref(1)
const tamanhoPaginaArtigosDisponiveis = ref(8)
const termoBuscaArtigosDisponiveis = ref('')
const categoriaBuscaArtigosDisponiveis = ref('')
const ativosDisponiveis = ref<InventarioAtivoListagem[]>([])
const totalAtivosDisponiveis = ref(0)
const paginaAtivosDisponiveis = ref(1)
const tamanhoPaginaAtivosDisponiveis = ref(8)
const termoBuscaAtivosDisponiveis = ref('')
const loadingAtivosDisponiveis = ref(false)
const vinculandoAtivoId = ref<string | null>(null)
const removendoVinculoAtivo = ref(false)
const ativoVinculadoDetalhe = ref<InventarioAtivoDetalhe | null>(null)

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
const podeVincularArtigoConhecimento = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.baseConhecimentoVincularChamado)
)
const podeVincularAtivoInventario = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.inventarioAtivosVincularChamado)
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
const totalPaginasArtigosDisponiveis = computed(() =>
  Math.max(1, Math.ceil(totalArtigosDisponiveis.value / tamanhoPaginaArtigosDisponiveis.value))
)
const totalPaginasAtivosDisponiveis = computed(() =>
  Math.max(1, Math.ceil(totalAtivosDisponiveis.value / tamanhoPaginaAtivosDisponiveis.value))
)
const mensagemConfirmarRemocaoVinculo = computed(() => {
  const titulo = artigoVinculoSelecionado.value?.titulo ?? ''
  return `Deseja remover o vinculo do artigo "${titulo}" deste chamado?`
})
const mensagemConfirmarRemocaoAtivo = computed(() => {
  const codigo = detalhe.value?.inventarioAtivoCodigo ?? ''
  const nome = detalhe.value?.inventarioAtivoNome ?? ''
  const referencia = [codigo, nome].filter(Boolean).join(' - ')
  return `Deseja remover o vinculo do ativo "${referencia || 'selecionado'}" deste chamado?`
})

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
  const mensagem = extrairMensagemErro(error, fallback)
  erro.value = mensagem
  $q.notify({ type: 'negative', message: mensagem })
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

function statusArtigoColor(status: number): string {
  if (status === 2) {
    return 'positive'
  }

  if (status === 3) {
    return 'negative'
  }

  if (status === 1) {
    return 'warning'
  }

  return 'grey-7'
}

function visibilidadeArtigoColor(visibilidade: number): string {
  if (visibilidade === 2) {
    return 'deep-orange'
  }

  if (visibilidade === 1) {
    return 'indigo'
  }

  return 'teal'
}

async function carregarAtivoVinculadoDetalhe(): Promise<void> {
  if (!detalhe.value?.inventarioAtivoId) {
    ativoVinculadoDetalhe.value = null
    return
  }

  try {
    ativoVinculadoDetalhe.value = await inventarioAtivosAdminService.obterPorId(detalhe.value.inventarioAtivoId)
  } catch {
    ativoVinculadoDetalhe.value = null
  }
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
    await carregarAtivoVinculadoDetalhe()

    if (podeVincularArtigoConhecimento.value) {
      await carregarArtigosConhecimento()
    }
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
  await carregarAtivoVinculadoDetalhe()
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

async function alterarCategoria(payload: {
  categoriaId: string
  subcategoriaId?: string
  tipoSolicitacaoId?: string
  localUnidadeId?: string
  departamentoId?: string
}): Promise<void> {
  if (!detalhe.value) return

  processing.value = true
  erro.value = null

  try {
    detalhe.value = await adminService.alterarCategoria(detalhe.value.id, payload)
    showCategoria.value = false
    registrarSucesso('Classificação alterada com sucesso.')
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

async function carregarArtigosConhecimento(): Promise<void> {
  if (!detalhe.value || !podeVincularArtigoConhecimento.value) {
    return
  }

  loadingArtigosConhecimento.value = true
  erroBaseConhecimento.value = null

  try {
    artigosConhecimento.value = await chamadoBaseConhecimentoService.listarArtigosDoChamado(detalhe.value.id)
  } catch (error) {
    erroBaseConhecimento.value = extrairMensagemErro(error, 'Não foi possível carregar os artigos vinculados ao chamado.')
  } finally {
    loadingArtigosConhecimento.value = false
  }
}

async function carregarArtigosDisponiveisParaVinculo(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  loadingArtigosDisponiveis.value = true

  try {
    const response = await chamadoBaseConhecimentoService.buscarArtigosDisponiveisParaVinculo(detalhe.value.id, {
      termo: termoBuscaArtigosDisponiveis.value.trim() || undefined,
      categoriaId: categoriaBuscaArtigosDisponiveis.value || undefined,
      page: paginaArtigosDisponiveis.value,
      pageSize: tamanhoPaginaArtigosDisponiveis.value,
    })

    artigosDisponiveis.value = response.items
    totalArtigosDisponiveis.value = response.total
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Não foi possível buscar artigos disponíveis para vínculo.')
    erroBaseConhecimento.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    loadingArtigosDisponiveis.value = false
  }
}

async function abrirModalVincularArtigo(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  termoBuscaArtigosDisponiveis.value = ''
  categoriaBuscaArtigosDisponiveis.value = ''
  paginaArtigosDisponiveis.value = 1
  totalArtigosDisponiveis.value = 0
  artigosDisponiveis.value = []
  erroBaseConhecimento.value = null
  showVincularArtigo.value = true

  await carregarArtigosDisponiveisParaVinculo()
}

async function buscarArtigosDisponiveis(): Promise<void> {
  paginaArtigosDisponiveis.value = 1
  await carregarArtigosDisponiveisParaVinculo()
}

async function alterarPaginaArtigosDisponiveis(value: number): Promise<void> {
  paginaArtigosDisponiveis.value = value
  await carregarArtigosDisponiveisParaVinculo()
}

async function vincularArtigo(artigo: ArtigoConhecimentoDisponivelParaVinculo): Promise<void> {
  if (!detalhe.value) {
    return
  }

  vinculandoArtigoId.value = artigo.artigoId

  try {
    await chamadoBaseConhecimentoService.vincularArtigoAoChamado(detalhe.value.id, artigo.artigoId)
    $q.notify({ type: 'positive', message: 'Artigo vinculado ao chamado com sucesso.' })
    await carregarArtigosConhecimento()
    await carregarArtigosDisponiveisParaVinculo()
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Não foi possível vincular o artigo ao chamado.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    vinculandoArtigoId.value = null
  }
}

function prepararRemocaoVinculo(artigo: ChamadoArtigoConhecimento): void {
  artigoVinculoSelecionado.value = artigo
  showConfirmarRemocaoVinculo.value = true
}

function cancelarRemocaoVinculo(): void {
  artigoVinculoSelecionado.value = null
}

async function confirmarRemocaoVinculo(): Promise<void> {
  if (!detalhe.value || !artigoVinculoSelecionado.value) {
    return
  }

  removendoArtigoId.value = artigoVinculoSelecionado.value.artigoId

  try {
    const response = await chamadoBaseConhecimentoService.removerArtigoDoChamado(
      detalhe.value.id,
      artigoVinculoSelecionado.value.artigoId
    )

    showConfirmarRemocaoVinculo.value = false
    artigoVinculoSelecionado.value = null
    $q.notify({ type: 'positive', message: response.mensagem || 'Vínculo removido com sucesso.' })
    await carregarArtigosConhecimento()

    if (showVincularArtigo.value) {
      await carregarArtigosDisponiveisParaVinculo()
    }
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Não foi possível remover o vínculo do artigo.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    removendoArtigoId.value = null
  }
}

async function carregarAtivosDisponiveisParaVinculo(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  loadingAtivosDisponiveis.value = true

  try {
    const response = await inventarioAtivosAdminService.listar({
      termo: termoBuscaAtivosDisponiveis.value.trim() || undefined,
      ativo: true,
      pagina: paginaAtivosDisponiveis.value,
      tamanhoPagina: tamanhoPaginaAtivosDisponiveis.value,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })

    ativosDisponiveis.value = response.items
    totalAtivosDisponiveis.value = response.total
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel buscar ativos para vinculo.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    loadingAtivosDisponiveis.value = false
  }
}

async function abrirModalVincularAtivo(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  termoBuscaAtivosDisponiveis.value = ''
  paginaAtivosDisponiveis.value = 1
  totalAtivosDisponiveis.value = 0
  ativosDisponiveis.value = []
  showVincularAtivo.value = true

  await carregarAtivosDisponiveisParaVinculo()
}

async function buscarAtivosDisponiveis(): Promise<void> {
  paginaAtivosDisponiveis.value = 1
  await carregarAtivosDisponiveisParaVinculo()
}

async function alterarPaginaAtivosDisponiveis(value: number): Promise<void> {
  paginaAtivosDisponiveis.value = value
  await carregarAtivosDisponiveisParaVinculo()
}

async function vincularAtivoChamado(ativo: InventarioAtivoListagem): Promise<void> {
  if (!detalhe.value) {
    return
  }

  vinculandoAtivoId.value = ativo.id

  try {
    detalhe.value = await chamadoInventarioAtivoService.vincularAtivo(detalhe.value.id, ativo.id)
    await carregarAtivoVinculadoDetalhe()
    await carregarAtivosDisponiveisParaVinculo()
    $q.notify({ type: 'positive', message: 'Ativo vinculado ao chamado com sucesso.' })
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel vincular o ativo ao chamado.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    vinculandoAtivoId.value = null
  }
}

function prepararRemocaoAtivoVinculado(): void {
  showConfirmarRemocaoAtivo.value = true
}

async function confirmarRemocaoAtivoVinculado(): Promise<void> {
  if (!detalhe.value?.inventarioAtivoId) {
    return
  }

  removendoVinculoAtivo.value = true

  try {
    detalhe.value = await chamadoInventarioAtivoService.removerAtivo(detalhe.value.id)
    ativoVinculadoDetalhe.value = null
    showConfirmarRemocaoAtivo.value = false
    $q.notify({ type: 'positive', message: 'Vinculo do ativo removido com sucesso.' })

    if (showVincularAtivo.value) {
      await carregarAtivosDisponiveisParaVinculo()
    }
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel remover o vinculo do ativo.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    removendoVinculoAtivo.value = false
  }
}

function abrirArtigoConhecimento(artigoId: string): void {
  router.push(`/admin/conhecimento/base-conhecimento/${artigoId}`)
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
              <q-item-label caption>Subcategoria</q-item-label>
              <q-item-label>{{ detalhe.subcategoria || '-' }}</q-item-label>
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
              <q-item-label caption>Tipo de solicitação</q-item-label>
              <q-item-label>{{ detalhe.tipoSolicitacao || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Local / Unidade</q-item-label>
              <q-item-label>{{ detalhe.localUnidade || '-' }}</q-item-label>
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

      <AppSectionCard
        v-if="podeVisualizar"
        titulo="Ativo vinculado"
        subtitulo="Rastreie o ativo principal associado ao chamado e ajuste o vinculo quando necessario."
      >
        <template #actions>
          <div class="row q-gutter-xs">
            <q-btn
              v-if="podeVincularAtivoInventario"
              color="primary"
              flat
              icon="link"
              label="Vincular ativo"
              @click="abrirModalVincularAtivo"
            />
            <q-btn
              v-if="podeVincularAtivoInventario && detalhe.inventarioAtivoId"
              color="negative"
              flat
              icon="link_off"
              label="Remover vinculo"
              :loading="removendoVinculoAtivo"
              @click="prepararRemocaoAtivoVinculado"
            />
          </div>
        </template>

        <EmptyState
          v-if="!detalhe.inventarioAtivoId"
          titulo="Nenhum ativo vinculado"
          mensagem="Este chamado ainda nao possui ativo associado."
          icon="inventory_2"
        />

        <q-list v-else separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Codigo</q-item-label>
              <q-item-label>{{ detalhe.inventarioAtivoCodigo || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Nome</q-item-label>
              <q-item-label>{{ detalhe.inventarioAtivoNome || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Patrimonio</q-item-label>
              <q-item-label>{{ ativoVinculadoDetalhe?.numeroPatrimonio || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Serie</q-item-label>
              <q-item-label>{{ ativoVinculadoDetalhe?.numeroSerie || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Status operacional</q-item-label>
              <q-item-label>{{ ativoVinculadoDetalhe?.statusOperacionalDescricao || '-' }}</q-item-label>
            </q-item-section>
            <q-item-section side>
              <q-btn
                v-if="detalhe.inventarioAtivoId"
                flat
                color="primary"
                icon="open_in_new"
                label="Abrir ativo"
                @click="router.push(`/admin/infraestrutura/inventario-ativos/${detalhe.inventarioAtivoId}`)"
              />
            </q-item-section>
          </q-item>
        </q-list>
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

      <AppSectionCard
        v-if="podeVincularArtigoConhecimento"
        titulo="Base de conhecimento"
        subtitulo="Vincule artigos publicados para acelerar respostas e padronizar atendimentos."
      >
        <template #actions>
          <q-btn color="primary" icon="add_link" label="Vincular artigo" :disable="loadingArtigosConhecimento" @click="abrirModalVincularArtigo" />
        </template>

        <LoadingState v-if="loadingArtigosConhecimento" inline mensagem="Carregando artigos vinculados..." />

        <ErrorState
          v-else-if="erroBaseConhecimento && !artigosConhecimento.length"
          titulo="Falha ao carregar artigos vinculados"
          :mensagem="erroBaseConhecimento"
          @retry="carregarArtigosConhecimento"
        />

        <EmptyState
          v-else-if="!artigosConhecimento.length"
          titulo="Nenhum artigo vinculado"
          mensagem="Vincule artigos da base de conhecimento para facilitar orientacoes e retornos."
          icon="article"
        />

        <q-list v-else bordered separator>
          <q-item v-for="artigo in artigosConhecimento" :key="artigo.artigoId">
            <q-item-section>
              <q-item-label class="text-weight-medium">{{ artigo.titulo }}</q-item-label>
              <q-item-label caption>{{ artigo.resumo || 'Sem resumo informado.' }}</q-item-label>
              <div class="row q-gutter-xs q-mt-xs">
                <q-chip dense square text-color="white" :color="statusArtigoColor(artigo.status)">
                  {{ artigo.statusDescricao }}
                </q-chip>
                <q-chip dense square text-color="white" :color="visibilidadeArtigoColor(artigo.visibilidade)">
                  {{ artigo.visibilidadeDescricao }}
                </q-chip>
                <q-chip v-if="artigo.categoriaNome" dense square color="grey-3" text-color="grey-9">
                  {{ artigo.categoriaNome }}
                </q-chip>
              </div>
              <q-item-label caption class="q-mt-xs">
                Vinculado por {{ artigo.vinculadoPorUsuario }} em {{ formatarData(artigo.vinculadoEm) }}
              </q-item-label>
              <q-item-label v-if="artigo.observacao" caption class="text-grey-8">
                Observação: {{ artigo.observacao }}
              </q-item-label>
            </q-item-section>

            <q-item-section side top>
              <div class="column q-gutter-xs items-end">
                <q-btn
                  flat
                  dense
                  color="primary"
                  icon="open_in_new"
                  label="Abrir artigo"
                  @click="abrirArtigoConhecimento(artigo.artigoId)"
                />
                <q-btn
                  flat
                  dense
                  color="negative"
                  icon="link_off"
                  label="Remover vínculo"
                  :loading="removendoArtigoId === artigo.artigoId"
                  @click="prepararRemocaoVinculo(artigo)"
                />
              </div>
            </q-item-section>
          </q-item>
        </q-list>
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
      :subcategorias="contexto?.subcategorias ?? []"
      :tipos-solicitacao="contexto?.tiposSolicitacao ?? []"
      :locais-unidade="contexto?.locaisUnidade ?? []"
      :departamentos="contexto?.departamentos ?? []"
      :valores-iniciais="{
        categoriaId: detalhe?.categoriaId ?? null,
        subcategoriaId: detalhe?.subcategoriaId ?? null,
        tipoSolicitacaoId: detalhe?.tipoSolicitacaoId ?? null,
        localUnidadeId: detalhe?.localUnidadeId ?? null,
        departamentoId: detalhe?.departamentoId ?? null,
      }"
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

    <q-dialog v-model="showVincularArtigo">
      <q-card class="sgx-card vinculo-artigo-dialog-card">
        <q-card-section class="row items-center q-gutter-sm">
          <div class="text-h6">Vincular artigo da base de conhecimento</div>
        </q-card-section>

        <q-card-section>
          <q-form class="column q-gutter-sm" @submit.prevent="buscarArtigosDisponiveis">
            <div class="row q-col-gutter-sm">
              <div class="col-12 col-md-6">
                <q-input
                  v-model="termoBuscaArtigosDisponiveis"
                  outlined
                  label="Busca"
                  placeholder="Titulo, resumo, conteudo ou tags"
                  :disable="loadingArtigosDisponiveis"
                />
              </div>

              <div class="col-12 col-md-4">
                <q-select
                  v-model="categoriaBuscaArtigosDisponiveis"
                  outlined
                  clearable
                  emit-value
                  map-options
                  label="Categoria"
                  :disable="loadingArtigosDisponiveis"
                  :options="(contexto?.categorias ?? []).map((item) => ({ label: item.nome, value: item.id }))"
                />
              </div>

              <div class="col-12 col-md-2">
                <q-btn
                  class="full-width"
                  color="primary"
                  icon="search"
                  label="Buscar"
                  type="submit"
                  :loading="loadingArtigosDisponiveis"
                />
              </div>
            </div>
          </q-form>
        </q-card-section>

        <q-card-section>
          <LoadingState v-if="loadingArtigosDisponiveis && !artigosDisponiveis.length" inline mensagem="Buscando artigos disponíveis..." />

          <EmptyState
            v-else-if="!artigosDisponiveis.length"
            titulo="Nenhum artigo disponível"
            mensagem="Nao ha artigos publicados elegiveis para vinculo com os filtros atuais."
            icon="search_off"
          />

          <q-list v-else bordered separator>
            <q-item v-for="artigo in artigosDisponiveis" :key="artigo.artigoId">
              <q-item-section>
                <q-item-label class="text-weight-medium">{{ artigo.titulo }}</q-item-label>
                <q-item-label caption>{{ artigo.resumo || 'Sem resumo informado.' }}</q-item-label>
                <div class="row q-gutter-xs q-mt-xs">
                  <q-chip dense square text-color="white" :color="statusArtigoColor(artigo.status)">
                    {{ artigo.statusDescricao }}
                  </q-chip>
                  <q-chip dense square text-color="white" :color="visibilidadeArtigoColor(artigo.visibilidade)">
                    {{ artigo.visibilidadeDescricao }}
                  </q-chip>
                  <q-chip v-if="artigo.categoriaNome" dense square color="grey-3" text-color="grey-9">
                    {{ artigo.categoriaNome }}
                  </q-chip>
                </div>
                <q-item-label caption class="q-mt-xs">
                  Publicado em {{ formatarData(artigo.publicadoEm) }}
                </q-item-label>
              </q-item-section>

              <q-item-section side top>
                <q-btn
                  color="positive"
                  dense
                  flat
                  icon="add_link"
                  label="Vincular"
                  :loading="vinculandoArtigoId === artigo.artigoId"
                  @click="vincularArtigo(artigo)"
                />
              </q-item-section>
            </q-item>
          </q-list>

          <div v-if="totalPaginasArtigosDisponiveis > 1" class="row justify-center q-mt-md">
            <q-pagination
              :model-value="paginaArtigosDisponiveis"
              :max="totalPaginasArtigosDisponiveis"
              :max-pages="6"
              boundary-links
              direction-links
              color="primary"
              @update:model-value="alterarPaginaArtigosDisponiveis"
            />
          </div>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Fechar" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="showVincularAtivo">
      <q-card class="sgx-card vinculo-ativo-dialog-card">
        <q-card-section class="row items-center q-gutter-sm">
          <div class="text-h6">Vincular ativo de inventario</div>
        </q-card-section>

        <q-card-section>
          <q-form class="row q-col-gutter-sm" @submit.prevent="buscarAtivosDisponiveis">
            <div class="col-12 col-md-9">
              <q-input
                v-model="termoBuscaAtivosDisponiveis"
                outlined
                label="Busca"
                placeholder="Codigo, nome, patrimonio, serie..."
                :disable="loadingAtivosDisponiveis"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-btn
                class="full-width"
                color="primary"
                icon="search"
                label="Buscar"
                type="submit"
                :loading="loadingAtivosDisponiveis"
              />
            </div>
          </q-form>
        </q-card-section>

        <q-card-section>
          <LoadingState v-if="loadingAtivosDisponiveis && !ativosDisponiveis.length" inline mensagem="Buscando ativos..." />

          <EmptyState
            v-else-if="!ativosDisponiveis.length"
            titulo="Nenhum ativo disponivel"
            mensagem="Nao ha ativos ativos disponiveis para vinculo com os filtros atuais."
            icon="search_off"
          />

          <q-list v-else bordered separator>
            <q-item v-for="ativo in ativosDisponiveis" :key="ativo.id">
              <q-item-section>
                <q-item-label class="text-weight-medium">{{ ativo.codigo }} - {{ ativo.nome }}</q-item-label>
                <q-item-label caption>
                  Tipo: {{ ativo.tipoAtivoInventarioNome }} | Patrimonio: {{ ativo.numeroPatrimonio || '-' }} | Serie:
                  {{ ativo.numeroSerie || '-' }}
                </q-item-label>
                <q-item-label caption>
                  Status: {{ ativo.statusOperacionalDescricao }} | Criticidade: {{ ativo.criticidadeDescricao }}
                </q-item-label>
              </q-item-section>

              <q-item-section side>
                <q-btn
                  color="positive"
                  dense
                  flat
                  icon="link"
                  label="Vincular"
                  :loading="vinculandoAtivoId === ativo.id"
                  @click="vincularAtivoChamado(ativo)"
                />
              </q-item-section>
            </q-item>
          </q-list>

          <div v-if="totalPaginasAtivosDisponiveis > 1" class="row justify-center q-mt-md">
            <q-pagination
              :model-value="paginaAtivosDisponiveis"
              :max="totalPaginasAtivosDisponiveis"
              :max-pages="6"
              boundary-links
              direction-links
              color="primary"
              @update:model-value="alterarPaginaAtivosDisponiveis"
            />
          </div>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Fechar" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <ConfirmDialog
      v-model="showConfirmarRemocaoVinculo"
      titulo="Remover vínculo"
      :mensagem="mensagemConfirmarRemocaoVinculo"
      confirmar-label="Remover"
      color="negative"
      :loading="!!removendoArtigoId"
      @confirm="confirmarRemocaoVinculo"
      @cancel="cancelarRemocaoVinculo"
    />

    <ConfirmDialog
      v-model="showConfirmarRemocaoAtivo"
      titulo="Remover vinculo de ativo"
      :mensagem="mensagemConfirmarRemocaoAtivo"
      confirmar-label="Remover"
      color="negative"
      :loading="removendoVinculoAtivo"
      @confirm="confirmarRemocaoAtivoVinculado"
    />
  </q-page>
</template>

<style scoped>
.comment-dialog-card {
  width: min(560px, 92vw);
}

.vinculo-artigo-dialog-card {
  width: min(1024px, 96vw);
  max-height: 90vh;
}

.vinculo-ativo-dialog-card {
  width: min(980px, 96vw);
  max-height: 90vh;
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
