import { createRouter, createWebHashHistory } from 'vue-router'
import RegisterPageView from '@/views/RegisterPageView.vue'
import MainView from '@/views/MainView.vue'
import { useAuthStore } from '@/stores/auth.js'
import LoginPageView from '@/views/LoginPageView.vue'
import LandingPageView from '@/views/LandingPageView.vue'
import ReminderPageView from '@/views/ReminderPageView.vue'
import CreateReminderView from '@/views/CreateReminderView.vue'
import PrivacyPolicyView from '@/views/PrivacyPolicyView.vue'
import TermsOfServiceView from '@/views/TermsOfServiceView.vue'

const router = createRouter({
  history: createWebHashHistory(),
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
      path: '/home',
      name: 'home',
      component: MainView,
      meta: { requiresAuth: true },
    },
    {
      path: '/reminders',
      name: 'reminders',
      component: ReminderPageView,
      meta: { requiresAuth: true },
    },
    {
      path: '/createReminder',
      name: 'createReminder',
      component: CreateReminderView,
      meta: { requiresAuth: true },
    },
    {
      path: '/privacy-policy',
      name: 'privacy-policy',
      component: PrivacyPolicyView,
      meta: {
        requiresAuth: false,
      },
    },
    {
      path: '/terms-of-service',
      name: 'terms-of-service',
      component: TermsOfServiceView,
      meta: {
        requiresAuth: false,
      },
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
