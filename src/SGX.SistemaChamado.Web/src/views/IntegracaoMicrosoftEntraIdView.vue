<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { integracoesMicrosoftService } from '../services/integracoesMicrosoftService'
import { useAuthStore } from '../stores/authStore'
import type { ProvedorPrincipal } from '../types/auth'
import type {
  AtualizarMicrosoftEntraIdIntegracaoRequest,
  MicrosoftEntraIdIntegracaoResponse,
} from '../types/integracoesMicrosoft'
import {
  deveExigirCamposMicrosoft,
  obterAvisoModo,
  validarIntegracaoMicrosoft,
} from './integracoes-microsoft/validacaoIntegracaoMicrosoftEntraId'

type CampoMicrosoftObrigatorio =
  | 'tenantId'
  | 'clientId'
  | 'audience'
  | 'issuer'
  | 'authority'
  | 'apiScope'
  | 'redirectUri'

const authStore = useAuthStore()
const loading = ref(false)
const salvando = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const confirmarDesabilitacao = ref(false)
const errosCampo = reactive<Record<CampoMicrosoftObrigatorio, string>>({
  tenantId: '',
  clientId: '',
  audience: '',
  issuer: '',
  authority: '',
  apiScope: '',
  redirectUri: '',
})
const form = reactive({
  habilitado: false,
  provedorPrincipal: 'MicrosoftEntraId' as ProvedorPrincipal,
  loginLocalHabilitado: false,
  tenantId: '',
  clientId: '',
  audience: '',
  issuer: '',
  authority: '',
  apiScope: '',
  redirectUri: '',
  dominiosPermitidosTexto: '',
  criarUsuarioAutomaticamente: true,
  perfilPadraoUsuarioMicrosoft: 'Solicitante',
})
const statusConfiguracao = ref('')
const pendenciasConfiguracao = ref<string[]>([])

const opcoesProvedor = [
  { label: 'MicrosoftEntraId', value: 'MicrosoftEntraId' as ProvedorPrincipal },
  { label: 'Local', value: 'Local' as ProvedorPrincipal },
  { label: 'Hibrido', value: 'Hibrido' as ProvedorPrincipal },
]

const opcoesPerfilPadrao = [
  { label: 'Solicitante', value: 'Solicitante' },
  { label: 'Atendente', value: 'Atendente' },
]

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)
const podeVisualizar = computed(
  () =>
    fallbackAdminSemPermissoes.value ||
    authStore.possuiPermissao(permissoes.integracoesMicrosoftVisualizar) ||
    authStore.possuiPermissao(permissoes.integracoesMicrosoftGerenciar)
)
const podeGerenciar = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.integracoesMicrosoftGerenciar)
)

const camposMicrosoftObrigatorios = computed(() =>
  deveExigirCamposMicrosoft(form.habilitado, form.provedorPrincipal)
)

const alertaSemProvedor = computed(() => {
  const payload = criarPayload()
  const validacao = validarIntegracaoMicrosoft(payload)
  return !validacao.valido &&
    validacao.erroGeral === 'Ao menos um provedor de autenticação deve permanecer habilitado.'
})

const avisoDesabilitarMicrosoftPrincipal = computed(
  () => !form.habilitado && form.provedorPrincipal === 'MicrosoftEntraId'
)

const avisoModo = computed(() => obterAvisoModo(form.provedorPrincipal))

function preencherFormulario(config: MicrosoftEntraIdIntegracaoResponse): void {
  form.habilitado = config.habilitado
  form.provedorPrincipal = config.provedorPrincipal
  form.loginLocalHabilitado = config.loginLocalHabilitado
  form.tenantId = config.tenantId ?? ''
  form.clientId = config.clientId ?? ''
  form.audience = config.audience ?? ''
  form.issuer = config.issuer ?? ''
  form.authority = config.authority ?? ''
  form.apiScope = config.apiScope ?? ''
  form.redirectUri = config.redirectUri ?? ''
  form.dominiosPermitidosTexto = (config.dominiosPermitidos ?? []).join('; ')
  form.criarUsuarioAutomaticamente = config.criarUsuarioAutomaticamente
  form.perfilPadraoUsuarioMicrosoft = config.perfilPadraoUsuarioMicrosoft || 'Solicitante'
  statusConfiguracao.value = config.statusConfiguracao
  pendenciasConfiguracao.value = [...(config.pendenciasConfiguracao ?? [])]
}

