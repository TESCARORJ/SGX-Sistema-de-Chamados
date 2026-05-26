<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { auditoriaAutenticacaoService } from '../services/auditoriaAutenticacaoService'
import { useAuthStore } from '../stores/authStore'
import type {
  FiltroAuditoriaAutenticacaoRequest,
  ListaEventosAuditoriaAutenticacaoResponse,
  ProvedorAutenticacao,
  ResultadoEventoAutenticacao,
  TipoEventoAutenticacao,
} from '../types/auditoriaAutenticacao'

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)

const lista = ref<ListaEventosAuditoriaAutenticacaoResponse>({
  items: [],
  total: 0,
  pagina: 1,
  tamanhoPagina: 20,
})

const filtros = reactive({
  dataInicio: '',
  dataFim: '',
  usuarioEmail: '',
  provedor: '' as '' | ProvedorAutenticacao,
  resultadoAutenticacao: '' as '' | ResultadoEventoAutenticacao,
  tipoEventoAutenticacao: '' as '' | TipoEventoAutenticacao,
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeVisualizar = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.auditoriaAutenticacaoVisualizar)
)

const opcoesProvedor = [
  { label: 'Microsoft Entra ID', value: 'MicrosoftEntraId' },
  { label: 'Active Directory', value: 'ActiveDirectory' },
  { label: 'Local SGX', value: 'LocalSgx' },
  { label: 'Local Development', value: 'LocalDevelopment' },
  { label: 'Nao informado', value: 'NaoInformado' },
]

const opcoesResultado = [
  { label: 'Sucesso', value: 'Sucesso' },
  { label: 'Falha', value: 'Falha' },
  { label: 'Bloqueado', value: 'Bloqueado' },
  { label: 'Negado', value: 'Negado' },
]

const opcoesTipoEvento = [
  'LoginLocalSgxSucesso',
  'LoginLocalSgxNegado',
  'LoginActiveDirectorySucesso',
  'LoginActiveDirectoryNegado',
  'LoginMicrosoftEntraIdSucesso',
  'UsuarioInativoBloqueado',
  'ProvedorDesabilitadoTentativaLogin',
  'FalhaConfiguracaoProvedor',
  'FalhaCredencialInvalida',
  'AutoProvisionamentoUsuario',
  'TrocaObrigatoriaSenhaConcluida',
  'RecuperacaoSenhaSolicitada',
  'RedefinicaoSenhaConcluida',
  'AlteracaoProvedorHabilitado',
  'AlteracaoProvedorPrincipal',
  'AlteracaoOrdemExibicao',
  'AlteracaoAutoProvisionamento',
  'AlteracaoPerfilPadraoProvisionamento',
  'TentativaNegadaAlteracaoMetodosLogin',
  'BloqueioConfiguracaoInsegura',
  'AlteracaoRotuloExibicao',
].map((value) => ({ label: value, value }))

const colunasTabela = [
  { name: 'dataEvento', label: 'Data/hora', field: 'dataEvento', align: 'left' as const },
  { name: 'usuario', label: 'Usuario', field: 'usuarioEmail', align: 'left' as const },
  { name: 'provedor', label: 'Provedor', field: 'provedor', align: 'left' as const },
  { name: 'tipoEvento', label: 'Tipo de evento', field: 'tipoEvento', align: 'left' as const },
  { name: 'resultado', label: 'Resultado', field: 'resultado', align: 'left' as const },
  { name: 'ipOrigem', label: 'IP de origem', field: 'ipOrigem', align: 'left' as const },
  { name: 'mensagem', label: 'Mensagem segura', field: 'mensagem', align: 'left' as const },
]

