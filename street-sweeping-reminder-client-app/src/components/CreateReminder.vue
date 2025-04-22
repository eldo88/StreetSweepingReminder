<script setup>
import ReminderForm from './ReminderForm.vue'
import StreetSweepingForm from './StreetSweepingForm.vue'
import { ref } from 'vue'

const isReminderFormVisible = ref(false)
const selectedStreetId = ref(null)

function handleStreetSelected(streetId) {
  console.log('Parent received streetSelected event with ID:', streetId)
  selectedStreetId.value = streetId
}
</script>

<template>
  <StreetSweepingForm
    v-model:isReminderFormVisible="isReminderFormVisible"
    @streetSelected="handleStreetSelected"
  />
  <div v-if="isReminderFormVisible">
    <br />
    <ReminderForm :street-id="selectedStreetId" />
  </div>
</template>
