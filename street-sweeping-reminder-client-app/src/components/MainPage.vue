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
    <div class="flex h-20 items-center justify-center gap-4 flex-wrap">
      <router-link
        to="/reminders"
        class="text-white bg-blue-600 hover:bg-gray-900 rounded-md px-6 py-2"
      >
        Reminders
      </router-link>
      <router-link
        to="/createReminder"
        class="text-white bg-blue-600 hover:bg-gray-900 rounded-md px-6 py-2"
      >
        Create New Reminder
      </router-link>
    </div>
  </div>
</template>

<style scoped></style>
