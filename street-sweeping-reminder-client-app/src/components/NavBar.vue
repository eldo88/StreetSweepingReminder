<script setup>
import logo from '@/assets/logo.svg'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { ref, onMounted } from 'vue'

const auth = useAuthStore()
const router = useRouter()
const isDark = ref(
  localStorage.getItem('darkMode') === null ? true : localStorage.getItem('darkMode') === 'true',
)

function handleLogout() {
  auth.logout()
  router.push('/')
}

function toggleDarkMode() {
  isDark.value = !isDark.value
  document.documentElement.classList.toggle('dark')
  localStorage.setItem('darkMode', isDark.value)
}

onMounted(() => {
  if (isDark.value) {
    document.documentElement.classList.add('dark')
  } else {
    document.documentElement.classList.remove('dark')
  }

  localStorage.setItem('darkMode', isDark.value)
})
</script>

<template>
  <nav class="bg-green-700 dark:bg-gray-800 border-b border-green-500">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="flex h-20 items-center justify-between">
        <div class="flex flex-1 items-center justify-center md:items-stretch md:justify-start">
          <!-- Logo -->
          <RouterLink to="/home" class="flex flex-shrink-0 items-center mr-4">
            <img class="h-10 w-auto" :src="logo" alt="Vue Jobs" />
            <span class="hidden md:block text-white text-2xl font-bold ml-2">
              Denver Street Sweeping Reminder
            </span>
          </RouterLink>
          <div class="md:ml-auto">
            <div class="flex space-x-2 items-center">
              <button
                @click="toggleDarkMode"
                class="text-white bg-green-900 dark:bg-gray-700 hover:bg-gray-900 hover:text-white rounded-md px-3 py-2"
              >
                <span v-if="isDark">☀️</span>
                <span v-else>🌙</span>
              </button>
              <router-link
                to="/home"
                class="text-white bg-green-900 dark:bg-gray-700 hover:bg-gray-900 dark:hover:bg-gray-600 hover:text-white rounded-md px-3 py-2"
                >Home</router-link
              >
              <router-link
                to="/reminders"
                class="text-white bg-green-900 dark:bg-gray-700 hover:bg-gray-900 dark:hover:bg-gray-600 hover:text-white rounded-md px-3 py-2"
                >Manage Reminders</router-link
              >

              <router-link
                to="#"
                @click.prevent="handleLogout"
                class="text-white bg-green-900 dark:bg-gray-700 hover:bg-gray-900 dark:hover:bg-gray-600 hover:text-white rounded-md px-3 py-2"
              >
                Logout
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>
