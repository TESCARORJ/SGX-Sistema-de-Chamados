<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { QTableColumn } from 'quasar'
import { useAuthStore } from '../../stores/authStore'
import CampoAtivoInativo from '../../components/admin/cadastros/CampoAtivoInativo.vue'
import CampoBuscaCadastro from '../../components/admin/cadastros/CampoBuscaCadastro.vue'
import PaginacaoTabela from '../../components/admin/cadastros/PaginacaoTabela.vue'
import TabelaAdministrativa from '../../components/admin/cadastros/TabelaAdministrativa.vue'
import { usuariosAdminService } from '../../services/usuariosAdminService'
import { cadastrosAdminService } from '../../services/cadastrosAdminService'
import { parametrosSistemaService } from '../../services/parametrosSistemaService'
import type { FiltroCadastroRequest, PagedResultResponse } from '../../types/adminCadastros'

type Entidade = 'usuarios' | 'perfis' | 'departamentos' | 'categorias' | 'prioridades' | 'status' | 'parametros'

const props = defineProps<{
  titulo: string
  entidade: Entidade
  detalhePathBase: string
  colunas: QTableColumn[]
}>()

const router = useRouter()
const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const texto = ref('')
const filtroAtivo = ref<'todos' | 'ativos' | 'inativos'>('ativos')
const pagina = ref(1)
const tamanhoPagina = ref(20)
const total = ref(0)
const rows = ref<unknown[]>([])

const isAdmin = computed(() => authStore.usuario?.perfis.includes('Administrador') ?? false)

function montarFiltro(): FiltroCadastroRequest {
  return {
    texto: texto.value || undefined,
    ativo: filtroAtivo.value === 'todos' ? undefined : filtroAtivo.value === 'ativos',
    pagina: pagina.value,
    tamanhoPagina: tamanhoPagina.value,
  }
}

async function listarComServico(filtro: FiltroCadastroRequest): Promise<PagedResultResponse<unknown>> {
  switch (props.entidade) {
    case 'usuarios':
      return usuariosAdminService.listar(filtro)
    case 'perfis':
      return cadastrosAdminService.listarPerfis(filtro)
    case 'departamentos':
      return cadastrosAdminService.listarDepartamentos(filtro)
    case 'categorias':
      return cadastrosAdminService.listarCategorias(filtro)
    case 'prioridades':
      return cadastrosAdminService.listarPrioridades(filtro)
    case 'status':
      return cadastrosAdminService.listarStatus(filtro)
    case 'parametros':
      return parametrosSistemaService.listar(filtro)
  }
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    const response = await listarComServico(montarFiltro())
    rows.value = response.items
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar cadastro.'
  } finally {
    loading.value = false
  }
}

function abrirDetalhe(id: string): void {
  router.push(`${props.detalhePathBase}/${id}`)
}

function novo(): void {
  router.push(`${props.detalhePathBase}/novo`)
}

onMounted(carregar)
</script>

<template>
  <div class="column q-gutter-md">
    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">{{ titulo }}</h1>
      <q-btn
        v-if="isAdmin"
        color="primary"
        icon="add"
        label="Novo"
        :disable="loading"
        @click="novo"
      />
    </div>

    <div class="row q-col-gutter-sm">
      <div class="col-12 col-md-7">
        <CampoBuscaCadastro v-model="texto" :loading="loading" />
      </div>
      <div class="col-12 col-md-5">
        <CampoAtivoInativo v-model="filtroAtivo" :loading="loading" />
      </div>
    </div>

    <div class="row q-gutter-sm">
      <q-btn color="primary" icon="search" label="Filtrar" :loading="loading" @click="() => { pagina = 1; carregar() }" />
      <q-btn flat label="Limpar" :disable="loading" @click="() => { texto = ''; filtroAtivo = 'ativos'; pagina = 1; carregar() }" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <TabelaAdministrativa :title="titulo" :rows="rows" :columns="colunas" :loading="loading">
      <template #acoes="{ row }">
        <q-btn flat dense icon="visibility" label="Detalhar" @click="abrirDetalhe(row.id)" />
      </template>
    </TabelaAdministrativa>

    <PaginacaoTabela
      :pagina="pagina"
      :tamanho-pagina="tamanhoPagina"
      :total="total"
      :loading="loading"
      @update:pagina="(value) => { pagina = value; carregar() }"
      @update:tamanho-pagina="(value) => { tamanhoPagina = value; pagina = 1; carregar() }"
    />
  </div>
</template>
