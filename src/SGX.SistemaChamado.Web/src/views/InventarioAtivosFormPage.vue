<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../components/admin/cadastros/FormCadastro.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { usuariosAdminService } from '../services/usuariosAdminService'
import { useAuthStore } from '../stores/authStore'
import type { AtendenteResumo } from '../types/admin'
import {
  CriticidadeAtivo,
  StatusOperacionalAtivo,
  StatusPatrimonialAtivo,
  type AtualizarInventarioAtivoRequest,
  type CriarInventarioAtivoRequest,
  type InventarioAtivoDetalhe,
  type TipoAtivoInventario,
} from '../types/inventarioAtivos'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const carregamentoConcluido = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const tiposAtivo = ref<TipoAtivoInventario[]>([])
const departamentos = ref<{ id: string; nome: string }[]>([])
const locaisUnidade = ref<{ id: string; nome: string }[]>([])
const usuariosResponsaveis = ref<AtendenteResumo[]>([])
const ativoAtual = ref<InventarioAtivoDetalhe | null>(null)

const form = reactive({
  codigo: '',
  nome: '',
  descricao: '',
  tipoAtivoInventarioId: '',
  numeroPatrimonio: '',
  numeroSerie: '',
  fabricante: '',
  modelo: '',
  departamentoId: '',
  localUnidadeId: '',
  usuarioResponsavelId: '',
  statusOperacional: StatusOperacionalAtivo.Operacional,
  statusPatrimonial: StatusPatrimonialAtivo.EmUso,
  criticidade: CriticidadeAtivo.Media,
  dataAquisicao: '',
  dataFimGarantia: '',
  valorAquisicao: null as number | null,
  fornecedor: '',
  observacoes: '',
  ativo: true,
})

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const tituloTela = computed(() => (isNovo.value ? 'Inventario/Ativos - Novo ativo' : 'Inventario/Ativos - Editar ativo'))
const subtituloTela = computed(() =>
  isNovo.value
    ? 'Cadastro de novo ativo com classificacao operacional, patrimonial e de responsabilidade.'
    : 'Atualize dados cadastrais e operacionais do ativo mantendo rastreabilidade completa.'
)

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.inventarioAtivosVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.inventarioAtivosGerenciar))
const somenteLeitura = computed(() => !podeGerenciar.value || (!isNovo.value && !form.ativo))

const opcoesStatusOperacional = [
  { label: 'Operacional', value: StatusOperacionalAtivo.Operacional },
  { label: 'Em manutencao', value: StatusOperacionalAtivo.EmManutencao },
  { label: 'Com defeito', value: StatusOperacionalAtivo.ComDefeito },
  { label: 'Reservado', value: StatusOperacionalAtivo.Reservado },
  { label: 'Baixado', value: StatusOperacionalAtivo.Baixado },
]

const opcoesStatusPatrimonial = [
  { label: 'Em uso', value: StatusPatrimonialAtivo.EmUso },
  { label: 'Em estoque', value: StatusPatrimonialAtivo.EmEstoque },
  { label: 'Emprestado', value: StatusPatrimonialAtivo.Emprestado },
  { label: 'Em transferencia', value: StatusPatrimonialAtivo.EmTransferencia },
  { label: 'Descartado', value: StatusPatrimonialAtivo.Descartado },
  { label: 'Extraviado', value: StatusPatrimonialAtivo.Extraviado },
]

const opcoesCriticidade = [
  { label: 'Baixa', value: CriticidadeAtivo.Baixa },
  { label: 'Media', value: CriticidadeAtivo.Media },
  { label: 'Alta', value: CriticidadeAtivo.Alta },
  { label: 'Critica', value: CriticidadeAtivo.Critica },
]

function regraObrigatoria(valor: unknown): true | string {
  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatorio.'
}

function regraValorAquisicao(valor: unknown): true | string {
  if (valor === null || valor === undefined || String(valor).trim() === '') {
    return true
  }

  const numero = Number(valor)
  if (!Number.isFinite(numero)) {
    return 'Informe um valor valido.'
  }

  if (numero < 0) {
    return 'Valor de aquisicao nao pode ser negativo.'
  }

  return true
}

