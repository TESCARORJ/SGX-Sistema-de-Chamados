<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import FormComentario from '../components/portal/FormComentario.vue'
import TimelineHistorico from '../components/portal/TimelineHistorico.vue'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { portalService } from '../services/portalService'
import type { ChamadoDetalhePortal } from '../types/portal'

const route = useRoute()
const loading = ref(false)
const erro = ref<string | null>(null)
const detalhe = ref<ChamadoDetalhePortal | null>(null)

function estaProximoVencimento(): boolean {
  if (!detalhe.value?.sla) return false
  if (detalhe.value.sla.estaVencido || detalhe.value.sla.estaPausado || detalhe.value.sla.resolvidoEm) return false
  return new Date(detalhe.value.sla.prazoResolucaoEm).getTime() <= Date.now() + 4 * 60 * 60 * 1000
}

async function carregar() {
  const id = String(route.params.id)
  loading.value = true
  erro.value = null
  try {
    detalhe.value = await portalService.obterChamado(id)
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar chamado.'
  } finally {
    loading.value = false
  }
}

async function comentar(mensagem: string) {
  if (!detalhe.value) return
  try {
    await portalService.comentarChamado(detalhe.value.id, { mensagem })
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao comentar.'
  }
}

async function anexar(file: File) {
  if (!detalhe.value) return
  try {
    await portalService.anexarArquivo(detalhe.value.id, file)
    await carregar()
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao anexar arquivo.'
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.titulo}` : 'Detalhe do chamado'"
      subtitulo="Acompanhe andamento, comentarios, anexos e historico"
    >
      <template #actions>
        <div v-if="detalhe" class="row q-gutter-xs">
          <StatusBadge :texto="detalhe.status" />
          <PrioridadeBadge :texto="detalhe.prioridade" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
    </div>

    <template v-if="detalhe && !loading">
      <q-card flat bordered class="sgx-card">
        <q-card-section>
          <div class="text-body1">{{ detalhe.descricao }}</div>
          <div class="text-caption text-grey-7 q-mt-sm">
            {{ detalhe.categoria }} | {{ detalhe.departamento || 'Sem departamento' }}
          </div>
        </q-card-section>
        <q-separator />
        <q-card-section>
          <SlaBadge
            :vencido="detalhe.sla?.estaVencido"
            :proximo="estaProximoVencimento()"
            :pausado="detalhe.sla?.estaPausado"
          />
          <div class="text-caption text-grey-8 q-mt-sm">
            Prazo previsto: {{ detalhe.sla ? new Date(detalhe.sla.prazoResolucaoEm).toLocaleString('pt-BR') : '-' }}
          </div>
        </q-card-section>
      </q-card>

      <q-card flat bordered class="sgx-card">
        <q-card-section class="text-subtitle1 text-weight-medium">Comentarios</q-card-section>
        <q-separator />
        <q-card-section class="column q-gutter-sm">
          <q-banner v-if="!detalhe.comentarios.length" rounded class="bg-blue-1 text-primary">
            Nenhum comentario ainda.
          </q-banner>
          <q-card v-for="comentario in detalhe.comentarios" :key="comentario.id" flat bordered>
            <q-card-section>
              <div class="text-caption text-grey-7">
                {{ comentario.usuario }} | {{ new Date(comentario.criadoEm).toLocaleString('pt-BR') }}
              </div>
              <div class="text-body2">{{ comentario.mensagem }}</div>
            </q-card-section>
          </q-card>
          <FormComentario @submit="comentar" />
        </q-card-section>
      </q-card>

      <q-card flat bordered class="sgx-card">
        <q-card-section class="text-subtitle1 text-weight-medium">Anexos</q-card-section>
        <q-separator />
        <q-card-section class="column q-gutter-sm">
          <q-banner v-if="!detalhe.anexos.length" rounded class="bg-blue-1 text-primary">
            Nenhum anexo enviado.
          </q-banner>
          <q-list bordered separator>
            <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
              <q-item-section>
                <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                <q-item-label caption>
                  {{ anexo.usuario }} | {{ (anexo.tamanhoBytes / 1024).toFixed(1) }} KB
                </q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
          <UploadAnexo @upload="anexar" />
        </q-card-section>
      </q-card>

      <q-card flat bordered class="sgx-card">
        <q-card-section class="text-subtitle1 text-weight-medium">Historico</q-card-section>
        <q-separator />
        <q-card-section>
          <TimelineHistorico :itens="detalhe.historico" />
        </q-card-section>
      </q-card>
    </template>
  </q-page>
</template>
