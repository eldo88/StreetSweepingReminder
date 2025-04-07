import { createRouter, createWebHistory } from 'vue-router'
import LoginOrRegisterView from "@/views/LoginOrRegisterView.vue";
import MainView from "@/views/MainView.vue";
import {useAuthStore} from "@/stores/auth.js";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
    },
    {
      path: '/login',
      name: 'login',
      component: LoginOrRegisterView,
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
  const authStore = useAuthStore()
  const isLoggedIn = authStore.token !== null

  if (/*to.meta.requiresAuth &&*/ !isLoggedIn) {
    next('/login')
  } else {
    next()
  }
})
export default router
