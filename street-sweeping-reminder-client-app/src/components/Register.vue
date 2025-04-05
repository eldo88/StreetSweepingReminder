<script setup>
import {registerUserAsync} from '@/services/authApi.js'
import {ref} from "vue";

const username = ref('');
const email = ref('');
const password = ref('');

const handleRegister = () => {

  registerUserAsync(username.value, email.value, password.value)
    .then(response => {
      console.log('Registration successful:', response);
      // Add logic here for successful registration (e.g., redirect, show message)
    })
    .catch(error => {
      console.error('Registration failed:', error);
      // Add logic here for handling registration errors (e.g., show error message)
    });
}
</script>

<template>

  <section class="bg-blue-50 py-10">
    <div class="container mx-auto max-w-md">
      <!-- Form Container -->
      <div class="bg-white shadow-md rounded-lg px-8 pt-6 pb-8 mb-4">
        <!-- Optional: Add a title consistent with other sections -->
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">
          Create Account
        </h2>

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
            <!-- Optional: Add password requirements info here -->
            <!-- <p class="text-gray-600 text-xs italic">Must be at least 8 characters long.</p> -->
          </div>

          <!-- Submit Button -->
          <div class="flex items-center justify-between">
            <button
              type="submit"
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
<style scoped>

</style>
