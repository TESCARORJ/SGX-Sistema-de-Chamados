<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../../components/admin/cadastros/FormCadastro.vue'
import AppSectionCard from '../../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../../components/ui/ConfirmDialog.vue'
import EmptyState from '../../components/ui/EmptyState.vue'
import ErrorState from '../../components/ui/ErrorState.vue'
import LoadingState from '../../components/ui/LoadingState.vue'
import PageHeader from '../../components/ui/PageHeader.vue'
import StatusBadge from '../../components/ui/StatusBadge.vue'
import { permissoes, permissoesCriticas } from '../../constants/permissoes'
import { cadastrosAdminService } from '../../services/cadastrosAdminService'
import { parametrosSistemaService } from '../../services/parametrosSistemaService'
import { usuariosAdminService } from '../../services/usuariosAdminService'
import { useAuthStore } from '../../stores/authStore'
import type {
  CategoriaChamadoResumoResponse,
  DepartamentoResumoResponse,
  PerfilAcessoResumoResponse,
  PerfilPermissoes,
  PermissaoSistema,
} from '../../types/adminCadastros'

type Entidade =
  | 'usuarios'
  | 'perfis'
  | 'departamentos'
  | 'categorias'
  | 'subcategorias'
  | 'prioridades'
  | 'tipos-solicitacao'
  | 'locais'
  | 'status'
  | 'parametros'

const props = defineProps<{
  titulo: string
  entidade: Entidade
  listPath: string
}>()

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const carregamentoConcluido = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const confirmarInativacao = ref(false)
const confirmarReativacao = ref(false)
const registroAtivo = ref(true)
const loadingPermissoesPerfil = ref(false)
const salvandoPermissoesPerfil = ref(false)
const erroPermissoesPerfil = ref<string | null>(null)
const sucessoPermissoesPerfil = ref<string | null>(null)
const perfilPermissoes = ref<PerfilPermissoes | null>(null)
const codigosPermissoesSelecionadas = ref<string[]>([])
const redefinirSenhaDialogAberto = ref(false)
const redefinirSenhaLoading = ref(false)
const redefinirSenhaErro = ref<string | null>(null)
const redefinirSenhaNova = ref('')
const redefinirSenhaConfirmacao = ref('')
const redefinirSenhaDeveAlterar = ref(true)

const perfis = ref<PerfilAcessoResumoResponse[]>([])
const departamentos = ref<DepartamentoResumoResponse[]>([])
const categorias = ref<CategoriaChamadoResumoResponse[]>([])

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const isAdmin = computed(() => authStore.usuario?.perfis.includes('Administrador') ?? false)
const fallbackAdminSemPermissoes = computed(() => isAdmin.value && (authStore.usuario?.permissoes?.length ?? 0) === 0)
const podeGerenciarRegistro = computed(() => {
  switch (props.entidade) {
    case 'usuarios':
      return authStore.possuiPermissao(permissoes.usuariosGerenciar)
    case 'perfis':
      return authStore.possuiPermissao(permissoes.perfisGerenciar)
    case 'parametros':
      return authStore.possuiPermissao(permissoes.parametrosGerenciar)
    default:
      return isAdmin.value
  }
})
const somenteLeitura = computed(() => !podeGerenciarRegistro.value)
const podeEditarPerfisDoUsuario = computed(() => authStore.possuiPermissao(permissoes.usuariosAlterarPerfis))
const podeRedefinirSenhaUsuario = computed(() => {
  if (props.entidade !== 'usuarios' || isNovo.value || !isAdmin.value) {
    return false
  }

  if (fallbackAdminSemPermissoes.value) {
    return true
  }

  return authStore.possuiAlgumaPermissao([permissoes.usuariosGerenciar, permissoes.usuariosRedefinirSenha])
})
const podeMostrarMatrizPermissoes = computed(() => props.entidade === 'perfis' && !isNovo.value)
const podeVisualizarPermissoesPerfil = computed(() =>
  authStore.possuiAlgumaPermissao([
    permissoes.perfisVisualizar,
    permissoes.perfisGerenciar,
    permissoes.perfisAlterarPermissoes,
  ])
)
const podeEditarPermissoesPerfil = computed(
  () => isAdmin.value && authStore.possuiPermissao(permissoes.perfisAlterarPermissoes)
)
const totalPermissoesSelecionadas = computed(() => codigosPermissoesSelecionadas.value.length)
const permissoesVinculadasSet = computed(
  () => new Set(perfilPermissoes.value?.permissoesVinculadas.map((item) => item.codigo) ?? [])
)
const modulosPermissoes = computed(() => {
  const disponiveis = perfilPermissoes.value?.permissoesDisponiveis ?? []
  const grupos = new Map<string, PermissaoSistema[]>()

  for (const permissao of disponiveis) {
    const chaveModulo = permissao.modulo || 'Outros'
    if (!grupos.has(chaveModulo)) {
      grupos.set(chaveModulo, [])
    }

    grupos.get(chaveModulo)!.push(permissao)
  }

  return Array.from(grupos.entries())
    .map(([modulo, permissoes]) => ({
      modulo,
      moduloLabel: mapModuloLabel(modulo),
      permissoes: [...permissoes].sort((a, b) => a.codigo.localeCompare(b.codigo)),
    }))
    .sort((a, b) => a.moduloLabel.localeCompare(b.moduloLabel))
})
const subtituloCabecalho = computed(() =>
  isNovo.value ? 'Criacao de novo cadastro' : 'Detalhe e manutencao de cadastro'
)
const subtituloFormulario = computed(() =>
  isNovo.value
    ? 'Preencha os campos obrigatorios para concluir o cadastro.'
    : 'Revise e atualize os dados mantendo o padrao administrativo.'
)

