<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import ComentariosAdministrativos from '../components/admin/ComentariosAdministrativos.vue'
import ChamadoRelacionamentosSection from '../components/admin/ChamadoRelacionamentosSection.vue'
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
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { TipoCampoFormularioServico } from '../types/formularioServicos'
import { chamadoBaseConhecimentoService } from '../services/chamadoBaseConhecimentoService'
import { chamadoInventarioAtivoService } from '../services/chamadoInventarioAtivoService'
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { aprovacaoChamadosAdminService } from '../services/aprovacaoChamadosAdminService'
import { aprovacoesMotorService } from '../services/aprovacoesMotorService'
import { useAuthStore } from '../stores/authStore'
import { StatusAprovacaoChamado, TipoOrigemAprovacaoChamado } from '../types/aprovacaoChamados'
import { StatusInstanciaAprovacaoChamado, TipoRegraAprovacao, EfeitoOperacionalRegraAprovacao } from '../types/aprovacoesMotor'
import type { InstanciaAprovacaoChamadoResumoResponse } from '../types/aprovacoesMotor'
import type {
  AdminContextoResponse,
  ArtigoConhecimentoDisponivelParaVinculo,
  ChamadoAdminDetalhe,
  ChamadoArtigoConhecimento,
  FilaAtendimentoGrupoTecnicoResponse,
  GrupoTecnicoResumo,
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
const showAcaoAprovacao = ref(false)
const showSolicitarAprovacao = ref(false)
const showConfirmarAssumirFila = ref(false)
const showTransferirGrupo = ref(false)

const comentarioMensagem = ref('')
const comentarioInterno = ref(false)
const transferenciaGrupoForm = reactive({
  grupoTecnicoId: '',
  filaAtendimentoId: '',
})
const acaoAprovacaoSelecionada = ref<'aprovar' | 'reprovar' | 'cancelar' | null>(null)
const justificativaAcaoAprovacao = ref('')
const origemDescricaoSolicitacaoAprovacao = ref('')
const justificativaSolicitacaoAprovacao = ref('')
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
const gruposTransferencia = ref<GrupoTecnicoResumo[]>([])
const filasTransferencia = ref<FilaAtendimentoGrupoTecnicoResponse[]>([])
const loadingGruposTransferencia = ref(false)
const loadingFilasTransferencia = ref(false)

const aprovacoesMotor = ref<InstanciaAprovacaoChamadoResumoResponse[]>([])
const loadingAprovacoesMotor = ref(false)
const erroAprovacoesMotor = ref<string | null>(null)

const showModalAprovarMotor = ref(false)
const processingAprovarMotor = ref(false)
const pendenciaMotorSelecionada = ref<InstanciaAprovacaoChamadoResumoResponse | null>(null)
const justificativaAprovacaoMotor = ref('')

const showModalReprovarMotor = ref(false)
const processingReprovarMotor = ref(false)
const pendenciaReprovarMotorSelecionada = ref<InstanciaAprovacaoChamadoResumoResponse | null>(null)
const justificativaReprovacaoMotor = ref('')
const observacaoReprovacaoMotor = ref('')

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const usuarioEhAtendente = computed(() => (authStore.usuario?.perfis ?? []).includes('Atendente'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeGerenciarOrquestracao = computed(() =>
  fallbackAdminSemPermissoes.value || usuarioEhAdministrador.value || usuarioEhAtendente.value
)
const podeVincularArtigoConhecimento = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.baseConhecimentoVincularChamado)
)
const podeVincularAtivoInventario = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.inventarioAtivosVincularChamado)
)
const podeVisualizar = computed(
  () =>
    fallbackAdminSemPermissoes.value ||
    authStore.possuiAlgumaPermissao([permissoes.chamadosVisualizar, permissoes.chamadosVisualizarTodos])
)
const podeSolicitarAprovacaoPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.aprovacaoChamadosGerenciar)
)
const podeAprovarAprovacaoPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.aprovacaoChamadosAprovar)
)
const podeReprovarAprovacaoPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.aprovacaoChamadosReprovar)
)
const podeCancelarAprovacaoPermissao = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.aprovacaoChamadosCancelar)
)

