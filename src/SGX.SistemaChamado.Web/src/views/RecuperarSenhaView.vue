<script setup lang="ts">
import { computed, ref } from 'vue'
import { RouterLink, useRoute } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const route = useRoute()

const email = ref('')
const novaSenha = ref('')
const confirmacaoNovaSenha = ref('')
const carregando = ref(false)
const mensagemSucesso = ref<string | null>(null)
const mensagemErro = ref<string | null>(null)

const token = computed(() => {
  const valor = route.query.token
  return typeof valor === 'string' ? valor.trim() : ''
})

const modoRedefinicao = computed(() => token.value.length > 0)

async function enviarSolicitacao(): Promise<void> {
  mensagemSucesso.value = null
  mensagemErro.value = null

  if (!email.value.trim()) {
    mensagemErro.value = 'Informe o e-mail para continuar.'
    return
  }

  carregando.value = true
  try {
    const mensagem = await authStore.solicitarRecuperacaoSenha(email.value.trim().toLowerCase())
    mensagemSucesso.value = mensagem
  } catch (error) {
    mensagemErro.value =
      error instanceof Error
        ? error.message
        : 'Não foi possível processar a solicitação de recuperação.'
  } finally {
    carregando.value = false
  }
}

async function redefinirSenha(): Promise<void> {
  mensagemSucesso.value = null
  mensagemErro.value = null

  if (!token.value) {
    mensagemErro.value = 'Token de recuperação inválido.'
    return
  }

  if (!novaSenha.value.trim() || !confirmacaoNovaSenha.value.trim()) {
    mensagemErro.value = 'Informe a nova senha e a confirmação.'
    return
  }

  carregando.value = true
  try {
    const mensagem = await authStore.redefinirSenhaLocal({
      token: token.value,
      novaSenha: novaSenha.value,
      confirmacaoNovaSenha: confirmacaoNovaSenha.value,
    })

    mensagemSucesso.value = mensagem
    novaSenha.value = ''
    confirmacaoNovaSenha.value = ''
  } catch (error) {
    mensagemErro.value =
      error instanceof Error
        ? error.message
        : 'Não foi possível redefinir a senha.'
  } finally {
    carregando.value = false
  }
}
</script>

<template>
  <q-page class="auth-page-shell">
    <q-card flat bordered class="sgx-card auth-card-shell recuperar-senha-card">
      <q-card-section class="auth-card-header">
        <q-chip color="blue-1" text-color="primary" icon="lock_reset" class="recuperar-senha-chip">
          Recuperação segura
        </q-chip>
        <div class="auth-card-title q-mt-sm">
          {{ modoRedefinicao ? 'Redefinir senha' : 'Recuperar senha' }}
        </div>
        <div class="auth-card-subtitle">
          {{
            modoRedefinicao
              ? 'Defina sua nova senha para restabelecer o acesso local SGX.'
              : 'Informe seu e-mail corporativo para receber as instruções de redefinição.'
          }}
        </div>
      </q-card-section>

      <q-card-section class="q-pt-none q-px-lg" v-if="!modoRedefinicao">
        <q-form class="q-gutter-md" @submit.prevent="enviarSolicitacao">
          <q-input
            v-model="email"
            outlined
            type="email"
            autocomplete="email"
            label="E-mail"
          />

          <q-btn
            type="submit"
            color="primary"
            unelevated
            class="full-width"
            label="Enviar instruções"
            :loading="carregando"
            :disable="carregando"
          />
        </q-form>
      </q-card-section>

      <q-card-section class="q-pt-none q-px-lg" v-else>
        <q-banner rounded class="bg-blue-1 text-primary q-mb-md auth-feedback">
          A nova senha deve conter mínimo de 12 caracteres, letra maiúscula, letra minúscula,
          número e caractere especial.
        </q-banner>

        <q-form class="q-gutter-md" @submit.prevent="redefinirSenha">
          <q-input
            v-model="novaSenha"
            outlined
            type="password"
            autocomplete="new-password"
            label="Nova senha"
          />

          <q-input
            v-model="confirmacaoNovaSenha"
            outlined
            type="password"
            autocomplete="new-password"
            label="Confirmar nova senha"
          />

          <q-btn
            type="submit"
            color="primary"
            unelevated
            class="full-width"
            label="Redefinir senha"
            :loading="carregando"
            :disable="carregando"
          />
        </q-form>
      </q-card-section>

      <q-card-section v-if="mensagemSucesso" class="q-pt-none q-px-lg q-pb-sm">
        <q-banner rounded class="bg-green-1 text-positive auth-feedback">{{ mensagemSucesso }}</q-banner>
      </q-card-section>

      <q-card-section v-if="mensagemErro" class="q-pt-none q-px-lg q-pb-sm">
        <q-banner rounded class="bg-red-1 text-negative auth-feedback">{{ mensagemErro }}</q-banner>
      </q-card-section>

      <q-card-section class="q-pt-none q-px-lg q-pb-lg">
        <RouterLink to="/login" class="text-primary text-weight-medium">Voltar para o login</RouterLink>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.recuperar-senha-card {
  max-width: 580px;
}

.recuperar-senha-chip {
  border: 1px solid rgba(11, 94, 215, 0.2);
  font-weight: 700;
}

.auth-feedback {
  border: 1px solid rgba(15, 23, 42, 0.06);
}
</style>
