# Sprint 3 - Regra para atribuir chamado a tecnico especifico

## Objetivo

Validar e ajustar a regra de atribuicao manual de chamado a um tecnico especifico, mantendo o fluxo legado de `AtribuirChamadoUseCase` e adicionando as validacoes necessarias para chamados vinculados a grupo tecnico.

## Decisao tecnica

Nao foi criado um novo use case de atribuicao. O use case existente `AtribuirChamadoUseCase` foi mantido como regra administrativa de atribuicao manual, porque ele ja define `ResponsavelId`, preserva os vinculos do chamado e registra historico de responsavel.

Quando o chamado nao possui `GrupoTecnicoId`, permanece o comportamento legado: o administrador pode atribuir o chamado diretamente a um usuario ativo com perfil de atendimento.

Quando o chamado possui `GrupoTecnicoId`, a atribuicao passa a exigir:

- grupo tecnico do chamado ativo;
- tecnico destino ativo e com perfil de atendimento;
- tecnico destino como membro ativo do grupo tecnico do chamado;
- fila ativa e pertencente ao mesmo grupo tecnico, quando `FilaAtendimentoId` estiver preenchido.

## Diferenca entre atribuir e assumir chamado da fila

Atribuir chamado a tecnico especifico e uma operacao administrativa: um administrador define o tecnico responsavel pelo atendimento.

Assumir chamado da fila e uma operacao do proprio tecnico: o usuario assume para si um chamado que esta em grupo/fila.

Ambas preenchem `ResponsavelId`, mas a atribuicao manual pode indicar outro tecnico e agora respeita a associacao do chamado com grupo tecnico.

## Regras confirmadas

- `ResponsavelId`: atualizado para o tecnico destino informado.
- `GrupoTecnicoId`: preservado; atribuicao nao transfere grupo.
- `FilaAtendimentoId`: preservado; atribuicao nao transfere nem limpa fila.
- Chamado sem grupo: segue atribuicao legada direta.
- Chamado com grupo: exige tecnico membro ativo do grupo.
- Chamado com fila: exige fila ativa e pertencente ao grupo do chamado.
- Reatribuicao: permitida quando o tecnico destino atende as mesmas regras.

## Historico

O historico existente `TipoHistoricoChamado.ResponsavelAlterado` foi preservado para registrar a atribuicao manual do responsavel.

Nao foi criado novo tipo de historico nesta etapa.

## Testes

Foram ajustados e adicionados testes em `AtribuirChamadoUseCaseTests` cobrindo:

- atribuicao legada de chamado sem grupo;
- atribuicao de chamado com grupo para tecnico membro ativo;
- rejeicao de tecnico fora do grupo;
- rejeicao de vinculo inativo no grupo;
- rejeicao de grupo tecnico inativo;
- rejeicao de fila inativa;
- rejeicao de fila pertencente a outro grupo;
- preservacao de `GrupoTecnicoId` e `FilaAtendimentoId`;
- atualizacao de `ResponsavelId`;
- reatribuicao para outro membro ativo;
- historico `ResponsavelAlterado`.

## Fora do escopo

Nao foram criados controller, endpoint publico, tela Vue, service frontend, dashboard, relatorio, regra de SLA, roteamento automatico, transferencia entre filas ou migration estrutural.

## Roadmap

O checklist da Sprint 3 foi atualizado marcando somente o item "Criar regra para atribuir chamado a tecnico especifico" como concluido.

Com 22 itens concluidos de 54 ativos, o percentual esperado da Sprint 3 passa para aproximadamente 41%.

## Proxima etapa recomendada

Criar historico/auditoria das movimentacoes, consolidando os eventos de grupo, fila, atribuicao e assuncao em trilha consultavel.
