<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QTableColumn } from 'quasar'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import EmptyState from '../components/ui/EmptyState.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { slaPoliciesService } from '../services/slaPoliciesService'
import type {
  CalendarioCorporativoResponse,
  ExcecaoCalendarioCorporativoResponse,
  HorarioAtendimentoCalendarioResponse,
} from '../types/slaPolicies'

const loading = ref(false)
const salvando = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const calendarios = ref<CalendarioCorporativoResponse[]>([])
const calendarioSelecionadoId = ref<string | null>(null)

const calendarioSelecionado = computed(
  () => calendarios.value.find((item) => item.id === calendarioSelecionadoId.value) ?? calendarios.value[0] ?? null
)
const totalCalendariosAtivos = computed(() => calendarios.value.filter((item) => item.ativo).length)
const totalCalendariosInativos = computed(() => calendarios.value.filter((item) => !item.ativo).length)
const totalCalendariosPadrao = computed(() => calendarios.value.filter((item) => item.padrao).length)
const totalExcecoesSelecionadas = computed(() => calendarioSelecionado.value?.excecoes.length ?? 0)
const totalHorariosSelecionados = computed(() => calendarioSelecionado.value?.horariosAtendimento.length ?? 0)

const calendarioForm = reactive({
  nome: '',
  descricao: '',
  ativo: true,
  padrao: false,
  timeZone: 'America/Sao_Paulo',
})

const horarioForm = reactive({
  id: null as string | null,
  diaSemana: 1,
  horaInicio: '09:00',
  horaFim: '18:00',
  ativo: true,
})

const excecaoForm = reactive({
  id: null as string | null,
  data: '',
  tipo: 1,
  descricao: '',
  horaInicio: '',
  horaFim: '',
  ativo: true,
})

const diasSemana = [
  { label: 'Domingo', value: 0 },
  { label: 'Segunda-feira', value: 1 },
  { label: 'Terca-feira', value: 2 },
  { label: 'Quarta-feira', value: 3 },
  { label: 'Quinta-feira', value: 4 },
  { label: 'Sexta-feira', value: 5 },
  { label: 'Sabado', value: 6 },
]

const tiposExcecao = [
  { label: 'Feriado', value: 1 },
  { label: 'Recesso', value: 2 },
  { label: 'Expediente especial', value: 3 },
  { label: 'Sem expediente', value: 4 },
]

const colunasCalendario: QTableColumn<CalendarioCorporativoResponse>[] = [
  { name: 'nome', label: 'Calendario', field: 'nome', align: 'left' },
  { name: 'timeZone', label: 'Time zone', field: 'timeZone', align: 'left' },
  { name: 'padrao', label: 'Padrao', field: 'padrao', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: '', field: 'id', align: 'right' },
]

const colunasHorario: QTableColumn<HorarioAtendimentoCalendarioResponse>[] = [
  { name: 'dia', label: 'Dia', field: 'diaSemanaNome', align: 'left' },
  { name: 'inicio', label: 'Inicio', field: 'horaInicio', align: 'center' },
  { name: 'fim', label: 'Fim', field: 'horaFim', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: '', field: 'id', align: 'right' },
]

const colunasExcecao: QTableColumn<ExcecaoCalendarioCorporativoResponse>[] = [
  { name: 'data', label: 'Data', field: 'data', align: 'left' },
  { name: 'tipo', label: 'Tipo', field: 'tipoDescricao', align: 'left' },
  { name: 'periodo', label: 'Periodo', field: 'id', align: 'center' },
  { name: 'ativo', label: 'Ativo', field: 'ativo', align: 'center' },
  { name: 'acoes', label: '', field: 'id', align: 'right' },
]

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null
  try {
    calendarios.value = await slaPoliciesService.listarCalendarios()
    if (
      !calendarioSelecionadoId.value ||
      !calendarios.value.some((item) => item.id === calendarioSelecionadoId.value)
    ) {
      calendarioSelecionadoId.value = calendarios.value[0]?.id ?? null
    }
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar calendarios de SLA.'
  } finally {
    loading.value = false
  }
}

function novoCalendario(): void {
  calendarioSelecionadoId.value = null
  calendarioForm.nome = ''
  calendarioForm.descricao = ''
  calendarioForm.ativo = true
  calendarioForm.padrao = false
  calendarioForm.timeZone = 'America/Sao_Paulo'
}

function editarCalendario(calendario: CalendarioCorporativoResponse): void {
  calendarioSelecionadoId.value = calendario.id
  calendarioForm.nome = calendario.nome
  calendarioForm.descricao = calendario.descricao ?? ''
  calendarioForm.ativo = calendario.ativo
  calendarioForm.padrao = calendario.padrao
  calendarioForm.timeZone = calendario.timeZone
}