const acoesDisponiveisSet = computed(() => new Set(detalhe.value?.acoesDisponiveisCodigos ?? []))
const podeAssumir = computed(() => acoesDisponiveisSet.value.has('Assumir'))
const podeAssumirFila = computed(
  () =>
    Boolean(detalhe.value?.grupoTecnicoId) &&
    Boolean(detalhe.value?.filaAtendimentoId) &&
    !detalhe.value?.responsavel &&
    Boolean(contexto.value?.usuario.id) &&
    (usuarioEhAdministrador.value || usuarioEhAtendente.value)
)
const podeTransferirGrupoTecnico = computed(
  () =>
    Boolean(detalhe.value?.grupoTecnicoId) &&
    (usuarioEhAdministrador.value || usuarioEhAtendente.value)
)
const podeAtribuir = computed(() => acoesDisponiveisSet.value.has('Atribuir'))
const podeAlterarStatus = computed(() => acoesDisponiveisSet.value.has('AlterarStatus'))
const podeAlterarPrioridade = computed(() => acoesDisponiveisSet.value.has('AlterarPrioridade'))
const podeAlterarCategoria = computed(() => acoesDisponiveisSet.value.has('AlterarCategoria'))
const podeComentar = computed(() => acoesDisponiveisSet.value.has('Comentar'))
const podeEncerrar = computed(() => acoesDisponiveisSet.value.has('Encerrar'))
const podeReabrir = computed(() => acoesDisponiveisSet.value.has('Reabrir'))
const statusAprovacaoDescricao = computed(() => {
  if (detalhe.value?.statusAprovacao === null || detalhe.value?.statusAprovacao === undefined) {
    return 'Nao aplicavel'
  }

  switch (detalhe.value.statusAprovacao) {
    case StatusAprovacaoChamado.Pendente:
      return 'Pendente'
    case StatusAprovacaoChamado.Aprovado:
      return 'Aprovado'
    case StatusAprovacaoChamado.Reprovado:
      return 'Reprovado'
    case StatusAprovacaoChamado.Cancelado:
      return 'Cancelado'
    default:
      return 'Nao informado'
  }
})
const corStatusAprovacao = computed(() => {
  if (detalhe.value?.statusAprovacao === null || detalhe.value?.statusAprovacao === undefined) {
    return 'grey-6'
  }

  switch (detalhe.value.statusAprovacao) {
    case StatusAprovacaoChamado.Pendente:
      return 'warning'
    case StatusAprovacaoChamado.Aprovado:
      return 'positive'
    case StatusAprovacaoChamado.Reprovado:
      return 'negative'
    case StatusAprovacaoChamado.Cancelado:
      return 'grey-7'
    default:
      return 'grey-6'
  }
})
const aprovacaoPendente = computed(() => detalhe.value?.aprovacaoPendente ?? false)
const aprovacaoReprovada = computed(() => detalhe.value?.statusAprovacao === StatusAprovacaoChamado.Reprovado)
const aprovacaoCancelada = computed(() => detalhe.value?.statusAprovacao === StatusAprovacaoChamado.Cancelado)
const aprovacaoAprovada = computed(() => detalhe.value?.statusAprovacao === StatusAprovacaoChamado.Aprovado)
const podeSolicitarAprovacaoManual = computed(
  () =>
    Boolean(detalhe.value) &&
    podeSolicitarAprovacaoPermissao.value &&
    !aprovacaoPendente.value
)
const podeAprovarAprovacaoRapido = computed(
  () =>
    Boolean(detalhe.value?.aprovacaoChamadoId) &&
    aprovacaoPendente.value &&
    podeAprovarAprovacaoPermissao.value
)
const podeReprovarAprovacaoRapido = computed(
  () =>
    Boolean(detalhe.value?.aprovacaoChamadoId) &&
    aprovacaoPendente.value &&
    podeReprovarAprovacaoPermissao.value
)
const podeCancelarAprovacaoRapido = computed(
  () =>
    Boolean(detalhe.value?.aprovacaoChamadoId) &&
    aprovacaoPendente.value &&
    podeCancelarAprovacaoPermissao.value
)
const statusDisponiveisParaNatureza = computed(() => {
  const todosStatus = contexto.value?.status ?? []
  const codigosPermitidos = detalhe.value?.statusPermitidosCodigos ?? []

  if (!codigosPermitidos.length) {
    return todosStatus
  }

  const codigos = new Set<number>(codigosPermitidos)
  return todosStatus.filter((status) => codigos.has(status.codigo))
})
const justificativaAcaoAprovacaoObrigatoria = computed(
  () => acaoAprovacaoSelecionada.value === 'reprovar' || acaoAprovacaoSelecionada.value === 'cancelar'
)
const tituloAcaoAprovacao = computed(() => {
  if (acaoAprovacaoSelecionada.value === 'aprovar') return 'Confirmar aprovacao'
  if (acaoAprovacaoSelecionada.value === 'reprovar') return 'Confirmar reprovacao'
  return 'Confirmar cancelamento'
})
const labelAcaoAprovacao = computed(() => {
  if (acaoAprovacaoSelecionada.value === 'aprovar') return 'Aprovar'
  if (acaoAprovacaoSelecionada.value === 'reprovar') return 'Reprovar'
  return 'Cancelar aprovacao'
})
const corAcaoAprovacao = computed(() => {
  if (acaoAprovacaoSelecionada.value === 'aprovar') return 'positive'
  if (acaoAprovacaoSelecionada.value === 'reprovar') return 'negative'
  return 'warning'
})

const temAprovacaoMotorPendente = computed(() => aprovacoesMotor.value.some(a => a.status === StatusInstanciaAprovacaoChamado.Pendente))
const temAprovacaoMotorBloqueante = computed(() => aprovacoesMotor.value.some(a => a.status === StatusInstanciaAprovacaoChamado.Pendente && a.bloqueante))
const temAprovacaoMotorEmReavaliacao = computed(() => aprovacoesMotor.value.some(a => a.status === StatusInstanciaAprovacaoChamado.EmReavaliacao))
const temAprovacaoMotorInformativa = computed(() => aprovacoesMotor.value.some(a => a.status === StatusInstanciaAprovacaoChamado.Pendente && !a.bloqueante))
const aprovacaoMotorAprovada = computed(() => aprovacoesMotor.value.length > 0 && aprovacoesMotor.value.every(a => a.status === StatusInstanciaAprovacaoChamado.Aprovado))
const aprovacaoMotorReprovada = computed(() => aprovacoesMotor.value.some(a => a.status === StatusInstanciaAprovacaoChamado.Reprovado))