function regraDataGarantia(_: unknown): true | string {
  if (!form.dataAquisicao || !form.dataFimGarantia) {
    return true
  }

  if (form.dataFimGarantia < form.dataAquisicao) {
    return 'Data fim de garantia nao pode ser anterior a data de aquisicao.'
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

function aplicarDetalheNoFormulario(ativo: InventarioAtivoDetalhe): void {
  ativoAtual.value = ativo
  form.codigo = ativo.codigo
  form.nome = ativo.nome
  form.descricao = ativo.descricao || ''
  form.tipoAtivoInventarioId = ativo.tipoAtivoInventarioId
  form.numeroPatrimonio = ativo.numeroPatrimonio || ''
  form.numeroSerie = ativo.numeroSerie || ''
  form.fabricante = ativo.fabricante || ''
  form.modelo = ativo.modelo || ''
  form.departamentoId = ativo.departamentoId || ''
  form.localUnidadeId = ativo.localUnidadeId || ''
  form.usuarioResponsavelId = ativo.usuarioResponsavelId || ''
  form.statusOperacional = ativo.statusOperacional
  form.statusPatrimonial = ativo.statusPatrimonial
  form.criticidade = ativo.criticidade
  form.dataAquisicao = ativo.dataAquisicao ? ativo.dataAquisicao.slice(0, 10) : ''
  form.dataFimGarantia = ativo.dataFimGarantia ? ativo.dataFimGarantia.slice(0, 10) : ''
  form.valorAquisicao = ativo.valorAquisicao
  form.fornecedor = ativo.fornecedor || ''
  form.observacoes = ativo.observacoes || ''
  form.ativo = ativo.ativo
}

function limparFormulario(): void {
  ativoAtual.value = null
  form.codigo = ''
  form.nome = ''
  form.descricao = ''
  form.tipoAtivoInventarioId = ''
  form.numeroPatrimonio = ''
  form.numeroSerie = ''
  form.fabricante = ''
  form.modelo = ''
  form.departamentoId = ''
  form.localUnidadeId = ''
  form.usuarioResponsavelId = ''
  form.statusOperacional = StatusOperacionalAtivo.Operacional
  form.statusPatrimonial = StatusPatrimonialAtivo.EmUso
  form.criticidade = CriticidadeAtivo.Media
  form.dataAquisicao = ''
  form.dataFimGarantia = ''
  form.valorAquisicao = null
  form.fornecedor = ''
  form.observacoes = ''
  form.ativo = true
}

function normalizarTexto(valor: string): string | null {
  const trimmed = valor.trim()
  return trimmed.length ? trimmed : null
}

function montarPayloadCriacao(): CriarInventarioAtivoRequest {
  return {
    codigo: form.codigo.trim(),
    nome: form.nome.trim(),
    descricao: normalizarTexto(form.descricao),
    tipoAtivoInventarioId: form.tipoAtivoInventarioId,
    numeroPatrimonio: normalizarTexto(form.numeroPatrimonio),
    numeroSerie: normalizarTexto(form.numeroSerie),
    fabricante: normalizarTexto(form.fabricante),
    modelo: normalizarTexto(form.modelo),
    departamentoId: form.departamentoId || null,
    localUnidadeId: form.localUnidadeId || null,
    usuarioResponsavelId: form.usuarioResponsavelId || null,
    statusOperacional: form.statusOperacional,
    statusPatrimonial: form.statusPatrimonial,
    criticidade: form.criticidade,
    dataAquisicao: form.dataAquisicao || null,
    dataFimGarantia: form.dataFimGarantia || null,
    valorAquisicao: form.valorAquisicao,
    fornecedor: normalizarTexto(form.fornecedor),
    observacoes: normalizarTexto(form.observacoes),
  }
}

function montarPayloadAtualizacao(): AtualizarInventarioAtivoRequest {
  return {
    codigo: form.codigo.trim(),
    nome: form.nome.trim(),
    descricao: normalizarTexto(form.descricao),
    tipoAtivoInventarioId: form.tipoAtivoInventarioId,
    numeroPatrimonio: normalizarTexto(form.numeroPatrimonio),
    numeroSerie: normalizarTexto(form.numeroSerie),
    fabricante: normalizarTexto(form.fabricante),
    modelo: normalizarTexto(form.modelo),
    departamentoId: form.departamentoId || null,
    localUnidadeId: form.localUnidadeId || null,
    usuarioResponsavelId: form.usuarioResponsavelId || null,
    statusOperacional: form.statusOperacional,
    statusPatrimonial: form.statusPatrimonial,
    criticidade: form.criticidade,
    dataAquisicao: form.dataAquisicao || null,
    dataFimGarantia: form.dataFimGarantia || null,
    valorAquisicao: form.valorAquisicao,
    fornecedor: normalizarTexto(form.fornecedor),
    observacoes: normalizarTexto(form.observacoes),
  }
}

async function carregarReferencias(): Promise<void> {
  const [tiposResponse, contextoResponse, usuariosResponse] = await Promise.all([
    inventarioAtivosAdminService.listarTipos(),
    adminService.obterAdminContexto(),
    usuariosAdminService.listar({ ativo: true, tamanhoPagina: 200, ordenarPor: 'nome', direcaoOrdenacao: 'asc' }),
  ])

  tiposAtivo.value = tiposResponse.filter((item) => item.ativo)
  departamentos.value = contextoResponse.departamentos
  locaisUnidade.value = contextoResponse.locaisUnidade
  usuariosResponsaveis.value = usuariosResponse.items.map((item) => ({
    id: item.id,
    nome: item.nome,
    email: item.email,
    perfis: item.perfis.map((perfil) => perfil.nome),
  }))
}

async function carregarDetalhe(): Promise<void> {
  if (isNovo.value) {
    limparFormulario()
    return
  }

  const detalhe = await inventarioAtivosAdminService.obterPorId(idParam.value)
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
    await carregarReferencias()
    await carregarDetalhe()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados do ativo.')
  } finally {
    loading.value = false
    carregamentoConcluido.value = true
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
      const criado = await inventarioAtivosAdminService.criar(montarPayloadCriacao())
      $q.notify({ type: 'positive', message: 'Ativo criado com sucesso.' })
      await router.replace(`/admin/infraestrutura/inventario-ativos/${criado.id}`)
      return
    }

    const atualizado = await inventarioAtivosAdminService.atualizar(idParam.value, montarPayloadAtualizacao())
    aplicarDetalheNoFormulario(atualizado)
    sucesso.value = 'Ativo salvo com sucesso.'
    $q.notify({ type: 'positive', message: 'Ativo atualizado com sucesso.' })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar o ativo.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    loading.value = false
  }
}

