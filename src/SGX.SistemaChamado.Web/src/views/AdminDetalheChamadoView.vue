<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import ComentariosAdministrativos from '../components/admin/ComentariosAdministrativos.vue'
import ModalAlterarCategoria from '../components/admin/ModalAlterarCategoria.vue'
import ModalAlterarPrioridade from '../components/admin/ModalAlterarPrioridade.vue'
import ModalAlterarStatus from '../components/admin/ModalAlterarStatus.vue'
import ModalAtribuirResponsavel from '../components/admin/ModalAtribuirResponsavel.vue'
import ModalEncerrarChamado from '../components/admin/ModalEncerrarChamado.vue'
import ModalReabrirChamado from '../components/admin/ModalReabrirChamado.vue'
import PainelAtendimento from '../components/admin/PainelAtendimento.vue'
import TimelineAdministrativa from '../components/admin/TimelineAdministrativa.vue'
import PageHeader from '../components/ui/PageHeader.vue'
import PrioridadeBadge from '../components/ui/PrioridadeBadge.vue'
import SlaBadge from '../components/ui/SlaBadge.vue'
import StatusBadge from '../components/ui/StatusBadge.vue'
import { adminService } from '../services/adminService'
import type { AdminContextoResponse, ChamadoAdminDetalhe } from '../types/admin'

const route = useRoute()
const chamadoId = route.params.id as string

const loading = ref(false)
const processing = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)

const contexto = ref<AdminContextoResponse | null>(null)
const detalhe = ref<ChamadoAdminDetalhe | null>(null)

const showAtribuir = ref(false)
const showStatus = ref(false)
const showPrioridade = ref(false)
const showCategoria = ref(false)
const showEncerrar = ref(false)
const showReabrir = ref(false)
const showComentar = ref(false)

const comentarioMensagem = ref('')
const comentarioInterno = ref(false)

const isAdministrador = computed(() => contexto.value?.usuario.perfis.includes('Administrador') ?? false)
const podeAssumir = computed(() => {
  if (!detalhe.value) return false
  return isAdministrador.value || !detalhe.value.responsavel
})
const chamadoEncerrado = computed(() => detalhe.value?.status.toLowerCase().includes('encerrado') ?? false)
const chamadoReabrivel = computed(() => {
  const status = detalhe.value?.status.toLowerCase() ?? ''
  return status.includes('encerrado') || status.includes('resolvido')
})
const slaProximo = computed(() => {
  if (!detalhe.value?.sla) return false
  if (detalhe.value.sla.estaVencido || detalhe.value.sla.estaPausado || detalhe.value.sla.resolvidoEm) return false
  return new Date(detalhe.value.sla.prazoResolucaoEm).getTime() <= Date.now() + 4 * 60 * 60 * 1000
})

function fmtDate(value: string | null): string {
  if (!value) return '-'
  return new Date(value).toLocaleString('pt-BR')
}

async function carregar(): Promise<void> {
  loading.value = true
  erro.value = null

  try {
    const [ctx, det] = await Promise.all([
      adminService.obterAdminContexto(),
      adminService.obterChamadoAdmin(chamadoId),
    ])

    contexto.value = ctx
    detalhe.value = det
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao carregar detalhe administrativo.'
  } finally {
    loading.value = false
  }
}

async function assumir(): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  erro.value = null
  sucesso.value = null
  try {
    detalhe.value = await adminService.assumirChamado(detalhe.value.id)
    sucesso.value = 'Chamado assumido com sucesso.'
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao assumir chamado.'
  } finally {
    processing.value = false
  }
}

async function atribuir(responsavelId: string): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.atribuirChamado(detalhe.value.id, { responsavelId })
    sucesso.value = 'Responsavel atualizado com sucesso.'
    showAtribuir.value = false
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao atribuir chamado.'
  } finally {
    processing.value = false
  }
}

async function alterarStatus(statusId: string): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.alterarStatus(detalhe.value.id, { statusId })
    sucesso.value = 'Status alterado com sucesso.'
    showStatus.value = false
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao alterar status.'
  } finally {
    processing.value = false
  }
}

