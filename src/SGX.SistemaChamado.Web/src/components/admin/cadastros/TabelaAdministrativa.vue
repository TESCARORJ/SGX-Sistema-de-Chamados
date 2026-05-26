<script setup lang="ts">
import { useQuasar } from 'quasar'
import type { QTableColumn } from 'quasar'
import StatusBadge from '../../ui/StatusBadge.vue'

withDefaults(
  defineProps<{
    title: string
    rows: unknown[]
    columns: QTableColumn[]
    loading?: boolean
    rowKey?: string
    gridOnMobile?: boolean
  }>(),
  {
    gridOnMobile: true,
  }
)

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
    <template #top>
      <div class="row items-center justify-between full-width q-gutter-sm">
        <div class="text-subtitle2 text-weight-bold">{{ title }}</div>
        <q-chip dense color="blue-grey-1" text-color="blue-grey-9" icon="table_rows" :label="`${rows.length} itens`" />
      </div>
    </template>

    <template #body-cell-ativo="slotProps">
      <q-td :props="slotProps">
        <StatusBadge :texto="slotProps.value ? 'Ativo' : 'Inativo'" />
      </q-td>
    </template>

    <template #body-cell-situacao="slotProps">
      <q-td :props="slotProps">
        <StatusBadge :texto="String(slotProps.value || '-')" />
      </q-td>
    </template>

    <template #body-cell-cor="slotProps">
      <q-td :props="slotProps">
        <div class="row items-center q-gutter-sm">
          <q-badge rounded :style="{ backgroundColor: slotProps.value || '#cbd5e1' }">&nbsp;</q-badge>
          <span>{{ slotProps.value || '-' }}</span>
        </div>
      </q-td>
    </template>

    <template #body-cell-sensivel="slotProps">
      <q-td :props="slotProps">
        <StatusBadge :texto="slotProps.row.sensivel ? 'Sensivel' : 'Nao sensivel'" />
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

    <template #item="slotProps">
      <div class="col-12">
        <q-card flat bordered class="sgx-card tabela-administrativa__item-card q-pa-sm">
          <q-list dense separator>
            <q-item v-for="col in slotProps.cols.filter((item) => item.name !== 'acoes')" :key="col.name">
              <q-item-section>
                <q-item-label class="text-caption text-grey-7">{{ col.label }}</q-item-label>
                <q-item-label v-if="col.name === 'ativo'">
                  <StatusBadge :texto="col.value ? 'Ativo' : 'Inativo'" />
                </q-item-label>
                <q-item-label v-else-if="col.name === 'situacao'">
                  <StatusBadge :texto="String(col.value || '-')" />
                </q-item-label>
                <q-item-label v-else class="text-body2">
                  {{ col.value || '-' }}
                </q-item-label>
              </q-item-section>
            </q-item>
          </q-list>

          <div class="row justify-end q-gutter-xs q-mt-sm">
            <slot name="acoes" :row="slotProps.row" />
          </div>
        </q-card>
      </div>
    </template>
  </q-table>
</template>

<style scoped>
.tabela-administrativa__item-card {
  border-radius: var(--sgx-radius-md);
}
</style>

