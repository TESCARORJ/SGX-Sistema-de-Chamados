import type {
  ChamadoAprovacaoAdminResponse,
  ChamadoRelacionamentoAdmin,
  ChamadoTarefaAdminResponse,
} from '../../types/admin'
import {
  StatusAprovacaoChamadoAdminEnum,
  StatusTarefaChamadoEnum,
} from '../../types/admin'

export enum TipoRelacionamentoChamado {
  Relacionado = 1,
  Pai = 2,
  Filho = 3,
  Duplicado = 4,
  Bloqueia = 5,
  BloqueadoPor = 6,
  DerivadoDe = 7,
  Origina = 8,
}

export function chamadoAtualEhOrigem(relacionamento: ChamadoRelacionamentoAdmin, chamadoAtualId: string): boolean {
  return relacionamento.chamadoOrigemId === chamadoAtualId
}

export function obterChamadoRelacionadoId(
  relacionamento: ChamadoRelacionamentoAdmin,
  chamadoAtualId: string
): string {
  return chamadoAtualEhOrigem(relacionamento, chamadoAtualId)
    ? relacionamento.chamadoDestinoId
    : relacionamento.chamadoOrigemId
}

export function obterChamadoRelacionadoCodigo(
  relacionamento: ChamadoRelacionamentoAdmin,
  chamadoAtualId: string
): string {
  return chamadoAtualEhOrigem(relacionamento, chamadoAtualId)
    ? relacionamento.chamadoDestinoCodigo
    : relacionamento.chamadoOrigemCodigo
}

export function obterDescricaoDirecionalRelacionamento(
  relacionamento: ChamadoRelacionamentoAdmin,
  chamadoAtualId: string
): string {
  const atualEhOrigem = chamadoAtualEhOrigem(relacionamento, chamadoAtualId)
  const codigoRelacionado = obterChamadoRelacionadoCodigo(relacionamento, chamadoAtualId)

  switch (relacionamento.tipoRelacionamento) {
    case TipoRelacionamentoChamado.Origina:
      return atualEhOrigem
        ? `Este chamado origina o chamado ${codigoRelacionado}.`
        : `Este chamado foi originado pelo chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.DerivadoDe:
      return atualEhOrigem
        ? `Este chamado foi derivado do chamado ${codigoRelacionado}.`
        : `Este chamado originou o chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.Bloqueia:
      return atualEhOrigem
        ? `Este chamado bloqueia o chamado ${codigoRelacionado}.`
        : `Este chamado esta bloqueado pelo chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.BloqueadoPor:
      return atualEhOrigem
        ? `Este chamado esta bloqueado pelo chamado ${codigoRelacionado}.`
        : `Este chamado bloqueia o chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.Pai:
      return atualEhOrigem
        ? `Este chamado e pai do chamado ${codigoRelacionado}.`
        : `Este chamado e filho do chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.Filho:
      return atualEhOrigem
        ? `Este chamado e filho do chamado ${codigoRelacionado}.`
        : `Este chamado e pai do chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.Duplicado:
      return `Marcado como duplicado em relacao ao chamado ${codigoRelacionado}.`
    case TipoRelacionamentoChamado.Relacionado:
    default:
      return `Relacionado ao chamado ${codigoRelacionado}.`
  }
}

export function obterCorTipoRelacionamento(tipo: number): string {
  switch (tipo) {
    case TipoRelacionamentoChamado.Bloqueia:
    case TipoRelacionamentoChamado.BloqueadoPor:
      return 'negative'
    case TipoRelacionamentoChamado.Origina:
    case TipoRelacionamentoChamado.DerivadoDe:
      return 'primary'
    case TipoRelacionamentoChamado.Duplicado:
      return 'warning'
    case TipoRelacionamentoChamado.Pai:
    case TipoRelacionamentoChamado.Filho:
      return 'secondary'
    default:
      return 'grey-7'
  }
}

export function obterIconeTipoRelacionamento(tipo: number): string {
  switch (tipo) {
    case TipoRelacionamentoChamado.Bloqueia:
    case TipoRelacionamentoChamado.BloqueadoPor:
      return 'lock'
    case TipoRelacionamentoChamado.Origina:
    case TipoRelacionamentoChamado.DerivadoDe:
      return 'call_split'
    case TipoRelacionamentoChamado.Duplicado:
      return 'content_copy'
    case TipoRelacionamentoChamado.Pai:
    case TipoRelacionamentoChamado.Filho:
      return 'account_tree'
    default:
      return 'link'
  }
}

export interface ResumoBloqueiosChamado {
  bloqueadoPor: ChamadoRelacionamentoAdmin[]
  bloqueiaOutros: ChamadoRelacionamentoAdmin[]
  possuiBloqueio: boolean
}

export interface ResumoDerivacoesChamado {
  origem: ChamadoRelacionamentoAdmin[]
  originados: ChamadoRelacionamentoAdmin[]
  possuiDerivacao: boolean
}

export interface ResumoTarefasChamado {
  total: number
  pendentes: number
  emAndamento: number
  concluidas: number
  canceladas: number
  proximas: ChamadoTarefaAdminResponse[]
}

export interface ResumoAprovacoesChamado {
  total: number
  pendentes: number
  pendentesBloqueantes: number
  aprovadas: number
  reprovadas: number
  canceladas: number
  listaPendentes: ChamadoAprovacaoAdminResponse[]
}

