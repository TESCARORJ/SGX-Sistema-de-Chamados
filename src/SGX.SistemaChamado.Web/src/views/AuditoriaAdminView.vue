<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { auditoriaService } from '../services/auditoriaService'
import { useAuthStore } from '../stores/authStore'
import type {
  AuditoriaAgrupamentoDiaResponse,
  AuditoriaAgrupamentoResponse,
  AuditoriaDashboardResponse,
  EventoAuditoriaDetalhe,
  EventoAuditoriaResumo,
  FiltroAuditoriaRequest,
  NivelAuditoria,
  ResultadoPaginadoEventoAuditoria,
  TipoAcaoAuditoria,
} from '../types/auditoria'

const authStore = useAuthStore()
const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const detalheAberto = ref(false)
const detalheLoading = ref(false)
const detalhe = ref<EventoAuditoriaDetalhe | null>(null)

const lista = ref<ResultadoPaginadoEventoAuditoria>({
  items: [],
  total: 0,
  pagina: 1,
  tamanhoPagina: 20,
})

const dashboard = ref<AuditoriaDashboardResponse | null>(null)

const filtros = reactive({
  dataInicio: '',
  dataFim: '',
  usuarioId: '',
  usuarioEmail: '',
  modulo: '',
  entidade: '',
  entidadeId: '',
  acao: '' as '' | TipoAcaoAuditoria,
  nivel: '' as '' | NivelAuditoria,
  sucesso: '' as '' | 'true' | 'false',
  ipOrigem: '',
  correlacaoId: '',
  texto: '',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeVisualizarAuditoria = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.auditoriaVisualizar)
)

const cardsResumo = computed(() => {
  if (!dashboard.value) {
    return []
  }

  return [
    { titulo: 'Total de eventos', valor: dashboard.value.totalEventos, icon: 'receipt_long', color: 'primary' },
    { titulo: 'Eventos crÃ­ticos', valor: dashboard.value.totalEventosCriticos, icon: 'priority_high', color: 'negative' },
    { titulo: 'Falhas', valor: dashboard.value.totalFalhas, icon: 'error_outline', color: 'warning' },
    { titulo: 'Sucessos', valor: dashboard.value.totalSucessos, icon: 'check_circle', color: 'positive' },
  ]
})
const totalPaginaAtual = computed(() => lista.value.items.length)

const opcoesAcao: Array<{ label: string; value: TipoAcaoAuditoria }> = [
  { label: 'Login', value: 'Login' },
  { label: 'Logout', value: 'Logout' },
  { label: 'CriaÃ§Ã£o', value: 'Criacao' },
  { label: 'EdiÃ§Ã£o', value: 'Edicao' },
  { label: 'ExclusÃ£o lÃ³gica', value: 'ExclusaoLogica' },
  { label: 'AtivaÃ§Ã£o', value: 'Ativacao' },
  { label: 'InativaÃ§Ã£o', value: 'Inativacao' },
  { label: 'AlteraÃ§Ã£o de status', value: 'AlteracaoStatus' },
  { label: 'AlteraÃ§Ã£o de permissÃ£o', value: 'AlteracaoPermissao' },
  { label: 'VisualizaÃ§Ã£o', value: 'Visualizacao' },
  { label: 'ExportaÃ§Ã£o', value: 'Exportacao' },
  { label: 'ImportaÃ§Ã£o', value: 'Importacao' },
  { label: 'Erro', value: 'Erro' },
  { label: 'ExecuÃ§Ã£o de job', value: 'ExecucaoJob' },
  { label: 'ConfiguraÃ§Ã£o', value: 'Configuracao' },
  { label: 'HomologaÃ§Ã£o', value: 'Homologacao' },
  { label: 'Outro', value: 'Outro' },
]

