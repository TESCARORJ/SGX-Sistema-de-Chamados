<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import FilterBar from '../components/ui/FilterBar.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { adminService } from '../services/adminService'
import { useAuthStore } from '../stores/authStore'
import type { GrupoTecnicoDetalhe, GrupoTecnicoResumo } from '../types/admin'

type FiltroStatus = 'todos' | 'ativos' | 'inativos'
type GrupoTecnicoLinha = GrupoTecnicoDetalhe

const authStore = useAuthStore()
const $q = useQuasar()
const router = useRouter()

const loading = ref(false)
const salvando = ref(false)
const atualizandoStatus = ref(false)
const erro = ref<string | null>(null)
const grupos = ref<GrupoTecnicoLinha[]>([])
const total = ref(0)
const pagina = ref(1)
const tamanhoPagina = ref(20)
const texto = ref('')
const filtroStatus = ref<FiltroStatus>('ativos')
const dialogoFormularioAberto = ref(false)
const dialogoStatusAberto = ref(false)
const editandoId = ref<string | null>(null)
const grupoSelecionado = ref<GrupoTecnicoLinha | null>(null)
const tentativaSubmit = ref(false)

const form = reactive({
  nome: '',
  descricao: '',
})

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const usuarioEhAtendente = computed(() => (authStore.usuario?.perfis ?? []).includes('Atendente'))
const podeVisualizar = computed(() => usuarioEhAdministrador.value || usuarioEhAtendente.value)
const podeGerenciar = computed(() => usuarioEhAdministrador.value)
const totalAtivos = computed(() => grupos.value.filter((grupo) => grupo.ativo).length)
const totalInativos = computed(() => grupos.value.length - totalAtivos.value)
const possuiFiltro = computed(() => Boolean(texto.value.trim()) || filtroStatus.value !== 'ativos')
const nomeInvalido = computed(() => tentativaSubmit.value && !form.nome.trim())
const tituloFormulario = computed(() => (editandoId.value ? 'Editar grupo tecnico' : 'Novo grupo tecnico'))
const labelAcaoStatus = computed(() => (grupoSelecionado.value?.ativo ? 'Inativar' : 'Ativar'))
const mensagemAcaoStatus = computed(() => {
  const nome = grupoSelecionado.value?.nome ? ` "${grupoSelecionado.value.nome}"` : ''
  return grupoSelecionado.value?.ativo
    ? `Deseja inativar o grupo tecnico${nome}?`
    : `Deseja ativar o grupo tecnico${nome}?`
})

const colunas: QTableColumn<GrupoTecnicoLinha>[] = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left', sortable: true },
  { name: 'descricao', label: 'Descricao', field: 'descricao', align: 'left' },
  { name: 'ativo', label: 'Status', field: 'ativo', align: 'center', sortable: true },
  { name: 'criadoEm', label: 'Criado em', field: 'criadoEm', align: 'left', sortable: true },
  { name: 'atualizadoEm', label: 'Atualizado em', field: 'atualizadoEm', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' },
]

function ativoFiltro(): boolean | undefined {
  if (filtroStatus.value === 'ativos') return true
  if (filtroStatus.value === 'inativos') return false
  return undefined
}

function formatarData(valor: string | null): string {
  if (!valor) return '-'
  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) return '-'
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(data)
}

async function carregarDetalhe(grupo: GrupoTecnicoResumo): Promise<GrupoTecnicoLinha> {
  try {
    return await adminService.obterGrupoTecnico(grupo.id)
  } catch {
    return {
      id: grupo.id,
      nome: grupo.nome,
      descricao: null,
      ativo: grupo.ativo,
      criadoEm: '',
      atualizadoEm: null,
    }
  }
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    erro.value = 'Sem permissao para visualizar grupos tecnicos.'
    return
  }

  loading.value = true
  erro.value = null

  try {
    const response = await adminService.listarGruposTecnicos({
      texto: texto.value.trim() || undefined,
      ativo: ativoFiltro(),
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })

    grupos.value = await Promise.all(response.items.map(carregarDetalhe))
    total.value = response.total
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar grupos tecnicos.'
  } finally {
    loading.value = false
  }
}

