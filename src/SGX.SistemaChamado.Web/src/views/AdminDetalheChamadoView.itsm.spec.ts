import { readFileSync } from 'node:fs'
import { describe, expect, it } from 'vitest'

describe('AdminDetalheChamadoView - ITSM', () => {
  it('deve exibir natureza, impacto, urgencia e prioridade no detalhe administrativo', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Natureza ITSM')
    expect(fonte).toContain('Impacto')
    expect(fonte).toContain('Urgencia')
    expect(fonte).toContain('Prioridade')
  })

  it('deve exibir grupo tecnico e fila com fallback para chamados legados', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('Grupo tecnico')
    expect(fonte).toContain('Fila de atendimento')
    expect(fonte).toContain("detalhe.grupoTecnicoNome || 'Sem grupo tecnico'")
    expect(fonte).toContain("detalhe.filaAtendimentoNome || 'Sem fila'")
  })

  it('deve consumir acoes disponiveis vindas do backend', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const acoesDisponiveisSet = computed(() => new Set(detalhe.value?.acoesDisponiveisCodigos ?? []))')
    expect(fonte).toContain(':can-alterar-status="podeAlterarStatus"')
    expect(fonte).toContain(':can-encerrar="podeEncerrar"')
    expect(fonte).toContain(':can-reabrir="podeReabrir"')
  })

  it('deve permitir assumir chamado pela fila com visibilidade basica e payload do usuario autenticado', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const podeAssumirFila = computed(')
    expect(fonte).toContain('Boolean(detalhe.value?.grupoTecnicoId)')
    expect(fonte).toContain('Boolean(detalhe.value?.filaAtendimentoId)')
    expect(fonte).toContain('!detalhe.value?.responsavel')
    expect(fonte).toContain(':can-assumir-fila="podeAssumirFila"')
    expect(fonte).toContain('@assumir-fila="showConfirmarAssumirFila = true"')
    expect(fonte).toContain('adminService.assumirChamadoFila(detalhe.value.id, { usuarioId: contexto.value.usuario.id })')
    expect(fonte).toContain('Chamado assumido da fila com sucesso.')
  })

  it('deve esconder assumir fila quando faltar grupo, fila, responsavel livre ou perfil permitido', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')
    const componentePainel = readFileSync(new URL('../components/admin/PainelAtendimento.vue', import.meta.url), 'utf-8')

    expect(fonte).toContain('Boolean(detalhe.value?.grupoTecnicoId)')
    expect(fonte).toContain('Boolean(detalhe.value?.filaAtendimentoId)')
    expect(fonte).toContain('!detalhe.value?.responsavel')
    expect(fonte).toContain('(usuarioEhAdministrador.value || usuarioEhAtendente.value)')
    expect(componentePainel).toContain('v-if="canAssumirFila"')
  })

  it('deve recarregar detalhe e exibir erro da API ao assumir fila sem atualizar responsavel localmente', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('await adminService.assumirChamadoFila(detalhe.value.id, { usuarioId: contexto.value.usuario.id })')
    expect(fonte).toContain('await recarregarDetalhe()')
    expect(fonte).toContain("registrarErro(error, 'Nao foi possivel assumir o chamado da fila.')")
    expect(fonte).not.toContain('detalhe.value.responsavel =')
    expect(fonte).not.toContain('detalhe.value.responsavelId =')
  })

  it('deve recarregar detalhe ao atribuir responsavel sem mutacao local direta', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('async function atribuir(responsavelId: string): Promise<void> {')
    expect(fonte).toContain('await adminService.atribuirChamado(detalhe.value.id, { responsavelId })')
    expect(fonte).toContain('await recarregarDetalhe()')
    expect(fonte).not.toContain('detalhe.value.responsavel =')
    expect(fonte).not.toContain('detalhe.value.responsavelId =')
  })

  it('deve permitir transferir chamado para outro grupo tecnico com fila opcional', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')
    const componentePainel = readFileSync(new URL('../components/admin/PainelAtendimento.vue', import.meta.url), 'utf-8')

    expect(fonte).toContain('const podeTransferirGrupoTecnico = computed(')
    expect(fonte).toContain('Boolean(detalhe.value?.grupoTecnicoId)')
    expect(fonte).toContain('(usuarioEhAdministrador.value || usuarioEhAtendente.value)')
    expect(fonte).toContain(':can-transferir-grupo="podeTransferirGrupoTecnico"')
    expect(fonte).toContain('@transferir-grupo="abrirTransferenciaGrupo"')
    expect(componentePainel).toContain('v-if="canTransferirGrupo"')
    expect(fonte).toContain('Transferir chamado para outro grupo tecnico')
    expect(fonte).toContain('Grupo tecnico de destino')
    expect(fonte).toContain('Fila de destino (opcional)')
    expect(fonte).toContain(":rules=\"[(v) => !!v || 'Selecione o grupo tecnico de destino']\"")
    expect(fonte).toContain("registrarErro(new Error('Selecione o grupo tecnico de destino.')")
    expect(fonte).toContain('const grupoDestinoIgualAtual = computed(')
    expect(fonte).toContain("registrarErro(new Error('Selecione um grupo tecnico diferente do grupo atual.')")
    expect(fonte).toContain('adminService.listarGruposTecnicos({')
    expect(fonte).toContain('adminService.listarFilasAtendimentoGrupoTecnico(grupoTecnicoId, { ativo: true })')
    expect(fonte).toContain('transferenciaGrupoForm.filaAtendimentoId = \'\'')
    expect(fonte).toContain(':disable="loadingGruposTransferencia || loadingFilasTransferencia || !transferenciaGrupoForm.grupoTecnicoId || grupoDestinoIgualAtual"')
    expect(fonte).toContain('adminService.transferirGrupoTecnicoChamado(detalhe.value.id, {')
    expect(fonte).toContain('grupoTecnicoId: transferenciaGrupoForm.grupoTecnicoId')
    expect(fonte).toContain('filaAtendimentoId: transferenciaGrupoForm.filaAtendimentoId || null')
    expect(fonte).toContain('await recarregarDetalhe()')
    expect(fonte).toContain("registrarErro(error, 'Nao foi possivel transferir o chamado para outro grupo tecnico.')")
    expect(fonte).not.toContain('detalhe.value.responsavel =')
    expect(fonte).not.toContain('detalhe.value.grupoTecnicoId =')
    expect(fonte).not.toContain('detalhe.value.filaAtendimentoId =')
    expect(fonte).toContain('Chamado transferido para outro grupo tecnico com sucesso.')
  })

  it('deve filtrar visualmente status pelo conjunto permitido e manter tratamento de erro da API', () => {
    const caminho = new URL('./AdminDetalheChamadoView.vue', import.meta.url)
    const fonte = readFileSync(caminho, 'utf-8')

    expect(fonte).toContain('const statusDisponiveisParaNatureza = computed(() => {')
    expect(fonte).toContain(':status="statusDisponiveisParaNatureza"')
    expect(fonte).toContain("registrarErro(error, 'Não foi possível concluir a ação.')")
    expect(fonte).toContain("A ação Encerrar não está disponível para este chamado no estado atual.")
  })
})
