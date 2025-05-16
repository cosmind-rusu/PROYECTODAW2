import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '../views/DashboardView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import AnimalSpeciesView from '../views/AnimalSpeciesView.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: DashboardView,
      meta: { requiresAuth: true },
    },
    {
      path: '/categories',
      name: 'animalspecies',
      component: AnimalSpeciesView,
      meta: { requiresAuth: true },
    },
    {
      path: '/consultations',
      name: 'consultations',
      component: () => import('../views/ConsultationsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/treatments',
      name: 'treatments',
      component: () => import('../views/ExpenseCategoriesView.vue'), // Reutilizamos temporalmente
      meta: { requiresAuth: true },
    },
    {
      path: '/healthplans',
      name: 'healthplans',
      component: () => import('../views/BudgetsView.vue'), // Reutilizamos temporalmente
      meta: { requiresAuth: true },
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { guest: true },
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterView,
      meta: { guest: true },
    },
  ],
})

// Route guard for auth
router.beforeEach((to, from) => {
  const authStore = useAuthStore()
  if (to.meta.requiresAuth && !authStore.isAuthenticated) return { name: 'login' }
  if (to.meta.guest && authStore.isAuthenticated) return { name: 'dashboard' }
})

export default router