const opcoesGruposTransferencia = computed(() =>
  gruposTransferencia.value
    .filter((grupo) => grupo.id !== detalhe.value?.grupoTecnicoId)
    .map((grupo) => ({ label: grupo.nome, value: grupo.id }))
)
const opcoesFilasTransferencia = computed(() =>
  filasTransferencia.value.map((fila) => ({ label: fila.nome, value: fila.id }))
)
const grupoDestinoIgualAtual = computed(
  () => Boolean(transferenciaGrupoForm.grupoTecnicoId) && transferenciaGrupoForm.grupoTecnicoId === detalhe.value?.grupoTecnicoId
)

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
const mensagemConfirmarAssumirFila = computed(() => {
  const fila = detalhe.value?.filaAtendimentoNome ? ` da fila "${detalhe.value.filaAtendimentoNome}"` : ' da fila'
  return `Deseja assumir este chamado${fila}?`
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
const totalComentarios = computed(() => detalhe.value?.comentarios.length ?? 0)
const totalEventosHistorico = computed(() => detalhe.value?.historico.length ?? 0)
const totalAnexos = computed(() => detalhe.value?.anexos.length ?? 0)
const totalEventosSla = computed(() => detalhe.value?.historicoSla.length ?? 0)
const slaEmRisco = computed(() => detalhe.value?.sla?.situacao === 'ProximoDoVencimento' || detalhe.value?.sla?.estaVencido)
const possuiRespostasFormulario = computed(() => (detalhe.value?.respostasFormulario?.length ?? 0) > 0)

function formatarData(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

function labelNaturezaChamado(value: number): string {
  switch (value) {
    case 1: return 'Incidente'
    case 2: return 'Requisicao'
    case 3: return 'Mudanca'
    case 4: return 'Problema'
    case 5: return 'Evento/Alerta'
    case 6: return 'Tarefa Operacional'
    default: return `#${value}`
  }
}

function labelImpactoChamado(value: number): string {
  switch (value) {
    case 1: return 'Baixo'
    case 2: return 'Medio'
    case 3: return 'Alto'
    default: return `#${value}`
  }
}

function labelUrgenciaChamado(value: number): string {
  switch (value) {
    case 1: return 'Baixa'
    case 2: return 'Media'
    case 3: return 'Alta'
    default: return `#${value}`
  }
}

function formatarTipoCampoFormulario(tipo: TipoCampoFormularioServico): string {
  switch (tipo) {
    case TipoCampoFormularioServico.TextoCurto:
      return 'Texto curto'
    case TipoCampoFormularioServico.TextoLongo:
      return 'Texto longo'
    case TipoCampoFormularioServico.Numero:
      return 'Numero'
    case TipoCampoFormularioServico.Data:
      return 'Data'
    case TipoCampoFormularioServico.Booleano:
      return 'Booleano'
    case TipoCampoFormularioServico.SelecaoUnica:
      return 'Selecao unica'
    case TipoCampoFormularioServico.SelecaoMultipla:
      return 'Selecao multipla'
    default:
      return 'Nao informado'
  }
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

  await carregarAprovacoesMotor()
}

async function carregarAprovacoesMotor(): Promise<void> {
  loadingAprovacoesMotor.value = true
  erroAprovacoesMotor.value = null

  try {
    const response = await aprovacoesMotorService.listarPendenciasPorChamado(chamadoId)
    aprovacoesMotor.value = response.items || []
  } catch (error) {
    erroAprovacoesMotor.value = extrairMensagemErro(error, 'Não foi possível carregar o status de aprovação.')
  } finally {
    loadingAprovacoesMotor.value = false
  }
}

function abrirAprovarMotorModal(pendencia: InstanciaAprovacaoChamadoResumoResponse): void {
  pendenciaMotorSelecionada.value = pendencia
  justificativaAprovacaoMotor.value = ''
  showModalAprovarMotor.value = true
}

function fecharAprovarMotorModal(): void {
  showModalAprovarMotor.value = false
  pendenciaMotorSelecionada.value = null
  justificativaAprovacaoMotor.value = ''
}

async function confirmarAprovacaoMotor(): Promise<void> {
  if (!pendenciaMotorSelecionada.value) return

  processingAprovarMotor.value = true
  
  try {
    await aprovacoesMotorService.aprovarAprovacao({
      instanciaAprovacaoChamadoId: pendenciaMotorSelecionada.value.id,
      decisaoFinal: true, // Aprovação simples por padrão no frontend
      justificativa: justificativaAprovacaoMotor.value || null,
      observacao: justificativaAprovacaoMotor.value || null
    })
    
    fecharAprovarMotorModal()
    await carregarAprovacoesMotor()
    
    // Atualizar também o chamado se necessário (dependendo do efeito da aprovação)
    await recarregarDetalhe()

    registrarSucesso('Aprovação registrada com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível registrar a aprovação.')
  } finally {
    processingAprovarMotor.value = false
  }
}

function abrirReprovarMotorModal(pendencia: InstanciaAprovacaoChamadoResumoResponse): void {
  pendenciaReprovarMotorSelecionada.value = pendencia
  justificativaReprovacaoMotor.value = ''
  observacaoReprovacaoMotor.value = ''
  showModalReprovarMotor.value = true
}

function fecharReprovarMotorModal(): void {
  showModalReprovarMotor.value = false
  pendenciaReprovarMotorSelecionada.value = null
  justificativaReprovacaoMotor.value = ''
  observacaoReprovacaoMotor.value = ''
}

async function confirmarReprovacaoMotor(): Promise<void> {
  if (!pendenciaReprovarMotorSelecionada.value) return
  if (!justificativaReprovacaoMotor.value || !justificativaReprovacaoMotor.value.trim()) {
    registrarErro(new Error('Informe a justificativa da rejeição.'), 'Não foi possível registrar a rejeição.')
    return
  }

  processingReprovarMotor.value = true
  
  try {
    await aprovacoesMotorService.reprovarAprovacao({
      instanciaAprovacaoChamadoId: pendenciaReprovarMotorSelecionada.value.id,
      decisaoFinal: true, // Reprovação simples por padrão no frontend
      justificativa: justificativaReprovacaoMotor.value.trim(),
      observacao: observacaoReprovacaoMotor.value || null
    })
    
    fecharReprovarMotorModal()
    await carregarAprovacoesMotor()
    
    // Atualizar também o chamado se necessário (dependendo do efeito da aprovação)
    await recarregarDetalhe()

    registrarSucesso('Rejeição registrada com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível registrar a rejeição.')
  } finally {
    processingReprovarMotor.value = false
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
  if (!podeAssumir.value) {
    registrarErro(new Error('A ação Assumir não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.assumirChamado(detalhe.value.id)
    await recarregarDetalhe()
    registrarSucesso('Chamado assumido com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

async function assumirFila(): Promise<void> {
  if (!detalhe.value || !contexto.value?.usuario.id) return
  if (!podeAssumirFila.value) {
    registrarErro(new Error('A acao Assumir da fila nao esta disponivel para este chamado no estado atual.'), 'Nao foi possivel concluir a acao.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.assumirChamadoFila(detalhe.value.id, { usuarioId: contexto.value.usuario.id })
    await recarregarDetalhe()
    showConfirmarAssumirFila.value = false
    registrarSucesso('Chamado assumido da fila com sucesso.')
  } catch (error) {
    registrarErro(error, 'Nao foi possivel assumir o chamado da fila.')
  } finally {
    processing.value = false
  }
}

function limparTransferenciaGrupo(): void {
  transferenciaGrupoForm.grupoTecnicoId = ''
  transferenciaGrupoForm.filaAtendimentoId = ''
  filasTransferencia.value = []
}

async function carregarGruposTransferencia(): Promise<void> {
  loadingGruposTransferencia.value = true

  try {
    const response = await adminService.listarGruposTecnicos({
      ativo: true,
      pagina: 1,
      tamanhoPagina: 100,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })
    gruposTransferencia.value = response.items
  } catch (error) {
    registrarErro(error, 'Nao foi possivel carregar os grupos tecnicos.')
  } finally {
    loadingGruposTransferencia.value = false
  }
}

async function carregarFilasTransferencia(grupoTecnicoId: string): Promise<void> {
  filasTransferencia.value = []
  transferenciaGrupoForm.filaAtendimentoId = ''

  if (!grupoTecnicoId) {
    return
  }

  loadingFilasTransferencia.value = true

  try {
    filasTransferencia.value = await adminService.listarFilasAtendimentoGrupoTecnico(grupoTecnicoId, { ativo: true })
  } catch (error) {
    registrarErro(error, 'Nao foi possivel carregar as filas do grupo tecnico.')
  } finally {
    loadingFilasTransferencia.value = false
  }
}

async function abrirTransferenciaGrupo(): Promise<void> {
  if (!podeTransferirGrupoTecnico.value) {
    registrarErro(new Error('A acao Transferir grupo nao esta disponivel para este chamado no estado atual.'), 'Nao foi possivel concluir a acao.')
    return
  }

  limparTransferenciaGrupo()
  showTransferirGrupo.value = true
  await carregarGruposTransferencia()
}

async function aoSelecionarGrupoTransferencia(grupoTecnicoId: string | null): Promise<void> {
  await carregarFilasTransferencia(grupoTecnicoId ?? '')
}

async function transferirGrupoTecnico(): Promise<void> {
  if (!detalhe.value) return

  if (!transferenciaGrupoForm.grupoTecnicoId) {
    registrarErro(new Error('Selecione o grupo tecnico de destino.'), 'Nao foi possivel concluir a acao.')
    return
  }

  if (grupoDestinoIgualAtual.value) {
    registrarErro(new Error('Selecione um grupo tecnico diferente do grupo atual.'), 'Nao foi possivel concluir a acao.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.transferirGrupoTecnicoChamado(detalhe.value.id, {
      grupoTecnicoId: transferenciaGrupoForm.grupoTecnicoId,
      filaAtendimentoId: transferenciaGrupoForm.filaAtendimentoId || null,
    })
    await recarregarDetalhe()
    showTransferirGrupo.value = false
    limparTransferenciaGrupo()
    registrarSucesso('Chamado transferido para outro grupo tecnico com sucesso.')
  } catch (error) {
    registrarErro(error, 'Nao foi possivel transferir o chamado para outro grupo tecnico.')
  } finally {
    processing.value = false
  }
}

async function atribuir(responsavelId: string): Promise<void> {
  if (!detalhe.value) return
  if (!podeAtribuir.value) {
    registrarErro(new Error('A ação Atribuir não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.atribuirChamado(detalhe.value.id, { responsavelId })
    await recarregarDetalhe()
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
  if (!podeAlterarStatus.value) {
    registrarErro(new Error('A ação Alterar status não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.alterarStatus(detalhe.value.id, { statusId })
    await recarregarDetalhe()
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
  if (!podeAlterarPrioridade.value) {
    registrarErro(new Error('A ação Alterar prioridade não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.alterarPrioridade(detalhe.value.id, { prioridadeId })
    await recarregarDetalhe()
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
  if (!podeAlterarCategoria.value) {
    registrarErro(new Error('A ação Alterar categoria não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.alterarCategoria(detalhe.value.id, payload)
    await recarregarDetalhe()
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
  if (!podeComentar.value) {
    registrarErro(new Error('A ação Comentar não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

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
  if (!podeEncerrar.value) {
    registrarErro(new Error('A ação Encerrar não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.encerrarChamado(detalhe.value.id, payload)
    await recarregarDetalhe()
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
  if (!podeReabrir.value) {
    registrarErro(new Error('A ação Reabrir não está disponível para este chamado no estado atual.'), 'Não foi possível concluir a ação.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    await adminService.reabrirChamado(detalhe.value.id, { mensagem })
    await recarregarDetalhe()
    showReabrir.value = false
    registrarSucesso('Chamado reaberto com sucesso.')
  } catch (error) {
    registrarErro(error, 'Não foi possível concluir a ação.')
  } finally {
    processing.value = false
  }
}

function abrirAcaoAprovacao(tipo: 'aprovar' | 'reprovar' | 'cancelar'): void {
  acaoAprovacaoSelecionada.value = tipo
  justificativaAcaoAprovacao.value = ''
  showAcaoAprovacao.value = true
}

function fecharAcaoAprovacao(): void {
  showAcaoAprovacao.value = false
  acaoAprovacaoSelecionada.value = null
  justificativaAcaoAprovacao.value = ''
}

function abrirSolicitarAprovacaoManual(): void {
  origemDescricaoSolicitacaoAprovacao.value = ''
  justificativaSolicitacaoAprovacao.value = ''
  showSolicitarAprovacao.value = true
}

function abrirDetalheAprovacao(): void {
  if (!detalhe.value?.aprovacaoChamadoId) {
    return
  }

  router.push(`/admin/atendimento/aprovacao-chamados/${detalhe.value.aprovacaoChamadoId}`)
}

async function confirmarAcaoAprovacao(): Promise<void> {
  if (!detalhe.value?.aprovacaoChamadoId || !acaoAprovacaoSelecionada.value) {
    return
  }

  if (justificativaAcaoAprovacaoObrigatoria.value && !justificativaAcaoAprovacao.value.trim()) {
    registrarErro(new Error('Informe a justificativa para continuar.'), 'Nao foi possivel concluir a acao.')
    return
  }

  processing.value = true
  erro.value = null

  try {
    const justificativa = justificativaAcaoAprovacao.value.trim()

    if (acaoAprovacaoSelecionada.value === 'aprovar') {
      await aprovacaoChamadosAdminService.aprovar(detalhe.value.aprovacaoChamadoId, {
        justificativaDecisao: justificativa || undefined,
      })
      registrarSucesso('Aprovacao registrada com sucesso.')
    } else if (acaoAprovacaoSelecionada.value === 'reprovar') {
      await aprovacaoChamadosAdminService.reprovar(detalhe.value.aprovacaoChamadoId, {
        justificativaDecisao: justificativa,
      })
      registrarSucesso('Reprovacao registrada com sucesso.')
    } else {
      await aprovacaoChamadosAdminService.cancelar(detalhe.value.aprovacaoChamadoId, {
        justificativaDecisao: justificativa,
      })
      registrarSucesso('Cancelamento registrado com sucesso.')
    }

    fecharAcaoAprovacao()
    await recarregarDetalhe()
    await carregarAprovacoesMotor()
  } catch (error) {
    registrarErro(error, 'Nao foi possivel concluir a acao de aprovacao.')
  } finally {
    processing.value = false
  }
}

async function solicitarAprovacaoManual(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  processing.value = true
  erro.value = null

  try {
    await aprovacaoChamadosAdminService.solicitar(detalhe.value.id, {
      tipoOrigem: TipoOrigemAprovacaoChamado.Manual,
      origemDescricao: origemDescricaoSolicitacaoAprovacao.value.trim() || undefined,
      justificativaSolicitacao: justificativaSolicitacaoAprovacao.value.trim() || undefined,
    })

    showSolicitarAprovacao.value = false
    origemDescricaoSolicitacaoAprovacao.value = ''
    justificativaSolicitacaoAprovacao.value = ''
    await recarregarDetalhe()
    await carregarAprovacoesMotor()
    registrarSucesso('Solicitacao de aprovacao registrada com sucesso.')
  } catch (error) {
    registrarErro(error, 'Nao foi possivel solicitar aprovacao manual.')
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
    await chamadoInventarioAtivoService.vincularAtivo(detalhe.value.id, ativo.id)
    await recarregarDetalhe()
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
    await chamadoInventarioAtivoService.removerAtivo(detalhe.value.id)
    await recarregarDetalhe()
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
      contexto="Operacao administrativa"
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
      <div class="sgx-kpi-grid">
        <MetricCard
          title="Responsavel"
          :value="detalhe.responsavel?.nome || 'Nao atribuido'"
          caption="Responsavel atual"
          icon="person"
          :tone="detalhe.responsavel ? 'info' : 'warning'"
        />
        <MetricCard
          title="Grupo tecnico"
          :value="detalhe.grupoTecnicoNome || 'Sem grupo tecnico'"
          caption="Grupo responsavel pelo atendimento"
          icon="groups"
          :tone="detalhe.grupoTecnicoNome ? 'info' : 'warning'"
        />
        <MetricCard
          title="Fila"
          :value="detalhe.filaAtendimentoNome || 'Sem fila'"
          caption="Fila de atendimento atual"
          icon="list_alt"
          :tone="detalhe.filaAtendimentoNome ? 'primary' : 'warning'"
        />
        <MetricCard
          title="SLA em risco"
          :value="slaEmRisco ? 'Sim' : 'Nao'"
          :caption="detalhe.sla?.situacao || 'NaoAplicavel'"
          icon="monitoring"
          :tone="slaEmRisco ? 'negative' : 'positive'"
        />
        <MetricCard title="Comentarios" :value="totalComentarios" caption="Registro de tratativas" icon="forum" tone="primary" />
        <MetricCard title="Historico" :value="totalEventosHistorico" caption="Eventos administrativos" icon="timeline" tone="info" />
        <MetricCard title="Historico de SLA" :value="totalEventosSla" caption="Mudancas de prazo e alerta" icon="schedule" tone="warning" />
        <MetricCard title="Anexos" :value="totalAnexos" caption="Evidencias associadas" icon="attach_file" tone="primary" />
      </div>

      <AppSectionCard titulo="Resumo do chamado" subtitulo="Dados principais para triagem e acompanhamento.">
        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Código</q-item-label>
              <q-item-label>{{ detalhe.codigo }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Natureza ITSM</q-item-label>
              <q-item-label>{{ labelNaturezaChamado(detalhe.naturezaChamado) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Impacto</q-item-label>
              <q-item-label>{{ labelImpactoChamado(detalhe.impactoChamado) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Urgencia</q-item-label>
              <q-item-label>{{ labelUrgenciaChamado(detalhe.urgenciaChamado) }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Prioridade</q-item-label>
              <q-item-label>
                <PrioridadeBadge :texto="detalhe.prioridade" />
              </q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
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
              <q-item-label caption>Grupo tecnico</q-item-label>
              <q-item-label>{{ detalhe.grupoTecnicoNome || 'Sem grupo tecnico' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Fila de atendimento</q-item-label>
              <q-item-label>{{ detalhe.filaAtendimentoNome || 'Sem fila' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Responsavel</q-item-label>
              <q-item-label>{{ detalhe.responsavel?.nome || 'Não atribuído' }}</q-item-label>
            </q-item-section>
          </q-item>

          <q-item>
            <q-item-section>
              <q-item-label caption>Solicitante</q-item-label>
              <q-item-label>{{ detalhe.solicitante.nome }} ({{ detalhe.solicitante.email }})</q-item-label>
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

      <AppSectionCard titulo="Aprovacao do motor ITSM" subtitulo="Acompanhe o status e as pendências de aprovação deste chamado via motor ITSM.">
        <LoadingState v-if="loadingAprovacoesMotor" inline mensagem="Carregando status de aprovação..." />
        <ErrorState
          v-else-if="erroAprovacoesMotor"
          titulo="Falha ao carregar status de aprovação"
          :mensagem="erroAprovacoesMotor"
          @retry="carregarAprovacoesMotor"
        />
        <template v-else>
          <q-banner v-if="temAprovacaoMotorBloqueante" rounded class="bg-orange-1 text-orange-10 q-mb-sm">
            Este chamado possui aprovacao pendente bloqueante. Algumas acoes sensiveis podem estar bloqueadas ate a decisao.
          </q-banner>
          <q-banner v-else-if="temAprovacaoMotorInformativa" rounded class="bg-info text-white q-mb-sm">
            Existe aprovacao pendente informativa, mas ela nao bloqueia o atendimento comum.
          </q-banner>
          <q-banner v-else-if="temAprovacaoMotorEmReavaliacao" rounded class="bg-amber-1 text-amber-10 q-mb-sm">
            A aprovacao precisa ser reavaliada porque dados sensiveis do chamado foram alterados.
          </q-banner>
          <q-banner v-else-if="aprovacaoMotorReprovada" rounded class="bg-red-1 text-negative q-mb-sm">
            A aprovacao foi reprovada. Verifique a justificativa e as proximas acoes permitidas.
          </q-banner>
          <q-banner v-else-if="aprovacaoMotorAprovada" rounded class="bg-green-1 text-positive q-mb-sm">
            A aprovacao foi concedida para o escopo registrado.
          </q-banner>
          <q-banner v-else-if="aprovacoesMotor.length === 0" rounded class="bg-grey-2 text-grey-8 q-mb-sm">
            Este chamado nao possui pendencia de aprovacao no motor atual.
          </q-banner>

          <q-list separator v-if="aprovacoesMotor.length > 0">
            <q-item v-for="aprovacao in aprovacoesMotor" :key="aprovacao.id">
              <q-item-section>
                <q-item-label caption>Status</q-item-label>
                <q-item-label>
                  <q-chip dense square text-color="white" :color="
                    aprovacao.status === StatusInstanciaAprovacaoChamado.Pendente ? 'warning' :
                    aprovacao.status === StatusInstanciaAprovacaoChamado.Aprovado ? 'positive' :
                    aprovacao.status === StatusInstanciaAprovacaoChamado.Reprovado ? 'negative' :
                    aprovacao.status === StatusInstanciaAprovacaoChamado.Cancelado || aprovacao.status === StatusInstanciaAprovacaoChamado.Expirado ? 'grey-7' : 'amber'
                  ">
                    {{ StatusInstanciaAprovacaoChamado[aprovacao.status] }}
                  </q-chip>
                </q-item-label>
              </q-item-section>
              <q-item-section>
                <q-item-label caption>Regra</q-item-label>
                <q-item-label>{{ aprovacao.nomeRegra || 'Regra não informada' }}</q-item-label>
              </q-item-section>
              <q-item-section>
                <q-item-label caption>Origem/Tipo Regra</q-item-label>
                <q-item-label>{{ TipoRegraAprovacao[aprovacao.tipoRegra] }}</q-item-label>
              </q-item-section>
              <q-item-section>
                <q-item-label caption>Efeito Operacional</q-item-label>
                <q-item-label>
                  <span v-if="aprovacao.efeitoOperacional === EfeitoOperacionalRegraAprovacao.Informativa">Informativa</span>
                  <span v-else-if="aprovacao.efeitoOperacional === EfeitoOperacionalRegraAprovacao.BloqueiaEncerramento">Bloqueia Encerramento</span>
                  <span v-else-if="aprovacao.efeitoOperacional === EfeitoOperacionalRegraAprovacao.BloqueiaAtendimento">Bloqueia Atendimento</span>
                </q-item-label>
              </q-item-section>
              <q-item-section>
                <q-item-label caption>Solicitada Em</q-item-label>
                <q-item-label>{{ formatarData(aprovacao.solicitadaEm) }}</q-item-label>
              </q-item-section>
              <q-item-section v-if="aprovacao.vencimentoEm">
                <q-item-label caption>Prazo/Vencimento</q-item-label>
                <q-item-label>{{ formatarData(aprovacao.vencimentoEm) }}</q-item-label>
              </q-item-section>
              <q-item-section side>
                <q-btn
                  v-if="aprovacao.status === StatusInstanciaAprovacaoChamado.Pendente || aprovacao.status === StatusInstanciaAprovacaoChamado.EmReavaliacao"
                  color="positive"
                  icon="check_circle"
                  label="Aprovar"
                  size="sm"
                  unelevated
                  @click="abrirAprovarMotorModal(aprovacao)"
                  class="q-mr-sm"
                />
                <q-btn
                  v-if="aprovacao.status === StatusInstanciaAprovacaoChamado.Pendente || aprovacao.status === StatusInstanciaAprovacaoChamado.EmReavaliacao"
                  color="negative"
                  icon="cancel"
                  label="Rejeitar"
                  size="sm"
                  unelevated
                  @click="abrirReprovarMotorModal(aprovacao)"
                />
              </q-item-section>
            </q-item>
          </q-list>

          <q-dialog v-model="showModalAprovarMotor" persistent>
            <q-card style="min-width: 400px">
              <q-card-section class="row items-center q-pb-none">
                <div class="text-h6">Aprovar pendência</div>
                <q-space />
                <q-btn icon="close" flat round dense v-close-popup />
              </q-card-section>

              <q-card-section>
                <p>Confirma a aprovação desta pendência do motor ITSM?</p>
                <q-input
                  v-model="justificativaAprovacaoMotor"
                  type="textarea"
                  label="Observação (opcional)"
                  outlined
                  autogrow
                  :rules="[val => !val || val.length <= 500 || 'Máximo de 500 caracteres']"
                />
              </q-card-section>

              <q-card-actions align="right" class="text-primary">
                <q-btn flat label="Cancelar" @click="fecharAprovarMotorModal" :disable="processingAprovarMotor" />
                <q-btn color="positive" label="Confirmar aprovação" @click="confirmarAprovacaoMotor" :loading="processingAprovarMotor" />
              </q-card-actions>
            </q-card>
          </q-dialog>

          <q-dialog v-model="showModalReprovarMotor" persistent>
            <q-card style="min-width: 400px">
              <q-card-section class="row items-center q-pb-none">
                <div class="text-h6">Rejeitar pendência</div>
                <q-space />
                <q-btn icon="close" flat round dense v-close-popup />
              </q-card-section>

              <q-card-section>
                <p>Informe a justificativa para rejeitar esta pendência do motor ITSM.</p>
                <q-input
                  v-model="justificativaReprovacaoMotor"
                  type="textarea"
                  label="Justificativa *"
                  outlined
                  autogrow
                  :rules="[val => (val && val.trim().length > 0) || 'Justificativa é obrigatória', val => val.length <= 500 || 'Máximo de 500 caracteres']"
                  class="q-mb-md"
                />
                <q-input
                  v-model="observacaoReprovacaoMotor"
                  type="textarea"
                  label="Observação (opcional)"
                  outlined
                  autogrow
                  :rules="[val => !val || val.length <= 500 || 'Máximo de 500 caracteres']"
                />
              </q-card-section>

              <q-card-actions align="right" class="text-primary">
                <q-btn flat label="Cancelar" @click="fecharReprovarMotorModal" :disable="processingReprovarMotor" />
                <q-btn color="negative" label="Confirmar rejeição" @click="confirmarReprovacaoMotor" :loading="processingReprovarMotor" />
              </q-card-actions>
            </q-card>
          </q-dialog>
        </template>
      </AppSectionCard>

      <AppSectionCard titulo="Aprovacao legada" subtitulo="Controle de liberacao do chamado para seguimento do atendimento.">
        <template #actions>
          <div class="row q-gutter-xs">
            <q-btn
              v-if="detalhe.aprovacaoChamadoId"
              flat
              color="primary"
              icon="visibility"
              label="Ver aprovacao"
              @click="abrirDetalheAprovacao"
            />
            <q-btn
              v-if="podeSolicitarAprovacaoManual"
              flat
              color="secondary"
              icon="add_task"
              label="Solicitar aprovacao"
              @click="abrirSolicitarAprovacaoManual"
            />
          </div>
        </template>

        <q-banner v-if="aprovacaoPendente" rounded class="bg-orange-1 text-orange-10 q-mb-sm">
          Este chamado aguarda aprovacao antes de seguir para atendimento.
        </q-banner>
        <q-banner v-else-if="aprovacaoReprovada" rounded class="bg-red-1 text-negative q-mb-sm">
          Este chamado foi reprovado e permanece bloqueado para avancar atendimento.
        </q-banner>
        <q-banner v-else-if="aprovacaoCancelada" rounded class="bg-grey-2 text-grey-8 q-mb-sm">
          A aprovacao foi cancelada. Sem outra pendencia ativa, o chamado pode seguir fluxo normal.
        </q-banner>
        <q-banner v-else-if="aprovacaoAprovada" rounded class="bg-green-1 text-positive q-mb-sm">
          Aprovacao concluida. Chamado liberado para atendimento.
        </q-banner>

        <q-list separator>
          <q-item>
            <q-item-section>
              <q-item-label caption>Requer aprovacao</q-item-label>
              <q-item-label>{{ detalhe.requerAprovacao ? 'Sim' : 'Nao' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Aprovacao pendente</q-item-label>
              <q-item-label>{{ detalhe.aprovacaoPendente ? 'Sim' : 'Nao' }}</q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Status da aprovacao</q-item-label>
              <q-item-label>
                <q-chip dense square text-color="white" :color="corStatusAprovacao">
                  {{ statusAprovacaoDescricao }}
                </q-chip>
              </q-item-label>
            </q-item-section>
            <q-item-section>
              <q-item-label caption>Id da aprovacao</q-item-label>
              <q-item-label>{{ detalhe.aprovacaoChamadoId || '-' }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <div
          v-if="podeAprovarAprovacaoRapido || podeReprovarAprovacaoRapido || podeCancelarAprovacaoRapido"
          class="row q-gutter-sm q-mt-md"
        >
          <q-btn
            v-if="podeAprovarAprovacaoRapido"
            color="positive"
            icon="check"
            label="Aprovar"
            :loading="processing"
            @click="abrirAcaoAprovacao('aprovar')"
          />
          <q-btn
            v-if="podeReprovarAprovacaoRapido"
            color="negative"
            icon="close"
            label="Reprovar"
            :loading="processing"
            @click="abrirAcaoAprovacao('reprovar')"
          />
          <q-btn
            v-if="podeCancelarAprovacaoRapido"
            color="warning"
            text-color="black"
            icon="cancel"
            label="Cancelar"
            :loading="processing"
            @click="abrirAcaoAprovacao('cancelar')"
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
        <q-banner
          v-if="!statusDisponiveisParaNatureza.length"
          rounded
          class="bg-amber-1 text-dark q-mb-sm"
        >
          Nao ha status compativeis disponiveis para a natureza ITSM deste chamado.
        </q-banner>
        <PainelAtendimento
          :chamado="detalhe"
          :loading="processing"
          :can-assumir="podeAssumir"
          :can-assumir-fila="podeAssumirFila"
          :can-transferir-grupo="podeTransferirGrupoTecnico"
          :can-atribuir="podeAtribuir"
          :can-alterar-status="podeAlterarStatus"
          :can-alterar-prioridade="podeAlterarPrioridade"
          :can-alterar-categoria="podeAlterarCategoria"
          :can-comentar="podeComentar"
          :can-encerrar="podeEncerrar"
          :can-reabrir="podeReabrir"
          @assumir="assumir"
          @assumir-fila="showConfirmarAssumirFila = true"
          @transferir-grupo="abrirTransferenciaGrupo"
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
        v-if="possuiRespostasFormulario"
        titulo="Respostas do formulario"
        subtitulo="Informacoes preenchidas na abertura guiada e disponiveis para o atendimento administrativo."
      >
        <q-list separator>
          <q-item v-for="resposta in detalhe.respostasFormulario" :key="resposta.campoFormularioServicoId">
            <q-item-section>
              <div class="row items-start justify-between q-col-gutter-md">
                <div class="col">
                  <q-item-label class="text-weight-medium">{{ resposta.rotulo }}</q-item-label>
                  <q-item-label caption>{{ resposta.nome }}</q-item-label>
                </div>

                <div class="col-auto">
                  <q-chip dense square color="grey-3" text-color="grey-9" icon="fact_check">
                    {{ formatarTipoCampoFormulario(resposta.tipo) }}
                  </q-chip>
                </div>
              </div>

              <q-item-label v-if="resposta.valor" class="text-body2 q-mt-sm">{{ resposta.valor }}</q-item-label>

              <div v-else-if="resposta.valores.length" class="row q-gutter-sm q-mt-sm">
                <q-chip
                  v-for="valor in resposta.valores"
                  :key="`${resposta.campoFormularioServicoId}-${valor}`"
                  dense
                  square
                  color="primary"
                  text-color="white"
                >
                  {{ valor }}
                </q-chip>
              </div>
            </q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>

      <ChamadoRelacionamentosSection
        :chamado-id="detalhe.id"
        :can-manage="podeGerenciarOrquestracao"
      />

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

    <q-dialog v-model="showAcaoAprovacao">
      <q-card class="sgx-card" style="width: min(640px, 94vw)">
        <q-card-section class="text-h6">{{ tituloAcaoAprovacao }}</q-card-section>

        <q-card-section class="column q-gutter-sm">
          <div class="text-body2 text-grey-8">
            Esta decisao sera registrada no historico da aprovacao e do chamado.
          </div>

          <q-input
            v-model="justificativaAcaoAprovacao"
            outlined
            type="textarea"
            autogrow
            :label="justificativaAcaoAprovacaoObrigatoria ? 'Justificativa (obrigatoria)' : 'Justificativa (opcional)'"
            :rules="justificativaAcaoAprovacaoObrigatoria ? [(v) => !!String(v || '').trim() || 'Informe a justificativa'] : []"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Voltar" :disable="processing" @click="fecharAcaoAprovacao" />
          <q-btn :color="corAcaoAprovacao" :label="labelAcaoAprovacao" :loading="processing" @click="confirmarAcaoAprovacao" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="showSolicitarAprovacao">
      <q-card class="sgx-card" style="width: min(700px, 94vw)">
        <q-card-section class="text-h6">Solicitar aprovacao manual</q-card-section>

        <q-card-section class="column q-gutter-sm">
          <q-banner rounded class="bg-blue-1 text-primary">
            Esta solicitacao usa origem manual e segue as validacoes do backend.
          </q-banner>

          <q-input
            outlined
            model-value="Manual"
            label="Tipo de origem"
            disable
          />
          <q-input
            v-model="origemDescricaoSolicitacaoAprovacao"
            outlined
            label="Origem descricao"
            maxlength="200"
            :disable="processing"
          />
          <q-input
            v-model="justificativaSolicitacaoAprovacao"
            outlined
            type="textarea"
            autogrow
            label="Justificativa da solicitacao"
            maxlength="2000"
            :disable="processing"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" :disable="processing" @click="showSolicitarAprovacao = false" />
          <q-btn color="primary" label="Solicitar aprovacao" :loading="processing" @click="solicitarAprovacaoManual" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <ModalAtribuirResponsavel
      v-model="showAtribuir"
      :atendentes="contexto?.atendentes ?? []"
      :loading="processing"
      @confirmar="atribuir"
    />

    <ModalAlterarStatus
      v-model="showStatus"
      :status="statusDisponiveisParaNatureza"
      :loading="processing"
      @confirmar="alterarStatus"
    />

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

    <q-dialog v-model="showTransferirGrupo">
      <q-card class="sgx-card transferencia-grupo-dialog-card">
        <q-card-section>
          <div class="text-h6">Transferir chamado para outro grupo tecnico</div>
          <div class="text-body2 text-grey-7 q-mt-xs">
            O backend validara grupo, fila e regras de transferencia antes de atualizar o chamado.
          </div>
        </q-card-section>

        <q-card-section class="column q-gutter-md">
          <q-banner rounded class="bg-blue-1 text-primary">
            A transferencia remove o responsavel atual conforme regra de negocio e recarrega o detalhe apos sucesso.
          </q-banner>

          <q-select
            v-model="transferenciaGrupoForm.grupoTecnicoId"
            outlined
            emit-value
            map-options
            label="Grupo tecnico de destino"
            :options="opcoesGruposTransferencia"
            :loading="loadingGruposTransferencia"
            :disable="processing || loadingGruposTransferencia"
            :rules="[(v) => !!v || 'Selecione o grupo tecnico de destino']"
            @update:model-value="aoSelecionarGrupoTransferencia"
          >
            <template #no-option>
              <q-item>
                <q-item-section class="text-grey-7">Nenhum grupo tecnico ativo disponivel.</q-item-section>
              </q-item>
            </template>
          </q-select>

          <q-banner v-if="grupoDestinoIgualAtual" rounded class="bg-amber-1 text-dark">
            Selecione um grupo tecnico diferente do grupo atual.
          </q-banner>

          <q-select
            v-model="transferenciaGrupoForm.filaAtendimentoId"
            outlined
            clearable
            emit-value
            map-options
            label="Fila de destino (opcional)"
            :options="opcoesFilasTransferencia"
            :loading="loadingFilasTransferencia"
            :disable="processing || loadingFilasTransferencia || !transferenciaGrupoForm.grupoTecnicoId"
          >
            <template #no-option>
              <q-item>
                <q-item-section class="text-grey-7">Nenhuma fila ativa disponivel para este grupo.</q-item-section>
              </q-item>
            </template>
          </q-select>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" :disable="processing" @click="showTransferirGrupo = false" />
          <q-btn
            color="primary"
            icon="move_up"
            label="Transferir grupo"
            :loading="processing"
            :disable="loadingGruposTransferencia || loadingFilasTransferencia || !transferenciaGrupoForm.grupoTecnicoId || grupoDestinoIgualAtual"
            @click="transferirGrupoTecnico"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>

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

    <ConfirmDialog
      v-model="showConfirmarAssumirFila"
      titulo="Assumir chamado da fila"
      :mensagem="mensagemConfirmarAssumirFila"
      confirmar-label="Assumir da fila"
      color="primary"
      :loading="processing"
      @confirm="assumirFila"
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

.transferencia-grupo-dialog-card {
  width: min(720px, 94vw);
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
