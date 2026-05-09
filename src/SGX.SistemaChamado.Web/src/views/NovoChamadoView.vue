<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QForm } from 'quasar'
import { useQuasar } from 'quasar'
import { useRouter } from 'vue-router'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, DepartamentoPortal, PrioridadePortal } from '../types/portal'

const EXTENSOES_PADRAO = ['.pdf', '.png', '.jpg', '.jpeg', '.txt', '.doc', '.docx', '.xls', '.xlsx']

const EXTENSOES_POR_CONTENT_TYPE: Record<string, string[]> = {
  'application/pdf': ['.pdf'],
  'image/png': ['.png'],
  'image/jpeg': ['.jpg', '.jpeg'],
  'text/plain': ['.txt'],
  'application/msword': ['.doc'],
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document': ['.docx'],
  'application/vnd.ms-excel': ['.xls'],
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
}

const router = useRouter()
const $q = useQuasar()
const formRef = ref<QForm | null>(null)

const carregandoContexto = ref(false)
const salvando = ref(false)
const erroContexto = ref<string | null>(null)
const erroSalvar = ref<string | null>(null)
const erroAnexo = ref<string | null>(null)

const departamentos = ref<DepartamentoPortal[]>([])
const categorias = ref<CategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])

const anexosPendentes = ref<File[]>([])
const extensoesPermitidas = ref<string[]>(EXTENSOES_PADRAO)
const tamanhoMaximoAnexoBytes = ref<number | null>(null)

const form = reactive({
  titulo: '',
  descricao: '',
  departamentoId: null as string | null,
  categoriaId: null as string | null,
  prioridadeId: null as string | null,
})

const exibirDepartamento = computed(() => departamentos.value.length > 0)

const opcoesDepartamento = computed(() =>
  departamentos.value.map((item) => ({
    label: `${item.sigla} - ${item.nome}`,
    value: item.id,
  }))
)

const opcoesCategoria = computed(() => {
  if (!form.departamentoId) {
    return categorias.value.map((item) => ({ label: item.nome, value: item.id }))
  }

  return categorias.value
    .filter((item) => item.departamentoId === null || item.departamentoId === form.departamentoId)
    .map((item) => ({ label: item.nome, value: item.id }))
})

const opcoesPrioridade = computed(() => prioridades.value.map((item) => ({ label: item.nome, value: item.id })))

function normalizarExtensoesPorContentType(contentTypes: string[]): string[] {
  const extensoes = new Set<string>()

  for (const contentType of contentTypes) {
    const extensoesDoTipo = EXTENSOES_POR_CONTENT_TYPE[contentType.trim().toLowerCase()] ?? []
    for (const extensao of extensoesDoTipo) {
      extensoes.add(extensao)
    }
  }

  return extensoes.size ? Array.from(extensoes) : EXTENSOES_PADRAO
}

async function carregarContexto(): Promise<void> {
  carregandoContexto.value = true
  erroContexto.value = null

  try {
    const contexto = await portalService.getPortalContexto()
    departamentos.value = contexto.departamentos
    categorias.value = contexto.categorias
    prioridades.value = contexto.prioridades

    const tiposPermitidos = contexto.configuracaoAnexos?.tiposPermitidos ?? []
    extensoesPermitidas.value = normalizarExtensoesPorContentType(tiposPermitidos)
    tamanhoMaximoAnexoBytes.value = contexto.configuracaoAnexos?.tamanhoMaximoBytes ?? null
  } catch (error) {
    erroContexto.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados da abertura.'
  } finally {
    carregandoContexto.value = false
  }
}

function adicionarAnexos(arquivos: File[]): void {
  erroAnexo.value = null
  anexosPendentes.value = [...anexosPendentes.value, ...arquivos]
}

function registrarErroAnexo(message: string): void {
  erroAnexo.value = message
}

function removerAnexo(index: number): void {
  anexosPendentes.value = anexosPendentes.value.filter((_, idx) => idx !== index)
}

async function salvar(): Promise<void> {
  if (salvando.value) {
    return
  }

  erroSalvar.value = null

  const formValido = await formRef.value?.validate()
  if (!formValido) {
    return
  }

  salvando.value = true

  try {
    const chamado = await portalService.criarChamado({
      titulo: form.titulo.trim(),
      descricao: form.descricao.trim(),
      departamentoId: exibirDepartamento.value ? (form.departamentoId ?? undefined) : undefined,
      categoriaId: form.categoriaId!,
      prioridadeId: form.prioridadeId!,
    })

    let anexosComFalha = 0

    for (const arquivo of anexosPendentes.value) {
      try {
        await portalService.anexarArquivo(chamado.id, arquivo)
      } catch {
        anexosComFalha++
      }
    }

    if (anexosComFalha > 0) {
      $q.notify({
        type: 'warning',
        message: 'Chamado aberto com sucesso, mas um ou mais anexos não foram enviados.',
      })
    } else {
      $q.notify({
        type: 'positive',
        message: 'Chamado aberto com sucesso.',
      })
    }

    await router.replace(`/portal/chamados/${chamado.id}`)
  } catch {
    erroSalvar.value = 'Não foi possível abrir o chamado. Verifique os dados e tente novamente.'
  } finally {
    salvando.value = false
  }
}

