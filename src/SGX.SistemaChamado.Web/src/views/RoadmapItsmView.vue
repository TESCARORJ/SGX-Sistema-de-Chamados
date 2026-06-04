<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { roadmapItsmService } from '../services/roadmapItsmService'
import type {
  AtualizarRoadmapCategoriaRequest,
  AtualizarRoadmapChecklistItemRequest,
  AtualizarRoadmapImplementacaoFuturaRequest,
  AtualizarRoadmapItsmItemRequest,
  CriarRoadmapCategoriaRequest,
  CriarRoadmapChecklistItemRequest,
  CriarRoadmapImplementacaoFuturaRequest,
  RoadmapCategoriaResponse,
  RoadmapChecklistItemResponse,
  RoadmapImplementacaoFuturaResponse,
  RoadmapItsmDetalheResponse,
  RoadmapItsmResumoResponse,
} from '../types/roadmapItsm'
import { useAuthStore } from '../stores/authStore'
import { permissoes } from '../constants/permissoes'
import {
  grupoChecklistLabels,
  prioridadeImplementacaoLabels,
  roadmapDecisaoLabels,
  roadmapImpactoLabels,
  roadmapPrioridadeLabels,
  roadmapStatusLabels,
  statusImplementacaoFuturaLabels,
  statusImplementacaoLabels,
  statusTecnicoLabels,
  tipoImplementacaoLabels,
} from '../utils/roadmapLabels'

type ModoDetalhe = 'visualizar' | 'editar'
type ModoCategoria = 'criar' | 'editar'
type ModoChecklist = 'criar' | 'editar'
type ModoImplementacao = 'criar' | 'editar'

const authStore = useAuthStore()
const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const itens = ref<RoadmapItsmResumoResponse[]>([])
const categorias = ref<RoadmapCategoriaResponse[]>([])
const checklist = ref<RoadmapChecklistItemResponse[]>([])
const implementacoes = ref<RoadmapImplementacaoFuturaResponse[]>([])

const modalDetalheAberto = ref(false)
const modoDetalhe = ref<ModoDetalhe>('visualizar')
const detalheAtual = ref<RoadmapItsmDetalheResponse | null>(null)
const salvandoRoadmap = ref(false)

const modalCategoriaAberto = ref(false)
const modoCategoria = ref<ModoCategoria>('criar')
const categoriaSelecionada = ref<RoadmapCategoriaResponse | null>(null)

const modalChecklistAberto = ref(false)
const modoChecklist = ref<ModoChecklist>('criar')
const checklistSelecionado = ref<RoadmapChecklistItemResponse | null>(null)

const modalImplementacaoAberto = ref(false)
const modoImplementacao = ref<ModoImplementacao>('criar')
const implementacaoSelecionada = ref<RoadmapImplementacaoFuturaResponse | null>(null)

const confirmacaoAberta = ref(false)
const confirmacaoMensagem = ref('')
const confirmacaoAcao = ref<null | (() => Promise<void>)>(null)

const filtros = reactive({
  status: null as number | null,
  roadmapCategoriaId: null as string | null,
})

const formRoadmap = reactive({
  area: '',
  categoria: '',
  objetivo: '',
  roadmapCategoriaId: null as string | null,
  ordem: 1,
  situacaoAtual: '',
  atencaoTecnica: '',
  status: 2,
  prioridade: 2,
  impacto: 2,
  decisao: 4,
  responsavel: '',
  prazoAlvo: '',
  ativo: true,
  observacao: '',
  statusImplementacao: 0,
  statusTecnico: 0,
  percentualImplementacao: 0,
  pendenciasTecnicas: '',
  pendenciasHomologacao: '',
  evidenciaImplementacao: '',
  dataConclusaoTecnica: '',
  dataHomologacao: '',
  criterioAceite: '',
  proximaAcao: '',
})

const formCategoria = reactive({
  nome: '',
  descricao: '',
  cor: '',
  icone: '',
  ordem: 0,
  ativo: true,
})

const formChecklist = reactive({
  titulo: '',
  descricao: '',
  grupo: 1,
  ordem: 1,
  concluido: false,
  obrigatorio: true,
  ativo: true,
})

const formImplementacao = reactive({
  titulo: '',
  descricao: '',
  tipo: 1,
  prioridade: 2,
  status: 1,
  responsavel: '',
  prazoAlvo: '',
  dataConclusao: '',
  observacao: '',
  ativo: true,
})

