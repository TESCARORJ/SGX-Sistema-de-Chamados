<script setup lang="ts">
</script>

<template>
  <q-layout view="lHh Lpr lFf" class="auth-layout">
    <q-page-container class="auth-layout__container">
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<style scoped>
.auth-layout {
  background:
    radial-gradient(circle at 4% 0%, rgba(11, 94, 215, 0.18), transparent 34%),
    radial-gradient(circle at 100% 100%, rgba(2, 132, 199, 0.14), transparent 36%),
    linear-gradient(145deg, var(--sgx-page-bg) 0%, var(--sgx-page-bg-alt) 100%);
  min-height: 100vh;
}

.auth-layout__container {
  position: relative;
}

.auth-layout__container::before,
.auth-layout__container::after {
  content: '';
  position: fixed;
  pointer-events: none;
  z-index: 0;
}

.auth-layout__container::before {
  top: -120px;
  right: -80px;
  width: 360px;
  height: 360px;
  background: radial-gradient(circle, rgba(11, 94, 215, 0.14) 0%, rgba(11, 94, 215, 0) 70%);
}

.auth-layout__container::after {
  bottom: -140px;
  left: -120px;
  width: 420px;
  height: 420px;
  background: radial-gradient(circle, rgba(15, 118, 110, 0.12) 0%, rgba(15, 118, 110, 0) 72%);
}

:deep(.auth-page-shell) {
  position: relative;
  z-index: 1;
  min-height: 100vh;
  padding: clamp(16px, 3vw, 32px);
  display: flex;
  align-items: center;
  justify-content: center;
}

:deep(.auth-card-shell) {
  width: min(580px, 100%);
  border-radius: var(--sgx-radius-lg);
  border: 1px solid var(--sgx-border);
  box-shadow: var(--sgx-shadow-lg);
  background: rgba(255, 255, 255, 0.96);
  backdrop-filter: blur(4px);
}

:deep(.auth-card-header) {
  padding-bottom: 10px;
}

:deep(.auth-card-title) {
  font-size: clamp(1.25rem, 1.1rem + 0.6vw, 1.75rem);
  line-height: 1.2;
  font-weight: 800;
  color: var(--sgx-text);
}

:deep(.auth-card-subtitle) {
  margin-top: 8px;
  color: var(--sgx-muted);
}

:deep(.auth-card-note) {
  color: var(--sgx-muted);
  font-size: 0.92rem;
}

@media (max-width: 768px) {
  .auth-layout__container::before,
  .auth-layout__container::after {
    opacity: 0.7;
    transform: scale(0.8);
  }
}
</style>
