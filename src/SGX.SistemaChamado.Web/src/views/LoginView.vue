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
  <q-page class="auth-page-shell">
    <q-card flat bordered class="sgx-card auth-card-shell login-card">
      <q-card-section class="text-center auth-card-header">
        <q-chip color="blue-1" text-color="primary" icon="verified_user" class="login-brand-chip">
          Acesso seguro
        </q-chip>
        <div class="auth-card-title q-mt-sm">SGX Sistema de Chamados</div>
        <div class="auth-card-subtitle">Service desk corporativo para operação ITSM</div>
        <div class="auth-card-note q-mt-sm">
          Autentique-se para acessar o painel com rastreabilidade e segurança.
        </div>
      </q-card-section>

      <q-card-section v-if="carregandoProvedores" class="q-pt-none q-px-lg">
        <q-linear-progress indeterminate color="primary" />
      </q-card-section>

      <template v-else>
        <q-card-section v-if="!possuiAlgumProvedor" class="q-pt-none q-px-lg">
          <q-banner rounded class="bg-orange-1 text-warning auth-feedback">
            Nenhum método de autenticação está configurado. Contate o administrador do sistema.
          </q-banner>
        </q-card-section>

        <q-card-section v-if="loginMicrosoftDisponivel" class="q-pt-none q-px-lg q-pb-md">
          <div class="text-body2 text-grey-8 q-mb-sm auth-provider-copy">
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

        <q-card-section v-if="loginLocalSgxDisponivel" class="q-px-lg" :class="loginMicrosoftDisponivel ? 'q-pt-none' : 'q-pt-sm'">
          <div v-if="loginMicrosoftDisponivel" class="row items-center no-wrap q-mb-md">
            <q-separator class="col" inset />
            <div class="q-px-sm text-caption text-grey-7">ou</div>
            <q-separator class="col" inset />
          </div>

          <div class="text-subtitle1 text-weight-bold">Acesso por credenciais SGX</div>
          <div class="text-body2 text-grey-8 q-mt-xs auth-provider-copy">Entre com e-mail corporativo e senha local.</div>

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
              color="primary"
              unelevated
              icon="login"
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

        <q-card-section v-if="loginLocalDevelopmentDisponivel" class="q-pt-none q-px-lg q-pb-lg">
          <div class="row items-center no-wrap q-mb-md">
            <q-separator class="col" inset />
            <div class="q-px-sm text-caption text-grey-7">desenvolvimento</div>
            <q-separator class="col" inset />
          </div>

          <div class="text-subtitle1 text-weight-bold">Acesso local de desenvolvimento</div>
          <div class="text-body2 text-grey-8 q-mt-xs auth-provider-copy">Disponível somente para ambiente local de testes.</div>

          <q-banner rounded class="bg-orange-1 text-warning q-mt-md auth-feedback">
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

      <q-card-section v-if="mensagemErro" class="q-pt-none q-px-lg q-pb-lg">
        <q-banner rounded class="bg-red-1 text-negative auth-feedback">
          {{ mensagemErro }}
        </q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.login-card {
  max-width: 580px;
}

.login-brand-chip {
  border: 1px solid rgba(11, 94, 215, 0.25);
  font-weight: 700;
}

.auth-provider-copy {
  line-height: 1.4;
}

.auth-feedback {
  border: 1px solid rgba(15, 23, 42, 0.06);
}
</style>
