<script setup>
import { computed, ref, onMounted, defineEmits } from 'vue'
import { useStreetsStore } from '@/stores/streets'
import { useRemindersStore } from '@/stores/reminder'
import { formatDate } from '@/utils/dateUtils.js'
import { getDayOfWeek, getSideOfStreet } from '@/utils/streetUtils.js'

const props = defineProps({
  reminder: {
    type: Object,
    required: true,
  },
  default: () => ({
    id: 0,
    title: 'Default Title',
    phoneNumber: 'N/A',
    status: 'N/A',
    streetSweepingSchedule: {
      dayOfWeek: null,
      weekOfMonth: null,
      year: null,
      streetId: null,
      schedule: [],
      sideOfStreet: null,
    },
    streetId: null,
    reminderSchedule: {
      schedule: [],
    },
  }),
})

const emit = defineEmits(['deleted'])

const reminderStore = useRemindersStore()

const streetsStore = useStreetsStore()
const streetName = ref('Loading...')
const isDeleting = ref(false)

async function fetchStreetDetails(streetId) {
  if (!streetId) {
    streetName.value = 'N/A'
    return
  }
  streetName.value = 'Loading...'
  try {
    const street = await streetsStore.getStreet(streetId)
    streetName.value = street ? street.streetName : 'Unknown Street'
  } catch (error) {
    console.error('Failed to fetch street details:', error)
    streetName.value = 'Error loading street'
  }
}

async function handleDeleteReminder() {
  if (
    !confirm(
      `Are you sure you want to delete the reminder "${props.reminder.title || 'this reminder'}"?`,
    )
  ) {
    return
  }

  isDeleting.value = true
  try {
    await reminderStore.deleteReminder(props.reminder.id)

    emit('deleted', props.reminder.id)

    console.log(`Reminder ${props.reminder.id} deletion process completed in component.`)
  } catch (error) {
    console.error(`UI Error: Failed to delete reminder ${props.reminder.id}.`, error)
    alert(`Failed to delete reminder: ${error.message || 'Please try again.'}`)
  } finally {
    isDeleting.value = false
  }
}

const nextStreetSweepingDateComputed = computed(() => {
  const schedule = props.reminder.streetSweepingSchedule?.schedule
  if (schedule && Array.isArray(schedule) && schedule.length > 0) {
    const today = new Date()
    const nextDate = schedule
      .map((s) => new Date(s.streetSweepingDate))
      .sort((a, b) => a - b)
      .find((date) => date > today)
    return nextDate ? formatDate(nextDate.toDateString()) : 'No upcoming dates'
  }
  if (!schedule || !Array.isArray(schedule)) {
    return 'Schedule data unavailable'
  }
  if (schedule.length === 0) {
    return 'No schedule entries'
  }
  return 'No schedule available'
})

const nextReminderDate = computed(() => {
  const schedule = props.reminder.reminderSchedule?.schedule
  if (!schedule || !Array.isArray(schedule)) {
    return 'Schedule data unavailable'
  }
  if (schedule.length === 0) {
    return 'No schedule entries'
  }

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const activeUpcomingSchedules = schedule.filter((item) => {
    if (!item.isActive || !item.nextNotificationDate) {
      return false
    }
    const itemDate = new Date(item.nextNotificationDate)

    return !isNaN(itemDate) && itemDate >= today
  })

  if (activeUpcomingSchedules.length === 0) {
    return 'No upcoming active reminders'
  }

  activeUpcomingSchedules.sort(
    (a, b) => new Date(a.nextNotificationDate) - new Date(b.nextNotificationDate),
  )

  const nextActiveItem = activeUpcomingSchedules[0]

  return formatDate(nextActiveItem.nextNotificationDate)
})

const streetSweepingScheduleList = computed(() => {
  return props.reminder.streetSweepingSchedule?.schedule ?? []
})

const reminderScheduleList = computed(() => {
  return props.reminder.reminderSchedule?.schedule ?? []
})

const dayOfWeek = computed(() => {
  const day = props.reminder.streetSweepingSchedule?.dayOfWeek
  return getDayOfWeek(day)
})

const sideOfStreet = computed(() => {
  let side = props.reminder.streetSweepingSchedule?.sideOfStreet
  return getSideOfStreet(side)
})

async function handleCancelIndividualReminder(reminderId, scheduleItemId) {
  console.log(`Attempting to cancel schedule item ${scheduleItemId} for reminder ${reminderId}`)
  try {
    await reminderStore.cancelIndividualReminder(reminderId, scheduleItemId) // Pass both IDs
    console.log(`Successfully requested cancellation for schedule item ${scheduleItemId}`)
  } catch (error) {
    console.error(`UI Error: Failed to cancel reminder schedule item ${scheduleItemId}:`, error)
    alert(`Failed to cancel reminder date: ${error.message || 'Please try again.'}`)
  }
}

onMounted(() => {
  fetchStreetDetails(props.reminder.streetId)
})
</script>

