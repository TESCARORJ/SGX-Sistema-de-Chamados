<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import type { QForm } from 'quasar'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import FormularioDinamicoCatalogoSection from '../components/portal/FormularioDinamicoCatalogoSection.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { catalogoServicosPortalService } from '../services/catalogoServicosPortalService'
import { portalService } from '../services/portalService'
import type {
  PortalFormularioPreparacaoCampo,
  PortalPrepararChamadoCatalogoServico,
} from '../types/catalogoServicos'
import {
  ImpactoChamado,
  NaturezaChamado,
  type RespostaFormularioAberturaRequest,
  UrgenciaChamado,
  type CategoriaPortal,
  type DepartamentoPortal,
  type LocalUnidadePortal,
  type PrioridadePortal,
  type SubcategoriaPortal,
  type TipoSolicitacaoPortal,
} from '../types/portal'
import { TipoCampoFormularioServico } from '../types/formularioServicos'

const EXTENSOES_PADRAO = ['.pdf', '.png', '.jpg', '.jpeg', '.txt', '.doc', '.docx', '.xls', '.xlsx']

const EXTENSOES_POR_CONTENT_TYPE: Record<string, string[]> = {
  'application/pdf': ['.pdf'],
  'image/png': ['.png'],
  'image/jpeg': ['.jpg', '.jpeg'],
  'text/plain': ['.txt'],
  'application/msword': ['.doc'],
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx'],
  'application/vnd.ms-excel': ['.xls'],
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
}

const route = useRoute()
const router = useRouter()
const $q = useQuasar()
const formRef = ref<QForm | null>(null)

const carregandoContexto = ref(false)
const carregandoCatalogoSelecionado = ref(false)
const salvando = ref(false)
const erroContexto = ref<string | null>(null)
const erroSalvar = ref<string | null>(null)
const erroAnexo = ref<string | null>(null)

const departamentos = ref<DepartamentoPortal[]>([])
const categorias = ref<CategoriaPortal[]>([])
const subcategorias = ref<SubcategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])
const tiposSolicitacao = ref<TipoSolicitacaoPortal[]>([])
const locaisUnidade = ref<LocalUnidadePortal[]>([])

const anexosPendentes = ref<File[]>([])
const extensoesPermitidas = ref<string[]>(EXTENSOES_PADRAO)
const tamanhoMaximoAnexoBytes = ref<number | null>(null)
const servicoSelecionado = ref<PortalPrepararChamadoCatalogoServico | null>(null)
const respostasFormulario = reactive<Record<string, boolean | string | string[] | null>>({})

type OrientacaoNaturezaConfig = {
  descricao: string
  obrigatoriedades: string[]
}

const ORIENTACOES_NATUREZA: Record<NaturezaChamado, OrientacaoNaturezaConfig> = {
  [NaturezaChamado.Incidente]: {
    descricao: 'Use para falha, erro, indisponibilidade, lentidao ou interrupcao de servico.',
    obrigatoriedades: ['Titulo', 'Descricao', 'Impacto', 'Urgencia'],
  },
  [NaturezaChamado.Requisicao]: {
    descricao: 'Use para solicitacao de servico, acesso, informacao ou atendimento padrao.',
    obrigatoriedades: ['Titulo', 'Descricao', 'Classificacao (Categoria/Tipo/Catalogo)'],
  },
  [NaturezaChamado.Mudanca]: {
    descricao: 'Use para alteracao planejada em sistema, infraestrutura, configuracao ou processo.',
    obrigatoriedades: ['Titulo', 'Descricao detalhada', 'Impacto', 'Urgencia'],
  },
  [NaturezaChamado.Problema]: {
    descricao: 'Use para analise de causa raiz, falhas recorrentes ou problemas repetitivos.',
    obrigatoriedades: ['Titulo', 'Descricao com evidencias/recorrencia', 'Impacto', 'Urgencia'],
  },
  [NaturezaChamado.EventoAlerta]: {
    descricao: 'Use para evento monitorado, alerta tecnico ou notificacao automatica.',
    obrigatoriedades: ['Titulo', 'Descricao', 'Impacto', 'Urgencia'],
  },
  [NaturezaChamado.TarefaOperacional]: {
    descricao: 'Use para atividade interna, rotina operacional ou execucao tecnica controlada.',
    obrigatoriedades: ['Titulo', 'Descricao', 'Impacto', 'Urgencia'],
  },
}

