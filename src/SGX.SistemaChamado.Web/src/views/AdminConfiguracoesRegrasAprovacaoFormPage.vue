<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useQuasar } from 'quasar'

import AppSectionCard from '../components/ui/AppSectionCard.vue'
import PageHeader from '../components/ui/PageHeader.vue'

import { configuracoesRegrasAprovacaoService } from '../services/configuracoesRegrasAprovacaoService'
import { adminService } from '../services/adminService'
import type { 
  CriarConfiguracaoRegraAprovacaoRequest,
  AtualizarConfiguracaoRegraAprovacaoRequest
} from '../types/aprovacoesMotor'
import { 
  TipoRegraAprovacao, 
  EscopoRegraAprovacao, 
  EfeitoOperacionalRegraAprovacao, 
  TipoFluxoAprovacao, 
  TipoResolucaoAprovadorRegraAprovacao 
} from '../types/aprovacoesMotor'
import type { AdminContextoResponse } from '../types/admin'

const route = useRoute()
const router = useRouter()
const $q = useQuasar()

const isEdicao = computed(() => route.params.id && route.params.id !== 'nova')
const regraId = computed(() => route.params.id as string)

const loading = ref(false)
const salvando = ref(false)
const validando = ref(false)
const contexto = ref<AdminContextoResponse | null>(null)

const form = ref<CriarConfiguracaoRegraAprovacaoRequest>({
  nome: '',
  descricao: '',
  tipoRegra: TipoRegraAprovacao.Geral,
  escopoRegra: EscopoRegraAprovacao.EscopoGeralChamado,
  ordem: 10,
  prioridade: 1,
  versao: 1,
  naturezaChamado: null,
  tipoSolicitacaoId: null,
  catalogoServicoId: null,
  categoriaId: null,
  subcategoriaId: null,
  impactoMinimo: null,
  urgenciaMinima: null,
  prioridadeMinima: null,
  custoMinimo: null,
  nivelRiscoMinimo: null,
  exigeAprovacao: true,
  bloqueante: true,
  permiteReenvio: false,
  permiteFallback: true,
  efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
  tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
  tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
  aprovadorEspecificoUsuarioId: null,
  aprovadorPadraoUsuarioId: null,
  prazoDecisaoHoras: 24,
  vigenteDe: null,
  vigenteAte: null,
  ativo: true
})

const validacaoErros = ref<string[]>([])
const validacaoAlertas = ref<string[]>([])

const opcoesTipoRegra = [
  { label: 'Geral', value: TipoRegraAprovacao.Geral },
  { label: 'Natureza ITSM', value: TipoRegraAprovacao.NaturezaItsm },
  { label: 'Tipo de Solicitação', value: TipoRegraAprovacao.TipoSolicitacao },
  { label: 'Catálogo de Serviço', value: TipoRegraAprovacao.CatalogoServico },
  { label: 'Categoria / Subcategoria', value: TipoRegraAprovacao.CategoriaSubcategoria },
  { label: 'Impacto / Urgência', value: TipoRegraAprovacao.ImpactoUrgencia },
  { label: 'Custo ou Risco Futuro', value: TipoRegraAprovacao.CustoOuRiscoFuturo },
  { label: 'Combinada', value: TipoRegraAprovacao.Combinada }
]

const opcoesEscopo = [
  { label: 'Escopo Geral', value: EscopoRegraAprovacao.EscopoGeralChamado },
  { label: 'Abertura', value: EscopoRegraAprovacao.AberturaChamado },
  { label: 'Atendimento', value: EscopoRegraAprovacao.AtendimentoChamado },
  { label: 'Encerramento', value: EscopoRegraAprovacao.EncerramentoChamado },
  { label: 'Reabertura', value: EscopoRegraAprovacao.ReaberturaChamado }
]

const opcoesEfeito = [
  { label: 'Permitir', value: EfeitoOperacionalRegraAprovacao.Permitir },
  { label: 'Sinalizar', value: EfeitoOperacionalRegraAprovacao.Sinalizar },
  { label: 'Exigir Aprovação', value: EfeitoOperacionalRegraAprovacao.ExigirAprovacao },
  { label: 'Exigir e Bloquear Avanço', value: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco },
  { label: 'Requer Reavaliação', value: EfeitoOperacionalRegraAprovacao.RequerReavaliacao }
]

