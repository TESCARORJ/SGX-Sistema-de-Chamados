<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import PaginacaoTabela from '../components/admin/cadastros/PaginacaoTabela.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { aprovacaoChamadosAdminService } from '../services/aprovacaoChamadosAdminService'
import { useAuthStore } from '../stores/authStore'
import {
  StatusAprovacaoChamado,
  TipoOrigemAprovacaoChamado,
  type AprovacaoChamadoListagem,
  type FiltroAprovacaoChamadoRequest,
} from '../types/aprovacaoChamados'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)
const processandoAcao = ref(false)

const aprovacoes = ref<AprovacaoChamadoListagem[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)

const filtros = reactive({
  termo: '',
  chamadoId: '',
  status: '' as '' | StatusAprovacaoChamado,
  tipoOrigem: '' as '' | TipoOrigemAprovacaoChamado,
  solicitanteId: '',
  aprovadorId: '',
  dataSolicitacaoInicial: '',
  dataSolicitacaoFinal: '',
  dataDecisaoInicial: '',
  dataDecisaoFinal: '',
})

const dialogAcaoAberto = ref(false)
const acaoSelecionada = ref<'aprovar' | 'reprovar' | 'cancelar' | null>(null)
const aprovacaoSelecionada = ref<AprovacaoChamadoListagem | null>(null)
const justificativaAcao = ref('')

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.aprovacaoChamadosVisualizar))
const podeAprovarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosAprovar))
const podeReprovarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosReprovar))
const podeCancelarPermissao = computed(() => possuiPermissao(permissoes.aprovacaoChamadosCancelar))

const opcoesStatus = [
  { label: 'Pendente', value: StatusAprovacaoChamado.Pendente },
  { label: 'Aprovado', value: StatusAprovacaoChamado.Aprovado },
  { label: 'Reprovado', value: StatusAprovacaoChamado.Reprovado },
  { label: 'Cancelado', value: StatusAprovacaoChamado.Cancelado },
]

const opcoesTipoOrigem = [
  { label: 'Manual', value: TipoOrigemAprovacaoChamado.Manual },
  { label: 'Catalogo de servico', value: TipoOrigemAprovacaoChamado.CatalogoServico },
  { label: 'Categoria', value: TipoOrigemAprovacaoChamado.Categoria },
  { label: 'Departamento', value: TipoOrigemAprovacaoChamado.Departamento },
  { label: 'Regra administrativa', value: TipoOrigemAprovacaoChamado.RegraAdministrativa },
]

const colunas: QTableColumn<AprovacaoChamadoListagem>[] = [
  {
    name: 'chamado',
    label: 'Chamado',
    align: 'left',
    field: (row) => `${row.numeroProtocoloChamado} - ${row.tituloChamado}`,
  },
  { name: 'status', label: 'Status', field: 'statusDescricao', align: 'left' },
  {
    name: 'origem',
    label: 'Origem',
    align: 'left',
    field: (row) => row.origemDescricao || row.tipoOrigemDescricao,
  },
  { name: 'solicitante', label: 'Solicitante', field: (row) => row.solicitanteNome || '-', align: 'left' },
  { name: 'aprovador', label: 'Aprovador', field: (row) => row.aprovadorNome || '-', align: 'left' },
  { name: 'solicitadaEm', label: 'Solicitada em', field: 'solicitadaEm', align: 'left' },
  { name: 'decididaEm', label: 'Decidida em', field: 'decididaEm', align: 'left' },
  { name: 'acoes', label: 'Acoes', field: 'id', align: 'right' },
]

const tituloDialogAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'Confirmar aprovacao'
  if (acaoSelecionada.value === 'reprovar') return 'Confirmar reprovacao'
  return 'Confirmar cancelamento'
})

const labelBotaoAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'Aprovar'
  if (acaoSelecionada.value === 'reprovar') return 'Reprovar'
  return 'Cancelar aprovacao'
})

const corBotaoAcao = computed(() => {
  if (acaoSelecionada.value === 'aprovar') return 'positive'
  if (acaoSelecionada.value === 'reprovar') return 'negative'
  return 'warning'
})

