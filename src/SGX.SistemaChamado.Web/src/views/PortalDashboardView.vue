<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import CardChamado from '../components/portal/CardChamado.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { portalService } from '../services/portalService'
import type { ChamadoResumoPortal } from '../types/portal'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)
const chamados = ref<ChamadoResumoPortal[]>([])

const total = computed(() => chamados.value.length)
const abertos = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('aberto')).length)
const emAtendimento = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('atendimento')).length)
const aguardando = computed(() => chamados.value.filter((x) => x.status.toLowerCase().includes('aguardando')).length)
const finalizados = computed(() =>
  chamados.value.filter((x) => ['resolvido', 'encerrado'].some((s) => x.status.toLowerCase().includes(s))).length
)

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
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Dashboard do solicitante"
      subtitulo="Resumo dos seus chamados mais recentes"
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="secondary" label="Abrir chamado" icon="add" @click="router.push('/portal/chamados/novo')" />
          <q-btn flat color="primary" label="Ver todos" @click="router.push('/portal/chamados')" />
        </div>
      </template>
    </PageHeader>

    <div class="row q-col-gutter-md">
      <div class="col-12 col-sm-6 col-md-3">
        <q-card flat bordered class="sgx-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Total</div>
            <div class="text-h5 text-weight-bold">{{ total }}</div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-card flat bordered class="sgx-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Abertos</div>
            <div class="text-h5 text-weight-bold">{{ abertos }}</div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-card flat bordered class="sgx-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Em atendimento</div>
            <div class="text-h5 text-weight-bold">{{ emAtendimento }}</div>
          </q-card-section>
        </q-card>
      </div>
      <div class="col-12 col-sm-6 col-md-3">
        <q-card flat bordered class="sgx-card">
          <q-card-section>
            <div class="text-caption text-grey-7">Finalizados</div>
            <div class="text-h5 text-weight-bold">{{ finalizados }}</div>
          </q-card-section>
        </q-card>
      </div>
    </div>

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
    </div>

    <q-card v-else flat bordered class="sgx-card">
      <q-card-section class="text-subtitle1 text-weight-medium">Ultimos chamados</q-card-section>
      <q-separator />
      <q-card-section class="column q-gutter-sm">
        <CardChamado
          v-for="item in chamados"
          :key="item.id"
          :chamado="item"
          @click="router.push(`/portal/chamados/${item.id}`)"
          class="cursor-pointer"
        />

        <q-banner v-if="!chamados.length" rounded class="bg-blue-1 text-primary">
          Nenhum chamado encontrado.
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>
