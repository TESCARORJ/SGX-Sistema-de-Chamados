import { createRouter, createWebHistory } from 'vue-router';
import PortalLayout from '@/layouts/PortalLayout.vue';
import AdminLayout from '@/layouts/AdminLayout.vue';
import PortalSolicitantePagina from '@/pages/portal/PortalSolicitantePagina.vue';
import AdminDashboardPagina from '@/pages/admin/AdminDashboardPagina.vue';

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
          component: PortalSolicitantePagina
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
          component: AdminDashboardPagina
        }
      ]
    }
  ]
});
