<script setup>
import { onMounted, ref } from 'vue'
import { useAuthStore } from '@/stores/auth.js'
import router from '@/router/index.js'
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'

const email = ref('')
const password = ref('')

const store = useAuthStore()

const schema = toTypedSchema(
  z.object({
    email: z.string().email('Email not in valid format'),
    password: z.string().min(5, 'Password must be 5 characters'),
  }),
)

const handleSignIn = () => {
  store
    .login({ email: email.value, password: password.value })
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
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <!-- Form Container -->
      <div class="bg-white shadow-md rounded-lg px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Login</h2>

        <Form @submit="handleSignIn" :validation-schema="schema">
          <!-- Email Field -->
          <div class="mb-4">
            <label for="email" class="block text-gray-700 text-sm font-bold mb-2">
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
            <label for="password" class="block text-gray-700 text-sm font-bold mb-2">
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
              class="w-full bg-green-700 hover:bg-green-600 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            >
              Register
            </button>
          </div>
        </Form>
      </div>
    </div>
  </section>
</template>

<style scoped lang="postcss">
.input-field {
  @apply shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500;
}
</style>