async function alterarPrioridade(prioridadeId: string): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.alterarPrioridade(detalhe.value.id, { prioridadeId })
    sucesso.value = 'Prioridade alterada com sucesso.'
    showPrioridade.value = false
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao alterar prioridade.'
  } finally {
    processing.value = false
  }
}

async function alterarCategoria(categoriaId: string): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.alterarCategoria(detalhe.value.id, { categoriaId })
    sucesso.value = 'Categoria alterada com sucesso.'
    showCategoria.value = false
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao alterar categoria.'
  } finally {
    processing.value = false
  }
}

async function comentar(): Promise<void> {
  if (!detalhe.value || !comentarioMensagem.value.trim()) return
  processing.value = true
  try {
    await adminService.comentarChamadoAdmin(detalhe.value.id, {
      mensagem: comentarioMensagem.value.trim(),
      interno: comentarioInterno.value,
    })

    comentarioMensagem.value = ''
    comentarioInterno.value = false
    showComentar.value = false

    detalhe.value = await adminService.obterChamadoAdmin(detalhe.value.id)
    sucesso.value = 'Comentario adicionado com sucesso.'
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao comentar chamado.'
  } finally {
    processing.value = false
  }
}

async function encerrar(payload: { solucao: string; comentarioInterno: boolean }): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.encerrarChamado(detalhe.value.id, payload)
    showEncerrar.value = false
    sucesso.value = 'Chamado encerrado com sucesso.'
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao encerrar chamado.'
  } finally {
    processing.value = false
  }
}

async function reabrir(mensagem: string): Promise<void> {
  if (!detalhe.value) return
  processing.value = true
  try {
    detalhe.value = await adminService.reabrirChamado(detalhe.value.id, { mensagem })
    showReabrir.value = false
    sucesso.value = 'Chamado reaberto com sucesso.'
  } catch (error) {
    erro.value = error instanceof Error ? error.message : 'Falha ao reabrir chamado.'
  } finally {
    processing.value = false
  }
}

