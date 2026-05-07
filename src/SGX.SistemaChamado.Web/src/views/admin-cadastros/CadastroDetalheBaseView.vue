<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../../components/admin/cadastros/FormCadastro.vue'
import ConfirmDialog from '../../components/ui/ConfirmDialog.vue'
import ErrorState from '../../components/ui/ErrorState.vue'
import LoadingState from '../../components/ui/LoadingState.vue'
import PageHeader from '../../components/ui/PageHeader.vue'
import { cadastrosAdminService } from '../../services/cadastrosAdminService'
import { parametrosSistemaService } from '../../services/parametrosSistemaService'
import { usuariosAdminService } from '../../services/usuariosAdminService'
import { useAuthStore } from '../../stores/authStore'
import type { DepartamentoResumoResponse, PerfilAcessoResumoResponse } from '../../types/adminCadastros'

type Entidade = 'usuarios' | 'perfis' | 'departamentos' | 'categorias' | 'prioridades' | 'status' | 'parametros'

const props = defineProps<{
  titulo: string
  entidade: Entidade
  listPath: string
}>()

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const carregamentoConcluido = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const confirmarInativacao = ref(false)
const confirmarReativacao = ref(false)
const registroAtivo = ref(true)

const perfis = ref<PerfilAcessoResumoResponse[]>([])
const departamentos = ref<DepartamentoResumoResponse[]>([])

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const isAdmin = computed(() => authStore.usuario?.perfis.includes('Administrador') ?? false)
const somenteLeitura = computed(() => !isAdmin.value)

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
  { label: 'Media', value: 2 },
  { label: 'Alta', value: 3 },
  { label: 'Critica', value: 4 },
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
    return Number.isFinite(valor) ? true : 'Campo obrigatorio.'
  }

  if (Array.isArray(valor)) {
    return valor.length > 0 ? true : 'Campo obrigatorio.'
  }

  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatorio.'
}

const regraEmail = (valor: string): true | string => {
  if (!valor?.trim()) {
    return 'Campo obrigatorio.'
  }

  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(valor) ? true : 'Informe um e-mail valido.'
}

const regraNumeroNaoNegativo = (valor: number): true | string =>
  Number(valor) >= 0 ? true : 'Informe um valor maior ou igual a zero.'

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

async function carregarTela(): Promise<void> {
  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await carregarAuxiliares()
    await carregarDetalhe()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar dados.'
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
          await usuariosAdminService.alterarPerfis(idParam.value, { perfilIds: form.perfilIds })
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
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar cadastro.'
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
    erro.value = error instanceof Error ? error.message : 'Falha ao inativar.'
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
    erro.value = error instanceof Error ? error.message : 'Falha ao reativar.'
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
      :subtitulo="isNovo ? 'Criacao de novo cadastro' : 'Detalhe e manutencao de cadastro'"
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
            v-if="isAdmin && !isNovo && registroAtivo"
            color="negative"
            outline
            icon="block"
            label="Inativar"
            :disable="loading"
            @click="confirmarInativacao = true"
          />
          <q-btn
            v-if="isAdmin && !isNovo && !registroAtivo"
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
      titulo="Nao foi possivel carregar o cadastro"
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
                label="Situacao"
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
                :disable="somenteLeitura"
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
                label="Descricao"
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
                label="Descricao"
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
                label="Descricao"
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
                label="Nivel"
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
                label="Prazo resolucao (h)"
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
                label="Descricao"
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
                label="Codigo"
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
                label="Descricao"
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
              <q-toggle v-model="form.sensivel" :disable="somenteLeitura" label="Sensivel" />
            </div>
            <div class="col-12 col-md-3">
              <q-badge
                :color="form.sensivel ? 'warning' : 'grey-6'"
                text-color="white"
                :label="form.sensivel ? 'Parametro sensivel' : 'Nao sensivel'"
              />
            </div>
            <div class="col-12">
              <q-input
                v-model="form.descricao"
                outlined
                dense
                type="textarea"
                label="Descricao"
                :readonly="somenteLeitura"
                :rules="[regraObrigatoria]"
              />
            </div>
          </template>
        </div>
      </FormCadastro>
    </template>

    <ConfirmDialog
      v-model="confirmarInativacao"
      titulo="Confirmar inativacao"
      mensagem="Deseja realmente inativar este cadastro?"
      color="negative"
      confirmar-label="Inativar"
      :loading="loading"
      @confirm="inativar"
    />

    <ConfirmDialog
      v-model="confirmarReativacao"
      titulo="Confirmar reativacao"
      mensagem="Deseja realmente reativar este cadastro?"
      color="positive"
      confirmar-label="Reativar"
      :loading="loading"
      @confirm="reativar"
    />
  </q-page>
</template>
