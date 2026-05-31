<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/authStore'
import { useQuasar } from 'quasar'

const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

onMounted(async () => {
  if (!authStore.inicializado) {
    await authStore.inicializarSessao()
  }
})

const emulando = computed(() => authStore.modoLocal && authStore.emulandoPerfil)
const sairCarregando = ref(false)

const rotaDestino = computed(() => {
  if (!authStore.autenticado) {
    return '/login'
  }

  return authStore.rotaInicial
})

const labelDestino = computed(() => (authStore.autenticado ? 'Ir para meu painel' : 'Ir para login'))
const iconDestino = computed(() => (authStore.autenticado ? 'space_dashboard' : 'login'))

function voltar(): void {
  router.back()
}

function irParaDestino(): void {
  router.push(rotaDestino.value)
}

async function sairVisao(): Promise<void> {
  sairCarregando.value = true
  try {
    await authStore.encerrarEmulacao()
    $q.notify({
      type: 'success',
      message: 'Voltou para o perfil original',
      position: 'top',
      timeout: 1000
    })
    await router.replace(authStore.rotaInicial)
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Não foi possível encerrar a emulação.'
    $q.notify({
      type: 'negative',
      message,
    })
  } finally {
    sairCarregando.value = false
  }
}
</script>

<template>
  <q-page class="auth-page-shell">
    <q-card flat bordered class="sgx-card auth-card-shell denied-card">
      <q-card-section class="text-center auth-card-header">
        <q-avatar size="64px" color="red-1" text-color="negative">
          <q-icon name="gpp_bad" size="34px" />
        </q-avatar>
        <div class="auth-card-title q-mt-md">Acesso negado</div>
        <div class="auth-card-subtitle">
          Seu usuário está autenticado, mas não possui autorização para esta área.
          Se necessário, solicite a liberação ao administrador do SGX.
        </div>
      </q-card-section>

      <q-card-section class="q-pt-none q-px-lg q-pb-lg">
        <div class="row q-col-gutter-sm q-mb-sm" v-if="emulando">
          <div class="col-12">
            <q-btn
              color="secondary"
              unelevated
              icon="visibility_off"
              label="Sair da visão de homologação"
              class="full-width"
              :loading="sairCarregando"
              @click="sairVisao"
            />
          </div>
        </div>
        <div class="row q-col-gutter-sm">
          <div class="col-12 col-sm-6">
            <q-btn flat color="primary" icon="arrow_back" label="Voltar" class="full-width" @click="voltar" />
          </div>
          <div class="col-12 col-sm-6">
            <q-btn color="primary" unelevated :icon="iconDestino" :label="labelDestino" class="full-width" @click="irParaDestino" />
          </div>
        </div>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<style scoped>
.denied-card {
  max-width: 560px;
}
</style>