onMounted(carregar)
</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="detalhe ? `${detalhe.codigo} - ${detalhe.titulo}` : 'Detalhe do chamado'"
      subtitulo="Acompanhe o contexto completo, acoes operacionais e historico"
    >
      <template #actions>
        <div class="row q-gutter-xs">
          <StatusBadge v-if="detalhe" :texto="detalhe.status" />
          <PrioridadeBadge v-if="detalhe" :texto="detalhe.prioridade" />
        </div>
      </template>
    </PageHeader>

    <q-banner v-if="erro" rounded class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">{{ sucesso }}</q-banner>

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="2.2rem" />
    </div>

    <template v-if="detalhe && !loading">
      <q-card flat bordered class="sgx-card">
        <q-card-section>
          <div class="text-body1">{{ detalhe.descricao }}</div>

          <div class="row q-col-gutter-lg q-mt-md">
            <div class="col-12 col-md-6 column q-gutter-xs">
              <div><strong>Solicitante:</strong> {{ detalhe.solicitante.nome }} ({{ detalhe.solicitante.email }})</div>
              <div><strong>Responsavel:</strong> {{ detalhe.responsavel?.nome ?? 'Nao definido' }}</div>
              <div><strong>Categoria:</strong> {{ detalhe.categoria }}</div>
              <div><strong>Departamento:</strong> {{ detalhe.departamento ?? '-' }}</div>
              <div><strong>Origem:</strong> {{ detalhe.origem }}</div>
            </div>
            <div class="col-12 col-md-6 column q-gutter-xs">
              <div><strong>Aberto em:</strong> {{ fmtDate(detalhe.abertoEm) }}</div>
              <div><strong>Encerrado em:</strong> {{ fmtDate(detalhe.encerradoEm) }}</div>
              <div><strong>Prazo primeira resposta:</strong> {{ fmtDate(detalhe.sla?.prazoPrimeiraRespostaEm ?? null) }}</div>
              <div><strong>Primeira resposta:</strong> {{ fmtDate(detalhe.sla?.primeiraRespostaEm ?? null) }}</div>
              <div><strong>Prazo resolucao:</strong> {{ fmtDate(detalhe.sla?.prazoResolucaoEm ?? null) }}</div>
              <div><strong>Resolvido em:</strong> {{ fmtDate(detalhe.sla?.resolvidoEm ?? null) }}</div>
              <div><strong>Total pausado:</strong> {{ detalhe.sla?.totalMinutosPausado ?? 0 }} min</div>
            </div>
          </div>
        </q-card-section>
      </q-card>

      <q-card flat bordered class="sgx-card">
        <q-card-section class="row items-center justify-between">
          <div class="text-subtitle1 text-weight-medium">Situacao do SLA</div>
          <SlaBadge
            :vencido="detalhe.sla?.estaVencido"
            :proximo="slaProximo"
            :pausado="detalhe.sla?.estaPausado"
          />
        </q-card-section>
      </q-card>

      <q-card flat bordered class="sgx-card">
        <q-card-section>
          <PainelAtendimento
            :chamado="detalhe"
            :loading="processing"
            :can-assumir="podeAssumir"
            :can-atribuir="isAdministrador"
            :can-encerrar="!chamadoEncerrado"
            :can-reabrir="chamadoReabrivel"
            @assumir="assumir"
            @atribuir="showAtribuir = true"
            @alterar-status="showStatus = true"
            @alterar-prioridade="showPrioridade = true"
            @alterar-categoria="showCategoria = true"
            @comentar="showComentar = true"
            @encerrar="showEncerrar = true"
            @reabrir="showReabrir = true"
          />
        </q-card-section>
      </q-card>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Comentarios administrativos</q-card-section>
            <q-separator />
            <q-card-section>
              <ComentariosAdministrativos :comentarios="detalhe.comentarios" />
            </q-card-section>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered class="sgx-card full-height">
            <q-card-section class="text-subtitle1 text-weight-medium">Historico administrativo</q-card-section>
            <q-separator />
            <q-card-section>
              <TimelineAdministrativa :historico="detalhe.historico" />
            </q-card-section>
          </q-card>
        </div>
      </div>

      <q-card flat bordered class="sgx-card">
        <q-card-section class="text-subtitle1 text-weight-medium">Anexos</q-card-section>
        <q-separator />
        <q-list bordered separator>
          <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
            <q-item-section>
              <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
              <q-item-label caption>
                {{ anexo.contentType }} | {{ anexo.usuario }} | {{ fmtDate(anexo.criadoEm) }}
              </q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
        <q-card-section v-if="!detalhe.anexos.length">
          <q-banner rounded class="bg-blue-1 text-primary">Sem anexos.</q-banner>
        </q-card-section>
      </q-card>
    </template>

    <ModalAtribuirResponsavel
      v-model="showAtribuir"
      :atendentes="contexto?.atendentes ?? []"
      :loading="processing"
      @confirmar="atribuir"
    />

    <ModalAlterarStatus
      v-model="showStatus"
      :status="contexto?.status ?? []"
      :loading="processing"
      @confirmar="alterarStatus"
    />

    <ModalAlterarPrioridade
      v-model="showPrioridade"
      :prioridades="contexto?.prioridades ?? []"
      :loading="processing"
      @confirmar="alterarPrioridade"
    />

    <ModalAlterarCategoria
      v-model="showCategoria"
      :categorias="contexto?.categorias ?? []"
      :loading="processing"
      @confirmar="alterarCategoria"
    />

    <ModalEncerrarChamado v-model="showEncerrar" :loading="processing" @confirmar="encerrar" />

    <ModalReabrirChamado v-model="showReabrir" :loading="processing" @confirmar="reabrir" />

    <q-dialog v-model="showComentar">
      <q-card style="min-width: 420px" class="sgx-card">
        <q-card-section><div class="text-h6">Comentar chamado</div></q-card-section>
        <q-card-section class="column q-gutter-sm">
          <q-input v-model="comentarioMensagem" outlined type="textarea" autogrow label="Mensagem" />
          <q-toggle v-model="comentarioInterno" label="Comentario interno" />
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Enviar" :loading="processing" @click="comentar" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>
