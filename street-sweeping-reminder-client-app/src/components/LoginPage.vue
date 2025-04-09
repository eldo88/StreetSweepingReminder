<script setup>
import { onMounted, ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'
import router from '@/router/index.js'

const username = ref('')
const email = ref('')
const password = ref('')

const store = useAuthStore()

const handleSignIn = () => {
  store
    .login({ username: username.value, password: password.value })
    .then(async (response) => {
      console.log('Sign-in successful', response)
      await router.push('/main')
    })
    .catch((error) => {
      console.error('Login failed', error)
    })
}

onMounted(() => {
  store.initialize()
})
</script>

<template>
  <section class="bg-blue-50 py-10">
    <div class="container mx-auto max-w-md">
      <!-- Form Container -->
      <div class="bg-white shadow-md rounded-lg px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Create Account or Sign In</h2>

        <form @submit.prevent="handleSignIn">
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

          <!-- Username Field -->
          <div class="mb-4">
            <label for="username" class="block text-gray-700 text-sm font-bold mb-2">
              Username <span class="text-gray-500 text-xs">(Optional)</span>
            </label>
            <input
              v-model="username"
              type="text"
              id="username"
              name="username"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline focus:border-green-500 focus:ring-1 focus:ring-green-500"
              placeholder="Choose a username"
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
              @click="handleSignIn"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            >
              Login
            </button>
          </div>
        </form>
      </div>
    </div>
  </section>
</template>
