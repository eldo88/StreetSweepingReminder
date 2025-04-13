<script setup>
import { onMounted } from 'vue'
import ReminderDetails from './ReminderDetails.vue'
import { useRemindersStore } from '@/stores/reminder.js'

const remindersStore = useRemindersStore()

onMounted(() => {
  remindersStore.getReminders()
})
</script>

<template>
  <div class="md:ml-auto">
    <div class="flex h-20 items-center justify-center">
      <a
        href="/createReminder"
        class="text-white bg-blue-600 hover:bg-gray-900 hover:text-white rounded-md px-6 py-2"
        >Create New Reminder</a
      >
    </div>
  </div>

  <ul class="space-y-4">
    <li
      v-for="r in remindersStore.reminders"
      :key="r.id"
      class="border border-gray-200 bg-white rounded-xl shadow-md overflow-hidden"
    >
      <ReminderDetails :reminder="r" />
    </li>
  </ul>
</template>
