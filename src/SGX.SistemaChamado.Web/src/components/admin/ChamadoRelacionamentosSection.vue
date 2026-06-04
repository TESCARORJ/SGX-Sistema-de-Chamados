<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { adminService } from '../../services/adminService'
import type {
  ChamadoAprovacaoAdminResponse,
  ChamadoRelacionamentoAdmin,
  ChamadoTarefaAdminResponse,
} from '../../types/admin'
import AppSectionCard from '../ui/AppSectionCard.vue'
import EmptyState from '../ui/EmptyState.vue'
import ErrorState from '../ui/ErrorState.vue'
import LoadingState from '../ui/LoadingState.vue'
import {
  obterChamadoRelacionadoCodigo,
  obterChamadoRelacionadoId,
  obterCorStatusAprovacao,
  obterCorStatusTarefa,
  obterCorTipoRelacionamento,
  obterDescricaoDirecionalRelacionamento,
  obterIconeTipoRelacionamento,
  obterResumoAprovacoesChamado,
  obterResumoBloqueiosChamado,
  obterResumoDerivacoesChamado,
  obterResumoTarefasChamado,
} from './chamadoRelacionamentosPresentation'

const props = withDefaults(
  defineProps<{
    chamadoId: string
    canManage?: boolean
  }>(),
  {
    canManage: false,
  }
)

const router = useRouter()
const loading = ref(false)
const error = ref<string | null>(null)
const erroTarefas = ref<string | null>(null)
const erroAprovacoes = ref<string | null>(null)
const relacionamentos = ref<ChamadoRelacionamentoAdmin[]>([])
const tarefas = ref<ChamadoTarefaAdminResponse[]>([])
const aprovacoes = ref<ChamadoAprovacaoAdminResponse[]>([])
const incluirInativos = ref(false)

const resumoBloqueios = computed(() => obterResumoBloqueiosChamado(relacionamentos.value, props.chamadoId))
const resumoDerivacoes = computed(() => obterResumoDerivacoesChamado(relacionamentos.value, props.chamadoId))
const resumoTarefas = computed(() => obterResumoTarefasChamado(tarefas.value))
const resumoAprovacoes = computed(() => obterResumoAprovacoesChamado(aprovacoes.value))

const totalRelacionamentos = computed(() => relacionamentos.value.length)
const totalPendenciasOperacionais = computed(
  () =>
    resumoBloqueios.value.bloqueadoPor.length +
    resumoTarefas.value.pendentes +
    resumoTarefas.value.emAndamento +
    resumoAprovacoes.value.pendentes
)