export function obterResumoBloqueiosChamado(
  relacionamentos: ChamadoRelacionamentoAdmin[],
  chamadoAtualId: string
): ResumoBloqueiosChamado {
  const bloqueadoPor: ChamadoRelacionamentoAdmin[] = []
  const bloqueiaOutros: ChamadoRelacionamentoAdmin[] = []

  relacionamentos
    .filter((item) => item.ativo)
    .forEach((item) => {
      const atualEhOrigem = chamadoAtualEhOrigem(item, chamadoAtualId)

      if (item.tipoRelacionamento === TipoRelacionamentoChamado.Bloqueia) {
        ;(atualEhOrigem ? bloqueiaOutros : bloqueadoPor).push(item)
      }

      if (item.tipoRelacionamento === TipoRelacionamentoChamado.BloqueadoPor) {
        ;(atualEhOrigem ? bloqueadoPor : bloqueiaOutros).push(item)
      }
    })

  return {
    bloqueadoPor,
    bloqueiaOutros,
    possuiBloqueio: bloqueadoPor.length > 0 || bloqueiaOutros.length > 0,
  }
}

export function obterResumoDerivacoesChamado(
  relacionamentos: ChamadoRelacionamentoAdmin[],
  chamadoAtualId: string
): ResumoDerivacoesChamado {
  const origem: ChamadoRelacionamentoAdmin[] = []
  const originados: ChamadoRelacionamentoAdmin[] = []

  relacionamentos
    .filter((item) => item.ativo)
    .forEach((item) => {
      const atualEhOrigem = chamadoAtualEhOrigem(item, chamadoAtualId)

      if (item.tipoRelacionamento === TipoRelacionamentoChamado.Origina) {
        ;(atualEhOrigem ? originados : origem).push(item)
      }

      if (item.tipoRelacionamento === TipoRelacionamentoChamado.DerivadoDe) {
        ;(atualEhOrigem ? origem : originados).push(item)
      }
    })

  return {
    origem,
    originados,
    possuiDerivacao: origem.length > 0 || originados.length > 0,
  }
}

export function obterResumoTarefasChamado(tarefas: ChamadoTarefaAdminResponse[]): ResumoTarefasChamado {
  const proximas = tarefas
    .filter((item) => item.ativo && item.status !== StatusTarefaChamadoEnum.Concluida && item.status !== StatusTarefaChamadoEnum.Cancelada)
    .sort((a, b) => {
      if (!a.prazo && !b.prazo) return 0
      if (!a.prazo) return 1
      if (!b.prazo) return -1
      return new Date(a.prazo).getTime() - new Date(b.prazo).getTime()
    })
    .slice(0, 3)

  return {
    total: tarefas.length,
    pendentes: tarefas.filter((item) => item.ativo && item.status === StatusTarefaChamadoEnum.Pendente).length,
    emAndamento: tarefas.filter((item) => item.ativo && item.status === StatusTarefaChamadoEnum.EmAndamento).length,
    concluidas: tarefas.filter((item) => item.status === StatusTarefaChamadoEnum.Concluida).length,
    canceladas: tarefas.filter((item) => item.status === StatusTarefaChamadoEnum.Cancelada || !item.ativo).length,
    proximas,
  }
}

export function obterResumoAprovacoesChamado(
  aprovacoes: ChamadoAprovacaoAdminResponse[]
): ResumoAprovacoesChamado {
  const pendentes = aprovacoes.filter(
    (item) => item.ativo && item.status === StatusAprovacaoChamadoAdminEnum.Pendente
  )

  return {
    total: aprovacoes.length,
    pendentes: pendentes.length,
    pendentesBloqueantes: pendentes.filter((item) => item.bloqueiaAvancoAtendimento).length,
    aprovadas: aprovacoes.filter((item) => item.status === StatusAprovacaoChamadoAdminEnum.Aprovado).length,
    reprovadas: aprovacoes.filter((item) => item.status === StatusAprovacaoChamadoAdminEnum.Reprovado).length,
    canceladas: aprovacoes.filter((item) => item.status === StatusAprovacaoChamadoAdminEnum.Cancelado || !item.ativo).length,
    listaPendentes: pendentes.slice(0, 3),
  }
}

export function obterCorStatusTarefa(status: StatusTarefaChamadoEnum): string {
  switch (status) {
    case StatusTarefaChamadoEnum.Pendente:
      return 'warning'
    case StatusTarefaChamadoEnum.EmAndamento:
      return 'info'
    case StatusTarefaChamadoEnum.Concluida:
      return 'positive'
    case StatusTarefaChamadoEnum.Cancelada:
      return 'grey-6'
    default:
      return 'grey-7'
  }
}

export function obterCorStatusAprovacao(aprovacao: ChamadoAprovacaoAdminResponse): string {
  if (
    aprovacao.ativo &&
    aprovacao.status === StatusAprovacaoChamadoAdminEnum.Pendente &&
    aprovacao.bloqueiaAvancoAtendimento
  ) {
    return 'negative'
  }

  switch (aprovacao.status) {
    case StatusAprovacaoChamadoAdminEnum.Pendente:
      return 'warning'
    case StatusAprovacaoChamadoAdminEnum.Aprovado:
      return 'positive'
    case StatusAprovacaoChamadoAdminEnum.Reprovado:
      return 'negative'
    case StatusAprovacaoChamadoAdminEnum.Cancelado:
      return 'grey-6'
    default:
      return 'grey-7'
  }
}
