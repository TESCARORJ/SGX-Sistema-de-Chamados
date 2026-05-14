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
  <q-page class="row items-center justify-center q-pa-md recuperar-senha-page">
    <q-card flat bordered class="recuperar-senha-card">
      <q-card-section>
        <div class="text-h6 text-weight-bold">
          {{ modoRedefinicao ? 'Redefinir senha' : 'Recuperar senha' }}
        </div>
        <div class="text-body2 text-grey-8 q-mt-xs">
          {{
            modoRedefinicao
              ? 'Defina uma nova senha para o acesso local SGX.'
              : 'Informe seu e-mail para receber instruções de redefinição de senha.'
          }}
        </div>
      </q-card-section>

      <q-card-section class="q-pt-none" v-if="!modoRedefinicao">
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

      <q-card-section class="q-pt-none" v-else>
        <q-banner rounded class="bg-blue-1 text-primary q-mb-md">
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
            color="secondary"
            unelevated
            class="full-width"
            label="Redefinir senha"
            :loading="carregando"
            :disable="carregando"
          />
        </q-form>
      </q-card-section>

      <q-card-section v-if="mensagemSucesso" class="q-pt-none">
        <q-banner rounded class="bg-green-1 text-positive">{{ mensagemSucesso }}</q-banner>
      </q-card-section>

      <q-card-section v-if="mensagemErro" class="q-pt-none">
        <q-banner rounded class="bg-red-1 text-negative">{{ mensagemErro }}</q-banner>
      </q-card-section>

      <q-card-section class="q-pt-none">
        <router-link to="/login" class="text-primary">Voltar para o login</router-link>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.recuperar-senha-page {
  background:
    radial-gradient(circle at 15% 10%, rgba(2, 132, 199, 0.14), transparent 38%),
    radial-gradient(circle at 100% 100%, rgba(22, 163, 74, 0.12), transparent 34%),
    linear-gradient(150deg, #f5f9ff 0%, #eef6ff 100%);
}

.recuperar-senha-card {
  width: min(560px, 100%);
  border-radius: 16px;
}
</style>
