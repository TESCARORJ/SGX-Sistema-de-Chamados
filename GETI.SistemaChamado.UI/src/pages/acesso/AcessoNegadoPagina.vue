<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Acesso Negado</h1>
      <p>Seu perfil interno nao possui autorizacao para esta area.</p>
    </div>

    <q-card flat bordered class="card-inicial">
      <q-card-section>
        O backend controla autorizacao por perfil de acesso interno. Solicite ajuste de permissao ao administrador.
      </q-card-section>
      <q-card-actions align="right">
        <q-btn flat no-caps label="Voltar para portal" to="/portal" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>
