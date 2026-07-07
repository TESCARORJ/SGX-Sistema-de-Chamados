<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useQuasar } from 'quasar'
import AppSectionCard from '../ui/AppSectionCard.vue'
import { formularioServicosAdminService } from '../../services/formularioServicosAdminService'
import {
  TipoCampoFormularioServico,
  type CampoFormularioServicoAdminDto,
  type FormularioServicoDetalheAdminDto,
  type FormularioServicoVersaoAdminDto,
  type OpcaoCampoFormularioServicoAdminDto,
} from '../../types/formularioServicos'

const props = defineProps<{
  catalogoServicoId: string
  somenteLeitura: boolean
}>()

const $q = useQuasar()

const loading = ref(false)
const salvando = ref(false)
const erro = ref<string | null>(null)
const sucesso = ref<string | null>(null)
const formulario = ref<FormularioServicoDetalheAdminDto | null>(null)

const formularioForm = reactive({
  nome: '',
  descricao: '',
  ativo: true,
})

const dialogVersaoAberto = ref(false)
const versaoEmEdicao = ref<FormularioServicoVersaoAdminDto | null>(null)
const versaoForm = reactive({
  numero: 1,
  publicada: false,
  publicadoEm: '',
  ativo: true,
})

const dialogCampoAberto = ref(false)
const campoEmEdicao = ref<CampoFormularioServicoAdminDto | null>(null)
const versaoSelecionadaId = ref('')
const campoForm = reactive({
  nome: '',
  rotulo: '',
  tipo: TipoCampoFormularioServico.TextoCurto,
  obrigatorio: false,
  ordem: 1,
  textoAjuda: '',
  visivel: true,
  ativo: true,
})

const dialogOpcaoAberto = ref(false)
const opcaoEmEdicao = ref<OpcaoCampoFormularioServicoAdminDto | null>(null)
const campoSelecionadoId = ref('')
const opcaoForm = reactive({
  valor: '',
  rotulo: '',
  ordem: 1,
  ativo: true,
})

const tiposCampoOptions = [
  { label: 'Texto curto', value: TipoCampoFormularioServico.TextoCurto },
  { label: 'Texto longo', value: TipoCampoFormularioServico.TextoLongo },
  { label: 'Numero', value: TipoCampoFormularioServico.Numero },
  { label: 'Data', value: TipoCampoFormularioServico.Data },
  { label: 'Booleano', value: TipoCampoFormularioServico.Booleano },
  { label: 'Selecao unica', value: TipoCampoFormularioServico.SelecaoUnica },
  { label: 'Selecao multipla', value: TipoCampoFormularioServico.SelecaoMultipla },
]

const possuiFormulario = computed(() => formulario.value !== null)
const versoesOrdenadas = computed(() => [...(formulario.value?.versoes ?? [])].sort((a, b) => a.numero - b.numero))

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

function formatarDataHora(valor: string | null): string {
  if (!valor) {
    return 'Nao publicado'
  }

  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return valor
  }

  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(data)
}

function converterIsoParaInput(valor: string | null): string {
  if (!valor) {
    return ''
  }

  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return ''
  }

  const offset = data.getTimezoneOffset()
  const local = new Date(data.getTime() - offset * 60_000)
  return local.toISOString().slice(0, 16)
}

function converterInputParaIso(valor: string): string | null {
  if (!valor.trim()) {
    return null
  }

  const data = new Date(valor)
  if (Number.isNaN(data.getTime())) {
    return null
  }

  return data.toISOString()
}

function tipoCampoDescricao(tipo: TipoCampoFormularioServico): string {
  return tiposCampoOptions.find((item) => item.value === tipo)?.label ?? `Tipo ${tipo}`
}

function campoAceitaOpcoes(campo: CampoFormularioServicoAdminDto): boolean {
  return campo.tipo === TipoCampoFormularioServico.SelecaoUnica || campo.tipo === TipoCampoFormularioServico.SelecaoMultipla
}

function sincronizarFormularioForm(): void {
  formularioForm.nome = formulario.value?.nome ?? ''
  formularioForm.descricao = formulario.value?.descricao ?? ''
  formularioForm.ativo = formulario.value?.ativo ?? true
}

function limparMensagens(): void {
  erro.value = null
  sucesso.value = null
}