const opcoesFluxo = [
  { label: 'Simples', value: TipoFluxoAprovacao.Simples },
  { label: 'Sequencial', value: TipoFluxoAprovacao.Sequencial },
  { label: 'Paralela', value: TipoFluxoAprovacao.Paralela },
  { label: 'Multinível', value: TipoFluxoAprovacao.Multinivel }
]

const opcoesResolucao = [
  { label: 'Não Definido', value: TipoResolucaoAprovadorRegraAprovacao.NaoDefinido },
  { label: 'Aprovador Específico', value: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico },
  { label: 'Aprovador Padrão', value: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao },
  { label: 'Grupo Aprovador (Futuro)', value: TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo },
  { label: 'Resolução Dinâmica (Futura)', value: TipoResolucaoAprovadorRegraAprovacao.ResolucaoDinamicaFutura }
]

const opcoesNatureza = [
  { label: 'Incidente', value: 1 },
  { label: 'Requisicao', value: 2 },
  { label: 'Problema', value: 3 },
  { label: 'Mudanca', value: 4 },
  { label: 'Tarefa', value: 5 }
]

const categoriasOpcoes = computed(() => {
  if (!contexto.value) return []
  return contexto.value.categorias.map(c => ({ label: c.nome, value: c.id }))
})

const subcategoriasOpcoes = computed(() => {
  if (!contexto.value || !form.value.categoriaId) return []
  return contexto.value.subcategorias
    .filter(s => s.categoriaChamadoId === form.value.categoriaId)
    .map(s => ({ label: s.nome, value: s.id }))
})

const tiposSolicitacaoOpcoes = computed(() => {
  if (!contexto.value) return []
  return contexto.value.tiposSolicitacao.map(t => ({ label: t.nome, value: t.id }))
})

const atendentesOpcoes = computed(() => {
  if (!contexto.value) return []
  return contexto.value.atendentes.map(a => ({ label: a.nome, value: a.id }))
})

async function carregarContexto() {
  try {
    contexto.value = await adminService.obterContextoAdministrativo()
  } catch (error) {
    $q.notify({ type: 'negative', message: 'Erro ao carregar contexto base.' })
  }
}

async function carregarRegistro() {
  if (!isEdicao.value) return

  loading.value = true
  try {
    const data = await configuracoesRegrasAprovacaoService.obterPorId(regraId.value)
    form.value = {
      nome: data.nome,
      descricao: data.descricao,
      tipoRegra: data.tipoRegra,
      escopoRegra: data.escopoRegra,
      ordem: data.ordem,
      prioridade: data.prioridade,
      versao: data.versao,
      naturezaChamado: data.naturezaChamado,
      tipoSolicitacaoId: data.tipoSolicitacaoId,
      catalogoServicoId: data.catalogoServicoId,
      categoriaId: data.categoriaId,
      subcategoriaId: data.subcategoriaId,
      impactoMinimo: data.impactoMinimo,
      urgenciaMinima: data.urgenciaMinima,
      prioridadeMinima: data.prioridadeMinima,
      custoMinimo: data.custoMinimo,
      nivelRiscoMinimo: data.nivelRiscoMinimo,
      exigeAprovacao: data.exigeAprovacao,
      bloqueante: data.bloqueante,
      permiteReenvio: data.permiteReenvio,
      permiteFallback: data.permiteFallback,
      efeitoOperacional: data.efeitoOperacional,
      tipoFluxoAprovacao: data.tipoFluxoAprovacao,
      tipoResolucaoAprovador: data.tipoResolucaoAprovador,
      aprovadorEspecificoUsuarioId: data.aprovadorEspecificoUsuarioId,
      aprovadorPadraoUsuarioId: data.aprovadorPadraoUsuarioId,
      prazoDecisaoHoras: data.prazoDecisaoHoras,
      vigenteDe: data.vigenteDe ? data.vigenteDe.split('T')[0] : null,
      vigenteAte: data.vigenteAte ? data.vigenteAte.split('T')[0] : null,
      ativo: data.ativo
    }
  } catch (error) {
    $q.notify({ type: 'negative', message: 'Erro ao carregar a regra de aprovação.' })
    router.push('/admin/configuracoes/regras-aprovacao')
  } finally {
    loading.value = false
  }
}