function extrairDominiosPermitidos(texto: string): string[] {
  return texto
    .split(/[;,\n\r]+/g)
    .map((item) => item.trim())
    .filter((item) => item.length > 0)
}

function criarPayload(): AtualizarMicrosoftEntraIdIntegracaoRequest {
  return {
    habilitado: form.habilitado,
    provedorPrincipal: form.provedorPrincipal,
    loginLocalHabilitado: form.loginLocalHabilitado,
    tenantId: form.tenantId.trim(),
    clientId: form.clientId.trim(),
    audience: form.audience.trim(),
    issuer: form.issuer.trim(),
    authority: form.authority.trim(),
    apiScope: form.apiScope.trim(),
    redirectUri: form.redirectUri.trim(),
    dominiosPermitidos: extrairDominiosPermitidos(form.dominiosPermitidosTexto),
    criarUsuarioAutomaticamente: form.criarUsuarioAutomaticamente,
    perfilPadraoUsuarioMicrosoft: form.perfilPadraoUsuarioMicrosoft,
  }
}

function limparErrosCampo(): void {
  errosCampo.tenantId = ''
  errosCampo.clientId = ''
  errosCampo.audience = ''
  errosCampo.issuer = ''
  errosCampo.authority = ''
  errosCampo.apiScope = ''
  errosCampo.redirectUri = ''
}

function validarFormularioAntesSalvar(): boolean {
  limparErrosCampo()

  const validacao = validarIntegracaoMicrosoft(criarPayload())
  for (const [campo, mensagem] of Object.entries(validacao.errosCampo)) {
    const campoTipado = campo as CampoMicrosoftObrigatorio
    errosCampo[campoTipado] = mensagem
  }

  if (!validacao.valido) {
    erro.value = validacao.erroGeral ?? 'Revise os campos obrigatórios da integração Microsoft Entra ID.'
    return false
  }

  return true
}

function rotuloCampo(nome: string): string {
  return camposMicrosoftObrigatorios.value ? `${nome} *` : nome
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    const response = await integracoesMicrosoftService.obterConfiguracao()
    preencherFormulario(response)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar a integração Microsoft Entra ID.'
  } finally {
    loading.value = false
  }
}

async function salvar(): Promise<void> {
  if (!podeGerenciar.value || salvando.value) {
    return
  }

  erro.value = null
  sucesso.value = null

  if (!validarFormularioAntesSalvar()) {
    return
  }

  if (avisoDesabilitarMicrosoftPrincipal.value) {
    confirmarDesabilitacao.value = true
    return
  }

  await confirmarSalvar()
}

