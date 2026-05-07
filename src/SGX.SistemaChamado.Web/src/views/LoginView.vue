<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const adminLocalEmail = 'admin@sgxdigital.com'
const adminLocalNome = 'Administrador SGX'
const adminLocalSenha = 'Admin@123456'

const emailLocal = ref(adminLocalEmail)
const senhaLocal = ref(adminLocalSenha)
const erroLocal = ref<string | null>(null)

const localAuthEnabled =
  !import.meta.env.PROD &&
  (import.meta.env.DEV || import.meta.env.VITE_AUTH_MODO_LOCAL === 'true')

async function entrarComMicrosoft(): Promise<void> {
  erroLocal.value = null

  try {
    await authStore.loginMicrosoft()
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Falha no login Microsoft.'
  }
}

async function entrarModoLocal(): Promise<void> {
  erroLocal.value = null
  const emailInformado = emailLocal.value.trim().toLowerCase()
  const senhaInformada = senhaLocal.value

  if (!emailInformado) {
    erroLocal.value = 'Informe o e-mail para login local.'
    return
  }

  if (!senhaInformada) {
    erroLocal.value = 'Informe a senha para login local.'
    return
  }

  if (emailInformado !== adminLocalEmail) {
    erroLocal.value = 'Para modo local Development use o e-mail admin@sgxdigital.com.'
    return
  }

  if (senhaInformada !== adminLocalSenha) {
    erroLocal.value = 'Senha local invalida.'
    return
  }

  try {
    await authStore.loginLocalDev({
      email: adminLocalEmail,
      nome: adminLocalNome,
      perfil: 'Administrador',
    })

    await router.replace('/admin')
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Falha no modo local.'
    if (message.includes('Failed to fetch')) {
      erroLocal.value = 'API indisponivel. Verifique se a API esta rodando em http://localhost:5168.'
      return
    }

    erroLocal.value = message
  }
}
</script>

<template>
  <q-page class="row items-center justify-center sgx-page">
    <q-card class="sgx-card login-card" flat bordered>
      <q-card-section class="text-center">
        <div class="text-h5 text-weight-bold">SGX Sistema de Chamados</div>
        <div class="text-body2 text-grey-8 q-mt-xs">
          Entre com sua conta corporativa para acessar o sistema.
        </div>
      </q-card-section>

      <q-card-section>
        <q-btn
          color="primary"
          unelevated
          icon="login"
          class="full-width"
          label="Entrar com Microsoft"
          :loading="authStore.carregando"
          @click="entrarComMicrosoft"
        />
      </q-card-section>

      <template v-if="localAuthEnabled">
        <q-card-section class="q-pb-none">
          <div class="row items-center no-wrap">
            <q-separator class="col" inset />
            <div class="q-px-sm text-caption text-grey-7">ou</div>
            <q-separator class="col" inset />
          </div>
        </q-card-section>

        <q-card-section>
          <div class="text-subtitle1 text-weight-medium">Login administrativo local</div>
          <div class="text-body2 text-grey-8 q-mt-xs">Uso exclusivo para desenvolvimento local.</div>

          <q-banner rounded class="bg-orange-1 text-warning q-mt-md">
            Login administrativo local disponivel somente em Development.
          </q-banner>

          <q-form class="q-gutter-md q-mt-md" @submit.prevent="entrarModoLocal">
            <q-input
              v-model="emailLocal"
              outlined
              type="email"
              autocomplete="username"
              label="E-mail"
              placeholder="admin@sgxdigital.com"
            />

            <q-input
              v-model="senhaLocal"
              outlined
              type="password"
              autocomplete="current-password"
              label="Senha"
              placeholder="Admin@123456"
            />

            <q-btn
              type="submit"
              color="secondary"
              unelevated
              icon="admin_panel_settings"
              class="full-width"
              :loading="authStore.carregando"
              label="Entrar como administrador local"
            />
          </q-form>
        </q-card-section>
      </template>

      <q-card-section v-if="erroLocal || authStore.erro">
        <q-banner rounded class="bg-red-1 text-negative">
          {{ erroLocal || authStore.erro }}
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.login-card {
  width: min(520px, 100%);
}
</style>
