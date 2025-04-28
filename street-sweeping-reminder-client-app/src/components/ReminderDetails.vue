<script setup>
import { computed, ref, onMounted, defineEmits } from 'vue'
import { useStreetsStore } from '@/stores/streets'
import { useRemindersStore } from '@/stores/reminder'
import { formatDate } from '@/utils/dateUtils.js'

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
  if (schedule && Array.isArray(schedule) && schedule.length > 0) {
    const today = new Date()
    const nextDate = schedule
      .map((s) => new Date(s.nextNotificationDate))
      .sort((a, b) => a - b)
      .find((date) => date > today)
    return nextDate ? formatDate(nextDate.toDateString()) : 'No upcoming reminders'
  }
  if (!schedule || !Array.isArray(schedule)) {
    return 'Schedule data unavailable'
  }
  if (schedule.length === 0) {
    return 'No schedule entries'
  }
  return 'No schedule available'
})

const streetSweepingScheduleList = computed(() => {
  return props.reminder.streetSweepingSchedule?.schedule ?? []
})

const reminderScheduleList = computed(() => {
  return props.reminder.reminderSchedule?.schedule ?? []
})

const dayOfWeek = computed(() => {
  const day = props.reminder.streetSweepingSchedule?.dayOfWeek
  if (day === 0) {
    return 'Sunday'
  } else if (day === 1) {
    return 'Monday'
  } else if (day === 2) {
    return 'Tuesday'
  } else if (day === 3) {
    return 'Wednesday'
  } else if (day === 4) {
    return 'Thursday'
  } else if (day === 5) {
    return 'Friday'
  } else if (day === 6) {
    return 'Saturday'
  }
  return 'Unknown Day'
})

const sideOfStreet = computed(() => {
  let side = props.reminder.streetSweepingSchedule?.sideOfStreet
  if (side === 0) {
    return 'North'
  } else if (side === 1) {
    return 'South'
  } else if (side === 2) {
    return 'East'
  } else if (side === 3) {
    return 'West'
  }
  return 'Unknown Side'
})

onMounted(() => {
  fetchStreetDetails(props.reminder.streetId)
})
</script>

<template>
  <div class="bg-white rounded-xl shadow-md relative p-4 max-w-3xl mx-auto my-4">
    <button
      @click="handleDeleteReminder"
      :disabled="isDeleting"
      class="absolute top-2 right-2 z-10 px-3 py-1 bg-red-100 text-red-700 text-xs font-medium rounded-md hover:bg-red-200 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-1 disabled:opacity-60 disabled:cursor-not-allowed transition-colors duration-150 ease-in-out"
      aria-label="Delete reminder"
    >
      {{ isDeleting ? 'Deleting...' : 'Delete' }}
      <!-- Show text, change on loading -->
    </button>

    <div class="mb-6">
      <h1 class="text-xl font-semibold text-gray-800 my-2">
        {{ reminder.title || 'Street Reminder' }}
      </h1>
      <div class="border-t border-gray-300 mb-5"></div>
    </div>
    <div class="flex flex-col md:flex-row gap-6 md:gap-8">
      <div class="flex-1">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Details</h3>
        <div class="space-y-2 text-sm text-gray-600">
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
          <ul class="space-y-1 text-sm list-disc list-inside text-gray-600">
            <li v-for="item in reminderScheduleList" :key="item.id">
              {{ formatDate(item.nextNotificationDate) }}
            </li>
          </ul>
        </div>
        <div v-else class="text-sm text-gray-500 italic">No schedule data available.</div>
      </div>

      <div class="md:w-1/3">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Full Street Sweeping Schedule</h3>
        <div v-if="streetSweepingScheduleList.length > 0">
          <ul class="space-y-1 text-sm list-disc list-inside text-gray-600">
            <li v-for="item in streetSweepingScheduleList" :key="item.id">
              {{ formatDate(item.streetSweepingDate) }}
            </li>
          </ul>
        </div>
        <div v-else class="text-sm text-gray-500 italic">No schedule data available.</div>
      </div>
    </div>
  </div>
</template>