const opcoesNivel: Array<{ label: string; value: NivelAuditoria }> = [
  { label: 'InformaÃ§Ã£o', value: 'Informacao' },
  { label: 'Alerta', value: 'Alerta' },
  { label: 'CrÃ­tico', value: 'Critico' },
]

const colunasTabela = [
  { name: 'dataEvento', label: 'Data/hora', field: 'dataEvento', align: 'left' as const },
  { name: 'usuario', label: 'UsuÃ¡rio', field: 'usuarioEmail', align: 'left' as const },
  { name: 'modulo', label: 'MÃ³dulo', field: 'modulo', align: 'left' as const },
  { name: 'entidade', label: 'Entidade', field: 'entidade', align: 'left' as const },
  { name: 'acao', label: 'AÃ§Ã£o', field: 'acao', align: 'left' as const },
  { name: 'descricao', label: 'DescriÃ§Ã£o', field: 'descricao', align: 'left' as const },
  { name: 'nivel', label: 'NÃ­vel', field: 'nivel', align: 'left' as const },
  { name: 'sucesso', label: 'Sucesso', field: 'sucesso', align: 'left' as const },
  { name: 'ipOrigem', label: 'IP', field: 'ipOrigem', align: 'left' as const },
  { name: 'acoes', label: 'AÃ§Ãµes', field: 'id', align: 'right' as const },
]

function construirFiltroEventos(pagina?: number): FiltroAuditoriaRequest {
  return {
    dataInicio: filtros.dataInicio || undefined,
    dataFim: filtros.dataFim || undefined,
    usuarioId: filtros.usuarioId || undefined,
    usuarioEmail: filtros.usuarioEmail || undefined,
    modulo: filtros.modulo || undefined,
    entidade: filtros.entidade || undefined,
    entidadeId: filtros.entidadeId || undefined,
    acao: filtros.acao || undefined,
    nivel: filtros.nivel || undefined,
    sucesso: filtros.sucesso === '' ? undefined : filtros.sucesso === 'true',
    ipOrigem: filtros.ipOrigem || undefined,
    correlacaoId: filtros.correlacaoId || undefined,
    texto: filtros.texto || undefined,
    pagina: pagina ?? lista.value.pagina,
    tamanhoPagina: lista.value.tamanhoPagina,
  }
}

function formatarData(valor: string): string {
  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return '-'
  }

  return data.toLocaleString('pt-BR')
}

function corNivel(nivel: NivelAuditoria): string {
  if (nivel === 'Critico') return 'negative'
  if (nivel === 'Alerta') return 'warning'
  return 'primary'
}

function corSucesso(sucesso: boolean): string {
  return sucesso ? 'positive' : 'negative'
}

function formatarJson(valor: string | null | undefined): string {
  if (!valor) {
    return 'Sem dados registrados.'
  }

  try {
    const parsed = JSON.parse(valor)
    return JSON.stringify(parsed, null, 2)
  } catch {
    return valor
  }
}

async function carregarDashboard(): Promise<void> {
  dashboard.value = await auditoriaService.obterDashboard({
    dataInicio: filtros.dataInicio || undefined,
    dataFim: filtros.dataFim || undefined,
    modulo: filtros.modulo || undefined,
    usuarioEmail: filtros.usuarioEmail || undefined,
    nivel: filtros.nivel || undefined,
    sucesso: filtros.sucesso === '' ? undefined : filtros.sucesso === 'true',
  })
}

async function carregarEventos(pagina?: number): Promise<void> {
  lista.value = await auditoriaService.listarEventos(construirFiltroEventos(pagina))
}

async function carregarTudo(pagina?: number): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    await Promise.all([carregarDashboard(), carregarEventos(pagina)])
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar os eventos de auditoria.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  await carregarTudo(1)
}

