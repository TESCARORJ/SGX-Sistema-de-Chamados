<script setup lang="ts">
import { computed } from 'vue'
import type {
  PortalFormularioPreparacao,
  PortalFormularioPreparacaoCampo,
  PortalFormularioPreparacaoOpcao,
} from '../../types/catalogoServicos'
import { TipoCampoFormularioServico } from '../../types/formularioServicos'

type RespostaFormularioState = Record<string, boolean | string | string[] | null>

const props = defineProps<{
  formulario: PortalFormularioPreparacao | null
  modelValue: RespostaFormularioState
}>()

const emit = defineEmits<{
  'update:modelValue': [value: RespostaFormularioState]
}>()

const formularioVisivel = computed(() => props.formulario !== null)

const camposOrdenados = computed(() => {
  const campos = props.formulario?.versao.campos ?? []

  return [...campos]
    .filter((campo) => campo.ativo !== false && campo.visivel !== false)
    .sort((a, b) => a.ordem - b.ordem || a.rotulo.localeCompare(b.rotulo))
})

function obterOpcoesOrdenadas(campo: PortalFormularioPreparacaoCampo): PortalFormularioPreparacaoOpcao[] {
  return [...campo.opcoes]
    .filter((opcao) => opcao.ativo !== false)
    .sort((a, b) => a.ordem - b.ordem || a.rotulo.localeCompare(b.rotulo))
}

function obterLabelCampo(campo: PortalFormularioPreparacaoCampo): string {
  return campo.obrigatorio ? `${campo.rotulo} *` : campo.rotulo
}

function tipoCampoDescricao(tipo: number): string {
  switch (tipo) {
    case TipoCampoFormularioServico.TextoCurto:
      return 'Texto curto'
    case TipoCampoFormularioServico.TextoLongo:
      return 'Texto longo'
    case TipoCampoFormularioServico.Numero:
      return 'Numero'
    case TipoCampoFormularioServico.Data:
      return 'Data'
    case TipoCampoFormularioServico.Booleano:
      return 'Booleano'
    case TipoCampoFormularioServico.SelecaoUnica:
      return 'Selecao unica'
    case TipoCampoFormularioServico.SelecaoMultipla:
      return 'Selecao multipla'
    default:
      return 'Campo'
  }
}

function obterValorInicial(campo: PortalFormularioPreparacaoCampo): boolean | string | string[] | null {
  if (campo.tipo === TipoCampoFormularioServico.SelecaoMultipla) {
    return []
  }

  return null
}

function garantirEstadoCampo(campo: PortalFormularioPreparacaoCampo): void {
  if (campo.id in props.modelValue) {
    return
  }

  emit('update:modelValue', {
    ...props.modelValue,
    [campo.id]: obterValorInicial(campo),
  })
}

function obterOpcoesSelect(campo: PortalFormularioPreparacaoCampo) {
  return obterOpcoesOrdenadas(campo).map((opcao) => ({
    label: opcao.rotulo,
    value: opcao.valor,
  }))
}

function isTextoCurto(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.TextoCurto
}

function isTextoLongo(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.TextoLongo
}

function isNumero(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.Numero
}

function isData(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.Data
}

function isBooleano(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.Booleano
}

function isSelecaoUnica(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.SelecaoUnica
}

function isSelecaoMultipla(campo: PortalFormularioPreparacaoCampo): boolean {
  return campo.tipo === TipoCampoFormularioServico.SelecaoMultipla
}

function atualizarResposta(campoId: string, valor: boolean | string | string[] | null): void {
  emit('update:modelValue', {
    ...props.modelValue,
    [campoId]: valor,
  })
}
</script>

<template>
  <div v-if="formularioVisivel" class="sgx-formulario-dinamico column q-gutter-md">
    <q-banner rounded class="bg-teal-1 text-teal-10">
      <div class="text-subtitle2">{{ formulario?.nome }}</div>
      <div v-if="formulario?.descricao" class="text-body2 q-mt-xs">{{ formulario.descricao }}</div>
      <div class="text-caption q-mt-sm">
        Versao {{ formulario?.versao.numero }}{{ formulario?.versao.publicada ? ' publicada' : '' }}
      </div>
    </q-banner>

    <q-banner v-if="!camposOrdenados.length" rounded class="bg-grey-1 text-grey-8">
      Nenhum campo adicional foi configurado para este servico.
    </q-banner>

    <div v-else class="column q-gutter-md">
      <div
        v-for="campo in camposOrdenados"
        :key="campo.id"
        class="sgx-formulario-campo"
      >
        <div class="row items-center q-col-gutter-sm q-mb-xs">
          <div class="col">
            <div class="text-subtitle2 text-weight-medium">{{ obterLabelCampo(campo) }}</div>
          </div>
          <div class="col-auto">
            <q-badge color="teal-2" text-color="teal-10">
              {{ tipoCampoDescricao(campo.tipo) }}
            </q-badge>
          </div>
        </div>

        <div v-if="campo.textoAjuda" class="text-caption text-grey-7 q-mb-sm">
          {{ campo.textoAjuda }}
        </div>

        <template v-if="garantirEstadoCampo(campo) || true">
          <q-input
            v-if="isTextoCurto(campo)"
            :model-value="props.modelValue[campo.id]"
            outlined
            maxlength="180"
            :label="obterLabelCampo(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, String(valor ?? ''))"
          />

          <q-input
            v-else-if="isTextoLongo(campo)"
            :model-value="props.modelValue[campo.id]"
            outlined
            type="textarea"
            autogrow
            maxlength="4000"
            :label="obterLabelCampo(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, String(valor ?? ''))"
          />

          <q-input
            v-else-if="isNumero(campo)"
            :model-value="props.modelValue[campo.id]"
            outlined
            type="number"
            :label="obterLabelCampo(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, String(valor ?? ''))"
          />

          <q-input
            v-else-if="isData(campo)"
            :model-value="props.modelValue[campo.id]"
            outlined
            type="date"
            :label="obterLabelCampo(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, String(valor ?? ''))"
          />

          <q-toggle
            v-else-if="isBooleano(campo)"
            :model-value="props.modelValue[campo.id]"
            color="primary"
            :label="obterLabelCampo(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, Boolean(valor))"
          />

          <q-select
            v-else-if="isSelecaoUnica(campo)"
            :model-value="props.modelValue[campo.id]"
            outlined
            emit-value
            map-options
            :label="obterLabelCampo(campo)"
            :options="obterOpcoesSelect(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, valor === null ? null : String(valor))"
          />

          <q-option-group
            v-else-if="isSelecaoMultipla(campo)"
            :model-value="props.modelValue[campo.id]"
            color="primary"
            type="checkbox"
            :options="obterOpcoesSelect(campo)"
            @update:model-value="(valor) => atualizarResposta(campo.id, Array.isArray(valor) ? valor.map((item) => String(item)) : [])"
          />

          <q-banner v-else rounded class="bg-amber-2 text-dark">
            Tipo de campo ainda nao suportado visualmente.
          </q-banner>
        </template>
      </div>
    </div>
  </div>
</template>

<style scoped>
.sgx-formulario-dinamico {
  padding-top: 4px;
}

.sgx-formulario-campo {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
</style>
