<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
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
import type { AtendenteResumo, GrupoTecnicoDetalhe, MembroGrupoTecnicoResponse } from '../types/admin'

type FiltroStatus = 'todos' | 'ativos' | 'inativos'
type OpcaoUsuario = {
  label: string
  value: string
  caption: string
}

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const grupo = ref<GrupoTecnicoDetalhe | null>(null)
const membros = ref<MembroGrupoTecnicoResponse[]>([])
const atendentes = ref<AtendenteResumo[]>([])
const loading = ref(false)
const salvando = ref(false)
const atualizandoStatus = ref(false)
const erro = ref<string | null>(null)
const filtroStatus = ref<FiltroStatus>('ativos')
const dialogoAdicionarAberto = ref(false)
const dialogoStatusAberto = ref(false)
const membroSelecionado = ref<MembroGrupoTecnicoResponse | null>(null)
const tentativaSubmit = ref(false)

const form = reactive({
  usuarioId: '',
})

const grupoTecnicoId = computed(() => String(route.params.id ?? ''))
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const usuarioEhAtendente = computed(() => (authStore.usuario?.perfis ?? []).includes('Atendente'))
const podeVisualizar = computed(() => usuarioEhAdministrador.value || usuarioEhAtendente.value)
const podeGerenciar = computed(() => usuarioEhAdministrador.value)
const totalAtivos = computed(() => membros.value.filter((membro) => membro.ativo).length)
const totalInativos = computed(() => membros.value.length - totalAtivos.value)
const usuarioInvalido = computed(() => tentativaSubmit.value && !form.usuarioId)
const opcoesUsuarios = computed<OpcaoUsuario[]>(() =>
  atendentes.value.map((atendente) => ({
    label: atendente.nome,
    value: atendente.id,
    caption: atendente.email,
  }))
)
const labelAcaoStatus = computed(() => (membroSelecionado.value?.ativo ? 'Inativar' : 'Ativar'))
const mensagemAcaoStatus = computed(() => {
  const nome = membroSelecionado.value?.usuarioNome ? ` "${membroSelecionado.value.usuarioNome}"` : ''
  return membroSelecionado.value?.ativo
    ? `Deseja inativar o membro${nome}?`
    : `Deseja ativar o membro${nome}?`
})

const colunas: QTableColumn<MembroGrupoTecnicoResponse>[] = [
  { name: 'usuarioNome', label: 'Nome do usuario', field: 'usuarioNome', align: 'left', sortable: true },
  { name: 'usuarioEmail', label: 'E-mail', field: 'usuarioEmail', align: 'left', sortable: true },
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

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    erro.value = 'Sem permissao para visualizar membros de grupos tecnicos.'
    return
  }

  loading.value = true
  erro.value = null

  try {
    const [grupoResponse, membrosResponse, contextoResponse] = await Promise.all([
      adminService.obterGrupoTecnico(grupoTecnicoId.value),
      adminService.listarMembrosGrupoTecnico(grupoTecnicoId.value, { ativo: ativoFiltro() }),
      podeGerenciar.value ? adminService.obterAdminContexto() : Promise.resolve(null),
    ])

    grupo.value = grupoResponse
    membros.value = membrosResponse
    atendentes.value = contextoResponse?.atendentes ?? []
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os membros do grupo tecnico.'
  } finally {
    loading.value = false
  }
}

function aplicarFiltros(): void {
  void carregar()
}

function limparFiltros(): void {
  filtroStatus.value = 'ativos'
  void carregar()
}

function abrirAdicionar(): void {
  if (!podeGerenciar.value) return
  tentativaSubmit.value = false
  form.usuarioId = ''
  dialogoAdicionarAberto.value = true
}

