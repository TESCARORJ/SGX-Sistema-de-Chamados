<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

const senhaAtual = ref('')
const novaSenha = ref('')
const confirmacaoNovaSenha = ref('')
const carregando = ref(false)
const mensagemSucesso = ref<string | null>(null)
const mensagemErro = ref<string | null>(null)

const podeSalvar = computed(
  () =>
    !carregando.value &&
    senhaAtual.value.trim().length > 0 &&
    novaSenha.value.trim().length > 0 &&
    confirmacaoNovaSenha.value.trim().length > 0
)

function rotaPorPerfil(): '/admin' | '/portal' {
  const perfis = authStore.usuario?.perfis ?? []
  if (perfis.includes('Administrador') || perfis.includes('Atendente')) {
    return '/admin'
  }

  return '/portal'
}

async function salvar(): Promise<void> {
  mensagemSucesso.value = null
  mensagemErro.value = null

  if (!podeSalvar.value) {
    mensagemErro.value = 'Preencha todos os campos para continuar.'
    return
  }

  carregando.value = true
  try {
    const mensagem = await authStore.alterarSenhaLocal({
      senhaAtual: senhaAtual.value,
      novaSenha: novaSenha.value,
      confirmacaoNovaSenha: confirmacaoNovaSenha.value,
    })

    mensagemSucesso.value = mensagem
    senhaAtual.value = ''
    novaSenha.value = ''
    confirmacaoNovaSenha.value = ''

    await router.replace(rotaPorPerfil())
  } catch (error) {
    mensagemErro.value =
      error instanceof Error
        ? error.message
        : 'Não foi possível alterar a senha. Tente novamente.'
  } finally {
    carregando.value = false
  }
}

async function sair(): Promise<void> {
  await authStore.logout()
  await router.replace('/login')
}
</script>

<template>
  <q-page class="row items-center justify-center q-pa-md alterar-senha-page">
    <q-card flat bordered class="alterar-senha-card">
      <q-card-section>
        <div class="text-h6 text-weight-bold">Troca obrigatória de senha</div>
        <div class="text-body2 text-grey-8 q-mt-xs">
          Para continuar no sistema, atualize sua senha local SGX.
        </div>
      </q-card-section>

      <q-card-section class="q-pt-none">
        <q-banner rounded class="bg-blue-1 text-primary">
          Requisitos da nova senha: mínimo de 12 caracteres, letra maiúscula, letra minúscula,
          número e caractere especial.
        </q-banner>
      </q-card-section>

      <q-card-section class="q-pt-none">
        <q-form class="q-gutter-md" @submit.prevent="salvar">
          <q-input
            v-model="senhaAtual"
            outlined
            type="password"
            autocomplete="current-password"
            label="Senha atual"
          />

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
            label="Salvar nova senha"
            class="full-width"
            :disable="!podeSalvar"
            :loading="carregando"
          />

          <q-btn
            color="grey-8"
            outline
            label="Sair"
            class="full-width"
            :disable="carregando"
            @click="sair"
          />
        </q-form>
      </q-card-section>

      <q-card-section v-if="mensagemSucesso" class="q-pt-none">
        <q-banner rounded class="bg-green-1 text-positive">{{ mensagemSucesso }}</q-banner>
      </q-card-section>

      <q-card-section v-if="mensagemErro" class="q-pt-none">
        <q-banner rounded class="bg-red-1 text-negative">{{ mensagemErro }}</q-banner>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.alterar-senha-page {
  background:
    radial-gradient(circle at 0% 0%, rgba(30, 136, 229, 0.16), transparent 40%),
    radial-gradient(circle at 100% 100%, rgba(15, 118, 110, 0.14), transparent 38%),
    linear-gradient(140deg, #f4f8ff 0%, #edf5ff 100%);
}

.alterar-senha-card {
  width: min(560px, 100%);
  border-radius: 16px;
}
</style>
