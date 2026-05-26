<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../components/admin/cadastros/FormCadastro.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { baseConhecimentoAdminService } from '../services/baseConhecimentoAdminService'
import { cadastrosAdminService } from '../services/cadastrosAdminService'
import { catalogoServicosAdminService } from '../services/catalogoServicosAdminService'
import { slaPoliciesService } from '../services/slaPoliciesService'
import { useAuthStore } from '../stores/authStore'
import type {
  CategoriaChamadoResumoResponse,
  DepartamentoResumoResponse,
  PrioridadeChamadoResumoResponse,
  SubcategoriaChamadoResumoResponse,
} from '../types/adminCadastros'
import { StatusArtigoConhecimento, type BaseConhecimentoArtigoListagem } from '../types/baseConhecimento'
import {
  StatusCatalogoServico,
  VisibilidadeCatalogoServico,
  type CatalogoServicoDetalhe,
  type CriarCatalogoServicoRequest,
} from '../types/catalogoServicos'
import type { PoliticaSlaResponse } from '../types/slaPolicies'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const carregamentoConcluido = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const departamentos = ref<DepartamentoResumoResponse[]>([])
const categorias = ref<CategoriaChamadoResumoResponse[]>([])
const subcategorias = ref<SubcategoriaChamadoResumoResponse[]>([])
const prioridades = ref<PrioridadeChamadoResumoResponse[]>([])
const politicasSla = ref<PoliticaSlaResponse[]>([])
const artigosConhecimento = ref<BaseConhecimentoArtigoListagem[]>([])

const servicoAtual = ref<CatalogoServicoDetalhe | null>(null)

const form = reactive({
  nome: '',
  descricao: '',
  instrucoesSolicitante: '',
  departamentoResponsavelId: '',
  categoriaId: null as string | null,
  subcategoriaId: null as string | null,
  prioridadePadraoId: null as string | null,
  slaPadraoId: null as string | null,
  artigoBaseConhecimentoId: null as string | null,
  visibilidade: VisibilidadeCatalogoServico.Interno,
  permiteAberturaChamado: true,
  requerAprovacao: false,
  ordem: 0,
  ativo: true,
  statusDescricao: '',
  slug: '',
})

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.catalogoServicosVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.catalogoServicosGerenciar))
const somenteLeitura = computed(() => !podeGerenciar.value)

const opcoesVisibilidade = [
  { label: 'Interno', value: VisibilidadeCatalogoServico.Interno },
  { label: 'Solicitante', value: VisibilidadeCatalogoServico.Solicitante },
  { label: 'Atendente', value: VisibilidadeCatalogoServico.Atendente },
  { label: 'Administrador', value: VisibilidadeCatalogoServico.Administrador },
]

const subcategoriasFiltradas = computed(() => {
  if (!form.categoriaId) {
    return subcategorias.value
  }

  return subcategorias.value.filter((item) => item.categoriaChamadoId === form.categoriaId)
})

const statusEmEdicao = computed(() => {
  if (!servicoAtual.value) {
    return null
  }

  return servicoAtual.value.status
})

function regraObrigatoria(valor: unknown): true | string {
  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatorio.'
}

