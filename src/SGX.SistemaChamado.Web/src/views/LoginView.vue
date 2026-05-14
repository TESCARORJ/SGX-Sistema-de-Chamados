<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { httpClient } from '../services/httpClient'
import { useAuthStore } from '../stores/authStore'
import type { ProvedoresAutenticacaoResponse } from '../types/auth'

const authStore = useAuthStore()
const router = useRouter()

const adminLocalEmail = 'admin@sgxdigital.com'
const adminLocalNome = 'Administrador SGX'

const emailLocalSgx = ref('')
const senhaLocalSgx = ref('')
const erroLocal = ref<string | null>(null)
const carregandoProvedores = ref(true)
const provedores = ref<ProvedoresAutenticacaoResponse | null>(null)

const fallbackLocalDevelopment =
  !import.meta.env.PROD &&
  (import.meta.env.DEV || import.meta.env.VITE_AUTH_MODO_LOCAL === 'true')

const autenticando = computed(() => authStore.carregando || authStore.inicializandoSessao)
const mensagemErro = computed(() => erroLocal.value || authStore.erroAutenticacao || authStore.erro)

const loginMicrosoftDisponivel = computed(() => provedores.value?.loginMicrosoftHabilitado ?? false)
const loginLocalSgxDisponivel = computed(() => provedores.value?.loginLocalSgxHabilitado ?? false)
const loginLocalDevelopmentDisponivel = computed(
  () => provedores.value?.loginLocalDevelopmentHabilitado ?? fallbackLocalDevelopment
)

const possuiAlgumProvedor = computed(
  () =>
    loginMicrosoftDisponivel.value ||
    loginLocalSgxDisponivel.value ||
    loginLocalDevelopmentDisponivel.value
)

async function carregarProvedoresAutenticacao(): Promise<void> {
  carregandoProvedores.value = true
  erroLocal.value = null

  try {
    provedores.value = await httpClient.get<ProvedoresAutenticacaoResponse>('/api/auth/provedores')
  } catch {
    // Mantém a tela utilizável sem erro técnico bloqueante.
    provedores.value = {
      provedorPrincipal: 'Local',
      loginMicrosoftHabilitado: false,
      loginLocalSgxHabilitado: false,
      loginLocalDevelopmentHabilitado: fallbackLocalDevelopment,
    }
  } finally {
    carregandoProvedores.value = false
  }
}

async function entrarComMicrosoft(): Promise<void> {
  if (autenticando.value) {
    return
  }

  erroLocal.value = null

  try {
    await authStore.loginMicrosoft()
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value =
      error instanceof Error ? error.message : 'Não foi possível concluir a autenticação. Tente novamente.'
  }
}

async function entrarComEmailSenha(): Promise<void> {
  if (autenticando.value) {
    return
  }

  erroLocal.value = null

  const email = emailLocalSgx.value.trim().toLowerCase()
  const senha = senhaLocalSgx.value

  if (!email) {
    erroLocal.value = 'Informe o e-mail para continuar.'
    return
  }

  if (!senha) {
    erroLocal.value = 'Informe a senha para continuar.'
    return
  }

  try {
    await authStore.loginLocalSgx({ email, senha })
    if (authStore.deveAlterarSenha) {
      await router.replace('/alterar-senha')
      return
    }

    await router.replace(authStore.rotaInicial)
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Não foi possível concluir o login local SGX.'
  }
}

async function entrarDevelopmentLocal(): Promise<void> {
  if (autenticando.value) {
    return
  }

  erroLocal.value = null

  try {
    await authStore.loginLocalDevelopment({
      email: adminLocalEmail,
      nome: adminLocalNome,
      perfil: 'Administrador',
    })

    await router.replace('/admin')
  } catch (error) {
    erroLocal.value = error instanceof Error ? error.message : 'Não foi possível concluir o login local Development.'
  }
}

onMounted(() => {
  void carregarProvedoresAutenticacao()
})
</script>

