<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { metodosLoginAdminService } from '../services/metodosLoginAdminService'
import { useAuthStore } from '../stores/authStore'
import type {
  AtualizarMetodosLoginAdminRequest,
  MetodoLoginAdminDto,
} from '../types/metodosLogin'

const $q = useQuasar()
const authStore = useAuthStore()

const loading = ref(false)
const salvando = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const metodos = ref<MetodoLoginAdminDto[]>([])

const codigosAutoProvisionamento = ['MicrosoftEntraId', 'ActiveDirectory']

const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

const podeVisualizar = computed(
  () =>
    fallbackAdminSemPermissoes.value ||
    authStore.possuiPermissao(permissoes.autenticacaoProvedoresVisualizar) ||
    authStore.possuiPermissao(permissoes.autenticacaoProvedoresGerenciar)
)

const podeGerenciar = computed(
  () => fallbackAdminSemPermissoes.value || authStore.possuiPermissao(permissoes.autenticacaoProvedoresGerenciar)
)

const provedoresOrdenados = computed(() =>
  [...metodos.value].sort((a, b) => a.ordem - b.ordem || a.codigo.localeCompare(b.codigo))
)

function marcarPrincipal(codigo: string): void {
  metodos.value = metodos.value.map((provedor) => ({
    ...provedor,
    principal: provedor.codigo === codigo,
  }))
}

function provedorPermiteAutoProvisionamento(codigo: string): boolean {
  return codigosAutoProvisionamento.includes(codigo)
}

function estaFuncionalNoEstadoAtual(provedor: MetodoLoginAdminDto): boolean {
  return provedor.habilitado && provedor.podeHabilitar
}

function existeAlternativaViavel(codigoIgnorado: string): boolean {
  return metodos.value.some(
    (provedor) => provedor.codigo !== codigoIgnorado && estaFuncionalNoEstadoAtual(provedor)
  )
}

function motivoBloqueioHabilitar(provedor: MetodoLoginAdminDto): string | null {
  if (provedor.habilitado) {
    return null
  }

  if (provedor.podeHabilitar) {
    return null
  }

  return provedor.motivoBloqueioHabilitar || 'Provedor indisponivel para habilitacao neste ambiente.'
}

function motivoBloqueioDesabilitar(provedor: MetodoLoginAdminDto): string | null {
  if (!provedor.habilitado) {
    return null
  }

  if (provedor.codigo === 'LocalSgx' && !existeAlternativaViavel(provedor.codigo)) {
    return 'Nao e permitido desabilitar Local SGX sem alternativa administrativa viavel.'
  }

  if (!existeAlternativaViavel(provedor.codigo)) {
    return 'Ao menos um metodo de login viavel deve permanecer habilitado.'
  }

  return provedor.motivoBloqueioDesabilitar || null
}

function motivoBloqueioAlternancia(provedor: MetodoLoginAdminDto): string | null {
  return provedor.habilitado ? motivoBloqueioDesabilitar(provedor) : motivoBloqueioHabilitar(provedor)
}

function podeAlternarHabilitacao(provedor: MetodoLoginAdminDto): boolean {
  return !motivoBloqueioAlternancia(provedor)
}

function statusOperacional(provedor: MetodoLoginAdminDto): { label: string; color: string } {
  if (provedor.habilitado && provedor.funcional) {
    return { label: 'Ativo e viavel', color: 'positive' }
  }

  if (provedor.habilitado && !provedor.funcional) {
    return { label: 'Habilitado sem viabilidade', color: 'warning' }
  }

  return { label: 'Desabilitado', color: 'grey-7' }
}

function normalizarPayload(): AtualizarMetodosLoginAdminRequest {
  return {
    provedores: metodos.value.map((provedor) => ({
      codigo: provedor.codigo,
      habilitado: provedor.habilitado,
      principal: provedor.principal,
      ordem: Number(provedor.ordem) || 0,
      permiteAutoProvisionamento: provedor.permiteAutoProvisionamento,
      perfilPadraoAutoProvisionamento: provedor.perfilPadraoAutoProvisionamento || 'Solicitante',
      rotuloExibicao: (provedor.rotuloExibicao || provedor.nome).trim(),
    })),
  }
}

