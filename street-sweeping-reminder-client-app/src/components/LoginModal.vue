<script setup>
import LoginForm from './LoginForm.vue'
defineProps({ visible: Boolean })
const emit = defineEmits(['close'])

const close = () => emit('close')
</script>

<template>
  <div
    v-if="visible"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50"
  >
    <div class="bg-white rounded-lg p-6 shadow-lg w-full max-w-md relative">
      <button @click="close" class="absolute top-2 right-2 text-gray-500 hover:text-gray-700">
        ✕
      </button>
      <LoginForm
        :redirect-after-login="false"
        @success="close"
        :login-form-message="'Please login agian to continue'"
      />
    </div>
  </div>
</template>
