<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import FiltrosChamadoAdmin from '../components/admin/FiltrosChamadoAdmin.vue'
import TabelaChamados from '../components/admin/TabelaChamados.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { adminService } from '../services/adminService'
import { useAuthStore } from '../stores/authStore'
import type { AdminContextoResponse, ChamadoAdminResumo, FiltroChamadosAdmin } from '../types/admin'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

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
const textoBuscaGlobal = ref('')
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeAssumirChamado = computed(() =>
  fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.chamadosAssumir)
)
const chamadosSemResponsavel = computed(() => chamados.value.filter((item) => !item.responsavelNome).length)
const chamadosCriticos = computed(() => chamados.value.filter((item) => item.slaVencido || item.slaProximoVencimento).length)
const chamadosAguardandoAprovacao = computed(() => chamados.value.filter((item) => item.aprovacaoPendente).length)
const chamadosEmAtendimento = computed(() =>
  chamados.value.filter((item) => item.status.toLowerCase().includes('atendimento')).length
)

function podeAssumirComResponsavel(): boolean {
  return (contexto.value?.usuario.perfis.includes('Administrador') ?? false) && podeAssumirChamado.value
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
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os chamados.'
  } finally {
    loading.value = false
  }
}

function extrairTextoDaQuery(): string {
  const valor = route.query.texto ?? route.query.busca

  if (Array.isArray(valor)) {
    return (valor[0] ?? '').trim()
  }

  return typeof valor === 'string' ? valor.trim() : ''
}

async function atualizarQueryTexto(texto: string | undefined): Promise<boolean> {
  const queryAtual = { ...route.query }
  const novoTexto = texto?.trim() || undefined
  const textoAtualRaw = route.query.texto ?? route.query.busca
  const textoAtual = Array.isArray(textoAtualRaw) ? textoAtualRaw[0] : textoAtualRaw
  const textoAtualNormalizado = typeof textoAtual === 'string' && textoAtual.trim() ? textoAtual.trim() : undefined

  if (novoTexto === textoAtualNormalizado && !('busca' in route.query)) {
    return false
  }

  delete queryAtual.busca

  if (novoTexto) {
    queryAtual.texto = novoTexto
  } else {
    delete queryAtual.texto
  }

  await router.replace({
    path: '/admin/chamados',
    query: queryAtual,
  })

  return true
}

async function assumir(id: string): Promise<void> {
  if (!podeAssumirChamado.value) {
    erro.value = 'Você não possui permissão para assumir chamados.'
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await adminService.assumirChamado(id)
    sucesso.value = 'Chamado assumido com sucesso.'
    await carregarChamados()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível concluir a ação.'
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

  const proximoFiltro: FiltroChamadosAdmin = {
    ...filtros,
    pagina: 1,
  }

  filtrosAtuais.value = proximoFiltro
  const mudouQuery = await atualizarQueryTexto(proximoFiltro.texto)

  if (!mudouQuery) {
    await carregarChamados(proximoFiltro)
  }
}

async function limparFiltros(filtros: FiltroChamadosAdmin): Promise<void> {
  sucesso.value = null

  const proximoFiltro = { ...filtros }
  filtrosAtuais.value = proximoFiltro
  const mudouQuery = await atualizarQueryTexto(undefined)

  if (!mudouQuery) {
    await carregarChamados(proximoFiltro)
  }
}

const mensagemVazio = computed(() => {
  if (textoBuscaGlobal.value) {
    return 'Nenhum chamado encontrado para sua busca.'
  }

  return 'Nenhum resultado corresponde aos filtros aplicados.'
})

onMounted(async () => {
  try {
    await carregarContexto()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  }
})

watch(
  () => [route.query.texto, route.query.busca],
  async () => {
    const texto = extrairTextoDaQuery()
    textoBuscaGlobal.value = texto
    sucesso.value = null

    await carregarChamados({
      ...filtrosAtuais.value,
      texto: texto || undefined,
      pagina: 1,
    })
  },
  { immediate: true }
)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="Operacao administrativa"
      titulo="Fila de Chamados"
      subtitulo="Visualize, priorize e distribua chamados com base em status, SLA e responsabilidade."
    >
      <template #actions>
        <q-chip color="primary" text-color="white" icon="confirmation_number" square>
          Total: {{ total }}
        </q-chip>
      </template>
    </PageHeader>

    <div class="sgx-kpi-grid">
      <MetricCard title="Total na consulta" :value="total" icon="confirmation_number" tone="primary" :loading="loading" />
      <MetricCard title="Em atendimento" :value="chamadosEmAtendimento" icon="support_agent" tone="info" :loading="loading" />
      <MetricCard
        title="Risco de SLA"
        :value="chamadosCriticos"
        icon="warning"
        :tone="chamadosCriticos > 0 ? 'negative' : 'warning'"
        :loading="loading"
      />
      <MetricCard title="Sem responsavel" :value="chamadosSemResponsavel" icon="person_off" tone="warning" :loading="loading" />
      <MetricCard
        title="Aprovacao pendente"
        :value="chamadosAguardandoAprovacao"
        icon="fact_check"
        tone="warning"
        :loading="loading"
      />
    </div>

    <AppSectionCard titulo="Filtros da fila" subtitulo="Busque por status, prioridade, categoria, responsável e SLA.">
      <FiltrosChamadoAdmin
        :contexto="contexto"
        :texto-inicial="textoBuscaGlobal"
        :loading="loading"
        @filtrar="aplicarFiltros"
        @limpar="limparFiltros"
      />
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
        :mensagem="mensagemVazio"
      />

      <template v-else>
        <TabelaChamados
          :rows="chamados"
          :loading="loading"
          :can-assumir="podeAssumirChamado"
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
