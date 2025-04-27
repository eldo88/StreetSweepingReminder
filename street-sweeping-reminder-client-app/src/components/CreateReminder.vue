<script setup>
import ReminderForm from './ReminderForm.vue'
import StreetSweepingForm from './StreetSweepingForm.vue'
import { ref } from 'vue'

const isReminderFormVisible = ref(false)
const selectedStreetId = ref(null)
const selectedSideOfStreet = ref(null)

function handleStreetSelected(streetId) {
  console.log('Parent received streetSelected event with ID:', streetId)
  selectedStreetId.value = streetId
}

function handleSideOfStreetSelected(side) {
  console.log('Parent received sideSelected event with side:', side)
  selectedSideOfStreet.value = side
}
</script>

<template>
  <StreetSweepingForm
    v-model:isReminderFormVisible="isReminderFormVisible"
    @streetSelected="handleStreetSelected"
    @sideSelected="handleSideOfStreetSelected"
  />
  <div v-if="isReminderFormVisible">
    <br />
    <ReminderForm :street-id="selectedStreetId" :side-of-street="selectedSideOfStreet" />
  </div>
</template>
