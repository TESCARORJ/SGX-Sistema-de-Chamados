<template>
  <q-layout view="lHh Lpr lFf" class="layout-admin">
    <q-header elevated class="bg-secondary text-white">
      <q-toolbar>
        <q-toolbar-title>Ambiente Administrativo</q-toolbar-title>
        <q-btn flat dense icon="home" label="Portal" to="/" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
