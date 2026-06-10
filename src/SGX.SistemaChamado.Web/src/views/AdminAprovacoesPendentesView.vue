<script setup lang="ts">
import { onMounted, reactive, ref, computed } from 'vue'
import type { QTableColumn } from 'quasar'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PaginacaoTabela from '../components/admin/cadastros/PaginacaoTabela.vue'
import { aprovacoesMotorService } from '../services/aprovacoesMotorService'
import type { InstanciaAprovacaoChamadoResumoResponse } from '../types/aprovacoesMotor'
import { StatusInstanciaAprovacaoChamado, EfeitoOperacionalRegraAprovacao } from '../types/aprovacoesMotor'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const aprovacoes = ref<InstanciaAprovacaoChamadoResumoResponse[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)

const showModalAprovar = ref(false)
const processingAprovar = ref(false)
const pendenciaSelecionada = ref<InstanciaAprovacaoChamadoResumoResponse | null>(null)
const justificativaAprovacao = ref('')

const showModalReprovar = ref(false)
const processingReprovar = ref(false)
const pendenciaReprovarSelecionada = ref<InstanciaAprovacaoChamadoResumoResponse | null>(null)
const justificativaReprovacao = ref('')
const observacaoReprovacao = ref('')

const filtros = reactive({
  termo: '',
  status: null as StatusInstanciaAprovacaoChamado | null,
  apenasPendentes: true,
  apenasBloqueantes: false,
  ordenarPor: 'solicitadaem',
  direcaoOrdenacao: 'desc' as 'asc' | 'desc'
})

const opcoesStatus = [
  { label: 'Todos', value: null },
  { label: 'Aguardando aprovação', value: StatusInstanciaAprovacaoChamado.Pendente },
  { label: 'Em reavaliação', value: StatusInstanciaAprovacaoChamado.EmReavaliacao },
  { label: 'Aprovada', value: StatusInstanciaAprovacaoChamado.Aprovado },
  { label: 'Reprovada', value: StatusInstanciaAprovacaoChamado.Reprovado },
  { label: 'Cancelada', value: StatusInstanciaAprovacaoChamado.Cancelado },
  { label: 'Expirada', value: StatusInstanciaAprovacaoChamado.Expirado }
]

const opcoesOrdenacao = [
  { label: 'Mais recentes primeiro', value: 'solicitadaem_desc' },
  { label: 'Mais antigas primeiro', value: 'solicitadaem_asc' },
  { label: 'Vencimento mais próximo', value: 'deveexpirarem_asc' }
]

const ordenacaoSelecionada = computed({
  get: () => `${filtros.ordenarPor}_${filtros.direcaoOrdenacao}`,
  set: (val: string) => {
    const partes = val.split('_')
    if (partes.length === 2) {
      filtros.ordenarPor = partes[0]
      filtros.direcaoOrdenacao = partes[1] as 'asc' | 'desc'
    }
  }
})

const colunas: QTableColumn<InstanciaAprovacaoChamadoResumoResponse>[] = [
  {
    name: 'chamado',
    label: 'Chamado',
    align: 'left',
    field: (row) => `${row.numeroProtocoloChamado || row.chamadoId}`,
  },
  {
    name: 'status',
    label: 'Status da pendência',
    field: 'status',
    align: 'left',
  },
  {
    name: 'urgencia',
    label: 'Urgência Operacional',
    field: 'bloqueante',
    align: 'left',
  },
  {
    name: 'regra',
    label: 'Regra aplicada',
    field: 'nomeRegra',
    align: 'left',
  },
  { name: 'criadoEm', label: 'Solicitada em', field: 'criadoEm', align: 'left' },
  { name: 'vencimentoEm', label: 'Vencimento', field: 'deveExpirarEm', align: 'left' },
  { name: 'acoes', label: 'Ações', field: 'id', align: 'right' },
]

function formatarData(data: string | null | undefined): string {
  if (!data) return '-'
  return new Date(data).toLocaleString('pt-BR')
}

function corStatus(status: StatusInstanciaAprovacaoChamado): string {
  switch (status) {
    case StatusInstanciaAprovacaoChamado.Pendente:
      return 'warning'
    case StatusInstanciaAprovacaoChamado.Aprovado:
      return 'positive'
    case StatusInstanciaAprovacaoChamado.Reprovado:
      return 'negative'
    case StatusInstanciaAprovacaoChamado.Cancelado:
      return 'grey-7'
    case StatusInstanciaAprovacaoChamado.Expirado:
      return 'negative'
    case StatusInstanciaAprovacaoChamado.EmReavaliacao:
      return 'info'
    default:
      return 'grey-7'
  }
}

