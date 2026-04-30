<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Portal do Solicitante</h1>
      <p>Abra novos chamados e acompanhe seu atendimento em tempo real.</p>
    </div>

    <div class="row q-col-gutter-md">
      <div class="col-12 col-md-6">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">Novo Chamado</div>
            <p class="q-mt-sm text-grey-8">Registre uma solicitacao com prioridade, categoria e servico.</p>
          </q-card-section>
          <q-card-actions align="right">
            <q-btn color="primary" no-caps icon="add" label="Abrir chamado" :to="{ name: 'portal-chamados-abertura' }" />
          </q-card-actions>
        </q-card>
      </div>

      <div class="col-12 col-md-6">
        <q-card flat bordered>
          <q-card-section>
            <div class="text-h6">Meus Chamados</div>
            <p class="q-mt-sm text-grey-8">Consulte historico, situacao e detalhes dos chamados em andamento.</p>
          </q-card-section>
          <q-card-actions align="right">
            <q-btn flat color="secondary" no-caps icon="list" label="Ver chamados" :to="{ name: 'portal-chamados-lista' }" />
          </q-card-actions>
        </q-card>
      </div>
    </div>
  </q-page>
</template>
