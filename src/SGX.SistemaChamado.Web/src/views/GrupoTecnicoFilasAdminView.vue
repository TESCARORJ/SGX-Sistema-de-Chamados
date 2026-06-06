<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { adminService } from '../services/adminService'
import { useAuthStore } from '../stores/authStore'
import type { FilaAtendimentoGrupoTecnicoResponse, GrupoTecnicoDetalhe } from '../types/admin'

type FiltroStatus = 'todos' | 'ativos' | 'inativos'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const grupo = ref<GrupoTecnicoDetalhe | null>(null)
const filas = ref<FilaAtendimentoGrupoTecnicoResponse[]>([])
const loading = ref(false)
const erro = ref<string | null>(null)
const filtroStatus = ref<FiltroStatus>('ativos')
const busca = ref('')

const grupoTecnicoId = computed(() => String(route.params.id ?? ''))
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const usuarioEhAtendente = computed(() => (authStore.usuario?.perfis ?? []).includes('Atendente'))
const podeVisualizar = computed(() => usuarioEhAdministrador.value || usuarioEhAtendente.value)
const totalAtivas = computed(() => filas.value.filter((fila) => fila.ativo).length)
const totalInativas = computed(() => filas.value.length - totalAtivas.value)
const possuiFiltro = computed(() => Boolean(busca.value.trim()) || filtroStatus.value !== 'ativos')

const colunas: QTableColumn<FilaAtendimentoGrupoTecnicoResponse>[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'descricao', label: 'Descricao', field: 'descricao', align: 'left' },
  { name: 'grupo', label: 'Grupo tecnico', field: 'grupoTecnicoId', align: 'left' },
  { name: 'ativo', label: 'Status', field: 'ativo', align: 'center', sortable: true },
]

function ativoFiltro(): boolean | undefined {
  if (filtroStatus.value === 'ativos') return true
  if (filtroStatus.value === 'inativos') return false
  return undefined
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    erro.value = 'Sem permissao para visualizar filas de grupos tecnicos.'
    return
  }

  loading.value = true
  erro.value = null

  try {
    const [grupoResponse, filasResponse] = await Promise.all([
      adminService.obterGrupoTecnico(grupoTecnicoId.value),
      adminService.listarFilasAtendimentoGrupoTecnico(grupoTecnicoId.value, {
        ativo: ativoFiltro(),
        busca: busca.value.trim() || undefined,
      }),
    ])

    grupo.value = grupoResponse
    filas.value = filasResponse
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar as filas do grupo tecnico.'
  } finally {
    loading.value = false
  }
}

function aplicarFiltros(): void {
  void carregar()
}

function limparFiltros(): void {
  busca.value = ''
  filtroStatus.value = 'ativos'
  void carregar()
}

function voltar(): void {
  router.push('/admin/cadastros/grupos-tecnicos')
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md grupo-filas-admin">
    <PageHeader
      contexto="Cadastros administrativos"
      titulo="Filas do grupo tecnico"
      :subtitulo="grupo ? `${grupo.nome} - ${grupo.ativo ? 'Ativo' : 'Inativo'}` : 'Grupo tecnico'"
    >
      <template #actions>
        <q-btn flat icon="arrow_back" label="Voltar" :disable="loading" @click="voltar" />
      </template>
    </PageHeader>

    <div class="grupo-filas-admin__kpis">
      <MetricCard titulo="Filas listadas" :valor="filas.length" icon="list_alt" :loading="loading" />
      <MetricCard titulo="Ativas" :valor="totalAtivas" icon="check_circle" tone="positive" :loading="loading" />
      <MetricCard titulo="Inativas" :valor="totalInativas" icon="pause_circle" tone="warning" :loading="loading" />
    </div>

    <AppSectionCard titulo="Identificacao do grupo" subtitulo="Dados principais do grupo tecnico." icon="groups">
      <div v-if="grupo" class="row q-col-gutter-md">
        <div class="col-12 col-md-7">
          <div class="text-caption text-grey-7">Nome</div>
          <div class="text-subtitle1 text-weight-medium">{{ grupo.nome }}</div>
          <div class="text-body2 text-grey-7">{{ grupo.descricao || 'Sem descricao cadastrada.' }}</div>
        </div>
        <div class="col-12 col-md-5">
          <div class="text-caption text-grey-7">Status</div>
          <q-badge :color="grupo.ativo ? 'positive' : 'grey-7'">
            {{ grupo.ativo ? 'Ativo' : 'Inativo' }}
          </q-badge>
        </div>
      </div>
      <q-skeleton v-else type="text" />
    </AppSectionCard>

    <AppSectionCard titulo="Filtros" subtitulo="Localize filas por texto e situacao." icon="filter_alt">
      <FilterBar compact>
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-7">
              <q-input v-model="busca" outlined dense label="Buscar por nome ou descricao" :disable="loading">
                <template #prepend>
                  <q-icon name="search" />
                </template>
              </q-input>
            </div>
            <div class="col-12 col-md-5">
              <q-select
                v-model="filtroStatus"
                outlined
                dense
                emit-value
                map-options
                :disable="loading"
                :options="[
                  { label: 'Ativas', value: 'ativos' },
                  { label: 'Inativas', value: 'inativos' },
                  { label: 'Todas', value: 'todos' },
                ]"
                label="Status"
              />
            </div>
          </div>

          <div class="row justify-end q-gutter-sm">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" unelevated />
            <q-btn flat icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </FilterBar>
    </AppSectionCard>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar filas de grupos tecnicos.
    </q-banner>

    <LoadingState v-else-if="loading && !filas.length" mensagem="Carregando filas do grupo tecnico..." />

    <ErrorState
      v-else-if="erro && !filas.length"
      titulo="Nao foi possivel carregar filas"
      :mensagem="erro"
      @retry="carregar"
    />

    <EmptyState
      v-else-if="!filas.length"
      titulo="Nenhuma fila cadastrada para este grupo tecnico."
      mensagem="Nenhuma fila corresponde aos filtros aplicados."
      icon="playlist_remove"
    >
      <template #actions>
        <q-btn v-if="possuiFiltro" flat color="primary" icon="filter_alt_off" label="Limpar filtros" @click="limparFiltros" />
      </template>
    </EmptyState>

    <AppSectionCard v-else titulo="Filas" subtitulo="Filas de atendimento vinculadas ao grupo tecnico." icon="list_alt">
      <q-banner v-if="erro" rounded class="bg-red-1 text-negative q-mb-md">
        {{ erro }}
      </q-banner>

      <q-table
        flat
        bordered
        row-key="id"
        :rows="filas"
        :columns="colunas"
        :loading="loading"
        :pagination="{ rowsPerPage: 0 }"
        hide-pagination
      >
        <template #body-cell-descricao="slotProps">
          <q-td :props="slotProps">
            {{ slotProps.row.descricao || '-' }}
          </q-td>
        </template>

        <template #body-cell-grupo="slotProps">
          <q-td :props="slotProps">
            {{ grupo?.nome || slotProps.row.grupoTecnicoId }}
          </q-td>
        </template>

        <template #body-cell-ativo="slotProps">
          <q-td :props="slotProps">
            <q-badge :color="slotProps.row.ativo ? 'positive' : 'grey-7'">
              {{ slotProps.row.ativo ? 'Ativa' : 'Inativa' }}
            </q-badge>
          </q-td>
        </template>
      </q-table>
    </AppSectionCard>
  </q-page>
</template>

<style scoped>
.grupo-filas-admin__kpis {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

@media (max-width: 900px) {
  .grupo-filas-admin__kpis {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
