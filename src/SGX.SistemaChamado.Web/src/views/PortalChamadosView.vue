<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import CardChamado from '../components/portal/CardChamado.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, ChamadoResumoPortal, PrioridadePortal, StatusPortal } from '../types/portal'

const router = useRouter()

const loading = ref(false)
const erro = ref<string | null>(null)
const chamados = ref<ChamadoResumoPortal[]>([])
const total = ref(0)
const categorias = ref<CategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])
const status = ref<StatusPortal[]>([])

const filtros = reactive({
  statusId: '',
  prioridadeId: '',
  categoriaId: '',
  texto: '',
})

async function carregarContexto() {
  const contexto = await portalService.obterPortalContexto()
  categorias.value = contexto.categorias
  prioridades.value = contexto.prioridades
  status.value = contexto.status
}

async function carregarChamados(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    const response = await portalService.listarMeusChamados({
      statusId: filtros.statusId || undefined,
      prioridadeId: filtros.prioridadeId || undefined,
      categoriaId: filtros.categoriaId || undefined,
      texto: filtros.texto || undefined,
      pagina: 1,
      tamanhoPagina: 50,
    })

    chamados.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao listar chamados.'
  } finally {
    loading.value = false
  }
}

function limpar(): void {
  filtros.statusId = ''
  filtros.prioridadeId = ''
  filtros.categoriaId = ''
  filtros.texto = ''
  carregarChamados()
}

onMounted(async () => {
  await carregarContexto()
  await carregarChamados()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Meus chamados" subtitulo="Consulte e acompanhe suas solicitacoes">
      <template #actions>
        <q-btn color="secondary" icon="add" label="Novo chamado" @click="router.push('/portal/chamados/novo')" />
      </template>
    </PageHeader>

    <q-card flat bordered class="sgx-card">
      <q-card-section class="row q-col-gutter-sm">
        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.statusId"
            :options="status.map((s) => ({ label: s.nome, value: s.id }))"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            clearable
            outlined
            label="Status"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.prioridadeId"
            :options="prioridades.map((p) => ({ label: p.nome, value: p.id }))"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            clearable
            outlined
            label="Prioridade"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.categoriaId"
            :options="categorias.map((c) => ({ label: c.nome, value: c.id }))"
            option-label="label"
            option-value="value"
            emit-value
            map-options
            clearable
            outlined
            label="Categoria"
          />
        </div>
        <div class="col-12 col-md-3">
          <q-input v-model="filtros.texto" outlined label="Buscar" />
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Limpar" @click="limpar" />
        <q-btn color="primary" label="Filtrar" :loading="loading" @click="carregarChamados" />
      </q-card-actions>
    </q-card>

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
    </div>

    <q-card v-else flat bordered class="sgx-card">
      <q-card-section class="text-caption text-grey-7">Total: {{ total }}</q-card-section>
      <q-separator />
      <q-card-section class="column q-gutter-sm">
        <CardChamado
          v-for="item in chamados"
          :key="item.id"
          :chamado="item"
          class="cursor-pointer"
          @click="router.push(`/portal/chamados/${item.id}`)"
        />

        <q-banner v-if="!chamados.length" rounded class="bg-blue-1 text-primary">
          Nenhum chamado encontrado para os filtros informados.
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>
