<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import CardChamado from '../components/portal/CardChamado.vue'
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

onMounted(async () => {
  await carregarContexto()
  await carregarChamados()
})
</script>

<template>
  <div class="column q-gutter-md">
    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">Meus chamados</h1>
      <q-btn color="secondary" label="Novo chamado" @click="router.push('/portal/chamados/novo')" />
    </div>

    <q-card flat bordered>
      <q-card-section class="row q-col-gutter-sm">
        <div class="col-12 col-md-3">
          <q-select v-model="filtros.statusId" :options="status.map(s => ({ label: s.nome, value: s.id }))" option-label="label" option-value="value" emit-value map-options clearable outlined label="Status" />
        </div>
        <div class="col-12 col-md-3">
          <q-select v-model="filtros.prioridadeId" :options="prioridades.map(p => ({ label: p.nome, value: p.id }))" option-label="label" option-value="value" emit-value map-options clearable outlined label="Prioridade" />
        </div>
        <div class="col-12 col-md-3">
          <q-select v-model="filtros.categoriaId" :options="categorias.map(c => ({ label: c.nome, value: c.id }))" option-label="label" option-value="value" emit-value map-options clearable outlined label="Categoria" />
        </div>
        <div class="col-12 col-md-3">
          <q-input v-model="filtros.texto" outlined label="Buscar" />
        </div>
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Limpar" @click="filtros.statusId='';filtros.prioridadeId='';filtros.categoriaId='';filtros.texto='';carregarChamados()" />
        <q-btn color="primary" label="Filtrar" @click="carregarChamados" />
      </q-card-actions>
    </q-card>

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-spinner v-if="loading" color="primary" size="2rem" />

    <div v-else class="column q-gutter-sm">
      <div class="text-caption text-grey-8">Total: {{ total }}</div>
      <CardChamado v-for="item in chamados" :key="item.id" :chamado="item" class="cursor-pointer" @click="router.push(`/portal/chamados/${item.id}`)" />
      <q-banner v-if="!chamados.length" class="bg-blue-1 text-primary">Nenhum chamado encontrado para os filtros informados.</q-banner>
    </div>
  </div>
</template>
