<script setup>
defineProps({
  reminder: Object,
})
</script>

<template>
  <!-- int Id, string Message, DateTime ScheduledDateTimeUtc, string Status, string PhoneNumber, int StreetId -->

  <div class="bg-white rounded-xl shadow-md relative">
    <div class="p-4">
      <div class="mb-6">
        <h1 class="text-black my-2">Yates Street</h1>
        <div class="border border-black mb-5"></div>
      </div>

      <div class="mb-5">
        {{ reminder.message }}
      </div>

      <h3 class="text-green-500 mb-2">Details</h3>

      <div class="flex flex-col lg:flex-row justify-between mb-4">
        <div>Phone Number: {{ reminder.phoneNumber }}</div>
        <div>Scheduled Date: {{ reminder.scheduledDateTimeUtc }}</div>
        <div>Status: {{ reminder.status }}</div>
      </div>
    </div>
  </div>
</template>
