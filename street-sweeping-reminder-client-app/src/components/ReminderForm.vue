<script setup>
import { Form, Field, ErrorMessage } from 'vee-validate'
import { z } from 'zod'
import { toTypedSchema } from '@vee-validate/zod'
import Datepicker from '@vuepic/vue-datepicker'
import '@vuepic/vue-datepicker/dist/main.css'
import { useRemindersStore } from '@/stores/reminder'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'

const props = defineProps({
  streetId: {
    type: Number,
    default: null,
  },
  sideOfStreet: {
    type: Number,
    default: null,
  },
})

const schema = toTypedSchema(
  z.object({
    title: z.string().min(1, 'Title is required'),
    phoneNumber: z.string().min(10, 'Phone number is required'),
    consent: z.boolean().refine((val) => val === true, {
      message: 'You must agree to receive text notifications to create a reminder.',
    }),
    reminderDate: z.coerce.date({
      required_error: 'Reminder Date is required',
      invalid_type_error: 'Invalid date format',
    }),
    isRecurring: z.boolean().default(true),
  }),
)

const toast = useToast()
const router = useRouter()
const reminderStore = useRemindersStore()

async function onSubmit(values) {
  console.log('Street ID from prop:', props.streetId)
  console.log('Side of street from prop:', props.sideOfStreet)
  if (!props.streetId) {
    toast.error('Cannot create reminder: No street selected.')
    console.error('Attempted to submit reminder without a streetId prop.')
    return
  }

  try {
    await reminderStore.createReminder(values, props.streetId, props.sideOfStreet)
    toast.success('Reminder created successfully!')
    setTimeout(() => {
      router.push('/reminders')
    }, 1000)
  } catch (error) {
    console.error('Reminder creation failed:', error)
    toast.error('Failed to create reminder. Please try again.')
  }
}
</script>

<template>
  <section class="bg-blue-50 py-10 dark:bg-gray-900 dark:border-t dark:border-green-600">
    <div
      class="container mx-auto px-4 sm:px-6 lg:px-8 max-w-sm sm:max-w-md md:max-w-lg lg:max-w-xl"
    >
      <div class="bg-white shadow-md rounded-lg px-4 sm:px-8 pt-6 pb-8 mb-4 dark:bg-gray-800">
        <h2 class="text-2xl font-bold text-center text-gray-800 mb-6">Schedule Text Reminder</h2>

        <Form @submit="onSubmit" :validation-schema="schema">
          <!-- Title -->
          <div class="mb-4">
            <label
              for="title"
              class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
            >
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
            <label
              for="phoneNumber"
              class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
            >
              Phone Number <span class="text-red-500">*</span>
            </label>
            <Field
              id="phoneNumber"
              name="phoneNumber"
              type="phoneNumber"
              placeholder="Phone Number"
              class="input-field"
            />
            <ErrorMessage name="phoneNumber" class="text-red-500 text-xs mt-1" />
          </div>

          <div class="mb-4">
            <div class="flex items-start">
              <Field
                name="consent"
                type="checkbox"
                id="consentCheckbox"
                :value="true"
                class="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500 mt-1 mr-2 flex-shrink-0"
              />
              <label for="consentCheckbox" class="text-sm text-gray-700 dark:text-gray-200">
                I agree to receive text notifications. To cancel,
                <router-link to="/reminders" class="text-blue-600 hover:underline"
                  >manage your notifications</router-link
                >
                or reply STOP to any message. <span class="text-red-500">*</span>
              </label>
            </div>
            <ErrorMessage name="consent" class="text-red-500 text-xs mt-1 block" />
          </div>

          <!-- Reminder Date Picker -->
          <div class="mb-6">
            <label
              for="reminderDate"
              class="block text-gray-700 text-sm font-bold mb-2 dark:text-gray-200"
            >
              Reminder Date <span class="text-red-500">*</span>
            </label>
            <Field name="reminderDate" v-slot="{ field }">
              <Datepicker
                v-bind="field"
                :model-value="field.value"
                @update:model-value="field.onChange"
                input-class-name="input-field"
                placeholder="Pick a date for the first reminder"
                :enable-time-picker="true"
                :is-24="false"
                format="MM/dd/yyyy hh:mm a"
                :teleport="true"
                :clearable="false"
                :auto-apply="false"
                :close-on-auto-apply="false"
                :week-start="0"
                :timezone="'America/Denver'"
                dark
                :dark-picker="true"
                class="dark:bg-gray-800 dark:text-gray-200"
              />
              <ErrorMessage name="reminderDate" class="text-red-500 text-xs mt-1 block" />
            </Field>
          </div>

          <!-- Is Recurring Toggle Switch -->
          <div class="mb-6">
            <label for="isRecurring" class="flex items-center">
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
                  class="w-11 h-6 bg-gray-300 peer-checked:bg-green-500 rounded-full relative transition-colors duration-300 cursor-pointer"
                >
                  <div
                    class="absolute left-1 top-1 bg-white w-4 h-4 rounded-full transition-transform duration-300 peer-checked:translate-x-full"
                  ></div>
                </div>
              </Field>
              <span class="ml-3 text-sm font-medium text-gray-700 dark:text-gray-200"
                >Recurring? (April - Nov)</span
              >
            </label>
          </div>

          <div class="flex flex-col gap-3">
            <button
              type="submit"
              class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline transition duration-150 ease-in-out dark:bg-purple-900"
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
  @apply shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight
  focus:outline-none focus:border-green-500 focus:ring-1 focus:ring-green-500
  dark:bg-gray-800 dark:border-gray-600 dark:text-gray-200
    dark:focus:border-green-500 dark:placeholder-gray-400;
}

input[type='checkbox'] {
  @apply h-4 w-4 rounded border-gray-300
    text-green-600
    focus:ring-green-500
    dark:border-gray-600
    dark:bg-gray-800
    dark:checked:bg-green-500
    dark:focus:ring-green-500;
}

:deep(.dp__theme_dark) {
  --dp-background-color: #1f2937;
  --dp-text-color: #e5e7eb;
  --dp-hover-color: #374151;
  --dp-hover-text-color: #e5e7eb;
  --dp-hover-icon-color: #e5e7eb;
  --dp-primary-color: #10b981;
  --dp-primary-text-color: #e5e7eb;
  --dp-secondary-color: #374151;
  --dp-border-color: #4b5563;
  --dp-menu-border-color: #4b5563;
  --dp-border-color-hover: #6b7280;
  --dp-disabled-color: #4b5563;
  --dp-scroll-bar-background: #1f2937;
  --dp-scroll-bar-color: #374151;
  --dp-success-color: #10b981;
  --dp-success-color-disabled: #1f2937;
  --dp-icon-color: #e5e7eb;
  --dp-danger-color: #ef4444;
}

:deep(.dp__input) {
  background-color: #1f2937;
  color: #e5e7eb;
  border-color: #4b5563;
}

:deep(.dp__input:hover) {
  border-color: #6b7280;
}

:deep(.dp__input_focus) {
  border-color: #10b981;
}
</style>