function descricaoStatus(status: StatusInstanciaAprovacaoChamado): string {
  switch (status) {
    case StatusInstanciaAprovacaoChamado.Pendente:
      return 'Aguardando aprovação'
    case StatusInstanciaAprovacaoChamado.Aprovado:
      return 'Aprovada'
    case StatusInstanciaAprovacaoChamado.Reprovado:
      return 'Reprovada'
    case StatusInstanciaAprovacaoChamado.Cancelado:
      return 'Cancelada'
    case StatusInstanciaAprovacaoChamado.Expirado:
      return 'Expirada'
    case StatusInstanciaAprovacaoChamado.EmReavaliacao:
      return 'Em reavaliação'
    default:
      return 'Desconhecido'
  }
}

function situacaoVencimento(dataVencimento: string | null | undefined): { label: string; cor: string } | null {
  if (!dataVencimento) return null
  
  const vencimento = new Date(dataVencimento)
  const agora = new Date()
  const diffTime = vencimento.getTime() - agora.getTime()
  const diffHoras = Math.ceil(diffTime / (1000 * 60 * 60))
  
  if (diffHoras < 0) {
    return { label: 'Vencida', cor: 'negative' }
  } else if (diffHoras <= 24) {
    return { label: 'Vence hoje', cor: 'orange-9' }
  } else if (diffHoras <= 72) {
    return { label: 'Prazo próximo', cor: 'warning' }
  }
  
  return null
}

async function carregarAprovacoes(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const params: Record<string, any> = {
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
      apenasPendentes: filtros.apenasPendentes,
      apenasBloqueantes: filtros.apenasBloqueantes,
      ordenarPor: filtros.ordenarPor,
      direcaoOrdenacao: filtros.direcaoOrdenacao
    }

    if (filtros.termo && filtros.termo.trim().length > 0) {
      params.termo = filtros.termo.trim()
    }

    if (filtros.status !== null) {
      params.status = filtros.status
      // Se filtrou por um status específico, não força 'apenasPendentes' caso o status seja resolvido
      if (filtros.status !== StatusInstanciaAprovacaoChamado.Pendente && 
          filtros.status !== StatusInstanciaAprovacaoChamado.EmReavaliacao) {
        params.apenasPendentes = false
      }
    }

    const response = await aprovacoesMotorService.listarMinhasPendencias(params)
    
    aprovacoes.value = response.items
    total.value = response.total
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Falha ao carregar as pendências de aprovação.'
    erro.value = message
  } finally {
    loading.value = false
  }
}

function aplicarFiltros(): void {
  pagina.value = 1
  carregarAprovacoes()
}

function limparFiltros(): void {
  filtros.termo = ''
  filtros.status = null
  filtros.apenasPendentes = true
  filtros.apenasBloqueantes = false
  filtros.ordenarPor = 'solicitadaem'
  filtros.direcaoOrdenacao = 'desc'
  aplicarFiltros()
}

