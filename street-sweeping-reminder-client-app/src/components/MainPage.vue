<script setup>
import NavBar from './NavBar.vue'
import HeroSection from './HeroSection.vue'
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
  <HeroSection :subtitle="displayName" />

  <div class="md:ml-auto">
    <div class="flex space-x-2 justify-evenly">
      <a
        href="/reminders"
        class="text-white bg-blue-600 hover:bg-gray-900 hover:text-white rounded-md px-3 py-2"
        >Reminders</a
      >
    </div>
  </div>
</template>

<style scoped></style>
