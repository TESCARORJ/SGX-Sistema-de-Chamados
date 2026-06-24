using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

var file = ""src/SGX.SistemaChamado.Infrastructure/Persistence/Seed/SeedData.cs"";
var text = File.ReadAllText(file, Encoding.UTF8);

var checklists = @""
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000901""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Testar domínio e estrutura persistente de notificações"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 5, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000902""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Criar serviço de geração idempotente de notificações"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 6, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000903""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar resolução de destinatários por participação e perfil"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 7, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000904""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Modelar templates e materialização de conteúdo"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 8, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000905""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar preferências de notificação por usuário e evento"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 9, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000906""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar processamento e controle de tentativas de entrega"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 10, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000907""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar entrega pelo canal Sistema"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 11, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000908""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar entrega pelo canal E-mail"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Desenvolvimento, Ordem = 12, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000909""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Criar API de consulta, leitura e marcação como não lida"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 13, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000910""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Implementar central de notificações no frontend"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Homologacao, Ordem = 14, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000911""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Integrar notificações aos eventos ITSM priorizados e executar testes de regressão"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Testes, Ordem = 15, Concluido = true, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
        new { Id = Guid.Parse(""""78787878-7878-7878-7878-000000000912""""), RoadmapItemId = RoadmapItsmItem25Id, Titulo = """"Documentar, homologar e registrar aceite da Sprint 6"""", Descricao = """"Sprint 6 Notificacoes ITSM"""", Grupo = GrupoRoadmapChecklist.Governanca, Ordem = 16, Concluido = false, Obrigatorio = true, Ativo = true, CriadoEm = DataBase, CriadoPor = UsuarioSistema, AtualizadoEm = DataBase, AtualizadoPor = UsuarioSistema },
"";

var targetLine = ""new { Id = Guid.Parse(\""78787878-7878-7878-7878-000000000140\""),"";
var split = text.Split(new[] { ""\r\n"", ""\n"" }, StringSplitOptions.None);
var result = new System.Collections.Generic.List<string>();

foreach (var line in split)
{
    result.Add(line);
    if (line.Contains(targetLine))
    {
        result.Add(checklists.TrimEnd());
    }
}

var text2 = string.Join(""\r\n"", result);

// Item 21 (starts with Id = RoadmapItsmItem21Id, find PercentualImplementacao = 25 and change to 6)
text2 = Regex.Replace(text2, @""(Id = RoadmapItsmItem21Id,.*?PercentualImplementacao = )25"", """", RegexOptions.Singleline);
// Item 36 (starts with Id = RoadmapItsmItem36Id)
text2 = Regex.Replace(text2, @""(Id = RoadmapItsmItem36Id,.*?PercentualImplementacao = )25"", """", RegexOptions.Singleline);

File.WriteAllText(file, text2, Encoding.UTF8);
