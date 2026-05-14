<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, ChamadoResumoPortal, FiltroChamadosPortal, PrioridadePortal, StatusPortal } from '../types/portal'

const $q = useQuasar()
const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)

const chamados = ref<ChamadoResumoPortal[]>([])
const total = ref(0)

const categorias = ref<CategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])
const status = ref<StatusPortal[]>([])

const filtros = reactive({
  texto: '',
  statusId: '',
  prioridadeId: '',
  categoriaId: '',
})

const columns: QTableColumn<ChamadoResumoPortal>[] = [
  { name: 'codigo', label: 'Código', field: 'codigo', align: 'left', sortable: true },
  { name: 'titulo', label: 'Título', field: 'titulo', align: 'left', sortable: true },
  { name: 'status', label: 'Status', field: 'status', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'sla', label: 'SLA', field: 'slaVencido', align: 'left' },
  { name: 'categoria', label: 'Categoria', field: 'categoria', align: 'left', sortable: true },
  { name: 'abertoEm', label: 'Aberto em', field: 'abertoEm', align: 'left', sortable: true },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left', sortable: true },
  { name: 'acoes', label: 'Ações', field: 'id', align: 'right' },
]

const opcoesStatus = computed(() => status.value.map((item) => ({ label: item.nome, value: item.id })))
const opcoesPrioridade = computed(() => prioridades.value.map((item) => ({ label: item.nome, value: item.id })))
const opcoesCategoria = computed(() => categorias.value.map((item) => ({ label: item.nome, value: item.id })))

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function montarFiltroRequest(): FiltroChamadosPortal {
  return {
    texto: filtros.texto || undefined,
    statusId: filtros.statusId || undefined,
    prioridadeId: filtros.prioridadeId || undefined,
    categoriaId: filtros.categoriaId || undefined,
    pagina: 1,
    tamanhoPagina: 100,
  }
}

async function carregarContexto(): Promise<void> {
  const contexto = await portalService.getPortalContexto()
  categorias.value = contexto.categorias
  prioridades.value = contexto.prioridades
  status.value = contexto.status
}

async function carregarChamados(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const response = await portalService.listarMeusChamados(montarFiltroRequest())
    chamados.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os chamados.'
  } finally {
    loading.value = false
  }
}

async function aplicarFiltros(): Promise<void> {
  await carregarChamados()
}

async function limparFiltros(): Promise<void> {
  filtros.texto = ''
  filtros.statusId = ''
  filtros.prioridadeId = ''
  filtros.categoriaId = ''
  await carregarChamados()
}

