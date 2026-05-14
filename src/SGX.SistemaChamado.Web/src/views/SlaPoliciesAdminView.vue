<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { useAuthStore } from '../stores/authStore'
import { permissoes } from '../constants/permissoes'
import { slaPoliciesService } from '../services/slaPoliciesService'
import { cadastrosAdminService } from '../services/cadastrosAdminService'
import type { AtualizarPoliticaSlaRequest, CalendarioCorporativoResponse, CriarPoliticaSlaRequest, PoliticaSlaResponse } from '../types/slaPolicies'

type PrioridadeOption = { id: string; nome: string; nivel: number }
type MetaForm = {
  id?: string
  prioridadeId: string
  prioridadeNome: string
  prioridadeNivel: number
  ativo: boolean
  tempoPrimeiraRespostaMinutos: number
  tempoResolucaoMinutos: number
  tempoAtualizacaoMinutos: number | null
  tempoRespostaSubsequenteMinutos: number | null
}

const authStore = useAuthStore()

const loading = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const politicas = ref<PoliticaSlaResponse[]>([])
const prioridades = ref<PrioridadeOption[]>([])
const categorias = ref<{ label: string; value: string }[]>([])
const departamentos = ref<{ label: string; value: string }[]>([])
const calendarios = ref<{ label: string; value: string }[]>([])

const filtros = reactive({
  ativo: true as boolean | null,
  texto: '',
})

const modalAberto = ref(false)
const editandoId = ref<string | null>(null)
const salvando = ref(false)

const form = reactive({
  nome: '',
  descricao: '',
  ativo: true,
  ordem: 1,
  categoriaId: null as string | null,
  departamentoId: null as string | null,
  calendarioCorporativoId: null as string | null,
  usarHorarioComercial: false,
  pausarQuandoAguardandoSolicitante: true,
  metas: [] as MetaForm[],
})

const podeVisualizar = computed(
  () =>
    authStore.possuiPermissao(permissoes.slaVisualizar) ||
    (authStore.usuario?.perfis.includes('Administrador') && (authStore.usuario?.permissoes?.length ?? 0) === 0)
)

const podeCriar = computed(
  () =>
    authStore.possuiPermissao(permissoes.slaCriar) ||
    (authStore.usuario?.perfis.includes('Administrador') && (authStore.usuario?.permissoes?.length ?? 0) === 0)
)

const podeEditar = computed(
  () =>
    authStore.possuiPermissao(permissoes.slaEditar) ||
    (authStore.usuario?.perfis.includes('Administrador') && (authStore.usuario?.permissoes?.length ?? 0) === 0)
)

const podeAtivarDesativar = computed(
  () =>
    authStore.possuiPermissao(permissoes.slaAtivarDesativar) ||
    (authStore.usuario?.perfis.includes('Administrador') && (authStore.usuario?.permissoes?.length ?? 0) === 0)
)

