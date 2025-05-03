<script setup>
import { onMounted } from 'vue'
import ReminderDetails from './ReminderDetails.vue'
import { useRemindersStore } from '@/stores/reminder.js'

const remindersStore = useRemindersStore()

function handleReminderDeleted(deletedReminderId) {
  console.log(`Parent notified: Reminder ${deletedReminderId} was deleted.`)

  alert(`Reminder (ID: ${deletedReminderId}) deleted successfully!`)
}

onMounted(() => {
  remindersStore.getReminders()
})
</script>

<template>
  <div class="p-4">
    <div class="flex h-20 items-center justify-center mb-6">
      <router-link
        to="/createReminder"
        class="text-white bg-blue-600 hover:bg-gray-900 hover:text-white rounded-md px-6 py-2"
        >Create New Reminder</router-link
      >
    </div>
  </div>

  <div class="space-y-4 mb-4">
    <ReminderDetails
      :reminder="r"
      @deleted="handleReminderDeleted"
      v-for="r in remindersStore.reminders"
      :key="r.id"
      class="flex-shrink-0"
    />
    <div
      v-if="!remindersStore.reminders || remindersStore.reminders.length === 0"
      class="text-center text-gray-500 w-full py-8"
    >
      You haven't created any reminders yet.
    </div>
  </div>
</template>
