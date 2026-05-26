<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useQuasar } from 'quasar'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import MetricCard from '../components/ui/MetricCard.vue'
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

const totalDestinatariosAtivos = computed(
  () => [form.notificarAtendente, form.notificarGestor, form.notificarDepartamento].filter(Boolean).length
)

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
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar a configuracao de alertas.'
  } finally {
    loading.value = false
  }
}

async function salvar(): Promise<void> {
  saving.value = true
  erro.value = null

  try {
    await slaPoliciesService.atualizarConfiguracaoAlertas({ ...form })
    $q.notify({ type: 'positive', message: 'Configuracao de alertas salva com sucesso.' })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel salvar a configuracao.'
  } finally {
    saving.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      contexto="SLA e monitoramento"
      titulo="Alertas de SLA"
      subtitulo="Defina janelas de aviso por prazo de resposta e resolucao."
    />

    <ErrorState v-if="erro" :mensagem="erro" @retry="carregar" />
    <LoadingState v-else-if="loading" inline mensagem="Carregando configuracao de alertas..." />

    <template v-else>
      <div class="sgx-kpi-grid">
        <MetricCard
          title="Status dos alertas"
          :value="form.ativo ? 'Ativos' : 'Inativos'"
          :tone="form.ativo ? 'positive' : 'warning'"
          icon="notifications_active"
        />
        <MetricCard
          title="Janela primeira resposta"
          :value="`${form.minutosAntesVencimentoPrimeiraResposta} min`"
          tone="info"
          icon="timer"
        />
        <MetricCard
          title="Janela resolucao"
          :value="`${form.minutosAntesVencimentoResolucao} min`"
          tone="warning"
          icon="schedule"
        />
        <MetricCard
          title="Destinatarios ativos"
          :value="totalDestinatariosAtivos"
          caption="Atendente, gestor e departamento"
          tone="primary"
          icon="groups"
        />
      </div>

      <AppSectionCard
        titulo="Configuracao atual"
        subtitulo="A rotina periodica usa estes valores para registrar eventos de alerta."
      >
        <q-form class="column q-gutter-md" @submit.prevent="salvar">
          <q-banner rounded class="bg-grey-1 text-grey-9">
            Ajuste minutos de antecedencia e destinatarios conforme a criticidade operacional.
          </q-banner>

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
                label="Minutos antes da resolucao"
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
    </template>
  </q-page>
</template>