function regraOrdem(valor: unknown): true | string {
  if (valor === null || valor === undefined || String(valor).trim() === '') {
    return true
  }

  const numero = Number(valor)
  if (!Number.isFinite(numero)) {
    return 'Informe um numero valido.'
  }

  if (numero < 0) {
    return 'A ordem nao pode ser negativa.'
  }

  return true
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

function mapearStatusDescricao(status: StatusCatalogoServico): string {
  switch (status) {
    case StatusCatalogoServico.Publicado:
      return 'Publicado'
    case StatusCatalogoServico.Arquivado:
      return 'Arquivado'
    default:
      return 'Rascunho'
  }
}

function aplicarDetalheNoFormulario(servico: CatalogoServicoDetalhe): void {
  servicoAtual.value = servico
  form.nome = servico.nome
  form.descricao = servico.descricao ?? ''
  form.instrucoesSolicitante = servico.instrucoesSolicitante ?? ''
  form.departamentoResponsavelId = servico.departamentoResponsavelId
  form.categoriaId = servico.categoriaId
  form.subcategoriaId = servico.subcategoriaId
  form.prioridadePadraoId = servico.prioridadePadraoId
  form.slaPadraoId = servico.slaPadraoId
  form.artigoBaseConhecimentoId = servico.artigoBaseConhecimentoId
  form.visibilidade = servico.visibilidade
  form.permiteAberturaChamado = servico.permiteAberturaChamado
  form.requerAprovacao = servico.requerAprovacao
  form.ordem = servico.ordem
  form.ativo = servico.ativo
  form.statusDescricao = servico.statusDescricao || mapearStatusDescricao(servico.status)
  form.slug = servico.slug
}

function limparFormulario(): void {
  servicoAtual.value = null
  form.nome = ''
  form.descricao = ''
  form.instrucoesSolicitante = ''
  form.departamentoResponsavelId = ''
  form.categoriaId = null
  form.subcategoriaId = null
  form.prioridadePadraoId = null
  form.slaPadraoId = null
  form.artigoBaseConhecimentoId = null
  form.visibilidade = VisibilidadeCatalogoServico.Interno
  form.permiteAberturaChamado = true
  form.requerAprovacao = false
  form.ordem = 0
  form.ativo = true
  form.statusDescricao = ''
  form.slug = ''
}

async function carregarReferenciasObrigatorias(): Promise<void> {
  const [departamentosResponse, categoriasResponse, subcategoriasResponse, prioridadesResponse] = await Promise.all([
    cadastrosAdminService.listarDepartamentos({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarSubcategorias({ ativo: true, tamanhoPagina: 200 }),
    cadastrosAdminService.listarPrioridades({ ativo: true, tamanhoPagina: 100 }),
  ])

  departamentos.value = departamentosResponse.items
  categorias.value = categoriasResponse.items
  subcategorias.value = subcategoriasResponse.items
  prioridades.value = prioridadesResponse.items
}

async function carregarReferenciasOpcionais(): Promise<void> {
  const tarefas = [
    slaPoliciesService
      .listar({ ativo: true })
      .then((data) => {
        politicasSla.value = data
      })
      .catch(() => {
        politicasSla.value = []
      }),
    baseConhecimentoAdminService
      .listarArtigos({ ativo: true, status: StatusArtigoConhecimento.Publicado, tamanhoPagina: 100 })
      .then((data) => {
        artigosConhecimento.value = data.items
      })
      .catch(() => {
        artigosConhecimento.value = []
      }),
  ]

  await Promise.all(tarefas)
}

async function carregarDetalhe(): Promise<void> {
  if (isNovo.value) {
    limparFormulario()
    return
  }

  const detalhe = await catalogoServicosAdminService.obterServico(idParam.value)
  aplicarDetalheNoFormulario(detalhe)
}

async function carregarTela(): Promise<void> {
  if ((!isNovo.value && !podeVisualizar.value) || (isNovo.value && !podeGerenciar.value)) {
    carregamentoConcluido.value = true
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await carregarReferenciasObrigatorias()
    await carregarReferenciasOpcionais()
    await carregarDetalhe()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados do servico.')
  } finally {
    loading.value = false
    carregamentoConcluido.value = true
  }
}

function montarPayloadBase(): CriarCatalogoServicoRequest {
  const slaId = form.slaPadraoId || null

  return {
    nome: form.nome.trim(),
    descricao: form.descricao.trim(),
    instrucoesSolicitante: form.instrucoesSolicitante.trim() || null,
    departamentoResponsavelId: form.departamentoResponsavelId,
    categoriaId: form.categoriaId,
    subcategoriaId: form.subcategoriaId,
    prioridadePadraoId: form.prioridadePadraoId,
    slaPadraoId: slaId,
    politicaSlaId: slaId,
    artigoBaseConhecimentoId: form.artigoBaseConhecimentoId,
    visibilidade: form.visibilidade,
    permiteAberturaChamado: form.permiteAberturaChamado,
    requerAprovacao: form.requerAprovacao,
    ordem: Number(form.ordem) || 0,
  }
}

async function salvar(): Promise<void> {
  if (somenteLeitura.value) {
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    if (isNovo.value) {
      const criado = await catalogoServicosAdminService.criarServico(montarPayloadBase())
      $q.notify({ type: 'positive', message: 'Servico criado com sucesso.' })
      await router.replace(`/admin/conhecimento/catalogo-servicos/${criado.id}`)
      return
    }

    const atualizado = await catalogoServicosAdminService.atualizarServico(idParam.value, {
      ...montarPayloadBase(),
      ativo: form.ativo,
    })

    aplicarDetalheNoFormulario(atualizado)
    sucesso.value = 'Servico salvo com sucesso.'
    $q.notify({ type: 'positive', message: 'Servico atualizado com sucesso.' })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar o servico.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    loading.value = false
  }
}

function voltar(): void {
  router.push('/admin/conhecimento/catalogo-servicos')
}

watch(
  () => route.params.id,
  async () => {
    carregamentoConcluido.value = false
    await carregarTela()
  }
)

watch(
  () => form.categoriaId,
  () => {
    if (!form.categoriaId) {
      form.subcategoriaId = null
      return
    }

    if (!subcategoriasFiltradas.value.some((item) => item.id === form.subcategoriaId)) {
      form.subcategoriaId = null
    }
  }
)

onMounted(async () => {
  await carregarTela()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="isNovo ? 'Catalogo de Servicos - Novo servico' : 'Catalogo de Servicos - Detalhe do servico'"
      subtitulo="Cadastro institucional de servicos por departamento, com visibilidade e ciclo de vida controlados."
    />

    <q-banner v-if="isNovo && !podeGerenciar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para criar servicos do catalogo.
    </q-banner>

    <q-banner v-else-if="!isNovo && !podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar este servico.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading && !carregamentoConcluido" mensagem="Carregando dados do servico..." />

      <ErrorState
        v-else-if="erro && !carregamentoConcluido"
        titulo="Nao foi possivel carregar o servico"
        :mensagem="erro"
        @retry="carregarTela"
      />

      <template v-else>
        <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
          {{ erro }}
        </q-banner>

        <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">
          {{ sucesso }}
        </q-banner>

        <FormCadastro
          :titulo="isNovo ? 'Novo servico' : 'Detalhe do servico'"
          :loading="loading"
          :somente-leitura="somenteLeitura"
          botao-salvar-label="Salvar servico"
          @salvar="salvar"
          @cancelar="voltar"
        >
          <div class="column q-gutter-md">
            <AppSectionCard titulo="Informacoes gerais" subtitulo="Dados principais e descricao funcional do servico.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-8">
                  <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.visibilidade"
                    outlined
                    dense
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="opcoesVisibilidade"
                    label="Visibilidade"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12">
                  <q-input
                    v-model="form.descricao"
                    outlined
                    dense
                    type="textarea"
                    autogrow
                    label="Descricao"
                    :readonly="somenteLeitura"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12">
                  <q-input
                    v-model="form.instrucoesSolicitante"
                    outlined
                    dense
                    type="textarea"
                    autogrow
                    label="Instrucoes ao solicitante"
                    :readonly="somenteLeitura"
                  />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard titulo="Classificacao e referencias" subtitulo="Vincule departamento, categoria, prioridade e base de conhecimento.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.departamentoResponsavelId"
                    outlined
                    dense
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
                    label="Departamento responsavel"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.categoriaId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="categorias.map((item) => ({ label: item.nome, value: item.id }))"
                    label="Categoria"
                  />
                </div>

                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.subcategoriaId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="subcategoriasFiltradas.map((item) => ({ label: item.nome, value: item.id }))"
                    label="Subcategoria"
                  />
                </div>

                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.prioridadePadraoId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="prioridades.map((item) => ({ label: item.nome, value: item.id }))"
                    label="Prioridade padrao"
                  />
                </div>

                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.slaPadraoId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="politicasSla.map((item) => ({ label: item.nome, value: item.id }))"
                    label="SLA padrao / Politica SLA"
                  />
                </div>

                <div class="col-12 col-md-6">
                  <q-select
                    v-model="form.artigoBaseConhecimentoId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="artigosConhecimento.map((item) => ({ label: item.titulo, value: item.id }))"
                    label="Artigo da base de conhecimento"
                  />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard titulo="Disponibilidade e governanca" subtitulo="Controle comportamento de abertura, aprovacao e ciclo de vida.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-3">
                  <q-input
                    v-model.number="form.ordem"
                    outlined
                    dense
                    type="number"
                    min="0"
                    label="Ordem"
                    :readonly="somenteLeitura"
                    :rules="[regraOrdem]"
                  />
                </div>

                <div class="col-12 col-md-3">
                  <q-toggle v-model="form.permiteAberturaChamado" :disable="somenteLeitura" label="Permite abertura de chamado" />
                </div>

                <div class="col-12 col-md-3">
                  <q-toggle v-model="form.requerAprovacao" :disable="somenteLeitura" label="Requer aprovacao" />
                </div>

                <div class="col-12 col-md-3">
                  <q-toggle
                    v-model="form.ativo"
                    :disable="somenteLeitura || statusEmEdicao === StatusCatalogoServico.Arquivado"
                    label="Ativo"
                  />
                </div>

                <div v-if="!isNovo" class="col-12 col-md-3">
                  <q-input v-model="form.statusDescricao" outlined dense readonly label="Status" />
                </div>

                <div v-if="!isNovo" class="col-12 col-md-9">
                  <q-input v-model="form.slug" outlined dense readonly label="Slug" />
                </div>
              </div>
            </AppSectionCard>
          </div>
        </FormCadastro>
      </template>
    </template>
  </q-page>
</template>