<template>
  <div
    class="bg-white rounded-xl shadow-md relative p-4 max-w-3xl mx-auto my-4 dark:bg-gray-800 dark:border dark:border-green-500"
  >
    <button
      @click="handleDeleteReminder"
      :disabled="isDeleting"
      class="absolute top-2 right-2 z-10 px-3 py-1 bg-red-100 text-red-700 text-xs font-medium rounded-md hover:bg-red-200 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-1 disabled:opacity-60 disabled:cursor-not-allowed transition-colors duration-150 ease-in-out"
      aria-label="Delete reminder"
    >
      {{ isDeleting ? 'Deleting...' : 'Delete' }}
    </button>

    <div class="mb-6">
      <h1 class="text-xl font-semibold text-gray-800 my-2 dark:text-gray-200">
        {{ reminder.title || 'Street Reminder' }}
      </h1>
      <div class="border-t border-gray-300 mb-5"></div>
    </div>
    <div class="flex flex-col md:flex-row gap-6 md:gap-8">
      <div class="flex-1">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Details</h3>
        <div class="space-y-2 text-sm text-gray-600 dark:text-gray-200">
          <div><strong>Phone Number:</strong> {{ reminder.phoneNumber }}</div>
          <div><strong>Next Reminder:</strong> {{ nextReminderDate }}</div>
          <div><strong>Next Sweeping:</strong> {{ nextStreetSweepingDateComputed }}</div>
          <div><strong>Street Sweeping Day:</strong> {{ dayOfWeek }}</div>
          <div><strong>Side:</strong> {{ sideOfStreet }}</div>
          <div><strong>Status:</strong> {{ reminder.status }}</div>
          <div><strong>Street:</strong> {{ streetName }}</div>
        </div>
      </div>

      <div class="md:w-1/3">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Your Reminder Schedule</h3>
        <div v-if="reminderScheduleList.length > 0">
          <ul class="space-y-1 text-sm list-none text-gray-600 dark:text-gray-200">
            <li
              v-for="item in reminderScheduleList"
              :key="item.id"
              class="flex items-start gap-2"
              :class="{ 'opacity-60': !item.isActive }"
            >
              <div class="flex items-center gap-2 mr-2">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke-width="1.5"
                  stroke="currentColor"
                  class="w-4 h-4 text-indigo-500 flex-shrink-0"
                  :class="item.isActive ? 'text-indigo-500' : 'text-gray-400'"
                  aria-hidden="true"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5M12 15h.008v.008H12V15zm0 2.25h.008v.008H12v-.008zM9.75 15h.008v.008H9.75V15zm0 2.25h.008v.008H9.75v-.008zm0 2.25h.008v.008H9.75v-.008zM7.5 15h.008v.008H7.5V15zm0 2.25h.008v.008H7.5v-.008zm0 2.25h.008v.008H7.5v-.008zm6.75-4.5h.008v.008h-.008v-.008zm0 2.25h.008v.008h-.008V15zm0 2.25h.008v.008h-.008v-.008zm2.25-4.5h.008v.008H16.5V15zm0 2.25h.008v.008H16.5v-.008z"
                  />
                </svg>
                <span :class="{ 'text-gray-500 line-through': !item.isActive }">{{
                  formatDate(item.nextNotificationDate)
                }}</span>
              </div>
              <button
                @click="handleCancelIndividualReminder(props.reminder.id, item.id)"
                type="button"
                class="ml-auto px-2 py-0.5 bg-gray-100 dark:bg-gray-400 dark:text-black dark:border dark:hover:border-red-500 dark: text-gray-700 text-xs font-medium rounded hover:bg-gray-200 focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:ring-offset-1 transition-colors duration-150 ease-in-out flex-shrink-0"
                :aria-label="`Cancel reminder for ${formatDate(item.nextNotificationDate)}`"
                :disabled="!item.isActive"
              >
                {{ item.isActive ? 'Cancel' : 'Cancelled' }}
              </button>
            </li>
          </ul>
        </div>
        <div v-else class="text-sm text-gray-500 italic">No schedule data available.</div>
      </div>

      <div class="md:w-1/3">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Full Street Sweeping Schedule</h3>
        <div v-if="streetSweepingScheduleList.length > 0">
          <ul class="space-y-1 text-sm list-none text-gray-600 dark:text-gray-200">
            <li
              v-for="item in streetSweepingScheduleList"
              :key="item.id"
              class="flex items-start gap-2"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke-width="1.5"
                stroke="currentColor"
                class="w-4 h-4 text-indigo-500 mt-1 flex-shrink-0"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5m-9-6h.008v.008H12v-.008ZM12 15h.008v.008H12v-.008ZM12 18h.008v.008H12v-.008ZM9.75 15h.008v.008H9.75v-.008ZM9.75 18h.008v.008H9.75v-.008ZM7.5 15h.008v.008H7.5v-.008ZM7.5 18h.008v.008H7.5v-.008ZM14.25 15h.008v.008H14.25v-.008ZM14.25 18h.008v.008H14.25v-.008ZM16.5 15h.008v.008H16.5v-.008ZM16.5 18h.008v.008H16.5v-.008Z"
                />
              </svg>
              <span>{{ formatDate(item.streetSweepingDate) }}</span>
            </li>
          </ul>
        </div>
        <div v-else class="text-sm text-gray-500 italic">No schedule data available.</div>
      </div>
    </div>
  </div>
</template>
