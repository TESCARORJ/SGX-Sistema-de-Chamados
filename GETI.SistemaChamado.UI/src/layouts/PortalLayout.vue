<template>
  <q-layout view="lHh Lpr lFf" class="layout-portal">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>Portal do Solicitante</q-toolbar-title>
        <div v-if="usuario" class="q-mr-md text-caption">
          {{ usuario.nome }} ({{ usuario.perfilAcesso }})
        </div>
        <q-btn flat dense icon="admin_panel_settings" label="Admin" to="/admin" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { obterUsuarioAtual, type UsuarioAtual } from '@/services/sessaoUsuario';

const usuario = ref<UsuarioAtual | null>(null);

onMounted(async () => {
  try {
    usuario.value = await obterUsuarioAtual();
  } catch {
    usuario.value = null;
  }
});
</script>
