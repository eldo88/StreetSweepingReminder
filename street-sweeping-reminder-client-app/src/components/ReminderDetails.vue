<script setup>
import { computed } from 'vue'
import { useStreetsStore } from '@/stores/streets'

const props = defineProps({
  reminder: {
    type: Object,
    required: true,
  },
  default: () => ({
    id: 0,
    title: 'Default Title',
    message: 'No message available',
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
  }),
})

const streetsStore = useStreetsStore()

const getStreetName = (streetId) => {
  const street = streetsStore.streets.find((s) => s.id === streetId)
  return street ? street.name : 'Unknown Street'
}

const formatDate = (dateString) => {
  if (!dateString) return 'Invalid Date'
  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) {
      return 'Invalid Date'
    }

    return date.toLocaleDateString(undefined, {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    })
  } catch (error) {
    console.error('Error formatting date:', dateString, error)
    return 'Invalid Date'
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

const scheduleList = computed(() => {
  return props.reminder.streetSweepingSchedule?.schedule ?? []
})
</script>

<template>
  <div class="bg-white rounded-xl shadow-md relative p-4">
    <!-- Top Header Section -->
    <div class="mb-6">
      <!-- Example: Use title from reminder data -->
      <h1 class="text-xl font-semibold text-gray-800 my-2">
        {{ reminder.title || 'Street Reminder' }}
      </h1>
      <div class="border-t border-gray-300 mb-5"></div>
    </div>

    <!-- Main Content Area (using Flexbox) -->
    <!-- On medium screens and up (md:), display side-by-side -->
    <div class="flex flex-col md:flex-row gap-6 md:gap-8">
      <!-- Left Column: Details -->
      <div class="flex-1">
        {{}}
        <div class="mb-5 text-gray-700">
          {{ reminder.message }}
        </div>

        <h3 class="text-lg font-medium text-indigo-600 mb-3">Details</h3>
        <div class="space-y-2 text-sm text-gray-600">
          <div><strong>Phone Number:</strong> {{ reminder.phoneNumber }}</div>
          <div><strong>Next Sweeping:</strong> {{ nextStreetSweepingDateComputed }}</div>
          <div><strong>Status:</strong> {{ reminder.status }}</div>
          <!-- Add other details if needed -->
          <div><strong>Street:</strong> {{ getStreetName() }}</div>
        </div>
      </div>

      <!-- Right Column: Schedule -->
      <!-- Give it a specific width or let it flex. md:w-1/3 or md:w-1/2 are common -->
      <div class="md:w-1/3">
        <h3 class="text-lg font-medium text-indigo-600 mb-3">Full Schedule</h3>

        <!-- Conditional Rendering: Show only if schedule exists and has items -->
        <div v-if="scheduleList.length > 0">
          <!-- Option 1: Simple List -->
          <ul class="space-y-1 text-sm list-disc list-inside text-gray-600">
            <li v-for="item in scheduleList" :key="item.id">
              {{ formatDate(item.streetSweepingDate) }}
            </li>
          </ul>
        </div>
        <!-- Message if no schedule data -->
        <div v-else class="text-sm text-gray-500 italic">No schedule data available.</div>
      </div>
    </div>
    {{}}
  </div>
</template>
