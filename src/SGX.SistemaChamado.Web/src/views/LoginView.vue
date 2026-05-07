<script setup lang="ts">
import { computed, ref } from 'vue'
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

const mensagemErro = computed(() => erroLocal.value || authStore.erro)

async function entrarComMicrosoft(): Promise<void> {
  erroLocal.value = null

  try {
    await authStore.loginMicrosoft()
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Nao foi possivel concluir o login Microsoft.'
  }
}

async function entrarModoLocal(): Promise<void> {
  erroLocal.value = null

  const emailInformado = emailLocal.value.trim().toLowerCase()
  const senhaInformada = senhaLocal.value

  if (!emailInformado) {
    erroLocal.value = 'Informe o e-mail para continuar no modo local.'
    return
  }

  if (!senhaInformada) {
    erroLocal.value = 'Informe a senha local de desenvolvimento.'
    return
  }

  if (emailInformado !== adminLocalEmail) {
    erroLocal.value = 'Use o e-mail admin@sgxdigital.com para login local Development.'
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
    const message = error instanceof Error ? error.message : 'Falha ao autenticar no modo local.'

    if (message.includes('Failed to fetch') || message.includes('NetworkError')) {
      erroLocal.value = 'API indisponivel. Verifique se a API esta ativa em http://localhost:5168.'
      return
    }

    erroLocal.value = message
  }
}
</script>

<template>
  <q-page class="row items-center justify-center q-pa-md">
    <q-card flat bordered class="sgx-card login-card">
      <q-card-section class="text-center q-pb-sm">
        <q-icon name="support_agent" color="primary" size="42px" />
        <div class="text-h5 text-weight-bold q-mt-sm">SGX Sistema de Chamados</div>
        <div class="text-subtitle2 text-grey-8 q-mt-xs">Atendimento interno, rastreável e organizado.</div>
        <div class="text-body2 text-grey-7 q-mt-sm">Entre com sua conta corporativa para acessar o sistema.</div>
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

      <q-card-section v-if="localAuthEnabled" class="q-pt-none">
        <div class="row items-center no-wrap q-mb-md">
          <q-separator class="col" inset />
          <div class="q-px-sm text-caption text-grey-7">ou</div>
          <q-separator class="col" inset />
        </div>

        <div class="text-subtitle1 text-weight-medium">Login administrativo local</div>
        <div class="text-body2 text-grey-8 q-mt-xs">Uso exclusivo para desenvolvimento local.</div>

        <q-banner rounded class="bg-orange-1 text-warning q-mt-md">
          Este bloco nao aparece em Production e serve apenas para ambiente de desenvolvimento.
        </q-banner>

        <q-form class="q-gutter-md q-mt-md" @submit.prevent="entrarModoLocal">
          <q-input
            v-model="emailLocal"
            outlined
            type="email"
            autocomplete="username"
            label="E-mail"
            placeholder="admin@sgxdigital.com"
            :rules="[(v) => !!String(v || '').trim() || 'Informe o e-mail']"
          />

          <q-input
            v-model="senhaLocal"
            outlined
            type="password"
            autocomplete="current-password"
            label="Senha"
            placeholder="Admin@123456"
            :rules="[(v) => !!String(v || '').trim() || 'Informe a senha']"
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

      <q-card-section v-if="mensagemErro" class="q-pt-none">
        <q-banner rounded class="bg-red-1 text-negative">
          {{ mensagemErro }}
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.login-card {
  width: min(560px, 100%);
}
</style>

