<script setup>
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import Datepicker from '@vuepic/vue-datepicker'
import '@vuepic/vue-datepicker/dist/main.css'
import { useRemindersStore } from '@/stores/reminder'

const schema = toTypedSchema(
  z.object({
    title: z.string().min(1, 'Title is required'),
    message: z.string().min(1, 'Message is required'),
    phoneNumber: z.string().min(10, 'Phone number is required'),
    street: z.string().min(1, 'Street is required'),
    zip: z.string().regex(/^\d{5}$/, 'Must be a valid 5-digit ZIP code'),
    reminderDate: z.date({ required_error: 'Reminder Date is required' }),
    isRecurring: z.boolean().default(true),
    streetSweepingDate: z.date({ required_error: 'Street Sweeping Date is required' }),
  }),
)

function onSubmit(values) {
  console.log('Form submitted:', values)
  const reminderStore = useRemindersStore()
  reminderStore.createReminder(values)
}
</script>

<template>
  <section class="bg-blue-50 py-10">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <div class="bg-white shadow-md rounded-lg px-4 sm:px-8 pt-6 pb-8 mb-4">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Schedule Text Reminder</h2>

        <Form @submit="onSubmit" :validation-schema="schema">
          <!-- Title -->
          <div class="mb-4">
            <label for="title" class="block text-gray-700 text-sm font-bold mb-2">
              Title <span class="text-red-500">*</span>
            </label>
            <Field
              id="title"
              name="title"
              type="text"
              placeholder="Reminder title"
              class="input-field"
            />
            <ErrorMessage name="title" class="text-red-500 text-xs mt-1" />
          </div>

          <!--Phone Number-->
          <div class="mb-4">
            <label for="phoneNumber" class="block text-gray-700 text-sm font-bold mb-2">
              Phone Number <span class="text-red-500">*</span>
            </label>
            <Field
              id="phoneNumber"
              name="phoneNumber"
              type="phoneNumber"
              placeholder="Phone Number"
              class="input-field"
            />
            <ErrorMessage name="title" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- Message -->
          <div class="mb-4">
            <label for="message" class="block text-gray-700 text-sm font-bold mb-2">
              Message <span class="text-red-500">*</span>
            </label>
            <Field
              id="message"
              name="message"
              type="text"
              placeholder="Reminder message"
              class="input-field"
            />
            <ErrorMessage name="message" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- Street -->
          <div class="mb-4">
            <label for="street" class="block text-gray-700 text-sm font-bold mb-2">
              Street Name <span class="text-red-500">*</span>
            </label>
            <Field
              id="street"
              name="street"
              type="text"
              placeholder="e.g. Main Street"
              class="input-field"
            />
            <ErrorMessage name="street" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- ZIP Code -->
          <div class="mb-4">
            <label for="zip" class="block text-gray-700 text-sm font-bold mb-2">
              ZIP Code <span class="text-red-500">*</span>
            </label>
            <Field id="zip" name="zip" type="text" placeholder="e.g. 90210" class="input-field" />
            <ErrorMessage name="zip" class="text-red-500 text-xs mt-1" />
          </div>

          <!-- Street Sweeping Date Picker -->
          <div class="mb-6">
            <label for="streetSweepingDate" class="block text-gray-700 text-sm font-bold mb-2">
              First Street Sweeping Date <span class="text-red-500">*</span>
            </label>
            <Field name="streetSweepingDate" v-slot="{ field, errors }">
              <Datepicker
                v-bind="field"
                :model-value="field.value"
                @update:model-value="field.onChange"
                input-class-name="input-field"
                placeholder="Pick a date for the first street sweeping day"
              />
              <span class="text-red-500 text-xs mt-1 block">{{ errors[0] }}</span>
            </Field>
          </div>

          <!-- Reminder Date Picker -->
          <div class="mb-6">
            <label for="reminderDate" class="block text-gray-700 text-sm font-bold mb-2">
              Reminder Date <span class="text-red-500">*</span>
            </label>
            <Field name="reminderDate" v-slot="{ field, errors }">
              <Datepicker
                v-bind="field"
                :model-value="field.value"
                @update:model-value="field.onChange"
                input-class-name="input-field"
                placeholder="Pick a date for the first reminder"
              />
              <span class="text-red-500 text-xs mt-1 block">{{ errors[0] }}</span>
            </Field>
          </div>

          <!-- Is Recurring Toggle Switch -->
          <div class="mb-6">
            <label for="isRecurring" class="flex items-center cursor-pointer">
              <Field name="isRecurring" type="checkbox" v-slot="{ field }">
                <input
                  type="checkbox"
                  v-bind="field"
                  :checked="field.value ?? true"
                  @change="field.onChange($event.target.checked)"
                  class="sr-only peer"
                  id="isRecurring"
                />
                <div
                  class="w-11 h-6 bg-gray-300 peer-checked:bg-green-500 rounded-full relative transition-colors duration-300"
                >
                  <div
                    class="absolute left-1 top-1 bg-white w-4 h-4 rounded-full transition-transform duration-300 peer-checked:translate-x-full"
                  ></div>
                </div>
              </Field>
              <span class="ml-3 text-sm font-medium text-gray-700">Recurring? (April - Nov)</span>
            </label>
          </div>

          <div class="flex flex-col gap-3">
            <button
              type="submit"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out"
            >
              Save Reminder
            </button>
          </div>
        </Form>
      </div>
    </div>
  </section>
</template>

<style scoped lang="postcss">
.input-field {
  @apply shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500;
}
</style>
