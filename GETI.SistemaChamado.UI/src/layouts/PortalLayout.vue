<template>
  <q-layout view="lHh Lpr lFf" class="layout-portal">
    <q-header elevated class="bg-primary text-white">
      <q-toolbar>
        <q-toolbar-title>Portal do Solicitante</q-toolbar-title>
        <q-btn flat dense icon="admin_panel_settings" label="Admin" to="/admin" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
