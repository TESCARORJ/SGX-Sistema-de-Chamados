<template>
  <q-page padding>
    <div class="pagina-cabecalho">
      <h1>Painel Administrativo</h1>
      <p>Cadastros mestres administrativos para sustentacao do service desk corporativo.</p>
    </div>

    <div class="row q-col-gutter-md">
      <div v-for="item in atalhos" :key="item.rota" class="col-12 col-md-6 col-lg-4">
        <q-card bordered flat class="card-atalho cursor-pointer" @click="$router.push(item.rota)">
          <q-card-section>
            <div class="text-h6">{{ item.titulo }}</div>
            <div class="text-body2 text-grey-8">{{ item.descricao }}</div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </q-page>
</template>

<script setup lang="ts">
const atalhos = [
  {
    titulo: 'Departamentos',
    descricao: 'Base organizacional para vinculos administrativos e operacionais.',
    rota: '/admin/departamentos'
  },
  {
    titulo: 'Caixas de E-mail',
    descricao: 'Canais de entrada vinculados aos departamentos da organizacao.',
    rota: '/admin/caixas-email'
  },
  {
    titulo: 'Categorias',
    descricao: 'Classificacao administrativa para estrutura do catalogo.',
    rota: '/admin/categorias'
  },
  {
    titulo: 'Servicos',
    descricao: 'Catalogo institucional de servicos por categoria e departamento.',
    rota: '/admin/servicos'
  },
  {
    titulo: 'Grupos de Atendimento',
    descricao: 'Times responsaveis por atendimento vinculados a departamentos.',
    rota: '/admin/grupos-atendimento'
  }
];
</script>

<style scoped>
.card-atalho {
  min-height: 150px;
}
</style>
