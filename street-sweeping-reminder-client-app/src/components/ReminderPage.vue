<script setup>
import { onMounted } from 'vue'
import ReminderDetails from './ReminderDetails.vue'
import { useRemindersStore } from '@/stores/reminder.js'

const remindersStore = useRemindersStore()

function handleReminderDeleted(deletedReminderId) {
  console.log(`Parent notified: Reminder ${deletedReminderId} was deleted.`)
  // The list 'remindersStore.reminders' should update automatically via Pinia reactivity
  // because the store action modified the state array.

  // You DON'T typically need to manually filter the list here:
  // remindersStore.reminders = remindersStore.reminders.filter(r => r.id !== deletedReminderId); // AVOID THIS

  // Instead, use this handler for side effects, like showing a success message:
  // toast.success(`Reminder (ID: ${deletedReminderId}) deleted successfully!`);
  alert(`Reminder (ID: ${deletedReminderId}) deleted successfully!`) // Simple browser alert
}

onMounted(() => {
  remindersStore.getReminders()
})
</script>

<template>
  <div class="md:ml-auto">
    <div class="flex h-20 items-center justify-center">
      <router-link
        to="/createReminder"
        class="text-white bg-blue-600 hover:bg-gray-900 hover:text-white rounded-md px-6 py-2"
        >Create New Reminder</router-link
      >
    </div>
  </div>

  <ul class="space-y-4">
    <li
      v-for="r in remindersStore.reminders"
      :key="r.id"
      class="border border-gray-200 bg-white rounded-xl shadow-md overflow-hidden"
    >
      <ReminderDetails :reminder="r" @deleted="handleReminderDeleted" />
    </li>
  </ul>
</template>
