<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'
import router from '@/router/index.js'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import { useToast } from 'vue-toastification'

const isLoading = ref(false)
const email = ref('')
const password = ref('')
const toast = useToast()

const store = useAuthStore()

const schema = toTypedSchema(
  z.object({
    email: z.string().email('Email not in valid format'),
    password: z.string().min(5, 'Password must be 5 characters'),
  }),
)

const handleRegister = async (values) => {
  isLoading.value = true
  try {
    await store.register(values)
    console.log('Registered and signed in!')
    isLoading.value = false
    await router.push('/home')
  } catch (error) {
    isLoading.value = false
    console.error('Registration failed', error)
    toast.error('Registration failed. Please check your credentials.' + error.message)
  }
}
</script>

<template>
  <section class="bg-blue-50 py-10 dark:bg-gray-900">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <!-- Form Container -->
      <div class="bg-white dark:bg-gray-800 shadow-md rounded-lg px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 dark:text-gray-300 mb-6">
          Create Account
        </h2>

        <Form @submit="handleRegister" :validation-schema="schema">
          <!-- Email Field -->
          <div class="mb-4">
            <label
              for="email"
              class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
            >
              Email Address <span class="text-red-500">*</span>
            </label>
            <Field
              v-model="email"
              type="email"
              id="email"
              name="email"
              placeholder="Enter your email"
              required
              class="input-field"
            />
            <ErrorMessage name="email" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- Password Field -->
          <div class="mb-6">
            <label
              for="password"
              class="block text-gray-700 dark:text-gray-300 text-sm font-bold mb-2"
            >
              Password <span class="text-red-500">*</span>
            </label>
            <Field
              v-model="password"
              type="password"
              id="password"
              name="password"
              placeholder="Enter your password"
              required
              class="input-field"
            />
            <ErrorMessage name="password" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- Button Group -->
          <div class="flex flex-col gap-3">
            <button
              type="submit"
              class="w-full bg-green-600 hover:bg-green-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            >
              <template v-if="isLoading">
                <svg
                  class="animate-spin -ml-1 mr-3 h-5 w-5 text-white"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                >
                  <circle
                    class="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    stroke-width="4"
                  ></circle>
                  <path
                    class="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  ></path>
                </svg>
                Logging in...
              </template>
              <template v-else> Login </template>
            </button>
          </div>
        </Form>
      </div>
    </div>
  </section>
</template>

<style scoped lang="postcss">
.input-field {
  @apply shadow appearance-none border rounded w-full py-2 px-3 text-gray-700
  leading-tight focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500
  dark:bg-gray-800 dark:border-gray-600 dark:text-gray-200
    dark:focus:border-green-500 dark:placeholder-gray-400;
}
</style>
