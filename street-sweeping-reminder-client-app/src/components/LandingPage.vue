<script setup>
import router from '@/router/index.js'
const register = () => {
  router.push('/register')
}

const login = () => {
  router.push('login')
}
</script>

<template>
  <main class="min-h-screen flex items-center justify-center bg-gray-100 px-4">
    <div class="bg-white rounded-2xl shadow-xl p-10 w-full max-w-md space-y-6 text-center">
      <h1 class="text-3xl font-bold text-gray-900">Welcome to Street Sweeping Reminder</h1>
      <p class="text-gray-500 text-sm">Never forget about a street sweeping day again</p>

      <div class="space-y-3">
        <button
          @click="register"
          class="w-full bg-green-600 hover:bg-green-500 text-white font-semibold py-2 rounded-xl transition duration-150"
        >
          Register
        </button>

        <button
          @click="login"
          class="w-full bg-blue-600 hover:bg-blue-500 text-white font-semibold py-2 rounded-xl transition duration-150"
        >
          Login
        </button>
      </div>

      <a
        href="https://github.com/eldo88/StreetSweepingReminder"
        target="_blank"
        class="inline-block p-4 black hover:text-gray-500 transition duration-150"
      >
        <i class="fab fa-github text-2xl"></i>
      </a>
    </div>
  </main>
</template>
