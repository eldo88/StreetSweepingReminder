<script setup>
import { RouterView } from 'vue-router'
import { ref, onMounted, onUnmounted } from 'vue'
import LoginModal from '@/components/LoginModal.vue'

const showLogin = ref(false)

const handleUnauthorized = () => {
  showLogin.value = true
}

onMounted(() => {
  window.addEventListener('unauthorized', handleUnauthorized)
})

onUnmounted(() => {
  window.removeEventListener('unauthorized', handleUnauthorized)
})
</script>

<template>
  <RouterView />
  <LoginModal :visible="showLogin" @close="showLogin = false" />
</template>