const justificativaObrigatoria = computed(() =>
  acaoSelecionada.value === 'reprovar' || acaoSelecionada.value === 'cancelar'
)

function podeAprovar(row: AprovacaoChamadoListagem): boolean {
  return row.status === StatusAprovacaoChamado.Pendente && podeAprovarPermissao.value
}

function podeReprovar(row: AprovacaoChamadoListagem): boolean {
  return row.status === StatusAprovacaoChamado.Pendente && podeReprovarPermissao.value
}

function podeCancelar(row: AprovacaoChamadoListagem): boolean {
  return row.status === StatusAprovacaoChamado.Pendente && podeCancelarPermissao.value
}

function corStatus(status: StatusAprovacaoChamado): string {
  switch (status) {
    case StatusAprovacaoChamado.Pendente:
      return 'warning'
    case StatusAprovacaoChamado.Aprovado:
      return 'positive'
    case StatusAprovacaoChamado.Reprovado:
      return 'negative'
    case StatusAprovacaoChamado.Cancelado:
      return 'grey-7'
    default:
      return 'grey-7'
  }
}

function formatarData(data: string | null): string {
  if (!data) return '-'
  return new Date(data).toLocaleString('pt-BR')
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

function montarFiltroRequest(): FiltroAprovacaoChamadoRequest {
  return {
    termo: filtros.termo.trim() || undefined,
    chamadoId: filtros.chamadoId || undefined,
    status: filtros.status === '' ? undefined : filtros.status,
    tipoOrigem: filtros.tipoOrigem === '' ? undefined : filtros.tipoOrigem,
    solicitanteId: filtros.solicitanteId || undefined,
    aprovadorId: filtros.aprovadorId || undefined,
    dataSolicitacaoInicial: filtros.dataSolicitacaoInicial || undefined,
    dataSolicitacaoFinal: filtros.dataSolicitacaoFinal || undefined,
    dataDecisaoInicial: filtros.dataDecisaoInicial || undefined,
    dataDecisaoFinal: filtros.dataDecisaoFinal || undefined,
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
    ordenarPor: 'solicitadaEm',
    direcaoOrdenacao: 'desc',
  }
}

async function carregarAprovacoes(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    const response = await aprovacaoChamadosAdminService.listar(montarFiltroRequest())
    aprovacoes.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar as aprovacoes de chamados.')
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  pagina.value = 1
  await carregarAprovacoes()
}

async function limparFiltros(): Promise<void> {
  filtros.termo = ''
  filtros.chamadoId = ''
  filtros.status = ''
  filtros.tipoOrigem = ''
  filtros.solicitanteId = ''
  filtros.aprovadorId = ''
  filtros.dataSolicitacaoInicial = ''
  filtros.dataSolicitacaoFinal = ''
  filtros.dataDecisaoInicial = ''
  filtros.dataDecisaoFinal = ''
  pagina.value = 1
  await carregarAprovacoes()
}

async function atualizarPagina(value: number): Promise<void> {
  pagina.value = value
  await carregarAprovacoes()
}

async function atualizarTamanhoPagina(value: number): Promise<void> {
  tamanhoPagina.value = value
  pagina.value = 1
  await carregarAprovacoes()
}

function abrirDetalhe(id: string): void {
  router.push(`/admin/atendimento/aprovacao-chamados/${id}`)
}

function abrirDialogAcao(row: AprovacaoChamadoListagem, acao: 'aprovar' | 'reprovar' | 'cancelar'): void {
  aprovacaoSelecionada.value = row
  acaoSelecionada.value = acao
  justificativaAcao.value = ''
  dialogAcaoAberto.value = true
}

function fecharDialogAcao(): void {
  dialogAcaoAberto.value = false
  aprovacaoSelecionada.value = null
  acaoSelecionada.value = null
  justificativaAcao.value = ''
}

async function confirmarAcao(): Promise<void> {
  if (!aprovacaoSelecionada.value || !acaoSelecionada.value) {
    return
  }

  if (justificativaObrigatoria.value && !justificativaAcao.value.trim()) {
    $q.notify({ type: 'warning', message: 'Informe a justificativa para continuar.' })
    return
  }

  processandoAcao.value = true

  try {
    const justificativa = justificativaAcao.value.trim()

    if (acaoSelecionada.value === 'aprovar') {
      await aprovacaoChamadosAdminService.aprovar(aprovacaoSelecionada.value.id, {
        justificativaDecisao: justificativa || undefined,
      })
      $q.notify({ type: 'positive', message: 'Aprovacao registrada com sucesso.' })
    }

    if (acaoSelecionada.value === 'reprovar') {
      await aprovacaoChamadosAdminService.reprovar(aprovacaoSelecionada.value.id, {
        justificativaDecisao: justificativa,
      })
      $q.notify({ type: 'positive', message: 'Reprovacao registrada com sucesso.' })
    }

    if (acaoSelecionada.value === 'cancelar') {
      await aprovacaoChamadosAdminService.cancelar(aprovacaoSelecionada.value.id, {
        justificativaDecisao: justificativa,
      })
      $q.notify({ type: 'positive', message: 'Cancelamento registrado com sucesso.' })
    }

    fecharDialogAcao()
    await carregarAprovacoes()
  } catch (error) {
    const mensagem = extrairMensagemErro(error, 'Nao foi possivel concluir a acao desta aprovacao.')
    erro.value = mensagem
    $q.notify({ type: 'negative', message: mensagem })
  } finally {
    processandoAcao.value = false
  }
}

onMounted(carregarAprovacoes)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Aprovacao de chamados"
      subtitulo="Lista chamados aguardando ou ja submetidos ao fluxo de aprovacao administrativa."
    />

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar aprovacoes de chamados.
    </q-banner>

    <template v-else>
      <AppSectionCard titulo="Filtros" subtitulo="Refine por status, origem, usuarios e periodos de solicitacao/decisao.">
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-3">
              <q-input v-model="filtros.termo" outlined label="Busca" placeholder="Protocolo ou titulo" :disable="loading" />
            </div>

            <div class="col-12 col-md-2">
              <q-input v-model="filtros.chamadoId" outlined label="Chamado (ID)" :disable="loading" />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.status"
                outlined
                clearable
                emit-value
                map-options
                label="Status"
                :disable="loading"
                :options="opcoesStatus"
              />
            </div>

            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.tipoOrigem"
                outlined
                clearable
                emit-value
                map-options
                label="Tipo de origem"
                :disable="loading"
                :options="opcoesTipoOrigem"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-input v-model="filtros.solicitanteId" outlined label="Solicitante (ID)" :disable="loading" />
            </div>

            <div class="col-12 col-md-3">
              <q-input v-model="filtros.aprovadorId" outlined label="Aprovador (ID)" :disable="loading" />
            </div>

            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.dataSolicitacaoInicial"
                outlined
                type="date"
                label="Solicitada de"
                :disable="loading"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.dataSolicitacaoFinal"
                outlined
                type="date"
                label="Solicitada ate"
                :disable="loading"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.dataDecisaoInicial"
                outlined
                type="date"
                label="Decidida de"
                :disable="loading"
              />
            </div>

            <div class="col-12 col-md-3">
              <q-input
                v-model="filtros.dataDecisaoFinal"
                outlined
                type="date"
                label="Decidida ate"
                :disable="loading"
              />
            </div>
          </div>

          <div class="row q-gutter-sm justify-end">
            <q-btn flat label="Limpar" color="primary" :disable="loading" @click="limparFiltros" />
            <q-btn unelevated type="submit" label="Filtrar" color="primary" :loading="loading" />
          </div>
        </q-form>
      </AppSectionCard>

      <AppSectionCard titulo="Aprovacoes" subtitulo="Resultados ordenados por data de solicitacao mais recente.">
        <LoadingState v-if="loading" inline mensagem="Carregando aprovacoes..." />

        <ErrorState
          v-else-if="erro"
          titulo="Falha ao carregar aprovacoes"
          :mensagem="erro"
          @retry="carregarAprovacoes"
        />

        <EmptyState
          v-else-if="!aprovacoes.length"
          titulo="Nenhuma aprovacao encontrada"
          mensagem="Ajuste os filtros e tente novamente."
        />

        <template v-else>
          <q-table :rows="aprovacoes" :columns="colunas" row-key="id" flat bordered>
            <template #body-cell-chamado="slotProps">
              <q-td :props="slotProps">
                <div class="text-weight-medium">{{ slotProps.row.numeroProtocoloChamado }}</div>
                <div class="text-caption text-grey-7">{{ slotProps.row.tituloChamado }}</div>
              </q-td>
            </template>

            <template #body-cell-status="slotProps">
              <q-td :props="slotProps">
                <q-chip dense square text-color="white" :color="corStatus(slotProps.row.status)">
                  {{ slotProps.row.statusDescricao }}
                </q-chip>
              </q-td>
            </template>

            <template #body-cell-origem="slotProps">
              <q-td :props="slotProps">
                <div>{{ slotProps.row.tipoOrigemDescricao }}</div>
                <div class="text-caption text-grey-7">{{ slotProps.row.origemDescricao || '-' }}</div>
              </q-td>
            </template>

            <template #body-cell-solicitadaEm="slotProps">
              <q-td :props="slotProps">{{ formatarData(slotProps.row.solicitadaEm) }}</q-td>
            </template>

            <template #body-cell-decididaEm="slotProps">
              <q-td :props="slotProps">{{ formatarData(slotProps.row.decididaEm) }}</q-td>
            </template>

            <template #body-cell-acoes="slotProps">
              <q-td :props="slotProps" class="text-right">
                <div class="row justify-end q-gutter-xs">
                  <q-btn flat dense color="primary" icon="visibility" label="Visualizar" @click="abrirDetalhe(slotProps.row.id)" />
                  <q-btn
                    v-if="podeAprovar(slotProps.row)"
                    flat
                    dense
                    color="positive"
                    icon="check"
                    label="Aprovar"
                    @click="abrirDialogAcao(slotProps.row, 'aprovar')"
                  />
                  <q-btn
                    v-if="podeReprovar(slotProps.row)"
                    flat
                    dense
                    color="negative"
                    icon="close"
                    label="Reprovar"
                    @click="abrirDialogAcao(slotProps.row, 'reprovar')"
                  />
                  <q-btn
                    v-if="podeCancelar(slotProps.row)"
                    flat
                    dense
                    color="warning"
                    icon="cancel"
                    label="Cancelar"
                    @click="abrirDialogAcao(slotProps.row, 'cancelar')"
                  />
                </div>
              </q-td>
            </template>
          </q-table>

          <div class="q-mt-md">
            <PaginacaoTabela
              :pagina="pagina"
              :tamanho-pagina="tamanhoPagina"
              :total="total"
              :loading="loading"
              @update:pagina="atualizarPagina"
              @update:tamanho-pagina="atualizarTamanhoPagina"
            />
          </div>
        </template>
      </AppSectionCard>
    </template>

    <q-dialog v-model="dialogAcaoAberto">
      <q-card class="sgx-card" style="width: min(640px, 94vw)">
        <q-card-section class="text-h6">{{ tituloDialogAcao }}</q-card-section>

        <q-card-section class="column q-gutter-sm">
          <div class="text-body2 text-grey-8">
            Confirme a acao para o chamado <strong>{{ aprovacaoSelecionada?.numeroProtocoloChamado }}</strong>.
          </div>

          <q-input
            v-model="justificativaAcao"
            outlined
            autogrow
            type="textarea"
            :label="justificativaObrigatoria ? 'Justificativa (obrigatoria)' : 'Justificativa (opcional)'"
            :rules="justificativaObrigatoria ? [(v) => !!String(v || '').trim() || 'Informe a justificativa'] : []"
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Voltar" :disable="processandoAcao" @click="fecharDialogAcao" />
          <q-btn :color="corBotaoAcao" :label="labelBotaoAcao" :loading="processandoAcao" @click="confirmarAcao" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
