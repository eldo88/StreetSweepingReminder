import { createRouter, createWebHistory } from 'vue-router'
import RegisterPageView from '@/views/RegisterPageView.vue'
import MainView from '@/views/MainView.vue'
import { useAuthStore } from '@/stores/auth.js'
import LoginPageView from '@/views/LoginPageView.vue'
import LandingPageView from '@/views/LandingPageView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: 'welcome',
    },
    {
      path: '/welcome',
      name: 'welcome',
      component: LandingPageView,
    },
    {
      path: '/login',
      name: 'login',
      component: LoginPageView,
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterPageView,
    },
    {
      path: '/main',
      name: 'main',
      component: MainView,
      meta: { requiresAuth: true },
    },
  ],
})

router.beforeEach((to, from, next) => {
  console.log('Navigating to:', to.path)
  const authStore = useAuthStore()
  const isLoggedIn = authStore.token !== null

  if (to.meta.requiresAuth && !isLoggedIn) {
    next('/login')
  } else {
    next()
  }
})
export default router
