<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Dashboard Administrativo</h1>
        <p>Visao operacional da fila de chamados para acompanhamento rapido da equipe.</p>
      </div>
      <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregar" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <template v-if="dashboard">
      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Chamados por Situacao</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in dashboard.porSituacao" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge color="primary" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Chamados por Prioridade</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in dashboard.porPrioridade" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge :color="corPrioridade(indicador.chave)" text-color="white">
                    {{ indicador.total }}
                  </q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-4">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Chamados por Departamento</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in dashboard.porDepartamento" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge color="secondary" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
              <q-item v-if="dashboard.porDepartamento.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum chamado registrado por departamento.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>

      <q-card flat bordered>
        <q-card-section class="row items-center justify-between">
          <div class="text-h6">Pendentes Recentes</div>
          <q-btn flat color="primary" label="Abrir Fila Completa" :to="{ name: 'admin-chamados-fila' }" />
        </q-card-section>
        <q-separator />
        <q-table
          flat
          :rows="dashboard.pendentesRecentes"
          :columns="colunasPendentes"
          row-key="id"
          :loading="carregando"
          no-data-label="Sem chamados pendentes recentes."
        >
          <template #body-cell-prioridade="props">
            <q-td :props="props">
              <q-badge :color="corPrioridade(props.row.prioridade)" text-color="white">
                {{ props.row.prioridade }}
              </q-badge>
            </q-td>
          </template>
          <template #body-cell-dataCriacao="props">
            <q-td :props="props">{{ formatarData(props.row.dataCriacao) }}</q-td>
          </template>
          <template #body-cell-acoes="props">
            <q-td :props="props" class="text-right">
              <q-btn
                flat
                dense
                color="primary"
                icon="visibility"
                label="Detalhar"
                :to="{ name: 'admin-chamados-detalhe', params: { id: props.row.id } }"
              />
            </q-td>
          </template>
        </q-table>
      </q-card>
    </template>
  </q-page>
</template>

<script setup lang="ts">
import type { QTableColumn } from 'quasar';
import { onMounted, ref } from 'vue';
import { consultarDashboardChamadoAdmin, type DashboardAdminChamado } from '@/services/apiAdmin';

const carregando = ref(false);
const erro = ref('');
const dashboard = ref<DashboardAdminChamado | null>(null);

const colunasPendentes: QTableColumn[] = [
  { name: 'numero', label: 'Numero', field: 'numero', align: 'left', sortable: true },
  { name: 'titulo', label: 'Titulo', field: 'titulo', align: 'left', sortable: true },
  { name: 'situacao', label: 'Situacao', field: 'situacao', align: 'left', sortable: true },
  { name: 'prioridade', label: 'Prioridade', field: 'prioridade', align: 'left', sortable: true },
  { name: 'departamento', label: 'Departamento', field: 'departamento', align: 'left', sortable: true },
  { name: 'responsavel', label: 'Responsavel', field: 'responsavel', align: 'left', sortable: true },
  { name: 'dataCriacao', label: 'Abertura', field: 'dataCriacao', align: 'left', sortable: true },
  { name: 'acoes', label: 'Acoes', field: 'acoes', align: 'right' }
];

function formatarData(valor: string): string {
  return new Date(valor).toLocaleString('pt-BR');
}

function corPrioridade(prioridade: string): string {
  if (prioridade === 'CRITICA') {
    return 'negative';
  }
  if (prioridade === 'ALTA') {
    return 'deep-orange';
  }
  if (prioridade === 'MEDIA') {
    return 'warning';
  }
  return 'positive';
}

async function carregar(): Promise<void> {
  carregando.value = true;
  erro.value = '';
  try {
    dashboard.value = await consultarDashboardChamadoAdmin();
  } catch (ex) {
    erro.value = ex instanceof Error ? ex.message : 'Falha ao carregar dashboard administrativo.';
  } finally {
    carregando.value = false;
  }
}

onMounted(async () => {
  await carregar();
});
</script>
