import { describe, expect, it } from 'vitest'
import type {
  ChamadoAprovacaoAdminResponse,
  ChamadoRelacionamentoAdmin,
  ChamadoTarefaAdminResponse,
} from '../../types/admin'
import {
  StatusAprovacaoChamadoAdminEnum,
  StatusTarefaChamadoEnum,
} from '../../types/admin'
import {
  TipoRelacionamentoChamado,
  obterChamadoRelacionadoCodigo,
  obterChamadoRelacionadoId,
  obterCorStatusAprovacao,
  obterCorStatusTarefa,
  obterCorTipoRelacionamento,
  obterDescricaoDirecionalRelacionamento,
  obterIconeTipoRelacionamento,
  obterResumoAprovacoesChamado,
  obterResumoBloqueiosChamado,
  obterResumoDerivacoesChamado,
  obterResumoTarefasChamado,
} from './chamadoRelacionamentosPresentation'

function relacionamento(tipoRelacionamento: TipoRelacionamentoChamado): ChamadoRelacionamentoAdmin {
  return {
    id: 'rel-1',
    chamadoOrigemId: 'ch-origem',
    chamadoOrigemCodigo: 'CHM-001',
    chamadoDestinoId: 'ch-destino',
    chamadoDestinoCodigo: 'CHM-002',
    tipoRelacionamento,
    tipoRelacionamentoDescricao: TipoRelacionamentoChamado[tipoRelacionamento],
    justificativa: 'Justificativa de teste',
    ativo: true,
    criadoEm: '2026-06-03T12:00:00Z',
    criadoPor: 'admin',
    removidoEm: null,
    motivoRemocao: null,
  }
}

function tarefa(status: StatusTarefaChamadoEnum, ativo = true): ChamadoTarefaAdminResponse {
  return {
    id: `tar-${status}-${ativo}`,
    chamadoId: 'ch-origem',
    titulo: `Tarefa ${status}`,
    descricao: null,
    status,
    statusDescricao: StatusTarefaChamadoEnum[status],
    responsavelUsuarioId: null,
    responsavelNome: null,
    prazo: status === StatusTarefaChamadoEnum.Pendente ? '2026-06-03T12:00:00Z' : null,
    criadoEm: '2026-06-03T12:00:00Z',
    criadoPor: 'admin',
    atualizadoEm: null,
    concluidoEm: null,
    canceladoEm: null,
    motivoCancelamento: null,
    ativo,
  }
}

function aprovacao(
  status: StatusAprovacaoChamadoAdminEnum,
  bloqueiaAvancoAtendimento = false,
  ativo = true
): ChamadoAprovacaoAdminResponse {
  return {
    id: `apr-${status}-${bloqueiaAvancoAtendimento}-${ativo}`,
    chamadoId: 'ch-origem',
    titulo: `Aprovacao ${status}`,
    descricao: null,
    status,
    statusDescricao: StatusAprovacaoChamadoAdminEnum[status],
    aprovadorUsuarioId: null,
    aprovadorNome: null,
    solicitadoPorUsuarioId: null,
    solicitadoPorNome: null,
    justificativaSolicitacao: null,
    justificativaDecisao: null,
    bloqueiaAvancoAtendimento,
    solicitadaEm: '2026-06-03T12:00:00Z',
    decididoEm: null,
    canceladoEm: null,
    motivoCancelamento: null,
    criadoEm: '2026-06-03T12:00:00Z',
    ativo,
  }
}

