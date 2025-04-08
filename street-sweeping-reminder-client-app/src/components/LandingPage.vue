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
  <div class="flex flex-col gap-3">
    <button
      type="button"
      @click="register"
      class="w-full bg-green-700 hover:bg-green-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
    >
      Register
    </button>

    <button
      type="button"
      @click="login"
      class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
    >
      Login
    </button>
  </div>
</template>
