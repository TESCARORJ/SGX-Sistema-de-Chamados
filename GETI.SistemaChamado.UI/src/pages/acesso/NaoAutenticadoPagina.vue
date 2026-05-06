<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Autenticacao Necessaria</h1>
      <p>O acesso ao sistema exige autenticacao corporativa.</p>
    </div>

    <q-card flat bordered class="card-inicial">
      <q-card-section>
        Nao foi possivel identificar seu usuario. Se estiver em ambiente local, configure os headers de
        autenticacao de desenvolvimento.
      </q-card-section>
      <q-card-actions align="right">
        <q-btn color="primary" no-caps label="Ir para login" to="/login" />
      </q-card-actions>
    </q-card>
  </q-page>
</template>
