<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ConfirmDialog from '../components/ui/ConfirmDialog.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { integracoesActiveDirectoryService } from '../services/integracoesActiveDirectoryService'
import { useAuthStore } from '../stores/authStore'
import type {
  ActiveDirectoryIntegracaoResponse,
  AtualizarActiveDirectoryIntegracaoRequest,
  TestarAutenticacaoActiveDirectoryRequest,
  TestarAutenticacaoActiveDirectoryResponse,
  TestarConexaoActiveDirectoryRequest,
  TestarConexaoActiveDirectoryResponse,
} from '../types/integracoesActiveDirectory'

const authStore = useAuthStore()

const loading = ref(false)
const salvando = ref(false)
const testandoConexao = ref(false)
const testandoAutenticacao = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const confirmarLdapSemTls = ref(false)
const callbackConfirmacao = ref<(() => Promise<void>) | null>(null)
const resultadoTesteConexao = ref<TestarConexaoActiveDirectoryResponse | null>(null)
const resultadoTesteAutenticacao = ref<TestarAutenticacaoActiveDirectoryResponse | null>(null)

const form = reactive({
  ativo: true,
  servidor: '',
  porta: 636,
  usarLdaps: true,
  permitirLdapSemTls: false,
  dominio: '',
  baseDn: '',
  userSearchFilter: '(&(objectClass=user)(sAMAccountName={0}))',
  permitirAutoProvisionamento: false,
  perfilPadrao: 'Solicitante',
  timeoutConexaoSegundos: 10,
})

const testeAutenticacao = reactive({
  usuario: '',
  senha: '',
  dominio: '',
})

const statusConfiguracao = ref('NaoConfigurado')
const pendenciasConfiguracao = ref<string[]>([])
const avisosSeguranca = ref<string[]>([])
const tecnicamenteConfigurado = ref(false)

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
    authStore.possuiPermissao(permissoes.integracoesActiveDirectoryVisualizar) ||
    authStore.possuiPermissao(permissoes.integracoesActiveDirectoryGerenciar)
)

const podeGerenciar = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.integracoesActiveDirectoryGerenciar)
)

const exigeConfirmacaoLdapSemTls = computed(() => !form.usarLdaps && form.permitirLdapSemTls)

function preencherFormulario(config: ActiveDirectoryIntegracaoResponse): void {
  form.ativo = config.ativo
  form.servidor = config.servidor ?? ''
  form.porta = config.porta ?? 636
  form.usarLdaps = config.usarLdaps
  form.permitirLdapSemTls = config.permitirLdapSemTls
  form.dominio = config.dominio ?? ''
  form.baseDn = config.baseDn ?? ''
  form.userSearchFilter = config.userSearchFilter || '(&(objectClass=user)(sAMAccountName={0}))'
  form.permitirAutoProvisionamento = config.permitirAutoProvisionamento
  form.perfilPadrao = config.perfilPadrao || 'Solicitante'
  form.timeoutConexaoSegundos = config.timeoutConexaoSegundos || 10
  statusConfiguracao.value = config.statusConfiguracao || 'NaoConfigurado'
  pendenciasConfiguracao.value = [...(config.pendenciasConfiguracao ?? [])]
  avisosSeguranca.value = [...(config.avisosSeguranca ?? [])]
  tecnicamenteConfigurado.value = Boolean(config.tecnicamenteConfigurado)
}

function criarPayload(confirmacaoPermitirLdapSemTls: boolean): AtualizarActiveDirectoryIntegracaoRequest {
  return {
    ativo: form.ativo,
    servidor: form.servidor.trim(),
    porta: Number(form.porta) || 0,
    usarLdaps: form.usarLdaps,
    permitirLdapSemTls: form.permitirLdapSemTls,
    confirmacaoPermitirLdapSemTls,
    dominio: form.dominio.trim(),
    baseDn: form.baseDn.trim(),
    userSearchFilter: form.userSearchFilter.trim(),
    permitirAutoProvisionamento: form.permitirAutoProvisionamento,
    perfilPadrao: form.perfilPadrao || 'Solicitante',
    timeoutConexaoSegundos: Number(form.timeoutConexaoSegundos) || 0,
  }
}

