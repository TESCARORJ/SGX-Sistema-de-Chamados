<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import { useRoute, useRouter } from 'vue-router'
import FormCadastro from '../components/admin/cadastros/FormCadastro.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { permissoes } from '../constants/permissoes'
import { baseConhecimentoAdminService } from '../services/baseConhecimentoAdminService'
import { cadastrosAdminService } from '../services/cadastrosAdminService'
import { useAuthStore } from '../stores/authStore'
import type { CategoriaChamadoResumoResponse } from '../types/adminCadastros'
import {
  StatusArtigoConhecimento,
  VisibilidadeArtigoConhecimento,
  type BaseConhecimentoArtigoDetalhe,
  type CriarBaseConhecimentoArtigoRequest,
} from '../types/baseConhecimento'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const $q = useQuasar()

const loading = ref(false)
const carregamentoConcluido = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const categorias = ref<CategoriaChamadoResumoResponse[]>([])
const artigoAtual = ref<BaseConhecimentoArtigoDetalhe | null>(null)

const form = reactive({
  titulo: '',
  resumo: '',
  conteudo: '',
  tags: '',
  visibilidade: VisibilidadeArtigoConhecimento.Solicitante,
  categoriaId: null as string | null,
  statusDescricao: '',
  slug: '',
})

const idParam = computed(() => String(route.params.id ?? 'novo'))
const isNovo = computed(() => idParam.value === 'novo')
const usuarioEhAdministrador = computed(() => (authStore.usuario?.perfis ?? []).includes('Administrador'))
const fallbackAdminSemPermissoes = computed(
  () => usuarioEhAdministrador.value && (authStore.usuario?.permissoes?.length ?? 0) === 0
)

function possuiPermissao(codigo: string): boolean {
  return fallbackAdminSemPermissoes.value || authStore.possuiPermissao(codigo)
}

const podeVisualizar = computed(() => possuiPermissao(permissoes.baseConhecimentoVisualizar))
const podeGerenciar = computed(() => possuiPermissao(permissoes.baseConhecimentoGerenciar))
const somenteLeitura = computed(() => !podeGerenciar.value)

const opcoesVisibilidade = [
  { label: 'Solicitante', value: VisibilidadeArtigoConhecimento.Solicitante },
  { label: 'Atendente', value: VisibilidadeArtigoConhecimento.Atendente },
  { label: 'Administrador', value: VisibilidadeArtigoConhecimento.Administrador },
]

function regraObrigatoria(valor: unknown): true | string {
  return String(valor ?? '').trim().length > 0 ? true : 'Campo obrigatorio.'
}

