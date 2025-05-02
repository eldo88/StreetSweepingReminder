<script setup>
const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="bg-green-700 text-white mt-auto dark:bg-gray-800">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="flex h-16 items-center justify-between">
        <div class="flex-1 text-center">
          <p class="text-sm">
            &copy; {{ currentYear }} Denver Street Sweeping Reminder. All rights reserved.
          </p>
        </div>
        <div class="flex space-x-4">
          <a
            href="https://www.denvergov.org/Government/Agencies-Departments-Offices/Agencies-Departments-Offices-Directory/Department-of-Transportation-and-Infrastructure/Programs-Services/Street-Sweeping"
            target="_blank"
            rel="noopener noreferrer"
            class="text-white hover:text-green-200 dark:hover:text-green-500"
          >
            Denver Street Sweeping Info
          </a>
          <span class="text-green-400">|</span>
          <router-link
            to="/privacy-policy"
            class="text-white hover:text-green-200 dark:hover:text-green-500"
          >
            Privacy Policy
          </router-link>
          <span class="text-green-400">|</span>
          <router-link
            to="/terms-of-service"
            class="text-white hover:text-green-200 dark:hover:text-green-500"
          >
            Terms of Service
          </router-link>
        </div>
      </div>
    </div>
  </footer>
</template>
