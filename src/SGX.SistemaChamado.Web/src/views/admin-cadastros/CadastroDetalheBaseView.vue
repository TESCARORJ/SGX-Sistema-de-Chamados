<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../../components/admin/cadastros/FormCadastro.vue'
import ConfirmDialog from '../../components/ui/ConfirmDialog.vue'
import EmptyState from '../../components/ui/EmptyState.vue'
import ErrorState from '../../components/ui/ErrorState.vue'
import LoadingState from '../../components/ui/LoadingState.vue'
import PageHeader from '../../components/ui/PageHeader.vue'
import { permissoes, permissoesCriticas } from '../../constants/permissoes'
import { cadastrosAdminService } from '../../services/cadastrosAdminService'
import { parametrosSistemaService } from '../../services/parametrosSistemaService'
import { usuariosAdminService } from '../../services/usuariosAdminService'
import { useAuthStore } from '../../stores/authStore'
import type {
  DepartamentoResumoResponse,
  PerfilAcessoResumoResponse,
  PerfilPermissoes,
  PermissaoSistema,
} from '../../types/adminCadastros'

type Entidade = 'usuarios' | 'perfis' | 'departamentos' | 'categorias' | 'prioridades' | 'status' | 'parametros'

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

const perfis = ref<PerfilAcessoResumoResponse[]>([])
const departamentos = ref<DepartamentoResumoResponse[]>([])

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const isAdmin = computed(() => authStore.usuario?.perfis.includes('Administrador') ?? false)
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

