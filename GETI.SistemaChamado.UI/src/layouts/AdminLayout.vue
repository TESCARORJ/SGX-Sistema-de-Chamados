<template>
  <q-layout view="lHh Lpr lFf" class="layout-admin">
    <q-header elevated class="bg-secondary text-white">
      <q-toolbar>
        <q-toolbar-title>Ambiente Administrativo</q-toolbar-title>
        <div v-if="usuario" class="q-mr-md text-caption">
          {{ usuario.nome }} ({{ usuario.perfilAcesso }})
        </div>
        <q-btn flat dense icon="home" label="Portal" to="/" />
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