const colunas: QTableColumn<PoliticaSlaResponse>[] = [
  { name: 'nome', label: 'Política', field: 'nome', align: 'left' },
  { name: 'escopo', label: 'Escopo', field: 'escopo', align: 'left' },
  { name: 'ordem', label: 'Ordem', field: 'ordem', align: 'center' },
  { name: 'horario', label: 'Horário comercial', field: 'usarHorarioComercial', align: 'center' },
  { name: 'pausa', label: 'Pausa aguardando solicitante', field: 'pausarQuandoAguardandoSolicitante', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' },
]

function formatarEscopo(politica: PoliticaSlaResponse): string {
  const partes = []
  if (politica.categoriaNome) partes.push(`Categoria: ${politica.categoriaNome}`)
  if (politica.departamentoNome) partes.push(`Departamento: ${politica.departamentoNome}`)
  return partes.length ? partes.join(' | ') : 'Geral'
}

function minutosParaLegenda(minutos: number): string {
  const horas = Math.floor(minutos / 60)
  const resto = minutos % 60
  if (horas > 0 && resto > 0) return `${horas}h ${resto}min`
  if (horas > 0) return `${horas}h`
  return `${minutos}min`
}

function preencherMetasDefault(): void {
  if (!prioridades.value.length) return
  form.metas = prioridades.value.map((prioridade) => ({
    prioridadeId: prioridade.id,
    prioridadeNome: prioridade.nome,
    prioridadeNivel: prioridade.nivel,
    ativo: true,
    tempoPrimeiraRespostaMinutos: prioridade.nivel <= 2 ? 240 : prioridade.nivel === 3 ? 60 : 30,
    tempoResolucaoMinutos: prioridade.nivel <= 2 ? 1440 : prioridade.nivel === 3 ? 480 : 240,
    tempoAtualizacaoMinutos: null,
    tempoRespostaSubsequenteMinutos: null,
  }))
}

async function carregarReferencias(): Promise<void> {
  const [prioridadesResponse, categoriasResponse, departamentosResponse, calendariosResponse] = await Promise.all([
    cadastrosAdminService.listarPrioridades({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 }),
    cadastrosAdminService.listarDepartamentos({ ativo: true, tamanhoPagina: 100 }),
    slaPoliciesService.listarCalendarios(),
  ])

  prioridades.value = prioridadesResponse.items
    .filter((item) => item.ativo)
    .map((item) => ({ id: item.id, nome: item.nome, nivel: item.nivel }))
    .sort((a, b) => a.nivel - b.nivel || a.nome.localeCompare(b.nome))

  categorias.value = categoriasResponse.items.filter((item) => item.ativo).map((item) => ({ label: item.nome, value: item.id }))
  departamentos.value = departamentosResponse.items.filter((item) => item.ativo).map((item) => ({ label: item.nome, value: item.id }))
  calendarios.value = calendariosResponse
    .filter((item: CalendarioCorporativoResponse) => item.ativo)
    .map((item) => ({ label: `${item.nome}${item.padrao ? ' (padrão)' : ''}`, value: item.id }))
}

async function carregarPoliticas(): Promise<void> {
  if (!podeVisualizar.value) {
    erro.value = 'Sem permissão para visualizar políticas de SLA.'
    return
  }

  loading.value = true
  erro.value = null
  try {
    politicas.value = await slaPoliciesService.listar({
      ativo: filtros.ativo ?? undefined,
      texto: filtros.texto || undefined,
    })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar políticas de SLA.'
  } finally {
    loading.value = false
  }
}

function abrirCriacao(): void {
  editandoId.value = null
  form.nome = ''
  form.descricao = ''
  form.ativo = true
  form.ordem = 1
  form.categoriaId = null
  form.departamentoId = null
  form.calendarioCorporativoId = null
  form.usarHorarioComercial = false
  form.pausarQuandoAguardandoSolicitante = true
  preencherMetasDefault()
  modalAberto.value = true
}

function abrirEdicao(politica: PoliticaSlaResponse): void {
  editandoId.value = politica.id
  form.nome = politica.nome
  form.descricao = politica.descricao ?? ''
  form.ativo = politica.ativo
  form.ordem = politica.ordem
  form.categoriaId = politica.categoriaId
  form.departamentoId = politica.departamentoId
  form.calendarioCorporativoId = politica.calendarioCorporativoId
  form.usarHorarioComercial = politica.usarHorarioComercial
  form.pausarQuandoAguardandoSolicitante = politica.pausarQuandoAguardandoSolicitante
  form.metas = politica.metas.map((meta) => ({
    id: meta.id,
    prioridadeId: meta.prioridadeId,
    prioridadeNome: meta.prioridadeNome,
    prioridadeNivel: meta.prioridadeNivel,
    ativo: meta.ativo,
    tempoPrimeiraRespostaMinutos: meta.tempoPrimeiraRespostaMinutos,
    tempoResolucaoMinutos: meta.tempoResolucaoMinutos,
    tempoAtualizacaoMinutos: meta.tempoAtualizacaoMinutos,
    tempoRespostaSubsequenteMinutos: meta.tempoRespostaSubsequenteMinutos,
  }))
  modalAberto.value = true
}

function validarFormulario(): string | null {
  if (!form.nome.trim()) return 'Nome da política é obrigatório.'
  if (form.ordem <= 0) return 'Ordem deve ser maior que zero.'
  if (!form.metas.length) return 'A política deve possuir metas de SLA.'

  const prioridadesAtivas = new Set<string>()
  for (const meta of form.metas) {
    if (meta.tempoPrimeiraRespostaMinutos <= 0) return `Tempo de primeira resposta inválido para ${meta.prioridadeNome}.`
    if (meta.tempoResolucaoMinutos <= 0) return `Tempo de resolução inválido para ${meta.prioridadeNome}.`
    if (meta.tempoAtualizacaoMinutos !== null && meta.tempoAtualizacaoMinutos <= 0) return `Tempo de atualização inválido para ${meta.prioridadeNome}.`
    if (meta.tempoRespostaSubsequenteMinutos !== null && meta.tempoRespostaSubsequenteMinutos <= 0) return `Tempo de resposta subsequente inválido para ${meta.prioridadeNome}.`

    if (meta.ativo) {
      if (prioridadesAtivas.has(meta.prioridadeId)) return 'Não é permitido repetir prioridade ativa na mesma política.'
      prioridadesAtivas.add(meta.prioridadeId)
    }
  }

  if (!prioridadesAtivas.size) return 'Ao menos uma meta ativa é obrigatória.'
  return null
}

async function salvar(): Promise<void> {
  const erroValidacao = validarFormulario()
  if (erroValidacao) {
    erro.value = erroValidacao
    return
  }

  salvando.value = true
  erro.value = null
  try {
    const basePayload = {
      nome: form.nome.trim(),
      descricao: form.descricao.trim() || null,
      ativo: form.ativo,
      ordem: form.ordem,
      categoriaId: form.categoriaId,
      departamentoId: form.departamentoId,
      calendarioCorporativoId: form.usarHorarioComercial ? form.calendarioCorporativoId : null,
      usarHorarioComercial: form.usarHorarioComercial,
      pausarQuandoAguardandoSolicitante: form.pausarQuandoAguardandoSolicitante,
      metas: form.metas.map((meta) => ({
        id: meta.id ?? null,
        prioridadeId: meta.prioridadeId,
        tempoPrimeiraRespostaMinutos: meta.tempoPrimeiraRespostaMinutos,
        tempoResolucaoMinutos: meta.tempoResolucaoMinutos,
        tempoAtualizacaoMinutos: meta.tempoAtualizacaoMinutos ?? null,
        tempoRespostaSubsequenteMinutos: meta.tempoRespostaSubsequenteMinutos ?? null,
        ativo: meta.ativo,
      })),
    }

    if (editandoId.value) {
      await slaPoliciesService.atualizar(editandoId.value, basePayload as AtualizarPoliticaSlaRequest)
      sucesso.value = 'Política de SLA atualizada com sucesso.'
    } else {
      await slaPoliciesService.criar(basePayload as CriarPoliticaSlaRequest)
      sucesso.value = 'Política de SLA criada com sucesso.'
    }

    modalAberto.value = false
    await carregarPoliticas()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar política de SLA.'
  } finally {
    salvando.value = false
  }
}

async function alternarStatus(politica: PoliticaSlaResponse): Promise<void> {
  if (!podeAtivarDesativar.value) return
  try {
    await slaPoliciesService.atualizarStatus(politica.id, { ativo: !politica.ativo })
    await carregarPoliticas()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao atualizar status da política.'
  }
}

onMounted(async () => {
  await carregarReferencias()
  await carregarPoliticas()
})
</script>

<template>
  <q-page class="q-pa-md">
    <PageHeader title="Políticas de SLA" subtitle="Administração > SLA" />

    <q-banner v-if="sucesso" class="bg-positive text-white q-mb-md" rounded>
      {{ sucesso }}
    </q-banner>

    <AppSectionCard title="Configuração de SLA" icon="schedule">
      <div class="row q-col-gutter-sm q-mb-md">
        <div class="col-12 col-md-3">
          <q-select
            v-model="filtros.ativo"
            outlined
            dense
            emit-value
            map-options
            :options="[
              { label: 'Ativas', value: true },
              { label: 'Inativas', value: false },
              { label: 'Todas', value: null },
            ]"
            label="Situação"
          />
        </div>
        <div class="col-12 col-md-5">
          <q-input v-model="filtros.texto" outlined dense label="Buscar por nome ou descrição" @keyup.enter="carregarPoliticas" />
        </div>
        <div class="col-12 col-md-4 row justify-end q-gutter-sm">
          <q-btn color="primary" label="Filtrar" @click="carregarPoliticas" />
          <q-btn v-if="podeCriar" color="secondary" icon="add" label="Nova política" @click="abrirCriacao" />
        </div>
      </div>

      <LoadingState v-if="loading" />
      <ErrorState v-else-if="erro" :mensagem="erro" @tentar-novamente="carregarPoliticas" />
      <q-table v-else :rows="politicas" :columns="colunas" row-key="id" flat bordered>
        <template #body-cell-escopo="slotProps">
          <q-td>{{ formatarEscopo(slotProps.row) }}</q-td>
        </template>
        <template #body-cell-horario="slotProps">
          <q-td>
            <q-badge :color="slotProps.row.usarHorarioComercial ? 'primary' : 'grey-7'">
              {{ slotProps.row.usarHorarioComercial ? 'Sim' : 'Não' }}
            </q-badge>
            <div v-if="slotProps.row.usarHorarioComercial" class="text-caption text-grey-7 q-mt-xs">
              {{ slotProps.row.calendarioCorporativoNome || 'Calendário padrão' }}
            </div>
          </q-td>
        </template>
        <template #body-cell-pausa="slotProps">
          <q-td>
            <q-badge :color="slotProps.row.pausarQuandoAguardandoSolicitante ? 'positive' : 'warning'">
              {{ slotProps.row.pausarQuandoAguardandoSolicitante ? 'Sim' : 'Não' }}
            </q-badge>
          </q-td>
        </template>
        <template #body-cell-ativo="slotProps">
          <q-td>
            <q-badge :color="slotProps.row.ativo ? 'positive' : 'negative'">{{ slotProps.row.ativo ? 'Sim' : 'Não' }}</q-badge>
          </q-td>
        </template>
        <template #body-cell-acoes="slotProps">
          <q-td class="text-right">
            <q-btn v-if="podeEditar" flat dense round icon="edit" color="secondary" @click="abrirEdicao(slotProps.row)" />
            <q-btn
              v-if="podeAtivarDesativar"
              flat
              dense
              round
              :icon="slotProps.row.ativo ? 'toggle_on' : 'toggle_off'"
              :color="slotProps.row.ativo ? 'positive' : 'grey-7'"
              @click="alternarStatus(slotProps.row)"
            />
          </q-td>
        </template>
      </q-table>
    </AppSectionCard>

    <q-dialog v-model="modalAberto" maximized>
      <q-card>
        <q-card-section class="row items-center">
          <div class="text-h6">{{ editandoId ? 'Editar política de SLA' : 'Nova política de SLA' }}</div>
          <q-space />
          <q-btn icon="close" flat round dense v-close-popup />
        </q-card-section>

        <q-card-section class="row q-col-gutter-sm">
          <div class="col-12 col-md-6"><q-input v-model="form.nome" outlined dense label="Nome" /></div>
          <div class="col-12 col-md-6"><q-input v-model.number="form.ordem" outlined dense type="number" min="1" label="Ordem" /></div>
          <div class="col-12"><q-input v-model="form.descricao" outlined dense type="textarea" autogrow label="Descrição" /></div>
          <div class="col-12 col-md-6"><q-select v-model="form.categoriaId" outlined dense clearable emit-value map-options :options="categorias" label="Categoria (opcional)" /></div>
          <div class="col-12 col-md-6"><q-select v-model="form.departamentoId" outlined dense clearable emit-value map-options :options="departamentos" label="Departamento (opcional)" /></div>
          <div class="col-12 col-md-4"><q-toggle v-model="form.usarHorarioComercial" label="Usar horário comercial" /></div>
          <div class="col-12 col-md-4">
            <q-select
              v-model="form.calendarioCorporativoId"
              outlined
              dense
              clearable
              emit-value
              map-options
              :disable="!form.usarHorarioComercial"
              :options="calendarios"
              label="Calendário corporativo"
              hint="Sem seleção usa o calendário padrão"
            />
          </div>
          <div class="col-12 col-md-4"><q-toggle v-model="form.pausarQuandoAguardandoSolicitante" label="Pausar quando aguardando solicitante" /></div>
          <div class="col-12 col-md-4"><q-toggle v-model="form.ativo" label="Ativo" /></div>

          <div class="col-12">
            <div class="text-subtitle1 text-weight-bold q-mb-sm">Metas por prioridade</div>
            <q-markup-table flat bordered>
              <thead>
                <tr>
                  <th>Prioridade</th>
                  <th>Primeira resposta (min)</th>
                  <th>Resolução (min)</th>
                  <th>Atualização (min)</th>
                  <th>Resposta subsequente (min)</th>
                  <th>Ativa</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="meta in form.metas" :key="meta.prioridadeId">
                  <td>
                    <div class="text-weight-medium">{{ meta.prioridadeNome }}</div>
                  </td>
                  <td>
                    <q-input v-model.number="meta.tempoPrimeiraRespostaMinutos" dense outlined type="number" min="1" />
                    <div class="text-caption text-grey-7">{{ minutosParaLegenda(meta.tempoPrimeiraRespostaMinutos) }}</div>
                  </td>
                  <td>
                    <q-input v-model.number="meta.tempoResolucaoMinutos" dense outlined type="number" min="1" />
                    <div class="text-caption text-grey-7">{{ minutosParaLegenda(meta.tempoResolucaoMinutos) }}</div>
                  </td>
                  <td>
                    <q-input v-model.number="meta.tempoAtualizacaoMinutos" dense outlined type="number" min="1" />
                  </td>
                  <td>
                    <q-input v-model.number="meta.tempoRespostaSubsequenteMinutos" dense outlined type="number" min="1" />
                  </td>
                  <td>
                    <q-toggle v-model="meta.ativo" />
                  </td>
                </tr>
              </tbody>
            </q-markup-table>
          </div>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" :loading="salvando" @click="salvar" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