<template>
  <q-page class="login-page row items-center justify-center q-pa-md">
    <q-card flat bordered class="sgx-card login-card">
      <q-card-section class="text-center q-pb-sm">
        <q-chip color="blue-1" text-color="primary" icon="verified_user" class="login-brand-chip">
          SGX
        </q-chip>
        <div class="text-h5 text-weight-bold q-mt-sm">SGX Sistema de Chamados</div>
        <div class="text-subtitle2 text-grey-8 q-mt-xs">Atendimento interno, rastreável e organizado.</div>
        <div class="text-body2 text-grey-7 q-mt-sm">
          Escolha o provedor de autenticação para acessar o sistema.
        </div>
      </q-card-section>

      <q-card-section v-if="carregandoProvedores" class="q-pt-none">
        <q-linear-progress indeterminate color="primary" />
      </q-card-section>

      <template v-else>
        <q-card-section v-if="!possuiAlgumProvedor" class="q-pt-none">
          <q-banner rounded class="bg-orange-1 text-warning">
            Nenhum método de autenticação está configurado. Contate o administrador do sistema.
          </q-banner>
        </q-card-section>

        <q-card-section v-if="loginMicrosoftDisponivel">
          <div class="text-body2 text-grey-8 q-mb-sm">
            Entre com sua conta corporativa Microsoft Entra ID.
          </div>
          <q-btn
            color="primary"
            unelevated
            icon="login"
            class="full-width"
            size="md"
            label="Entrar com Microsoft Entra ID"
            :loading="autenticando"
            :disable="autenticando"
            @click="entrarComMicrosoft"
          />
        </q-card-section>

        <q-card-section v-if="loginLocalSgxDisponivel" :class="loginMicrosoftDisponivel ? 'q-pt-none' : ''">
          <div v-if="loginMicrosoftDisponivel" class="row items-center no-wrap q-mb-md">
            <q-separator class="col" inset />
            <div class="q-px-sm text-caption text-grey-7">ou</div>
            <q-separator class="col" inset />
          </div>

          <div class="text-subtitle1 text-weight-medium">Login local SGX</div>
          <div class="text-body2 text-grey-8 q-mt-xs">Acesso por e-mail e senha com autenticação local da API.</div>

          <q-form class="q-gutter-md q-mt-md" @submit.prevent="entrarComEmailSenha">
            <q-input
              v-model="emailLocalSgx"
              outlined
              type="email"
              autocomplete="username"
              label="E-mail"
              :rules="[(v) => !!String(v || '').trim() || 'Informe o e-mail']"
            />

            <q-input
              v-model="senhaLocalSgx"
              outlined
              type="password"
              autocomplete="current-password"
              label="Senha"
              :rules="[(v) => !!String(v || '').trim() || 'Informe a senha']"
            />

            <q-btn
              type="submit"
              color="secondary"
              unelevated
              icon="mail"
              class="full-width"
              :loading="autenticando"
              :disable="autenticando"
              label="Entrar com e-mail e senha"
            />

            <div class="text-right">
              <router-link to="/recuperar-senha" class="text-primary">Esqueci minha senha</router-link>
            </div>
          </q-form>
        </q-card-section>

        <q-card-section v-if="loginLocalDevelopmentDisponivel" class="q-pt-none">
          <div class="row items-center no-wrap q-mb-md">
            <q-separator class="col" inset />
            <div class="q-px-sm text-caption text-grey-7">desenvolvimento</div>
            <q-separator class="col" inset />
          </div>

          <div class="text-subtitle1 text-weight-medium">Login local Development</div>
          <div class="text-body2 text-grey-8 q-mt-xs">Uso exclusivo para desenvolvimento local.</div>

          <q-banner rounded class="bg-orange-1 text-warning q-mt-md">
            Este acesso existe apenas em ambiente Development.
          </q-banner>

          <q-btn
            class="full-width q-mt-md"
            color="grey-8"
            outline
            icon="admin_panel_settings"
            label="Entrar como administrador local (Development)"
            :loading="autenticando"
            :disable="autenticando"
            @click="entrarDevelopmentLocal"
          />
        </q-card-section>
      </template>

      <q-card-section v-if="mensagemErro" class="q-pt-none">
        <q-banner rounded class="bg-red-1 text-negative">
          {{ mensagemErro }}
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.login-page {
  background:
    radial-gradient(circle at 8% 0%, rgba(11, 94, 215, 0.16), transparent 40%),
    radial-gradient(circle at 100% 100%, rgba(2, 132, 199, 0.14), transparent 38%),
    linear-gradient(135deg, #f3f7fd 0%, #edf3fb 100%);
}

.login-card {
  width: min(560px, 100%);
  border-radius: 18px;
}

.login-brand-chip {
  border: 1px solid rgba(11, 94, 215, 0.25);
  font-weight: 700;
}
</style>
