<template>
  <q-page padding>
    <div class="pagina-cabecalho row items-center justify-between q-col-gutter-md">
      <div>
        <h1>Dashboard Gerencial</h1>
        <p>Indicadores operacionais de SLA e distribuicao da fila administrativa.</p>
      </div>
      <q-btn flat icon="refresh" label="Atualizar" :loading="carregando" @click="carregar" />
    </div>

    <q-banner v-if="erro" class="bg-red-1 text-negative q-mb-md" rounded>
      {{ erro }}
    </q-banner>

    <template v-if="dashboard">
      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-md-4">
          <q-card flat bordered class="card-sla card-sla-vencido">
            <q-card-section>
              <div class="text-overline">SLA</div>
              <div class="text-h5">{{ dashboard.totalVencidosSla }}</div>
              <div class="text-caption">Chamados vencidos</div>
            </q-card-section>
          </q-card>
        </div>
        <div class="col-12 col-md-4">
          <q-card flat bordered class="card-sla card-sla-proximo">
            <q-card-section>
              <div class="text-overline">SLA</div>
              <div class="text-h5">{{ dashboard.totalProximosVencimentoSla }}</div>
              <div class="text-caption">Proximos do vencimento</div>
            </q-card-section>
          </q-card>
        </div>
        <div class="col-12 col-md-4">
          <q-card flat bordered class="card-sla card-sla-total">
            <q-card-section>
              <div class="text-overline">OPERACAO</div>
              <div class="text-h5">{{ dashboard.pendentesRecentes.length }}</div>
              <div class="text-caption">Pendentes recentes monitorados</div>
            </q-card-section>
          </q-card>
        </div>
      </div>

      <div class="row q-col-gutter-md q-mb-md">
        <div class="col-12 col-xl-3 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Por Situacao</div>
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

        <div class="col-12 col-xl-3 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Por Prioridade</div>
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

        <div class="col-12 col-xl-3 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Por Departamento</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in dashboard.porDepartamento" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge color="secondary" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-xl-3 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6">Status SLA</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="indicador in dashboard.porStatusSla" :key="indicador.chave">
                <q-item-section>{{ indicador.chave }}</q-item-section>
                <q-item-section side>
                  <q-badge :color="corStatusSla(indicador.chave)" text-color="white">{{ indicador.total }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>

      <q-card flat bordered class="q-mb-md">
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
          <template #body-cell-statusSla="props">
            <q-td :props="props">
              <q-badge :color="corStatusSla(props.row.statusSla)" text-color="white">
                {{ props.row.statusSla }}
              </q-badge>
            </q-td>
          </template>
          <template #body-cell-dataLimiteSla="props">
            <q-td :props="props">{{ formatarData(props.row.dataLimiteSla) }}</q-td>
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

      <div class="row q-col-gutter-md">
        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6 text-negative">Chamados Vencidos</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="chamado in dashboard.chamadosVencidosSla" :key="chamado.id" clickable :to="{ name: 'admin-chamados-detalhe', params: { id: chamado.id } }">
                <q-item-section>
                  <q-item-label>{{ chamado.numero }} - {{ chamado.titulo }}</q-item-label>
                  <q-item-label caption>
                    {{ chamado.departamento }} | {{ chamado.responsavel }} | Atraso: {{ chamado.minutosAtrasoSla }} min
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="dashboard.chamadosVencidosSla.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum chamado vencido no momento.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>

        <div class="col-12 col-lg-6">
          <q-card flat bordered>
            <q-card-section>
              <div class="text-h6 text-warning">Proximos do Vencimento</div>
            </q-card-section>
            <q-separator />
            <q-list bordered separator>
              <q-item v-for="chamado in dashboard.chamadosProximosVencimentoSla" :key="chamado.id" clickable :to="{ name: 'admin-chamados-detalhe', params: { id: chamado.id } }">
                <q-item-section>
                  <q-item-label>{{ chamado.numero }} - {{ chamado.titulo }}</q-item-label>
                  <q-item-label caption>
                    {{ chamado.departamento }} | {{ chamado.responsavel }} | Limite: {{ formatarData(chamado.dataLimiteSla) }}
                  </q-item-label>
                </q-item-section>
              </q-item>
              <q-item v-if="dashboard.chamadosProximosVencimentoSla.length === 0">
                <q-item-section>
                  <q-item-label caption>Nenhum chamado proximo do vencimento.</q-item-label>
                </q-item-section>
              </q-item>
            </q-list>
          </q-card>
        </div>
      </div>
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
  { name: 'statusSla', label: 'SLA', field: 'statusSla', align: 'left', sortable: true },
  { name: 'dataLimiteSla', label: 'Data Limite SLA', field: 'dataLimiteSla', align: 'left', sortable: true },
  { name: 'responsavel', label: 'Responsavel', field: 'responsavel', align: 'left', sortable: true },
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

function corStatusSla(statusSla: string): string {
  if (statusSla === 'VENCIDO') {
    return 'negative';
  }
  if (statusSla === 'PROXIMO_DO_VENCIMENTO') {
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

<style scoped>
.card-sla {
  border-left: 6px solid transparent;
}

.card-sla-vencido {
  border-left-color: #c10015;
}

.card-sla-proximo {
  border-left-color: #f2c037;
}

.card-sla-total {
  border-left-color: #1d7c98;
}
</style>
