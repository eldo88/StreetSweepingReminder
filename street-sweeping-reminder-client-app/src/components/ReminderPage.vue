<script setup>
import { onMounted, ref } from 'vue'
import ReminderForm from './ReminderForm.vue'
import ReminderDetails from './ReminderDetails.vue'
import { useRemindersStore } from '@/stores/reminder.js'
import { useAuthStore } from '@/stores/auth.js'

const remindersStore = useRemindersStore()
var reminders = ref([])
const authStore = useAuthStore()
var userId = ''

onMounted(() => {
  userId = authStore.getUserId
  reminders.value = remindersStore.getReminders(userId)
})
</script>

<template>
  <ReminderForm />
  <ReminderDetails reminder="reminders" />
</template>
