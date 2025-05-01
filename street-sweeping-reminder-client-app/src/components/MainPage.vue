<script setup>
import NavBar from './NavBar.vue'
import HeroSection from './HeroSection.vue'
import { useAuthStore } from '@/stores/auth'
import { useRemindersStore } from '@/stores/reminder'
import { useStreetsStore } from '@/stores/streets'
import { ref, onMounted, computed } from 'vue'

const authStore = useAuthStore()
const remindersStore = useRemindersStore()
const streetsStore = useStreetsStore()
var displayName = ref('')
const isLoading = ref(true)
const error = ref(null)
const streetName = ref('')

const getStreetName = async (streetId) => {
  try {
    const street = await streetsStore.getStreet(streetId)
    if (street) {
      streetName.value = street.name
    } else {
      console.error('Street not found for ID:', streetId)
    }
  } catch (error) {
    console.error('Error fetching street name:', error)
  }
}

const calendarAttributes = computed(() => {
  if (!remindersStore.reminders || remindersStore.reminders.length === 0) {
    console.log('No reminders found.')
    return []
  }

  const attributes = []

  remindersStore.reminders.forEach((reminder) => {
    // Ensure the reminder has a schedule and it's an array
    if (reminder.reminderSchedule?.schedule && Array.isArray(reminder.reminderSchedule.schedule)) {
      getStreetName(reminder.streetId)
      reminder.reminderSchedule.schedule.forEach((scheduleItem) => {
        // Only include active schedule items
        if (scheduleItem.isActive && scheduleItem.nextNotificationDate) {
          try {
            const eventDate = new Date(scheduleItem.nextNotificationDate)
            // Format the date/time for the popover for better readability
            const formattedTime = eventDate.toLocaleTimeString([], {
              hour: '2-digit',
              minute: '2-digit',
            })

            attributes.push({
              key: `schedule-${scheduleItem.id}`,
              highlight: {
                // Or use 'dot', 'bar', etc. See v-calendar docs
                color: 'blue', // Example color
                fillMode: 'light',
              },
              popover: {
                // Content to show on hover/click
                label: `${reminder.title} at ${formattedTime}`, // Display reminder title and time
                visibility: 'hover', // Show on hover
                // You can add more complex HTML here if needed
              },
              dates: eventDate, // The date for this specific event
            })
          } catch (e) {
            console.error(
              `Invalid date format for schedule item ${scheduleItem.id}:`,
              scheduleItem.scheduledDateTimeUtc,
              e,
            )
            // Skip this item if the date is invalid
          }
        }
      })
      reminder.streetSweepingSchedule.schedule.forEach((sweepingSchedule) => {
        // Only include active schedule items
        if (sweepingSchedule.streetSweepingDate) {
          try {
            const eventDate = new Date(sweepingSchedule.streetSweepingDate)

            attributes.push({
              key: `sweeping-${sweepingSchedule.id}`,
              highlight: {
                // Or use 'dot', 'bar', etc. See v-calendar docs
                color: 'red', // Example color
                fillMode: 'light',
              },
              popover: {
                // Content to show on hover/click
                label: `Street Sweeping: ${streetName.value}`, // Display reminder title and time
                visibility: 'hover', // Show on hover
                // You can add more complex HTML here if needed
              },
              dates: eventDate, // The date for this specific event
            })
          } catch (e) {
            console.error(
              `Invalid date format for street sweeping item ${sweepingSchedule.id}:`,
              sweepingSchedule.scheduledDateTimeUtc,
              e,
            )
            // Skip this item if the date is invalid
          }
        }
      })
    }
  })

  // console.log("Generated Calendar Attributes:", attributes); // For debugging
  return attributes
})

onMounted(async () => {
  displayName.value = authStore.getEmail

  try {
    await remindersStore.getReminders()
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
    <h2 class="text-2xl font-semibold mb-4 text-center">Upcoming Street Sweeping</h2>

    <div v-if="isLoading" class="text-center text-gray-500">Loading calendar...</div>
    <div v-else-if="error" class="text-center text-red-500">
      {{ error }}
    </div>
    <!-- V-Calendar Component -->
    <div v-else-if="calendarAttributes.length > 0">
      <v-calendar
        :attributes="calendarAttributes"
        is-expanded
        :rows="1"
        title-position="left"
        class="border rounded-md shadow-md"
      />
      <!-- You can customize appearance further:
          :rows="2" for 2 months
          :is-dark="true" for dark mode
          etc. Check v-calendar docs -->
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
