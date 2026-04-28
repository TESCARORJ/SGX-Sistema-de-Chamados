package br.geti.sistemachamado.api.erro;

import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import jakarta.servlet.http.HttpServletRequest;
import java.time.OffsetDateTime;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

@RestControllerAdvice
public class TratadorGlobalExcecao {

    private static final Logger LOGGER = LoggerFactory.getLogger(TratadorGlobalExcecao.class);

    @ExceptionHandler(ErroDeDominio.class)
    public ResponseEntity<ErroApiResposta> tratarErroDeDominio(
            final ErroDeDominio exception,
            final HttpServletRequest request
    ) {
        return ResponseEntity.badRequest().body(new ErroApiResposta(
                "ERRO_DOMINIO",
                exception.getMessage(),
                request.getRequestURI(),
                OffsetDateTime.now()
        ));
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<ErroApiResposta> tratarValidacao(
            final MethodArgumentNotValidException exception,
            final HttpServletRequest request
    ) {
        final var mensagem = exception.getBindingResult().getFieldErrors().stream()
                .findFirst()
                .map(e -> String.format("Campo %s %s", e.getField(), e.getDefaultMessage()))
                .orElse("Falha de validacao");

        return ResponseEntity.status(HttpStatus.UNPROCESSABLE_ENTITY).body(new ErroApiResposta(
                "ERRO_VALIDACAO",
                mensagem,
                request.getRequestURI(),
                OffsetDateTime.now()
        ));
    }

    @ExceptionHandler(Exception.class)
    public ResponseEntity<ErroApiResposta> tratarNaoMapeado(
            final Exception exception,
            final HttpServletRequest request
    ) {
        LOGGER.error("Erro nao tratado na API", exception);

        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(new ErroApiResposta(
                "ERRO_INTERNO",
                "Falha inesperada ao processar a requisicao.",
                request.getRequestURI(),
                OffsetDateTime.now()
        ));
    }
}

