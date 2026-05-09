<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import type { QForm } from 'quasar'
import { useRouter } from 'vue-router'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import AppSectionCard from '../components/ui/AppSectionCard.vue'
import ErrorState from '../components/ui/ErrorState.vue'
import LoadingState from '../components/ui/LoadingState.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, DepartamentoPortal, PrioridadePortal } from '../types/portal'

const router = useRouter()
const formRef = ref<QForm | null>(null)

const carregandoContexto = ref(false)
const salvando = ref(false)
const erro = ref<string | null>(null)

const departamentos = ref<DepartamentoPortal[]>([])
const categorias = ref<CategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])

const anexosPendentes = ref<File[]>([])

const form = reactive({
  titulo: '',
  descricao: '',
  departamentoId: '',
  categoriaId: '',
  prioridadeId: '',
})

const opcoesDepartamento = computed(() =>
  departamentos.value.map((item) => ({
    label: `${item.sigla} - ${item.nome}`,
    value: item.id,
  }))
)

const opcoesCategoria = computed(() => categorias.value.map((item) => ({ label: item.nome, value: item.id })))
const opcoesPrioridade = computed(() => prioridades.value.map((item) => ({ label: item.nome, value: item.id })))

async function carregarContexto(): Promise<void> {
  carregandoContexto.value = true
  erro.value = null

  try {
    const contexto = await portalService.obterPortalContexto()
    departamentos.value = contexto.departamentos
    categorias.value = contexto.categorias
    prioridades.value = contexto.prioridades
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível carregar os dados.'
  } finally {
    carregandoContexto.value = false
  }
}

function adicionarAnexos(arquivos: File[]): void {
  anexosPendentes.value = [...anexosPendentes.value, ...arquivos]
}

function removerAnexo(indice: number): void {
  anexosPendentes.value = anexosPendentes.value.filter((_, idx) => idx !== indice)
}

async function salvar(): Promise<void> {
  erro.value = null

  const formValido = await formRef.value?.validate()
  if (!formValido) {
    return
  }

  salvando.value = true

  try {
    const chamado = await portalService.abrirChamado({
      titulo: form.titulo,
      descricao: form.descricao,
      departamentoId: form.departamentoId || undefined,
      categoriaId: form.categoriaId,
      prioridadeId: form.prioridadeId,
    })

    for (const arquivo of anexosPendentes.value) {
      await portalService.anexarArquivo(chamado.id, arquivo)
    }

    await router.replace(`/portal/chamados/${chamado.id}`)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Não foi possível salvar as informações.'
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
    <PageHeader titulo="Novo chamado" subtitulo="Registre a solicitação com todos os detalhes necessários.">
      <template #actions>
        <q-btn flat color="primary" icon="arrow_back" label="Voltar" @click="cancelar" />
      </template>
    </PageHeader>

    <ErrorState v-if="erro && !carregandoContexto" :mensagem="erro" @retry="carregarContexto" />

    <LoadingState v-else-if="carregandoContexto" inline mensagem="Carregando opções do formulário..." />

    <q-form v-else ref="formRef" class="column q-gutter-md" @submit.prevent="salvar">
      <AppSectionCard titulo="Dados do chamado" subtitulo="Os campos com * são obrigatórios.">
        <div class="column q-gutter-md">
          <q-input
            v-model="form.titulo"
            outlined
            maxlength="180"
            counter
            label="Título *"
            :rules="[(v) => !!String(v || '').trim() || 'Informe o título do chamado']"
          />

          <q-input
            v-model="form.descricao"
            outlined
            type="textarea"
            autogrow
            maxlength="4000"
            counter
            label="Descrição *"
            :rules="[(v) => !!String(v || '').trim() || 'Informe a descrição do chamado']"
          />

          <div class="row q-col-gutter-sm">
            <div class="col-12 col-md-4">
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

            <div class="col-12 col-md-4">
              <q-select
                v-model="form.categoriaId"
                outlined
                emit-value
                map-options
                label="Categoria *"
                :options="opcoesCategoria"
                :rules="[(v) => !!v || 'Selecione uma categoria']"
              />
            </div>

            <div class="col-12 col-md-4">
              <q-select
                v-model="form.prioridadeId"
                outlined
                emit-value
                map-options
                label="Prioridade *"
                :options="opcoesPrioridade"
                :rules="[(v) => !!v || 'Selecione uma prioridade']"
              />
            </div>
          </div>
        </div>
      </AppSectionCard>

      <AppSectionCard titulo="Anexos" subtitulo="Opcional: adicione arquivos de apoio para acelerar o atendimento.">
        <div class="column q-gutter-md">
          <UploadAnexo titulo="Selecionar anexo" :loading="salvando" @upload="adicionarAnexos" />

          <q-banner v-if="!anexosPendentes.length" rounded class="bg-blue-1 text-primary">
            Nenhum anexo adicionado.
          </q-banner>

          <q-list v-else bordered separator>
            <q-item v-for="(arquivo, indice) in anexosPendentes" :key="`${arquivo.name}-${indice}`">
              <q-item-section>
                <q-item-label>{{ arquivo.name }}</q-item-label>
                <q-item-label caption>{{ (arquivo.size / 1024).toFixed(1) }} KB</q-item-label>
              </q-item-section>

              <q-item-section side>
                <q-btn
                  flat
                  round
                  dense
                  icon="delete"
                  color="negative"
                  :disable="salvando"
                  @click="removerAnexo(indice)"
                />
              </q-item-section>
            </q-item>
          </q-list>
        </div>
      </AppSectionCard>

      <div class="row justify-end q-gutter-sm">
        <q-btn flat color="primary" label="Cancelar" :disable="salvando" @click="cancelar" />
        <q-btn type="submit" color="secondary" icon="save" label="Salvar chamado" :loading="salvando" />
      </div>
    </q-form>
  </q-page>
</template>