onMounted(async () => {
  loading.value = true
  erro.value = null

  try {
    await carregarContexto()
    await carregarChamados()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Meus chamados" subtitulo="Filtre e acompanhe os chamados abertos pelo seu usuário.">
      <template #actions>
        <q-btn color="secondary" icon="add" label="Abrir novo chamado" @click="router.push('/portal/chamados/novo')" />
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros" subtitulo="Use os filtros para localizar chamados específicos.">
      <q-form class="row q-col-gutter-sm" @submit.prevent="aplicarFiltros">
        <div class="col-12 col-md-3">
          <q-input v-model="filtros.texto" outlined label="Texto" placeholder="Código, título ou descrição" />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.statusId"
            outlined
            clearable
            emit-value
            map-options
            label="Status"
            :options="opcoesStatus"
          />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.prioridadeId"
            outlined
            clearable
            emit-value
            map-options
            label="Prioridade"
            :options="opcoesPrioridade"
          />
        </div>

        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.categoriaId"
            outlined
            clearable
            emit-value
            map-options
            label="Categoria"
            :options="opcoesCategoria"
          />
        </div>

        <div class="col-12 row justify-end q-gutter-sm">
          <q-btn flat color="primary" icon="cleaning_services" label="Limpar" :disable="loading" @click="limparFiltros" />
          <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" />
        </div>
      </q-form>
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregarChamados" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando chamados..." />

    <AppSectionCard v-else titulo="Resultado da consulta" :subtitulo="`Total de chamados: ${total}`">
      <EmptyState
        v-if="!chamados.length"
        titulo="Nenhum chamado encontrado"
        mensagem="Nenhum resultado corresponde aos filtros aplicados."
      >
        <template #actions>
          <q-btn color="secondary" icon="add" label="Abrir chamado" @click="router.push('/portal/chamados/novo')" />
        </template>
      </EmptyState>

      <q-table
        v-else
        class="sgx-table"
        flat
        bordered
        :rows="chamados"
        :columns="columns"
        row-key="id"
        :pagination="{ rowsPerPage: 15 }"
        :grid="$q.screen.lt.md"
        :rows-per-page-options="[10, 15, 25, 50]"
        separator="horizontal"
      >
        <template #body-cell-codigo="slotProps">
          <q-td :props="slotProps">
            <q-btn
              flat
              dense
              color="primary"
              class="q-pa-none text-weight-bold"
              :label="slotProps.row.codigo"
              @click="router.push(`/portal/chamados/${slotProps.row.id}`)"
            />
          </q-td>
        </template>

        <template #body-cell-status="slotProps">
          <q-td :props="slotProps">
            <StatusBadge :texto="slotProps.row.status" />
          </q-td>
        </template>

        <template #body-cell-prioridade="slotProps">
          <q-td :props="slotProps">
            <PrioridadeBadge :texto="slotProps.row.prioridade" />
          </q-td>
        </template>

        <template #body-cell-sla="slotProps">
          <q-td :props="slotProps">
            <SlaBadge
              :vencido="slotProps.row.slaVencido"
              :proximo="slotProps.row.slaProximoVencimento"
              :pausado="slotProps.row.estaPausado"
              :situacao="slotProps.row.situacaoSla"
            />
          </q-td>
        </template>

        <template #body-cell-abertoEm="slotProps">
          <q-td :props="slotProps">{{ formatarData(slotProps.row.abertoEm) }}</q-td>
        </template>

        <template #body-cell-atualizadoEm="slotProps">
          <q-td :props="slotProps">{{ formatarData(slotProps.row.atualizadoEm) }}</q-td>
        </template>

        <template #body-cell-acoes="slotProps">
          <q-td :props="slotProps" class="text-right">
            <q-btn
              flat
              dense
              color="primary"
              icon="visibility"
              label="Ver detalhe"
              @click="router.push(`/portal/chamados/${slotProps.row.id}`)"
            />
          </q-td>
        </template>

        <template #item="slotProps">
          <div class="col-12 q-mb-sm">
            <q-card flat bordered class="sgx-card">
              <q-card-section class="row items-start justify-between q-col-gutter-sm">
                <div class="col">
                  <div class="text-caption text-grey-7">{{ slotProps.row.codigo }}</div>
                  <div class="text-subtitle1 text-weight-medium">{{ slotProps.row.titulo }}</div>
                  <div class="text-caption text-grey-7 q-mt-xs">{{ slotProps.row.categoria }}</div>
                  <div class="text-caption text-grey-7 q-mt-xs">Aberto em: {{ formatarData(slotProps.row.abertoEm) }}</div>
                </div>

                <div class="col-auto column items-end q-gutter-xs">
                  <StatusBadge :texto="slotProps.row.status" />
                  <PrioridadeBadge :texto="slotProps.row.prioridade" />
                </div>
              </q-card-section>

              <q-separator />

              <q-card-actions align="between" class="q-pa-sm">
                <SlaBadge
                  :vencido="slotProps.row.slaVencido"
                  :proximo="slotProps.row.slaProximoVencimento"
                  :pausado="slotProps.row.estaPausado"
                  :situacao="slotProps.row.situacaoSla"
                />

                <q-btn
                  flat
                  dense
                  color="primary"
                  icon="visibility"
                  label="Ver detalhe"
                  @click="router.push(`/portal/chamados/${slotProps.row.id}`)"
                />
              </q-card-actions>
            </q-card>
          </div>
        </template>
      </q-table>
    </AppSectionCard>
  </q-page>
</template>

<style scoped>
:deep(.sgx-table .q-table__middle) {
  overflow-x: auto;
}

:deep(.sgx-table tbody tr:hover) {
  background: rgba(11, 94, 215, 0.04);
}
</style>
