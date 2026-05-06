<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()
</script>

<template>
  <main class="denied-page">
    <section class="denied-card">
      <h1>Acesso negado</h1>
      <p>Seu usuário está autenticado, mas não possui perfil interno para esta área.</p>
      <q-btn color="primary" label="Voltar para login" @click="router.push('/login')" />
    </section>
  </main>
</template>

<style scoped>
.denied-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 1.5rem;
  background: linear-gradient(140deg, #fef2f2 0%, #eff6ff 100%);
}

.denied-card {
  width: min(540px, 100%);
  background: #ffffff;
  border-radius: 16px;
  border: 1px solid #fecaca;
  box-shadow: 0 18px 44px rgba(127, 29, 29, 0.1);
  padding: 1.4rem;
  display: grid;
  gap: 0.9rem;
}

h1 {
  margin: 0;
}

p {
  margin: 0;
  color: #475569;
}
</style>
