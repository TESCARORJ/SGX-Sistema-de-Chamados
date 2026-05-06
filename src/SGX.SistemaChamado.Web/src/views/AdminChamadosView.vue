<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import FiltrosChamadoAdmin from '../components/admin/FiltrosChamadoAdmin.vue'
import TabelaChamados from '../components/admin/TabelaChamados.vue'
import { adminService } from '../services/adminService'
import type { AdminContextoResponse, ChamadoAdminResumo, FiltroChamadosAdmin } from '../types/admin'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const contexto = ref<AdminContextoResponse | null>(null)
const chamados = ref<ChamadoAdminResumo[]>([])
const total = ref(0)
const filtrosAtuais = ref<FiltroChamadosAdmin>({ pagina: 1, tamanhoPagina: 20, ordenarPor: 'atualizadoEm', direcaoOrdenacao: 'desc' })

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

onMounted(async () => {
  await carregarContexto()
  await carregarChamados()
})
</script>

<template>
  <div class="column q-gutter-md">
    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">Fila administrativa de chamados</h1>
      <div class="text-caption text-grey-8">Total: {{ total }}</div>
    </div>

    <FiltrosChamadoAdmin
      :contexto="contexto"
      :loading="loading"
      @filtrar="carregarChamados"
      @limpar="carregarChamados({ pagina: 1, tamanhoPagina: 20, ordenarPor: 'atualizadoEm', direcaoOrdenacao: 'desc' })"
    />

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-banner v-if="sucesso" class="bg-green-1 text-positive">{{ sucesso }}</q-banner>

    <TabelaChamados
      :rows="chamados"
      :loading="loading"
      :can-force-assume="podeAssumirComResponsavel()"
      @detalhar="(id) => router.push(`/admin/chamados/${id}`)"
      @assumir="assumir"
    />

    <q-banner v-if="!loading && !chamados.length" class="bg-blue-1 text-primary">
      Nenhum chamado encontrado para os filtros informados.
    </q-banner>
  </div>
</template>