function formatarData(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

async function carregarDadosOperacionais(): Promise<void> {
  if (!props.chamadoId) {
    relacionamentos.value = []
    tarefas.value = []
    aprovacoes.value = []
    return
  }

  loading.value = true
  error.value = null
  erroTarefas.value = null
  erroAprovacoes.value = null

  const [relacionamentosResult, tarefasResult, aprovacoesResult] = await Promise.allSettled([
    adminService.listarRelacionamentosChamado(props.chamadoId, incluirInativos.value),
    adminService.listarTarefasChamado(props.chamadoId, incluirInativos.value),
    adminService.listarAprovacoesChamado(props.chamadoId, incluirInativos.value),
  ])

  if (relacionamentosResult.status === 'fulfilled') {
    relacionamentos.value = relacionamentosResult.value
  } else {
    relacionamentos.value = []
    error.value = 'Nao foi possivel carregar os relacionamentos do chamado.'
  }

  if (tarefasResult.status === 'fulfilled') {
    tarefas.value = tarefasResult.value
  } else {
    tarefas.value = []
    erroTarefas.value = 'Nao foi possivel carregar as tarefas vinculadas.'
  }

  if (aprovacoesResult.status === 'fulfilled') {
    aprovacoes.value = aprovacoesResult.value
  } else {
    aprovacoes.value = []
    erroAprovacoes.value = 'Nao foi possivel carregar as aprovacoes vinculadas.'
  }

  loading.value = false
}

function abrirChamadoRelacionado(relacionamento: ChamadoRelacionamentoAdmin): void {
  router.push(`/admin/chamados/${obterChamadoRelacionadoId(relacionamento, props.chamadoId)}`)
}

onMounted(carregarDadosOperacionais)
watch(() => props.chamadoId, carregarDadosOperacionais)
watch(incluirInativos, carregarDadosOperacionais)
</script>

<template>
  <AppSectionCard
    titulo="Relacionamentos"
    subtitulo="Acompanhe vinculos, dependencias, derivacoes, tarefas e aprovacoes relacionadas a este chamado."
  >
    <template v-if="canManage" #actions>
      <div class="row q-gutter-xs items-center">
        <q-toggle v-model="incluirInativos" dense label="Mostrar inativos" :disable="loading" />
        <q-btn outline color="primary" icon="add_link" label="Novo vinculo" disable />
        <q-btn outline color="secondary" icon="task_alt" label="Nova tarefa" disable />
        <q-btn outline color="accent" icon="fact_check" label="Nova aprovacao" disable />
        <q-btn outline color="primary" icon="call_split" label="Criar chamado derivado" disable />
      </div>
    </template>

    <LoadingState v-if="loading" inline mensagem="Carregando orquestracao do chamado..." />

    <ErrorState
      v-else-if="error"
      titulo="Falha ao carregar relacionamentos"
      :mensagem="error"
      @retry="carregarDadosOperacionais"
    />

    <div v-else class="column q-gutter-md">
      <div class="relacionamentos-summary">
        <div class="relacionamentos-summary__item">
          <span class="text-caption text-grey-7">Vinculos</span>
          <strong>{{ totalRelacionamentos }}</strong>
        </div>
        <div class="relacionamentos-summary__item">
          <span class="text-caption text-grey-7">Bloqueios ativos</span>
          <strong>{{ resumoBloqueios.bloqueadoPor.length }}</strong>
        </div>
        <div class="relacionamentos-summary__item">
          <span class="text-caption text-grey-7">Pendencias operacionais</span>
          <strong>{{ totalPendenciasOperacionais }}</strong>
        </div>
        <div class="relacionamentos-summary__item">
          <span class="text-caption text-grey-7">Aprovacoes bloqueantes</span>
          <strong>{{ resumoAprovacoes.pendentesBloqueantes }}</strong>
        </div>
      </div>

      <div class="relacionamentos-grid">
        <section class="relacionamentos-panel">
          <div class="row items-center q-gutter-sm q-mb-sm">
            <q-icon name="hub" color="negative" size="22px" />
            <div class="text-subtitle2 text-weight-bold">Bloqueios e dependencias</div>
          </div>

          <q-banner
            v-if="resumoBloqueios.bloqueadoPor.length"
            dense
            rounded
            class="bg-red-1 text-negative q-mb-sm"
          >
            Este chamado esta bloqueado por outro chamado.
          </q-banner>
          <q-banner
            v-if="resumoBloqueios.bloqueiaOutros.length"
            dense
            rounded
            class="bg-orange-1 text-orange-10 q-mb-sm"
          >
            Este chamado bloqueia outros chamados.
          </q-banner>
          <p v-if="!resumoBloqueios.possuiBloqueio" class="text-body2 text-grey-7 q-mb-none">
            Nenhum bloqueio ativo identificado.
          </p>

          <div v-if="resumoBloqueios.possuiBloqueio" class="mini-list">
            <div
              v-for="relacionamento in [...resumoBloqueios.bloqueadoPor, ...resumoBloqueios.bloqueiaOutros].slice(0, 3)"
              :key="`bloqueio-${relacionamento.id}`"
              class="mini-list__item"
            >
              <q-icon name="lock" color="negative" />
              <span>{{ obterDescricaoDirecionalRelacionamento(relacionamento, chamadoId) }}</span>
            </div>
          </div>
        </section>

        <section class="relacionamentos-panel">
          <div class="row items-center q-gutter-sm q-mb-sm">
            <q-icon name="call_split" color="primary" size="22px" />
            <div class="text-subtitle2 text-weight-bold">Derivacoes</div>
          </div>

          <p v-if="!resumoDerivacoes.possuiDerivacao" class="text-body2 text-grey-7 q-mb-none">
            Nenhuma derivacao registrada.
          </p>
          <div v-else class="column q-gutter-xs">
            <div v-if="resumoDerivacoes.originados.length" class="text-body2">
              Este chamado originou {{ resumoDerivacoes.originados.length }} chamado(s).
            </div>
            <div
              v-for="relacionamento in resumoDerivacoes.origem.slice(0, 2)"
              :key="`origem-${relacionamento.id}`"
              class="text-body2"
            >
              Este chamado foi criado a partir do chamado
              {{ obterChamadoRelacionadoCodigo(relacionamento, chamadoId) }}.
            </div>
            <div class="mini-list">
              <div
                v-for="relacionamento in resumoDerivacoes.originados.slice(0, 3)"
                :key="`derivado-${relacionamento.id}`"
                class="mini-list__item"
              >
                <q-icon name="account_tree" color="primary" />
                <span>{{ obterDescricaoDirecionalRelacionamento(relacionamento, chamadoId) }}</span>
              </div>
            </div>
          </div>
        </section>

        <section class="relacionamentos-panel">
          <div class="row items-center q-gutter-sm q-mb-sm">
            <q-icon name="task_alt" color="secondary" size="22px" />
            <div class="text-subtitle2 text-weight-bold">Tarefas vinculadas</div>
          </div>

          <q-banner v-if="erroTarefas" dense rounded class="bg-grey-2 text-grey-9 q-mb-sm">
            {{ erroTarefas }}
          </q-banner>
          <p v-else-if="!tarefas.length" class="text-body2 text-grey-7 q-mb-none">
            Nenhuma tarefa vinculada a este chamado.
          </p>
          <div v-else class="column q-gutter-sm">
            <div class="row q-gutter-xs">
              <q-chip dense square color="warning" text-color="white">Pendentes: {{ resumoTarefas.pendentes }}</q-chip>
              <q-chip dense square color="info" text-color="white">Em andamento: {{ resumoTarefas.emAndamento }}</q-chip>
              <q-chip dense square color="positive" text-color="white">Concluidas: {{ resumoTarefas.concluidas }}</q-chip>
              <q-chip dense square color="grey-6" text-color="white">Canceladas: {{ resumoTarefas.canceladas }}</q-chip>
            </div>

            <div class="mini-list">
              <div v-for="tarefa in resumoTarefas.proximas" :key="tarefa.id" class="mini-list__item mini-list__item--stacked">
                <div class="row items-center q-gutter-xs">
                  <span class="text-weight-medium">{{ tarefa.titulo }}</span>
                  <q-chip dense square :color="obterCorStatusTarefa(tarefa.status)" text-color="white">
                    {{ tarefa.statusDescricao }}
                  </q-chip>
                </div>
                <span class="text-caption text-grey-7">
                  Responsavel: {{ tarefa.responsavelNome || '-' }} | Prazo: {{ formatarData(tarefa.prazo) }}
                </span>
              </div>
            </div>
          </div>
        </section>

        <section class="relacionamentos-panel">
          <div class="row items-center q-gutter-sm q-mb-sm">
            <q-icon name="fact_check" color="accent" size="22px" />
            <div class="text-subtitle2 text-weight-bold">Aprovacoes vinculadas</div>
          </div>

          <q-banner v-if="erroAprovacoes" dense rounded class="bg-grey-2 text-grey-9 q-mb-sm">
            {{ erroAprovacoes }}
          </q-banner>
          <q-banner
            v-else-if="resumoAprovacoes.pendentesBloqueantes"
            dense
            rounded
            class="bg-red-1 text-negative q-mb-sm"
          >
            Este chamado possui aprovacao pendente bloqueante.
          </q-banner>
          <p v-else-if="!resumoAprovacoes.pendentes" class="text-body2 text-grey-7 q-mb-none">
            Nenhuma aprovacao pendente vinculada a este chamado.
          </p>

          <div v-if="!erroAprovacoes && aprovacoes.length" class="column q-gutter-sm">
            <div class="row q-gutter-xs">
              <q-chip dense square color="warning" text-color="white">Pendentes: {{ resumoAprovacoes.pendentes }}</q-chip>
              <q-chip dense square color="negative" text-color="white">
                Bloqueantes: {{ resumoAprovacoes.pendentesBloqueantes }}
              </q-chip>
              <q-chip dense square color="positive" text-color="white">Aprovadas: {{ resumoAprovacoes.aprovadas }}</q-chip>
              <q-chip dense square color="grey-6" text-color="white">Canceladas: {{ resumoAprovacoes.canceladas }}</q-chip>
            </div>

            <div class="mini-list">
              <div
                v-for="aprovacao in resumoAprovacoes.listaPendentes"
                :key="aprovacao.id"
                class="mini-list__item mini-list__item--stacked"
              >
                <div class="row items-center q-gutter-xs">
                  <span class="text-weight-medium">{{ aprovacao.titulo }}</span>
                  <q-chip dense square :color="obterCorStatusAprovacao(aprovacao)" text-color="white">
                    {{ aprovacao.statusDescricao }}
                  </q-chip>
                  <q-chip
                    v-if="aprovacao.bloqueiaAvancoAtendimento"
                    dense
                    square
                    color="negative"
                    text-color="white"
                  >
                    Bloqueante
                  </q-chip>
                </div>
                <span class="text-caption text-grey-7">
                  Aprovador: {{ aprovacao.aprovadorNome || '-' }} | Solicitada em:
                  {{ formatarData(aprovacao.solicitadaEm) }}
                </span>
              </div>
            </div>
          </div>
        </section>
      </div>

      <section>
        <div class="text-subtitle2 text-weight-bold q-mb-sm">Chamados vinculados</div>

        <EmptyState
          v-if="!relacionamentos.length"
          titulo="Nenhum relacionamento registrado"
          mensagem="Nenhum relacionamento registrado para este chamado."
          icon="account_tree"
        />

        <div v-else class="relacionamentos-list">
          <article v-for="relacionamento in relacionamentos" :key="relacionamento.id" class="relacionamento-card">
            <div class="row items-start q-col-gutter-sm no-wrap">
              <div class="col-auto">
                <q-avatar :color="obterCorTipoRelacionamento(relacionamento.tipoRelacionamento)" text-color="white">
                  <q-icon :name="obterIconeTipoRelacionamento(relacionamento.tipoRelacionamento)" />
                </q-avatar>
              </div>

              <div class="col min-width-0">
                <div class="row items-center q-gutter-xs q-mb-xs">
                  <span class="text-subtitle2 text-weight-bold">
                    {{ obterChamadoRelacionadoCodigo(relacionamento, chamadoId) }}
                  </span>
                  <q-chip dense square :color="obterCorTipoRelacionamento(relacionamento.tipoRelacionamento)" text-color="white">
                    {{ relacionamento.tipoRelacionamentoDescricao }}
                  </q-chip>
                  <q-chip dense square :color="relacionamento.ativo ? 'positive' : 'grey-6'" text-color="white">
                    {{ relacionamento.ativo ? 'Ativo' : 'Inativo' }}
                  </q-chip>
                </div>

                <div class="text-body2 text-grey-9">
                  {{ obterDescricaoDirecionalRelacionamento(relacionamento, chamadoId) }}
                </div>

                <div class="row q-col-gutter-sm q-mt-sm relacionamento-meta">
                  <div class="col-12 col-sm-6">
                    <span class="text-caption text-grey-7">Criado em</span>
                    <div class="text-body2">{{ formatarData(relacionamento.criadoEm) }}</div>
                  </div>
                  <div class="col-12 col-sm-6">
                    <span class="text-caption text-grey-7">Criado por</span>
                    <div class="text-body2">{{ relacionamento.criadoPor || '-' }}</div>
                  </div>
                </div>

                <div v-if="relacionamento.justificativa" class="text-body2 text-grey-8 q-mt-sm">
                  Justificativa: {{ relacionamento.justificativa }}
                </div>

                <q-banner v-if="!relacionamento.ativo" dense rounded class="bg-grey-2 text-grey-9 q-mt-sm">
                  <div>Removido em: {{ formatarData(relacionamento.removidoEm) }}</div>
                  <div v-if="relacionamento.motivoRemocao">Motivo: {{ relacionamento.motivoRemocao }}</div>
                </q-banner>
              </div>

              <div class="col-auto relacionamento-card__actions">
                <q-btn
                  flat
                  dense
                  color="primary"
                  icon="open_in_new"
                  label="Abrir chamado"
                  @click="abrirChamadoRelacionado(relacionamento)"
                />
              </div>
            </div>
          </article>
        </div>
      </section>
    </div>
  </AppSectionCard>
</template>

<style scoped>
.relacionamentos-summary {
  display: grid;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  gap: 12px;
}

.relacionamentos-summary__item,
.relacionamentos-panel,
.relacionamento-card {
  border: 1px solid #dbe3ef;
  border-radius: 8px;
  background: #ffffff;
}

.relacionamentos-summary__item {
  min-height: 72px;
  padding: 12px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 4px;
}

.relacionamentos-summary__item strong {
  font-size: 1.35rem;
  line-height: 1;
  color: #1f2a44;
}

.relacionamentos-list {
  display: grid;
  gap: 10px;
}

.relacionamento-card {
  padding: 14px;
}

.relacionamento-meta {
  max-width: 720px;
}

.relacionamentos-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 12px;
}

.relacionamentos-panel {
  min-height: 160px;
  padding: 14px;
}

.mini-list {
  display: grid;
  gap: 8px;
}

.mini-list__item {
  display: flex;
  gap: 8px;
  align-items: flex-start;
  font-size: 0.875rem;
  color: #1f2a44;
}

.mini-list__item--stacked {
  display: grid;
  gap: 2px;
  padding-top: 4px;
}

@media (max-width: 900px) {
  .relacionamentos-summary,
  .relacionamentos-grid {
    grid-template-columns: minmax(0, 1fr);
  }

  .relacionamento-card > .row {
    flex-wrap: wrap;
  }

  .relacionamento-card__actions {
    width: 100%;
    padding-left: 52px;
  }
}
</style>
