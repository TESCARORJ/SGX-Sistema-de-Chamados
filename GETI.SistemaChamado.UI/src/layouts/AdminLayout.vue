<template>
  <q-layout view="lHh Lpr lFf" class="layout-admin">
    <q-header elevated class="bg-secondary text-white">
      <q-toolbar class="q-px-md q-py-sm">
        <q-toolbar-title>Ambiente Administrativo</q-toolbar-title>
        <div v-if="usuario" class="q-mr-md text-caption">
          {{ usuario.nome }} ({{ usuario.perfilAcesso }})
        </div>
        <q-btn flat dense icon="home" label="Portal" to="/" />
      </q-toolbar>
      <q-toolbar class="q-gutter-sm q-px-md q-pb-sm">
        <q-btn
          v-for="item in menuAdmin"
          :key="item.to"
          flat
          dense
          no-caps
          :label="item.label"
          :to="item.to"
          :class="{ 'bg-white text-secondary': rotaAtual.startsWith(item.to) }"
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

const menuAdmin = [
  { label: 'Dashboard', to: '/admin' },
  { label: 'Departamentos', to: '/admin/departamentos' },
  { label: 'Caixas de E-mail', to: '/admin/caixas-email' },
  { label: 'Categorias', to: '/admin/categorias' },
  { label: 'Servicos', to: '/admin/servicos' },
  { label: 'Grupos de Atendimento', to: '/admin/grupos-atendimento' }
];

onMounted(async () => {
  try {
    usuario.value = await obterUsuarioAtual();
  } catch {
    usuario.value = null;
  }
});
</script>