const form = reactive({
  nome: '',
  email: '',
  login: '',
  situacao: 1,
  departamentoId: null as string | null,
  perfilIds: [] as string[],
  sigla: '',
  descricao: '',
  nivel: 1,
  prazoPrimeiraRespostaHoras: 0,
  prazoResolucaoHoras: 0,
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

const opcoesNivel = [
  { label: 'Baixa', value: 1 },
  { label: 'Média', value: 2 },
  { label: 'Alta', value: 3 },
  { label: 'Crítica', value: 4 },
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
    return Number.isFinite(valor) ? true : 'Campo obrigatório.'
  }

  if (Array.isArray(valor)) {
    return valor.length > 0 ? true : 'Campo obrigatório.'
  }

  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatório.'
}

const regraEmail = (valor: string): true | string => {
  if (!valor?.trim()) {
    return 'Campo obrigatório.'
  }

  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(valor) ? true : 'Informe um e-mail válido.'
}

const regraNumeroNaoNegativo = (valor: number): true | string =>
  Number(valor) >= 0 ? true : 'Informe um valor maior ou igual a zero.'

function mapModuloLabel(modulo: string): string {
  const mapa: Record<string, string> = {
    Dashboard: 'Dashboard',
    Chamados: 'Chamados',
    Cadastros: 'Cadastros',
    Usuarios: 'Usuários',
    Perfis: 'Perfis',
    Parametros: 'Parâmetros',
    IntegracoesEmail: 'Integrações',
    Notificacoes: 'Notificações',
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
  form.descricao = ''
  form.nivel = 1
  form.prazoPrimeiraRespostaHoras = 0
  form.prazoResolucaoHoras = 0
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
    case 'prioridades': {
      const prioridade = await cadastrosAdminService.obterPrioridadePorId(idParam.value)
      form.nome = prioridade.nome
      form.descricao = prioridade.descricao ?? ''
      form.nivel = prioridade.nivel
      form.prazoPrimeiraRespostaHoras = prioridade.prazoPrimeiraRespostaHoras
      form.prazoResolucaoHoras = prioridade.prazoResolucaoHoras
      registroAtivo.value = prioridade.ativo
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
      error instanceof Error ? error.message : 'Não foi possível carregar as permissões do perfil.'
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
    sucessoPermissoesPerfil.value = 'Permissões atualizadas com sucesso.'
    $q.notify({
      type: 'positive',
      message: 'Permissões do perfil salvas com sucesso.',
    })
  } catch (error) {
    erroPermissoesPerfil.value =
      error instanceof Error ? error.message : 'Não foi possível salvar as permissões do perfil.'
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
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
      case 'prioridades':
        if (isNovo.value) {
          const created = await cadastrosAdminService.criarPrioridade({
            nome: form.nome,
            descricao: form.descricao || null,
            nivel: form.nivel,
            prazoPrimeiraRespostaHoras: form.prazoPrimeiraRespostaHoras,
            prazoResolucaoHoras: form.prazoResolucaoHoras,
          })
          await router.replace(`${props.listPath}/${created.id}`)
        } else {
          await cadastrosAdminService.atualizarPrioridade(idParam.value, {
            nome: form.nome,
            descricao: form.descricao || null,
            nivel: form.nivel,
            prazoPrimeiraRespostaHoras: form.prazoPrimeiraRespostaHoras,
            prazoResolucaoHoras: form.prazoResolucaoHoras,
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível salvar as informações.'
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
      case 'prioridades':
        await cadastrosAdminService.inativarPrioridade(idParam.value)
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
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
      case 'prioridades':
        await cadastrosAdminService.reativarPrioridade(idParam.value)
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
  } finally {
    loading.value = false
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
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="titulo"
      :subtitulo="isNovo ? 'Criação de novo cadastro' : 'Detalhe e manutenção de cadastro'"
    >
      <template #actions>
        <div class="row q-gutter-sm items-center">
          <q-badge
            v-if="!isNovo"
            :color="registroAtivo ? 'positive' : 'grey-7'"
            text-color="white"
            :label="registroAtivo ? 'Ativo' : 'Inativo'"
          />
          <q-btn flat icon="arrow_back" label="Voltar" @click="router.push(listPath)" />
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
      titulo="Não foi possível carregar o cadastro"
      :mensagem="erro"
      @retry="carregarTela"
    />

    <template v-else>
      <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
      <q-banner v-if="sucesso" class="bg-green-1 text-positive">{{ sucesso }}</q-banner>

      <FormCadastro
        :titulo="isNovo ? `${titulo} - Novo` : `${titulo} - Detalhe`"
        :loading="loading"
        :somente-leitura="somenteLeitura"
        @cancelar="() => router.push(listPath)"
        @salvar="salvar"
      >
        <div class="row q-col-gutter-md">
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
                label="Situação"
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
                label="Descrição"
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
                label="Descrição"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
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
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="Descrição"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>

          <template v-if="entidade === 'prioridades'">
            <div class="col-12 col-md-6">
              <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
            </div>
            <div class="col-12 col-md-3">
              <q-select
                v-model="form.nivel"
                outlined
                dense
                emit-value
                map-options
                :disable="somenteLeitura"
                :options="opcoesNivel"
                label="Nível"
                :rules="[regraObrigatoria]"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-input
                v-model.number="form.prazoPrimeiraRespostaHoras"
                outlined
                dense
                type="number"
                label="Prazo 1a resposta (h)"
                :readonly="somenteLeitura"
                :rules="[regraNumeroNaoNegativo]"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-input
                v-model.number="form.prazoResolucaoHoras"
                outlined
                dense
                type="number"
                label="Prazo resolução (h)"
                :readonly="somenteLeitura"
                :rules="[regraNumeroNaoNegativo]"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="Descrição"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
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
                label="Código"
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
                label="Descrição"
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
              <q-toggle v-model="form.sensivel" :disable="somenteLeitura" label="Sensível" />
            </div>
            <div class="col-12 col-md-3">
              <q-badge
                :color="form.sensivel ? 'warning' : 'grey-6'"
                text-color="white"
                :label="form.sensivel ? 'Parâmetro sensível' : 'Não sensível'"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="Descrição"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>
        </div>
      </FormCadastro>

      <q-card
        v-if="podeMostrarMatrizPermissoes"
        flat
        bordered
        class="sgx-card q-pa-md q-mt-md"
      >
        <div class="text-h6">Permissões do perfil</div>
        <div class="text-caption text-grey-7 q-mb-md">
          Defina quais módulos e ações este perfil pode acessar no SGX Sistema de Chamados.
        </div>

        <q-banner rounded class="bg-orange-1 text-orange-10 q-mb-md">
          Permissões críticas alteram recursos administrativos do sistema. Revise antes de salvar.
        </q-banner>

        <q-banner
          v-if="!podeEditarPermissoesPerfil"
          rounded
          class="bg-blue-1 text-blue-10 q-mb-md"
        >
          Somente administradores com permissão adequada podem alterar permissões de perfil.
        </q-banner>

        <LoadingState
          v-if="loadingPermissoesPerfil"
          inline
          mensagem="Carregando permissões do perfil..."
        />

        <ErrorState
          v-else-if="erroPermissoesPerfil"
          titulo="Não foi possível carregar permissões"
          :mensagem="erroPermissoesPerfil"
          @retry="carregarPermissoesPerfil"
        />

        <EmptyState
          v-else-if="!podeVisualizarPermissoesPerfil"
          titulo="Sem permissão de visualização"
          mensagem="Você não possui permissão para consultar a matriz deste perfil."
          icon="lock"
        />

        <EmptyState
          v-else-if="!modulosPermissoes.length"
          titulo="Nenhuma permissão disponível"
          mensagem="Não há permissões cadastradas para exibição."
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
                      label="Crítica"
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
              label="Salvar permissões"
              :loading="salvandoPermissoesPerfil"
              @click="salvarPermissoesPerfil"
            />
          </div>

          <q-banner v-if="sucessoPermissoesPerfil" rounded class="bg-green-1 text-positive q-mt-md">
            {{ sucessoPermissoesPerfil }}
          </q-banner>
        </template>
      </q-card>
    </template>

    <ConfirmDialog
      v-model="confirmarInativacao"
      titulo="Confirmar inativação"
      mensagem="Deseja realmente inativar este cadastro?"
      color="negative"
      confirmar-label="Inativar"
      :loading="loading"
      @confirm="inativar"
    />

    <ConfirmDialog
      v-model="confirmarReativacao"
      titulo="Confirmar reativação"
      mensagem="Deseja realmente reativar este cadastro?"
      color="positive"
      confirmar-label="Reativar"
      :loading="loading"
      @confirm="reativar"
    />
  </q-page>
</template>
