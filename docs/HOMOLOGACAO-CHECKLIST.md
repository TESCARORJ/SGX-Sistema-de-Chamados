# Checklist de Homologacao - GETI.SistemaChamado

## 1. Configuracao obrigatoria de ambiente
- [ ] Java 21 instalado.
- [ ] Node 20+ e npm 10+ instalados.
- [ ] PostgreSQL acessivel com base `chamados_geti`.
- [ ] `SPRING_PROFILES_ACTIVE` configurado para API e Worker.
- [ ] `APP_SECURITY_OAUTH2_ISSUER_URI` configurada em `hml` (API).
- [ ] Credenciais IMAP configuradas no Worker (`APP_WORKER_EMAIL_IMAP_*`).
- [ ] Dependencias externas acessiveis:
  - [ ] AD/Azure (issuer JWT)
  - [ ] Servidor IMAP corporativo

## 2. Build e qualidade
- [ ] Backend: `mvn clean verify` sem falhas.
- [ ] Frontend: `npm run lint` sem falhas.
- [ ] Frontend: `npm run build` sem falhas.

## 3. Banco e migracoes
- [ ] Flyway aplicado ate a ultima versao (`V9`).
- [ ] Sem drift entre entidade e banco (`ddl-auto=validate`).
- [ ] Sem necessidade de migration pendente da sprint.

## 4. Validacao funcional minima
- [ ] Login/autorizacao por perfil funcionando.
- [ ] CRUD de departamentos funcionando no admin.
- [ ] CRUD de caixas de e-mail funcionando no admin.
- [ ] Abertura manual de chamado no portal funcionando.
- [ ] Operacao administrativa do chamado funcionando.
- [ ] Abertura automatica por e-mail funcionando.
- [ ] Correlacao de resposta por e-mail criando interacao (sem novo chamado indevido).
- [ ] SLA calculado no chamado (prazo e estado dentro/proximo/vencido).
- [ ] Automacoes operacionais iniciais registrando historico.

## 5. Seguranca e permissao
- [ ] Rotas admin exigindo perfis internos corretos.
- [ ] Endpoints tecnicos desabilitados fora de local (`app.api.expor-endpoints-tecnicos=false`).
- [ ] Actuator exposto apenas com `health` e `info`.
- [ ] Backend permanecendo como fonte de verdade da autorizacao.

## 6. Observabilidade e operacao
- [ ] Logs de excecao com metodo/URI e contexto minimo.
- [ ] Logs do worker com resumo por ciclo (contadores de processamento).
- [ ] Logs de integracao por e-mail rastreaveis por `messageId` e caixa.

## 7. Pontos de atencao conhecidos
- [ ] `local` usa autenticacao tecnica por header apenas para desenvolvimento.
- [ ] Integracao AD/Azure depende de issuer e claims corporativos corretos.
- [ ] Integracao IMAP depende de politica de rede e credenciais do ambiente.
- [ ] Cenarios de carga alta ainda exigem avaliacao de capacidade em homologacao.