function aplicarFiltros(): void {
  pagina.value = 1
  void carregar()
}

function limparFiltros(): void {
  texto.value = ''
  filtroStatus.value = 'ativos'
  pagina.value = 1
  void carregar()
}

function abrirCriacao(): void {
  if (!podeGerenciar.value) return
  editandoId.value = null
  tentativaSubmit.value = false
  form.nome = ''
  form.descricao = ''
  dialogoFormularioAberto.value = true
}

function abrirEdicao(grupo: GrupoTecnicoLinha): void {
  if (!podeGerenciar.value) return
  editandoId.value = grupo.id
  tentativaSubmit.value = false
  form.nome = grupo.nome
  form.descricao = grupo.descricao ?? ''
  dialogoFormularioAberto.value = true
}

async function salvar(): Promise<void> {
  tentativaSubmit.value = true
  if (!form.nome.trim() || !podeGerenciar.value) {
    return
  }

  salvando.value = true
  erro.value = null

  try {
    const payload = {
      nome: form.nome.trim(),
      descricao: form.descricao.trim() || null,
    }

    if (editandoId.value) {
      await adminService.atualizarGrupoTecnico(editandoId.value, payload)
      $q.notify({ type: 'positive', message: 'Grupo tecnico atualizado com sucesso.' })
    } else {
      await adminService.criarGrupoTecnico(payload)
      $q.notify({ type: 'positive', message: 'Grupo tecnico criado com sucesso.' })
    }

    dialogoFormularioAberto.value = false
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel salvar o grupo tecnico.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

function abrirConfirmacaoStatus(grupo: GrupoTecnicoLinha): void {
  if (!podeGerenciar.value) return
  grupoSelecionado.value = grupo
  dialogoStatusAberto.value = true
}

function abrirMembros(grupo: GrupoTecnicoLinha): void {
  router.push(`/admin/cadastros/grupos-tecnicos/${grupo.id}/membros`)
}

function abrirFilas(grupo: GrupoTecnicoLinha): void {
  router.push(`/admin/cadastros/grupos-tecnicos/${grupo.id}/filas`)
}

async function alternarStatus(): Promise<void> {
  if (!grupoSelecionado.value || !podeGerenciar.value) {
    return
  }

  atualizandoStatus.value = true
  erro.value = null

  try {
    const ativo = !grupoSelecionado.value.ativo
    await adminService.atualizarStatusGrupoTecnico(grupoSelecionado.value.id, { ativo })
    $q.notify({
      type: 'positive',
      message: ativo ? 'Grupo tecnico ativado com sucesso.' : 'Grupo tecnico inativado com sucesso.',
    })
    dialogoStatusAberto.value = false
    grupoSelecionado.value = null
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel alterar o status do grupo tecnico.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    atualizandoStatus.value = false
  }
}

function atualizarPagina(value: number): void {
  pagina.value = value
  void carregar()
}

function atualizarTamanhoPagina(value: number): void {
  tamanhoPagina.value = value
  pagina.value = 1
  void carregar()
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md grupos-tecnicos-admin">
    <PageHeader
      contexto="Cadastros administrativos"
      titulo="Grupos Tecnicos"
      subtitulo="Cadastro administrativo de grupos responsaveis por filas e atendimento."
    >
      <template #actions>
        <q-btn
          v-if="podeGerenciar"
          color="primary"
          icon="add"
          label="Novo grupo tecnico"
          unelevated
          :disable="loading"
          @click="abrirCriacao"
        />
      </template>
    </PageHeader>

    <div class="grupos-tecnicos-admin__kpis">
      <MetricCard titulo="Grupos na pagina" :valor="grupos.length" icon="groups" :loading="loading" />
      <MetricCard titulo="Ativos" :valor="totalAtivos" icon="check_circle" tone="positive" :loading="loading" />
      <MetricCard titulo="Inativos" :valor="totalInativos" icon="pause_circle" tone="warning" :loading="loading" />
    </div>

    <AppSectionCard titulo="Filtros" subtitulo="Localize grupos por nome e situacao." icon="filter_alt">
      <FilterBar compact>
        <q-form class="column q-gutter-md" @submit.prevent="aplicarFiltros">
          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-7">
              <q-input v-model="texto" outlined dense label="Buscar por nome" :disable="loading">
                <template #prepend>
                  <q-icon name="search" />
                </template>
              </q-input>
            </div>
            <div class="col-12 col-md-5">
              <q-select
                v-model="filtroStatus"
                outlined
                dense
                emit-value
                map-options
                :disable="loading"
                :options="[
                  { label: 'Ativos', value: 'ativos' },
                  { label: 'Inativos', value: 'inativos' },
                  { label: 'Todos', value: 'todos' },
                ]"
                label="Status"
              />
            </div>
          </div>

          <div class="row justify-end q-gutter-sm">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" unelevated />
            <q-btn flat icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </FilterBar>
    </AppSectionCard>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar grupos tecnicos.
    </q-banner>

    <LoadingState v-else-if="loading && !grupos.length" mensagem="Carregando grupos tecnicos..." />

    <ErrorState
      v-else-if="erro && !grupos.length"
      titulo="Nao foi possivel carregar grupos tecnicos"
      :mensagem="erro"
      @retry="carregar"
    />

    <EmptyState
      v-else-if="!grupos.length"
      titulo="Nenhum grupo tecnico encontrado"
      mensagem="Nenhum grupo corresponde aos filtros aplicados."
      icon="groups"
    >
      <template #actions>
        <q-btn
          v-if="possuiFiltro"
          flat
          color="primary"
          icon="filter_alt_off"
          label="Limpar filtros"
          @click="limparFiltros"
        />
      </template>
    </EmptyState>

    <AppSectionCard v-else titulo="Grupos Tecnicos" subtitulo="Resultados da listagem administrativa." icon="groups">
      <q-banner v-if="erro" rounded class="bg-red-1 text-negative q-mb-md">
        {{ erro }}
      </q-banner>

      <q-table
        flat
        bordered
        row-key="id"
        :rows="grupos"
        :columns="colunas"
        :loading="loading"
        :pagination="{ rowsPerPage: 0 }"
        hide-pagination
      >
        <template #body-cell-descricao="slotProps">
          <q-td :props="slotProps">
            {{ slotProps.row.descricao || '-' }}
          </q-td>
        </template>

        <template #body-cell-ativo="slotProps">
          <q-td :props="slotProps">
            <q-badge :color="slotProps.row.ativo ? 'positive' : 'grey-7'">
              {{ slotProps.row.ativo ? 'Ativo' : 'Inativo' }}
            </q-badge>
          </q-td>
        </template>

        <template #body-cell-criadoEm="slotProps">
          <q-td :props="slotProps">{{ formatarData(slotProps.row.criadoEm) }}</q-td>
        </template>

        <template #body-cell-atualizadoEm="slotProps">
          <q-td :props="slotProps">{{ formatarData(slotProps.row.atualizadoEm) }}</q-td>
        </template>

        <template #body-cell-acoes="slotProps">
          <q-td :props="slotProps" class="text-right">
            <q-btn
              flat
              round
              dense
              color="secondary"
              icon="group_add"
              aria-label="Ver membros do grupo tecnico"
              @click="abrirMembros(slotProps.row)"
            >
              <q-tooltip>Membros do grupo tecnico</q-tooltip>
            </q-btn>
            <q-btn
              flat
              round
              dense
              color="secondary"
              icon="list_alt"
              aria-label="Ver filas do grupo tecnico"
              @click="abrirFilas(slotProps.row)"
            >
              <q-tooltip>Filas do grupo tecnico</q-tooltip>
            </q-btn>
            <q-btn
              v-if="podeGerenciar"
              flat
              round
              dense
              color="primary"
              icon="edit"
              aria-label="Editar grupo tecnico"
              @click="abrirEdicao(slotProps.row)"
            >
              <q-tooltip>Editar grupo tecnico</q-tooltip>
            </q-btn>
            <q-btn
              v-if="podeGerenciar"
              flat
              round
              dense
              :color="slotProps.row.ativo ? 'negative' : 'positive'"
              :icon="slotProps.row.ativo ? 'block' : 'check_circle'"
              :aria-label="slotProps.row.ativo ? 'Inativar grupo tecnico' : 'Ativar grupo tecnico'"
              @click="abrirConfirmacaoStatus(slotProps.row)"
            >
              <q-tooltip>{{ slotProps.row.ativo ? 'Inativar' : 'Ativar' }}</q-tooltip>
            </q-btn>
            <q-badge v-if="!podeGerenciar" color="grey-6">Somente leitura</q-badge>
          </q-td>
        </template>
      </q-table>

      <div class="row items-center justify-between q-gutter-md q-mt-md grupos-tecnicos-admin__paginacao">
        <div class="text-caption text-grey-7">
          {{ total }} grupo(s) encontrado(s)
        </div>
        <div class="row items-center q-gutter-sm">
          <q-select
            :model-value="tamanhoPagina"
            dense
            outlined
            emit-value
            map-options
            :options="[
              { label: '10', value: 10 },
              { label: '20', value: 20 },
              { label: '50', value: 50 },
            ]"
            label="Por pagina"
            style="width: 128px"
            @update:model-value="atualizarTamanhoPagina"
          />
          <q-pagination
            :model-value="pagina"
            :max="Math.max(1, Math.ceil(total / tamanhoPagina))"
            :disable="loading"
            direction-links
            boundary-links
            @update:model-value="atualizarPagina"
          />
        </div>
      </div>
    </AppSectionCard>

    <q-dialog v-model="dialogoFormularioAberto" persistent>
      <q-card class="grupos-tecnicos-admin__dialog">
        <q-card-section class="row items-center">
          <div class="text-h6">{{ tituloFormulario }}</div>
          <q-space />
          <q-btn icon="close" flat round dense aria-label="Fechar formulario" :disable="salvando" v-close-popup />
        </q-card-section>

        <q-separator />

        <q-card-section>
          <q-form class="column q-gutter-md" @submit.prevent="salvar">
            <q-input
              v-model="form.nome"
              outlined
              dense
              label="Nome"
              maxlength="120"
              counter
              :error="nomeInvalido"
              error-message="Nome e obrigatorio."
              @blur="tentativaSubmit = true"
            />
            <q-input
              v-model="form.descricao"
              outlined
              dense
              type="textarea"
              autogrow
              maxlength="500"
              counter
              label="Descricao"
            />

            <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
              {{ erro }}
            </q-banner>

            <div class="row justify-end q-gutter-sm">
              <q-btn flat label="Cancelar" :disable="salvando" v-close-popup />
              <q-btn type="submit" color="primary" icon="save" label="Salvar" :loading="salvando" unelevated />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>

    <ConfirmDialog
      v-model="dialogoStatusAberto"
      :titulo="`${labelAcaoStatus} grupo tecnico`"
      :mensagem="mensagemAcaoStatus"
      :confirmar-label="labelAcaoStatus"
      :color="grupoSelecionado?.ativo ? 'negative' : 'positive'"
      :loading="atualizandoStatus"
      @confirm="alternarStatus"
    />
  </q-page>
</template>

<style scoped>
.grupos-tecnicos-admin__kpis {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

.grupos-tecnicos-admin__dialog {
  width: min(620px, 94vw);
}

.grupos-tecnicos-admin__paginacao {
  min-width: 0;
}

@media (max-width: 900px) {
  .grupos-tecnicos-admin__kpis {
    grid-template-columns: minmax(0, 1fr);
  }

  .grupos-tecnicos-admin__paginacao {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