const form = reactive({
  nome: '',
  email: '',
  login: '',
  situacao: 1,
  departamentoId: null as string | null,
  perfilIds: [] as string[],
  sigla: '',
  categoriaChamadoId: null as string | null,
  descricao: '',
  peso: 1,
  cor: '',
  endereco: '',
  codigo: 1,
  ehStatusFinal: false,
  pausaSla: false,
  chave: '',
  valor: '',
  sensivel: false,
  tipoPerfil: 2,
})

const opcoesSituacao = [
  { label: 'Ativo', value: 1 },
  { label: 'Inativo', value: 2 },
  { label: 'Bloqueado', value: 3 },
]

const opcoesCodigoStatus = [
  { label: 'Aberto', value: 1 },
  { label: 'Em Atendimento', value: 2 },
  { label: 'Aguardando Solicitante', value: 3 },
  { label: 'Resolvido', value: 4 },
  { label: 'Encerrado', value: 5 },
  { label: 'Cancelado', value: 6 },
]

const opcoesTipoPerfil = [
  { label: 'Administrador', value: 1 },
  { label: 'Atendente', value: 2 },
  { label: 'Solicitante', value: 3 },
]

const regraObrigatoria = (valor: unknown): true | string => {
  if (typeof valor === 'number') {
    return Number.isFinite(valor) ? true : 'Campo obrigatÃ³rio.'
  }

  if (Array.isArray(valor)) {
    return valor.length > 0 ? true : 'Campo obrigatÃ³rio.'
  }

  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatÃ³rio.'
}

const regraEmail = (valor: string): true | string => {
  if (!valor?.trim()) {
    return 'Campo obrigatÃ³rio.'
  }

  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(valor) ? true : 'Informe um e-mail vÃ¡lido.'
}

const regraPesoPositivo = (valor: number): true | string =>
  Number(valor) > 0 ? true : 'Informe um peso maior que zero.'

const regraCorHex = (valor: string): true | string => {
  const cor = valor.trim()
  if (!cor) {
    return true
  }

  return /^#[0-9A-Fa-f]{6}$/.test(cor) ? true : 'Informe uma cor no formato #RRGGBB.'
}

function mapModuloLabel(modulo: string): string {
  const mapa: Record<string, string> = {
    Dashboard: 'Dashboard',
    Chamados: 'Chamados',
    Cadastros: 'Cadastros',
    Usuarios: 'UsuÃ¡rios',
    Perfis: 'Perfis',
    Parametros: 'ParÃ¢metros',
    IntegracoesEmail: 'IntegraÃ§Ãµes',
    IntegracoesMicrosoft: 'IntegraÃ§Ãµes',
    Notificacoes: 'NotificaÃ§Ãµes',
    Indicadores: 'Indicadores',
  }

  return mapa[modulo] ?? modulo
}

function permissaoEhCritica(codigo: string): boolean {
  return permissoesCriticas.some((item) => item.toLowerCase() === codigo.toLowerCase())
}

function resetarFormulario(): void {
  form.nome = ''
  form.email = ''
  form.login = ''
  form.situacao = 1
  form.departamentoId = null
  form.perfilIds = []
  form.sigla = ''
  form.categoriaChamadoId = null
  form.descricao = ''
  form.peso = 1
  form.cor = ''
  form.endereco = ''
  form.codigo = 1
  form.ehStatusFinal = false
  form.pausaSla = false
  form.chave = ''
  form.valor = ''
  form.sensivel = false
  form.tipoPerfil = 2
}

