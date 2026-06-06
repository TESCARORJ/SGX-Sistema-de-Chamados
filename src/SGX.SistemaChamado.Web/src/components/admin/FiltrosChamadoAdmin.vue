<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import FilterBar from '../ui/FilterBar.vue'
import { adminService } from '../../services/adminService'
import type { AdminContextoResponse, FilaAtendimentoGrupoTecnicoResponse, FiltroChamadosAdmin, GrupoTecnicoResumo } from '../../types/admin'
import { NaturezaChamado } from '../../types/portal'

const props = defineProps<{
  contexto: AdminContextoResponse | null
  textoInicial?: string
  loading?: boolean
}>()

const emit = defineEmits<{
  (e: 'filtrar', filtros: FiltroChamadosAdmin): void
  (e: 'limpar', filtros: FiltroChamadosAdmin): void
}>()

const filtros = reactive<FiltroChamadosAdmin>({
  pagina: 1,
  tamanhoPagina: 20,
  ordenarPor: 'atualizadoEm',
  direcaoOrdenacao: 'desc',
})
const gruposTecnicos = ref<GrupoTecnicoResumo[]>([])
const filasAtendimento = ref<FilaAtendimentoGrupoTecnicoResponse[]>([])
const loadingGruposTecnicos = ref(false)
const loadingFilasAtendimento = ref(false)

const opcoesNatureza = [
  { label: 'Todos', value: undefined },
  { label: 'Incidente', value: NaturezaChamado.Incidente },
  { label: 'Requisicao', value: NaturezaChamado.Requisicao },
  { label: 'Mudanca', value: NaturezaChamado.Mudanca },
  { label: 'Problema', value: NaturezaChamado.Problema },
  { label: 'Evento/Alerta', value: NaturezaChamado.EventoAlerta },
  { label: 'Tarefa operacional', value: NaturezaChamado.TarefaOperacional },
]
const opcoesGruposTecnicos = computed(() =>
  gruposTecnicos.value.map((grupo) => ({ label: grupo.nome, value: grupo.id }))
)
const opcoesFilasAtendimento = computed(() =>
  filasAtendimento.value.map((fila) => ({ label: fila.nome, value: fila.id }))
)

watch(
  () => props.textoInicial,
  (valor) => {
    const texto = (valor ?? '').trim()
    filtros.texto = texto ? texto : undefined
  },
  { immediate: true }
)

watch(
  () => props.contexto,
  async (contexto) => {
    if (!contexto) return
    await carregarGruposTecnicos()
  },
  { immediate: true }
)

async function carregarGruposTecnicos(): Promise<void> {
  loadingGruposTecnicos.value = true

  try {
    const response = await adminService.listarGruposTecnicos({
      ativo: true,
      pagina: 1,
      tamanhoPagina: 100,
      ordenarPor: 'nome',
      direcaoOrdenacao: 'asc',
    })
    gruposTecnicos.value = response.items
  } finally {
    loadingGruposTecnicos.value = false
  }
}

async function carregarFilasAtendimento(grupoTecnicoId: string): Promise<void> {
  filasAtendimento.value = []

  if (!grupoTecnicoId) {
    return
  }

  loadingFilasAtendimento.value = true

  try {
    filasAtendimento.value = await adminService.listarFilasAtendimentoGrupoTecnico(grupoTecnicoId, { ativo: true })
  } finally {
    loadingFilasAtendimento.value = false
  }
}

function opcoesSubcategorias() {
  const categoriaSelecionada = filtros.categoriaId
  const subcategorias = props.contexto?.subcategorias ?? []

  return subcategorias
    .filter((subcategoria) => !categoriaSelecionada || subcategoria.categoriaChamadoId === categoriaSelecionada)
    .map((subcategoria) => ({ label: subcategoria.nome, value: subcategoria.id }))
}

function onCategoriaChange(): void {
  const categoriaSelecionada = filtros.categoriaId
  if (!categoriaSelecionada) {
    filtros.subcategoriaId = undefined
    return
  }

  const pertenceACategoria = (props.contexto?.subcategorias ?? []).some(
    (subcategoria) =>
      subcategoria.id === filtros.subcategoriaId && subcategoria.categoriaChamadoId === categoriaSelecionada
  )

  if (!pertenceACategoria) {
    filtros.subcategoriaId = undefined
  }
}

async function onGrupoTecnicoChange(value: string | null): Promise<void> {
  filtros.filaAtendimentoId = undefined
  await carregarFilasAtendimento(value ?? '')
}

function aplicar(): void {
  filtros.pagina = 1
  emit('filtrar', { ...filtros })
}

function limpar(): void {
  filtros.statusId = undefined
  filtros.naturezaChamado = undefined
  filtros.prioridadeId = undefined
  filtros.categoriaId = undefined
  filtros.subcategoriaId = undefined
  filtros.tipoSolicitacaoId = undefined
  filtros.localUnidadeId = undefined
  filtros.departamentoId = undefined
  filtros.responsavelId = undefined
  filtros.grupoTecnicoId = undefined
  filtros.filaAtendimentoId = undefined
  filtros.solicitanteId = undefined
  filtros.dataInicio = undefined
  filtros.dataFim = undefined
  filtros.slaVencido = undefined
  filtros.slaSituacao = undefined
  filtros.texto = undefined
  filtros.pagina = 1
  filtros.tamanhoPagina = 20
  filtros.ordenarPor = 'atualizadoEm'
  filtros.direcaoOrdenacao = 'desc'
  filasAtendimento.value = []

  emit('limpar', { ...filtros })
}
</script>

