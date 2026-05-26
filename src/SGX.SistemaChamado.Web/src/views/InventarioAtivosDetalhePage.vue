<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import PaginacaoTabela from '../components/admin/cadastros/PaginacaoTabela.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { inventarioAtivosAdminService } from '../services/inventarioAtivosAdminService'
import { usuariosAdminService } from '../services/usuariosAdminService'
import { useAuthStore } from '../stores/authStore'
import type { AtendenteResumo } from '../types/admin'
import {
  StatusOperacionalAtivo,
  StatusPatrimonialAtivo,
  type ChamadoRelacionadoInventarioAtivo,
  type HistoricoInventarioAtivo,
  type InventarioAtivoDetalhe,
} from '../types/inventarioAtivos'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const ativoId = computed(() => String(route.params.id ?? ''))

const loading = ref(false)
const erro = ref<string | null>(null)
const detalhe = ref<InventarioAtivoDetalhe | null>(null)

const loadingHistorico = ref(false)
const historico = ref<HistoricoInventarioAtivo[]>([])
const totalHistorico = ref(0)
const paginaHistorico = ref(1)
const tamanhoPaginaHistorico = ref(10)

const loadingChamados = ref(false)
const chamados = ref<ChamadoRelacionadoInventarioAtivo[]>([])
const totalChamados = ref(0)
const paginaChamados = ref(1)
const tamanhoPaginaChamados = ref(10)

const showConfirmarAcao = ref(false)
const executandoAcao = ref(false)
const tipoAcao = ref<'inativar' | 'reativar' | null>(null)

const showMovimentacao = ref(false)
const salvandoMovimentacao = ref(false)
const departamentos = ref<{ id: string; nome: string }[]>([])
const locaisUnidade = ref<{ id: string; nome: string }[]>([])
const usuariosResponsaveis = ref<AtendenteResumo[]>([])

const formMovimentacao = reactive({
  departamentoId: '',
  localUnidadeId: '',
  usuarioResponsavelId: '',
  statusOperacional: null as StatusOperacionalAtivo | null,
  statusPatrimonial: null as StatusPatrimonialAtivo | null,
  observacao: '',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.inventarioAtivosVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.inventarioAtivosGerenciar))
const podeInativar = computed(() => possuiPermissao(permissoes.inventarioAtivosInativar))
const podeMovimentar = computed(() => possuiPermissao(permissoes.inventarioAtivosMovimentar))

const tituloConfirmacao = computed(() =>
  tipoAcao.value === 'inativar' ? 'Confirmar inativacao' : 'Confirmar reativacao'
)

const mensagemConfirmacao = computed(() => {
  if (!detalhe.value) {
    return ''
  }

  if (tipoAcao.value === 'inativar') {
    return `Deseja inativar o ativo "${detalhe.value.nome}"?`
  }

  return `Deseja reativar o ativo "${detalhe.value.nome}"?`
})

const labelConfirmacao = computed(() => (tipoAcao.value === 'inativar' ? 'Inativar' : 'Reativar'))
const corConfirmacao = computed(() => (tipoAcao.value === 'inativar' ? 'negative' : 'primary'))

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

const detalheSemVinculos = computed(() => {
  if (!detalhe.value) {
    return false
  }

  return !detalhe.value.descricao && !detalhe.value.observacoes
})

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

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function formatarMoeda(valor: number | null): string {
  if (valor === null || valor === undefined) {
    return '-'
  }

  return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor)
}

function navegarParaEditar(): void {
  router.push(`/admin/infraestrutura/inventario-ativos/${ativoId.value}/editar`)
}

function abrirConfirmacao(acao: 'inativar' | 'reativar'): void {
  tipoAcao.value = acao
  showConfirmarAcao.value = true
}

function montarDescricaoMudanca(origem: string | null, destino: string | null): string {
  return `${origem || '-'} -> ${destino || '-'}`
}

function formatarStatusOperacional(valor: StatusOperacionalAtivo | null): string {
  if (valor === null) {
    return '-'
  }

  return opcoesStatusOperacional.find((item) => item.value === valor)?.label ?? String(valor)
}

