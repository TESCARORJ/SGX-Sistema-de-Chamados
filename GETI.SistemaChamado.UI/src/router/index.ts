import { createRouter, createWebHistory } from 'vue-router';
import PortalLayout from '@/layouts/PortalLayout.vue';
import AdminLayout from '@/layouts/AdminLayout.vue';
import PortalSolicitantePagina from '@/pages/portal/PortalSolicitantePagina.vue';
import AdminDashboardPagina from '@/pages/admin/AdminDashboardPagina.vue';
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
          name: 'portal-solicitante',
          component: PortalSolicitantePagina,
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
