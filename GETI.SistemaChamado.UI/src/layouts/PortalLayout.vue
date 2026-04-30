<template>
  <q-layout view="lHh Lpr lFf" class="layout-portal">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar class="q-px-md q-py-sm">
        <q-toolbar-title>Sistema de Chamados CREA-RJ</q-toolbar-title>
        <div v-if="usuario" class="q-mr-md text-caption">
          {{ usuario.nome }} ({{ usuario.perfilAcesso }})
        </div>
        <q-btn flat dense icon="admin_panel_settings" label="Admin" to="/admin" />
      </q-toolbar>
      <q-toolbar class="q-gutter-sm q-px-md q-pb-sm">
        <q-btn
          v-for="item in menuPortal"
          :key="item.to"
          flat
          dense
          no-caps
          :label="item.label"
          :to="item.to"
          :class="{ 'bg-white text-primary': itemAtivo(item.to) }"
        />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';
import { obterUsuarioAtual, type UsuarioAtual } from '@/services/sessaoUsuario';

const usuario = ref<UsuarioAtual | null>(null);
const route = useRoute();

const rotaAtual = computed(() => route.path);

const menuPortal = [
  { label: 'Inicio', to: '/portal' },
  { label: 'Meus Chamados', to: '/portal/chamados' },
  { label: 'Abrir Chamado', to: '/portal/chamados/novo' }
];

function itemAtivo(caminho: string): boolean {
  if (caminho === '/portal') {
    return rotaAtual.value === '/portal';
  }
  return rotaAtual.value.startsWith(caminho);
}

onMounted(async () => {
  try {
    usuario.value = await obterUsuarioAtual();
  } catch {
    usuario.value = null;
  }
});
</script>