describe('chamadoRelacionamentosPresentation', () => {
  it('deve identificar chamado relacionado quando atual e origem ou destino', () => {
    const item = relacionamento(TipoRelacionamentoChamado.Relacionado)

    expect(obterChamadoRelacionadoId(item, 'ch-origem')).toBe('ch-destino')
    expect(obterChamadoRelacionadoCodigo(item, 'ch-origem')).toBe('CHM-002')
    expect(obterChamadoRelacionadoId(item, 'ch-destino')).toBe('ch-origem')
    expect(obterChamadoRelacionadoCodigo(item, 'ch-destino')).toBe('CHM-001')
  })

  it('deve descrever Origina conforme direcao', () => {
    const item = relacionamento(TipoRelacionamentoChamado.Origina)

    expect(obterDescricaoDirecionalRelacionamento(item, 'ch-origem')).toBe(
      'Este chamado origina o chamado CHM-002.'
    )
    expect(obterDescricaoDirecionalRelacionamento(item, 'ch-destino')).toBe(
      'Este chamado foi originado pelo chamado CHM-001.'
    )
  })

  it('deve descrever Bloqueia e BloqueadoPor conforme direcao', () => {
    const bloqueia = relacionamento(TipoRelacionamentoChamado.Bloqueia)
    const bloqueadoPor = relacionamento(TipoRelacionamentoChamado.BloqueadoPor)

    expect(obterDescricaoDirecionalRelacionamento(bloqueia, 'ch-origem')).toBe(
      'Este chamado bloqueia o chamado CHM-002.'
    )
    expect(obterDescricaoDirecionalRelacionamento(bloqueia, 'ch-destino')).toBe(
      'Este chamado esta bloqueado pelo chamado CHM-001.'
    )
    expect(obterDescricaoDirecionalRelacionamento(bloqueadoPor, 'ch-origem')).toBe(
      'Este chamado esta bloqueado pelo chamado CHM-002.'
    )
    expect(obterDescricaoDirecionalRelacionamento(bloqueadoPor, 'ch-destino')).toBe(
      'Este chamado bloqueia o chamado CHM-001.'
    )
  })

  it('deve descrever DerivadoDe, Relacionado e Duplicado', () => {
    expect(obterDescricaoDirecionalRelacionamento(relacionamento(TipoRelacionamentoChamado.DerivadoDe), 'ch-origem')).toBe(
      'Este chamado foi derivado do chamado CHM-002.'
    )
    expect(obterDescricaoDirecionalRelacionamento(relacionamento(TipoRelacionamentoChamado.Relacionado), 'ch-origem')).toBe(
      'Relacionado ao chamado CHM-002.'
    )
    expect(obterDescricaoDirecionalRelacionamento(relacionamento(TipoRelacionamentoChamado.Duplicado), 'ch-origem')).toBe(
      'Marcado como duplicado em relacao ao chamado CHM-002.'
    )
  })

  it('deve definir apresentacao visual por tipo', () => {
    expect(obterCorTipoRelacionamento(TipoRelacionamentoChamado.Bloqueia)).toBe('negative')
    expect(obterIconeTipoRelacionamento(TipoRelacionamentoChamado.Origina)).toBe('call_split')
    expect(obterIconeTipoRelacionamento(TipoRelacionamentoChamado.Relacionado)).toBe('link')
  })

  it('deve resumir bloqueios conforme a direcao do chamado atual', () => {
    const bloqueia = relacionamento(TipoRelacionamentoChamado.Bloqueia)
    const bloqueadoPor = relacionamento(TipoRelacionamentoChamado.BloqueadoPor)

    const resumo = obterResumoBloqueiosChamado([bloqueia, bloqueadoPor], 'ch-origem')

    expect(resumo.bloqueiaOutros).toHaveLength(1)
    expect(resumo.bloqueadoPor).toHaveLength(1)
    expect(resumo.possuiBloqueio).toBe(true)
  })

  it('deve resumir derivacoes originadas e recebidas', () => {
    const origina = relacionamento(TipoRelacionamentoChamado.Origina)
    const derivadoDe = relacionamento(TipoRelacionamentoChamado.DerivadoDe)

    const resumo = obterResumoDerivacoesChamado([origina, derivadoDe], 'ch-origem')

    expect(resumo.originados).toHaveLength(1)
    expect(resumo.origem).toHaveLength(1)
    expect(resumo.possuiDerivacao).toBe(true)
  })

  it('deve resumir tarefas por status e listar proximas pendencias', () => {
    const resumo = obterResumoTarefasChamado([
      tarefa(StatusTarefaChamadoEnum.Pendente),
      tarefa(StatusTarefaChamadoEnum.EmAndamento),
      tarefa(StatusTarefaChamadoEnum.Concluida),
      tarefa(StatusTarefaChamadoEnum.Cancelada, false),
    ])

    expect(resumo.pendentes).toBe(1)
    expect(resumo.emAndamento).toBe(1)
    expect(resumo.concluidas).toBe(1)
    expect(resumo.canceladas).toBe(1)
    expect(resumo.proximas).toHaveLength(2)
    expect(obterCorStatusTarefa(StatusTarefaChamadoEnum.Concluida)).toBe('positive')
  })

  it('deve resumir aprovacoes pendentes e destacar bloqueantes', () => {
    const pendenteBloqueante = aprovacao(StatusAprovacaoChamadoAdminEnum.Pendente, true)
    const pendenteInformativa = aprovacao(StatusAprovacaoChamadoAdminEnum.Pendente, false)
    const aprovada = aprovacao(StatusAprovacaoChamadoAdminEnum.Aprovado)

    const resumo = obterResumoAprovacoesChamado([pendenteBloqueante, pendenteInformativa, aprovada])

    expect(resumo.pendentes).toBe(2)
    expect(resumo.pendentesBloqueantes).toBe(1)
    expect(resumo.aprovadas).toBe(1)
    expect(resumo.listaPendentes).toHaveLength(2)
    expect(obterCorStatusAprovacao(pendenteBloqueante)).toBe('negative')
    expect(obterCorStatusAprovacao(pendenteInformativa)).toBe('warning')
  })
})