async function limparFiltros(): Promise<void> {
  filtros.dataInicio = ''
  filtros.dataFim = ''
  filtros.usuarioId = ''
  filtros.usuarioEmail = ''
  filtros.modulo = ''
  filtros.entidade = ''
  filtros.entidadeId = ''
  filtros.acao = ''
  filtros.nivel = ''
  filtros.sucesso = ''
  filtros.ipOrigem = ''
  filtros.correlacaoId = ''
  filtros.texto = ''
  await carregarTudo(1)
}

async function mudarPagina(pagina: number): Promise<void> {
  await carregarTudo(pagina)
}

async function abrirDetalhe(item: EventoAuditoriaResumo): Promise<void> {
  detalheAberto.value = true
  detalheLoading.value = true
  detalhe.value = null

  try {
    detalhe.value = await auditoriaService.obterEvento(item.id)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'NÃ£o foi possÃ­vel carregar o detalhe do evento.'
  } finally {
    detalheLoading.value = false
  }
}

function irParaDocumentacao(): void {
  router.push('/admin/gestao-itsm/documentacao')
}

onMounted(() => {
  if (podeVisualizarAuditoria.value) {
    void carregarTudo(1)
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Governanca e rastreabilidade"
      titulo="Historico de Auditoria"
      subtitulo="Consulte trilhas de usuario, acoes, entidades e resultados operacionais do SGX."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" icon="refresh" label="Recarregar" :loading="loading" @click="carregarTudo(lista.pagina)" />
          <q-btn flat color="primary" icon="menu_book" label="Ver documentaÃ§Ã£o" @click="irParaDocumentacao" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizarAuditoria" rounded class="bg-orange-1 text-orange-10">
      VocÃª nÃ£o possui permissÃ£o para consultar auditoria.
    </q-banner>

    <template v-else>
      <div class="row q-col-gutter-md">
        <div v-for="card in cardsResumo" :key="card.titulo" class="col-12 col-sm-6 col-lg-3">
          <MetricCard :titulo="card.titulo" :valor="String(card.valor)" :icon="card.icon" :color="card.color" />
        </div>
        <div class="col-12 col-sm-6 col-lg-3">
          <MetricCard titulo="Pagina atual" :valor="totalPaginaAtual" icon="view_list" color="primary" :loading="loading" />
        </div>
      </div>

      <AppSectionCard titulo="Filtros avanÃ§ados" subtitulo="Combine perÃ­odo, usuÃ¡rio, contexto e resultado para refinar a auditoria.">
        <FilterBar compact>
          <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltros">
          <div class="col-12 col-md-2"><q-input v-model="filtros.dataInicio" type="date" outlined dense label="Data inicial" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.dataFim" type="date" outlined dense label="Data final" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.usuarioEmail" outlined dense label="UsuÃ¡rio/e-mail" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.usuarioId" outlined dense label="UsuÃ¡rio ID" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.modulo" outlined dense label="MÃ³dulo" /></div>

          <div class="col-12 col-md-3"><q-input v-model="filtros.entidade" outlined dense label="Entidade" /></div>
          <div class="col-12 col-md-2"><q-input v-model="filtros.entidadeId" outlined dense label="Entidade ID" /></div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtros.acao"
              outlined
              dense
              clearable
              emit-value
              map-options
              label="AÃ§Ã£o"
              :options="opcoesAcao"
            />
          </div>
          <div class="col-12 col-md-2">
            <q-select
              v-model="filtros.nivel"
              outlined
              dense
              clearable
              emit-value
              map-options
              label="NÃ­vel"
              :options="opcoesNivel"
            />
          </div>
          <div class="col-12 col-md-3">
            <q-select
              v-model="filtros.sucesso"
              outlined
              dense
              clearable
              emit-value
              map-options
              label="Sucesso/Falha"
              :options="[
                { label: 'Sucesso', value: 'true' },
                { label: 'Falha', value: 'false' },
              ]"
            />
          </div>

          <div class="col-12 col-md-3"><q-input v-model="filtros.ipOrigem" outlined dense label="IP de origem" /></div>
          <div class="col-12 col-md-3"><q-input v-model="filtros.correlacaoId" outlined dense label="CorrelaÃ§Ã£o" /></div>
          <div class="col-12 col-md-6"><q-input v-model="filtros.texto" outlined dense label="Texto" /></div>

          <div class="col-12 row justify-end q-gutter-sm">
            <q-btn flat label="Limpar filtros" :disable="loading" @click="limparFiltros" />
            <q-btn color="primary" icon="search" label="Filtrar" type="submit" :loading="loading" />
          </div>
          </q-form>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregarTudo(lista.pagina)" />
      <LoadingState v-else-if="loading && !lista.items.length" inline mensagem="Carregando histÃ³rico de auditoria..." />

      <AppSectionCard v-else-if="lista.items.length" titulo="Eventos auditÃ¡veis" subtitulo="Listagem paginada dos eventos mais recentes.">
        <q-table
          flat
          :rows="lista.items"
          :columns="colunasTabela"
          row-key="id"
          hide-pagination
        >
          <template #body-cell-dataEvento="props">
            <q-td :props="props">{{ formatarData(props.row.dataEvento) }}</q-td>
          </template>

          <template #body-cell-usuario="props">
            <q-td :props="props">
              <div class="text-body2">{{ props.row.usuarioNome || '-' }}</div>
              <div class="text-caption text-grey-7">{{ props.row.usuarioEmail || '-' }}</div>
            </q-td>
          </template>

          <template #body-cell-modulo="props">
            <q-td :props="props">
              <q-chip dense color="blue-1" text-color="blue-9">{{ props.row.modulo }}</q-chip>
            </q-td>
          </template>

          <template #body-cell-entidade="props">
            <q-td :props="props">
              <div>{{ props.row.entidade }}</div>
              <div class="text-caption text-grey-7">{{ props.row.entidadeId || '-' }}</div>
            </q-td>
          </template>

          <template #body-cell-acao="props">
            <q-td :props="props">
              <q-chip dense color="grey-2" text-color="grey-9">{{ props.row.acao }}</q-chip>
            </q-td>
          </template>

          <template #body-cell-nivel="props">
            <q-td :props="props">
              <q-chip dense :color="corNivel(props.row.nivel)" text-color="white">{{ props.row.nivel }}</q-chip>
            </q-td>
          </template>

          <template #body-cell-sucesso="props">
            <q-td :props="props">
              <q-chip dense :color="corSucesso(props.row.sucesso)" text-color="white">
                {{ props.row.sucesso ? 'Sucesso' : 'Falha' }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-acoes="props">
            <q-td :props="props" class="text-right">
              <q-btn flat color="primary" icon="visibility" label="Ver detalhe" @click="abrirDetalhe(props.row)" />
            </q-td>
          </template>
        </q-table>

        <div class="row items-center justify-between q-mt-md">
          <div class="text-caption text-grey-7">Total: {{ lista.total }} eventos</div>
          <q-pagination
            :model-value="lista.pagina"
            :max="Math.max(1, Math.ceil(lista.total / lista.tamanhoPagina))"
            max-pages="8"
            boundary-links
            direction-links
            @update:model-value="mudarPagina"
          />
        </div>
      </AppSectionCard>

      <EmptyState
        v-else
        titulo="Sem eventos de auditoria"
        mensagem="Ainda nÃ£o hÃ¡ eventos para os filtros informados."
        icon="manage_search"
      />

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Eventos por mÃ³dulo">
            <EmptyState
              v-if="!(dashboard?.eventosPorModulo?.length ?? 0)"
              titulo="Sem dados por modulo"
              mensagem="Nenhum agrupamento disponivel para os filtros atuais."
              icon="view_module"
            />
            <q-list v-else separator dense>
              <q-item v-for="item in (dashboard?.eventosPorModulo ?? []) as AuditoriaAgrupamentoResponse[]" :key="`mod-${item.chave}`">
                <q-item-section>{{ item.chave }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Eventos por aÃ§Ã£o">
            <EmptyState
              v-if="!(dashboard?.eventosPorAcao?.length ?? 0)"
              titulo="Sem dados por acao"
              mensagem="Nao ha agrupamentos por acao para os filtros atuais."
              icon="bolt"
            />
            <q-list v-else separator dense>
              <q-item v-for="item in (dashboard?.eventosPorAcao ?? []) as AuditoriaAgrupamentoResponse[]" :key="`acao-${item.chave}`">
                <q-item-section>{{ item.chave }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Eventos por usuÃ¡rio">
            <EmptyState
              v-if="!(dashboard?.eventosPorUsuario?.length ?? 0)"
              titulo="Sem dados por usuario"
              mensagem="Nenhum agrupamento por usuario para o recorte selecionado."
              icon="person_search"
            />
            <q-list v-else separator dense>
              <q-item v-for="item in (dashboard?.eventosPorUsuario ?? []) as AuditoriaAgrupamentoResponse[]" :key="`usr-${item.chave}`">
                <q-item-section>{{ item.chave }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Eventos por dia">
            <EmptyState
              v-if="!(dashboard?.eventosPorDia?.length ?? 0)"
              titulo="Sem dados por dia"
              mensagem="Nao ha consolidacao diaria para os filtros atuais."
              icon="calendar_today"
            />
            <q-list v-else separator dense>
              <q-item v-for="item in (dashboard?.eventosPorDia ?? []) as AuditoriaAgrupamentoDiaResponse[]" :key="`dia-${item.dia}`">
                <q-item-section>{{ formatarData(item.dia) }}</q-item-section>
                <q-item-section side>{{ item.total }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
        <div class="col-12 col-lg-4">
          <AppSectionCard titulo="Ãšltimas falhas">
            <EmptyState
              v-if="!(dashboard?.ultimasFalhas?.length ?? 0)"
              titulo="Sem falhas recentes"
              mensagem="Nao foram registradas falhas no periodo filtrado."
              icon="task_alt"
            />
            <q-list v-else separator dense>
              <q-item v-for="item in dashboard?.ultimasFalhas ?? []" :key="`falha-${item.id}`">
                <q-item-section>
                  <q-item-label>{{ item.modulo }} / {{ item.acao }}</q-item-label>
                  <q-item-label caption>{{ item.descricao }}</q-item-label>
                </q-item-section>
                <q-item-section side>{{ formatarData(item.dataEvento) }}</q-item-section>
              </q-item>
            </q-list>
          </AppSectionCard>
        </div>
      </div>

      <AppSectionCard titulo="Ãšltimos eventos crÃ­ticos">
        <EmptyState
          v-if="!(dashboard?.ultimosEventosCriticos?.length ?? 0)"
          titulo="Sem eventos criticos"
          mensagem="Nao houve ocorrencias criticas nos filtros selecionados."
          icon="verified"
        />
        <q-list v-else separator dense>
          <q-item v-for="item in dashboard?.ultimosEventosCriticos ?? []" :key="`crit-${item.id}`">
            <q-item-section>
              <q-item-label>{{ item.modulo }} / {{ item.acao }}</q-item-label>
              <q-item-label caption>{{ item.descricao }}</q-item-label>
            </q-item-section>
            <q-item-section side>{{ formatarData(item.dataEvento) }}</q-item-section>
          </q-item>
        </q-list>
      </AppSectionCard>
    </template>

    <q-drawer
      v-model="detalheAberto"
      side="right"
      overlay
      bordered
      :width="520"
      content-class="auditoria-detalhe-drawer"
    >
      <div class="q-pa-md">
        <div class="text-h6 q-mb-sm">Detalhe do evento</div>
        <LoadingState v-if="detalheLoading" inline mensagem="Carregando detalhe..." />
        <template v-else-if="detalhe">
          <q-list dense bordered separator>
            <q-item><q-item-section>Data/hora</q-item-section><q-item-section side>{{ formatarData(detalhe.dataEvento) }}</q-item-section></q-item>
            <q-item><q-item-section>UsuÃ¡rio</q-item-section><q-item-section side>{{ detalhe.usuarioNome || '-' }}</q-item-section></q-item>
            <q-item><q-item-section>E-mail</q-item-section><q-item-section side>{{ detalhe.usuarioEmail || '-' }}</q-item-section></q-item>
            <q-item><q-item-section>Login</q-item-section><q-item-section side>{{ detalhe.usuarioLogin || '-' }}</q-item-section></q-item>
            <q-item><q-item-section>IP</q-item-section><q-item-section side>{{ detalhe.ipOrigem || '-' }}</q-item-section></q-item>
            <q-item><q-item-section>MÃ³dulo</q-item-section><q-item-section side>{{ detalhe.modulo }}</q-item-section></q-item>
            <q-item><q-item-section>Entidade</q-item-section><q-item-section side>{{ detalhe.entidade }}</q-item-section></q-item>
            <q-item><q-item-section>Entidade ID</q-item-section><q-item-section side>{{ detalhe.entidadeId || '-' }}</q-item-section></q-item>
            <q-item><q-item-section>AÃ§Ã£o</q-item-section><q-item-section side>{{ detalhe.acao }}</q-item-section></q-item>
            <q-item><q-item-section>NÃ­vel</q-item-section><q-item-section side>{{ detalhe.nivel }}</q-item-section></q-item>
            <q-item><q-item-section>Sucesso</q-item-section><q-item-section side>{{ detalhe.sucesso ? 'Sim' : 'NÃ£o' }}</q-item-section></q-item>
            <q-item><q-item-section>CorrelaÃ§Ã£o</q-item-section><q-item-section side>{{ detalhe.correlacaoId || '-' }}</q-item-section></q-item>
          </q-list>

          <div class="q-mt-md">
            <div class="text-subtitle2">DescriÃ§Ã£o</div>
            <q-card flat bordered class="q-pa-sm q-mt-xs">{{ detalhe.descricao }}</q-card>
          </div>

          <div class="q-mt-md">
            <div class="text-subtitle2">User-Agent</div>
            <q-card flat bordered class="q-pa-sm q-mt-xs">{{ detalhe.userAgent || 'Sem dados registrados.' }}</q-card>
          </div>

          <div class="q-mt-md">
            <div class="text-subtitle2">Mensagem de erro</div>
            <q-card flat bordered class="q-pa-sm q-mt-xs">{{ detalhe.mensagemErro || 'Sem dados registrados.' }}</q-card>
          </div>

          <div class="q-mt-md">
            <div class="text-subtitle2">Dados antes</div>
            <pre class="auditoria-json">{{ formatarJson(detalhe.dadosAntes) }}</pre>
          </div>

          <div class="q-mt-md">
            <div class="text-subtitle2">Dados depois</div>
            <pre class="auditoria-json">{{ formatarJson(detalhe.dadosDepois) }}</pre>
          </div>

          <div class="q-mt-md">
            <div class="text-subtitle2">Metadados</div>
            <pre class="auditoria-json">{{ formatarJson(detalhe.metadados) }}</pre>
          </div>
        </template>
      </div>
    </q-drawer>
  </q-page>
</template>

<style scoped>
.auditoria-json {
  margin: 6px 0 0;
  padding: 10px;
  border: 1px solid var(--sgx-border);
  border-radius: 8px;
  background: #f8fafc;
  color: #0f172a;
  max-height: 280px;
  overflow: auto;
  white-space: pre-wrap;
  word-break: break-word;
  font-size: 0.8rem;
  line-height: 1.35;
}

:deep(.auditoria-detalhe-drawer) {
  background: #ffffff;
}
</style>