function voltar(): void {
  if (isNovo.value) {
    router.push('/admin/infraestrutura/inventario-ativos')
    return
  }

  router.push(`/admin/infraestrutura/inventario-ativos/${idParam.value}`)
}

function voltarParaLista(): void {
  router.push('/admin/infraestrutura/inventario-ativos')
}

watch(
  () => route.params.id,
  async () => {
    carregamentoConcluido.value = false
    await carregarTela()
  }
)

onMounted(async () => {
  await carregarTela()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader :titulo="tituloTela" :subtitulo="subtituloTela" contexto="Infraestrutura">
      <template #actions>
        <q-btn flat color="primary" icon="arrow_back" label="Voltar para lista" @click="voltarParaLista" />
      </template>
    </PageHeader>

    <q-banner v-if="isNovo && !podeGerenciar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para criar ativos.
    </q-banner>

    <q-banner v-else-if="!isNovo && !podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar este ativo.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading && !carregamentoConcluido" mensagem="Carregando dados do ativo..." />

      <ErrorState
        v-else-if="erro && !carregamentoConcluido"
        titulo="Nao foi possivel carregar o ativo"
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

        <q-banner v-if="!isNovo && !form.ativo" rounded class="bg-amber-2 text-dark">
          Este ativo esta inativo. Reative no detalhe antes de editar.
        </q-banner>

        <FormCadastro
          :titulo="isNovo ? 'Novo ativo' : 'Editar ativo'"
          subtitulo="Preencha os dados do ativo para manter a rastreabilidade patrimonial e operacional."
          :loading="loading"
          :somente-leitura="somenteLeitura"
          botao-salvar-label="Salvar"
          @salvar="salvar"
          @cancelar="voltar"
        >
          <div class="column q-gutter-md">
            <AppSectionCard sem-separador titulo="Resumo operacional" subtitulo="Situcao atual para tomada de decisao rapida.">
              <div class="row q-col-gutter-sm items-center">
                <div class="col-auto">
                  <StatusBadge :texto="form.ativo ? 'Ativo' : 'Inativo'" />
                </div>
                <div class="col-auto">
                  <StatusBadge :texto="opcoesStatusOperacional.find((item) => item.value === form.statusOperacional)?.label || '-'" />
                </div>
                <div class="col-auto">
                  <StatusBadge :texto="opcoesStatusPatrimonial.find((item) => item.value === form.statusPatrimonial)?.label || '-'" />
                </div>
                <div class="col-auto">
                  <StatusBadge :texto="opcoesCriticidade.find((item) => item.value === form.criticidade)?.label || '-'" />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard sem-separador titulo="Informacoes gerais" subtitulo="Identificacao principal do ativo e classificacao de tipo.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-input v-model="form.codigo" outlined dense label="Codigo" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
                </div>

                <div class="col-12 col-md-8">
                  <q-input v-model="form.nome" outlined dense label="Nome" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.tipoAtivoInventarioId"
                    outlined
                    dense
                    emit-value
                    map-options
                    label="Tipo de ativo"
                    :disable="somenteLeitura"
                    :options="tiposAtivo.map((item) => ({ label: item.nome, value: item.id }))"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-input v-model="form.numeroPatrimonio" outlined dense label="Numero de patrimonio" :readonly="somenteLeitura" />
                </div>

                <div class="col-12 col-md-4">
                  <q-input v-model="form.numeroSerie" outlined dense label="Numero de serie" :readonly="somenteLeitura" />
                </div>

                <div class="col-12 col-md-6">
                  <q-input v-model="form.fabricante" outlined dense label="Fabricante" :readonly="somenteLeitura" />
                </div>

                <div class="col-12 col-md-6">
                  <q-input v-model="form.modelo" outlined dense label="Modelo" :readonly="somenteLeitura" />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard sem-separador titulo="Localizacao e responsavel" subtitulo="Contexto de lotacao e ownership atual do ativo.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.departamentoId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    label="Departamento"
                    :disable="somenteLeitura"
                    :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.localUnidadeId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    label="Local / Unidade"
                    :disable="somenteLeitura"
                    :options="locaisUnidade.map((item) => ({ label: item.nome, value: item.id }))"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.usuarioResponsavelId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    label="Usuario responsavel"
                    :disable="somenteLeitura"
                    :options="usuariosResponsaveis.map((item) => ({ label: item.nome, value: item.id }))"
                  />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard sem-separador titulo="Classificacao operacional e criticidade" subtitulo="Estado operacional e patrimonial usado nos controles de risco e suporte.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.statusOperacional"
                    outlined
                    dense
                    emit-value
                    map-options
                    label="Status operacional"
                    :disable="somenteLeitura"
                    :options="opcoesStatusOperacional"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.statusPatrimonial"
                    outlined
                    dense
                    emit-value
                    map-options
                    label="Status patrimonial"
                    :disable="somenteLeitura"
                    :options="opcoesStatusPatrimonial"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.criticidade"
                    outlined
                    dense
                    emit-value
                    map-options
                    label="Criticidade"
                    :disable="somenteLeitura"
                    :options="opcoesCriticidade"
                    :rules="[regraObrigatoria]"
                  />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard sem-separador titulo="Ciclo de vida, aquisicao e observacoes" subtitulo="Datas, custo, fornecedor e notas adicionais do ativo.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-3">
                  <q-input
                    v-model="form.dataAquisicao"
                    outlined
                    dense
                    type="date"
                    label="Data aquisicao"
                    :readonly="somenteLeitura"
                    :rules="[regraDataGarantia]"
                  />
                </div>

                <div class="col-12 col-md-3">
                  <q-input
                    v-model="form.dataFimGarantia"
                    outlined
                    dense
                    type="date"
                    label="Data fim garantia"
                    :readonly="somenteLeitura"
                    :rules="[regraDataGarantia]"
                  />
                </div>

                <div class="col-12 col-md-3">
                  <q-input
                    v-model.number="form.valorAquisicao"
                    outlined
                    dense
                    type="number"
                    min="0"
                    step="0.01"
                    label="Valor aquisicao"
                    :readonly="somenteLeitura"
                    :rules="[regraValorAquisicao]"
                  />
                </div>

                <div class="col-12 col-md-3">
                  <q-input v-model="form.fornecedor" outlined dense label="Fornecedor" :readonly="somenteLeitura" />
                </div>

                <div class="col-12">
                  <q-input v-model="form.descricao" outlined dense type="textarea" autogrow label="Descricao" :readonly="somenteLeitura" />
                </div>

                <div class="col-12">
                  <q-input v-model="form.observacoes" outlined dense type="textarea" autogrow label="Observacoes" :readonly="somenteLeitura" />
                </div>
              </div>
            </AppSectionCard>
          </div>
        </FormCadastro>
      </template>
    </template>
  </q-page>
</template>
