import { createRouter, createWebHistory } from 'vue-router';
import PortalLayout from '@/layouts/PortalLayout.vue';
import AdminLayout from '@/layouts/AdminLayout.vue';
import PortalChamadosListaPagina from '@/pages/portal/chamados/PortalChamadosListaPagina.vue';
import PortalChamadoAberturaPagina from '@/pages/portal/chamados/PortalChamadoAberturaPagina.vue';
import PortalChamadoDetalhePagina from '@/pages/portal/chamados/PortalChamadoDetalhePagina.vue';
import AdminDashboardPagina from '@/pages/admin/AdminDashboardPagina.vue';
import DepartamentosAdminPagina from '@/pages/admin/DepartamentosAdminPagina.vue';
import CaixasEmailAdminPagina from '@/pages/admin/CaixasEmailAdminPagina.vue';
import CategoriasAdminPagina from '@/pages/admin/CategoriasAdminPagina.vue';
import ServicosAdminPagina from '@/pages/admin/ServicosAdminPagina.vue';
import GruposAtendimentoAdminPagina from '@/pages/admin/GruposAtendimentoAdminPagina.vue';
import AdminChamadosFilaPagina from '@/pages/admin/chamados/AdminChamadosFilaPagina.vue';
import AdminChamadoDetalhePagina from '@/pages/admin/chamados/AdminChamadoDetalhePagina.vue';
import AdminRelatoriosOperacionaisPagina from '@/pages/admin/chamados/AdminRelatoriosOperacionaisPagina.vue';
import NaoAutenticadoPagina from '@/pages/acesso/NaoAutenticadoPagina.vue';
import AcessoNegadoPagina from '@/pages/acesso/AcessoNegadoPagina.vue';
import { obterUsuarioAtual, possuiAcessoAdministrativo } from '@/services/sessaoUsuario';

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: PortalLayout,
      children: [
        {
          path: '',
          name: 'portal-chamados-lista',
          component: PortalChamadosListaPagina,
          meta: {
            requerAutenticacao: true
          }
        },
        {
          path: 'portal/chamados/novo',
          name: 'portal-chamados-abertura',
          component: PortalChamadoAberturaPagina,
          meta: {
            requerAutenticacao: true
          }
        },
        {
          path: 'portal/chamados/:id',
          name: 'portal-chamados-detalhe',
          component: PortalChamadoDetalhePagina,
          meta: {
            requerAutenticacao: true
          }
        }
      ]
    },
    {
      path: '/admin',
      component: AdminLayout,
      children: [
        {
          path: '',
          name: 'admin-dashboard',
          component: AdminDashboardPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'departamentos',
          name: 'admin-departamentos',
          component: DepartamentosAdminPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'caixas-email',
          name: 'admin-caixas-email',
          component: CaixasEmailAdminPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'categorias',
          name: 'admin-categorias',
          component: CategoriasAdminPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'servicos',
          name: 'admin-servicos',
          component: ServicosAdminPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'grupos-atendimento',
          name: 'admin-grupos-atendimento',
          component: GruposAtendimentoAdminPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'chamados',
          name: 'admin-chamados-fila',
          component: AdminChamadosFilaPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'chamados/relatorios',
          name: 'admin-chamados-relatorios',
          component: AdminRelatoriosOperacionaisPagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        },
        {
          path: 'chamados/:id',
          name: 'admin-chamados-detalhe',
          component: AdminChamadoDetalhePagina,
          meta: {
            requerAutenticacao: true,
            requerAcessoAdmin: true
          }
        }
      ]
    },
    {
      path: '/nao-autenticado',
      name: 'nao-autenticado',
      component: NaoAutenticadoPagina
    },
    {
      path: '/acesso-negado',
      name: 'acesso-negado',
      component: AcessoNegadoPagina
    }
  ]
});

router.beforeEach(async (to) => {
  const requerAutenticacao = Boolean(to.meta.requerAutenticacao);
  const requerAcessoAdmin = Boolean(to.meta.requerAcessoAdmin);

  if (!requerAutenticacao && !requerAcessoAdmin) {
    return true;
  }

  let usuario = null;
  try {
    usuario = await obterUsuarioAtual(true);
  } catch {
    return { name: 'nao-autenticado' };
  }
  if (!usuario) {
    if (to.name === 'nao-autenticado') {
      return true;
    }

    return { name: 'nao-autenticado' };
  }

  if (requerAcessoAdmin && !possuiAcessoAdministrativo(usuario)) {
    if (to.name === 'acesso-negado') {
      return true;
    }

    return { name: 'acesso-negado' };
  }

  return true;
});