async function carregarFormulario(): Promise<void> {
  if (!props.catalogoServicoId) {
    formulario.value = null
    sincronizarFormularioForm()
    return
  }

  loading.value = true
  limparMensagens()

  try {
    const formularios = await formularioServicosAdminService.listarFormularios({
      catalogoServicoId: props.catalogoServicoId,
    })

    if (!formularios.length) {
      formulario.value = null
      sincronizarFormularioForm()
      return
    }

    formulario.value = await formularioServicosAdminService.obterFormulario(formularios[0].id)
    sincronizarFormularioForm()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel carregar a configuracao de formulario do servico.')
  } finally {
    loading.value = false
  }
}

async function criarOuSalvarFormulario(): Promise<void> {
  if (props.somenteLeitura) {
    return
  }

  salvando.value = true
  limparMensagens()

  try {
    if (!possuiFormulario.value) {
      formulario.value = await formularioServicosAdminService.criarFormulario({
        catalogoServicoId: props.catalogoServicoId,
        nome: formularioForm.nome.trim(),
        descricao: formularioForm.descricao.trim() || null,
        ativo: formularioForm.ativo,
      })
      sucesso.value = 'Formulario criado com sucesso.'
      $q.notify({ type: 'positive', message: sucesso.value })
    } else {
      formulario.value = await formularioServicosAdminService.atualizarFormulario(formulario.value.id, {
        nome: formularioForm.nome.trim(),
        descricao: formularioForm.descricao.trim() || null,
        ativo: formularioForm.ativo,
      })
      sucesso.value = 'Formulario atualizado com sucesso.'
      $q.notify({ type: 'positive', message: sucesso.value })
    }

    sincronizarFormularioForm()
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar o formulario.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

async function alterarSituacaoFormulario(ativar: boolean): Promise<void> {
  if (!formulario.value || props.somenteLeitura) {
    return
  }

  salvando.value = true
  limparMensagens()

  try {
    if (ativar) {
      await formularioServicosAdminService.reativarFormulario(formulario.value.id)
      sucesso.value = 'Formulario reativado com sucesso.'
    } else {
      await formularioServicosAdminService.inativarFormulario(formulario.value.id)
      sucesso.value = 'Formulario inativado com sucesso.'
    }

    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel alterar a situacao do formulario.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

function abrirDialogNovaVersao(): void {
  versaoEmEdicao.value = null
  versaoForm.numero = Math.max(1, ...versoesOrdenadas.value.map((item) => item.numero + 1))
  versaoForm.publicada = false
  versaoForm.publicadoEm = ''
  versaoForm.ativo = true
  dialogVersaoAberto.value = true
}

function abrirDialogEditarVersao(versao: FormularioServicoVersaoAdminDto): void {
  versaoEmEdicao.value = versao
  versaoForm.numero = versao.numero
  versaoForm.publicada = versao.publicada
  versaoForm.publicadoEm = converterIsoParaInput(versao.publicadoEm)
  versaoForm.ativo = versao.ativo
  dialogVersaoAberto.value = true
}

async function salvarVersao(): Promise<void> {
  if (!formulario.value || props.somenteLeitura) {
    return
  }

  salvando.value = true
  limparMensagens()

  try {
    const payload = {
      numero: versaoForm.numero,
      publicada: versaoForm.publicada,
      publicadoEm: converterInputParaIso(versaoForm.publicadoEm),
      ativo: versaoForm.ativo,
    }

    if (versaoEmEdicao.value) {
      await formularioServicosAdminService.atualizarVersao(versaoEmEdicao.value.id, payload)
      sucesso.value = 'Versao atualizada com sucesso.'
    } else {
      await formularioServicosAdminService.criarVersao(formulario.value.id, {
        formularioServicoId: formulario.value.id,
        ...payload,
      })
      sucesso.value = 'Versao criada com sucesso.'
    }

    dialogVersaoAberto.value = false
    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar a versao.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

async function alterarSituacaoVersao(versaoId: string, ativar: boolean): Promise<void> {
  salvando.value = true
  limparMensagens()

  try {
    if (ativar) {
      await formularioServicosAdminService.reativarVersao(versaoId)
      sucesso.value = 'Versao reativada com sucesso.'
    } else {
      await formularioServicosAdminService.inativarVersao(versaoId)
      sucesso.value = 'Versao inativada com sucesso.'
    }

    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel alterar a situacao da versao.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

function abrirDialogNovoCampo(versaoId: string, ordemSugerida = 1): void {
  campoEmEdicao.value = null
  versaoSelecionadaId.value = versaoId
  campoForm.nome = ''
  campoForm.rotulo = ''
  campoForm.tipo = TipoCampoFormularioServico.TextoCurto
  campoForm.obrigatorio = false
  campoForm.ordem = ordemSugerida
  campoForm.textoAjuda = ''
  campoForm.visivel = true
  campoForm.ativo = true
  dialogCampoAberto.value = true
}

function abrirDialogEditarCampo(campo: CampoFormularioServicoAdminDto): void {
  campoEmEdicao.value = campo
  versaoSelecionadaId.value = campo.formularioServicoVersaoId
  campoForm.nome = campo.nome
  campoForm.rotulo = campo.rotulo
  campoForm.tipo = campo.tipo
  campoForm.obrigatorio = campo.obrigatorio
  campoForm.ordem = campo.ordem
  campoForm.textoAjuda = campo.textoAjuda ?? ''
  campoForm.visivel = campo.visivel
  campoForm.ativo = campo.ativo
  dialogCampoAberto.value = true
}

async function salvarCampo(): Promise<void> {
  if (props.somenteLeitura) {
    return
  }

  salvando.value = true
  limparMensagens()

  try {
    const payload = {
      nome: campoForm.nome.trim(),
      rotulo: campoForm.rotulo.trim(),
      tipo: campoForm.tipo,
      obrigatorio: campoForm.obrigatorio,
      ordem: campoForm.ordem,
      textoAjuda: campoForm.textoAjuda.trim() || null,
      visivel: campoForm.visivel,
      ativo: campoForm.ativo,
    }

    if (campoEmEdicao.value) {
      await formularioServicosAdminService.atualizarCampo(campoEmEdicao.value.id, payload)
      sucesso.value = 'Campo atualizado com sucesso.'
    } else {
      await formularioServicosAdminService.criarCampo(versaoSelecionadaId.value, {
        formularioServicoVersaoId: versaoSelecionadaId.value,
        ...payload,
      })
      sucesso.value = 'Campo criado com sucesso.'
    }

    dialogCampoAberto.value = false
    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar o campo.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

async function alterarSituacaoCampo(campoId: string, ativar: boolean): Promise<void> {
  salvando.value = true
  limparMensagens()

  try {
    if (ativar) {
      await formularioServicosAdminService.reativarCampo(campoId)
      sucesso.value = 'Campo reativado com sucesso.'
    } else {
      await formularioServicosAdminService.inativarCampo(campoId)
      sucesso.value = 'Campo inativado com sucesso.'
    }

    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel alterar a situacao do campo.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

function abrirDialogNovaOpcao(campo: CampoFormularioServicoAdminDto): void {
  opcaoEmEdicao.value = null
  campoSelecionadoId.value = campo.id
  opcaoForm.valor = ''
  opcaoForm.rotulo = ''
  opcaoForm.ordem = Math.max(1, ...campo.opcoes.map((item) => item.ordem + 1))
  opcaoForm.ativo = true
  dialogOpcaoAberto.value = true
}

function abrirDialogEditarOpcao(campoId: string, opcao: OpcaoCampoFormularioServicoAdminDto): void {
  opcaoEmEdicao.value = opcao
  campoSelecionadoId.value = campoId
  opcaoForm.valor = opcao.valor
  opcaoForm.rotulo = opcao.rotulo
  opcaoForm.ordem = opcao.ordem
  opcaoForm.ativo = opcao.ativo
  dialogOpcaoAberto.value = true
}

async function salvarOpcao(): Promise<void> {
  if (props.somenteLeitura) {
    return
  }

  salvando.value = true
  limparMensagens()

  try {
    const payload = {
      valor: opcaoForm.valor.trim(),
      rotulo: opcaoForm.rotulo.trim(),
      ordem: opcaoForm.ordem,
      ativo: opcaoForm.ativo,
    }

    if (opcaoEmEdicao.value) {
      await formularioServicosAdminService.atualizarOpcao(opcaoEmEdicao.value.id, payload)
      sucesso.value = 'Opcao atualizada com sucesso.'
    } else {
      await formularioServicosAdminService.criarOpcao(campoSelecionadoId.value, {
        campoFormularioServicoId: campoSelecionadoId.value,
        ...payload,
      })
      sucesso.value = 'Opcao criada com sucesso.'
    }

    dialogOpcaoAberto.value = false
    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel salvar a opcao.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

async function alterarSituacaoOpcao(opcaoId: string, ativar: boolean): Promise<void> {
  salvando.value = true
  limparMensagens()

  try {
    if (ativar) {
      await formularioServicosAdminService.reativarOpcao(opcaoId)
      sucesso.value = 'Opcao reativada com sucesso.'
    } else {
      await formularioServicosAdminService.inativarOpcao(opcaoId)
      sucesso.value = 'Opcao inativada com sucesso.'
    }

    await carregarFormulario()
    $q.notify({ type: 'positive', message: sucesso.value })
  } catch (error) {
    erro.value = extrairMensagemErro(error, 'Nao foi possivel alterar a situacao da opcao.')
    $q.notify({ type: 'negative', message: erro.value })
  } finally {
    salvando.value = false
  }
}

watch(
  () => props.catalogoServicoId,
  async () => {
    await carregarFormulario()
  }
)

onMounted(async () => {
  await carregarFormulario()
})
</script>

<template>
  <AppSectionCard
    titulo="Formulario do servico"
    subtitulo="Configure cabecalho, versoes, campos e opcoes administrativas do formulario vinculado ao catalogo."
  >
    <div class="column q-gutter-md">
      <q-banner v-if="erro" rounded class="bg-red-1 text-negative">
        {{ erro }}
      </q-banner>

      <q-banner v-if="sucesso" rounded class="bg-green-1 text-positive">
        {{ sucesso }}
      </q-banner>

      <q-linear-progress v-if="loading || salvando" indeterminate color="primary" rounded />

      <div class="row items-center justify-between q-col-gutter-md">
        <div class="col-12 col-md">
          <div class="text-subtitle1 text-weight-medium">
            {{ possuiFormulario ? 'Formulario configurado' : 'Nenhum formulario configurado' }}
          </div>
          <div class="text-body2 text-grey-7">
            {{ possuiFormulario ? 'Gerencie a estrutura administrativa do formulario abaixo.' : 'Crie o formulario base deste servico para iniciar a configuracao.' }}
          </div>
        </div>

        <div v-if="possuiFormulario" class="col-12 col-md-auto row q-gutter-sm">
          <q-btn color="primary" icon="save" label="Salvar formulario" :disable="props.somenteLeitura || salvando" @click="criarOuSalvarFormulario" />
          <q-btn
            v-if="formulario?.ativo"
            color="negative"
            flat
            icon="visibility_off"
            label="Inativar"
            :disable="props.somenteLeitura || salvando"
            @click="alterarSituacaoFormulario(false)"
          />
          <q-btn
            v-else
            color="positive"
            flat
            icon="visibility"
            label="Reativar"
            :disable="props.somenteLeitura || salvando"
            @click="alterarSituacaoFormulario(true)"
          />
        </div>
      </div>

      <div class="row q-col-gutter-md">
        <div class="col-12 col-md-6">
          <q-input v-model="formularioForm.nome" outlined dense label="Nome do formulario" :readonly="props.somenteLeitura || loading" />
        </div>

        <div class="col-12 col-md-3">
          <q-toggle v-model="formularioForm.ativo" :disable="props.somenteLeitura || !possuiFormulario" label="Ativo" />
        </div>

        <div class="col-12">
          <q-input
            v-model="formularioForm.descricao"
            outlined
            dense
            autogrow
            type="textarea"
            label="Descricao do formulario"
            :readonly="props.somenteLeitura || loading"
          />
        </div>
      </div>

      <div v-if="!possuiFormulario" class="row justify-end">
        <q-btn color="primary" icon="note_add" label="Criar formulario" :disable="props.somenteLeitura || salvando" @click="criarOuSalvarFormulario" />
      </div>

      <template v-else>
        <div class="row items-center justify-between q-col-gutter-md">
          <div class="col-12 col-md">
            <div class="text-subtitle2 text-weight-medium">Versoes do formulario</div>
            <div class="text-body2 text-grey-7">Cadastre as revisoes estruturais, campos e opcoes associadas.</div>
          </div>
          <div class="col-12 col-md-auto">
            <q-btn color="primary" flat icon="add" label="Nova versao" :disable="props.somenteLeitura || salvando" @click="abrirDialogNovaVersao" />
          </div>
        </div>

        <q-list bordered separator class="rounded-borders">
          <q-expansion-item
            v-for="versao in versoesOrdenadas"
            :key="versao.id"
            expand-separator
            header-class="bg-grey-1"
            :label="`Versao ${versao.numero}`"
            :caption="`${versao.ativo ? 'Ativa' : 'Inativa'} • ${versao.publicada ? `Publicada em ${formatarDataHora(versao.publicadoEm)}` : 'Nao publicada'}`"
          >
            <div class="q-pa-md column q-gutter-md">
              <div class="row items-center justify-between q-col-gutter-sm">
                <div class="col-12 col-md">
                  <q-chip dense :color="versao.ativo ? 'positive' : 'grey-6'" text-color="white">
                    {{ versao.ativo ? 'Ativa' : 'Inativa' }}
                  </q-chip>
                  <q-chip dense :color="versao.publicada ? 'primary' : 'orange-8'" text-color="white">
                    {{ versao.publicada ? 'Publicada' : 'Rascunho' }}
                  </q-chip>
                </div>

                <div class="col-12 col-md-auto row q-gutter-sm">
                  <q-btn flat color="primary" icon="edit" label="Editar versao" :disable="props.somenteLeitura || salvando" @click="abrirDialogEditarVersao(versao)" />
                  <q-btn
                    v-if="versao.ativo"
                    flat
                    color="negative"
                    icon="visibility_off"
                    label="Inativar"
                    :disable="props.somenteLeitura || salvando"
                    @click="alterarSituacaoVersao(versao.id, false)"
                  />
                  <q-btn
                    v-else
                    flat
                    color="positive"
                    icon="visibility"
                    label="Reativar"
                    :disable="props.somenteLeitura || salvando"
                    @click="alterarSituacaoVersao(versao.id, true)"
                  />
                  <q-btn
                    color="primary"
                    unelevated
                    icon="playlist_add"
                    label="Novo campo"
                    :disable="props.somenteLeitura || salvando"
                    @click="abrirDialogNovoCampo(versao.id, Math.max(1, ...versao.campos.map((item) => item.ordem + 1)))"
                  />
                </div>
              </div>

              <q-banner v-if="!versao.campos.length" rounded class="bg-grey-1 text-grey-8">
                Esta versao ainda nao possui campos cadastrados.
              </q-banner>

              <q-list v-else bordered separator class="rounded-borders">
                <q-expansion-item
                  v-for="campo in versao.campos"
                  :key="campo.id"
                  expand-separator
                  :label="`${campo.ordem}. ${campo.rotulo}`"
                  :caption="`${campo.nome} • ${tipoCampoDescricao(campo.tipo)} • ${campo.ativo ? 'Ativo' : 'Inativo'}`"
                >
                  <div class="q-pa-md column q-gutter-md">
                    <div class="row q-col-gutter-sm">
                      <div class="col-12 col-md">
                        <q-chip dense color="primary" text-color="white">{{ tipoCampoDescricao(campo.tipo) }}</q-chip>
                        <q-chip dense :color="campo.obrigatorio ? 'orange-8' : 'grey-6'" text-color="white">
                          {{ campo.obrigatorio ? 'Obrigatorio' : 'Opcional' }}
                        </q-chip>
                        <q-chip dense :color="campo.visivel ? 'positive' : 'grey-7'" text-color="white">
                          {{ campo.visivel ? 'Visivel' : 'Oculto' }}
                        </q-chip>
                      </div>

                      <div class="col-12 col-md-auto row q-gutter-sm">
                        <q-btn flat color="primary" icon="edit" label="Editar campo" :disable="props.somenteLeitura || salvando" @click="abrirDialogEditarCampo(campo)" />
                        <q-btn
                          v-if="campo.ativo"
                          flat
                          color="negative"
                          icon="visibility_off"
                          label="Inativar"
                          :disable="props.somenteLeitura || salvando"
                          @click="alterarSituacaoCampo(campo.id, false)"
                        />
                        <q-btn
                          v-else
                          flat
                          color="positive"
                          icon="visibility"
                          label="Reativar"
                          :disable="props.somenteLeitura || salvando"
                          @click="alterarSituacaoCampo(campo.id, true)"
                        />
                        <q-btn
                          v-if="campoAceitaOpcoes(campo)"
                          color="primary"
                          outline
                          icon="add_circle"
                          label="Nova opcao"
                          :disable="props.somenteLeitura || salvando"
                          @click="abrirDialogNovaOpcao(campo)"
                        />
                      </div>
                    </div>

                    <div class="text-body2 text-grey-8">
                      <strong>Nome tecnico:</strong> {{ campo.nome }}
                    </div>
                    <div class="text-body2 text-grey-8">
                      <strong>Texto de ajuda:</strong> {{ campo.textoAjuda || 'Nao informado' }}
                    </div>

                    <div v-if="campoAceitaOpcoes(campo)" class="column q-gutter-sm">
                      <div class="text-subtitle2 text-weight-medium">Opcoes configuradas</div>

                      <q-banner v-if="!campo.opcoes.length" rounded class="bg-grey-1 text-grey-8">
                        Nenhuma opcao cadastrada para este campo.
                      </q-banner>

                      <q-list v-else bordered separator class="rounded-borders">
                        <q-item v-for="opcao in campo.opcoes" :key="opcao.id">
                          <q-item-section>
                            <q-item-label>{{ opcao.ordem }}. {{ opcao.rotulo }}</q-item-label>
                            <q-item-label caption>{{ opcao.valor }} • {{ opcao.ativo ? 'Ativa' : 'Inativa' }}</q-item-label>
                          </q-item-section>
                          <q-item-section side>
                            <div class="row q-gutter-xs">
                              <q-btn flat round dense color="primary" icon="edit" :disable="props.somenteLeitura || salvando" @click="abrirDialogEditarOpcao(campo.id, opcao)" />
                              <q-btn
                                v-if="opcao.ativo"
                                flat
                                round
                                dense
                                color="negative"
                                icon="visibility_off"
                                :disable="props.somenteLeitura || salvando"
                                @click="alterarSituacaoOpcao(opcao.id, false)"
                              />
                              <q-btn
                                v-else
                                flat
                                round
                                dense
                                color="positive"
                                icon="visibility"
                                :disable="props.somenteLeitura || salvando"
                                @click="alterarSituacaoOpcao(opcao.id, true)"
                              />
                            </div>
                          </q-item-section>
                        </q-item>
                      </q-list>
                    </div>
                  </div>
                </q-expansion-item>
              </q-list>
            </div>
          </q-expansion-item>
        </q-list>
      </template>
    </div>

    <q-dialog v-model="dialogVersaoAberto">
      <q-card style="min-width: 420px; max-width: 96vw">
        <q-card-section>
          <div class="text-h6">{{ versaoEmEdicao ? 'Editar versao' : 'Nova versao' }}</div>
        </q-card-section>
        <q-card-section class="column q-gutter-md">
          <q-input v-model.number="versaoForm.numero" outlined dense type="number" min="1" label="Numero" />
          <q-toggle v-model="versaoForm.publicada" label="Publicada" />
          <q-input v-model="versaoForm.publicadoEm" outlined dense type="datetime-local" label="Publicado em" />
          <q-toggle v-model="versaoForm.ativo" label="Ativa" />
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" :loading="salvando" @click="salvarVersao" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="dialogCampoAberto">
      <q-card style="min-width: 520px; max-width: 96vw">
        <q-card-section>
          <div class="text-h6">{{ campoEmEdicao ? 'Editar campo' : 'Novo campo' }}</div>
        </q-card-section>
        <q-card-section class="column q-gutter-md">
          <q-input v-model="campoForm.nome" outlined dense label="Nome tecnico" />
          <q-input v-model="campoForm.rotulo" outlined dense label="Rotulo" />
          <q-select v-model="campoForm.tipo" outlined dense emit-value map-options :options="tiposCampoOptions" label="Tipo" />
          <q-input v-model.number="campoForm.ordem" outlined dense type="number" min="1" label="Ordem" />
          <q-input v-model="campoForm.textoAjuda" outlined dense autogrow type="textarea" label="Texto de ajuda" />
          <div class="row q-col-gutter-md">
            <div class="col-12 col-md-4">
              <q-toggle v-model="campoForm.obrigatorio" label="Obrigatorio" />
            </div>
            <div class="col-12 col-md-4">
              <q-toggle v-model="campoForm.visivel" label="Visivel" />
            </div>
            <div class="col-12 col-md-4">
              <q-toggle v-model="campoForm.ativo" label="Ativo" />
            </div>
          </div>
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" :loading="salvando" @click="salvarCampo" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-dialog v-model="dialogOpcaoAberto">
      <q-card style="min-width: 420px; max-width: 96vw">
        <q-card-section>
          <div class="text-h6">{{ opcaoEmEdicao ? 'Editar opcao' : 'Nova opcao' }}</div>
        </q-card-section>
        <q-card-section class="column q-gutter-md">
          <q-input v-model="opcaoForm.valor" outlined dense label="Valor" />
          <q-input v-model="opcaoForm.rotulo" outlined dense label="Rotulo" />
          <q-input v-model.number="opcaoForm.ordem" outlined dense type="number" min="1" label="Ordem" />
          <q-toggle v-model="opcaoForm.ativo" label="Ativa" />
        </q-card-section>
        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
          <q-btn color="primary" label="Salvar" :loading="salvando" @click="salvarOpcao" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </AppSectionCard>
</template>
