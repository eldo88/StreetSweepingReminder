<script setup>
import { computed, ref, onMounted } from 'vue'
import { useStreetsStore } from '@/stores/streets'
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
    },
    streetId: null,
    reminderSchedule: {
      schedule: [],
    },
  }),
})

const streetsStore = useStreetsStore()
const streetName = ref('Loading...')

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

const streetSweepingScheduleList = computed(() => {
  return props.reminder.streetSweepingSchedule?.schedule ?? []
})

const reminderScheduleList = computed(() => {
  return props.reminder.reminderSchedule?.schedule ?? []
})

onMounted(() => {
  fetchStreetDetails(props.reminder.streetId)
})
</script>

<template>
  <div class="bg-white rounded-xl shadow-md relative p-4">
    <div class="mb-6">
      <h1 class="text-xl font-semibold text-gray-800 my-2">
        {{ reminder.title || 'Street Reminder' }}
      </h1>
      <div class="border-t border-gray-300 mb-5"></div>
    </div>
    <div class="flex flex-col md:flex-row gap-6 md:gap-8">
      <div class="flex-1">
        {{}}

        <h3 class="text-lg font-medium text-indigo-600 mb-3">Details</h3>
        <div class="space-y-2 text-sm text-gray-600">
          <div><strong>Phone Number:</strong> {{ reminder.phoneNumber }}</div>
          <div><strong>Next Sweeping:</strong> {{ nextStreetSweepingDateComputed }}</div>
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
    {{}}
  </div>
</template>