async function validarRegra() {
  validando.value = true
  validacaoErros.value = []
  validacaoAlertas.value = []
  try {
    const res = await configuracoesRegrasAprovacaoService.validar({
      configuracaoRegraAprovacaoId: isEdicao.value ? regraId.value : null,
      configuracao: form.value
    })
    
    if (res.valida) {
      $q.notify({ type: 'positive', message: 'Configuração válida!' })
    } else {
      $q.notify({ type: 'warning', message: 'A configuração possui erros de validação.' })
    }
    
    validacaoErros.value = res.erros || []
    validacaoAlertas.value = res.alertas || []
  } catch (error) {
    $q.notify({ type: 'negative', message: 'Erro ao validar a configuração com o servidor.' })
  } finally {
    validando.value = false
  }
}

async function salvar() {
  salvando.value = true
  try {
    if (isEdicao.value) {
      await configuracoesRegrasAprovacaoService.atualizar(regraId.value, form.value as AtualizarConfiguracaoRegraAprovacaoRequest)
      $q.notify({ type: 'positive', message: 'Regra atualizada com sucesso.' })
    } else {
      await configuracoesRegrasAprovacaoService.criar(form.value)
      $q.notify({ type: 'positive', message: 'Regra criada com sucesso.' })
    }
    router.push('/admin/configuracoes/regras-aprovacao')
  } catch (error) {
    $q.notify({ type: 'negative', message: error instanceof Error ? error.message : 'Erro ao salvar a regra de aprovação.' })
  } finally {
    salvando.value = false
  }
}

onMounted(async () => {
  loading.value = true
  await carregarContexto()
  await carregarRegistro()
  loading.value = false
})

</script>

