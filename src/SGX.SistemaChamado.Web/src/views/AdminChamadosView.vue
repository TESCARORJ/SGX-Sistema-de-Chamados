<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import FiltrosChamadoAdmin from '../components/admin/FiltrosChamadoAdmin.vue'
import TabelaChamados from '../components/admin/TabelaChamados.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
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

  try {
    if (filtros) {
      filtrosAtuais.value = {
        ...filtros,
        pagina: filtros.pagina ?? 1,
        tamanhoPagina: filtros.tamanhoPagina ?? filtrosAtuais.value.tamanhoPagina ?? 20,
      }
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

async function aplicarFiltros(filtros: FiltroChamadosAdmin): Promise<void> {
  sucesso.value = null
  await carregarChamados({
    ...filtros,
    pagina: 1,
  })
}

async function limparFiltros(filtros: FiltroChamadosAdmin): Promise<void> {
  sucesso.value = null
  await carregarChamados({ ...filtros })
}

onMounted(async () => {
  loading.value = true
  erro.value = null

  try {
    await carregarContexto()
    await carregarChamados()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao inicializar tela administrativa.'
    loading.value = false
  }
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Fila administrativa" subtitulo="Visualize, priorize e assuma chamados da operacao.">
      <template #actions>
        <q-chip color="primary" text-color="white" icon="confirmation_number" square>
          Total: {{ total }}
        </q-chip>
      </template>
    </PageHeader>

    <AppSectionCard titulo="Filtros da fila" subtitulo="Busque por status, prioridade, categoria, responsavel e SLA.">
      <FiltrosChamadoAdmin :contexto="contexto" :loading="loading" @filtrar="aplicarFiltros" @limpar="limparFiltros" />
    </AppSectionCard>

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregarChamados()" />

    <LoadingState v-else-if="loading" inline mensagem="Carregando fila administrativa..." />

    <AppSectionCard v-else titulo="Chamados da fila" :subtitulo="`Registros encontrados: ${total}`">
      <template #actions>
        <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">
          {{ sucesso }}
        </q-banner>
      </template>

      <EmptyState
        v-if="!chamados.length"
        titulo="Sem chamados para exibir"
        mensagem="Nao ha chamados para os filtros selecionados."
      />

      <template v-else>
        <TabelaChamados
          :rows="chamados"
          :loading="loading"
          :can-force-assume="podeAssumirComResponsavel()"
          @detalhar="(id) => router.push(`/admin/chamados/${id}`)"
          @assumir="assumir"
        />

        <div class="row justify-end q-mt-md">
          <q-pagination
            v-model="paginaAtual"
            color="primary"
            :max="Math.max(1, Math.ceil(total / (filtrosAtuais.tamanhoPagina || 20)))"
            :max-pages="7"
            boundary-numbers
            direction-links
            @update:model-value="alterarPagina"
          />
        </div>
      </template>
    </AppSectionCard>
  </q-page>
</template>
