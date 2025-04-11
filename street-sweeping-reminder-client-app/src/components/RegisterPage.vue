<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'
import router from '@/router/index.js'

const email = ref('')
const password = ref('')

const store = useAuthStore()
const handleRegister = async () => {
  try {
    await store.register({ email: email.value, password: password.value })
    console.log('Registered and signed in!')
    await router.push('/main')
  } catch (error) {
    console.error('Registration failed', error)
    // Show error to user
  }
}
</script>

<template>
  <section class="bg-blue-50 py-10">
    <div class="container mx-auto max-w-md">
      <!-- Form Container -->
      <div class="bg-white shadow-md rounded-lg px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Create Account or Sign In</h2>

        <form @submit.prevent="handleRegister">
          <!-- Email Field -->
          <div class="mb-4">
            <label for="email" class="block text-gray-700 text-sm font-bold mb-2">
              Email Address <span class="text-red-500">*</span>
            </label>
            <input
              v-model="email"
              type="email"
              id="email"
              name="email"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:border-green-500 focus:ring-1 focus:ring-green-500"
              placeholder="Enter your email"
              required
            />
          </div>

          <!-- Password Field -->
          <div class="mb-6">
            <label for="password" class="block text-gray-700 text-sm font-bold mb-2">
              Password <span class="text-red-500">*</span>
            </label>
            <input
              v-model="password"
              type="password"
              id="password"
              name="password"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 mb-3 leading-tight focus:outline-none focus:shadow-outline focus:border-green-500 focus:ring-1 focus:ring-green-500"
              placeholder="Enter your password"
              required
            />
          </div>

          <!-- Button Group -->
          <div class="flex flex-col gap-3">
            <button
              type="button"
              @click="handleRegister"
              class="w-full bg-green-700 hover:bg-green-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            >
              Register
            </button>
          </div>
        </form>
      </div>
    </div>
  </section>
</template>