function abrirDetalheChamado(id: string): void {
  router.push(`/admin/chamados/${id}`)
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

function abrirAprovarModal(pendencia: InstanciaAprovacaoChamadoResumoResponse): void {
  pendenciaSelecionada.value = pendencia
  justificativaAprovacao.value = ''
  showModalAprovar.value = true
}

function fecharAprovarModal(): void {
  showModalAprovar.value = false
  pendenciaSelecionada.value = null
  justificativaAprovacao.value = ''
}

async function confirmarAprovacao(): Promise<void> {
  if (!pendenciaSelecionada.value) return

  processingAprovar.value = true
  
  try {
    await aprovacoesMotorService.aprovarAprovacao({
      instanciaAprovacaoChamadoId: pendenciaSelecionada.value.id,
      decisaoFinal: true, // Aprovação simples por padrão no frontend
      justificativa: justificativaAprovacao.value || null,
      observacao: justificativaAprovacao.value || null
    })
    
    fecharAprovarModal()
    await carregarAprovacoes()
    
    import('quasar').then(({ Notify }) => {
      Notify.create({ type: 'positive', message: 'Aprovação registrada com sucesso.' })
    })
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Não foi possível registrar a aprovação.'
    import('quasar').then(({ Notify }) => {
      Notify.create({ type: 'negative', message })
    })
  } finally {
    processingAprovar.value = false
  }
}

function abrirReprovarModal(pendencia: InstanciaAprovacaoChamadoResumoResponse): void {
  pendenciaReprovarSelecionada.value = pendencia
  justificativaReprovacao.value = ''
  observacaoReprovacao.value = ''
  showModalReprovar.value = true
}

function fecharReprovarModal(): void {
  showModalReprovar.value = false
  pendenciaReprovarSelecionada.value = null
  justificativaReprovacao.value = ''
  observacaoReprovacao.value = ''
}

async function confirmarReprovacao(): Promise<void> {
  if (!pendenciaReprovarSelecionada.value) return
  if (!justificativaReprovacao.value || !justificativaReprovacao.value.trim()) {
    import('quasar').then(({ Notify }) => {
      Notify.create({ type: 'warning', message: 'Informe a justificativa da rejeição.' })
    })
    return
  }

  processingReprovar.value = true
  
  try {
    await aprovacoesMotorService.reprovarAprovacao({
      instanciaAprovacaoChamadoId: pendenciaReprovarSelecionada.value.id,
      decisaoFinal: true, // Reprovação simples por padrão no frontend
      justificativa: justificativaReprovacao.value.trim(),
      observacao: observacaoReprovacao.value || null
    })
    
    fecharReprovarModal()
    await carregarAprovacoes()
    
    import('quasar').then(({ Notify }) => {
      Notify.create({ type: 'positive', message: 'Rejeição registrada com sucesso.' })
    })
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Não foi possível registrar a rejeição.'
    import('quasar').then(({ Notify }) => {
      Notify.create({ type: 'negative', message })
    })
  } finally {
    processingReprovar.value = false
  }
}

onMounted(carregarAprovacoes)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Pendências de aprovação"
      subtitulo="Filtre e acompanhe aprovações pendentes do motor ITSM."
    />

    <AppSectionCard titulo="Filtros" subtitulo="Refine sua busca por pendências.">
      <div class="row q-col-gutter-md items-center">
        <div class="col-12 col-md-3">
          <q-input
            v-model="filtros.termo"
            label="Buscar chamado ou regra"
            outlined
            dense
            clearable
            @keyup.enter="aplicarFiltros"
          >
            <template v-slot:append>
              <q-icon name="search" />
            </template>
          </q-input>
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.status"
            :options="opcoesStatus"
            label="Status"
            outlined
            dense
            emit-value
            map-options
            @update:model-value="aplicarFiltros"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="ordenacaoSelecionada"
            :options="opcoesOrdenacao"
            label="Ordenar por"
            outlined
            dense
            emit-value
            map-options
            @update:model-value="aplicarFiltros"
          />
        </div>
        <div class="col-12 col-md-3 flex q-gutter-sm items-center">
          <q-toggle
            v-model="filtros.apenasPendentes"
            label="Apenas pendentes"
            dense
            color="primary"
            @update:model-value="aplicarFiltros"
          />
          <q-toggle
            v-model="filtros.apenasBloqueantes"
            label="Apenas bloqueantes"
            dense
            color="negative"
            @update:model-value="aplicarFiltros"
          />
        </div>
        <div class="col-12 flex justify-end q-gutter-sm">
          <q-btn flat label="Limpar Filtros" color="grey-7" @click="limparFiltros" />
          <q-btn unelevated color="primary" label="Filtrar" icon="filter_alt" @click="aplicarFiltros" />
        </div>
      </div>
    </AppSectionCard>

    <AppSectionCard titulo="Resultados" subtitulo="Pendências encontradas de acordo com os filtros.">
      <LoadingState v-if="loading" inline mensagem="Carregando pendências..." />

      <ErrorState
        v-else-if="erro"
        titulo="Falha ao carregar pendências"
        :mensagem="erro"
        @retry="carregarAprovacoes"
      />

      <EmptyState
        v-else-if="!aprovacoes.length"
        titulo="Nenhuma pendência encontrada com os filtros selecionados."
        mensagem="Tente alterar os termos de busca ou limpar os filtros."
      />

      <template v-else>
        <q-table :rows="aprovacoes" :columns="colunas" row-key="id" flat bordered>
          <template #body-cell-chamado="slotProps">
            <q-td :props="slotProps">
              <div class="text-weight-medium text-primary cursor-pointer" @click="abrirDetalheChamado(slotProps.row.chamadoId)">
                {{ slotProps.row.numeroProtocoloChamado || slotProps.row.chamadoId }}
              </div>
              <div class="text-caption text-grey-7" v-if="slotProps.row.titulo">{{ slotProps.row.titulo }}</div>
            </q-td>
          </template>

          <template #body-cell-status="slotProps">
            <q-td :props="slotProps">
              <q-chip dense square text-color="white" :color="corStatus(slotProps.row.status)">
                {{ descricaoStatus(slotProps.row.status) }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-urgencia="slotProps">
            <q-td :props="slotProps">
              <q-badge v-if="slotProps.row.bloqueante" color="negative" class="q-mr-xs">Bloqueante</q-badge>
              <q-badge v-else color="info" class="q-mr-xs">Informativa</q-badge>
            </q-td>
          </template>

          <template #body-cell-regra="slotProps">
            <q-td :props="slotProps">
              <div>{{ slotProps.row.nomeRegra || 'Regra padrão' }}</div>
            </q-td>
          </template>

          <template #body-cell-criadoEm="slotProps">
            <q-td :props="slotProps">
              <div>{{ formatarData(slotProps.row.criadoEm) }}</div>
              <div class="text-caption text-grey-7" v-if="slotProps.row.solicitanteNome">
                Por: {{ slotProps.row.solicitanteNome }}
              </div>
            </q-td>
          </template>

          <template #body-cell-vencimentoEm="slotProps">
            <q-td :props="slotProps">
              <div v-if="slotProps.row.deveExpirarEm">
                <div>{{ formatarData(slotProps.row.deveExpirarEm) }}</div>
                <q-badge
                  v-if="situacaoVencimento(slotProps.row.deveExpirarEm)"
                  :color="situacaoVencimento(slotProps.row.deveExpirarEm)?.cor"
                  class="q-mt-xs"
                >
                  {{ situacaoVencimento(slotProps.row.deveExpirarEm)?.label }}
                </q-badge>
              </div>
              <div v-else class="text-grey-7 text-italic">Sem prazo definido</div>
            </q-td>
          </template>

          <template #body-cell-acoes="slotProps">
            <q-td :props="slotProps" class="text-right">
              <q-btn
                v-if="slotProps.row.status === StatusInstanciaAprovacaoChamado.Pendente || slotProps.row.status === StatusInstanciaAprovacaoChamado.EmReavaliacao"
                flat
                dense
                color="positive"
                icon="check_circle"
                label="Aprovar"
                @click="abrirAprovarModal(slotProps.row)"
                class="q-mr-sm"
              />
              <q-btn
                v-if="slotProps.row.status === StatusInstanciaAprovacaoChamado.Pendente || slotProps.row.status === StatusInstanciaAprovacaoChamado.EmReavaliacao"
                flat
                dense
                color="negative"
                icon="cancel"
                label="Rejeitar"
                @click="abrirReprovarModal(slotProps.row)"
                class="q-mr-sm"
              />
              <q-btn
                flat
                dense
                color="primary"
                icon="open_in_new"
                label="Ver chamado"
                @click="abrirDetalheChamado(slotProps.row.chamadoId)"
              />
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

    <q-dialog v-model="showModalAprovar" persistent>
      <q-card style="min-width: 400px">
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">Aprovar pendência</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section>
          <p>Confirma a aprovação desta pendência do motor ITSM?</p>
          <q-input
            v-model="justificativaAprovacao"
            type="textarea"
            label="Observação (opcional)"
            outlined
            autogrow
            :rules="[val => !val || val.length <= 500 || 'Máximo de 500 caracteres']"
          />
        </q-card-section>

        <q-card-actions align="right" class="text-primary">
          <q-btn flat label="Cancelar" @click="fecharAprovarModal" :disable="processingAprovar" />
          <q-btn color="positive" label="Confirmar aprovação" @click="confirmarAprovacao" :loading="processingAprovar" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="showModalReprovar" persistent>
      <q-card style="min-width: 400px">
        <q-card-section class="row items-center q-pb-none">
          <div class="text-h6">Rejeitar pendência</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section>
          <p>Informe a justificativa para rejeitar esta pendência do motor ITSM.</p>
          <q-input
            v-model="justificativaReprovacao"
            type="textarea"
            label="Justificativa *"
            outlined
            autogrow
            :rules="[val => (val && val.trim().length > 0) || 'Justificativa é obrigatória', val => val.length <= 500 || 'Máximo de 500 caracteres']"
            class="q-mb-md"
          />
          <q-input
            v-model="observacaoReprovacao"
            type="textarea"
            label="Observação (opcional)"
            outlined
            autogrow
            :rules="[val => !val || val.length <= 500 || 'Máximo de 500 caracteres']"
          />
        </q-card-section>

        <q-card-actions align="right" class="text-primary">
          <q-btn flat label="Cancelar" @click="fecharReprovarModal" :disable="processingReprovar" />
          <q-btn color="negative" label="Confirmar rejeição" @click="confirmarReprovacao" :loading="processingReprovar" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
