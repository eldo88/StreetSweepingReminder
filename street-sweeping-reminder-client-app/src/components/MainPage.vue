<script setup>
import NavBar from './NavBar.vue'
import HeroSection from './HeroSection.vue'
import { useAuthStore } from '@/stores/auth'
import { useRemindersStore } from '@/stores/reminder'
import { ref, onMounted } from 'vue'
import { generateCalendarAttributes } from '@/utils/calendarUtils'

const authStore = useAuthStore()
const remindersStore = useRemindersStore()
const displayName = ref('')
const isLoading = ref(true)
const error = ref(null)
const calendarAttributes = ref([])

const today = new Date()
const initialCalendarPage = {
  month: today.getMonth() + 1,
  year: today.getFullYear(),
}

onMounted(async () => {
  displayName.value = authStore.getEmail

  try {
    await remindersStore.getReminders()
    calendarAttributes.value = await generateCalendarAttributes()
  } catch (error) {
    console.error('Failed to fetch reminders:', error)
    error.value = 'Could not load reminder data. Please try again later.'
  } finally {
    isLoading.value = false
  }
})
</script>

<template>
  <NavBar />
  <HeroSection :subtitle="displayName" />

  <div class="md:ml-auto">
    <div class="flex h-20 items-center justify-center gap-4 flex-wrap">
      <router-link
        to="/reminders"
        class="text-white bg-blue-600 hover:bg-gray-900 rounded-md px-6 py-2"
      >
        Reminders
      </router-link>
      <router-link
        to="/createReminder"
        class="text-white bg-blue-600 hover:bg-gray-900 rounded-md px-6 py-2"
      >
        Create New Reminder
      </router-link>
    </div>
  </div>

  <div class="container mx-auto px-4 py-8">
    <h2 class="text-2xl font-semibold mb-4 text-center">Upcoming Street Sweeping And Reminders</h2>

    <div v-if="isLoading" class="text-center text-gray-500">Loading calendar...</div>
    <div v-else-if="error" class="text-center text-red-500">
      {{ error }}
    </div>
    <div v-else-if="calendarAttributes.length > 0">
      <v-calendar
        :attributes="calendarAttributes"
        :initial-page="initialCalendarPage"
        is-expanded
        :rows="1"
        title-position="left"
        class="border rounded-md shadow-md"
      />
    </div>
    <div v-else class="text-center text-gray-500">
      No upcoming street sweeping reminders found.
      <router-link to="/createReminder" class="text-blue-600 hover:underline"
        >Create one?</router-link
      >
    </div>
  </div>
</template>

<style scoped></style>
