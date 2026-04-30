<template>
  <q-layout view="hHh lpR fFf" class="layout-publico">
    <q-header class="bg-dark text-white">
      <q-toolbar class="q-px-md">
        <q-toolbar-title>Sistema de Chamados CREA-RJ</q-toolbar-title>
        <q-btn flat no-caps label="Login" to="/login" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
