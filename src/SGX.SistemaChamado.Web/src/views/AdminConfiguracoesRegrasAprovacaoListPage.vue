<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'

import CampoAtivoInativo from '../components/admin/cadastros/CampoAtivoInativo.vue'
import CampoBuscaCadastro from '../components/admin/cadastros/CampoBuscaCadastro.vue'
import PaginacaoTabela from '../components/admin/cadastros/PaginacaoTabela.vue'
import TabelaAdministrativa from '../components/admin/cadastros/TabelaAdministrativa.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'

import { permissoes } from '../constants/permissoes'
import { configuracoesRegrasAprovacaoService } from '../services/configuracoesRegrasAprovacaoService'
import { useAuthStore } from '../stores/authStore'
import type { ConfiguracaoRegraAprovacaoResumoResponse } from '../types/aprovacoesMotor'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)

// Filtros
const texto = ref('')
const filtroAtivo = ref<'todos' | 'ativos' | 'inativos'>('ativos')
const pagina = ref(1)
const tamanhoPagina = ref(20)
const total = ref(0)

const rows = ref<ConfiguracaoRegraAprovacaoResumoResponse[]>([])
const dialogoSituacaoAberto = ref(false)
const atualizandoSituacao = ref(false)
const registroSelecionado = ref<{ id: string; ativo: boolean; nome: string } | null>(null)

const temRegistros = computed(() => rows.value.length > 0)
const filtrosAplicados = computed(() => Boolean(texto.value.trim()) || filtroAtivo.value !== 'ativos')
const totalAtivos = computed(() => rows.value.filter((r) => r.ativo).length)
const totalInativos = computed(() => rows.value.length - totalAtivos.value)

const podeCriar = computed(() => authStore.possuiPermissao(permissoes.aprovacaoChamadosGerenciar))
const podeDetalhar = computed(() =>
  authStore.possuiAlgumaPermissao([permissoes.aprovacaoChamadosVisualizar, permissoes.aprovacaoChamadosGerenciar])
)
const podeAlterarSituacao = computed(() => authStore.possuiPermissao(permissoes.aprovacaoChamadosGerenciar))

const colunas: QTableColumn[] = [
  { name: 'nome', label: 'Nome da Regra', field: 'nome', align: 'left', sortable: true },
  { name: 'tipoRegraDescricao', label: 'Tipo', field: 'tipoRegraDescricao', align: 'left' },
  { name: 'escopoRegraDescricao', label: 'Escopo', field: 'escopoRegraDescricao', align: 'left' },
  { name: 'efeitoOperacionalDescricao', label: 'Efeito', field: 'efeitoOperacionalDescricao', align: 'center' },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'center', sortable: true },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await configuracoesRegrasAprovacaoService.listar({
      termo: texto.value || undefined,
      ativo: filtroAtivo.value === 'todos' ? undefined : filtroAtivo.value === 'ativos',
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    })
    rows.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  } finally {
    loading.value = false
  }
}

function aplicarFiltros(): void {
  pagina.value = 1
  void carregar()
}

function limparFiltros(): void {
  texto.value = ''
  filtroAtivo.value = 'ativos'
  pagina.value = 1
  void carregar()
}

function atualizarPagina(value: number): void {
  pagina.value = value
  void carregar()
}

function atualizarTamanhoPagina(value: number): void {
  tamanhoPagina.value = value
  pagina.value = 1
  void carregar()
}

function abrirDetalhe(row: ConfiguracaoRegraAprovacaoResumoResponse): void {
  router.push(`/admin/configuracoes/regras-aprovacao/${row.id}`)
}

function novo(): void {
  router.push('/admin/configuracoes/regras-aprovacao/nova')
}

function abrirConfirmacaoSituacao(row: ConfiguracaoRegraAprovacaoResumoResponse): void {
  if (!podeAlterarSituacao.value) return
  registroSelecionado.value = { id: row.id, ativo: row.ativo, nome: row.nome }
  dialogoSituacaoAberto.value = true
}