<template>
  <FilterBar titulo="Filtro operacional" subtitulo="Combine triagem, classificacao, SLA e ordenacao da fila.">
    <q-form class="row q-col-gutter-sm" @submit.prevent="aplicar">
      <div class="col-12">
        <div class="text-caption text-weight-semibold sgx-muted">Dados principais</div>
      </div>

      <div class="col-12 col-md-3">
        <q-input v-model="filtros.texto" outlined label="Texto" placeholder="Código, título ou descrição" />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.naturezaChamado"
          :options="opcoesNatureza"
          emit-value
          map-options
          clearable
          outlined
          label="Natureza ITSM"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.statusId"
          :options="props.contexto?.status.map((s) => ({ label: s.nome, value: s.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Status"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.prioridadeId"
          :options="props.contexto?.prioridades.map((p) => ({ label: p.nome, value: p.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Prioridade"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.responsavelId"
          :options="props.contexto?.atendentes.map((a) => ({ label: a.nome, value: a.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Responsável"
        />
      </div>

      <div class="col-12">
        <q-separator class="q-my-sm" />
        <div class="text-caption text-weight-semibold sgx-muted">Atendimento</div>
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.grupoTecnicoId"
          :options="opcoesGruposTecnicos"
          :loading="loadingGruposTecnicos"
          emit-value
          map-options
          clearable
          outlined
          label="Grupo tecnico"
          @update:model-value="onGrupoTecnicoChange"
        >
          <template #no-option>
            <q-item>
              <q-item-section class="text-grey-7">Nenhum grupo tecnico ativo disponivel.</q-item-section>
            </q-item>
          </template>
        </q-select>
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.filaAtendimentoId"
          :options="opcoesFilasAtendimento"
          :loading="loadingFilasAtendimento"
          :disable="!filtros.grupoTecnicoId || loadingFilasAtendimento"
          emit-value
          map-options
          clearable
          outlined
          label="Fila de atendimento"
        >
          <template #no-option>
            <q-item>
              <q-item-section class="text-grey-7">Nenhuma fila ativa disponivel para este grupo.</q-item-section>
            </q-item>
          </template>
        </q-select>
      </div>

      <div class="col-12">
        <q-separator class="q-my-sm" />
        <div class="text-caption text-weight-semibold sgx-muted">Classificação</div>
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.categoriaId"
          :options="props.contexto?.categorias.map((c) => ({ label: c.nome, value: c.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Categoria"
          @update:model-value="onCategoriaChange"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.subcategoriaId"
          :options="opcoesSubcategorias()"
          emit-value
          map-options
          clearable
          outlined
          label="Subcategoria"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.tipoSolicitacaoId"
          :options="props.contexto?.tiposSolicitacao.map((t) => ({ label: t.nome, value: t.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Tipo de solicitação"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.localUnidadeId"
          :options="props.contexto?.locaisUnidade.map((l) => ({ label: l.nome, value: l.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Local / Unidade"
        />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.departamentoId"
          :options="props.contexto?.departamentos.map((d) => ({ label: d.nome, value: d.id })) ?? []"
          emit-value
          map-options
          clearable
          outlined
          label="Departamento"
        />
      </div>

      <div class="col-12 col-md-2">
        <q-input v-model="filtros.solicitanteId" outlined label="Solicitante" placeholder="ID do solicitante" />
      </div>

      <div class="col-12">
        <q-separator class="q-my-sm" />
        <div class="text-caption text-weight-semibold sgx-muted">SLA, período e ordenação</div>
      </div>

      <div class="col-12 col-md-2">
        <q-input v-model="filtros.dataInicio" type="date" outlined label="Período inicial" />
      </div>

      <div class="col-12 col-md-2">
        <q-input v-model="filtros.dataFim" type="date" outlined label="Período final" />
      </div>

      <div class="col-12 col-md-3">
        <q-select
          v-model="filtros.slaSituacao"
          :options="[
            { label: 'Todos', value: undefined },
            { label: 'Não aplicável', value: 'NaoAplicavel' },
            { label: 'Dentro do prazo', value: 'DentroDoPrazo' },
            { label: 'Próximo do vencimento', value: 'ProximoDoVencimento' },
            { label: 'Vencido', value: 'Vencido' },
            { label: 'Cumprido', value: 'Cumprido' },
            { label: 'Violado', value: 'Violado' },
            { label: 'Pausado', value: 'Pausado' },
          ]"
          emit-value
          map-options
          outlined
          label="Situação SLA"
        />
      </div>

      <div class="col-6 col-md-2">
        <q-select
          v-model="filtros.ordenarPor"
          :options="[
            { label: 'Atualização', value: 'atualizadoEm' },
            { label: 'Abertura', value: 'abertoEm' },
            { label: 'Código', value: 'codigo' },
            { label: 'Título', value: 'titulo' },
          ]"
          emit-value
          map-options
          outlined
          label="Ordenar por"
        />
      </div>

      <div class="col-6 col-md-2">
        <q-select
          v-model="filtros.direcaoOrdenacao"
          :options="[
            { label: 'Desc', value: 'desc' },
            { label: 'Asc', value: 'asc' },
          ]"
          emit-value
          map-options
          outlined
          label="Direção"
        />
      </div>
    </q-form>

    <template #actions>
      <q-btn flat label="Limpar" :disable="props.loading" @click="limpar" />
      <q-btn color="primary" label="Filtrar" icon="search" :loading="props.loading" @click="aplicar" />
    </template>
  </FilterBar>
</template>