async function carregar(): Promise<void> {
  if (!podeVisualizar.value) {
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    const response = await metodosLoginAdminService.obterConfiguracao()
    metodos.value = [...(response.provedores ?? [])]
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel carregar os metodos de login.'
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
  salvando.value = true

  try {
    const payload = normalizarPayload()
    const response = await metodosLoginAdminService.atualizarConfiguracao(payload)
    metodos.value = [...(response.provedores ?? [])]
    sucesso.value = 'Metodos de login atualizados com sucesso.'
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Nao foi possivel salvar os metodos de login.'
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

onMounted(() => {
  void carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Metodos de Login"
      subtitulo="Configure quais autenticacoes ficam disponiveis para os usuarios."
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
      Voce nao possui permissao para visualizar os metodos de login.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading" mensagem="Carregando metodos de login..." />

      <ErrorState
        v-else-if="erro && !metodos.length"
        titulo="Nao foi possivel carregar os metodos de login."
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
          titulo="Avisos de seguranca"
          subtitulo="Recomendacoes para manter contingencia administrativa e autenticacao corporativa segura."
        >
          <ul class="q-my-none q-pl-md">
            <li>Prefira LDAPS para Active Directory em homologacao e producao.</li>
            <li>Mantenha Local SGX como contingencia administrativa quando possivel.</li>
            <li>Auto provisionamento nao deve atribuir perfil Administrador automaticamente.</li>
          </ul>
        </AppSectionCard>

        <AppSectionCard
          titulo="Provedores configuraveis"
          subtitulo="Defina habilitacao, provedor principal, ordem e auto provisionamento por metodo."
        >
          <q-table
            flat
            bordered
            :rows="provedoresOrdenados"
            :columns="[
              { name: 'nome', label: 'Provedor', field: 'nome', align: 'left' },
              { name: 'habilitado', label: 'Habilitado', field: 'habilitado', align: 'center' },
              { name: 'principal', label: 'Principal', field: 'principal', align: 'center' },
              { name: 'ordem', label: 'Ordem', field: 'ordem', align: 'center' },
              { name: 'auto', label: 'Auto provisionamento', field: 'permiteAutoProvisionamento', align: 'center' },
              { name: 'perfil', label: 'Perfil padrao', field: 'perfilPadraoAutoProvisionamento', align: 'center' },
              { name: 'rotulo', label: 'Rotulo no login', field: 'rotuloExibicao', align: 'left' },
            ]"
            row-key="codigo"
            :pagination="{ rowsPerPage: 0 }"
            hide-bottom
          >
            <template #body-cell-nome="props">
              <q-td :props="props">
                <div class="text-weight-medium">{{ props.row.nome }}</div>
                <div class="text-caption text-grey-7">{{ props.row.codigo }}</div>
                <div class="text-caption text-grey-7">{{ props.row.descricao }}</div>
                <q-chip dense square :color="statusOperacional(props.row).color" text-color="white" class="q-mt-xs">
                  {{ statusOperacional(props.row).label }}
                </q-chip>
              </q-td>
            </template>

            <template #body-cell-habilitado="props">
              <q-td :props="props" class="text-center">
                <q-toggle
                  v-model="props.row.habilitado"
                  dense
                  :disable="!podeGerenciar || !podeAlternarHabilitacao(props.row)"
                />
                <div v-if="motivoBloqueioAlternancia(props.row)" class="text-caption text-negative q-mt-xs">
                  {{ motivoBloqueioAlternancia(props.row) }}
                </div>
              </q-td>
            </template>

            <template #body-cell-principal="props">
              <q-td :props="props" class="text-center">
                <q-radio
                  :model-value="metodos.find((x) => x.principal)?.codigo"
                  :val="props.row.codigo"
                  :disable="!podeGerenciar || !props.row.habilitado || !props.row.podeHabilitar"
                  @update:model-value="marcarPrincipal(String($event))"
                />
              </q-td>
            </template>

            <template #body-cell-ordem="props">
              <q-td :props="props" class="text-center">
                <q-input
                  v-model.number="props.row.ordem"
                  type="number"
                  dense
                  outlined
                  min="1"
                  class="ordem-input"
                  :readonly="!podeGerenciar"
                />
              </q-td>
            </template>

            <template #body-cell-auto="props">
              <q-td :props="props" class="text-center">
                <q-toggle
                  v-model="props.row.permiteAutoProvisionamento"
                  dense
                  :disable="!podeGerenciar || !provedorPermiteAutoProvisionamento(props.row.codigo)"
                />
              </q-td>
            </template>

            <template #body-cell-perfil="props">
              <q-td :props="props" class="text-center">
                <q-select
                  v-model="props.row.perfilPadraoAutoProvisionamento"
                  :options="['Solicitante', 'Atendente']"
                  dense
                  outlined
                  class="perfil-select"
                  :disable="!podeGerenciar || !provedorPermiteAutoProvisionamento(props.row.codigo)"
                />
              </q-td>
            </template>

            <template #body-cell-rotulo="props">
              <q-td :props="props">
                <q-input
                  v-model="props.row.rotuloExibicao"
                  dense
                  outlined
                  :readonly="!podeGerenciar"
                />
              </q-td>
            </template>
          </q-table>
        </AppSectionCard>
      </template>
    </template>
  </q-page>
</template>

<style scoped>
.ordem-input {
  width: 84px;
  margin: 0 auto;
}

.perfil-select {
  min-width: 150px;
}
</style>
