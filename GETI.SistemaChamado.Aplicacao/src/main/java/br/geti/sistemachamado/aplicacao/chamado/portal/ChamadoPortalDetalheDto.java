package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;

public record ChamadoPortalDetalheDto(
        UUID id,
        String numero,
        String titulo,
        String descricao,
        String situacao,
        String prioridade,
        String origem,
        UUID departamentoId,
        String departamento,
        UUID categoriaId,
        String categoria,
        UUID servicoId,
        String servico,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao,
        List<InteracaoChamadoPortalDto> interacoes,
        List<HistoricoChamadoPortalDto> historicos,
        List<AnexoChamadoPortalDto> anexos
) {
}