async function salvarCalendario(): Promise<void> {
  if (!calendarioForm.nome.trim()) {
    erro.value = 'Nome do calendario e obrigatorio.'
    return
  }

  salvando.value = true
  erro.value = null
  try {
    if (calendarioSelecionadoId.value) {
      const salvo = await slaPoliciesService.atualizarCalendario(calendarioSelecionadoId.value, {
        nome: calendarioForm.nome.trim(),
        descricao: calendarioForm.descricao.trim() || null,
        timeZone: calendarioForm.timeZone.trim() || 'America/Sao_Paulo',
      })
      await slaPoliciesService.atualizarStatusCalendario(salvo.id, { ativo: calendarioForm.ativo })
      if (calendarioForm.padrao) await slaPoliciesService.definirCalendarioPadrao(salvo.id)
      sucesso.value = 'Calendario atualizado.'
    } else {
      const salvo = await slaPoliciesService.criarCalendario({
        nome: calendarioForm.nome.trim(),
        descricao: calendarioForm.descricao.trim() || null,
        ativo: calendarioForm.ativo,
        padrao: calendarioForm.padrao,
        timeZone: calendarioForm.timeZone.trim() || 'America/Sao_Paulo',
      })
      calendarioSelecionadoId.value = salvo.id
      sucesso.value = 'Calendario criado.'
    }

    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao salvar calendario.'
  } finally {
    salvando.value = false
  }
}

async function definirPadrao(calendario: CalendarioCorporativoResponse): Promise<void> {
  await slaPoliciesService.definirCalendarioPadrao(calendario.id)
  sucesso.value = 'Calendario padrao atualizado.'
  await carregar()
}

async function alternarStatus(calendario: CalendarioCorporativoResponse): Promise<void> {
  await slaPoliciesService.atualizarStatusCalendario(calendario.id, { ativo: !calendario.ativo })
  await carregar()
}

function limparHorario(): void {
  horarioForm.id = null
  horarioForm.diaSemana = 1
  horarioForm.horaInicio = '09:00'
  horarioForm.horaFim = '18:00'
  horarioForm.ativo = true
}

function editarHorario(horario: HorarioAtendimentoCalendarioResponse): void {
  horarioForm.id = horario.id
  horarioForm.diaSemana = horario.diaSemana
  horarioForm.horaInicio = horario.horaInicio
  horarioForm.horaFim = horario.horaFim
  horarioForm.ativo = horario.ativo
}

async function salvarHorario(): Promise<void> {
  const calendario = calendarioSelecionado.value
  if (!calendario) return

  const payload = {
    diaSemana: horarioForm.diaSemana,
    horaInicio: horarioForm.horaInicio,
    horaFim: horarioForm.horaFim,
    ativo: horarioForm.ativo,
  }

  if (horarioForm.id) await slaPoliciesService.atualizarHorarioCalendario(calendario.id, horarioForm.id, payload)
  else await slaPoliciesService.criarHorarioCalendario(calendario.id, payload)

  sucesso.value = 'Horario salvo.'
  limparHorario()
  await carregar()
}

async function excluirHorario(horario: HorarioAtendimentoCalendarioResponse): Promise<void> {
  const calendario = calendarioSelecionado.value
  if (!calendario) return
  await slaPoliciesService.excluirHorarioCalendario(calendario.id, horario.id)
  await carregar()
}

function limparExcecao(): void {
  excecaoForm.id = null
  excecaoForm.data = ''
  excecaoForm.tipo = 1
  excecaoForm.descricao = ''
  excecaoForm.horaInicio = ''
  excecaoForm.horaFim = ''
  excecaoForm.ativo = true
}

function editarExcecao(excecao: ExcecaoCalendarioCorporativoResponse): void {
  excecaoForm.id = excecao.id
  excecaoForm.data = excecao.data
  excecaoForm.tipo = Number(excecao.tipo)
  excecaoForm.descricao = excecao.descricao ?? ''
  excecaoForm.horaInicio = excecao.horaInicio ?? ''
  excecaoForm.horaFim = excecao.horaFim ?? ''
  excecaoForm.ativo = excecao.ativo
}