<template>
  <q-page class="sgx-page column q-gutter-md">
    <PageHeader
      :titulo="isEdicao ? 'Editar Regra de Aprovação' : 'Nova Regra de Aprovação'"
      contexto="Configurações Administrativas"
      :subtitulo="isEdicao ? 'Altere as configurações desta regra do motor' : 'Defina os parâmetros para a nova regra'"
      voltar-para="/admin/configuracoes/regras-aprovacao"
    />

    <div v-if="loading" class="row justify-center q-py-xl">
      <q-spinner color="primary" size="3em" />
    </div>

    <q-form v-else @submit.prevent="salvar" class="column q-gutter-md">
      
      <div v-if="validacaoErros.length > 0" class="q-mb-md">
        <q-banner class="bg-red-1 text-negative" rounded>
          <div class="text-weight-bold q-mb-sm">Erros de Validação:</div>
          <ul>
            <li v-for="(erro, idx) in validacaoErros" :key="idx">{{ erro }}</li>
          </ul>
        </q-banner>
      </div>

      <div v-if="validacaoAlertas.length > 0" class="q-mb-md">
        <q-banner class="bg-orange-1 text-warning" rounded>
          <div class="text-weight-bold q-mb-sm">Alertas:</div>
          <ul>
            <li v-for="(alerta, idx) in validacaoAlertas" :key="idx">{{ alerta }}</li>
          </ul>
        </q-banner>
      </div>

      <!-- Identificação -->
      <AppSectionCard titulo="Identificação" subtitulo="Dados básicos da regra">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-8">
            <q-input v-model="form.nome" label="Nome da Regra *" outlined dense :rules="[val => !!val || 'Nome é obrigatório']" />
          </div>
          <div class="col-12 col-md-2">
            <q-input v-model.number="form.ordem" label="Ordem *" type="number" outlined dense :rules="[val => val !== null || 'Obrigatório']" />
          </div>
          <div class="col-12 col-md-2">
            <q-input v-model.number="form.prioridade" label="Prioridade *" type="number" outlined dense :rules="[val => val !== null || 'Obrigatório']" />
          </div>
          <div class="col-12">
            <q-input v-model="form.descricao" label="Descrição" type="textarea" outlined dense rows="3" />
          </div>
        </div>
      </AppSectionCard>

      <!-- Critérios de Aplicação -->
      <AppSectionCard titulo="Critérios de Aplicação" subtitulo="Quando esta regra será avaliada e acionada?">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <q-select v-model="form.tipoRegra" :options="opcoesTipoRegra" label="Tipo de Regra *" outlined dense emit-value map-options />
          </div>
          <div class="col-12 col-md-6">
            <q-select v-model="form.escopoRegra" :options="opcoesEscopo" label="Escopo da Regra *" outlined dense emit-value map-options />
          </div>
          <div class="col-12 col-md-4">
            <q-select v-model="form.naturezaChamado" :options="opcoesNatureza" label="Natureza do Chamado" outlined dense clearable emit-value map-options />
          </div>
          <div class="col-12 col-md-4">
            <q-select v-model="form.tipoSolicitacaoId" :options="tiposSolicitacaoOpcoes" label="Tipo de Solicitação" outlined dense clearable emit-value map-options />
          </div>
          <div class="col-12 col-md-4">
            <!-- Catálogo de Serviço não mapeado integralmente aqui, deixando campo simples por hora se precisar -->
            <q-input v-model="form.catalogoServicoId" label="ID Catálogo Serviço (Opcional)" outlined dense clearable />
          </div>
          <div class="col-12 col-md-6">
            <q-select v-model="form.categoriaId" :options="categoriasOpcoes" label="Categoria" outlined dense clearable emit-value map-options />
          </div>
          <div class="col-12 col-md-6">
            <q-select v-model="form.subcategoriaId" :options="subcategoriasOpcoes" label="Subcategoria" outlined dense clearable emit-value map-options :disable="!form.categoriaId" />
          </div>
        </div>
      </AppSectionCard>

      <!-- Efeitos Operacionais -->
      <AppSectionCard titulo="Efeitos Operacionais" subtitulo="Comportamento quando a regra for acionada">
        <div class="row q-col-gutter-md q-mb-md">
          <div class="col-12 col-md-6">
            <q-select v-model="form.efeitoOperacional" :options="opcoesEfeito" label="Efeito Operacional *" outlined dense emit-value map-options />
          </div>
          <div class="col-12 col-md-3">
            <q-toggle v-model="form.exigeAprovacao" label="Exige Aprovação" color="primary" />
          </div>
          <div class="col-12 col-md-3">
            <q-toggle v-model="form.bloqueante" label="Regra Bloqueante" color="negative" />
          </div>
        </div>
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <q-toggle v-model="form.permiteReenvio" label="Permite Reenvio após Rejeição?" color="primary" />
          </div>
          <div class="col-12 col-md-6">
            <q-toggle v-model="form.permiteFallback" label="Permite Fallback (escalonamento)?" color="primary" />
          </div>
        </div>
      </AppSectionCard>

      <!-- Fluxo e Decisores -->
      <AppSectionCard titulo="Fluxo e Decisores" subtitulo="Como será o processo decisório">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-6">
            <q-select v-model="form.tipoFluxoAprovacao" :options="opcoesFluxo" label="Tipo de Fluxo *" outlined dense emit-value map-options />
          </div>
          <div class="col-12 col-md-6">
            <q-select v-model="form.tipoResolucaoAprovador" :options="opcoesResolucao" label="Resolução do Aprovador *" outlined dense emit-value map-options />
          </div>
          <div class="col-12 col-md-6" v-if="form.tipoResolucaoAprovador === TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico">
            <q-select v-model="form.aprovadorEspecificoUsuarioId" :options="atendentesOpcoes" label="Aprovador Específico *" outlined dense clearable emit-value map-options />
          </div>
          <div class="col-12 col-md-6" v-if="form.tipoResolucaoAprovador === TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao">
            <q-select v-model="form.aprovadorPadraoUsuarioId" :options="atendentesOpcoes" label="Aprovador Padrão (Fallback)" outlined dense clearable emit-value map-options />
          </div>
          <div class="col-12 col-md-6">
            <q-input v-model.number="form.prazoDecisaoHoras" label="Prazo para Decisão (Horas)" type="number" outlined dense clearable />
          </div>
        </div>
      </AppSectionCard>

      <!-- Vigência e Status -->
      <AppSectionCard titulo="Vigência e Status" subtitulo="Controle de ativação e tempo de vida">
        <div class="row q-col-gutter-md">
          <div class="col-12 col-md-4">
            <q-input v-model="form.vigenteDe" label="Vigente a partir de" type="date" outlined dense clearable />
          </div>
          <div class="col-12 col-md-4">
            <q-input v-model="form.vigenteAte" label="Vigente até" type="date" outlined dense clearable />
          </div>
          <div class="col-12 col-md-4 row items-center">
            <q-toggle v-model="form.ativo" label="Regra Ativa" color="positive" />
          </div>
        </div>
      </AppSectionCard>

      <div class="row justify-end q-gutter-sm q-mt-md">
        <q-btn flat label="Cancelar" color="grey-8" to="/admin/configuracoes/regras-aprovacao" :disable="salvando" />
        <q-btn flat label="Validar Regra" color="warning" icon="rule" @click="validarRegra" :loading="validando" :disable="salvando" />
        <q-btn type="submit" label="Salvar Regra" color="primary" icon="save" :loading="salvando" :disable="validando" />
      </div>

    </q-form>
  </q-page>
</template>