function formatarStatusPatrimonial(valor: StatusPatrimonialAtivo | null): string {
  if (valor === null) {
    return '-'
  }

  return opcoesStatusPatrimonial.find((item) => item.value === valor)?.label ?? String(valor)
}

async function carregarDetalhe(): Promise<void> {
  detalhe.value = await inventarioAtivosAdminService.obterPorId(ativoId.value)
}

async function carregarHistorico(): Promise<void> {
  loadingHistorico.value = true

  try {
    const response = await inventarioAtivosAdminService.listarHistorico(ativoId.value, {
      pagina: paginaHistorico.value,
      tamanhoPagina: tamanhoPaginaHistorico.value,
    })

    historico.value = response.items
    totalHistorico.value = response.total
  } finally {
    loadingHistorico.value = false
  }
}

async function carregarChamados(): Promise<void> {
  loadingChamados.value = true

  try {
    const response = await inventarioAtivosAdminService.listarChamados(ativoId.value, {
      pagina: paginaChamados.value,
      tamanhoPagina: tamanhoPaginaChamados.value,
    })

    chamados.value = response.items
    totalChamados.value = response.total
  } finally {
    loadingChamados.value = false
  }
}

async function carregarReferenciasMovimentacao(): Promise<void> {
  const [contextoResponse, usuariosResponse] = await Promise.all([
    adminService.obterAdminContexto(),
    usuariosAdminService.listar({ ativo: true, tamanhoPagina: 200, ordenarPor: 'nome', direcaoOrdenacao: 'asc' }),
  ])

  departamentos.value = contextoResponse.departamentos
  locaisUnidade.value = contextoResponse.locaisUnidade
  usuariosResponsaveis.value = usuariosResponse.items.map((item) => ({
    id: item.id,
    nome: item.nome,
    email: item.email,
    perfis: item.perfis.map((perfil) => perfil.nome),
  }))
}

function preencherMovimentacaoComValoresAtuais(): void {
  if (!detalhe.value) {
    return
  }

  formMovimentacao.departamentoId = detalhe.value.departamentoId || ''
  formMovimentacao.localUnidadeId = detalhe.value.localUnidadeId || ''
  formMovimentacao.usuarioResponsavelId = detalhe.value.usuarioResponsavelId || ''
  formMovimentacao.statusOperacional = detalhe.value.statusOperacional
  formMovimentacao.statusPatrimonial = detalhe.value.statusPatrimonial
  formMovimentacao.observacao = ''
}

function existeMudancaMovimentacao(): boolean {
  if (!detalhe.value) {
    return false
  }

  return (
    (formMovimentacao.departamentoId || null) !== detalhe.value.departamentoId ||
    (formMovimentacao.localUnidadeId || null) !== detalhe.value.localUnidadeId ||
    (formMovimentacao.usuarioResponsavelId || null) !== detalhe.value.usuarioResponsavelId ||
    formMovimentacao.statusOperacional !== detalhe.value.statusOperacional ||
    formMovimentacao.statusPatrimonial !== detalhe.value.statusPatrimonial
  )
}

async function carregarTela(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    await Promise.all([carregarDetalhe(), carregarHistorico(), carregarChamados(), carregarReferenciasMovimentacao()])
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar o detalhe do ativo.')
  } finally {
    loading.value = false
  }
}

