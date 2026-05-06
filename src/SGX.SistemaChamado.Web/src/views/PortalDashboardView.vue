<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { portalService } from '../services/portalService'
import type { ChamadoResumoPortal } from '../types/portal'
import CardChamado from '../components/portal/CardChamado.vue'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)
const chamados = ref<ChamadoResumoPortal[]>([])

const total = computed(() => chamados.value.length)
const abertos = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('aberto')).length)
const emAtendimento = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('atendimento')).length)
const aguardando = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('aguardando')).length)
const finalizados = computed(() => chamados.value.filter((x) => ['resolvido', 'encerrado'].some((s) => x.status.toLowerCase().includes(s))).length)

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    const response = await portalService.listarMeusChamados({ pagina: 1, tamanhoPagina: 5 })
    chamados.value = response.items
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <div class="column q-gutter-md">
    <div class="row q-col-gutter-md">
      <div class="col-12 col-sm-6 col-md-3"><q-card flat bordered class="kpi"><q-card-section><div class="text-caption">Total</div><div class="text-h6">{{ total }}</div></q-card-section></q-card></div>
      <div class="col-12 col-sm-6 col-md-3"><q-card flat bordered class="kpi"><q-card-section><div class="text-caption">Abertos</div><div class="text-h6">{{ abertos }}</div></q-card-section></q-card></div>
      <div class="col-12 col-sm-6 col-md-3"><q-card flat bordered class="kpi"><q-card-section><div class="text-caption">Em atendimento</div><div class="text-h6">{{ emAtendimento }}</div></q-card-section></q-card></div>
      <div class="col-12 col-sm-6 col-md-3"><q-card flat bordered class="kpi"><q-card-section><div class="text-caption">Finalizados</div><div class="text-h6">{{ finalizados }}</div></q-card-section></q-card></div>
    </div>

    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">Últimos chamados</h1>
      <div class="row q-gutter-sm">
        <q-btn color="secondary" label="Abrir chamado" @click="router.push('/portal/chamados/novo')" />
        <q-btn flat color="primary" label="Ver todos" @click="router.push('/portal/chamados')" />
      </div>
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-spinner v-if="loading" color="primary" size="2rem" />

    <div v-else class="column q-gutter-sm">
      <CardChamado v-for="item in chamados" :key="item.id" :chamado="item" @click="router.push(`/portal/chamados/${item.id}`)" class="cursor-pointer" />
      <q-banner v-if="!chamados.length" class="bg-blue-1 text-primary">Nenhum chamado encontrado.</q-banner>
    </div>
  </div>
</template>

<style scoped>
.kpi {
  border-radius: 12px;
}
</style>