const opcoesNatureza = [
  { label: 'Incidente', value: NaturezaChamado.Incidente },
  { label: 'Requisicao', value: NaturezaChamado.Requisicao },
  { label: 'Mudanca', value: NaturezaChamado.Mudanca },
  { label: 'Problema', value: NaturezaChamado.Problema },
  { label: 'Evento/Alerta', value: NaturezaChamado.EventoAlerta },
  { label: 'Tarefa Operacional', value: NaturezaChamado.TarefaOperacional },
]

const opcoesImpacto = [
  { label: 'Baixo', value: ImpactoChamado.Baixo },
  { label: 'Medio', value: ImpactoChamado.Medio },
  { label: 'Alto', value: ImpactoChamado.Alto },
]

const opcoesUrgencia = [
  { label: 'Baixa', value: UrgenciaChamado.Baixa },
  { label: 'Media', value: UrgenciaChamado.Media },
  { label: 'Alta', value: UrgenciaChamado.Alta },
]

const form = reactive({
  naturezaChamado: null as NaturezaChamado | null,
  impactoChamado: null as ImpactoChamado | null,
  urgenciaChamado: null as UrgenciaChamado | null,
  titulo: '',
  descricao: '',
  catalogoServicoId: null as string | null,
  catalogoServicoSlug: null as string | null,
  departamentoId: null as string | null,
  categoriaId: null as string | null,
  subcategoriaId: null as string | null,
  prioridadeId: null as string | null,
  tipoSolicitacaoId: null as string | null,
  localUnidadeId: null as string | null,
})

const exibirDepartamento = computed(() => departamentos.value.length > 0)
const aberturaPorCatalogo = computed(() => Boolean(form.catalogoServicoId))
const orientacaoNaturezaSelecionada = computed(() =>
  form.naturezaChamado ? ORIENTACOES_NATUREZA[form.naturezaChamado] : null
)

const opcoesDepartamento = computed(() =>
  departamentos.value.map((item) => ({
    label: `${item.sigla} - ${item.nome}`,
    value: item.id,
  }))
)

const opcoesCategoria = computed(() => {
  if (!form.departamentoId) {
    return categorias.value.map((item) => ({ label: item.nome, value: item.id }))
  }

  return categorias.value
    .filter((item) => item.departamentoId === null || item.departamentoId === form.departamentoId)
    .map((item) => ({ label: item.nome, value: item.id }))
})

const opcoesPrioridade = computed(() => prioridades.value.map((item) => ({ label: item.nome, value: item.id })))
const opcoesTipoSolicitacao = computed(() =>
  tiposSolicitacao.value.map((item) => ({ label: item.nome, value: item.id }))
)
const opcoesLocalUnidade = computed(() => locaisUnidade.value.map((item) => ({ label: item.nome, value: item.id })))
const opcoesSubcategoria = computed(() => {
  if (!form.categoriaId) {
    return []
  }

  return subcategorias.value
    .filter((item) => item.categoriaChamadoId === form.categoriaId)
    .map((item) => ({ label: item.nome, value: item.id }))
})

function onCategoriaChanged(): void {
  if (!form.categoriaId) {
    form.subcategoriaId = null
    return
  }

  const subcategoriaValida = subcategorias.value.some(
    (item) => item.id === form.subcategoriaId && item.categoriaChamadoId === form.categoriaId
  )

  if (!subcategoriaValida) {
    form.subcategoriaId = null
  }
}

