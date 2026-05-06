<template>
  <main class="home-page">
    <section class="hero">
      <h1>SGX Sistema de Chamados</h1>
      <p>Base frontend pronta para integrar com a API .NET do SGX.SistemaChamado.</p>
      <q-badge color="positive" text-color="white" label="Sprint 0 - Fundação técnica" />
    </section>
  </main>
</template>

<style scoped>
.home-page {
  min-height: 100vh;
  display: grid;
  place-items: center;
  padding: 2rem;
  background: linear-gradient(140deg, #fff7ed 0%, #ecfeff 100%);
}

.hero {
  width: min(800px, 100%);
  background: #ffffff;
  border-radius: 1rem;
  box-shadow: 0 20px 50px rgba(15, 23, 42, 0.08);
  padding: 2rem;
  border: 1px solid #e2e8f0;
}

h1 {
  margin: 0;
  color: #0f172a;
  font-size: clamp(1.8rem, 4vw, 2.6rem);
}

p {
  margin: 0.9rem 0 1.5rem;
  color: #334155;
  line-height: 1.6;
}
</style>
