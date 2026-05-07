<script setup lang="ts">
import { useQuasar } from 'quasar'
import type { QTableColumn } from 'quasar'

withDefaults(defineProps<{
  title: string
  rows: unknown[]
  columns: QTableColumn[]
  loading?: boolean
  rowKey?: string
  gridOnMobile?: boolean
}>(), {
  gridOnMobile: true,
})

const $q = useQuasar()
</script>

<template>
  <q-table
    flat
    bordered
    :title="title"
    :rows="rows"
    :columns="columns"
    :row-key="rowKey || 'id'"
    :loading="loading"
    :grid="Boolean(gridOnMobile && $q.screen.lt.md)"
    wrap-cells
    :rows-per-page-options="[0]"
    hide-bottom
  >
    <template #body-cell-ativo="slotProps">
      <q-td :props="slotProps">
        <q-badge :color="slotProps.value ? 'positive' : 'grey-6'" text-color="white">
          {{ slotProps.value ? 'Ativo' : 'Inativo' }}
        </q-badge>
      </q-td>
    </template>

    <template #body-cell-sensivel="slotProps">
      <q-td :props="slotProps">
        <q-badge :color="slotProps.row.sensivel ? 'warning' : 'grey-6'" text-color="white">
          {{ slotProps.row.sensivel ? 'Sensivel' : 'Nao sensivel' }}
        </q-badge>
      </q-td>
    </template>

    <template #body-cell-acoes="slotProps">
      <q-td :props="slotProps" class="q-gutter-xs">
        <slot name="acoes" :row="slotProps.row" />
      </q-td>
    </template>

    <template #no-data>
      <div class="full-width text-center q-pa-lg text-grey-7">
        Nenhum registro encontrado.
      </div>
    </template>
  </q-table>
</template>
