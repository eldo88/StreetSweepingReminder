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
        <div class="text-gray-600 my-2">{{ reminder.status }}</div>
        <!-- <h3 class="text-xl font-bold">{{ reminder.title }}</h3> -->
      </div>

      <div class="mb-5">
        {{ reminder.message }}
      </div>

      <h3 class="text-green-500 mb-2">{{ reminder.phoneNumber }}</h3>

      <div class="border border-gray-100 mb-5"></div>

      <div class="flex flex-col lg:flex-row justify-between mb-4"></div>
    </div>
  </div>

  <!-- <div class="text-gray-600 my-2">{{ reminder.status }}</div>
  <h3 class="text-xl font-bold">Reminder #{{ reminder.id }}</h3>

  <div class="mb-5">
    {{ reminder.message }}
  </div>

  <h3 class="text-green-500 mb-2">{{ reminder.phoneNumber }}</h3> -->
</template>