async function salvarExcecao(): Promise<void> {
  const calendario = calendarioSelecionado.value
  if (!calendario || !excecaoForm.data) return

  const payload = {
    data: excecaoForm.data,
    tipo: excecaoForm.tipo,
    descricao: excecaoForm.descricao.trim() || null,
    horaInicio: excecaoForm.tipo === 3 ? excecaoForm.horaInicio || null : null,
    horaFim: excecaoForm.tipo === 3 ? excecaoForm.horaFim || null : null,
    ativo: excecaoForm.ativo,
  }

  if (excecaoForm.id) await slaPoliciesService.atualizarExcecaoCalendario(calendario.id, excecaoForm.id, payload)
  else await slaPoliciesService.criarExcecaoCalendario(calendario.id, payload)

  sucesso.value = 'Excecao salva.'
  limparExcecao()
  await carregar()
}

async function excluirExcecao(excecao: ExcecaoCalendarioCorporativoResponse): Promise<void> {
  const calendario = calendarioSelecionado.value
  if (!calendario) return
  await slaPoliciesService.excluirExcecaoCalendario(calendario.id, excecao.id)
  await carregar()
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="SLA e capacidade operacional"
      title="Calendarios de SLA"
      subtitle="Gerencie jornadas, excecoes e calendario padrao usado no horario comercial."
    />

    <q-banner v-if="sucesso" class="bg-positive text-white q-mb-md" rounded>{{ sucesso }}</q-banner>
    <ErrorState v-if="erro" class="q-mb-md" :mensagem="erro" @retry="carregar" />
    <LoadingState v-if="loading" inline mensagem="Carregando calendarios de SLA..." />

    <template v-else>
      <div class="sgx-kpi-grid">
        <MetricCard title="Total de calendarios" :value="calendarios.length" icon="calendar_month" tone="primary" />
        <MetricCard title="Ativos" :value="totalCalendariosAtivos" icon="task_alt" tone="positive" />
        <MetricCard title="Inativos" :value="totalCalendariosInativos" icon="pause_circle" tone="warning" />
        <MetricCard title="Padrao" :value="totalCalendariosPadrao" icon="star" tone="info" />
        <MetricCard title="Horarios do selecionado" :value="totalHorariosSelecionados" icon="schedule" tone="primary" />
        <MetricCard title="Excecoes do selecionado" :value="totalExcecoesSelecionadas" icon="event_busy" tone="warning" />
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-7">
          <AppSectionCard titulo="Calendarios corporativos" subtitulo="Expediente usado por politicas de SLA em horario comercial.">
            <template #actions>
              <q-btn color="primary" icon="add" label="Novo" @click="novoCalendario" />
            </template>

            <EmptyState
              v-if="!calendarios.length"
              titulo="Nenhum calendario cadastrado"
              mensagem="Crie um calendario para habilitar calculo em horario comercial."
              icon="calendar_month"
            />

            <q-table v-else :rows="calendarios" :columns="colunasCalendario" row-key="id" flat bordered>
              <template #body-cell-padrao="props">
                <q-td class="text-center">
                  <q-badge :color="props.row.padrao ? 'primary' : 'grey-6'">{{ props.row.padrao ? 'Sim' : 'Nao' }}</q-badge>
                </q-td>
              </template>
              <template #body-cell-ativo="props">
                <q-td class="text-center">
                  <q-badge :color="props.row.ativo ? 'positive' : 'negative'">{{ props.row.ativo ? 'Sim' : 'Nao' }}</q-badge>
                </q-td>
              </template>
              <template #body-cell-acoes="props">
                <q-td class="text-right">
                  <q-btn flat dense round icon="edit" color="secondary" aria-label="Editar calendário" @click="editarCalendario(props.row)" />
                  <q-btn flat dense round icon="star" color="primary" aria-label="Definir calendário padrão" :disable="props.row.padrao" @click="definirPadrao(props.row)" />
                  <q-btn flat dense round :icon="props.row.ativo ? 'toggle_on' : 'toggle_off'" :aria-label="props.row.ativo ? 'Inativar calendário' : 'Ativar calendário'" @click="alternarStatus(props.row)" />
                </q-td>
              </template>
            </q-table>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-5">
          <AppSectionCard titulo="Dados do calendario" subtitulo="Nome, fuso horario e marcacao de padrao.">
            <div class="row q-col-gutter-sm">
              <div class="col-12"><q-input v-model="calendarioForm.nome" outlined dense label="Nome" /></div>
              <div class="col-12"><q-input v-model="calendarioForm.descricao" outlined dense type="textarea" autogrow label="Descricao" /></div>
              <div class="col-12"><q-input v-model="calendarioForm.timeZone" outlined dense label="Time zone" /></div>
              <div class="col-6"><q-toggle v-model="calendarioForm.ativo" label="Ativo" /></div>
              <div class="col-6"><q-toggle v-model="calendarioForm.padrao" label="Padrao" /></div>
              <div class="col-12 row justify-end">
                <q-btn color="primary" icon="save" label="Salvar" :loading="salvando" @click="salvarCalendario" />
              </div>
            </div>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Horarios de atendimento" subtitulo="Janelas semanais de expediente.">
            <div class="row q-col-gutter-sm q-mb-md">
              <div class="col-12 col-sm-4"><q-select v-model="horarioForm.diaSemana" outlined dense emit-value map-options :options="diasSemana" label="Dia" /></div>
              <div class="col-6 col-sm-2"><q-input v-model="horarioForm.horaInicio" outlined dense type="time" label="Inicio" /></div>
              <div class="col-6 col-sm-2"><q-input v-model="horarioForm.horaFim" outlined dense type="time" label="Fim" /></div>
              <div class="col-6 col-sm-2"><q-toggle v-model="horarioForm.ativo" label="Ativo" /></div>
              <div class="col-6 col-sm-2"><q-btn color="primary" icon="save" label="Salvar" class="full-width" @click="salvarHorario" /></div>
            </div>

            <EmptyState
              v-if="!calendarioSelecionado?.horariosAtendimento?.length"
              titulo="Sem horarios definidos"
              mensagem="Cadastre ao menos uma janela para o calendario selecionado."
              icon="schedule"
            />

            <q-table
              v-else
              :rows="calendarioSelecionado?.horariosAtendimento ?? []"
              :columns="colunasHorario"
              row-key="id"
              flat
              bordered
              dense
            >
              <template #body-cell-ativo="props"><q-td class="text-center">{{ props.row.ativo ? 'Sim' : 'Nao' }}</q-td></template>
              <template #body-cell-acoes="props">
                <q-td class="text-right">
                  <q-btn flat dense round icon="edit" aria-label="Editar horário" @click="editarHorario(props.row)" />
                  <q-btn flat dense round icon="delete" color="negative" aria-label="Excluir horário" @click="excluirHorario(props.row)" />
                </q-td>
              </template>
            </q-table>
          </AppSectionCard>
        </div>

        <div class="col-12 col-lg-6">
          <AppSectionCard titulo="Excecoes" subtitulo="Feriados, recessos, expediente especial e dias sem expediente.">
            <div class="row q-col-gutter-sm q-mb-md">
              <div class="col-12 col-sm-3"><q-input v-model="excecaoForm.data" outlined dense type="date" label="Data" /></div>
              <div class="col-12 col-sm-4"><q-select v-model="excecaoForm.tipo" outlined dense emit-value map-options :options="tiposExcecao" label="Tipo" /></div>
              <div class="col-6 col-sm-2"><q-input v-model="excecaoForm.horaInicio" outlined dense type="time" label="Inicio" :disable="excecaoForm.tipo !== 3" /></div>
              <div class="col-6 col-sm-2"><q-input v-model="excecaoForm.horaFim" outlined dense type="time" label="Fim" :disable="excecaoForm.tipo !== 3" /></div>
              <div class="col-12"><q-input v-model="excecaoForm.descricao" outlined dense label="Descricao" /></div>
              <div class="col-6"><q-toggle v-model="excecaoForm.ativo" label="Ativa" /></div>
              <div class="col-6 row justify-end"><q-btn color="primary" icon="save" label="Salvar" @click="salvarExcecao" /></div>
            </div>

            <EmptyState
              v-if="!calendarioSelecionado?.excecoes?.length"
              titulo="Sem excecoes cadastradas"
              mensagem="Use excecoes para feriados, recessos e expedientes especiais."
              icon="event_busy"
            />

            <q-table
              v-else
              :rows="calendarioSelecionado?.excecoes ?? []"
              :columns="colunasExcecao"
              row-key="id"
              flat
              bordered
              dense
            >
              <template #body-cell-periodo="props">
                <q-td class="text-center">{{ props.row.horaInicio && props.row.horaFim ? `${props.row.horaInicio} - ${props.row.horaFim}` : '-' }}</q-td>
              </template>
              <template #body-cell-ativo="props"><q-td class="text-center">{{ props.row.ativo ? 'Sim' : 'Nao' }}</q-td></template>
              <template #body-cell-acoes="props">
                <q-td class="text-right">
                  <q-btn flat dense round icon="edit" aria-label="Editar exceção" @click="editarExcecao(props.row)" />
                  <q-btn flat dense round icon="delete" color="negative" aria-label="Excluir exceção" @click="excluirExcecao(props.row)" />
                </q-td>
              </template>
            </q-table>
          </AppSectionCard>
        </div>
      </div>
    </template>
  </q-page>
</template>
