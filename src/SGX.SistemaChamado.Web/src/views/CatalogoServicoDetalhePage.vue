<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { catalogoServicosPortalService } from '../services/catalogoServicosPortalService'
import { HttpRequestError } from '../services/httpClient'
import type { PortalCatalogoServicoDetalhe } from '../types/catalogoServicos'

const route = useRoute()
const router = useRouter()
const $q = useQuasar()

const loading = ref(false)
const erro = ref<string | null>(null)
const servico = ref<PortalCatalogoServicoDetalhe | null>(null)

const slug = computed(() => String(route.params.slug ?? '').trim())

function formatarData(data: string | null): string {
  if (!data) {
    return '-'
  }

  return new Date(data).toLocaleString('pt-BR')
}

function extrairMensagemErro(error: unknown): string {
  if (error instanceof HttpRequestError && error.status === 404) {
    return 'Servico indisponivel ou nao encontrado para o seu perfil de acesso.'
  }

  if (error instanceof Error) {
    const mensagem = error.message
    const jsonStart = mensagem.indexOf('{')

    if (jsonStart >= 0) {
      const trechoJson = mensagem.slice(jsonStart)
      try {
        const parsed = JSON.parse(trechoJson) as { mensagem?: string }
        if (parsed?.mensagem) {
          return parsed.mensagem
        }
      } catch {
        return mensagem
      }
    }

    return mensagem
  }

  return 'Nao foi possivel carregar o servico selecionado.'
}

async function carregar(): Promise<void> {
  if (!slug.value) {
    erro.value = 'Slug do servico nao informado.'
    return
  }

  loading.value = true
  erro.value = null

  try {
    servico.value = await catalogoServicosPortalService.obterServicoPorSlug(slug.value)
  } catch (error) {
    servico.value = null
    erro.value = extrairMensagemErro(error)
  } finally {
    loading.value = false
  }
}

function voltar(): void {
  router.push('/portal/catalogo-servicos')
}

async function abrirChamadoPreparado(): Promise<void> {
  if (!servico.value?.permiteAberturaChamado) {
    $q.notify({
      type: 'warning',
      message: 'Este servico esta disponivel apenas para consulta.',
    })
    return
  }

  try {
    const preparado = await catalogoServicosPortalService.prepararAberturaChamado(servico.value.slug)
    await router.push({
      path: '/portal/chamados/novo',
      query: {
        catalogoServicoId: preparado.catalogoServicoId,
        catalogoServicoSlug: preparado.slug,
      },
    })
  } catch {
    $q.notify({
      type: 'negative',
      message: 'Nao foi possivel iniciar a abertura do chamado para este servico.',
    })
  }
}

function abrirArtigoRelacionado(): void {
  if (!servico.value?.artigoBaseConhecimentoSlug) {
    return
  }

  router.push(`/portal/base-conhecimento/${servico.value.artigoBaseConhecimentoSlug}`)
}

onMounted(async () => {
  await carregar()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader titulo="Catalogo de Servicos" subtitulo="Detalhes do servico institucional selecionado.">
      <template #actions>
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="voltar" />
      </template>
    </PageHeader>

    <LoadingState v-if="loading" inline mensagem="Carregando detalhes do servico..." />

    <ErrorState v-else-if="erro" titulo="Nao foi possivel abrir o servico" :mensagem="erro" @retry="carregar" />

    <AppSectionCard
      v-else-if="servico"
      :titulo="servico.nome"
      :subtitulo="servico.departamentoResponsavelNome || 'Departamento nao informado'"
    >
      <div class="text-caption text-grey-7 q-mb-md">Publicado em {{ formatarData(servico.publicadoEm) }}</div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-md-8">
          <q-banner rounded class="bg-blue-1 text-primary">
            {{ servico.descricao || 'Sem descricao complementar para este servico.' }}
          </q-banner>
        </div>

        <div class="col-12 col-md-4">
          <q-list bordered separator class="rounded-borders">
            <q-item>
              <q-item-section>
                <q-item-label caption>Categoria</q-item-label>
                <q-item-label>{{ servico.categoriaNome || 'Sem categoria' }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="servico.subcategoriaNome">
              <q-item-section>
                <q-item-label caption>Subcategoria</q-item-label>
                <q-item-label>{{ servico.subcategoriaNome }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="servico.prioridadePadraoNome">
              <q-item-section>
                <q-item-label caption>Prioridade padrao</q-item-label>
                <q-item-label>{{ servico.prioridadePadraoNome }}</q-item-label>
              </q-item-section>
            </q-item>
            <q-item v-if="servico.slaPadraoNome">
              <q-item-section>
                <q-item-label caption>SLA padrao</q-item-label>
                <q-item-label>{{ servico.slaPadraoNome }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </div>

      <div class="q-mt-md">
        <div class="text-subtitle2 q-mb-sm">Instrucoes ao solicitante</div>
        <div class="text-body2 text-grey-8 instrucoes">
          {{ servico.instrucoesSolicitante || 'Sem instrucoes adicionais para este servico.' }}
        </div>
      </div>

      <div class="row q-gutter-sm q-mt-md">
        <q-badge :color="servico.permiteAberturaChamado ? 'positive' : 'grey-6'" text-color="white">
          {{ servico.permiteAberturaChamado ? 'Permite abertura de chamado' : 'Consulta informativa' }}
        </q-badge>
        <q-badge :color="servico.requerAprovacao ? 'orange-8' : 'blue-grey'" text-color="white">
          {{ servico.requerAprovacao ? 'Requer aprovacao' : 'Sem aprovacao obrigatoria' }}
        </q-badge>
      </div>

      <q-banner v-if="!servico.permiteAberturaChamado" rounded class="bg-grey-2 text-grey-8 q-mt-md">
        Este servico esta disponivel apenas para consulta.
      </q-banner>

      <q-banner
        v-if="servico.artigoBaseConhecimentoTitulo"
        class="bg-grey-2 text-grey-9 q-mt-md"
        rounded
      >
        Artigo relacionado: <strong>{{ servico.artigoBaseConhecimentoTitulo }}</strong>
        <q-btn
          v-if="servico.artigoBaseConhecimentoSlug"
          flat
          color="primary"
          icon="menu_book"
          label="Abrir artigo"
          @click="abrirArtigoRelacionado"
        />
      </q-banner>

      <div class="row justify-end q-gutter-sm q-mt-lg">
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="voltar" />
        <q-btn
          color="secondary"
          icon="add_circle"
          label="Abrir chamado para este servico"
          :disable="!servico.permiteAberturaChamado"
          @click="abrirChamadoPreparado"
        />
      </div>
    </AppSectionCard>
  </q-page>
</template>

<style scoped>
.instrucoes {
  white-space: pre-wrap;
  line-height: 1.6;
}
</style>
