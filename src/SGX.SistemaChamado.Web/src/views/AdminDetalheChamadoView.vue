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
const slaProximo = computed(() => detalhe.value?.sla ? !detalhe.value.sla.estaVencido && !detalhe.value.sla.estaPausado && !detalhe.value.sla.resolvidoEm && new Date(detalhe.value.sla.prazoResolucaoEm).getTime() <= Date.now() + (4 * 60 * 60 * 1000) : false)

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
  <div class="column q-gutter-md">
    <q-banner v-if="erro" class="bg-red-1 text-negative">{{ erro }}</q-banner>
    <q-banner v-if="sucesso" class="bg-green-1 text-positive">{{ sucesso }}</q-banner>
    <q-spinner v-if="loading" size="2rem" color="primary" />

    <template v-if="detalhe">
      <div class="row items-center justify-between">
        <h1 class="text-h6 q-my-none">{{ detalhe.codigo }} - {{ detalhe.titulo }}</h1>
        <div class="row q-gutter-sm">
          <q-badge color="primary">{{ detalhe.status }}</q-badge>
          <q-badge color="secondary">{{ detalhe.prioridade }}</q-badge>
        </div>
      </div>

      <q-card flat bordered>
        <q-card-section>
          <div class="text-body1">{{ detalhe.descricao }}</div>
          <div class="row q-col-gutter-md q-mt-md">
            <div class="col-12 col-md-6">
              <div><strong>Solicitante:</strong> {{ detalhe.solicitante.nome }} ({{ detalhe.solicitante.email }})</div>
              <div><strong>Responsavel:</strong> {{ detalhe.responsavel?.nome ?? 'Nao definido' }}</div>
              <div><strong>Categoria:</strong> {{ detalhe.categoria }}</div>
              <div><strong>Departamento:</strong> {{ detalhe.departamento ?? '-' }}</div>
            </div>
            <div class="col-12 col-md-6">
              <div><strong>Origem:</strong> {{ detalhe.origem }}</div>
              <div><strong>Aberto em:</strong> {{ fmtDate(detalhe.abertoEm) }}</div>
              <div><strong>Encerrado em:</strong> {{ fmtDate(detalhe.encerradoEm) }}</div>
              <div><strong>Prazo primeira resposta:</strong> {{ fmtDate(detalhe.sla?.prazoPrimeiraRespostaEm ?? null) }}</div>
              <div><strong>Primeira resposta:</strong> {{ fmtDate(detalhe.sla?.primeiraRespostaEm ?? null) }}</div>
              <div><strong>Prazo resolucao:</strong> {{ fmtDate(detalhe.sla?.prazoResolucaoEm ?? null) }}</div>
              <div><strong>Resolvido em:</strong> {{ fmtDate(detalhe.sla?.resolvidoEm ?? null) }}</div>
              <div><strong>SLA pausado:</strong> {{ detalhe.sla?.estaPausado ? 'Sim' : 'Nao' }}</div>
              <div><strong>Total pausado:</strong> {{ detalhe.sla?.totalMinutosPausado ?? 0 }} min</div>
            </div>
          </div>
        </q-card-section>
      </q-card>

      <q-banner v-if="detalhe.sla?.estaVencido" class="bg-red-1 text-negative">
        SLA vencido para este chamado.
      </q-banner>
      <q-banner v-else-if="slaProximo" class="bg-orange-1 text-warning">
        SLA proximo do vencimento (ate 4 horas).
      </q-banner>
      <q-banner v-else-if="detalhe.sla?.estaPausado" class="bg-grey-2 text-grey-9">
        SLA pausado pelo status atual.
      </q-banner>

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

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Comentarios administrativos</div>
            </q-card-section>
            <q-separator />
            <q-card-section>
              <ComentariosAdministrativos :comentarios="detalhe.comentarios" />
            </q-card-section>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-subtitle1">Historico administrativo</div>
            </q-card-section>
            <q-separator />
            <q-card-section>
              <TimelineAdministrativa :historico="detalhe.historico" />
            </q-card-section>
          </q-card>
        </div>
      </div>

      <q-card flat bordered>
        <q-card-section>
          <div class="text-subtitle1">Anexos</div>
          <q-list bordered separator>
            <q-item v-for="anexo in detalhe.anexos" :key="anexo.id">
              <q-item-section>
                <q-item-label>{{ anexo.nomeArquivo }}</q-item-label>
                <q-item-label caption>{{ anexo.contentType }} | {{ anexo.usuario }} | {{ fmtDate(anexo.criadoEm) }}</q-item-label>
              </q-item-section>
            </q-item>
          </q-list>
          <q-banner v-if="!detalhe.anexos.length" class="bg-blue-1 text-primary q-mt-sm">Sem anexos.</q-banner>
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

    <ModalEncerrarChamado
      v-model="showEncerrar"
      :loading="processing"
      @confirmar="encerrar"
    />

    <ModalReabrirChamado
      v-model="showReabrir"
      :loading="processing"
      @confirmar="reabrir"
    />

    <q-dialog v-model="showComentar">
      <q-card style="min-width: 420px">
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
  </div>
</template>
