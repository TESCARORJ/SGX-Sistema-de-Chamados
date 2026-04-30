<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Usuarios Administrativos</h1>
      <p>Gestao de usuarios e permissoes administrativas (evolucao planejada).</p>
    </div>

    <q-card flat bordered>
      <q-card-section>
        <q-banner class="bg-blue-1 text-primary" rounded>
          Esta tela e um ponto de entrada para a futura gestao de perfis e vinculacoes com identidade corporativa AD/Azure.
        </q-banner>
      </q-card-section>
      <q-separator />
      <q-card-section>
        <ul class="q-pl-md">
          <li>Consulta de usuarios internos sincronizados</li>
          <li>Ajuste de perfil de acesso (Atendente, Supervisor, Administrador)</li>
          <li>Vinculacao de departamento de atuacao</li>
        </ul>
      </q-card-section>
    </q-card>
  </q-page>
</template>
