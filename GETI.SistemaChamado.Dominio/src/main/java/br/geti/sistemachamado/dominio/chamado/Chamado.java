package br.geti.sistemachamado.dominio.chamado;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record Chamado(
        UUID id,
        String numero,
        String titulo,
        String descricao,
        SituacaoChamado situacao,
        PrioridadeChamado prioridade,
        OrigemChamado origem,
        Usuario solicitante,
        Usuario responsavel,
        Departamento departamento,
        Categoria categoria,
        Servico servico,
        Integer prazoSlaMinutos,
        LocalDateTime dataLimiteSla,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public Chamado {
        ValidadorDominio.obrigatorio(id, "id do chamado e obrigatorio");
        numero = ValidadorDominio.textoObrigatorio(numero, "numero do chamado e obrigatorio");
        titulo = ValidadorDominio.textoObrigatorio(titulo, "titulo do chamado e obrigatorio");
        descricao = ValidadorDominio.textoObrigatorio(descricao, "descricao do chamado e obrigatoria");
        ValidadorDominio.obrigatorio(situacao, "situacao do chamado e obrigatoria");
        ValidadorDominio.obrigatorio(prioridade, "prioridade do chamado e obrigatoria");
        ValidadorDominio.obrigatorio(origem, "origem do chamado e obrigatoria");
        ValidadorDominio.obrigatorio(solicitante, "solicitante do chamado e obrigatorio");
        if (responsavel != null && !responsavel.ativo()) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "responsavel do chamado precisa estar ativo"
            );
        }
        ValidadorDominio.obrigatorio(departamento, "departamento do chamado e obrigatorio");
        ValidadorDominio.obrigatorio(categoria, "categoria do chamado e obrigatoria");
        ValidadorDominio.obrigatorio(servico, "servico do chamado e obrigatorio");
        prazoSlaMinutos = ValidadorDominio.obrigatorio(prazoSlaMinutos, "prazo de sla do chamado e obrigatorio");
        if (prazoSlaMinutos <= 0) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "prazo de sla do chamado deve ser maior que zero"
            );
        }
        ValidadorDominio.obrigatorio(dataLimiteSla, "data limite de sla do chamado e obrigatoria");

        if (!departamento.id().equals(servico.departamento().id())) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "Servico selecionado nao pertence ao departamento informado."
            );
        }
        if (!categoria.id().equals(servico.categoria().id())) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "Servico selecionado nao pertence a categoria informada."
            );
        }

        validarAuditoria(dataCriacao, dataAtualizacao);
        if (dataLimiteSla.isBefore(dataCriacao)) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "data limite de sla nao pode ser anterior a data de criacao do chamado"
            );
        }
    }
}


