<script setup>
import NavBar from './NavBar.vue'
import Hero from './Hero.vue'
import { useAuthStore } from '@/stores/auth'
import { ref, onMounted } from 'vue'

const authStore = useAuthStore()
var displayName = ref('')

onMounted(() => {
  displayName.value = authStore.getEmail
})
</script>

<template>
  <NavBar />
  <Hero :subtitle="displayName" />
</template>

<style scoped></style>
