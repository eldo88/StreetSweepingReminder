<script setup>
import { RouterView } from 'vue-router'
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import LoginModal from '@/components/LoginModal.vue'
import { useAuthStore } from '@/stores/auth'
import { setGlobalTimeout } from '@/utils/globalTimeout'
import { useRoute } from 'vue-router'
import AppFooter from '@/components/AppFooter.vue'
import NavBar from '@/components/NavBar.vue'

const showLogin = ref(false)
const authStore = useAuthStore()
const route = useRoute()

const showNavBar = computed(() => {
  const publicRoutes = ['welcome', 'login', 'register']
  return !publicRoutes.includes(route.name)
})

const showFooter = computed(() => {
  const publicRoutes = ['welcome', 'login', 'register']
  return !publicRoutes.includes(route.name)
})

const handleUnauthorized = () => {
  showLogin.value = true
}

const handleSuccessfulLogin = () => {
  showLogin.value = false
  window.location.reload()
}

onMounted(() => {
  window.addEventListener('unauthorized', handleUnauthorized)

  if (authStore.isLoggedIn && authStore.tokenExpirationInMinutes) {
    setGlobalTimeout(authStore.tokenExpirationInMinutes)
  }
})

onUnmounted(() => {
  window.removeEventListener('unauthorized', handleUnauthorized)
})

watch(
  () => authStore.isLoggedIn,
  (isLoggedIn) => {
    if (isLoggedIn && authStore.tokenExpirationInMinutes) {
      setGlobalTimeout(authStore.tokenExpirationInMinutes)
    }
  },
)
</script>

<template>
  <div class="flex flex-col min-h-screen bg-white dark:bg-gray-900 transition-colors duration-200">
    <NavBar v-if="showNavBar" />

    <main class="flex-grow">
      <RouterView />
    </main>

    <AppFooter v-if="showFooter" />

    <LoginModal :visible="showLogin" @close="showLogin = false" @loggedIn="handleSuccessfulLogin" />
  </div>
</template>

<style>
html {
  height: 100%;
  background-color: white;
}

html.dark {
  background-color: #111827; /* Tailwind's gray-900 */
}

body {
  min-height: 100%;
  overscroll-behavior-y: none;
}
</style>
