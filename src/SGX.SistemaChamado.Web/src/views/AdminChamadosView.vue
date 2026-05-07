<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import FiltrosChamadoAdmin from '../components/admin/FiltrosChamadoAdmin.vue'
import TabelaChamados from '../components/admin/TabelaChamados.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { adminService } from '../services/adminService'
import type { AdminContextoResponse, ChamadoAdminResumo, FiltroChamadosAdmin } from '../types/admin'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const contexto = ref<AdminContextoResponse | null>(null)
const chamados = ref<ChamadoAdminResumo[]>([])
const total = ref(0)
const filtrosPadrao: FiltroChamadosAdmin = {
  pagina: 1,
  tamanhoPagina: 20,
  ordenarPor: 'atualizadoEm',
  direcaoOrdenacao: 'desc',
}
const filtrosAtuais = ref<FiltroChamadosAdmin>({ ...filtrosPadrao })
const paginaAtual = ref(1)

function podeAssumirComResponsavel(): boolean {
  return contexto.value?.usuario.perfis.includes('Administrador') ?? false
}

async function carregarContexto(): Promise<void> {
  contexto.value = await adminService.obterAdminContexto()
}

async function carregarChamados(filtros?: FiltroChamadosAdmin): Promise<void> {
  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    if (filtros) {
      filtrosAtuais.value = { ...filtros }
    }

    const response = await adminService.listarChamadosAdmin(filtrosAtuais.value)
    chamados.value = response.items
    total.value = response.total
    paginaAtual.value = filtrosAtuais.value.pagina ?? 1
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar fila administrativa.'
  } finally {
    loading.value = false
  }
}

async function assumir(id: string): Promise<void> {
  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await adminService.assumirChamado(id)
    sucesso.value = 'Chamado assumido com sucesso.'
    await carregarChamados()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao assumir chamado.'
  } finally {
    loading.value = false
  }
}

async function alterarPagina(page: number): Promise<void> {
  filtrosAtuais.value = {
    ...filtrosAtuais.value,
    pagina: page,
  }

  await carregarChamados(filtrosAtuais.value)
}

onMounted(async () => {
  await carregarContexto()
  await carregarChamados()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Fila administrativa de chamados"
      subtitulo="Monitore, assuma e direcione os chamados da operacao"
    >
      <template #actions>
        <q-chip color="primary" text-color="white" icon="confirmation_number">Total: {{ total }}</q-chip>
      </template>
    </PageHeader>

    <FiltrosChamadoAdmin
      :contexto="contexto"
      :loading="loading"
      @filtrar="carregarChamados"
      @limpar="carregarChamados({ ...filtrosPadrao })"
    />

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">{{ sucesso }}</q-banner>

    <q-card flat bordered class="sgx-card">
      <q-card-section>
        <TabelaChamados
          :rows="chamados"
          :loading="loading"
          :can-force-assume="podeAssumirComResponsavel()"
          @detalhar="(id) => router.push(`/admin/chamados/${id}`)"
          @assumir="assumir"
        />
      </q-card-section>
      <q-separator />
      <q-card-section class="row justify-end">
        <q-pagination
          v-model="paginaAtual"
          color="primary"
          :max="Math.max(1, Math.ceil(total / (filtrosAtuais.tamanhoPagina || 20)))"
          :max-pages="7"
          boundary-numbers
          direction-links
          @update:model-value="alterarPagina"
        />
      </q-card-section>
    </q-card>

    <q-banner v-if="!loading && !chamados.length" rounded class="bg-blue-1 text-primary">
      Nenhum chamado encontrado para os filtros informados.
    </q-banner>
  </q-page>
</template>