function criarPayloadTesteConexao(confirmacaoPermitirLdapSemTls: boolean): TestarConexaoActiveDirectoryRequest {
  return {
    ...criarPayload(confirmacaoPermitirLdapSemTls),
  }
}

function criarPayloadTesteAutenticacao(confirmacaoPermitirLdapSemTls: boolean): TestarAutenticacaoActiveDirectoryRequest {
  return {
    usuario: testeAutenticacao.usuario.trim(),
    senha: testeAutenticacao.senha,
    dominio: testeAutenticacao.dominio.trim(),
    ativo: form.ativo,
    servidor: form.servidor.trim(),
    porta: Number(form.porta) || 0,
    usarLdaps: form.usarLdaps,
    permitirLdapSemTls: form.permitirLdapSemTls,
    confirmacaoPermitirLdapSemTls,
    baseDn: form.baseDn.trim(),
    userSearchFilter: form.userSearchFilter.trim(),
    timeoutConexaoSegundos: Number(form.timeoutConexaoSegundos) || 0,
  }
}

function abrirConfirmacao(action: () => Promise<void>): void {
  callbackConfirmacao.value = action
  confirmarLdapSemTls.value = true
}

async function executarComConfirmacao(action: (confirmacaoPermitirLdapSemTls: boolean) => Promise<void>): Promise<void> {
  if (exigeConfirmacaoLdapSemTls.value) {
    abrirConfirmacao(() => action(true))
    return
  }

  await action(false)
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null
  resultadoTesteConexao.value = null
  resultadoTesteAutenticacao.value = null

  try {
    const response = await integracoesActiveDirectoryService.obterConfiguracao()
    preencherFormulario(response)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar a configuracao Active Directory.'
  } finally {
    loading.value = false
  }
}

async function salvar(): Promise<void> {
  if (!podeGerenciar.value || salvando.value) {
    return
  }

  await executarComConfirmacao(async (confirmacaoPermitirLdapSemTls) => {
    salvando.value = true
    erro.value = null
    sucesso.value = null

    try {
      const response = await integracoesActiveDirectoryService.atualizarConfiguracao(
        criarPayload(confirmacaoPermitirLdapSemTls)
      )
      preencherFormulario(response)
      sucesso.value = 'Configuracao Active Directory atualizada com sucesso.'
    } catch (error) {
      erro.value = error instanceof Error ? error.message : 'Nao foi possivel salvar a configuracao Active Directory.'
    } finally {
      salvando.value = false
    }
  })
}

async function testarConexao(): Promise<void> {
  if (!podeGerenciar.value || testandoConexao.value) {
    return
  }

  await executarComConfirmacao(async (confirmacaoPermitirLdapSemTls) => {
    testandoConexao.value = true
    erro.value = null
    resultadoTesteConexao.value = null

    try {
      const response = await integracoesActiveDirectoryService.testarConexao(
        criarPayloadTesteConexao(confirmacaoPermitirLdapSemTls)
      )
      resultadoTesteConexao.value = response
    } catch (error) {
      erro.value = error instanceof Error ? error.message : 'Nao foi possivel testar conexao LDAP/LDAPS.'
    } finally {
      testandoConexao.value = false
    }
  })
}

async function testarAutenticacao(): Promise<void> {
  if (!podeGerenciar.value || testandoAutenticacao.value) {
    return
  }

  await executarComConfirmacao(async (confirmacaoPermitirLdapSemTls) => {
    testandoAutenticacao.value = true
    erro.value = null
    resultadoTesteAutenticacao.value = null

    try {
      const response = await integracoesActiveDirectoryService.testarAutenticacao(
        criarPayloadTesteAutenticacao(confirmacaoPermitirLdapSemTls)
      )
      resultadoTesteAutenticacao.value = response
    } catch (error) {
      erro.value = error instanceof Error ? error.message : 'Nao foi possivel testar autenticacao controlada.'
    } finally {
      testandoAutenticacao.value = false
      testeAutenticacao.senha = ''
    }
  })
}

