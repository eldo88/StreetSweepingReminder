<script setup>
import { RouterView } from 'vue-router'
import { ref, onMounted, onUnmounted, watch } from 'vue'
import LoginModal from '@/components/LoginModal.vue'
import { useAuthStore } from '@/stores/auth'
import { setGlobalTimeout } from '@/utils/globalTimeout'

const showLogin = ref(false)
const authStore = useAuthStore()

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
  <RouterView />
  <LoginModal :visible="showLogin" @close="showLogin = false" @loggedIn="handleSuccessfulLogin" />
</template>
