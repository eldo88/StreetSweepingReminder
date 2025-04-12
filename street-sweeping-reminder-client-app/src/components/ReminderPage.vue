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
  <div class="space-y-4">
    <ReminderDetails v-for="r in remindersStore.reminders" :key="r.id" :reminder="r" />
  </div>
</template>