async function confirmarSalvar(): Promise<void> {
  salvando.value = true
  erro.value = null
  sucesso.value = null

  try {
    const response = await integracoesMicrosoftService.atualizarConfiguracao(criarPayload())
    preencherFormulario(response)
    limparErrosCampo()
    sucesso.value =
      'Configuração salva com sucesso. Em ambientes com variáveis de ambiente, pode ser necessário reiniciar a API.'
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível salvar a integração Microsoft Entra ID.'
  } finally {
    salvando.value = false
    confirmarDesabilitacao.value = false
  }
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Microsoft Entra ID"
      subtitulo="Gerencie a integração corporativa do SGX Sistema de Chamados."
    >
      <template #actions>
        <div class="row q-gutter-sm">
          <q-btn color="primary" label="Atualizar" :loading="loading" @click="carregar" />
          <q-btn
            color="secondary"
            label="Salvar"
            :loading="salvando"
            :disable="!podeGerenciar || loading || salvando"
            @click="salvar"
          />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="!podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Você não possui permissão para visualizar a integração Microsoft Entra ID.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading" mensagem="Carregando configuração da integração Microsoft Entra ID..." />

      <ErrorState
        v-else-if="erro && !statusConfiguracao"
        titulo="Não foi possível carregar a integração Microsoft Entra ID."
        :mensagem="erro"
        @retry="carregar"
      />

      <template v-else>
        <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
          {{ erro }}
        </q-banner>
        <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">
          {{ sucesso }}
        </q-banner>
        <q-banner v-if="alertaSemProvedor" rounded class="bg-orange-1 text-orange-10">
          Ao menos um provedor de autenticação deve permanecer habilitado.
        </q-banner>

        <AppSectionCard titulo="Status da integração" subtitulo="Visibilidade técnica da configuração atual.">
          <div class="row q-col-gutter-md items-center">
            <div class="col-12 col-md-4">
              <q-badge
                :color="statusConfiguracao === 'Configurado' ? 'positive' : statusConfiguracao === 'Desabilitado' ? 'grey-7' : 'warning'"
                text-color="white"
                :label="statusConfiguracao || 'PendenteConfiguracao'"
              />
            </div>
            <div class="col-12 col-md-8">
              <q-list bordered separator>
                <q-item v-if="!pendenciasConfiguracao.length">
                  <q-item-section>Sem pendências de configuração.</q-item-section>
                </q-item>
                <q-item v-for="(pendencia, index) in pendenciasConfiguracao" :key="`pendencia-${index}`">
                  <q-item-section>{{ pendencia }}</q-item-section>
                </q-item>
              </q-list>
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard titulo="Configuração Microsoft Entra ID" subtitulo="Defina modo de autenticação e parâmetros da integração.">
          <q-banner rounded class="bg-blue-1 text-primary q-mb-md">
            Quando a integração Microsoft Entra ID estiver habilitada, os campos Tenant ID, Client ID, Audience,
            Issuer, Authority, API Scope e Redirect URI são obrigatórios.
          </q-banner>

          <q-banner rounded class="bg-grey-2 text-grey-10 q-mb-md">
            {{ avisoModo }}
          </q-banner>

          <q-form class="q-gutter-md" @submit.prevent="salvar">
            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-4">
                <q-toggle
                  v-model="form.habilitado"
                  label="Integração habilitada"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-4">
                <q-toggle
                  v-model="form.loginLocalHabilitado"
                  label="Login local SGX habilitado"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-4">
                <q-select
                  v-model="form.provedorPrincipal"
                  outlined
                  dense
                  emit-value
                  map-options
                  :options="opcoesProvedor"
                  label="Modo de autenticação"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.tenantId"
                  outlined
                  dense
                  :label="rotuloCampo('Tenant ID')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.tenantId)"
                  :error-message="errosCampo.tenantId"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.clientId"
                  outlined
                  dense
                  :label="rotuloCampo('Client ID')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.clientId)"
                  :error-message="errosCampo.clientId"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.audience"
                  outlined
                  dense
                  :label="rotuloCampo('Audience')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.audience)"
                  :error-message="errosCampo.audience"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.issuer"
                  outlined
                  dense
                  :label="rotuloCampo('Issuer')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.issuer)"
                  :error-message="errosCampo.issuer"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.authority"
                  outlined
                  dense
                  :label="rotuloCampo('Authority')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.authority)"
                  :error-message="errosCampo.authority"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.apiScope"
                  outlined
                  dense
                  :label="rotuloCampo('API Scope')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.apiScope)"
                  :error-message="errosCampo.apiScope"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.redirectUri"
                  outlined
                  dense
                  :label="rotuloCampo('Redirect URI')"
                  :readonly="!podeGerenciar"
                  :error="Boolean(errosCampo.redirectUri)"
                  :error-message="errosCampo.redirectUri"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-select
                  v-model="form.perfilPadraoUsuarioMicrosoft"
                  outlined
                  dense
                  emit-value
                  map-options
                  :options="opcoesPerfilPadrao"
                  label="Perfil padrão para usuário Microsoft"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-6">
                <q-toggle
                  v-model="form.criarUsuarioAutomaticamente"
                  label="Criar usuário automaticamente"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="form.dominiosPermitidosTexto"
                  outlined
                  dense
                  type="textarea"
                  autogrow
                  label="Domínios permitidos"
                  hint="Separe por ; ou ,"
                  :readonly="!podeGerenciar"
                />
              </div>
            </div>
          </q-form>
        </AppSectionCard>

        <q-card flat bordered class="sgx-card q-pa-md">
          <div class="text-subtitle1 text-weight-medium">Observações técnicas</div>
          <ul class="q-mt-sm q-pl-md">
            <li>Segredos (client secret) não são exibidos nesta tela.</li>
            <li>SPA não utiliza client secret no frontend.</li>
            <li>Se Microsoft estiver desabilitado, o botão Microsoft não aparece no login.</li>
            <li>Se o ambiente usar appsettings/variáveis, pode ser necessário reiniciar a API após salvar.</li>
          </ul>
        </q-card>
      </template>
    </template>

    <ConfirmDialog
      v-model="confirmarDesabilitacao"
      titulo="Confirmar desabilitação"
      mensagem="Microsoft está como provedor principal. Deseja desabilitar a integração mesmo assim?"
      color="warning"
      confirmar-label="Desabilitar e salvar"
      :loading="salvando"
      @confirm="confirmarSalvar"
    />
  </q-page>
</template>
