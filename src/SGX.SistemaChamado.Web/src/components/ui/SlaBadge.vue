<script setup lang="ts">
import type { PropType } from 'vue'
import { computed } from 'vue'
import {
  normalizarSituacaoSla,
  obterEstiloSla,
  type SituacaoSla,
  type SituacaoSlaEntrada,
} from './slaBadge'

const props = defineProps({
  vencido: {
    type: Boolean,
    default: false,
  },
  proximo: {
    type: Boolean,
    default: false,
  },
  pausado: {
    type: Boolean,
    default: false,
  },
  situacao: {
    type: [String, Number] as PropType<SituacaoSlaEntrada>,
    default: null,
  },
})

const situacaoNormalizada = computed<SituacaoSla | null>(() => normalizarSituacaoSla(props.situacao))

const estilo = computed(() =>
  obterEstiloSla(situacaoNormalizada.value, {
    vencido: props.vencido,
    proximo: props.proximo,
    pausado: props.pausado,
  })
)
</script>

<template>
  <q-chip
    dense
    rounded
    size="12px"
    :color="estilo.color"
    :text-color="estilo.textColor"
    :icon="estilo.icon"
    :label="estilo.label"
  />
</template>