async function confirmarAcaoLdapSemTls(): Promise<void> {
  const callback = callbackConfirmacao.value
  callbackConfirmacao.value = null
  confirmarLdapSemTls.value = false

  if (callback) {
    await callback()
  }
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Active Directory / LDAP"
      subtitulo="Configure parametros tecnicos do provedor Active Directory."
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
      Voce nao possui permissao para visualizar a integracao Active Directory / LDAP.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading" mensagem="Carregando configuracao Active Directory / LDAP..." />

      <ErrorState
        v-else-if="erro && !statusConfiguracao"
        titulo="Nao foi possivel carregar configuracao Active Directory / LDAP."
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

        <AppSectionCard
          titulo="Status tecnico"
          subtitulo="Visao consolidada da configuracao tecnica do Active Directory."
        >
          <div class="row q-col-gutter-md items-start">
            <div class="col-12 col-md-4">
              <q-badge
                :color="tecnicamenteConfigurado ? 'positive' : statusConfiguracao === 'Inativo' ? 'grey-7' : 'warning'"
                text-color="white"
                :label="statusConfiguracao"
              />
            </div>
            <div class="col-12 col-md-8">
              <q-list bordered separator>
                <q-item v-if="!pendenciasConfiguracao.length">
                  <q-item-section>Sem pendencias tecnicas.</q-item-section>
                </q-item>
                <q-item v-for="(pendencia, index) in pendenciasConfiguracao" :key="`pendencia-${index}`">
                  <q-item-section>{{ pendencia }}</q-item-section>
                </q-item>
              </q-list>
            </div>
          </div>
        </AppSectionCard>

        <AppSectionCard
          titulo="Configuracao tecnica"
          subtitulo="Defina servidor, dominio, Base DN e regras de seguranca LDAP/LDAPS."
        >
          <q-banner rounded class="bg-blue-1 text-primary q-mb-md">
            Recomenda-se LDAPS em homologacao e producao.
          </q-banner>

          <q-banner v-if="avisosSeguranca.length" rounded class="bg-orange-1 text-orange-10 q-mb-md">
            <div class="text-weight-medium q-mb-xs">Avisos de seguranca</div>
            <ul class="q-my-none q-pl-md">
              <li v-for="(aviso, idx) in avisosSeguranca" :key="`aviso-${idx}`">{{ aviso }}</li>
            </ul>
          </q-banner>

          <q-form class="q-gutter-md" @submit.prevent="salvar">
            <div class="row q-col-gutter-md">
              <div class="col-12 col-md-3">
                <q-toggle
                  v-model="form.ativo"
                  label="Ativo"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-3">
                <q-toggle
                  v-model="form.usarLdaps"
                  label="Usar LDAPS"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-3">
                <q-toggle
                  v-model="form.permitirLdapSemTls"
                  label="Permitir LDAP sem TLS"
                  :disable="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-3">
                <q-toggle
                  v-model="form.permitirAutoProvisionamento"
                  label="Permitir auto provisionamento"
                  :disable="!podeGerenciar"
                />
              </div>

              <div class="col-12 col-md-6">
                <q-input
                  v-model="form.servidor"
                  outlined
                  dense
                  label="Servidor LDAP/LDAPS *"
                  hint="Ex.: ldaps://dc01.empresa.local"
                  :readonly="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-2">
                <q-input
                  v-model.number="form.porta"
                  outlined
                  dense
                  type="number"
                  label="Porta *"
                  min="1"
                  max="65535"
                  :readonly="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-4">
                <q-input
                  v-model.number="form.timeoutConexaoSegundos"
                  outlined
                  dense
                  type="number"
                  label="Timeout (segundos) *"
                  min="1"
                  max="120"
                  :readonly="!podeGerenciar"
                />
              </div>

              <div class="col-12 col-md-4">
                <q-input
                  v-model="form.dominio"
                  outlined
                  dense
                  label="Dominio"
                  :readonly="!podeGerenciar"
                />
              </div>
              <div class="col-12 col-md-8">
                <q-input
                  v-model="form.baseDn"
                  outlined
                  dense
                  label="Base DN *"
                  :readonly="!podeGerenciar"
                />
              </div>
              <div class="col-12">
                <q-input
                  v-model="form.userSearchFilter"
                  outlined
                  dense
                  label="Filtro de busca do usuario *"
                  hint="Obrigatorio conter o placeholder {0}"
                  :readonly="!podeGerenciar"
                />
              </div>

              <div class="col-12 col-md-6">
                <q-select
                  v-model="form.perfilPadrao"
                  outlined
                  dense
                  emit-value
                  map-options
                  :options="opcoesPerfilPadrao"
                  label="Perfil padrao auto provisionamento"
                  :disable="!podeGerenciar"
                />
              </div>
            </div>
          </q-form>
        </AppSectionCard>

        <AppSectionCard
          titulo="Testes operacionais"
          subtitulo="Validacao tecnica sem persistir senha de teste."
        >
          <div class="row q-col-gutter-md">
            <div class="col-12 col-md-6">
              <q-btn
                color="primary"
                label="Testar conexao"
                icon="cable"
                :loading="testandoConexao"
                :disable="!podeGerenciar || testandoConexao"
                @click="testarConexao"
              />
              <q-banner v-if="resultadoTesteConexao" rounded class="q-mt-sm" :class="resultadoTesteConexao.sucesso ? 'bg-green-1 text-positive' : 'bg-red-1 text-negative'">
                {{ resultadoTesteConexao.mensagem }} ({{ resultadoTesteConexao.duracaoMs }} ms)
              </q-banner>
            </div>

            <div class="col-12 col-md-6 q-gutter-sm column">
              <q-input
                v-model="testeAutenticacao.usuario"
                outlined
                dense
                label="Usuario para teste controlado"
                :readonly="!podeGerenciar"
              />
              <q-input
                v-model="testeAutenticacao.senha"
                outlined
                dense
                type="password"
                label="Senha de teste (nao persistida)"
                :readonly="!podeGerenciar"
              />
              <q-input
                v-model="testeAutenticacao.dominio"
                outlined
                dense
                label="Dominio para teste (opcional)"
                :readonly="!podeGerenciar"
              />
              <q-btn
                color="secondary"
                label="Testar autenticacao controlada"
                icon="verified_user"
                :loading="testandoAutenticacao"
                :disable="!podeGerenciar || testandoAutenticacao"
                @click="testarAutenticacao"
              />
              <q-banner v-if="resultadoTesteAutenticacao" rounded :class="resultadoTesteAutenticacao.sucesso ? 'bg-green-1 text-positive' : 'bg-red-1 text-negative'">
                <div>{{ resultadoTesteAutenticacao.mensagem }} ({{ resultadoTesteAutenticacao.duracaoMs }} ms)</div>
                <div v-if="resultadoTesteAutenticacao.sucesso" class="text-caption q-mt-xs">
                  Usuario: {{ resultadoTesteAutenticacao.usuarioSamAccountName || '-' }} |
                  Nome: {{ resultadoTesteAutenticacao.nomeCompleto || '-' }} |
                  Email: {{ resultadoTesteAutenticacao.email || '-' }}
                </div>
              </q-banner>
            </div>
          </div>
        </AppSectionCard>
      </template>
    </template>

    <ConfirmDialog
      v-model="confirmarLdapSemTls"
      titulo="Confirmar LDAP sem TLS"
      mensagem="LDAP sem TLS aumenta risco de seguranca. Confirma habilitar essa opcao?"
      color="warning"
      confirmar-label="Confirmar e continuar"
      @confirm="confirmarAcaoLdapSemTls"
    />
  </q-page>
</template>
