<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useQuasar } from 'quasar'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { slaPoliciesService } from '../services/slaPoliciesService'
import type { AtualizarConfiguracaoAlertaSlaRequest } from '../types/slaPolicies'

const $q = useQuasar()
const loading = ref(false)
const saving = ref(false)
const erro = ref<string | null>(null)

const form = reactive<AtualizarConfiguracaoAlertaSlaRequest>({
  ativo: true,
  minutosAntesVencimentoPrimeiraResposta: 30,
  minutosAntesVencimentoResolucao: 120,
  notificarAtendente: true,
  notificarGestor: false,
  notificarDepartamento: false,
})

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const config = await slaPoliciesService.obterConfiguracaoAlertas()
    form.ativo = config.ativo
    form.minutosAntesVencimentoPrimeiraResposta = config.minutosAntesVencimentoPrimeiraResposta
    form.minutosAntesVencimentoResolucao = config.minutosAntesVencimentoResolucao
    form.notificarAtendente = config.notificarAtendente
    form.notificarGestor = config.notificarGestor
    form.notificarDepartamento = config.notificarDepartamento
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar a configuração de alertas.'
  } finally {
    loading.value = false
  }
}

async function salvar(): Promise<void> {
  saving.value = true
  erro.value = null

  try {
    await slaPoliciesService.atualizarConfiguracaoAlertas({ ...form })
    $q.notify({ type: 'positive', message: 'Configuração de alertas salva com sucesso.' })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível salvar a configuração.'
  } finally {
    saving.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Alertas de SLA" subtitulo="Configure janelas de alerta e destinatários preparados para notificações futuras." />

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
    <LoadingState v-else-if="loading" inline mensagem="Carregando configuração de alertas..." />

    <AppSectionCard v-else titulo="Configuração atual" subtitulo="A rotina periódica usa estes valores para registrar eventos de alerta.">
      <q-form class="column q-gutter-md" @submit.prevent="salvar">
        <q-toggle v-model="form.ativo" label="Alertas ativos" />

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <q-input
              v-model.number="form.minutosAntesVencimentoPrimeiraResposta"
              type="number"
              min="0"
              outlined
              label="Minutos antes da primeira resposta"
            />
          </div>
          <div class="col-12 col-md-6">
            <q-input
              v-model.number="form.minutosAntesVencimentoResolucao"
              type="number"
              min="0"
              outlined
              label="Minutos antes da resolução"
            />
          </div>
        </div>

        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-4">
            <q-toggle v-model="form.notificarAtendente" label="Notificar atendente" />
          </div>
          <div class="col-12 col-md-4">
            <q-toggle v-model="form.notificarGestor" label="Notificar gestor" />
          </div>
          <div class="col-12 col-md-4">
            <q-toggle v-model="form.notificarDepartamento" label="Notificar departamento" />
          </div>
        </div>

        <div class="row justify-end">
          <q-btn color="primary" icon="save" label="Salvar" type="submit" :loading="saving" />
        </div>
      </q-form>
    </AppSectionCard>
  </q-page>
</template>