function cancelar(): void {
  router.push('/portal/chamados')
}

onMounted(carregarContexto)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      titulo="Abrir chamado"
      subtitulo="Informe os dados da solicitação para que a equipe responsável possa realizar o atendimento."
    >
      <template #actions>
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" :disable="salvando" @click="cancelar" />
      </template>
    </PageHeader>

    <ErrorState v-if="erroContexto && !carregandoContexto" :mensagem="erroContexto" @retry="carregarContexto" />

    <LoadingState v-else-if="carregandoContexto" inline mensagem="Carregando contexto de abertura..." />

    <q-form v-else ref="formRef" class="column q-gutter-md" @submit.prevent="salvar">
      <q-banner v-if="erroSalvar" rounded class="bg-negative text-white">
        Não foi possível abrir o chamado. Verifique os dados e tente novamente.
      </q-banner>

      <AppSectionCard titulo="Dados da solicitação" subtitulo="Preencha os campos obrigatórios para abrir o chamado.">
        <div class="column q-gutter-md">
          <q-input
            v-model="form.titulo"
            outlined
            maxlength="180"
            counter
            label="Título *"
            :rules="[(v) => !!String(v ?? '').trim() || 'Título obrigatório']"
          />

          <q-input
            v-model="form.descricao"
            outlined
            type="textarea"
            autogrow
            maxlength="4000"
            counter
            label="Descrição *"
            :rules="[(v) => !!String(v ?? '').trim() || 'Descrição obrigatória']"
          />

          <div class="row q-col-gutter-md">
            <div v-if="exibirDepartamento" class="col-12 col-md-4">
              <q-select
                v-model="form.departamentoId"
                outlined
                clearable
                emit-value
                map-options
                label="Departamento"
                :options="opcoesDepartamento"
              />
            </div>

            <div :class="exibirDepartamento ? 'col-12 col-md-4' : 'col-12 col-md-6'">
              <q-select
                v-model="form.categoriaId"
                outlined
                emit-value
                map-options
                label="Categoria *"
                :options="opcoesCategoria"
                :rules="[(v) => !!v || 'Categoria obrigatória']"
              />
            </div>

            <div :class="exibirDepartamento ? 'col-12 col-md-4' : 'col-12 col-md-6'">
              <q-select
                v-model="form.prioridadeId"
                outlined
                emit-value
                map-options
                label="Prioridade *"
                :options="opcoesPrioridade"
                :rules="[(v) => !!v || 'Prioridade obrigatória']"
              />
            </div>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Anexos" subtitulo="Opcional: adicione arquivos antes de abrir o chamado.">
        <div class="column q-gutter-sm">
          <q-banner v-if="erroAnexo" dense rounded class="bg-amber-2 text-dark">
            {{ erroAnexo }}
          </q-banner>

          <UploadAnexo
            :loading="salvando"
            :extensoes-permitidas="extensoesPermitidas"
            :tamanho-maximo-bytes="tamanhoMaximoAnexoBytes"
            @upload="adicionarAnexos"
            @invalid="registrarErroAnexo"
          />

          <q-banner v-if="!anexosPendentes.length" rounded class="bg-blue-1 text-primary">
            Nenhum anexo selecionado.
          </q-banner>

          <q-list v-else bordered separator>
            <q-item v-for="(anexo, index) in anexosPendentes" :key="`${anexo.name}-${index}`">
              <q-item-section>
                <q-item-label>{{ anexo.name }}</q-item-label>
                <q-item-label caption>{{ (anexo.size / 1024).toFixed(1) }} KB</q-item-label>
              </q-item-section>

              <q-item-section side>
                <q-btn flat dense round icon="delete" color="negative" :disable="salvando" @click="removerAnexo(index)" />
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </AppSectionCard>

      <div class="row justify-end q-gutter-sm">
        <q-btn flat color="primary" label="Cancelar" :disable="salvando" @click="cancelar" />
        <q-btn
          type="submit"
          color="secondary"
          icon="send"
          label="Abrir chamado"
          :loading="salvando"
          :disable="salvando"
        />
      </div>
    </q-form>
  </q-page>
</template>
