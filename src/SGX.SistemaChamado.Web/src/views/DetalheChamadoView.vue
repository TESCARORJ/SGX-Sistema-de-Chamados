<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import { portalService } from '../services/portalService'
import type { ChamadoDetalhePortal } from '../types/portal'
import FormComentario from '../components/portal/FormComentario.vue'
import UploadAnexo from '../components/portal/UploadAnexo.vue'
import TimelineHistorico from '../components/portal/TimelineHistorico.vue'
import StatusBadge from '../components/portal/StatusBadge.vue'
import PrioridadeBadge from '../components/portal/PrioridadeBadge.vue'

const route = useRoute()
const loading = ref(false)
const erro = ref<string | null>(null)
const detalhe = ref<ChamadoDetalhePortal | null>(null)

function estaProximoVencimento(): boolean {
  if (!detalhe.value?.sla) return false
  if (detalhe.value.sla.estaVencido || detalhe.value.sla.estaPausado || detalhe.value.sla.resolvidoEm) return false
  return new Date(detalhe.value.sla.prazoResolucaoEm).getTime() <= Date.now() + (4 * 60 * 60 * 1000)
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
  <div class="column q-gutter-md">
    <q-spinner v-if="loading" color="primary" size="2rem" />
    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>

    <template v-if="detalhe && !loading">
      <q-card flat bordered>
        <q-card-section class="row items-start justify-between">
          <div>
            <div class="text-caption text-grey-7">{{ detalhe.codigo }}</div>
            <div class="text-h6">{{ detalhe.titulo }}</div>
            <div class="text-body2 q-mt-sm">{{ detalhe.descricao }}</div>
            <div class="text-caption text-grey-8 q-mt-sm">
              {{ detalhe.categoria }} • {{ detalhe.departamento || 'Sem departamento' }}
            </div>
          </div>
          <div class="column items-end q-gutter-xs">
            <StatusBadge :status="detalhe.status" />
            <PrioridadeBadge :prioridade="detalhe.prioridade" />
          </div>
        </q-card-section>
        <q-separator />
        <q-card-section class="column q-gutter-xs">
          <q-badge v-if="detalhe.sla?.estaVencido" color="negative" outline>SLA vencido</q-badge>
          <q-badge v-else-if="estaProximoVencimento()" color="warning" outline>Prazo proximo do vencimento</q-badge>
          <q-badge v-else-if="detalhe.sla?.estaPausado" color="grey-7" outline>SLA pausado</q-badge>
          <q-badge v-else color="positive" outline>Dentro do prazo</q-badge>
          <div class="text-caption text-grey-8">
            Prazo previsto: {{ detalhe.sla ? new Date(detalhe.sla.prazoResolucaoEm).toLocaleString('pt-BR') : '-' }}
          </div>
        </q-card-section>
      </q-card>

      <q-card flat bordered>
        <q-card-section>
          <div class="text-subtitle1">Comentarios</div>
        </q-card-section>
        <q-separator />
        <q-card-section class="column q-gutter-sm">
          <q-banner v-if="!detalhe.comentarios.length" class="bg-blue-1 text-primary">Nenhum comentario ainda.</q-banner>
          <q-card v-for="comentario in detalhe.comentarios" :key="comentario.id" flat bordered>
            <q-card-section>
              <div class="text-caption text-grey-7">{{ comentario.usuario }} • {{ new Date(comentario.criadoEm).toLocaleString() }}</div>
              <div class="text-body2">{{ comentario.mensagem }}</div>
            </q-card-section>
          </q-card>
          <FormComentario @submit="comentar" />
        </q-card-section>
      </q-card>

      <q-card flat bordered>
        <q-card-section>
          <div class="text-subtitle1">Anexos</div>
        </q-card-section>
        <q-separator />
        <q-card-section class="column q-gutter-sm">
          <q-banner v-if="!detalhe.anexos.length" class="bg-blue-1 text-primary">Nenhum anexo enviado.</q-banner>
          <q-list bordered separator>
            <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
              <q-item-section>
                <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                <q-item-label caption>{{ anexo.usuario }} • {{ (anexo.tamanhoBytes / 1024).toFixed(1) }} KB</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
          <UploadAnexo @upload="anexar" />
        </q-card-section>
      </q-card>

      <q-card flat bordered>
        <q-card-section>
          <div class="text-subtitle1">Historico</div>
        </q-card-section>
        <q-separator />
        <q-card-section>
          <TimelineHistorico :itens="detalhe.historico" />
        </q-card-section>
      </q-card>
    </template>
  </div>
</template>