async function alterarSituacao(): Promise<void> {
  if (!registroSelecionado.value) return

  atualizandoSituacao.value = true
  erro.value = null

  try {
    const { id, ativo } = registroSelecionado.value
    await configuracoesRegrasAprovacaoService.atualizarStatus(id, { ativo: !ativo })

    $q.notify({
      type: 'positive',
      message: ativo ? 'Regra inativada com sucesso.' : 'Regra reativada com sucesso.',
    })
    dialogoSituacaoAberto.value = false
    registroSelecionado.value = null
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível alterar a situação.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    atualizandoSituacao.value = false
  }
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md regra-aprovacao-lista">
    <PageHeader
      titulo="Regras de Aprovação"
      contexto="Configurações Administrativas"
      subtitulo="Gerenciamento das políticas e configurações do motor de aprovações"
    >
      <template #actions>
        <q-btn
          v-if="podeCriar"
          color="primary"
          icon="add"
          label="Nova regra"
          unelevated
          :disable="loading"
          @click="novo"
        />
      </template>
    </PageHeader>

    <div class="regra-aprovacao-lista__kpis">
      <MetricCard
        titulo="Registros na página"
        :valor="rows.length"
        subtitulo="Total exibido após filtros"
        icon="summarize"
      />
      <MetricCard
        titulo="Ativos"
        :valor="totalAtivos"
        subtitulo="Regras ativas na listagem"
        icon="check_circle"
        tone="positive"
      />
      <MetricCard
        titulo="Inativos"
        :valor="totalInativos"
        subtitulo="Regras inativas na listagem"
        icon="pause_circle"
        tone="warning"
      />
    </div>

    <AppSectionCard titulo="Filtros" subtitulo="Refine por texto e situação para localizar regras">
      <FilterBar compact>
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-7">
              <CampoBuscaCadastro v-model="texto" :loading="loading" />
            </div>
            <div class="col-12 col-md-5">
              <CampoAtivoInativo v-model="filtroAtivo" :loading="loading" />
            </div>
          </div>
          <div class="row justify-end q-gutter-sm regra-aprovacao-lista__filtro-acoes">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" unelevated />
            <q-btn flat label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </FilterBar>
    </AppSectionCard>

    <LoadingState v-if="loading && !temRegistros" mensagem="Carregando listagem de regras..." />

    <q-banner v-else-if="!podeDetalhar" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar as regras de aprovação.
    </q-banner>

    <ErrorState
      v-else-if="erro && !temRegistros"
      titulo="Não foi possível carregar as regras"
      :mensagem="erro"
      @retry="carregar"
    />

    <EmptyState
      v-else-if="!temRegistros"
      titulo="Nenhuma regra encontrada"
      mensagem="Nenhum resultado corresponde aos filtros aplicados."
      icon="search_off"
    >
      <template #actions>
        <q-btn
          v-if="filtrosAplicados"
          flat
          color="primary"
          icon="filter_alt_off"
          label="Limpar filtros"
          @click="limparFiltros"
        />
      </template>
    </EmptyState>

    <AppSectionCard v-else titulo="Regras de Aprovação" subtitulo="Listagem de políticas do motor">
      <q-banner v-if="erro && temRegistros" rounded class="bg-red-1 text-negative q-mb-md">
        {{ erro }}
      </q-banner>

      <TabelaAdministrativa titulo="Regras de Aprovação" :rows="rows" :columns="colunas" :loading="loading">
        <template #body-cell-efeitoOperacionalDescricao="props">
          <q-td :props="props">
            <q-chip
              dense
              outline
              :color="props.row.bloqueante ? 'negative' : 'info'"
              :label="props.row.efeitoOperacionalDescricao"
            />
          </q-td>
        </template>

        <template #acoes="{ row }">
          <q-btn
            v-if="podeDetalhar"
            flat
            round
            dense
            color="primary"
            icon="edit"
            aria-label="Visualizar ou editar regra"
            @click="abrirDetalhe(row)"
          >
            <q-tooltip>Visualizar ou editar</q-tooltip>
          </q-btn>
          <q-btn
            v-if="podeAlterarSituacao"
            flat
            round
            dense
            :icon="row.ativo ? 'block' : 'check_circle'"
            :color="row.ativo ? 'negative' : 'positive'"
            :aria-label="row.ativo ? 'Inativar regra' : 'Reativar regra'"
            @click="abrirConfirmacaoSituacao(row)"
          >
            <q-tooltip>{{ row.ativo ? 'Inativar regra' : 'Reativar regra' }}</q-tooltip>
          </q-btn>
        </template>
      </TabelaAdministrativa>

      <q-separator class="q-my-md" />

      <PaginacaoTabela
        :pagina="pagina"
        :tamanho-pagina="tamanhoPagina"
        :total="total"
        :loading="loading"
        @update:pagina="atualizarPagina"
        @update:tamanho-pagina="atualizarTamanhoPagina"
      />
    </AppSectionCard>

    <ConfirmDialog
      v-model="dialogoSituacaoAberto"
      :titulo="registroSelecionado?.ativo ? 'Confirmar inativação' : 'Confirmar reativação'"
      :mensagem="registroSelecionado?.ativo ? `Deseja realmente inativar a regra '${registroSelecionado?.nome}'?` : `Deseja realmente reativar a regra '${registroSelecionado?.nome}'?`"
      :color="registroSelecionado?.ativo ? 'negative' : 'positive'"
      :confirmar-label="registroSelecionado?.ativo ? 'Inativar' : 'Reativar'"
      :loading="atualizandoSituacao"
      @confirm="alterarSituacao"
    />
  </q-page>
</template>

<style scoped>
.regra-aprovacao-lista__kpis {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

.regra-aprovacao-lista__filtro-acoes {
  margin-top: 4px;
}

@media (max-width: 1024px) {
  .regra-aprovacao-lista__kpis {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }
}

@media (max-width: 768px) {
  .regra-aprovacao-lista__kpis {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
