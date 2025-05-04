<script setup>
const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="bg-green-700 text-white mt-auto dark:bg-gray-800">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div
        class="flex flex-col md:flex-row h-auto md:h-16 items-center justify-between py-4 md:py-0 space-y-4 md:space-y-0"
      >
        <div
          class="flex flex-wrap justify-center md:justify-end space-x-2 md:space-x-4 text-xs md:text-sm"
        >
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
        <div class="w-1/2 h-px bg-green-500 md:hidden my-2"></div>
        <div class="text-center md:text-left w-full md:w-auto">
          <p class="text-xs md:text-sm">
            &copy; {{ currentYear }} Denver Street Sweeping Reminder. All rights reserved.
          </p>
        </div>
      </div>
    </div>
  </footer>
</template>