function normalizarExtensoesPorContentType(contentTypes: string[]): string[] {
  const extensoes = new Set<string>()

  for (const contentType of contentTypes) {
    const extensoesDoTipo = EXTENSOES_POR_CONTENT_TYPE[contentType.trim().toLowerCase()] ?? []
    for (const extensao of extensoesDoTipo) {
      extensoes.add(extensao)
    }
  }

  return extensoes.size ? Array.from(extensoes) : EXTENSOES_PADRAO
}

async function carregarContexto(): Promise<void> {
  carregandoContexto.value = true
  erroContexto.value = null

  try {
    const contexto = await portalService.getPortalContexto()
    departamentos.value = contexto.departamentos
    categorias.value = contexto.categorias
    subcategorias.value = contexto.subcategorias
    prioridades.value = contexto.prioridades
    tiposSolicitacao.value = contexto.tiposSolicitacao
    locaisUnidade.value = contexto.locaisUnidade

    const tiposPermitidos = contexto.configuracaoAnexos?.tiposPermitidos ?? []
    extensoesPermitidas.value = normalizarExtensoesPorContentType(tiposPermitidos)
    tamanhoMaximoAnexoBytes.value = contexto.configuracaoAnexos?.tamanhoMaximoBytes ?? null
  } catch (error) {
    erroContexto.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os dados da abertura.'
  } finally {
    carregandoContexto.value = false
  }
}

function aplicarServicoSelecionadoNoFormulario(servico: PortalPrepararChamadoCatalogoServico): void {
  limparRespostasFormulario()
  servicoSelecionado.value = servico
  form.catalogoServicoId = servico.catalogoServicoId
  form.catalogoServicoSlug = servico.slug
  form.departamentoId = servico.departamentoResponsavelId
  form.categoriaId = servico.categoriaId
  form.subcategoriaId = servico.subcategoriaId
  form.prioridadeId = servico.prioridadePadraoId
}

async function carregarServicoSelecionado(): Promise<void> {
  const slugQuery = String(route.query.catalogoServicoSlug ?? '').trim()
  const idQuery = String(route.query.catalogoServicoId ?? '').trim()

  if (!slugQuery && !idQuery) {
    limparRespostasFormulario()
    servicoSelecionado.value = null
    form.catalogoServicoId = null
    form.catalogoServicoSlug = null
    return
  }

  if (!slugQuery) {
    erroContexto.value = 'Nao foi possivel iniciar a abertura do chamado para este servico.'
    return
  }

  carregandoCatalogoSelecionado.value = true

  try {
    const servico = await catalogoServicosPortalService.prepararAberturaChamado(slugQuery)

    if (idQuery && servico.catalogoServicoId !== idQuery) {
      throw new Error('Servico divergente para abertura.')
    }

    aplicarServicoSelecionadoNoFormulario(servico)
  } catch {
    limparRespostasFormulario()
    servicoSelecionado.value = null
    form.catalogoServicoId = null
    form.catalogoServicoSlug = null
    erroContexto.value = 'Nao foi possivel iniciar a abertura do chamado para este servico.'
  } finally {
    carregandoCatalogoSelecionado.value = false
  }
}

function adicionarAnexos(arquivos: File[]): void {
  erroAnexo.value = null
  anexosPendentes.value = [...anexosPendentes.value, ...arquivos]
}

