<script setup lang="ts">
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="auth-layout">
    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.auth-layout {
  background:
    radial-gradient(circle at 10% 0%, rgba(15, 118, 110, 0.12), transparent 45%),
    radial-gradient(circle at 100% 80%, rgba(249, 115, 22, 0.12), transparent 35%),
    linear-gradient(135deg, #f4f7fb 0%, #eef3f8 100%);
}
</style>