async function carregarAuxiliares(): Promise<void> {
  perfis.value = []
  departamentos.value = []
  categorias.value = []

  if (props.entidade === 'usuarios') {
    const [perfisResponse, departamentosResponse] = await Promise.all([
      cadastrosAdminService.listarPerfis({ ativo: true, tamanhoPagina: 100 }),
      cadastrosAdminService.listarDepartamentos({ ativo: true, tamanhoPagina: 100 }),
    ])
    perfis.value = perfisResponse.items
    departamentos.value = departamentosResponse.items
  }

  if (props.entidade === 'categorias') {
    const departamentosResponse = await cadastrosAdminService.listarDepartamentos({ ativo: true, tamanhoPagina: 100 })
    departamentos.value = departamentosResponse.items
  }

  if (props.entidade === 'subcategorias') {
    const categoriasResponse = await cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 })
    categorias.value = categoriasResponse.items
  }
}

function mapSituacaoTextoParaNumero(situacao: string): number {
  if (situacao === 'Inativo') return 2
  if (situacao === 'Bloqueado') return 3
  return 1
}

async function carregarDetalhe(): Promise<void> {
  resetarFormulario()

  if (isNovo.value) {
    registroAtivo.value = true
    return
  }

  switch (props.entidade) {
    case 'usuarios': {
      const usuario = await usuariosAdminService.obterPorId(idParam.value)
      form.nome = usuario.nome
      form.email = usuario.email
      form.login = usuario.login
      form.situacao = mapSituacaoTextoParaNumero(usuario.situacao)
      form.departamentoId = usuario.departamentoId
      form.perfilIds = usuario.perfis.map((perfil) => perfil.id)
      registroAtivo.value = usuario.ativo
      break
    }
    case 'perfis': {
      const perfil = await cadastrosAdminService.obterPerfilPorId(idParam.value)
      form.nome = perfil.nome
      form.descricao = perfil.descricao ?? ''
      form.tipoPerfil = perfil.tipoPerfil
      registroAtivo.value = perfil.ativo
      break
    }
    case 'departamentos': {
      const departamento = await cadastrosAdminService.obterDepartamentoPorId(idParam.value)
      form.nome = departamento.nome
      form.sigla = departamento.sigla
      form.descricao = departamento.descricao ?? ''
      registroAtivo.value = departamento.ativo
      break
    }
    case 'categorias': {
      const categoria = await cadastrosAdminService.obterCategoriaPorId(idParam.value)
      form.nome = categoria.nome
      form.descricao = categoria.descricao ?? ''
      form.departamentoId = categoria.departamentoId
      registroAtivo.value = categoria.ativo
      break
    }
    case 'subcategorias': {
      const subcategoria = await cadastrosAdminService.obterSubcategoriaPorId(idParam.value)
      form.nome = subcategoria.nome
      form.descricao = subcategoria.descricao ?? ''
      form.categoriaChamadoId = subcategoria.categoriaChamadoId
      registroAtivo.value = subcategoria.ativo
      break
    }
    case 'prioridades': {
      const prioridade = await cadastrosAdminService.obterPrioridadePorId(idParam.value)
      form.nome = prioridade.nome
      form.descricao = prioridade.descricao ?? ''
      form.peso = prioridade.peso
      form.cor = prioridade.cor ?? ''
      registroAtivo.value = prioridade.ativo
      break
    }
    case 'tipos-solicitacao': {
      const tipoSolicitacao = await cadastrosAdminService.obterTipoSolicitacaoPorId(idParam.value)
      form.nome = tipoSolicitacao.nome
      form.descricao = tipoSolicitacao.descricao ?? ''
      registroAtivo.value = tipoSolicitacao.ativo
      break
    }
    case 'locais': {
      const localUnidade = await cadastrosAdminService.obterLocalUnidadePorId(idParam.value)
      form.nome = localUnidade.nome
      form.descricao = localUnidade.descricao ?? ''
      form.endereco = localUnidade.endereco ?? ''
      registroAtivo.value = localUnidade.ativo
      break
    }
    case 'status': {
      const status = await cadastrosAdminService.obterStatusPorId(idParam.value)
      form.nome = status.nome
      form.descricao = status.descricao ?? ''
      form.codigo = status.codigo
      form.ehStatusFinal = status.ehStatusFinal
      form.pausaSla = status.pausaSla
      registroAtivo.value = status.ativo
      break
    }
    case 'parametros': {
      const parametro = await parametrosSistemaService.obterPorId(idParam.value)
      form.chave = parametro.chave
      form.valor = parametro.valor
      form.descricao = parametro.descricao ?? ''
      form.sensivel = parametro.sensivel
      registroAtivo.value = parametro.ativo
      break
    }
  }
}