function registrarErroAnexo(message: string): void {
  erroAnexo.value = message
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

function removerAnexo(index: number): void {
  anexosPendentes.value = anexosPendentes.value.filter((_, idx) => idx !== index)
}

function limparRespostasFormulario(): void {
  for (const chave of Object.keys(respostasFormulario)) {
    delete respostasFormulario[chave]
  }
}

function obterCamposFormularioAplicaveis(): PortalFormularioPreparacaoCampo[] {
  const campos = servicoSelecionado.value?.formulario?.versao.campos ?? []
  return campos
    .filter((campo) => campo.ativo !== false && campo.visivel !== false)
    .sort((a, b) => a.ordem - b.ordem || a.rotulo.localeCompare(b.rotulo))
}

function obterRespostaTexto(campoId: string): string | null {
  const valor = respostasFormulario[campoId]
  if (typeof valor !== 'string') {
    return null
  }

  const texto = valor.trim()
  return texto ? texto : null
}

function obterRespostaLista(campoId: string): string[] {
  const valor = respostasFormulario[campoId]
  if (!Array.isArray(valor)) {
    return []
  }

  return valor.map((item) => item.trim()).filter((item) => item.length > 0)
}

function serializarRespostasFormulario(): RespostaFormularioAberturaRequest[] | undefined {
  const formulario = servicoSelecionado.value?.formulario
  if (!formulario) {
    return undefined
  }

  const respostasSerializadas = obterCamposFormularioAplicaveis()
    .map((campo): RespostaFormularioAberturaRequest | null => {
      if (campo.tipo === TipoCampoFormularioServico.SelecaoMultipla) {
        const valores = obterRespostaLista(campo.id)
        if (!valores.length) {
          return null
        }

        return {
          campoFormularioServicoId: campo.id,
          valores,
        }
      }

      if (campo.tipo === TipoCampoFormularioServico.Booleano) {
        const valor = respostasFormulario[campo.id]
        if (typeof valor !== 'boolean') {
          return null
        }

        return {
          campoFormularioServicoId: campo.id,
          valor: valor ? 'true' : 'false',
        }
      }

      const valor = obterRespostaTexto(campo.id)
      if (!valor) {
        return null
      }

      return {
        campoFormularioServicoId: campo.id,
        valor,
      }
    })
    .filter((item): item is RespostaFormularioAberturaRequest => item !== null)

  return respostasSerializadas.length ? respostasSerializadas : undefined
}

async function salvar(): Promise<void> {
  if (salvando.value) {
    return
  }

  erroSalvar.value = null

  const formValido = await formRef.value?.validate()
  if (!formValido) {
    return
  }

  if (!aberturaPorCatalogo.value && (form.naturezaChamado === null || form.impactoChamado === null || form.urgenciaChamado === null)) {
    erroSalvar.value = 'Selecione natureza, impacto e urgencia para abrir o chamado.'
    return
  }

  salvando.value = true

  try {
    const chamado = aberturaPorCatalogo.value && form.catalogoServicoId
      ? await portalService.abrirRequisicaoServicoCatalogo({
          catalogoServicoId: form.catalogoServicoId,
          titulo: form.titulo.trim(),
          descricao: form.descricao.trim() || undefined,
          respostasFormulario: serializarRespostasFormulario(),
        })
      : await portalService.criarChamado({
          titulo: form.titulo.trim(),
          descricao: form.descricao.trim(),
          catalogoServicoId: form.catalogoServicoId ?? undefined,
          catalogoServicoSlug: form.catalogoServicoSlug ?? undefined,
          departamentoId:
            exibirDepartamento.value && !aberturaPorCatalogo.value ? (form.departamentoId ?? undefined) : undefined,
          categoriaId: form.categoriaId ?? undefined,
          subcategoriaId: form.subcategoriaId ?? undefined,
          prioridadeId: form.prioridadeId ?? undefined,
          naturezaChamado: form.naturezaChamado,
          impactoChamado: form.impactoChamado,
          urgenciaChamado: form.urgenciaChamado,
          tipoSolicitacaoId: form.tipoSolicitacaoId ?? undefined,
          localUnidadeId: form.localUnidadeId ?? undefined,
        })

    let anexosComFalha = 0

    for (const arquivo of anexosPendentes.value) {
      try {
        await portalService.anexarArquivo(chamado.id, arquivo)
      } catch {
        anexosComFalha++
      }
    }

    if (anexosComFalha > 0) {
      $q.notify({
        type: 'warning',
        message: 'Chamado aberto com sucesso, mas um ou mais anexos nao foram enviados.',
      })
    } else {
      $q.notify({
        type: 'positive',
        message: 'Chamado aberto com sucesso.',
      })
    }

    await router.replace(`/portal/chamados/${chamado.id}`)
  } catch (error) {
    erroSalvar.value = extrairMensagemErro(error, 'Nao foi possivel abrir o chamado. Verifique os dados e tente novamente.')
  } finally {
    salvando.value = false
  }
}

function cancelar(): void {
  router.push('/portal/chamados')
}

onMounted(async () => {
  await carregarContexto()
  await carregarServicoSelecionado()
})

watch(
  () => [route.query.catalogoServicoSlug, route.query.catalogoServicoId],
  async () => {
    await carregarServicoSelecionado()
  }
)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Portal do solicitante"
      titulo="Abrir chamado"
      subtitulo="Registre sua solicitacao com contexto suficiente para classificacao, priorizacao e atendimento."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn flat color="primary" icon="arrow_back" label="Meus chamados" :disable="salvando" @click="cancelar" />
          <q-btn flat color="primary" icon="inventory_2" label="Catalogo" :disable="salvando" @click="router.push('/portal/catalogo-servicos')" />
        </div>
      </template>
    </PageHeader>

    <ErrorState v-if="erroContexto && !carregandoContexto" :mensagem="erroContexto" @retry="carregarContexto" />

    <LoadingState
      v-else-if="carregandoContexto || carregandoCatalogoSelecionado"
      inline
      mensagem="Carregando contexto de abertura..."
    />

    <q-form v-else ref="formRef" class="column q-gutter-md" @submit.prevent="salvar">
      <q-banner v-if="erroSalvar" rounded class="bg-negative text-white">
        {{ erroSalvar }}
      </q-banner>
      <q-banner v-if="!servicoSelecionado" rounded class="bg-blue-1 text-primary">
        Dica: se preferir, abra chamados pelo Catalogo de Servicos para herdar categoria e prioridade padrao.
      </q-banner>

      <AppSectionCard titulo="Dados da solicitacao" subtitulo="Preencha os campos obrigatorios para abrir o chamado.">
        <div class="column q-gutter-md">
          <q-banner rounded class="bg-grey-1 text-grey-9">
            O backend valida as regras finais de ITSM. Este formulario ajuda na classificacao correta antes do envio.
          </q-banner>

          <q-banner v-if="servicoSelecionado" rounded class="bg-blue-1 text-primary">
            <div class="text-subtitle2">Servico selecionado</div>
            <div class="text-body2"><strong>{{ servicoSelecionado.nome }}</strong></div>
            <div class="text-body2">Departamento: {{ servicoSelecionado.departamentoResponsavelNome || 'Nao informado' }}</div>
            <div class="text-body2">{{ servicoSelecionado.descricao || 'Sem descricao complementar.' }}</div>
            <div v-if="servicoSelecionado.instrucoesSolicitante" class="text-body2 q-mt-sm">
              Instrucoes: {{ servicoSelecionado.instrucoesSolicitante }}
            </div>
            <div class="text-caption q-mt-sm">
              Categoria: {{ servicoSelecionado.categoriaNome || 'Nao informada' }} |
              Subcategoria: {{ servicoSelecionado.subcategoriaNome || 'Nao informada' }} |
              Prioridade: {{ servicoSelecionado.prioridadePadraoNome || 'Nao informada' }} |
              SLA: {{ servicoSelecionado.slaPadraoNome || 'Nao informado' }}
            </div>
          </q-banner>

          <div v-if="!aberturaPorCatalogo" class="sgx-form-group">
            <div class="text-subtitle2 text-weight-semibold">Classificacao ITSM</div>
            <div class="text-caption sgx-muted">Selecione natureza, impacto e urgencia para direcionar o atendimento.</div>

            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-6">
                <q-select
                  v-model="form.naturezaChamado"
                  outlined
                  emit-value
                  map-options
                  label="Natureza do chamado *"
                  :options="opcoesNatureza"
                  :rules="[(v) => !!v || 'Natureza do chamado obrigatoria']"
                />
              </div>

              <div class="col-12 col-md-3">
                <q-select
                  v-model="form.impactoChamado"
                  outlined
                  emit-value
                  map-options
                  label="Impacto *"
                  :options="opcoesImpacto"
                  :rules="[(v) => !!v || 'Impacto obrigatorio']"
                />
              </div>

              <div class="col-12 col-md-3">
                <q-select
                  v-model="form.urgenciaChamado"
                  outlined
                  emit-value
                  map-options
                  label="Urgencia *"
                  :options="opcoesUrgencia"
                  :rules="[(v) => !!v || 'Urgencia obrigatoria']"
                />
              </div>
            </div>

            <q-banner v-if="orientacaoNaturezaSelecionada" rounded class="bg-blue-1 text-primary">
              <div class="text-body2">{{ orientacaoNaturezaSelecionada.descricao }}</div>
              <div class="text-caption q-mt-xs">
                Campos minimos: {{ orientacaoNaturezaSelecionada.obrigatoriedades.join(' | ') }}
              </div>
            </q-banner>

            <q-banner v-else rounded class="bg-grey-1 text-grey-8">
              Selecione a natureza para visualizar orientacoes de preenchimento por tipo de chamado.
            </q-banner>

            <div class="text-caption sgx-muted">
              A prioridade final e calculada no backend com base em impacto e urgencia.
            </div>
          </div>

          <q-separator v-if="!aberturaPorCatalogo" />

          <div class="sgx-form-group">
            <div class="text-subtitle2 text-weight-semibold">Dados do chamado</div>
            <div class="text-caption sgx-muted">Registre titulo objetivo e detalhes suficientes para triagem.</div>

            <q-input
              v-model="form.titulo"
              outlined
              maxlength="180"
              counter
              label="Titulo *"
              :rules="[(v) => !!String(v ?? '').trim() || 'Titulo obrigatorio']"
            />

            <q-input
              v-model="form.descricao"
              outlined
              type="textarea"
              autogrow
              maxlength="4000"
              counter
              label="Descricao *"
              :rules="[(v) => !!String(v ?? '').trim() || 'Descricao obrigatoria']"
            />
          </div>

          <q-separator v-if="servicoSelecionado?.formulario" />

          <div v-if="servicoSelecionado?.formulario" class="sgx-form-group">
            <div class="text-subtitle2 text-weight-semibold">Formulario do servico</div>
            <div class="text-caption sgx-muted">
              Campos adicionais exibidos conforme a configuracao administrativa do servico selecionado.
            </div>

            <FormularioDinamicoCatalogoSection
              v-model="respostasFormulario"
              :formulario="servicoSelecionado.formulario"
            />
          </div>

          <q-separator v-if="!aberturaPorCatalogo" />

          <div v-if="!aberturaPorCatalogo" class="sgx-form-group">
            <div class="text-subtitle2 text-weight-semibold">Classificacao operacional</div>
            <div class="text-caption sgx-muted">Categoria, tipo e contexto organizacional ajudam no roteamento interno.</div>

            <div class="row q-col-gutter-md">
              <div v-if="exibirDepartamento" class="col-12 col-md-4">
                <q-select
                  v-model="form.departamentoId"
                  outlined
                  clearable
                  emit-value
                  map-options
                  label="Departamento"
                  :options="opcoesDepartamento"
                  :disable="aberturaPorCatalogo"
                />
              </div>

              <div :class="exibirDepartamento ? 'col-12 col-md-4' : 'col-12 col-md-6'">
                <q-select
                  v-model="form.categoriaId"
                  outlined
                  emit-value
                  map-options
                  label="Categoria *"
                  :options="opcoesCategoria"
                  :rules="[(v) => aberturaPorCatalogo || !!v || 'Categoria obrigatoria']"
                  :disable="aberturaPorCatalogo"
                  @update:model-value="onCategoriaChanged"
                />
              </div>

              <div :class="exibirDepartamento ? 'col-12 col-md-4' : 'col-12 col-md-6'">
                <q-select
                  v-model="form.subcategoriaId"
                  outlined
                  emit-value
                  map-options
                  clearable
                  label="Subcategoria"
                  :options="opcoesSubcategoria"
                  :disable="aberturaPorCatalogo || !form.categoriaId"
                />
              </div>

              <div :class="exibirDepartamento ? 'col-12 col-md-4' : 'col-12 col-md-6'">
                <q-select
                  v-model="form.prioridadeId"
                  outlined
                  emit-value
                  map-options
                  label="Prioridade *"
                  :options="opcoesPrioridade"
                  :rules="[(v) => aberturaPorCatalogo || !!v || 'Prioridade obrigatoria']"
                  :disable="aberturaPorCatalogo"
                />
              </div>

              <div class="col-12 col-md-6">
                <q-select
                  v-model="form.tipoSolicitacaoId"
                  outlined
                  emit-value
                  map-options
                  clearable
                  label="Tipo de solicitacao"
                  :options="opcoesTipoSolicitacao"
                />
              </div>

              <div class="col-12 col-md-6">
                <q-select
                  v-model="form.localUnidadeId"
                  outlined
                  emit-value
                  map-options
                  clearable
                  label="Local / Unidade"
                  :options="opcoesLocalUnidade"
                />
              </div>
            </div>

            <q-banner rounded class="bg-grey-1 text-grey-8">
              Para Requisicao, garanta ao menos uma classificacao (Categoria, Tipo de solicitacao ou Catalogo).
            </q-banner>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Anexos" subtitulo="Opcional: adicione arquivos antes de abrir o chamado.">
        <div class="column q-gutter-sm">
          <q-banner v-if="erroAnexo" dense rounded class="bg-amber-2 text-dark">
            {{ erroAnexo }}
          </q-banner>

          <UploadAnexo
            :loading="salvando"
            :extensoes-permitidas="extensoesPermitidas"
            :tamanho-maximo-bytes="tamanhoMaximoAnexoBytes"
            @upload="adicionarAnexos"
            @invalid="registrarErroAnexo"
          />

          <q-banner v-if="!anexosPendentes.length" rounded class="bg-blue-1 text-primary">
            Nenhum anexo selecionado.
          </q-banner>

          <q-list v-else bordered separator>
            <q-item v-for="(anexo, index) in anexosPendentes" :key="`${anexo.name}-${index}`">
              <q-item-section>
                <q-item-label>{{ anexo.name }}</q-item-label>
                <q-item-label caption>{{ (anexo.size / 1024).toFixed(1) }} KB</q-item-label>
              </q-item-section>

              <q-item-section side>
                <q-btn flat dense round icon="delete" color="negative" aria-label="Remover anexo" :disable="salvando" @click="removerAnexo(index)" />
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </AppSectionCard>

      <div class="sgx-form-actions row justify-end q-gutter-sm">
        <q-btn flat color="primary" label="Cancelar" :disable="salvando" @click="cancelar" />
        <q-btn type="submit" color="secondary" icon="send" label="Abrir chamado" :loading="salvando" :disable="salvando" />
      </div>
    </q-form>
  </q-page>
</template>

<style scoped>
.sgx-form-group {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.sgx-form-actions {
  padding: 12px 14px;
  border-radius: var(--sgx-radius-md);
  border: 1px solid var(--sgx-border);
  background: var(--sgx-card-bg);
}
</style>