function construirFiltro(pagina?: number): FiltroAuditoriaAutenticacaoRequest {
  return {
    dataInicio: filtros.dataInicio || undefined,
    dataFim: filtros.dataFim || undefined,
    usuarioEmail: filtros.usuarioEmail || undefined,
    provedor: filtros.provedor || undefined,
    resultadoAutenticacao: filtros.resultadoAutenticacao || undefined,
    tipoEventoAutenticacao: filtros.tipoEventoAutenticacao || undefined,
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

function corResultado(resultado: string): string {
  if (resultado === 'Sucesso') return 'positive'
  if (resultado === 'Falha') return 'negative'
  if (resultado === 'Bloqueado') return 'warning'
  if (resultado === 'Negado') return 'warning'
  return 'grey-7'
}

function corProvedor(provedor: string): string {
  if (provedor === 'LocalSgx') return 'blue-2'
  if (provedor === 'ActiveDirectory') return 'orange-2'
  if (provedor === 'MicrosoftEntraId') return 'indigo-2'
  if (provedor === 'LocalDevelopment') return 'green-2'
  return 'grey-3'
}

async function carregar(pagina?: number): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null

  try {
    lista.value = await auditoriaAutenticacaoService.listarEventos(construirFiltro(pagina))
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar a auditoria de autenticacao.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  await carregar(1)
}

async function limparFiltros(): Promise<void> {
  filtros.dataInicio = ''
  filtros.dataFim = ''
  filtros.usuarioEmail = ''
  filtros.provedor = ''
  filtros.resultadoAutenticacao = ''
  filtros.tipoEventoAutenticacao = ''
  await carregar(1)
}

async function mudarPagina(pagina: number): Promise<void> {
  await carregar(pagina)
}

onMounted(() => {
  if (podeVisualizar.value) {
    void carregar(1)
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Seguranca e governanca"
      titulo="Auditoria de Autenticacao"
      subtitulo="Consulte evidencias de login, bloqueios, falhas e alteracoes dos metodos de login."
    >
      <template #actions>
        <q-btn color="primary" icon="refresh" label="Recarregar" :loading="loading" @click="carregar(lista.pagina)" />
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar auditoria de autenticacao.
    </q-banner>

    <template v-else>
      <AppSectionCard
        titulo="Filtros"
        subtitulo="Refine por periodo, provedor, resultado, tipo de evento e usuario/e-mail."
      >
        <FilterBar compact>
          <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltros">
            <div class="col-12 col-md-2">
              <q-input v-model="filtros.dataInicio" type="date" outlined dense label="Data inicial" />
            </div>
            <div class="col-12 col-md-2">
              <q-input v-model="filtros.dataFim" type="date" outlined dense label="Data final" />
            </div>
            <div class="col-12 col-md-3">
              <q-select
                v-model="filtros.provedor"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Provedor"
                :options="opcoesProvedor"
              />
            </div>
            <div class="col-12 col-md-2">
              <q-select
                v-model="filtros.resultadoAutenticacao"
                outlined
                dense
                clearable
                emit-value
                map-options
                label="Resultado"
                :options="opcoesResultado"
              />
            </div>
            <div class="col-12 col-md-3">
              <q-input v-model="filtros.usuarioEmail" outlined dense label="Usuario/e-mail" />
            </div>
            <div class="col-12 col-md-6">
              <q-select
                v-model="filtros.tipoEventoAutenticacao"
                outlined
                dense
                clearable
                emit-value
                map-options
                use-input
                fill-input
                input-debounce="0"
                label="Tipo de evento"
                :options="opcoesTipoEvento"
              />
            </div>
            <div class="col-12 row justify-end q-gutter-sm">
              <q-btn flat color="primary" label="Limpar filtros" :disable="loading" @click="limparFiltros" />
              <q-btn color="primary" icon="search" label="Filtrar" type="submit" :loading="loading" />
            </div>
          </q-form>
        </FilterBar>
      </AppSectionCard>

      <ErrorState v-if="erro" :mensagem="erro" @retry="carregar(lista.pagina)" />
      <LoadingState
        v-else-if="loading && !lista.items.length"
        inline
        mensagem="Carregando auditoria de autenticacao..."
      />

      <AppSectionCard
        v-else-if="lista.items.length"
        titulo="Eventos de autenticacao"
        subtitulo="Eventos mais recentes primeiro."
      >
        <q-table
          flat
          bordered
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

          <template #body-cell-provedor="props">
            <q-td :props="props">
              <q-chip dense :color="corProvedor(props.row.provedor)" text-color="grey-10">
                {{ props.row.provedor }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-tipoEvento="props">
            <q-td :props="props">
              <span class="text-caption">{{ props.row.tipoEvento }}</span>
            </q-td>
          </template>

          <template #body-cell-resultado="props">
            <q-td :props="props">
              <q-chip dense :color="corResultado(props.row.resultado)" text-color="white">
                {{ props.row.resultado }}
              </q-chip>
            </q-td>
          </template>

          <template #body-cell-ipOrigem="props">
            <q-td :props="props">
              <code>{{ props.row.ipOrigem || '-' }}</code>
            </q-td>
          </template>

          <template #body-cell-mensagem="props">
            <q-td :props="props">
              <span class="mensagem-segura">{{ props.row.mensagem }}</span>
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
        titulo="Sem eventos de autenticacao"
        mensagem="Nao ha eventos para os filtros informados."
        icon="manage_search"
      />
    </template>
  </q-page>
</template>

<style scoped>
.mensagem-segura {
  display: inline-block;
  max-width: 480px;
  white-space: normal;
  word-break: break-word;
}
</style>