async function carregarPermissoesPerfil(): Promise<void> {
  if (!podeMostrarMatrizPermissoes.value || !podeVisualizarPermissoesPerfil.value) {
    perfilPermissoes.value = null
    codigosPermissoesSelecionadas.value = []
    erroPermissoesPerfil.value = null
    sucessoPermissoesPerfil.value = null
    return
  }

  loadingPermissoesPerfil.value = true
  erroPermissoesPerfil.value = null

  try {
    const response = await cadastrosAdminService.obterPermissoesPerfil(idParam.value)
    perfilPermissoes.value = response
    codigosPermissoesSelecionadas.value = response.permissoesVinculadas.map((item) => item.codigo)
  } catch (error) {
    erroPermissoesPerfil.value =
      error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar as permissÃµes do perfil.'
  } finally {
    loadingPermissoesPerfil.value = false
  }
}

async function salvarPermissoesPerfil(): Promise<void> {
  if (!podeMostrarMatrizPermissoes.value || !podeEditarPermissoesPerfil.value) {
    return
  }

  salvandoPermissoesPerfil.value = true
  erroPermissoesPerfil.value = null
  sucessoPermissoesPerfil.value = null

  try {
    const payload = {
      codigosPermissoes: [...new Set(codigosPermissoesSelecionadas.value)],
    }

    const response = await cadastrosAdminService.atualizarPermissoesPerfil(idParam.value, payload)
    perfilPermissoes.value = response
    codigosPermissoesSelecionadas.value = response.permissoesVinculadas.map((item) => item.codigo)
    sucessoPermissoesPerfil.value = 'PermissÃµes atualizadas com sucesso.'
    $q.notify({
      type: 'positive',
      message: 'PermissÃµes do perfil salvas com sucesso.',
    })
  } catch (error) {
    erroPermissoesPerfil.value =
      error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel salvar as permissÃµes do perfil.'
    $q.notify({
      type: 'negative',
      message: erroPermissoesPerfil.value,
    })
  } finally {
    salvandoPermissoesPerfil.value = false
  }
}

async function carregarTela(): Promise<void> {
  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await carregarAuxiliares()
    await carregarDetalhe()
    await carregarPermissoesPerfil()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar os dados.'
  } finally {
    loading.value = false
    carregamentoConcluido.value = true
  }
}

