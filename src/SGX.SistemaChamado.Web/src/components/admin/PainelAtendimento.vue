<script setup lang="ts">
import type { ChamadoAdminDetalhe } from '../../types/admin'

defineProps<{
  chamado: ChamadoAdminDetalhe
  loading?: boolean
  canAssumir?: boolean
  canAssumirFila?: boolean
  canTransferirGrupo?: boolean
  canAtribuir?: boolean
  canAlterarStatus?: boolean
  canAlterarPrioridade?: boolean
  canAlterarCategoria?: boolean
  canComentar?: boolean
  canEncerrar?: boolean
  canReabrir?: boolean
}>()

const emit = defineEmits<{
  (e: 'assumir'): void
  (e: 'assumir-fila'): void
  (e: 'transferir-grupo'): void
  (e: 'atribuir'): void
  (e: 'alterar-status'): void
  (e: 'alterar-prioridade'): void
  (e: 'alterar-categoria'): void
  (e: 'comentar'): void
  (e: 'encerrar'): void
  (e: 'reabrir'): void
}>()
</script>

<template>
  <div class="column q-gutter-md">
    <div class="text-subtitle1 text-weight-medium">Painel de acoes</div>

    <div class="painel-acoes">
      <q-btn color="secondary" icon="assignment_ind" label="Assumir" :loading="loading" :disable="canAssumir === false" @click="emit('assumir')" />

      <q-btn
        v-if="canAssumirFila"
        color="primary"
        icon="playlist_add_check"
        label="Assumir da fila"
        :loading="loading"
        @click="emit('assumir-fila')"
      />

      <q-btn
        v-if="canTransferirGrupo"
        outline
        color="primary"
        icon="move_up"
        label="Transferir grupo"
        :loading="loading"
        @click="emit('transferir-grupo')"
      />

      <q-btn
        v-if="canAtribuir !== false"
        outline
        color="primary"
        icon="group_add"
        label="Atribuir"
        :loading="loading"
        @click="emit('atribuir')"
      />

      <q-btn
        v-if="canAlterarStatus !== false"
        outline
        color="primary"
        icon="swap_horiz"
        label="Alterar status"
        :loading="loading"
        @click="emit('alterar-status')"
      />
      <q-btn
        v-if="canAlterarPrioridade !== false"
        outline
        color="primary"
        icon="priority_high"
        label="Alterar prioridade"
        :loading="loading"
        @click="emit('alterar-prioridade')"
      />
      <q-btn
        v-if="canAlterarCategoria !== false"
        outline
        color="primary"
        icon="category"
        label="Alterar categoria"
        :loading="loading"
        @click="emit('alterar-categoria')"
      />
      <q-btn v-if="canComentar !== false" outline color="primary" icon="comment" label="Comentar" :loading="loading" @click="emit('comentar')" />

      <q-btn v-if="canEncerrar !== false" color="negative" icon="check_circle" label="Encerrar" :loading="loading" @click="emit('encerrar')" />
      <q-btn
        v-if="canReabrir !== false"
        color="warning"
        text-color="black"
        icon="autorenew"
        label="Reabrir"
        :loading="loading"
        @click="emit('reabrir')"
      />
    </div>
  </div>
</template>

<style scoped>
.painel-acoes {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

@media (max-width: 768px) {
  .painel-acoes {
    display: grid;
    grid-template-columns: 1fr;
  }

  .painel-acoes :deep(.q-btn) {
    width: 100%;
  }
}
</style>