function extrairMensagemErro(error: unknown, fallback: string): string {
  if (!(error instanceof Error)) {
    return fallback
  }

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

function aplicarDetalheNoFormulario(artigo: BaseConhecimentoArtigoDetalhe): void {
  artigoAtual.value = artigo
  form.titulo = artigo.titulo
  form.resumo = artigo.resumo ?? ''
  form.conteudo = artigo.conteudo
  form.tags = artigo.tags ?? ''
  form.visibilidade = artigo.visibilidade
  form.categoriaId = artigo.categoriaId
  form.statusDescricao = artigo.statusDescricao
  form.slug = artigo.slug
}

function limparFormulario(): void {
  artigoAtual.value = null
  form.titulo = ''
  form.resumo = ''
  form.conteudo = ''
  form.tags = ''
  form.visibilidade = VisibilidadeArtigoConhecimento.Solicitante
  form.categoriaId = null
  form.statusDescricao = ''
  form.slug = ''
}

async function carregarCategorias(): Promise<void> {
  const response = await cadastrosAdminService.listarCategorias({ ativo: true, tamanhoPagina: 100 })
  categorias.value = response.items
}

async function carregarDetalhe(): Promise<void> {
  if (isNovo.value) {
    limparFormulario()
    return
  }

  const detalhe = await baseConhecimentoAdminService.obterArtigo(idParam.value)
  aplicarDetalheNoFormulario(detalhe)
}

async function carregarTela(): Promise<void> {
  if ((!isNovo.value && !podeVisualizar.value) || (isNovo.value && !podeGerenciar.value)) {
    carregamentoConcluido.value = true
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    await carregarCategorias()
    await carregarDetalhe()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar os dados do artigo.')
  } finally {
    loading.value = false
    carregamentoConcluido.value = true
  }
}

function montarPayload(): CriarBaseConhecimentoArtigoRequest {
  return {
    titulo: form.titulo.trim(),
    resumo: form.resumo.trim() || null,
    conteudo: form.conteudo.trim(),
    visibilidade: form.visibilidade,
    categoriaId: form.categoriaId,
    tags: form.tags.trim() || null,
  }
}

async function salvar(): Promise<void> {
  if (somenteLeitura.value) {
    return
  }

  loading.value = true
  erro.value = null
  sucesso.value = null

  try {
    if (isNovo.value) {
      const criado = await baseConhecimentoAdminService.criarArtigo(montarPayload())
      $q.notify({ type: 'positive', message: 'Artigo criado com sucesso.' })
      await router.replace(`/admin/conhecimento/base-conhecimento/${criado.id}`)
      return
    }

    const atualizado = await baseConhecimentoAdminService.atualizarArtigo(idParam.value, montarPayload())
    aplicarDetalheNoFormulario(atualizado)
    sucesso.value = 'Artigo salvo com sucesso.'
    $q.notify({ type: 'positive', message: 'Artigo atualizado com sucesso.' })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar o artigo.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    loading.value = false
  }
}

function voltar(): void {
  router.push('/admin/conhecimento/base-conhecimento')
}

watch(
  () => route.params.id,
  async () => {
    carregamentoConcluido.value = false
    await carregarTela()
  }
)

onMounted(async () => {
  await carregarTela()
})
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="isNovo ? 'Base de conhecimento - Novo artigo' : 'Base de conhecimento - Detalhe do artigo'"
      subtitulo="Preencha os dados obrigatorios para manter o conhecimento atualizado e rastreavel."
    />

    <q-banner v-if="isNovo && !podeGerenciar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para criar artigos da base de conhecimento.
    </q-banner>

    <q-banner v-else-if="!isNovo && !podeVisualizar" rounded class="bg-orange-1 text-orange-10">
      Voce nao possui permissao para visualizar este artigo.
    </q-banner>

    <template v-else>
      <LoadingState v-if="loading && !carregamentoConcluido" mensagem="Carregando dados do artigo..." />

      <ErrorState
        v-else-if="erro && !carregamentoConcluido"
        titulo="Nao foi possivel carregar o artigo"
        :mensagem="erro"
        @retry="carregarTela"
      />

      <template v-else>
        <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
          {{ erro }}
        </q-banner>

        <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">
          {{ sucesso }}
        </q-banner>

        <FormCadastro
          :titulo="isNovo ? 'Novo artigo' : 'Detalhe do artigo'"
          :loading="loading"
          :somente-leitura="somenteLeitura"
          botao-salvar-label="Salvar artigo"
          @salvar="salvar"
          @cancelar="voltar"
        >
          <div class="column q-gutter-md">
            <AppSectionCard titulo="Conteudo do artigo" subtitulo="Defina titulo, resumo e texto principal para publicacao.">
              <div class="row q-col-gutter-md">
                <div class="col-12">
                  <q-input v-model="form.titulo" outlined dense label="Titulo" :readonly="somenteLeitura" :rules="[regraObrigatoria]" />
                </div>

                <div class="col-12">
                  <q-input v-model="form.resumo" outlined dense type="textarea" autogrow label="Resumo" :readonly="somenteLeitura" />
                </div>

                <div class="col-12">
                  <q-input
                    v-model="form.conteudo"
                    outlined
                    dense
                    type="textarea"
                    autogrow
                    label="Conteudo"
                    :readonly="somenteLeitura"
                    :rules="[regraObrigatoria]"
                  />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard titulo="Classificacao e publicacao" subtitulo="Controle visibilidade, categoria e metadados de ciclo de vida.">
              <div class="row q-col-gutter-md">
                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.visibilidade"
                    outlined
                    dense
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="opcoesVisibilidade"
                    label="Visibilidade"
                    :rules="[regraObrigatoria]"
                  />
                </div>

                <div class="col-12 col-md-4">
                  <q-select
                    v-model="form.categoriaId"
                    outlined
                    dense
                    clearable
                    emit-value
                    map-options
                    :disable="somenteLeitura"
                    :options="categorias.map((item) => ({ label: item.nome, value: item.id }))"
                    label="Categoria"
                  />
                </div>

                <div v-if="!isNovo" class="col-12 col-md-2">
                  <q-input v-model="form.statusDescricao" outlined dense readonly label="Status" />
                </div>

                <div v-if="!isNovo" class="col-12 col-md-2">
                  <q-input v-model="form.slug" outlined dense readonly label="Slug" />
                </div>
              </div>
            </AppSectionCard>

            <AppSectionCard titulo="Taxonomia" subtitulo="Utilize tags para facilitar busca e reutilizacao do conhecimento.">
              <q-input
                v-model="form.tags"
                outlined
                dense
                label="Tags"
                hint="Separe por virgula. Exemplo: senha, vpn, acesso remoto"
                :readonly="somenteLeitura"
              />
            </AppSectionCard>
          </div>
        </FormCadastro>
      </template>
    </template>
  </q-page>
</template>