async function executarAcao(): Promise<void> {
  if (!detalhe.value || !tipoAcao.value) {
    return
  }

  executandoAcao.value = true

  try {
    if (tipoAcao.value === 'inativar') {
      const response = await inventarioAtivosAdminService.inativar(detalhe.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Ativo inativado com sucesso.' })
    } else {
      const response = await inventarioAtivosAdminService.reativar(detalhe.value.id)
      $q.notify({ type: 'positive', message: response.mensagem || 'Ativo reativado com sucesso.' })
    }

    showConfirmarAcao.value = false
    await Promise.all([carregarDetalhe(), carregarHistorico()])
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel executar a acao selecionada.')
    erro.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    executandoAcao.value = false
  }
}

function abrirMovimentacao(): void {
  if (!detalhe.value?.ativo) {
    $q.notify({ type: 'warning', message: 'Ativo inativo nao pode ser movimentado.' })
    return
  }

  preencherMovimentacaoComValoresAtuais()
  showMovimentacao.value = true
}

async function salvarMovimentacao(): Promise<void> {
  if (!detalhe.value) {
    return
  }

  if (!existeMudancaMovimentacao()) {
    $q.notify({ type: 'warning', message: 'Informe ao menos uma alteracao para movimentar o ativo.' })
    return
  }

  salvandoMovimentacao.value = true

  try {
    await inventarioAtivosAdminService.movimentar(detalhe.value.id, {
      departamentoId: formMovimentacao.departamentoId || null,
      localUnidadeId: formMovimentacao.localUnidadeId || null,
      usuarioResponsavelId: formMovimentacao.usuarioResponsavelId || null,
      statusOperacional: formMovimentacao.statusOperacional,
      statusPatrimonial: formMovimentacao.statusPatrimonial,
      observacao: formMovimentacao.observacao.trim() || null,
    })

    $q.notify({ type: 'positive', message: 'Movimentacao registrada com sucesso.' })
    showMovimentacao.value = false
    paginaHistorico.value = 1
    await Promise.all([carregarDetalhe(), carregarHistorico()])
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel registrar a movimentacao.')
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    salvandoMovimentacao.value = false
  }
}

async function atualizarPaginaHistorico(value: number): Promise<void> {
  paginaHistorico.value = value
  await carregarHistorico()
}

async function atualizarTamanhoPaginaHistorico(value: number): Promise<void> {
  tamanhoPaginaHistorico.value = value
  paginaHistorico.value = 1
  await carregarHistorico()
}

async function atualizarPaginaChamados(value: number): Promise<void> {
  paginaChamados.value = value
  await carregarChamados()
}

async function atualizarTamanhoPaginaChamados(value: number): Promise<void> {
  tamanhoPaginaChamados.value = value
  paginaChamados.value = 1
  await carregarChamados()
}

onMounted(async () => {
  await carregarTela()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.nome}` : 'Inventario/Ativos - Detalhe do ativo'"
      subtitulo="Acompanhe dados gerais, historico de movimentacoes e chamados relacionados."
      contexto="Infraestrutura"
    >
      <template #actions>
        <div class="row q-gutter-xs">
          <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="router.push('/admin/infraestrutura/inventario-ativos')" />

          <q-btn v-if="podeGerenciar && detalhe?.ativo" flat color="primary" icon="edit" label="Editar" @click="navegarParaEditar" />

          <q-btn
            v-if="podeMovimentar && detalhe?.ativo"
            flat
            color="secondary"
            icon="swap_horiz"
            label="Movimentar"
            @click="abrirMovimentacao"
          />

          <q-btn
            v-if="podeInativar && detalhe?.ativo"
            flat
            color="negative"
            icon="block"
            label="Inativar"
            @click="abrirConfirmacao('inativar')"
          />

          <q-btn
            v-if="podeInativar && detalhe && !detalhe.ativo"
            flat
            color="primary"
            icon="restart_alt"
            label="Reativar"
            @click="abrirConfirmacao('reativar')"
          />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar ativos de inventario.
    </q-banner>

    <ErrorState v-else-if="erro && !loading" :mensagem="erro" @retry="carregarTela" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando detalhe do ativo..." />

    <template v-else-if="detalhe">
      <div class="detalhe-ativo__kpis">
        <MetricCard titulo="Status do ativo" :valor="detalhe.ativo ? 'Ativo' : 'Inativo'" icon="inventory_2" :tone="detalhe.ativo ? 'positive' : 'warning'" />
        <MetricCard titulo="Criticidade" :valor="detalhe.criticidadeDescricao" icon="priority_high" tone="negative" />
        <MetricCard titulo="Eventos de historico" :valor="totalHistorico" icon="history" tone="info" />
        <MetricCard titulo="Chamados vinculados" :valor="totalChamados" icon="support_agent" tone="primary" />
      </div>

      <AppSectionCard titulo="Dados gerais" subtitulo="Informacoes cadastrais e operacionais do ativo.">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-lg-6">
            <AppSectionCard sem-separador titulo="Identificacao">
              <q-list separator>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Codigo</q-item-label>
                    <q-item-label>{{ detalhe.codigo }}</q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Tipo</q-item-label>
                    <q-item-label>{{ detalhe.tipoAtivoInventarioNome }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Nome</q-item-label>
                    <q-item-label>{{ detalhe.nome }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Patrimonio</q-item-label>
                    <q-item-label>{{ detalhe.numeroPatrimonio || '-' }}</q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Numero de serie</q-item-label>
                    <q-item-label>{{ detalhe.numeroSerie || '-' }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Fabricante</q-item-label>
                    <q-item-label>{{ detalhe.fabricante || '-' }}</q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Modelo</q-item-label>
                    <q-item-label>{{ detalhe.modelo || '-' }}</q-item-label>
                  </q-item-section>
                </q-item>
              </q-list>
            </AppSectionCard>
          </div>

          <div class="col-12 col-lg-6">
            <AppSectionCard sem-separador titulo="Classificacao e status">
              <q-list separator>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Situacao cadastral</q-item-label>
                    <q-item-label>
                      <StatusBadge :texto="detalhe.ativo ? 'Ativo' : 'Inativo'" />
                    </q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Criticidade</q-item-label>
                    <q-item-label>
                      <StatusBadge :texto="detalhe.criticidadeDescricao" />
                    </q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Status operacional</q-item-label>
                    <q-item-label>
                      <StatusBadge :texto="detalhe.statusOperacionalDescricao" />
                    </q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Status patrimonial</q-item-label>
                    <q-item-label>
                      <StatusBadge :texto="detalhe.statusPatrimonialDescricao" />
                    </q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Departamento</q-item-label>
                    <q-item-label>{{ detalhe.departamentoNome || '-' }}</q-item-label>
                  </q-item-section>
                  <q-item-section>
                    <q-item-label caption>Local / Unidade</q-item-label>
                    <q-item-label>{{ detalhe.localUnidadeNome || '-' }}</q-item-label>
                  </q-item-section>
                </q-item>
                <q-item>
                  <q-item-section>
                    <q-item-label caption>Responsavel</q-item-label>
                    <q-item-label>{{ detalhe.usuarioResponsavelNome || '-' }}</q-item-label>
                  </q-item-section>
                </q-item>
              </q-list>
            </AppSectionCard>
          </div>

          <div class="col-12">
            <AppSectionCard sem-separador titulo="Ciclo de vida e aquisicao">
              <div class="row q-col-gutter-sm">
                <div class="col-12 col-md-3">
                  <div class="text-caption sgx-muted">Aquisicao</div>
                  <div class="text-body2 text-weight-medium">{{ formatarData(detalhe.dataAquisicao) }}</div>
                </div>
                <div class="col-12 col-md-3">
                  <div class="text-caption sgx-muted">Fim de garantia</div>
                  <div class="text-body2 text-weight-medium">{{ formatarData(detalhe.dataFimGarantia) }}</div>
                </div>
                <div class="col-12 col-md-3">
                  <div class="text-caption sgx-muted">Valor de aquisicao</div>
                  <div class="text-body2 text-weight-medium">{{ formatarMoeda(detalhe.valorAquisicao) }}</div>
                </div>
                <div class="col-12 col-md-3">
                  <div class="text-caption sgx-muted">Fornecedor</div>
                  <div class="text-body2 text-weight-medium">{{ detalhe.fornecedor || '-' }}</div>
                </div>
              </div>
            </AppSectionCard>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Descricao e observacoes" subtitulo="Notas operacionais e contexto complementar do ativo.">
        <EmptyState
          v-if="detalheSemVinculos"
          titulo="Sem descricoes adicionais"
          mensagem="Este ativo nao possui descricao ou observacoes cadastradas."
          icon="description"
        />
        <div v-else class="row q-col-gutter-md">
          <div class="col-12 col-lg-6">
            <q-card flat bordered class="sgx-card detalhe-ativo__bloco-texto">
              <q-card-section>
                <div class="text-subtitle2 text-weight-bold q-mb-sm">Descricao</div>
                <div class="text-body2">{{ detalhe.descricao || '-' }}</div>
              </q-card-section>
            </q-card>
          </div>
          <div class="col-12 col-lg-6">
            <q-card flat bordered class="sgx-card detalhe-ativo__bloco-texto">
              <q-card-section>
                <div class="text-subtitle2 text-weight-bold q-mb-sm">Observacoes</div>
                <div class="text-body2">{{ detalhe.observacoes || '-' }}</div>
              </q-card-section>
            </q-card>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Historico / Movimentacoes" :subtitulo="`Eventos registrados: ${totalHistorico}`">
        <LoadingState v-if="loadingHistorico && !historico.length" inline mensagem="Carregando historico..." />

        <EmptyState
          v-else-if="!historico.length"
          titulo="Nenhum historico registrado"
          mensagem="Nao ha movimentacoes registradas para este ativo."
          icon="history"
        />

        <q-list v-else bordered separator>
          <q-item v-for="evento in historico" :key="evento.id">
            <q-item-section>
              <q-item-label class="text-weight-medium">{{ evento.tipoMovimentacaoDescricao }}</q-item-label>

              <q-item-label v-if="evento.departamentoOrigemNome || evento.departamentoDestinoNome" caption>
                Departamento: {{ montarDescricaoMudanca(evento.departamentoOrigemNome, evento.departamentoDestinoNome) }}
              </q-item-label>

              <q-item-label v-if="evento.localUnidadeOrigemNome || evento.localUnidadeDestinoNome" caption>
                Local: {{ montarDescricaoMudanca(evento.localUnidadeOrigemNome, evento.localUnidadeDestinoNome) }}
              </q-item-label>

              <q-item-label v-if="evento.usuarioResponsavelOrigemNome || evento.usuarioResponsavelDestinoNome" caption>
                Responsavel:
                {{ montarDescricaoMudanca(evento.usuarioResponsavelOrigemNome, evento.usuarioResponsavelDestinoNome) }}
              </q-item-label>

              <q-item-label
                v-if="evento.statusOperacionalAnterior !== null || evento.statusOperacionalNovo !== null"
                caption
              >
                Status operacional: {{ formatarStatusOperacional(evento.statusOperacionalAnterior) }} -> {{ formatarStatusOperacional(evento.statusOperacionalNovo) }}
              </q-item-label>

              <q-item-label
                v-if="evento.statusPatrimonialAnterior !== null || evento.statusPatrimonialNovo !== null"
                caption
              >
                Status patrimonial: {{ formatarStatusPatrimonial(evento.statusPatrimonialAnterior) }} -> {{ formatarStatusPatrimonial(evento.statusPatrimonialNovo) }}
              </q-item-label>

              <q-item-label v-if="evento.observacao" caption>Observacao: {{ evento.observacao }}</q-item-label>
            </q-item-section>

            <q-item-section side>
              <q-item-label caption>{{ formatarData(evento.criadoEm) }}</q-item-label>
              <q-item-label caption>{{ evento.criadoPorUsuarioNome }}</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>

        <q-separator class="q-my-md" />

        <PaginacaoTabela
          :pagina="paginaHistorico"
          :tamanho-pagina="tamanhoPaginaHistorico"
          :total="totalHistorico"
          :loading="loadingHistorico"
          @update:pagina="atualizarPaginaHistorico"
          @update:tamanho-pagina="atualizarTamanhoPaginaHistorico"
        />
      </AppSectionCard>

      <AppSectionCard titulo="Chamados relacionados" :subtitulo="`Chamados vinculados: ${totalChamados}`">
        <LoadingState v-if="loadingChamados && !chamados.length" inline mensagem="Carregando chamados relacionados..." />

        <EmptyState
          v-else-if="!chamados.length"
          titulo="Nenhum chamado relacionado"
          mensagem="Nao ha chamados vinculados a este ativo."
          icon="support_agent"
        />

        <q-table
          v-else
          flat
          bordered
          row-key="chamadoId"
          :rows="chamados"
          :columns="[
            { name: 'protocolo', label: 'Protocolo', field: 'protocolo', align: 'left' },
            { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left' },
            { name: 'status', label: 'Status', field: 'status', align: 'left' },
            { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left' },
            { name: 'solicitanteNome', label: 'Solicitante', field: 'solicitanteNome', align: 'left' },
            { name: 'criadoEm', label: 'Criado em', field: 'criadoEm', align: 'left' },
            { name: 'encerradoEm', label: 'Encerrado em', field: 'encerradoEm', align: 'left' },
            { name: 'acoes', label: 'Acoes', field: 'chamadoId', align: 'right' },
          ]"
          :rows-per-page-options="[0]"
          hide-bottom
        >
          <template #body-cell-criadoEm="slotProps">
            <q-td :props="slotProps">{{ formatarData(slotProps.row.criadoEm) }}</q-td>
          </template>

          <template #body-cell-status="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.status" />
            </q-td>
          </template>

          <template #body-cell-prioridade="slotProps">
            <q-td :props="slotProps">
              <StatusBadge :texto="slotProps.row.prioridade" />
            </q-td>
          </template>

          <template #body-cell-encerradoEm="slotProps">
            <q-td :props="slotProps">{{ formatarData(slotProps.row.encerradoEm) }}</q-td>
          </template>

          <template #body-cell-acoes="slotProps">
            <q-td :props="slotProps" class="text-right">
              <q-btn
                flat
                dense
                color="primary"
                icon="open_in_new"
                label="Abrir chamado"
                @click="router.push(`/admin/chamados/${slotProps.row.chamadoId}`)"
              />
            </q-td>
          </template>
        </q-table>

        <q-separator class="q-my-md" />

        <PaginacaoTabela
          :pagina="paginaChamados"
          :tamanho-pagina="tamanhoPaginaChamados"
          :total="totalChamados"
          :loading="loadingChamados"
          @update:pagina="atualizarPaginaChamados"
          @update:tamanho-pagina="atualizarTamanhoPaginaChamados"
        />
      </AppSectionCard>
    </template>

    <EmptyState
      v-else
      titulo="Ativo nao encontrado"
      mensagem="Nao foi possivel localizar o ativo solicitado."
      icon="inventory_2"
    />

    <ConfirmDialog
      v-model="showConfirmarAcao"
      :titulo="tituloConfirmacao"
      :mensagem="mensagemConfirmacao"
      :confirmar-label="labelConfirmacao"
      :color="corConfirmacao"
      :loading="executandoAcao"
      @confirm="executarAcao"
    />

    <q-dialog v-model="showMovimentacao">
      <q-card class="sgx-card dialog-movimentacao">
        <q-card-section>
          <div class="text-h6">Movimentar ativo</div>
        </q-card-section>

        <q-card-section>
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-6">
              <q-select
                v-model="formMovimentacao.departamentoId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Departamento"
                :disable="salvandoMovimentacao"
                :options="departamentos.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-6">
              <q-select
                v-model="formMovimentacao.localUnidadeId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Local / Unidade"
                :disable="salvandoMovimentacao"
                :options="locaisUnidade.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-6">
              <q-select
                v-model="formMovimentacao.usuarioResponsavelId"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Usuario responsavel"
                :disable="salvandoMovimentacao"
                :options="usuariosResponsaveis.map((item) => ({ label: item.nome, value: item.id }))"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-select
                v-model="formMovimentacao.statusOperacional"
                outlined
                dense
                emit-value
                map-options
                label="Status operacional"
                :disable="salvandoMovimentacao"
                :options="opcoesStatusOperacional"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-select
                v-model="formMovimentacao.statusPatrimonial"
                outlined
                dense
                emit-value
                map-options
                label="Status patrimonial"
                :disable="salvandoMovimentacao"
                :options="opcoesStatusPatrimonial"
              />
            </div>

            <div class="col-12">
              <q-input
                v-model="formMovimentacao.observacao"
                outlined
                dense
                type="textarea"
                autogrow
                maxlength="2000"
                counter
                label="Observacao"
                hint="Opcional, mas recomendada para contexto da movimentacao."
                :readonly="salvandoMovimentacao"
              />
            </div>
          </div>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" :disable="salvandoMovimentacao" v-close-popup />
          <q-btn color="primary" label="Salvar movimentacao" :loading="salvandoMovimentacao" @click="salvarMovimentacao" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<style scoped>
.detalhe-ativo__kpis {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

.dialog-movimentacao {
  width: min(860px, 96vw);
}

.detalhe-ativo__bloco-texto {
  height: 100%;
}

@media (max-width: 1100px) {
  .detalhe-ativo__kpis {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 768px) {
  .detalhe-ativo__kpis {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
