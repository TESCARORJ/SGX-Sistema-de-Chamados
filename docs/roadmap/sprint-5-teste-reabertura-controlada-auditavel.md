# Teste: Reabertura Controlada e Auditável

**Sprint:** 5 - Regras de fechamento, aceite e reabertura
**Item:** 26
**Status:** Concluído

## Objetivo
Criar e reforçar testes automatizados que comprovem o comportamento da reabertura controlada por política de prazo limitante administrativo, incluindo a auditoria desse evento.

## Validações Implementadas

Foram implementados testes na classe `ReabrirChamadoUseCaseTests.cs` para validar os seguintes cenários:

1. **Bloqueio após prazo máximo (Reabertura por política negada)**:
   - Uma vez que o chamado está encerrado, o tempo de encerramento (`EncerradoEm`) começa a contar.
   - O teste simulou a tentativa de reabertura após o transcurso do tempo padrão do sistema (168 horas / 7 dias).
   - O uso do caso resultou na recusa da operação (`InvalidOperationException`) reportando o limite ultrapassado.

2. **Permissão dentro do prazo com configuração administrativa distinta**:
   - Um registro do tipo `ParametroSistema` foi mockado no EF Core em memória (com id configurado via Reflection) definindo um tempo administrativo estrito (48 horas).
   - O teste verificou que o sistema carregou corretamente este parâmetro e autorizou a reabertura no limite de 47 horas.
   - O chamado foi retornado ao estado de `Em Atendimento` sem erros.

3. **Geração de Auditoria de Reabertura (Existente/Reforçado)**:
   - Foi validado que o histórico relata com precisão o fato da reabertura ocorrer dentro da "Política aplicada" e que auditorias de nível de sistema rastreiam a ação para governança de encerramentos.

## Conclusão
O sistema é capaz de aplicar as políticas de expiração de reabertura (por padrão ou por variável de banco do administrador) de forma hermética, controlando chamados que não podem mais ser reabertos sem intervenção superior. O escopo do Item 26 está integralmente testado e aderente ao design do domínio da plataforma.
