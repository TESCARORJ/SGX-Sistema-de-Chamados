from __future__ import annotations

from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[2]
SEED_PATH = ROOT / "src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs"
TEST_PATH = ROOT / "tests/SGX.SistemaChamado.Tests/RoadmapSprint9GerenciamentoIncidentesChecklistTests.cs"
DOC_PATH = ROOT / "docs/roadmap/sprint-9-gerenciamento-incidentes.md"
ROADMAP_PATH = ROOT / "docs/ROADMAP.md"
ROADMAP_ITSM_PATH = ROOT / "docs/ROADMAP-ITSM.md"

CHECKLIST = [
    ('Planejamento', 'Diagnosticar estado atual dos chamados operacionais e capacidades ja reutilizaveis para incidentes', 'Planejamento', True),
    ('Planejamento', 'Confirmar escopo da Sprint 9 sem incluir problema, mudanca ou requisicao', 'Planejamento', True),
    ('Planejamento', 'Definir criterios de aceite do fluxo de incidente', 'Planejamento', True),
    ('Planejamento', 'Documentar diferenca entre incidente, requisicao e chamado legado', 'Documentacao', True),
    ('Modelagem', 'Confirmar reutilizacao de NaturezaChamadoEnum.Incidente sem duplicar enum ou campo', 'Desenvolvimento', True),
    ('Modelagem', 'Definir campos especificos de incidente sem duplicar dados existentes', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar servico afetado', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar CI afetado como vinculo preparatorio enquanto a CMDB nao estiver funcional', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar causa provavel sem introduzir fluxo de Problema', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar diagnostico inicial do incidente', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar solucao de contorno do incidente', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar resolucao do incidente', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar regra de reabertura do incidente', 'Desenvolvimento', False),
    ('Modelagem', 'Modelar regra de fechamento do incidente', 'Desenvolvimento', False),
    ('Modelagem', 'Definir compatibilidade dos status atuais com o ciclo de incidente', 'Desenvolvimento', False),
    ('Backend', 'Ajustar DTOs de abertura para incidente', 'Desenvolvimento', False),
    ('Backend', 'Criar validators dedicados para dados de incidente', 'Desenvolvimento', False),
    ('Backend', 'Ajustar use case de abertura de incidente preservando abertura legada', 'Desenvolvimento', False),
    ('Backend', 'Aplicar classificacao de incidente sem interferir em requisicoes e catalogo', 'Desenvolvimento', False),
    ('Backend', 'Aplicar prioridade de incidente por impacto e urgencia', 'Desenvolvimento', False),
    ('Backend', 'Ajustar use case de atendimento de incidente', 'Desenvolvimento', False),
    ('Backend', 'Registrar historico de diagnostico do incidente', 'Desenvolvimento', False),
    ('Backend', 'Registrar historico de solucao de contorno', 'Desenvolvimento', False),
    ('Backend', 'Ajustar use case de resolucao de incidente', 'Desenvolvimento', False),
    ('Backend', 'Registrar historico de resolucao do incidente', 'Desenvolvimento', False),
    ('Backend', 'Ajustar use case de reabertura de incidente', 'Desenvolvimento', False),
    ('Backend', 'Ajustar use case de fechamento de incidente', 'Desenvolvimento', False),
    ('Backend', 'Aplicar SLA ao incidente documentando reutilizacao temporaria do SLA atual', 'Desenvolvimento', False),
    ('Backend', 'Registrar auditoria minima das operacoes de incidente', 'Governanca', False),
    ('API', 'Criar ou ajustar endpoints de incidente sem expor detalhes internos do dominio', 'Desenvolvimento', False),
    ('API', 'Garantir contratos de API compativeis com abertura legada, requisicao e catalogo', 'Desenvolvimento', False),
    ('Frontend', 'Ajustar frontend de abertura para incidente', 'Desenvolvimento', False),
    ('Frontend', 'Ajustar frontend de atendimento e diagnostico de incidente', 'Desenvolvimento', False),
    ('Frontend', 'Ajustar frontend para solucao de contorno e resolucao', 'Desenvolvimento', False),
    ('Frontend', 'Ajustar frontend para reabertura e fechamento com validacao responsiva e build', 'Desenvolvimento', False),
    ('Testes', 'Testar abertura de incidente', 'Testes', False),
    ('Testes', 'Testar triagem e classificacao de incidente', 'Testes', False),
    ('Testes', 'Testar atendimento e diagnostico de incidente', 'Testes', False),
    ('Testes', 'Testar registro de solucao de contorno', 'Testes', False),
    ('Testes', 'Testar resolucao de incidente', 'Testes', False),
    ('Testes', 'Testar reabertura de incidente', 'Testes', False),
    ('Testes', 'Testar fechamento de incidente', 'Testes', False),
    ('Testes', 'Testar compatibilidade com abertura legada', 'Testes', False),
    ('Testes', 'Testar compatibilidade com requisicao', 'Testes', False),
    ('Testes', 'Testar compatibilidade com catalogo', 'Testes', False),
    ('Testes', 'Testar SLA aplicado ao incidente e registrar limitacao do SLA atual', 'Testes', False),
    ('Testes', 'Testar permissoes e auditoria do fluxo de incidente', 'Testes', False),
    ('Seguranca', 'Restringir operacoes de incidente conforme permissoes existentes', 'Seguranca', False),
    ('Seguranca', 'Impedir solicitante de manipular classificacao, SLA e dados tecnicos do incidente', 'Seguranca', False),
    ('Seguranca', 'Validar exposicao segura de diagnostico, workaround e resolucao nos contratos', 'Seguranca', False),
    ('Governanca', 'Atualizar SeedData e teste de checklist da Sprint 9', 'Governanca', True),
    ('Governanca', 'Criar migration apenas de checklist e dados da Sprint 9', 'Governanca', True),
    ('Governanca', 'Executar build backend e testes obrigatorios da atualizacao de checklist', 'Governanca', True),
    ('Governanca', 'Verificar ausencia de pending model changes do EF', 'Governanca', True),
    ('Documentacao', 'Atualizar documentacao tecnica da Sprint 9', 'Documentacao', True),
    ('Documentacao', 'Atualizar docs/ROADMAP.md e docs/ROADMAP-ITSM.md', 'Documentacao', True),
    ('Homologacao', 'Registrar homologacao funcional do ciclo completo de incidente', 'Homologacao', False),
    ('Homologacao', 'Registrar homologacao visual responsiva', 'Homologacao', False),
    ('Homologacao', 'Registrar aceite formal somente com evidencia', 'Homologacao', False),
]

DESCRIPTION = "Sprint 9 Gerenciamento de Incidentes"
PERCENTUAL = 19
CONCLUIDOS = 11
TOTAL = 59
PROXIMA_ACAO = "Definir campos especificos de incidente sem duplicar dados existentes."

SITUACAO_ATUAL = (
    "Chamados operacionais e a natureza Incidente ja existem como base reutilizavel, "
    "mas nao ha fluxo dedicado e completo de incidente com servico afetado, CI afetado, "
    "diagnostico, causa provavel, solucao de contorno, resolucao e reabertura rastreaveis."
)
ATENCAO_TECNICA = (
    "Preservar a abertura atual de chamados, requisicoes e catalogo. Reutilizar "
    "NaturezaChamadoEnum.Incidente sem duplicacao. Tratar CI afetado apenas como preparacao "
    "enquanto a CMDB nao estiver funcional e reutilizar temporariamente o SLA atual, "
    "documentando a limitacao ate existir politica especifica de incidente."
)
PENDENCIAS_TECNICAS = (
    "Modelagem dos campos de incidente; servico afetado; vinculo preparatorio de CI; causa "
    "provavel; diagnostico; solucao de contorno; resolucao; regras de reabertura e fechamento; "
    "DTOs, validators, use cases, API, frontend, seguranca, testes e SLA especifico."
)
PENDENCIAS_HOMOLOGACAO = (
    "Homologar somente apos implementacao funcional o ciclo abrir, triar, atender, diagnosticar, "
    "registrar workaround, resolver, reabrir e fechar, incluindo validacao responsiva e aceite formal."
)
EVIDENCIA = (
    "docs/roadmap/sprint-9-gerenciamento-incidentes.md; docs/ROADMAP.md; docs/ROADMAP-ITSM.md; "
    "SeedData.cs; RoadmapSprint9GerenciamentoIncidentesChecklistTests.cs; migration de checklist/dados. "
    "As evidencias atuais comprovam planejamento, reutilizacao da natureza Incidente e governanca; "
    "nao comprovam implementacao funcional do gerenciamento de incidentes."
)
CRITERIO_ACEITE = (
    "Incidente deve ser aberto, classificado, priorizado, atendido, diagnosticado, receber solucao "
    "de contorno quando aplicavel, ser resolvido, reaberto e fechado com historico, auditoria, "
    "permissoes e SLA rastreaveis, sem regressao em chamados legados, requisicoes ou catalogo."
)
OBSERVACAO = (
    "Atualizacao restrita a checklist, dados de roadmap, testes de consistencia e documentacao. "
    "Nenhuma funcionalidade de incidente foi implementada. CI afetado permanece preparatorio ate "
    "a evolucao da CMDB e o SLA de incidente reutiliza o mecanismo atual ate item especifico."
)


def read_text(path: Path) -> tuple[str, bool]:
    raw = path.read_bytes()
    has_bom = raw.startswith(b"\xef\xbb\xbf")
    return raw.decode("utf-8-sig"), has_bom


def write_text(path: Path, text: str, has_bom: bool = False) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    encoding = "utf-8-sig" if has_bom else "utf-8"
    path.write_text(text, encoding=encoding, newline="\n")


def csharp_escape(value: str) -> str:
    return value.replace("\\", "\\\\").replace('"', '\\"')


def checklist_id(ordem: int) -> str:
    if ordem <= 4:
        return f"78787878-7878-7878-7878-00000000010{ordem + 4}"
    return f"78787878-7878-7878-7878-0000000012{ordem:02d}"


def patch_seed() -> None:
    text, has_bom = read_text(SEED_PATH)

    old_lines_pattern = re.compile(
        r'^[ \t]*new \{ Id = Guid\.Parse\("78787878-7878-7878-7878-00000000010[5-8]"\), '
        r'RoadmapItemId = RoadmapItsmItem17Id,.*$\n?',
        re.MULTILINE,
    )
    matches = list(old_lines_pattern.finditer(text))
    if len(matches) != 4:
        raise RuntimeError(f"Esperados 4 itens genericos da Sprint 9; encontrados {len(matches)}.")

    generated_lines: list[str] = []
    for ordem, (_, titulo, grupo, concluido) in enumerate(CHECKLIST, start=1):
        updated = "DataBase" if concluido else "(DateTime?)null"
        updated_by = "UsuarioSistema" if concluido else "(string?)null"
        generated_lines.append(
            '        new { Id = Guid.Parse("'
            + checklist_id(ordem)
            + '"), RoadmapItemId = RoadmapItsmItem17Id, Titulo = "'
            + csharp_escape(titulo)
            + '", Descricao = "'
            + DESCRIPTION
            + '", Grupo = GrupoRoadmapChecklist.'
            + grupo
            + f", Ordem = {ordem}, Concluido = {str(concluido).lower()}, Obrigatorio = true, "
              f"Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = {updated}, "
              f"AtualizadoPor = {updated_by} }},"
        )

    replacement = "\n".join(generated_lines) + "\n"
    first_start = matches[0].start()
    last_end = matches[-1].end()
    text = text[:first_start] + replacement + text[last_end:]

    marker = "            Id = RoadmapItsmItem17Id,"
    marker_index = text.index(marker)
    block_start = text.rfind("        new\n        {", 0, marker_index)
    next_block = text.index("        new\n        {", marker_index + len(marker))
    block = text[block_start:next_block]

    replacements = {
        '            SituacaoAtual = "Chamados operacionais existem, mas sem trilha completa de incidente com diagnostico e workaround.",':
            f'            SituacaoAtual = "{csharp_escape(SITUACAO_ATUAL)}",',
        '            AtencaoTecnica = "Separar status, campos e SLA de incidente sem quebrar fluxo atual.",':
            f'            AtencaoTecnica = "{csharp_escape(ATENCAO_TECNICA)}",',
        "            PercentualImplementacao = 90,":
            f"            PercentualImplementacao = {PERCENTUAL},",
        '            PendenciasTecnicas = "Servico afetado, CI afetado, causa provavel, solucao de contorno e regra de reabertura.",':
            f'            PendenciasTecnicas = "{csharp_escape(PENDENCIAS_TECNICAS)}",',
        '            PendenciasHomologacao = "Homologar ciclo abrir, triar, atender, resolver, reabrir e fechar.",':
            f'            PendenciasHomologacao = "{csharp_escape(PENDENCIAS_HOMOLOGACAO)}",',
        '            EvidenciaImplementacao = "Fluxo alvo definido no novo roadmap ITIL.",':
            f'            EvidenciaImplementacao = "{csharp_escape(EVIDENCIA)}",',
        '            CriterioAceite = "Incidente deve ser aberto, classificado, priorizado, atendido, resolvido, reaberto e fechado com rastreabilidade.",':
            f'            CriterioAceite = "{csharp_escape(CRITERIO_ACEITE)}",',
        '            ProximaAcao = "Implementar estados de incidente e campos especificos no chamado.",':
            f'            ProximaAcao = "{csharp_escape(PROXIMA_ACAO)}",',
        "            Observacao = (string?)null,":
            f'            Observacao = "{csharp_escape(OBSERVACAO)}",',
    }
    for old, new in replacements.items():
        if old not in block:
            raise RuntimeError(f"Trecho esperado nao encontrado no bloco da Sprint 9: {old}")
        block = block.replace(old, new, 1)

    text = text[:block_start] + block + text[next_block:]
    write_text(SEED_PATH, text, has_bom)


def generate_test() -> None:
    titles = "\n".join(
        f'                "{csharp_escape(title)}",' for _, title, _, _ in CHECKLIST
    )
    completed_orders = ", ".join(str(i) for i, item in enumerate(CHECKLIST, start=1) if item[3])
    content = f'''using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapSprint9GerenciamentoIncidentesChecklistTests
{{
    [Fact]
    public async Task RoadmapSprint9DeveRefletirGerenciamentoIncidentesComoBacklogTecnicoRastreavel()
    {{
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var item = context.RoadmapItsmItens.Single(x => x.Id == SeedData.RoadmapItsmItem17Id);
        Assert.Equal("Sprint 9 - Gerenciamento de Incidentes", item.Area);
        Assert.Equal(StatusImplementacaoRoadmapItsm.EmDesenvolvimento, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.Parcial, item.StatusTecnico);
        Assert.Equal({PERCENTUAL}, item.PercentualImplementacao);
        Assert.Equal("{csharp_escape(PROXIMA_ACAO)}", item.ProximaAcao);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == SeedData.RoadmapItsmItem17Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal({TOTAL}, checklistAtivo.Length);
        Assert.All(checklistAtivo, x => Assert.True(x.Ativo));
        Assert.All(checklistAtivo, x => Assert.True(x.Obrigatorio));
        Assert.Equal({CONCLUIDOS}, checklistAtivo.Count(x => x.Concluido));
        Assert.Equal({TOTAL - CONCLUIDOS}, checklistAtivo.Count(x => !x.Concluido));
        Assert.Equal(Enumerable.Range(1, {TOTAL}), checklistAtivo.Select(x => x.Ordem));

        Assert.Equal(
            new[]
            {{
{titles}
            }},
            checklistAtivo.Select(x => x.Titulo).ToArray());

        Assert.Equal(
            new[] {{ {completed_orders} }},
            checklistAtivo.Where(x => x.Concluido).Select(x => x.Ordem).ToArray());

        Assert.Contains(checklistAtivo, x =>
            x.Titulo == "Modelar CI afetado como vinculo preparatorio enquanto a CMDB nao estiver funcional"
            && !x.Concluido);
        Assert.Contains(checklistAtivo, x =>
            x.Titulo == "Aplicar SLA ao incidente documentando reutilizacao temporaria do SLA atual"
            && !x.Concluido);
        Assert.Contains(checklistAtivo, x =>
            x.Titulo == "Confirmar reutilizacao de NaturezaChamadoEnum.Incidente sem duplicar enum ou campo"
            && x.Concluido);

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            CriarUsuarioAdmin());

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem17Id);

        Assert.Equal({TOTAL}, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal({CONCLUIDOS}, detalhe.QuantidadeChecklistConcluido);
        Assert.Equal({PERCENTUAL}, detalhe.PercentualImplementacao);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal("{csharp_escape(PROXIMA_ACAO)}", detalhe.ProximaAcao);

        var percentualEsperado = (int)Math.Round(({CONCLUIDOS} * 100.0) / {TOTAL}, MidpointRounding.AwayFromZero);
        Assert.Equal({PERCENTUAL}, percentualEsperado);
        Assert.Equal(percentualEsperado, detalhe.PercentualImplementacao);
    }}

    private static IUsuarioContextoAplicacaoService CriarUsuarioAdmin()
        => new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador",
            "admin@sgx.local",
            "admin",
            ["Administrador"]));
}}
'''
    write_text(TEST_PATH, content)


def generate_detailed_doc() -> None:
    sections: dict[str, list[tuple[int, str, bool]]] = {}
    for ordem, (section, title, _, completed) in enumerate(CHECKLIST, start=1):
        sections.setdefault(section, []).append((ordem, title, completed))

    checklist_markdown: list[str] = []
    for section, items in sections.items():
        checklist_markdown.append(f"### {section}")
        checklist_markdown.append("")
        for ordem, title, completed in items:
            mark = "x" if completed else " "
            checklist_markdown.append(f"- [{mark}] {ordem}. {title}.")
        checklist_markdown.append("")

    doc = f'''# Sprint 9 - Gerenciamento de Incidentes

## Objetivo

Formalizar o Gerenciamento de Incidentes para falha, indisponibilidade ou degradacao de servico, preservando os fluxos atuais de chamados, requisicoes e catalogo.

Esta atualizacao e exclusivamente de planejamento, checklist, dados de roadmap, testes de consistencia e documentacao. Nenhuma funcionalidade de incidente foi implementada.

## Diagnostico do estado atual

- `NaturezaChamadoEnum.Incidente` ja existe e e reutilizada em servicos, controllers e testes.
- A abertura e o atendimento atuais fornecem uma base operacional generica, mas nao constituem um processo completo de Gerenciamento de Incidentes.
- Nao foram identificados campos dedicados e rastreaveis para servico afetado, CI afetado, causa provavel, diagnostico inicial e solucao de contorno.
- O ciclo atual de status deve ser reaproveitado somente onde houver compatibilidade comprovada; novos estados nao devem ser criados sem regra explicita.
- A Sprint 12 ainda concentra a evolucao de CMDB. Por isso, `CI afetado` permanece como modelagem preparatoria e nao deve assumir uma CMDB funcional.
- O SLA de incidente ainda reutiliza o mecanismo atual. Nao existe nesta etapa uma politica de SLA exclusiva para incidentes.
- Problema, Mudanca e Requisicao permanecem fora do escopo funcional desta sprint.

## Escopo desta atualizacao

- substituir o checklist generico de quatro itens;
- registrar grupos tecnicos e criterios verificaveis;
- corrigir o percentual para a evidencia real;
- atualizar seed, migration de dados, teste de checklist e documentos de roadmap;
- preservar comportamento legado e a Sprint 8.

## Percentual

- Total de itens: `{TOTAL}`.
- Itens concluidos com evidencia: `{CONCLUIDOS}`.
- Itens pendentes: `{TOTAL - CONCLUIDOS}`.
- Percentual calculado: `{PERCENTUAL}%`.
- Proxima acao: `{PROXIMA_ACAO}`

O percentual mede o checklist completo. Os itens concluidos representam diagnostico, definicao de escopo, reutilizacao comprovada da natureza `Incidente`, governanca tecnica e documentacao. Nao representam entrega funcional do processo.

## Checklist detalhado

{chr(10).join(checklist_markdown)}
## Impactos obrigatorios a verificar na implementacao futura

| Area | Diretriz |
|---|---|
| Abertura de chamado | Preservar a abertura atual e introduzir dados de incidente sem tornar o fluxo legado invalido. |
| Atendimento | Adicionar diagnostico, workaround e resolucao sem bloquear comentarios, anexos ou acompanhamento comum. |
| Status | Reutilizar estados compativeis e criar regra dedicada antes de qualquer novo estado ou transicao. |
| SLA | Reutilizacao temporaria do SLA atual; politica especifica permanece pendente. |
| Requisicao e catalogo | Nao converter requisicoes em incidentes nem alterar a abertura guiada da Sprint 8. |
| CI afetado | Vinculo preparatorio ate a CMDB estar funcional; nao assumir relacionamento de CI inexistente. |
| Permissoes | Restringir dados e acoes tecnicas sem elevar privilegios do solicitante. |
| Banco de dados | Somente migration de checklist/dados nesta atualizacao; nenhuma migration estrutural. |

## Fora do escopo

- implementar entidades, campos, DTOs, validators, use cases, endpoints ou telas de incidente;
- criar fluxo de Problema;
- criar ou antecipar a CMDB;
- criar SLA exclusivo de incidente;
- alterar status operacional;
- alterar abertura legada, requisicoes, catalogo ou Sprint 8;
- executar homologacao funcional ou registrar aceite sem evidencia real.

## Criterios de aceite da atualizacao do checklist

1. O checklist ativo da Sprint 9 possui `{TOTAL}` itens ordenados e obrigatorios.
2. Somente `{CONCLUIDOS}` itens estao concluidos e todos possuem evidencia no codigo ou na documentacao.
3. O percentual exibido e `{PERCENTUAL}%`, calculado automaticamente pelo checklist.
4. `CI afetado` e registrado como preparatorio enquanto a CMDB nao estiver funcional.
5. O SLA de incidente registra explicitamente a reutilizacao temporaria do SLA atual.
6. A migration altera somente dados de roadmap/checklist.
7. A Sprint 8 e os fluxos legados nao sao modificados.

## Proxima etapa recomendada

{PROXIMA_ACAO}
'''
    write_text(DOC_PATH, doc)


def patch_roadmap(path: Path, itsm: bool) -> None:
    text, has_bom = read_text(path)
    text = text.replace(
        "9. Sprint 9 - Gerenciamento de Incidentes (50% - Em desenvolvimento)",
        f"9. Sprint 9 - Gerenciamento de Incidentes ({PERCENTUAL}% - Em desenvolvimento)",
    )
    text = text.replace(
        "| Sprint 9 - Gerenciamento de Incidentes | ITIL/ITSM | Em desenvolvimento | Parcial | 50% |",
        f"| Sprint 9 - Gerenciamento de Incidentes | ITIL/ITSM | Em desenvolvimento | Parcial | {PERCENTUAL}% |",
    )

    heading = "## Atualizacao 2026-07-07 - Sprint 9 Gerenciamento de Incidentes - Checklist tecnico"
    if heading not in text:
        section = f'''{heading}

- O checklist generico de `2/4` foi substituido por `{TOTAL}` itens tecnicos e rastreaveis.
- Permanecem concluidos somente `{CONCLUIDOS}` itens com evidencia de planejamento, reutilizacao da natureza `Incidente`, governanca e documentacao.
- Percentual recalculado para `{PERCENTUAL}%` (`{CONCLUIDOS}/{TOTAL}`).
- Nenhuma funcionalidade, fluxo legado, endpoint, frontend ou migration estrutural foi alterado.
- `CI afetado` permanece preparatorio enquanto a CMDB nao estiver funcional.
- O SLA de incidente reutiliza temporariamente o mecanismo atual; politica especifica continua pendente.
- Documento de referencia: `docs/roadmap/sprint-9-gerenciamento-incidentes.md`.
- Proxima acao: `{PROXIMA_ACAO}`

'''
        insertion_marker = "## Atualizacao 2026-07-07 - Sprint 8"
        index = text.find(insertion_marker)
        if index < 0:
            raise RuntimeError(f"Marcador de insercao nao encontrado em {path}.")
        text = text[:index] + section + text[index:]

    write_text(path, text, has_bom)


def validate_result() -> None:
    seed, _ = read_text(SEED_PATH)
    sprint9_lines = [
        line for line in seed.splitlines()
        if "RoadmapItemId = RoadmapItsmItem17Id" in line
    ]
    if len(sprint9_lines) != TOTAL:
        raise RuntimeError(f"Seed final da Sprint 9 possui {len(sprint9_lines)} itens; esperado {TOTAL}.")
    if sum("Concluido = true" in line for line in sprint9_lines) != CONCLUIDOS:
        raise RuntimeError("Quantidade de itens concluidos no seed diverge do esperado.")
    if f"PercentualImplementacao = {PERCENTUAL}," not in seed:
        raise RuntimeError("Percentual da Sprint 9 nao foi atualizado no seed.")
    if not TEST_PATH.exists() or not DOC_PATH.exists():
        raise RuntimeError("Teste ou documento da Sprint 9 nao foi criado.")


def main() -> None:
    patch_seed()
    generate_test()
    generate_detailed_doc()
    patch_roadmap(ROADMAP_PATH, itsm=False)
    patch_roadmap(ROADMAP_ITSM_PATH, itsm=True)
    validate_result()
    print(f"Sprint 9 atualizada: {CONCLUIDOS}/{TOTAL} ({PERCENTUAL}%).")


if __name__ == "__main__":
    main()
