<script setup>
import { onMounted } from 'vue'
import ReminderForm from './ReminderForm.vue'
import ReminderDetails from './ReminderDetails.vue'
import { useRemindersStore } from '@/stores/reminder.js'

const remindersStore = useRemindersStore()

onMounted(() => {
  remindersStore.getReminders()
})
</script>

<template>
  <ReminderForm />

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