async function salvar(): Promise<void> {
  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    switch (props.entidade) {
      case 'usuarios': {
        if (isNovo.value) {
          const created = await usuariosAdminService.criar({
            nome: form.nome,
            email: form.email,
            login: form.login || null,
            departamentoId: form.departamentoId,
            perfilIds: form.perfilIds,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await usuariosAdminService.atualizar(idParam.value, {
            nome: form.nome,
            email: form.email,
            login: form.login || null,
            departamentoId: form.departamentoId,
            situacao: form.situacao,
          })

          if (podeEditarPerfisDoUsuario.value) {
            await usuariosAdminService.alterarPerfis(idParam.value, { perfilIds: form.perfilIds })
          }
        }
        break
      }
      case 'perfis':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarPerfil({
            nome: form.nome,
            tipoPerfil: form.tipoPerfil,
            descricao: form.descricao || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarPerfil(idParam.value, {
            nome: form.nome,
            tipoPerfil: form.tipoPerfil,
            descricao: form.descricao || null,
          })
        }
        break
      case 'departamentos':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarDepartamento({
            nome: form.nome,
            sigla: form.sigla,
            descricao: form.descricao || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarDepartamento(idParam.value, {
            nome: form.nome,
            sigla: form.sigla,
            descricao: form.descricao || null,
          })
        }
        break
      case 'categorias':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarCategoria({
            nome: form.nome,
            descricao: form.descricao || null,
            departamentoId: form.departamentoId,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarCategoria(idParam.value, {
            nome: form.nome,
            descricao: form.descricao || null,
            departamentoId: form.departamentoId,
          })
        }
        break
      case 'subcategorias':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarSubcategoria({
            categoriaChamadoId: form.categoriaChamadoId!,
            nome: form.nome,
            descricao: form.descricao || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarSubcategoria(idParam.value, {
            categoriaChamadoId: form.categoriaChamadoId!,
            nome: form.nome,
            descricao: form.descricao || null,
          })
        }
        break
      case 'prioridades':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarPrioridade({
            nome: form.nome,
            descricao: form.descricao || null,
            peso: form.peso,
            cor: form.cor || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarPrioridade(idParam.value, {
            nome: form.nome,
            descricao: form.descricao || null,
            peso: form.peso,
            cor: form.cor || null,
          })
        }
        break
      case 'tipos-solicitacao':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarTipoSolicitacao({
            nome: form.nome,
            descricao: form.descricao || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarTipoSolicitacao(idParam.value, {
            nome: form.nome,
            descricao: form.descricao || null,
          })
        }
        break
      case 'locais':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarLocalUnidade({
            nome: form.nome,
            descricao: form.descricao || null,
            endereco: form.endereco || null,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarLocalUnidade(idParam.value, {
            nome: form.nome,
            descricao: form.descricao || null,
            endereco: form.endereco || null,
          })
        }
        break
      case 'status':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarStatus({
            nome: form.nome,
            codigo: form.codigo,
            descricao: form.descricao || null,
            ehStatusFinal: form.ehStatusFinal,
            pausaSla: form.pausaSla,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarStatus(idParam.value, {
            nome: form.nome,
            codigo: form.codigo,
            descricao: form.descricao || null,
            ehStatusFinal: form.ehStatusFinal,
            pausaSla: form.pausaSla,
          })
        }
        break
      case 'parametros':
        if (isNovo.value) {
          const created = await parametrosSistemaService.criar({
            chave: form.chave,
            valor: form.valor,
            descricao: form.descricao || null,
            sensivel: form.sensivel,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await parametrosSistemaService.atualizar(idParam.value, {
            chave: form.chave,
            valor: form.valor,
            descricao: form.descricao || null,
            sensivel: form.sensivel,
          })
        }
        break
    }

    sucesso.value = 'Registro salvo com sucesso.'
    await carregarDetalhe()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel salvar as informaÃ§Ãµes.'
  } finally {
    loading.value = false
  }
}

async function inativar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    switch (props.entidade) {
      case 'usuarios':
        await usuariosAdminService.inativar(idParam.value)
        break
      case 'perfis':
        await cadastrosAdminService.inativarPerfil(idParam.value)
        break
      case 'departamentos':
        await cadastrosAdminService.inativarDepartamento(idParam.value)
        break
      case 'categorias':
        await cadastrosAdminService.inativarCategoria(idParam.value)
        break
      case 'subcategorias':
        await cadastrosAdminService.inativarSubcategoria(idParam.value)
        break
      case 'prioridades':
        await cadastrosAdminService.inativarPrioridade(idParam.value)
        break
      case 'tipos-solicitacao':
        await cadastrosAdminService.inativarTipoSolicitacao(idParam.value)
        break
      case 'locais':
        await cadastrosAdminService.inativarLocalUnidade(idParam.value)
        break
      case 'status':
        await cadastrosAdminService.inativarStatus(idParam.value)
        break
      case 'parametros':
        await parametrosSistemaService.inativar(idParam.value)
        break
    }

    sucesso.value = 'Registro inativado com sucesso.'
    confirmarInativacao.value = false
    await carregarDetalhe()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel concluir a aÃ§Ã£o.'
  } finally {
    loading.value = false
  }
}

async function reativar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    switch (props.entidade) {
      case 'usuarios':
        await usuariosAdminService.reativar(idParam.value)
        break
      case 'perfis':
        await cadastrosAdminService.reativarPerfil(idParam.value)
        break
      case 'departamentos':
        await cadastrosAdminService.reativarDepartamento(idParam.value)
        break
      case 'categorias':
        await cadastrosAdminService.reativarCategoria(idParam.value)
        break
      case 'subcategorias':
        await cadastrosAdminService.reativarSubcategoria(idParam.value)
        break
      case 'prioridades':
        await cadastrosAdminService.reativarPrioridade(idParam.value)
        break
      case 'tipos-solicitacao':
        await cadastrosAdminService.reativarTipoSolicitacao(idParam.value)
        break
      case 'locais':
        await cadastrosAdminService.reativarLocalUnidade(idParam.value)
        break
      case 'status':
        await cadastrosAdminService.reativarStatus(idParam.value)
        break
      case 'parametros':
        await parametrosSistemaService.reativar(idParam.value)
        break
    }

    sucesso.value = 'Registro reativado com sucesso.'
    confirmarReativacao.value = false
    await carregarDetalhe()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel concluir a aÃ§Ã£o.'
  } finally {
    loading.value = false
  }
}

function abrirRedefinicaoSenha(): void {
  redefinirSenhaDialogAberto.value = true
  redefinirSenhaErro.value = null
  redefinirSenhaNova.value = ''
  redefinirSenhaConfirmacao.value = ''
  redefinirSenhaDeveAlterar.value = true
}

async function confirmarRedefinicaoSenha(): Promise<void> {
  redefinirSenhaErro.value = null

  const novaSenha = redefinirSenhaNova.value.trim()
  const confirmacao = redefinirSenhaConfirmacao.value.trim()
  if (!novaSenha || !confirmacao) {
    redefinirSenhaErro.value = 'Informe a nova senha e a confirmaÃ§Ã£o.'
    return
  }

  redefinirSenhaLoading.value = true
  try {
    const response = await usuariosAdminService.redefinirSenha(idParam.value, {
      novaSenha,
      confirmarNovaSenha: confirmacao,
      deveAlterarSenha: redefinirSenhaDeveAlterar.value,
    })

    redefinirSenhaDialogAberto.value = false
    redefinirSenhaNova.value = ''
    redefinirSenhaConfirmacao.value = ''
    redefinirSenhaDeveAlterar.value = true
    $q.notify({
      type: 'positive',
      message: response.mensagem || 'Senha redefinida com sucesso.',
    })
  } catch (error) {
    redefinirSenhaErro.value = error instanceof Error ? error.message : 'Nao foi possivel redefinir a senha do usuario.'
  } finally {
    redefinirSenhaLoading.value = false
  }
}

onMounted(() => {
  void carregarTela()
})

watch(
  () => route.params.id,
  (novoId, antigoId) => {
    if (novoId !== antigoId) {
      void carregarTela()
    }
  }
)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md cadastro-detalhe">
    <PageHeader
      :titulo="titulo"
      :subtitulo="subtituloCabecalho"
      contexto="Gestao administrativa"
    >
      <template #actions>
        <div class="row q-gutter-sm items-center">
          <StatusBadge v-if="!isNovo" :texto="registroAtivo ? 'Ativo' : 'Inativo'" />
          <q-btn flat icon="arrow_back" label="Voltar" @click="router.push(listPath)" />
          <q-btn
            v-if="podeRedefinirSenhaUsuario"
            color="warning"
            outline
            icon="password"
            label="Redefinir senha"
            :disable="loading"
            @click="abrirRedefinicaoSenha"
          />
          <q-btn
            v-if="podeGerenciarRegistro && !isNovo && registroAtivo"
            color="negative"
            outline
            icon="block"
            label="Inativar"
            :disable="loading"
            @click="confirmarInativacao = true"
          />
          <q-btn
            v-if="podeGerenciarRegistro && !isNovo && !registroAtivo"
            color="positive"
            outline
            icon="check_circle"
            label="Reativar"
            :disable="loading"
            @click="confirmarReativacao = true"
          />
        </div>
      </template>
    </PageHeader>

    <LoadingState v-if="loading && !carregamentoConcluido" mensagem="Carregando dados do cadastro..." />

    <ErrorState
      v-else-if="erro && !carregamentoConcluido"
      titulo="NÃ£o foi possÃ­vel carregar o cadastro"
      :mensagem="erro"
      @retry="carregarTela"
    />

    <template v-else>
      <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
      <q-banner v-if="sucesso" class="bg-green-1 text-positive">{{ sucesso }}</q-banner>
      <q-banner v-if="entidade === 'parametros'" rounded class="bg-orange-1 text-orange-10">
        Esta area controla configuracoes sensiveis do sistema. Revise antes de salvar.
      </q-banner>

      <FormCadastro
        :titulo="isNovo ? `${titulo} - Novo` : `${titulo} - Detalhe`"
        :subtitulo="subtituloFormulario"
        :loading="loading"
        :somente-leitura="somenteLeitura"
        @cancelar="() => router.push(listPath)"
        @salvar="salvar"
      >
        <div class="row q-col-gutter-md cadastro-detalhe__form-grid">
          <template v-if="entidade === 'usuarios'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="form.email" outlined dense label="E-mail" :readonly="somenteLeitura" :rules="[regraEmail]" />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="form.login" outlined dense label="Login" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.situacao"
                outlined
                dense
                emit-value
                map-options
                :disable="somenteLeitura || isNovo"
                :options="opcoesSituacao"
                label="SituaÃ§Ã£o"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.departamentoId"
                outlined
                dense
                emit-value
                map-options
                clearable
                :disable="somenteLeitura"
                :options="departamentos.map((item) => ({ label: `${item.sigla} - ${item.nome}`, value: item.id }))"
                label="Departamento"
              />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.perfilIds"
                outlined
                dense
                emit-value
                map-options
                multiple
                :disable="somenteLeitura || !podeEditarPerfisDoUsuario"
                :options="perfis.map((item) => ({ label: item.nome, value: item.id }))"
                label="Perfis"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>

          <template v-if="entidade === 'perfis'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.tipoPerfil"
                outlined
                dense
                emit-value
                map-options
                :disable="somenteLeitura"
                :options="opcoesTipoPerfil"
                label="Tipo de Perfil"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>

          <template v-if="entidade === 'departamentos'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-3">
              <q-input v-model="form.sigla" outlined dense label="Sigla" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'categorias'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.departamentoId"
                outlined
                dense
                emit-value
                map-options
                clearable
                :disable="somenteLeitura"
                :options="departamentos.map((item) => ({ label: `${item.sigla} - ${item.nome}`, value: item.id }))"
                label="Departamento"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'subcategorias'">
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.categoriaChamadoId"
                outlined
                dense
                emit-value
                map-options
                :disable="somenteLeitura"
                :options="categorias.map((item) => ({ label: item.nome, value: item.id }))"
                label="Categoria"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'prioridades'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-3">
              <q-input
                v-model.number="form.peso"
                outlined
                dense
                type="number"
                min="1"
                label="Peso"
                :readonly="somenteLeitura"
                :rules="[regraPesoPositivo]"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-input
                v-model="form.cor"
                outlined
                dense
                label="Cor (#RRGGBB)"
                :readonly="somenteLeitura"
                :rules="[regraCorHex]"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-chip
                square
                dense
                class="full-width justify-center"
                :style="{ backgroundColor: form.cor || '#e2e8f0', color: '#0f172a' }"
              >
                {{ form.cor || 'Sem cor' }}
              </q-chip>
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'tipos-solicitacao'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'locais'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.endereco"
                outlined
                dense
                label="EndereÃ§o"
                :readonly="somenteLeitura"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
              />
            </div>
          </template>

          <template v-if="entidade === 'status'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="form.codigo"
                outlined
                dense
                emit-value
                map-options
                :disable="somenteLeitura"
                :options="opcoesCodigoStatus"
                label="CÃ³digo"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-toggle v-model="form.ehStatusFinal" :disable="somenteLeitura" label="Status final" />
            </div>
            <div class="col-12 col-md-3">
              <q-toggle v-model="form.pausaSla" :disable="somenteLeitura" label="Pausa SLA" />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>

          <template v-if="entidade === 'parametros'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.chave" outlined dense label="Chave" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-6">
              <q-input
                v-model="form.valor"
                outlined
                dense
                :type="form.sensivel ? 'password' : 'text'"
                :label="form.sensivel ? 'Valor (mascarado)' : 'Valor'"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-toggle v-model="form.sensivel" :disable="somenteLeitura" label="SensÃ­vel" />
            </div>
            <div class="col-12 col-md-3">
              <q-badge
                :color="form.sensivel ? 'warning' : 'grey-6'"
                text-color="white"
                :label="form.sensivel ? 'ParÃ¢metro sensÃ­vel' : 'NÃ£o sensÃ­vel'"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="DescriÃ§Ã£o"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>
        </div>
      </FormCadastro>

      <AppSectionCard
        v-if="podeMostrarMatrizPermissoes"
        class="q-mt-md"
        titulo="Permissoes do perfil"
        subtitulo="Defina acessos por modulo e acao para este perfil."
      >
        <div class="text-caption text-grey-7 q-mb-md">
          Defina quais mÃ³dulos e aÃ§Ãµes este perfil pode acessar no SGX Sistema de Chamados.
        </div>

        <q-banner rounded class="bg-orange-1 text-orange-10 q-mb-md">
          PermissÃµes crÃ­ticas alteram recursos administrativos do sistema. Revise antes de salvar.
        </q-banner>

        <q-banner
          v-if="!podeEditarPermissoesPerfil"
          rounded
          class="bg-blue-1 text-blue-10 q-mb-md"
        >
          Somente administradores com permissÃ£o adequada podem alterar permissÃµes de perfil.
        </q-banner>

        <LoadingState
          v-if="loadingPermissoesPerfil"
          inline
          mensagem="Carregando permissÃµes do perfil..."
        />

        <ErrorState
          v-else-if="erroPermissoesPerfil"
          titulo="NÃ£o foi possÃ­vel carregar permissÃµes"
          :mensagem="erroPermissoesPerfil"
          @retry="carregarPermissoesPerfil"
        />

        <EmptyState
          v-else-if="!podeVisualizarPermissoesPerfil"
          titulo="Sem permissÃ£o de visualizaÃ§Ã£o"
          mensagem="VocÃª nÃ£o possui permissÃ£o para consultar a matriz deste perfil."
          icon="lock"
        />

        <EmptyState
          v-else-if="!modulosPermissoes.length"
          titulo="Nenhuma permissÃ£o disponÃ­vel"
          mensagem="NÃ£o hÃ¡ permissÃµes cadastradas para exibiÃ§Ã£o."
          icon="shield"
        />

        <template v-else>
          <q-expansion-item
            v-for="modulo in modulosPermissoes"
            :key="modulo.modulo"
            switch-toggle-side
            expand-separator
            default-opened
            :label="modulo.moduloLabel"
            icon="shield"
            class="q-mb-sm"
          >
            <div class="column q-gutter-sm q-px-md q-pb-md">
              <q-checkbox
                v-for="permissao in modulo.permissoes"
                :key="permissao.codigo"
                v-model="codigosPermissoesSelecionadas"
                :val="permissao.codigo"
                :disable="!podeEditarPermissoesPerfil || salvandoPermissoesPerfil"
              >
                <div class="column">
                  <div class="row items-center q-gutter-sm">
                    <span class="text-body2 text-weight-medium">{{ permissao.nome || permissao.codigo }}</span>
                    <q-badge
                      v-if="permissaoEhCritica(permissao.codigo)"
                      color="negative"
                      text-color="white"
                      label="CrÃ­tica"
                    />
                    <q-badge
                      v-else-if="permissoesVinculadasSet.has(permissao.codigo)"
                      color="positive"
                      text-color="white"
                      label="Vinculada"
                    />
                  </div>
                  <span class="text-caption text-grey-7">{{ permissao.codigo }}</span>
                  <span v-if="permissao.descricao" class="text-caption text-grey-8">{{ permissao.descricao }}</span>
                </div>
              </q-checkbox>
            </div>
          </q-expansion-item>

          <q-separator class="q-my-md" />

          <div class="row items-center justify-between q-gutter-sm">
            <div class="text-caption text-grey-8">
              Total selecionado: {{ totalPermissoesSelecionadas }}
            </div>

            <q-btn
              v-if="podeEditarPermissoesPerfil"
              color="primary"
              icon="save"
              label="Salvar permissÃµes"
              :loading="salvandoPermissoesPerfil"
              @click="salvarPermissoesPerfil"
            />
          </div>

          <q-banner v-if="sucessoPermissoesPerfil" rounded class="bg-green-1 text-positive q-mt-md">
            {{ sucessoPermissoesPerfil }}
          </q-banner>
        </template>
      </AppSectionCard>
    </template>

    <ConfirmDialog
      v-model="confirmarInativacao"
      titulo="Confirmar inativaÃ§Ã£o"
      mensagem="Deseja realmente inativar este cadastro?"
      color="negative"
      confirmar-label="Inativar"
      :loading="loading"
      @confirm="inativar"
    />

    <ConfirmDialog
      v-model="confirmarReativacao"
      titulo="Confirmar reativaÃ§Ã£o"
      mensagem="Deseja realmente reativar este cadastro?"
      color="positive"
      confirmar-label="Reativar"
      :loading="loading"
      @confirm="reativar"
    />
    <q-dialog v-model="redefinirSenhaDialogAberto" persistent>
      <q-card style="min-width: 420px; max-width: 94vw">
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">Redefinir senha do usuário</div>
          <q-space />
          <q-btn icon="close" flat round dense aria-label="Fechar redefinição de senha" :disable="redefinirSenhaLoading" v-close-popup />
        </q-card-section>

        <q-card-section>
          <q-banner rounded class="bg-orange-1 text-orange-10 q-mb-md">
            Defina uma senha forte. A senha é armazenada com hash seguro.
          </q-banner>

          <q-banner v-if="redefinirSenhaErro" rounded class="bg-red-1 text-negative q-mb-md">
            {{ redefinirSenhaErro }}
          </q-banner>

          <q-form class="q-gutter-md" @submit.prevent="confirmarRedefinicaoSenha">
            <q-input
              v-model="redefinirSenhaNova"
              outlined
              dense
              type="password"
              autocomplete="new-password"
              label="Nova senha"
            />
            <q-input
              v-model="redefinirSenhaConfirmacao"
              outlined
              dense
              type="password"
              autocomplete="new-password"
              label="Confirmar nova senha"
            />
            <q-toggle v-model="redefinirSenhaDeveAlterar" label="Exigir troca no próximo login" />
          </q-form>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" :disable="redefinirSenhaLoading" v-close-popup />
          <q-btn
            color="primary"
            label="Salvar senha"
            :loading="redefinirSenhaLoading"
            @click="confirmarRedefinicaoSenha"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
.cadastro-detalhe__form-grid {
  row-gap: var(--sgx-space-3);
}
</style>