const opcoesStatus = Object.entries(roadmapStatusLabels).map(([value, label]) => ({ value: Number(value), label: `${label} (legado/geral)` }))
const opcoesPrioridade = Object.entries(roadmapPrioridadeLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesImpacto = Object.entries(roadmapImpactoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesDecisao = Object.entries(roadmapDecisaoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesStatusImplementacao = Object.entries(statusImplementacaoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesStatusTecnico = Object.entries(statusTecnicoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesGrupoChecklist = Object.entries(grupoChecklistLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesTipoImplementacao = Object.entries(tipoImplementacaoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesPrioridadeImplementacao = Object.entries(prioridadeImplementacaoLabels).map(([value, label]) => ({ value: Number(value), label }))
const opcoesStatusImplementacaoFutura = Object.entries(statusImplementacaoFuturaLabels).map(([value, label]) => ({ value: Number(value), label }))

const opcoesCategoriasAtivas = computed(() =>
  categorias.value
    .filter((x) => x.ativo)
    .sort((a, b) => (a.ordem ?? 9999) - (b.ordem ?? 9999) || a.nome.localeCompare(b.nome))
    .map((x) => ({ value: x.id, label: x.nome, cor: x.cor, icone: x.icone }))
)
const totalItensAtivos = computed(() => itens.value.filter((item) => item.ativo).length)
const checklistOrdenado = computed(() =>
  [...checklist.value].sort((a, b) => a.ordem - b.ordem || a.grupo - b.grupo || a.titulo.localeCompare(b.titulo))
)
const totalConcluidos = computed(() => itens.value.filter((item) => item.statusImplementacao === 4).length)
const totalEmAndamento = computed(() => itens.value.filter((item) => item.statusImplementacao === 2).length)
const totalComPendencia = computed(() => itens.value.filter((item) => item.statusTecnico === 3).length)
const mediaPercentual = computed(() => {
  if (!itens.value.length) return 0
  const total = itens.value.reduce((acc, item) => acc + item.percentualImplementacao, 0)
  return Math.round(total / itens.value.length)
})

const podeGerenciarRoadmap = computed(
  () =>
    authStore.usuario?.perfis.includes('Administrador') &&
    authStore.possuiAlgumaPermissao([permissoes.roadmapGerenciar])
)

const podeGerenciarCategorias = computed(() => podeGerenciarRoadmap.value)
const podeGerenciarChecklist = computed(() => podeGerenciarRoadmap.value)
const podeGerenciarImplementacoes = computed(
  () =>
    authStore.usuario?.perfis.includes('Administrador') &&
    authStore.possuiAlgumaPermissao([permissoes.roadmapImplementacoesGerenciar])
)

const modoSomenteLeitura = computed(() => modoDetalhe.value === 'visualizar')
const percentualCalculadoPorChecklist = computed(() => detalheAtual.value?.percentualCalculadoPorChecklist ?? false)
const mostrarBannerImplementadoFuncionalmente = computed(() => formRoadmap.statusImplementacao === 3)
const mostrarPendenciasEvolutivas = computed(() => formRoadmap.statusTecnico === 3)

const colunasRoadmap: QTableColumn<RoadmapItsmResumoResponse>[] = [
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'left' },
  { name: 'area', label: 'Área', field: 'area', align: 'left', sortable: true },
  { name: 'categoria', label: 'Categoria', field: 'categoria', align: 'left' },
  { name: 'statusImplementacao', label: 'Status implementação', field: 'statusImplementacaoDescricao', align: 'left' },
  { name: 'statusTecnico', label: 'Status técnico', field: 'statusTecnicoDescricao', align: 'left' },
  { name: 'percentual', label: '%', field: 'percentualImplementacao', align: 'center' },
]

const colunasCategorias: QTableColumn<RoadmapCategoriaResponse>[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'cor', label: 'Cor', field: 'cor', align: 'left' },
  { name: 'icone', label: 'Ícone', field: 'icone', align: 'left' },
  { name: 'ordem', label: 'Ordem', field: 'ordem', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]

const colunasChecklist: QTableColumn<RoadmapChecklistItemResponse>[] = [
  { name: 'titulo', label: 'Título', field: 'titulo', align: 'left' },
  { name: 'grupo', label: 'Grupo', field: 'grupoDescricao', align: 'left' },
  { name: 'ordem', label: 'Ordem', field: 'ordem', align: 'center' },
  { name: 'concluido', label: 'Concluído', field: 'concluido', align: 'center' },
  { name: 'obrigatorio', label: 'Obrigatório', field: 'obrigatorio', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]

const colunasImplementacao: QTableColumn<RoadmapImplementacaoFuturaResponse>[] = [
  { name: 'titulo', label: 'Título', field: 'titulo', align: 'left' },
  { name: 'tipo', label: 'Tipo', field: 'tipoDescricao', align: 'left' },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridadeDescricao', align: 'left' },
  { name: 'status', label: 'Status', field: 'statusDescricao', align: 'left' },
  { name: 'responsavel', label: 'Responsável', field: 'responsavel', align: 'left' },
  { name: 'prazoAlvo', label: 'Prazo alvo', field: 'prazoAlvo', align: 'left' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]

function formatarDataCurta(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleDateString('pt-BR')
}

function abrirDocumentacaoItsm(): void {
  router.push('/admin/gestao-itsm/documentacao')
}

function labelStatusImplementacao(value: number): string {
  return statusImplementacaoLabels[value] ?? `#${value}`
}

function labelStatusTecnico(value: number): string {
  return statusTecnicoLabels[value] ?? `#${value}`
}

function carregarFormRoadmap(item: RoadmapItsmDetalheResponse): void {
  formRoadmap.area = item.area
  formRoadmap.categoria = item.categoria
  formRoadmap.objetivo = item.objetivo ?? ''
  formRoadmap.roadmapCategoriaId = item.roadmapCategoriaId
  formRoadmap.ordem = item.ordem
  formRoadmap.situacaoAtual = item.situacaoAtual
  formRoadmap.atencaoTecnica = item.atencaoTecnica
  formRoadmap.status = item.status
  formRoadmap.prioridade = item.prioridade
  formRoadmap.impacto = item.impacto
  formRoadmap.decisao = item.decisao
  formRoadmap.responsavel = item.responsavel ?? ''
  formRoadmap.prazoAlvo = item.prazoAlvo ? item.prazoAlvo.substring(0, 10) : ''
  formRoadmap.ativo = item.ativo
  formRoadmap.observacao = item.observacao ?? ''
  formRoadmap.statusImplementacao = item.statusImplementacao
  formRoadmap.statusTecnico = item.statusTecnico
  formRoadmap.percentualImplementacao = item.percentualImplementacao
  formRoadmap.pendenciasTecnicas = item.pendenciasTecnicas ?? ''
  formRoadmap.pendenciasHomologacao = item.pendenciasHomologacao ?? ''
  formRoadmap.evidenciaImplementacao = item.evidenciaImplementacao ?? ''
  formRoadmap.dataConclusaoTecnica = item.dataConclusaoTecnica ? item.dataConclusaoTecnica.substring(0, 10) : ''
  formRoadmap.dataHomologacao = item.dataHomologacao ? item.dataHomologacao.substring(0, 10) : ''
  formRoadmap.criterioAceite = item.criterioAceite ?? ''
  formRoadmap.proximaAcao = item.proximaAcao ?? ''
}

async function carregarCategorias(): Promise<void> {
  categorias.value = await roadmapItsmService.listarCategorias({})
}

async function carregarListaRoadmap(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    itens.value = await roadmapItsmService.listar({
      status: filtros.status ?? undefined,
      roadmapCategoriaId: filtros.roadmapCategoriaId ?? undefined,
      ativo: true,
    })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar roadmap.'
  } finally {
    loading.value = false
  }
}

async function abrirDetalhe(item: RoadmapItsmResumoResponse, modo: ModoDetalhe): Promise<void> {
  erro.value = null
  modalDetalheAberto.value = true
  modoDetalhe.value = modo
  detalheAtual.value = null
  checklist.value = []
  implementacoes.value = []
  try {
    detalheAtual.value = await roadmapItsmService.obterPorId(item.id)
    carregarFormRoadmap(detalheAtual.value)
    checklist.value = await roadmapItsmService.listarChecklistPorItem(item.id)
    implementacoes.value = await roadmapItsmService.listarImplementacoesPorItem(item.id)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao abrir detalhe.'
  }
}

async function recarregarDetalheAtual(): Promise<void> {
  if (!detalheAtual.value) return
  detalheAtual.value = await roadmapItsmService.obterPorId(detalheAtual.value.id)
  carregarFormRoadmap(detalheAtual.value)
  checklist.value = await roadmapItsmService.listarChecklistPorItem(detalheAtual.value.id)
  implementacoes.value = await roadmapItsmService.listarImplementacoesPorItem(detalheAtual.value.id)
}

async function salvarRoadmap(): Promise<void> {
  if (!detalheAtual.value) return
  salvandoRoadmap.value = true
  erro.value = null
  try {
    const payload: AtualizarRoadmapItsmItemRequest = {
      area: formRoadmap.area,
      categoria: formRoadmap.categoria || (opcoesCategoriasAtivas.value.find((x) => x.value === formRoadmap.roadmapCategoriaId)?.label ?? ''),
      objetivo: formRoadmap.objetivo || null,
      roadmapCategoriaId: formRoadmap.roadmapCategoriaId,
      ordem: formRoadmap.ordem,
      situacaoAtual: formRoadmap.situacaoAtual,
      atencaoTecnica: formRoadmap.atencaoTecnica,
      status: formRoadmap.status,
      prioridade: formRoadmap.prioridade,
      impacto: formRoadmap.impacto,
      decisao: formRoadmap.decisao,
      responsavel: formRoadmap.responsavel || null,
      prazoAlvo: formRoadmap.prazoAlvo || null,
      ativo: formRoadmap.ativo,
      observacao: formRoadmap.observacao || null,
      statusImplementacao: formRoadmap.statusImplementacao,
      statusTecnico: formRoadmap.statusTecnico,
      percentualImplementacao: percentualCalculadoPorChecklist.value ? null : formRoadmap.percentualImplementacao,
      pendenciasTecnicas: formRoadmap.pendenciasTecnicas || null,
      pendenciasHomologacao: formRoadmap.pendenciasHomologacao || null,
      evidenciaImplementacao: formRoadmap.evidenciaImplementacao || null,
      dataConclusaoTecnica: formRoadmap.dataConclusaoTecnica || null,
      dataHomologacao: formRoadmap.dataHomologacao || null,
      criterioAceite: formRoadmap.criterioAceite || null,
      proximaAcao: formRoadmap.proximaAcao || null,
    }

    const atualizado = await roadmapItsmService.atualizar(detalheAtual.value.id, payload)
    detalheAtual.value = atualizado
    carregarFormRoadmap(atualizado)
    sucesso.value = 'Roadmap atualizado com sucesso.'
    await carregarListaRoadmap()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar roadmap.'
  } finally {
    salvandoRoadmap.value = false
  }
}

function abrirConfirmacao(mensagem: string, acao: () => Promise<void>): void {
  confirmacaoMensagem.value = mensagem
  confirmacaoAcao.value = acao
  confirmacaoAberta.value = true
}

async function executarConfirmacao(): Promise<void> {
  if (!confirmacaoAcao.value) return
  try {
    await confirmacaoAcao.value()
  } finally {
    confirmacaoAberta.value = false
    confirmacaoAcao.value = null
  }
}

function abrirModalCategoria(modo: ModoCategoria, categoria?: RoadmapCategoriaResponse): void {
  modoCategoria.value = modo
  categoriaSelecionada.value = categoria ?? null
  formCategoria.nome = categoria?.nome ?? ''
  formCategoria.descricao = categoria?.descricao ?? ''
  formCategoria.cor = categoria?.cor ?? ''
  formCategoria.icone = categoria?.icone ?? ''
  formCategoria.ordem = categoria?.ordem ?? 0
  formCategoria.ativo = categoria?.ativo ?? true
  modalCategoriaAberto.value = true
}

async function salvarCategoria(): Promise<void> {
  try {
    if (modoCategoria.value === 'criar') {
      const payload: CriarRoadmapCategoriaRequest = {
        nome: formCategoria.nome,
        descricao: formCategoria.descricao || null,
        cor: formCategoria.cor || null,
        icone: formCategoria.icone || null,
        ordem: formCategoria.ordem || null,
      }
      await roadmapItsmService.criarCategoria(payload)
      sucesso.value = 'Categoria criada com sucesso.'
    } else if (categoriaSelecionada.value) {
      const payload: AtualizarRoadmapCategoriaRequest = {
        nome: formCategoria.nome,
        descricao: formCategoria.descricao || null,
        cor: formCategoria.cor || null,
        icone: formCategoria.icone || null,
        ordem: formCategoria.ordem || null,
        ativo: formCategoria.ativo,
      }
      await roadmapItsmService.atualizarCategoria(categoriaSelecionada.value.id, payload)
      sucesso.value = 'Categoria atualizada com sucesso.'
    }
    modalCategoriaAberto.value = false
    await carregarCategorias()
    await carregarListaRoadmap()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar categoria.'
  }
}

function abrirModalChecklist(modo: ModoChecklist, item?: RoadmapChecklistItemResponse): void {
  modoChecklist.value = modo
  checklistSelecionado.value = item ?? null
  formChecklist.titulo = item?.titulo ?? ''
  formChecklist.descricao = item?.descricao ?? ''
  formChecklist.grupo = item?.grupo ?? 1
  formChecklist.ordem = item?.ordem ?? 1
  formChecklist.concluido = item?.concluido ?? false
  formChecklist.obrigatorio = item?.obrigatorio ?? true
  formChecklist.ativo = item?.ativo ?? true
  modalChecklistAberto.value = true
}

async function salvarChecklist(): Promise<void> {
  if (!detalheAtual.value) return
  try {
    if (modoChecklist.value === 'criar') {
      const payload: CriarRoadmapChecklistItemRequest = {
        titulo: formChecklist.titulo,
        descricao: formChecklist.descricao || null,
        grupo: formChecklist.grupo,
        ordem: formChecklist.ordem,
        concluido: formChecklist.concluido,
        obrigatorio: formChecklist.obrigatorio,
      }
      await roadmapItsmService.criarChecklist(detalheAtual.value.id, payload)
      sucesso.value = 'Checklist criado com sucesso.'
    } else if (checklistSelecionado.value) {
      const payload: AtualizarRoadmapChecklistItemRequest = {
        titulo: formChecklist.titulo,
        descricao: formChecklist.descricao || null,
        grupo: formChecklist.grupo,
        ordem: formChecklist.ordem,
        concluido: formChecklist.concluido,
        obrigatorio: formChecklist.obrigatorio,
        ativo: formChecklist.ativo,
      }
      await roadmapItsmService.atualizarChecklist(checklistSelecionado.value.id, payload)
      sucesso.value = 'Checklist atualizado com sucesso.'
    }
    modalChecklistAberto.value = false
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar checklist.'
  }
}

function concluirChecklist(item: RoadmapChecklistItemResponse): void {
  abrirConfirmacao('Concluir item do checklist?', async () => {
    await roadmapItsmService.concluirChecklist(item.id)
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  })
}

function reabrirChecklist(item: RoadmapChecklistItemResponse): void {
  abrirConfirmacao('Reabrir item do checklist?', async () => {
    await roadmapItsmService.reabrirChecklist(item.id)
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  })
}

function inativarChecklist(item: RoadmapChecklistItemResponse): void {
  abrirConfirmacao('Inativar item do checklist? Ele deixará de contar no percentual, mas continuará preservado no histórico.', async () => {
    await roadmapItsmService.inativarChecklist(item.id)
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  })
}

function reativarChecklist(item: RoadmapChecklistItemResponse): void {
  abrirConfirmacao('Reativar item do checklist?', async () => {
    await roadmapItsmService.reativarChecklist(item.id)
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  })
}

function abrirModalImplementacao(modo: ModoImplementacao, item?: RoadmapImplementacaoFuturaResponse): void {
  modoImplementacao.value = modo
  implementacaoSelecionada.value = item ?? null
  formImplementacao.titulo = item?.titulo ?? ''
  formImplementacao.descricao = item?.descricao ?? ''
  formImplementacao.tipo = item?.tipo ?? 1
  formImplementacao.prioridade = item?.prioridade ?? 2
  formImplementacao.status = item?.status ?? 1
  formImplementacao.responsavel = item?.responsavel ?? ''
  formImplementacao.prazoAlvo = item?.prazoAlvo ? item.prazoAlvo.substring(0, 10) : ''
  formImplementacao.dataConclusao = item?.dataConclusao ? item.dataConclusao.substring(0, 10) : ''
  formImplementacao.observacao = item?.observacao ?? ''
  formImplementacao.ativo = item?.ativo ?? true
  modalImplementacaoAberto.value = true
}

async function salvarImplementacao(): Promise<void> {
  if (!detalheAtual.value) return
  try {
    if (modoImplementacao.value === 'criar') {
      const payload: CriarRoadmapImplementacaoFuturaRequest = {
        roadmapItemId: detalheAtual.value.id,
        titulo: formImplementacao.titulo,
        descricao: formImplementacao.descricao || null,
        tipo: formImplementacao.tipo,
        prioridade: formImplementacao.prioridade,
        status: formImplementacao.status,
        responsavel: formImplementacao.responsavel || null,
        prazoAlvo: formImplementacao.prazoAlvo || null,
        observacao: formImplementacao.observacao || null,
      }
      await roadmapItsmService.criarImplementacao(payload)
      sucesso.value = 'Implementação criada com sucesso.'
    } else if (implementacaoSelecionada.value) {
      const payload: AtualizarRoadmapImplementacaoFuturaRequest = {
        titulo: formImplementacao.titulo,
        descricao: formImplementacao.descricao || null,
        tipo: formImplementacao.tipo,
        prioridade: formImplementacao.prioridade,
        status: formImplementacao.status,
        responsavel: formImplementacao.responsavel || null,
        prazoAlvo: formImplementacao.prazoAlvo || null,
        dataConclusao: formImplementacao.dataConclusao || null,
        observacao: formImplementacao.observacao || null,
        ativo: formImplementacao.ativo,
      }
      await roadmapItsmService.atualizarImplementacao(implementacaoSelecionada.value.id, payload)
      sucesso.value = 'Implementação atualizada com sucesso.'
    }
    modalImplementacaoAberto.value = false
    await recarregarDetalheAtual()
    await carregarListaRoadmap()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar implementação.'
  }
}

function concluirImplementacao(item: RoadmapImplementacaoFuturaResponse): void {
  abrirConfirmacao('Concluir implementacao futura?', async () => {
    await roadmapItsmService.concluirImplementacao(item.id)
    await recarregarDetalheAtual()
  })
}

function inativarImplementacao(item: RoadmapImplementacaoFuturaResponse): void {
  abrirConfirmacao('Inativar implementacao futura?', async () => {
    await roadmapItsmService.inativarImplementacao(item.id)
    await recarregarDetalheAtual()
  })
}

function reativarImplementacao(item: RoadmapImplementacaoFuturaResponse): void {
  abrirConfirmacao('Reativar implementacao futura?', async () => {
    await roadmapItsmService.reativarImplementacao(item.id)
    await recarregarDetalheAtual()
  })
}

onMounted(async () => {
  await carregarCategorias()
  await carregarListaRoadmap()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Gestao ITSM e melhoria continua"
      titulo="Roadmap ITSM"
      subtitulo="Acompanhe evolução, status técnico, pendências, critérios de aceite e evidências da implantação ITSM."
    >
      <template #actions>
        <q-btn color="primary" icon="library_books" label="Ver Documentação" @click="abrirDocumentacaoItsm" />
      </template>
    </PageHeader>

    <q-banner v-if="sucesso" class="bg-positive text-white q-mb-md">{{ sucesso }}</q-banner>
    <q-banner v-if="erro" class="bg-negative text-white q-mb-md">{{ erro }}</q-banner>

    <div class="sgx-kpi-grid">
      <MetricCard title="Itens ativos" :value="totalItensAtivos" icon="inventory_2" tone="primary" :loading="loading" />
      <MetricCard title="Concluidos" :value="totalConcluidos" icon="task_alt" tone="positive" :loading="loading" />
      <MetricCard title="Em andamento" :value="totalEmAndamento" icon="autorenew" tone="info" :loading="loading" />
      <MetricCard title="Com pendencia tecnica" :value="totalComPendencia" icon="warning" tone="warning" :loading="loading" />
      <MetricCard title="Media de progresso" :value="`${mediaPercentual}%`" icon="query_stats" tone="primary" :loading="loading" />
    </div>

    <AppSectionCard title="Itens do roadmap" icon="track_changes">
      <FilterBar compact titulo="Filtro operacional" subtitulo="Refine a visualizacao por status geral e categoria.">
        <q-form class="row q-col-gutter-sm q-mb-md">
          <div class="col-12 col-md-4">
            <q-select v-model="filtros.status" outlined dense clearable emit-value map-options :options="opcoesStatus" label="Status geral" />
          </div>
          <div class="col-12 col-md-4">
            <q-select v-model="filtros.roadmapCategoriaId" outlined dense clearable emit-value map-options :options="opcoesCategoriasAtivas" label="Categoria" />
          </div>
          <div class="col-12 col-md-4 flex items-center justify-end">
            <q-btn color="primary" label="Filtrar" @click="carregarListaRoadmap" />
          </div>
        </q-form>
      </FilterBar>

      <LoadingState v-if="loading" />
      <ErrorState v-else-if="erro" :mensagem="erro" @retry="carregarListaRoadmap" />
      <EmptyState v-else-if="!itens.length" titulo="Nenhum item encontrado" descricao="Ajuste os filtros para exibir o roadmap." />
      <q-table v-else :rows="itens" :columns="colunasRoadmap" row-key="id" flat bordered>
        <template #body-cell-acoes="slotProps">
          <q-td>
            <q-btn flat round dense icon="visibility" color="primary" aria-label="Visualizar item do roadmap" @click="abrirDetalhe(slotProps.row, 'visualizar')" />
            <q-btn
              v-if="podeGerenciarRoadmap"
              flat
              round
              dense
              icon="edit"
              color="secondary"
              aria-label="Editar item do roadmap"
              @click="abrirDetalhe(slotProps.row, 'editar')"
            />
          </q-td>
        </template>
        <template #body-cell-categoria="slotProps">
          <q-td>
            <q-chip dense :color="slotProps.row.roadmapCategoriaCor || 'primary'" text-color="white">
              <q-icon v-if="slotProps.row.roadmapCategoriaIcone" :name="slotProps.row.roadmapCategoriaIcone" class="q-mr-xs" />
              {{ slotProps.row.roadmapCategoriaNome || slotProps.row.categoria }}
            </q-chip>
          </q-td>
        </template>
        <template #body-cell-statusImplementacao="slotProps">
          <q-td>{{ labelStatusImplementacao(slotProps.row.statusImplementacao) }}</q-td>
        </template>
        <template #body-cell-statusTecnico="slotProps">
          <q-td>{{ labelStatusTecnico(slotProps.row.statusTecnico) }}</q-td>
        </template>
      </q-table>
    </AppSectionCard>

    <AppSectionCard title="Categorias do roadmap" icon="category" class="q-mt-md">
      <div class="row justify-end q-mb-md" v-if="podeGerenciarCategorias">
        <q-btn color="primary" icon="add" label="Nova categoria" @click="abrirModalCategoria('criar')" />
      </div>
      <q-table :rows="categorias" :columns="colunasCategorias" row-key="id" flat bordered>
        <template #body-cell-cor="slotProps">
          <q-td>
            <q-badge :color="slotProps.row.cor || 'grey-6'">{{ slotProps.row.cor || 'Sem cor' }}</q-badge>
          </q-td>
        </template>
        <template #body-cell-ativo="slotProps">
          <q-td><q-badge :color="slotProps.row.ativo ? 'positive' : 'negative'">{{ slotProps.row.ativo ? 'Sim' : 'Não' }}</q-badge></q-td>
        </template>
        <template #body-cell-acoes="slotProps">
          <q-td>
            <q-btn v-if="podeGerenciarCategorias" flat dense round icon="edit" color="secondary" aria-label="Editar categoria do roadmap" @click="abrirModalCategoria('editar', slotProps.row)" />
            <q-btn
              v-if="podeGerenciarCategorias && slotProps.row.ativo"
              flat dense round icon="block" color="negative"
              aria-label="Inativar categoria do roadmap"
              @click="abrirConfirmacao('Inativar categoria?', async () => { await roadmapItsmService.inativarCategoria(slotProps.row.id); await carregarCategorias() })"
            />
            <q-btn
              v-if="podeGerenciarCategorias && !slotProps.row.ativo"
              flat dense round icon="restart_alt" color="primary"
              aria-label="Reativar categoria do roadmap"
              @click="abrirConfirmacao('Reativar categoria?', async () => { await roadmapItsmService.reativarCategoria(slotProps.row.id); await carregarCategorias() })"
            />
          </q-td>
        </template>
      </q-table>
    </AppSectionCard>

    <q-dialog v-model="modalDetalheAberto" maximized>
      <q-card>
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">{{ modoDetalhe === 'editar' ? 'Editar roadmap' : 'Detalhe do roadmap' }}</div>
          <q-space />
          <q-btn icon="close" flat round dense aria-label="Fechar detalhe do roadmap" v-close-popup />
        </q-card-section>

        <q-card-section v-if="detalheAtual" class="q-gutter-md">
          <q-form class="row q-col-gutter-sm roadmap-form-texto-preto">
            <div class="col-12 col-md-6"><q-input v-model="formRoadmap.area" outlined dense label="Área" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-6"><q-select v-model="formRoadmap.roadmapCategoriaId" outlined dense emit-value map-options :options="opcoesCategoriasAtivas" label="Categoria" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-4"><q-input v-model.number="formRoadmap.ordem" type="number" outlined dense label="Ordem" :disable="modoSomenteLeitura" /></div>
            <div class="col-12">
              <q-input
                v-model="formRoadmap.objetivo"
                outlined
                dense
                type="textarea"
                autogrow
                label="Objetivo"
                hint="Explique o objetivo deste item dentro do sistema."
                :disable="modoSomenteLeitura"
              />
              <div v-if="!formRoadmap.objetivo" class="text-caption text-grey-7 q-mt-xs">Objetivo ainda não informado.</div>
            </div>
            <div class="col-12 col-md-8"><q-input v-model="formRoadmap.situacaoAtual" outlined dense label="Situação atual" :disable="modoSomenteLeitura" /></div>
            <div class="col-12"><q-input v-model="formRoadmap.atencaoTecnica" outlined dense autogrow label="Atenção técnica" :disable="modoSomenteLeitura" /></div>

            <div class="col-12"><div class="text-subtitle2">Status real da implementação</div></div>
            <div class="col-12 col-md-4"><q-select v-model="formRoadmap.statusImplementacao" outlined dense emit-value map-options :options="opcoesStatusImplementacao" label="Status da implementação" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-4"><q-select v-model="formRoadmap.statusTecnico" outlined dense emit-value map-options :options="opcoesStatusTecnico" label="Status técnico" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-4">
              <q-input v-model.number="formRoadmap.percentualImplementacao" type="number" outlined dense label="Percentual (%)" :disable="true" />
            </div>
            <div class="col-12">
              <q-linear-progress :value="(formRoadmap.percentualImplementacao || 0) / 100" color="primary" size="10px" rounded />
              <div class="text-caption q-mt-xs">{{ formRoadmap.percentualImplementacao }}% - {{ percentualCalculadoPorChecklist ? 'calculado pelo checklist ativo' : 'sem checklist ativo' }}</div>
              <q-banner v-if="percentualCalculadoPorChecklist" dense class="bg-blue-1 text-primary q-mt-sm">
                O percentual é calculado automaticamente com base no checklist ativo.
              </q-banner>
              <q-banner v-else dense class="bg-grey-2 text-grey-8 q-mt-sm">
                Sem checklist ativo, o progresso exibido permanece em 0%.
              </q-banner>
            </div>

            <div class="col-12">
              <q-banner v-if="mostrarBannerImplementadoFuncionalmente" class="bg-warning text-dark">
                Este status indica entrega funcional, mas não necessariamente homologação ou produção.
              </q-banner>
            </div>
            <div class="col-12" v-if="mostrarPendenciasEvolutivas">
              <q-banner class="bg-orange-2 text-dark">
                Status técnico indica pendências evolutivas. Detalhe as pendências para governança.
              </q-banner>
            </div>

            <div class="col-12 col-md-6"><q-input v-model="formRoadmap.pendenciasTecnicas" outlined dense autogrow label="Pendências técnicas" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-6"><q-input v-model="formRoadmap.pendenciasHomologacao" outlined dense autogrow label="Pendências de homologação" :disable="modoSomenteLeitura" /></div>
            <div class="col-12"><q-input v-model="formRoadmap.evidenciaImplementacao" outlined dense autogrow label="Evidência da implementação" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-6"><q-input v-model="formRoadmap.dataConclusaoTecnica" outlined dense type="date" label="Data de conclusão técnica" :disable="modoSomenteLeitura" /></div>
            <div class="col-12 col-md-6"><q-input v-model="formRoadmap.dataHomologacao" outlined dense type="date" label="Data de homologação" :disable="modoSomenteLeitura" /></div>
            <div class="col-12"><q-input v-model="formRoadmap.criterioAceite" outlined dense autogrow label="Critério de aceite" :disable="modoSomenteLeitura" /></div>
            <div class="col-12"><q-input v-model="formRoadmap.proximaAcao" outlined dense autogrow label="Próxima ação" :disable="modoSomenteLeitura" /></div>
          </q-form>

          <AppSectionCard title="Checklist da implementação" icon="checklist">
            <div class="row items-center q-mb-sm">
              <div class="text-caption">Concluídos: {{ detalheAtual.quantidadeChecklistConcluido }}/{{ detalheAtual.quantidadeChecklistAtivo }}</div>
              <q-space />
              <q-btn v-if="podeGerenciarChecklist && modoDetalhe === 'editar'" flat color="primary" icon="add" label="Novo item" @click="abrirModalChecklist('criar')" />
            </div>
            <q-banner dense class="bg-blue-1 text-primary q-mb-sm">
              O percentual do roadmap é calculado com base nos itens ativos do checklist.
            </q-banner>
            <q-table :rows="checklistOrdenado" :columns="colunasChecklist" row-key="id" flat bordered>
              <template #body-cell-concluido="slotProps">
                <q-td>
                  <q-badge :color="slotProps.row.concluido ? 'positive' : 'grey-6'">{{ slotProps.row.concluido ? 'Sim' : 'Não' }}</q-badge>
                </q-td>
              </template>
              <template #body-cell-obrigatorio="slotProps">
                <q-td>
                  <q-badge :color="slotProps.row.obrigatorio ? 'indigo' : 'grey-6'">{{ slotProps.row.obrigatorio ? 'Sim' : 'Não' }}</q-badge>
                </q-td>
              </template>
              <template #body-cell-ativo="slotProps">
                <q-td>
                  <q-badge :color="slotProps.row.ativo ? 'positive' : 'negative'">{{ slotProps.row.ativo ? 'Sim' : 'Não' }}</q-badge>
                </q-td>
              </template>
              <template #body-cell-acoes="slotProps">
                <q-td class="text-right">
                  <div class="row no-wrap justify-end items-center q-gutter-xs">
                    <q-btn v-if="podeGerenciarChecklist && modoDetalhe === 'editar'" flat dense round icon="edit" color="secondary" aria-label="Editar item do checklist" @click="abrirModalChecklist('editar', slotProps.row)"><q-tooltip>Editar</q-tooltip></q-btn>
                    <q-btn
                      v-if="podeGerenciarChecklist && modoDetalhe === 'editar' && !slotProps.row.concluido"
                      flat dense round icon="task_alt" color="positive"
                      aria-label="Concluir item do checklist"
                      @click="concluirChecklist(slotProps.row)"
                    ><q-tooltip>Concluir</q-tooltip></q-btn>
                    <q-btn
                      v-if="podeGerenciarChecklist && modoDetalhe === 'editar' && slotProps.row.concluido"
                      flat dense round icon="undo" color="warning"
                      aria-label="Reabrir item do checklist"
                      @click="reabrirChecklist(slotProps.row)"
                    ><q-tooltip>Reabrir</q-tooltip></q-btn>
                    <q-btn
                      v-if="podeGerenciarChecklist && modoDetalhe === 'editar' && slotProps.row.ativo"
                      flat dense round icon="block" color="negative"
                      aria-label="Inativar item do checklist"
                      @click="inativarChecklist(slotProps.row)"
                    ><q-tooltip>Inativar</q-tooltip></q-btn>
                    <q-btn
                      v-if="podeGerenciarChecklist && modoDetalhe === 'editar' && !slotProps.row.ativo"
                      flat dense round icon="restart_alt" color="primary"
                      aria-label="Reativar item do checklist"
                      @click="reativarChecklist(slotProps.row)"
                    ><q-tooltip>Reativar</q-tooltip></q-btn>
                  </div>
                </q-td>
              </template>
            </q-table>
          </AppSectionCard>

          <AppSectionCard title="Futuras implementações" icon="engineering">
            <div class="row justify-end q-mb-sm">
              <q-btn v-if="podeGerenciarImplementacoes && modoDetalhe === 'editar'" flat color="primary" icon="add" label="Nova implementação" @click="abrirModalImplementacao('criar')" />
            </div>
            <q-table :rows="implementacoes" :columns="colunasImplementacao" row-key="id" flat bordered>
              <template #body-cell-prazoAlvo="slotProps"><q-td>{{ formatarDataCurta(slotProps.row.prazoAlvo) }}</q-td></template>
              <template #body-cell-ativo="slotProps"><q-td><q-badge :color="slotProps.row.ativo ? 'positive' : 'negative'">{{ slotProps.row.ativo ? 'Sim' : 'Não' }}</q-badge></q-td></template>
              <template #body-cell-acoes="slotProps">
                <q-td>
                  <q-btn v-if="podeGerenciarImplementacoes && modoDetalhe === 'editar'" flat dense round icon="edit" color="secondary" aria-label="Editar implementação futura" @click="abrirModalImplementacao('editar', slotProps.row)" />
                  <q-btn
                    v-if="podeGerenciarImplementacoes && modoDetalhe === 'editar' && slotProps.row.status !== 6"
                    flat dense round icon="task_alt" color="positive"
                    aria-label="Concluir implementação futura"
                    @click="concluirImplementacao(slotProps.row)"
                  />
                  <q-btn
                    v-if="podeGerenciarImplementacoes && modoDetalhe === 'editar' && slotProps.row.ativo"
                    flat dense round icon="block" color="negative"
                    aria-label="Inativar implementação futura"
                    @click="inativarImplementacao(slotProps.row)"
                  />
                  <q-btn
                    v-if="podeGerenciarImplementacoes && modoDetalhe === 'editar' && !slotProps.row.ativo"
                    flat dense round icon="restart_alt" color="primary"
                    aria-label="Reativar implementação futura"
                    @click="reativarImplementacao(slotProps.row)"
                  />
                </q-td>
              </template>
            </q-table>
          </AppSectionCard>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Fechar" v-close-popup />
          <q-btn v-if="!modoSomenteLeitura" color="primary" label="Salvar" :loading="salvandoRoadmap" @click="salvarRoadmap" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="modalCategoriaAberto">
      <q-card style="min-width: 520px">
        <q-card-section class="text-h6">{{ modoCategoria === 'criar' ? 'Nova categoria' : 'Editar categoria' }}</q-card-section>
        <q-card-section class="row q-col-gutter-sm">
          <div class="col-12"><q-input v-model="formCategoria.nome" outlined dense label="Nome" /></div>
          <div class="col-12"><q-input v-model="formCategoria.descricao" outlined dense autogrow label="Descrição" /></div>
          <div class="col-12 col-md-4"><q-input v-model="formCategoria.cor" outlined dense label="Cor" /></div>
          <div class="col-12 col-md-4"><q-input v-model="formCategoria.icone" outlined dense label="Ícone" /></div>
          <div class="col-12 col-md-4"><q-input v-model.number="formCategoria.ordem" type="number" outlined dense label="Ordem" /></div>
          <div class="col-12"><q-toggle v-model="formCategoria.ativo" label="Ativo" /></div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" @click="salvarCategoria" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="modalChecklistAberto">
      <q-card style="min-width: 720px; width: 820px; max-width: 92vw">
        <q-card-section class="text-h6">{{ modoChecklist === 'criar' ? 'Novo item do checklist' : 'Editar item do checklist' }}</q-card-section>
        <q-card-section class="row q-col-gutter-sm">
          <div class="col-12"><q-input v-model="formChecklist.titulo" outlined dense label="Título" /></div>
          <div class="col-12"><q-input v-model="formChecklist.descricao" class="checklist-descricao-textarea" outlined dense type="textarea" autogrow :rows="4" label="Descrição" /></div>
          <div class="col-12 col-md-4"><q-select v-model="formChecklist.grupo" outlined dense emit-value map-options :options="opcoesGrupoChecklist" label="Grupo" /></div>
          <div class="col-12 col-md-4"><q-input v-model.number="formChecklist.ordem" outlined dense type="number" label="Ordem" /></div>
          <div class="col-12 col-md-4"><q-toggle v-model="formChecklist.obrigatorio" label="Obrigatório" /></div>
          <div class="col-12 col-md-6"><q-toggle v-model="formChecklist.concluido" label="Concluído" /></div>
          <div class="col-12 col-md-6"><q-toggle v-model="formChecklist.ativo" label="Ativo" /></div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" @click="salvarChecklist" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="modalImplementacaoAberto">
      <q-card style="min-width: 680px">
        <q-card-section class="text-h6">{{ modoImplementacao === 'criar' ? 'Nova implementação' : 'Editar implementação' }}</q-card-section>
        <q-card-section class="row q-col-gutter-sm">
          <div class="col-12"><q-input v-model="formImplementacao.titulo" outlined dense label="Título" /></div>
          <div class="col-12"><q-input v-model="formImplementacao.descricao" outlined dense autogrow label="Descrição" /></div>
          <div class="col-12 col-md-4"><q-select v-model="formImplementacao.tipo" outlined dense emit-value map-options :options="opcoesTipoImplementacao" label="Tipo" /></div>
          <div class="col-12 col-md-4"><q-select v-model="formImplementacao.prioridade" outlined dense emit-value map-options :options="opcoesPrioridadeImplementacao" label="Prioridade" /></div>
          <div class="col-12 col-md-4"><q-select v-model="formImplementacao.status" outlined dense emit-value map-options :options="opcoesStatusImplementacaoFutura" label="Status" /></div>
          <div class="col-12 col-md-6"><q-input v-model="formImplementacao.responsavel" outlined dense label="Responsável" /></div>
          <div class="col-12 col-md-3"><q-input v-model="formImplementacao.prazoAlvo" outlined dense type="date" label="Prazo alvo" /></div>
          <div class="col-12 col-md-3"><q-input v-model="formImplementacao.dataConclusao" outlined dense type="date" label="Data conclusão" /></div>
          <div class="col-12"><q-input v-model="formImplementacao.observacao" outlined dense autogrow label="Observação" /></div>
          <div class="col-12"><q-toggle v-model="formImplementacao.ativo" label="Ativo" /></div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" @click="salvarImplementacao" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <ConfirmDialog v-model="confirmacaoAberta" :mensagem="confirmacaoMensagem" @confirm="executarConfirmacao" />
  </q-page>
</template>

<style scoped>
.roadmap-form-texto-preto :deep(.q-field__native),
.roadmap-form-texto-preto :deep(.q-field__input),
.roadmap-form-texto-preto :deep(.q-field__prefix),
.roadmap-form-texto-preto :deep(.q-field__suffix) {
  color: #000 !important;
}

.roadmap-form-texto-preto :deep(.q-field--disabled .q-field__native),
.roadmap-form-texto-preto :deep(.q-field--disabled .q-field__input),
.roadmap-form-texto-preto :deep(.q-field--readonly .q-field__native),
.roadmap-form-texto-preto :deep(.q-field--readonly .q-field__input) {
  color: #000 !important;
  -webkit-text-fill-color: #000 !important;
  opacity: 1 !important;
}

.checklist-descricao-textarea :deep(textarea.q-field__native) {
  min-height: 92px !important;
  line-height: 1.35;
}

:deep(.q-table__middle) {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}
</style>




