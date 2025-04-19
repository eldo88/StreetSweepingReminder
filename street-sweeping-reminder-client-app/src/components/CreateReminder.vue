<script setup>
import ReminderForm from './ReminderForm.vue'
import StreetSweepingForm from './StreetSweepingForm.vue'
import { ref } from 'vue'

const isReminderFormVisible = ref(false)
</script>

<template>
  <StreetSweepingForm v-model:isReminderFormVisible="isReminderFormVisible" />
  <div v-if="isReminderFormVisible">
    <br />
    <ReminderForm />
  </div>
</template>
