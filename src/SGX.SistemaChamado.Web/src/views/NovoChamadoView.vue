<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { portalService } from '../services/portalService'
import type { CategoriaPortal, DepartamentoPortal, PrioridadePortal } from '../types/portal'

const router = useRouter()
const loading = ref(false)
const erro = ref<string | null>(null)

const departamentos = ref<DepartamentoPortal[]>([])
const categorias = ref<CategoriaPortal[]>([])
const prioridades = ref<PrioridadePortal[]>([])

const form = reactive({
  titulo: '',
  descricao: '',
  departamentoId: '',
  categoriaId: '',
  prioridadeId: '',
})

async function carregarContexto() {
  const contexto = await portalService.obterPortalContexto()
  departamentos.value = contexto.departamentos
  categorias.value = contexto.categorias
  prioridades.value = contexto.prioridades
}

async function salvar() {
  loading.value = true
  erro.value = null
  try {
    const response = await portalService.abrirChamado({
      titulo: form.titulo,
      descricao: form.descricao,
      departamentoId: form.departamentoId || undefined,
      categoriaId: form.categoriaId,
      prioridadeId: form.prioridadeId,
    })

    await router.replace(`/portal/chamados/${response.id}`)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao abrir chamado.'
  } finally {
    loading.value = false
  }
}

onMounted(carregarContexto)
</script>

<template>
  <div class="column q-gutter-md">
    <div class="row items-center justify-between">
      <h1 class="text-h6 q-my-none">Novo chamado</h1>
      <q-btn flat color="primary" label="Voltar" @click="router.push('/portal/chamados')" />
    </div>

    <q-card flat bordered>
      <q-card-section class="column q-gutter-md">
        <q-input v-model="form.titulo" outlined label="Título" maxlength="180" />
        <q-input v-model="form.descricao" outlined type="textarea" autogrow label="Descrição" maxlength="4000" />
        <q-select v-model="form.departamentoId" :options="departamentos.map(d => ({ label: `${d.sigla} - ${d.nome}`, value: d.id }))" option-label="label" option-value="value" emit-value map-options clearable outlined label="Departamento" />
        <q-select v-model="form.categoriaId" :options="categorias.map(c => ({ label: c.nome, value: c.id }))" option-label="label" option-value="value" emit-value map-options outlined label="Categoria" />
        <q-select v-model="form.prioridadeId" :options="prioridades.map(p => ({ label: p.nome, value: p.id }))" option-label="label" option-value="value" emit-value map-options outlined label="Prioridade" />
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat label="Cancelar" @click="router.push('/portal/chamados')" />
        <q-btn color="secondary" :loading="loading" label="Criar chamado" @click="salvar" />
      </q-card-actions>
    </q-card>

    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
  </div>
</template>