async function adicionarMembro(): Promise<void> {
  tentativaSubmit.value = true
  if (!form.usuarioId || !podeGerenciar.value) {
    return
  }

  salvando.value = true
  erro.value = null

  try {
    await adminService.adicionarMembroGrupoTecnico(grupoTecnicoId.value, { usuarioId: form.usuarioId })
    $q.notify({ type: 'positive', message: 'Membro adicionado ao grupo tecnico com sucesso.' })
    dialogoAdicionarAberto.value = false
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel adicionar o membro ao grupo tecnico.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

function abrirConfirmacaoStatus(membro: MembroGrupoTecnicoResponse): void {
  if (!podeGerenciar.value) return
  membroSelecionado.value = membro
  dialogoStatusAberto.value = true
}

async function alternarStatus(): Promise<void> {
  if (!membroSelecionado.value || !podeGerenciar.value) {
    return
  }

  atualizandoStatus.value = true
  erro.value = null

  try {
    const ativo = !membroSelecionado.value.ativo
    await adminService.alterarStatusMembroGrupoTecnico(grupoTecnicoId.value, membroSelecionado.value.id, { ativo })
    $q.notify({
      type: 'positive',
      message: ativo ? 'Membro ativado com sucesso.' : 'Membro inativado com sucesso.',
    })
    dialogoStatusAberto.value = false
    membroSelecionado.value = null
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel alterar o status do membro.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    atualizandoStatus.value = false
  }
}

function voltar(): void {
  router.push('/admin/cadastros/grupos-tecnicos')
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md grupo-membros-admin">
    <PageHeader
      contexto="Cadastros administrativos"
      titulo="Membros do grupo tecnico"
      :subtitulo="grupo ? `${grupo.nome} - ${grupo.ativo ? 'Ativo' : 'Inativo'}` : 'Grupo tecnico'"
    >
      <template #actions>
        <q-btn flat icon="arrow_back" label="Voltar" :disable="loading" @click="voltar" />
        <q-btn
          v-if="podeGerenciar"
          color="primary"
          icon="person_add"
          label="Adicionar membro"
          unelevated
          :disable="loading || !grupo"
          @click="abrirAdicionar"
        />
      </template>
    </PageHeader>

    <div class="grupo-membros-admin__kpis">
      <MetricCard titulo="Membros listados" :valor="membros.length" icon="group_add" :loading="loading" />
      <MetricCard titulo="Ativos" :valor="totalAtivos" icon="check_circle" tone="positive" :loading="loading" />
      <MetricCard titulo="Inativos" :valor="totalInativos" icon="pause_circle" tone="warning" :loading="loading" />
    </div>

    <AppSectionCard titulo="Identificacao do grupo" subtitulo="Dados principais do grupo tecnico." icon="groups">
      <div v-if="grupo" class="row q-col-gutter-md">
        <div class="col-12 col-md-7">
          <div class="text-caption text-grey-7">Nome</div>
          <div class="text-subtitle1 text-weight-medium">{{ grupo.nome }}</div>
          <div class="text-body2 text-grey-7">{{ grupo.descricao || 'Sem descricao cadastrada.' }}</div>
        </div>
        <div class="col-12 col-md-5">
          <div class="text-caption text-grey-7">Status</div>
          <q-badge :color="grupo.ativo ? 'positive' : 'grey-7'">
            {{ grupo.ativo ? 'Ativo' : 'Inativo' }}
          </q-badge>
        </div>
      </div>
      <q-skeleton v-else type="text" />
    </AppSectionCard>

    <AppSectionCard titulo="Filtros" subtitulo="Filtre membros pela situacao." icon="filter_alt">
      <FilterBar compact>
        <q-form class="row q-col-gutter-sm items-end" @submit.prevent="aplicarFiltros">
          <div class="col-12 col-md-6">
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
          <div class="col-12 col-md-6 row justify-end q-gutter-sm">
            <q-btn type="submit" color="primary" icon="search" label="Filtrar" :loading="loading" unelevated />
            <q-btn flat icon="filter_alt_off" label="Limpar" :disable="loading" @click="limparFiltros" />
          </div>
        </q-form>
      </FilterBar>
    </AppSectionCard>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar membros de grupos tecnicos.
    </q-banner>

    <LoadingState v-else-if="loading && !membros.length" mensagem="Carregando membros do grupo tecnico..." />

    <ErrorState
      v-else-if="erro && !membros.length"
      titulo="Nao foi possivel carregar membros"
      :mensagem="erro"
      @retry="carregar"
    />

    <EmptyState
      v-else-if="!membros.length"
      titulo="Nenhum membro encontrado"
      mensagem="Nenhum membro corresponde aos filtros aplicados."
      icon="group_off"
    >
      <template #actions>
        <q-btn v-if="podeGerenciar" color="primary" icon="person_add" label="Adicionar membro" unelevated @click="abrirAdicionar" />
      </template>
    </EmptyState>

    <AppSectionCard v-else titulo="Membros" subtitulo="Usuarios vinculados ao grupo tecnico." icon="group_add">
      <q-banner v-if="erro" rounded class="bg-red-1 text-negative q-mb-md">
        {{ erro }}
      </q-banner>

      <q-table
        flat
        bordered
        row-key="id"
        :rows="membros"
        :columns="colunas"
        :loading="loading"
        :pagination="{ rowsPerPage: 0 }"
        hide-pagination
      >
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
              v-if="podeGerenciar"
              flat
              round
              dense
              :color="slotProps.row.ativo ? 'negative' : 'positive'"
              :icon="slotProps.row.ativo ? 'person_off' : 'person'"
              :aria-label="slotProps.row.ativo ? 'Inativar membro' : 'Ativar membro'"
              @click="abrirConfirmacaoStatus(slotProps.row)"
            >
              <q-tooltip>{{ slotProps.row.ativo ? 'Inativar' : 'Ativar' }}</q-tooltip>
            </q-btn>
            <q-badge v-if="!podeGerenciar" color="grey-6">Somente leitura</q-badge>
          </q-td>
        </template>
      </q-table>
    </AppSectionCard>

    <q-dialog v-model="dialogoAdicionarAberto" persistent>
      <q-card class="grupo-membros-admin__dialog">
        <q-card-section class="row items-center">
          <div class="text-h6">Adicionar membro</div>
          <q-space />
          <q-btn icon="close" flat round dense aria-label="Fechar formulario" :disable="salvando" v-close-popup />
        </q-card-section>

        <q-separator />

        <q-card-section>
          <q-form class="column q-gutter-md" @submit.prevent="adicionarMembro">
            <q-select
              v-model="form.usuarioId"
              outlined
              dense
              emit-value
              map-options
              label="Usuario"
              :options="opcoesUsuarios"
              :loading="loading"
              :error="usuarioInvalido"
              error-message="Usuario e obrigatorio."
              option-label="label"
              option-value="value"
              @blur="tentativaSubmit = true"
            >
              <template #option="scope">
                <q-item v-bind="scope.itemProps">
                  <q-item-section>
                    <q-item-label>{{ scope.opt.label }}</q-item-label>
                    <q-item-label caption>{{ scope.opt.caption }}</q-item-label>
                  </q-item-section>
                </q-item>
              </template>
              <template #no-option>
                <q-item>
                  <q-item-section class="text-grey">Nenhum atendente disponivel.</q-item-section>
                </q-item>
              </template>
            </q-select>

            <q-banner rounded class="bg-blue-1 text-blue-10">
              A lista usa os atendentes retornados pelo contexto administrativo. O backend valida duplicidade e existencia do usuario.
            </q-banner>

            <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
              {{ erro }}
            </q-banner>

            <div class="row justify-end q-gutter-sm">
              <q-btn flat label="Cancelar" :disable="salvando" v-close-popup />
              <q-btn type="submit" color="primary" icon="save" label="Adicionar" :loading="salvando" unelevated />
            </div>
          </q-form>
        </q-card-section>
      </q-card>
    </q-dialog>

    <ConfirmDialog
      v-model="dialogoStatusAberto"
      :titulo="`${labelAcaoStatus} membro`"
      :mensagem="mensagemAcaoStatus"
      :confirmar-label="labelAcaoStatus"
      :color="membroSelecionado?.ativo ? 'negative' : 'positive'"
      :loading="atualizandoStatus"
      @confirm="alternarStatus"
    />
  </q-page>
</template>

<style scoped>
.grupo-membros-admin__kpis {
  display: grid;
  grid-template-columns: repeat(3, minmax(0, 1fr));
  gap: var(--sgx-space-4);
}

.grupo-membros-admin__dialog {
  width: min(620px, 94vw);
}

@media (max-width: 900px) {
  .grupo-membros-admin__kpis {
    grid-template-columns: minmax(0, 1fr);
  }
}
</style>
